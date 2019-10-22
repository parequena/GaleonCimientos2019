using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Input mgr. Gestion del input generica. Inplementa ciertas funcionalidades que asumimos tendran todos los juegos como el caso de volver para atras
/// lanzar un rallo desde la camara para implementar botones en 3d (la GUI de Unity es un poco tediosa de utilizar)
/// </summary>
public class InputMgr : MonoBehaviour
{
    public enum TMouseButtonID { LEFT = 0, MIDDLE, RIGHT };
	//Eventos a los que quiero avisar. El raton o cualquier dispositivo de puntero a presionado sobre alguna parte de la pantalla.
	public delegate void PointAndClickEvent(GameObject onCollision,Vector3 point, float distance);


    //Enumerado de diferentes eventos de un boton.
    protected enum TButtonEvent { BEGIN, PRESSED, END, BEGIN_OVER, END_OVER };
	private Vector3 m_axis = new Vector3(0.0f,0.0f,0.0f);
	
	
	//configuramos el InputMgr con el boton de pointAndClick del raton (0 normalmente) y si queremos activar o no
	//el point and click (por si no lo necesitaramos....)
    public void Configure(TMouseButtonID pointAndClickButton, bool pointAndClickActive = false)
	{
		m_pointAndClickButton = pointAndClickButton;
		m_pointAndClickActive = pointAndClickActive;
	}
	
	//registramos al evento Return
	public void RegisterReturn(System.Action ret)
	{
        //TODO 1 registramos el evento return
        m_returnDelegate += ret;

    }
    //nos desregistramos al evento Return
    public void UnRegisterReturn(System.Action ret)
	{
        //TODO 2 desregistramos el evento return
        m_returnDelegate -= ret;
    }

	//llamamos al evento Return.
	public void ReturnCallback()
	{
        if (m_returnDelegate != null)
            m_returnDelegate();
	}
	//registramos un boton. Tenemos dos posibles eventos. pulsado y fin de pulsado.
	
	//Registramos los eventos de pointAndClick. Begin es cuando se inicia el pulsado. End cuando termina. pressed mientras se esta presionando
	public void RegisterPointAndClickEvent(PointAndClickEvent begin,PointAndClickEvent end, PointAndClickEvent pressed)
	{
		if(begin != null)
		{
			m_pcBegin += begin;
		}
		
		if (end != null)
		{
			m_pcEnd += end;
		}
		
		if(pressed != null)
		{
			m_pcPressed += pressed;
		}
	}
	//Desregistro de point and click.
	public void UnRegisterPointAndClickEvent(PointAndClickEvent begin,PointAndClickEvent end, PointAndClickEvent pressed)
	{
		if(begin != null)
		{
			m_pcBegin -= begin;
		}
		
		if (end != null)
		{
			m_pcEnd -= end;
		}
		
		if(pressed != null)
		{
			m_pcPressed -= pressed;
		}
	}
	
	//Activar o desactivar el pointAndclick.
	//Nota: una ventaja de tener centralizado todo el manejo es que podemos desactivar el point and click de una 
	//forma rapida y eficaz.
	public void ActivePointAndClick(bool active)
	{
		m_pointAndClickActive = active;
	}
	
	//Obtiene el inputController definido.
	public T GetInput<T>() where T : InputController
	{
		return (T) m_currentController;
	}
	
	//Cambia le input controller
	public void SetInput(InputController input)
	{
		m_currentController = input;
	}
	//Pone el inputController a NULL.
	public void ClearInput()
	{
		m_currentController = null;
	}
	
	//Comprueba si existe algun InputController asignado.
	public bool IsSetAnyInput()
	{
		return m_currentController != null;
	}
	
	// Use this for initialization
	protected void Update()
	{
		OnReturn();
		//PointAndClick()
		if(m_pointAndClickActive)
			OnClick();
	}
	
	//Comprobamos si se ha pulsado el Return.
	protected void OnReturn()
	{
		#if UNITY_IPHONE || UNITY_ANDROID
			//TODO
			if(Input.GetKey(KeyCode.Escape))
			{
				ReturnCallback();
			}
		#else
		try
		{
			if(Input.GetButtonDown("Return"))
			{
				ReturnCallback();
			}
		} catch (UnityException)
		{
			if(!m_alertShow)
			{
				m_alertShow = true;
				Debug.LogWarning("El boton de return no ha sido establecido");
			}
		}
		#endif
	}
	
	//On click de moviles.
	protected void OnTouch()
	{
		
		if(m_pressed && Input.touchCount == 0)
		{
			if(m_pcEnd != null)
				m_pcEnd(m_objectTouch,m_targetPoint,m_distanceTotouch);
            m_pressed = false;
		}
		
		if(Input.touchCount > 0)
		{
			Touch touch = Input.touches[0];
			m_axis.x = touch.position.x;
			m_axis.y = touch.position.y;
			if(!m_pressed)
			{
				m_pressed = true;
                CheckTouch(m_axis, true);	
			}
			else
                CheckTouch(m_axis, false);	
		}

	}
	
	
	//Onclick puede ser producido de muchas formas dependiendo de la plataforma.
	// 
	protected void OnClick()
	{
		#if UNITY_IPHONE || UNITY_ANDROID
			//TODO
			OnTouch();
		#else
        Vector3 mousePosition = Input.mousePosition;
		if(Input.GetMouseButtonDown((int)m_pointAndClickButton))
		{
			//begin
            CheckTouch(mousePosition,true);
			m_pressed = true;
		}
        else if (Input.GetMouseButton((int)m_pointAndClickButton))
		{
			//pressed
            CheckTouch(mousePosition,false);
			m_pressed = true;
		}
		else if(m_pressed)
		{
			//end
            if (m_pcEnd != null)
            {
                m_pcEnd(m_objectTouch, m_targetPoint, m_distanceTotouch);
            }
			m_pressed = false;
		}
		#endif
	}

    public GameObject CheckOver(Vector3 mousePosition)
    {
        GameObject button = null;
        bool collision = false ;
        Camera[] cameras = Camera.allCameras;
        for (int i = cameras.Length - 1; !collision && i >= 0; --i)
        {
            Ray ray = cameras[i].ScreenPointToRay(mousePosition);

            RaycastHit hit;

            collision = Physics.Raycast(ray, out hit); 
            if(collision)
            {
                button = hit.collider.gameObject;
            }
        }
        return button;
    }

	protected void CheckTouch(Vector3 mousePosition, bool begin)
	{
        bool isCollision = false;
        //Lanzamos un rayo desde todas las cámaras en orden de profundidad de la cámara... De esta forma podemos hacer point and click desde cualquier camara
        // ¿Porque? podemos tener en una cámara la GUI y en otra el pintado del juego y mezclarlas al final.
        Camera[] cameras = Camera.allCameras;
        for(int i = cameras.Length -  1; !isCollision && i >= 0; --i)
        {
            isCollision = ThrowRay(cameras[i],mousePosition, begin);
            if(isCollision)
                m_cameraUsedToTouch = null;
        }
		
	}

    public Camera GetCameraUsedToTouch() { return m_cameraUsedToTouch; }
	//Lanzamos un rayo desde las camaras hasta la posicion y le indicamos
	//si ese rallo se ha producido al iniciar el Click/touch o no.
    protected bool ThrowRay(Camera camera, Vector3 position, bool begin)
	{
        bool eventTrow = false;
        //TODO 3 lanzamos el rayo.
        Ray ray = camera.ScreenPointToRay(position);
        RaycastHit hit;
        bool collision = Physics.Raycast(ray, out hit);
        // RaycastHit hit = default(RaycastHit);
        //bool collision = false;
		if (collision)
		{
			m_targetPoint = hit.point;
			m_objectTouch = hit.collider.gameObject;
			m_distanceTotouch = hit.distance;
			if(begin)
			{
                if (m_pcBegin != null)
                {
                    m_pcBegin(m_objectTouch, m_targetPoint, m_distanceTotouch);
                    eventTrow = true;
                }

            }
            else
			{
                if (m_pcPressed != null)
                {
                    m_pcPressed(m_objectTouch, m_targetPoint, m_distanceTotouch);
                    eventTrow = true;
                }

            }
        }
        return eventTrow;
	}
	
	protected void Start()
	{
		DontDestroyOnLoad(this);
	}

   

	
	private InputController m_currentController = null;
    protected System.Action m_returnDelegate;
    //Eventos de Point And click
	protected PointAndClickEvent m_pcBegin;
	protected PointAndClickEvent m_pcEnd;
	protected PointAndClickEvent m_pcPressed;
	//Eventos de boton pulsado y soltado.
	
	private Vector3 m_targetPoint;

	private float m_distanceTotouch;
	
	private bool m_pressed = false;
	private bool m_alertShow = false;

    private GameObject m_objectTouch;

    private Camera m_cameraUsedToTouch = null;
	
	protected bool m_pointAndClickActive = false;
    protected TMouseButtonID m_pointAndClickButton = TMouseButtonID.LEFT;
}
