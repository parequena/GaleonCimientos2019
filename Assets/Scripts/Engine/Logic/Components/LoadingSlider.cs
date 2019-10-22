using UnityEngine;
using System.Collections;
using UnityEngine.UI;


/// <summary>
/// componente que maneja un Slider de unity y lo utiliza como una barra de carga.
/// </summary>
public class LoadingSlider : MonoBehaviour
{
    public AsyncLoader asyncLoader;

    private SceneMgr m_gameMgr;
    private Slider m_slider;
    
    // Use this for initialization
    protected void Start () {

        //TODO 1: registrar el evento ProgressLoading en el AsyncLoader


        m_slider = GetComponent<Slider>();
        m_slider.value = 0f;
    }

    // Update is called once per frame
    protected void Update () {
	

	}

    protected void ProgressLoading(float progress, bool finish)
    {
        m_slider.value = progress;
        //TODO 2: deregistrar el evento cuando termina la carga (finish)

    }

    protected void OnDestroy()
    {
        //TODO 3: deregistrar el evento cuando se destruye el objeto. Comprobar que asyncLoader sea distinto de null por sanidad :)

    }
}
