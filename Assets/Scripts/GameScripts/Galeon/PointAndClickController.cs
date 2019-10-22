using UnityEngine;
using System.Collections;

/// <summary>
/// Implementa un control del player usando Point And Click
/// </summary>
[RequireComponent (typeof (AttackComponent))]
public class PointAndClickController : MonoBehaviour
{
	
	
	public float m_angleToInstantRotate = 15f;
	public float m_distanceToStop = 0.5f;
	public float m_initialSpeed = 1.0f;
	public float m_accel = 1.0f;
	public float m_gravity = 1.0f;
	public float m_rotationSpeed = 1.0f;
	public string[] m_excludeTagsToGroundObject;
	public float m_maxGroundSpeed = 20f;
	public float m_maxAirSpeed = 20f;
	public string[] m_enemiesTags;
	public Light m_lantern;
	
	private InputMgr m_inputMgr;
	private Vector3 m_targetPoint;
	private Vector3 m_move = new Vector3(0f,0f,0f);
	private float m_rotationAngleY;
	private CharacterController m_characterController;
	private float m_rightInstantAngle;
	private float m_leftInstantAngle;
	private Vector3 m_rotationVector3  = new Vector3(0f,0f,0f);
	private bool m_isGrouned;
	private bool m_preparingToAttack = false;
	private float m_speed = 0f;
	private float m_gravityVelocity = 0f;
	private PlayerAnimation m_animationComponent;
	private AttackComponent m_attackComponent;
	
	private enum TState {ATTACK_BLOCK, ATTACK_NO_BLOCK, GO_TO_ATTACK, MOVE, STOP};
	private TState m_state;
	private float sensorCoeficient = 0.35f;
	
	protected void Start()
	{
		m_inputMgr = GameMgr.GetInstance().GetServer<InputMgr>();
        Debug.Assert(m_inputMgr != null, "Error, no se ha cargado el inputMgr");
        //TODO 1 registrarnos en RegisterPointAndClickEvent con (OnBeginClick, null, OnBeginClick);
        
        m_speed = m_initialSpeed;
		m_rightInstantAngle = m_angleToInstantRotate;
		m_leftInstantAngle = 365 - m_angleToInstantRotate;
		m_targetPoint = transform.position;
		m_characterController = GetComponent<CharacterController>();
        Debug.Assert(m_characterController != null,"Error, debe existir un character controller para poder utilizar FpsController");
		m_state = TState.STOP;
		m_animationComponent = GetComponent<PlayerAnimation>();
        Debug.Assert(m_animationComponent != null, "Error, el componente PlayerAnimation debe estar asignado en el GameObject");
		m_attackComponent = GetComponent<AttackComponent>();
        Debug.Assert(m_attackComponent != null,"Error, el componente AttackComponent debe estar asignado en el GameObject");
		
	}
	
	protected void OnDestroy ()
	{
		m_inputMgr.UnRegisterPointAndClickEvent(OnBeginClick,null,OnBeginClick);
	}
	
	public void OnClicked(GameObject onCollision,Vector3 point, float distance)
    {

    }
	
	
	public void OnBeginClick(GameObject onCollision,Vector3 point, float distance)
	{
		if(Utils.IsInArray(onCollision.tag,m_enemiesTags))
		{
			if(m_state != TState.ATTACK_BLOCK)
			{
				AttackToTarget(onCollision);
				GoToTarger(point);
			}
		}
		else
		{
			if(m_state != TState.ATTACK_BLOCK)
			{
				GoToTarger(point);
			}
		}
	}
	
	protected void AttackToTarget(GameObject onCollision)
	{
		if(m_attackComponent.CheckCanAttack(onCollision))
		{
			m_state = TState.ATTACK_BLOCK;
			m_preparingToAttack = true;
		}
		else
		{
			m_state = TState.GO_TO_ATTACK;
		}
	}
	
	protected void ThrowAttack()
	{
        m_attackComponent.TryToAttack();
		m_preparingToAttack = false;
	}
	
	void OnFinishAttackAnim(bool continuesToAttack)
	{
		m_targetPoint = transform.position;
		m_state = continuesToAttack ? TState.ATTACK_NO_BLOCK : TState.STOP;   
    }
	
	protected void GoToTarger(Vector3 point)
	{
		Vector3 pointToGo = point;
		pointToGo.y = transform.position.y;
		Vector3 direction = pointToGo - transform.position;
		m_targetPoint = pointToGo;
		m_targetPoint.y = transform.position.y; //asumimos por ahora que no sube rampas.
		Quaternion targetRotation = Quaternion.LookRotation(direction);		
		m_rotationAngleY = (targetRotation.eulerAngles.y - transform.rotation.eulerAngles.y);
		m_rotationAngleY = m_rotationAngleY  < 0 ? 365.0f +m_rotationAngleY: m_rotationAngleY;
		if(m_rotationAngleY < m_rightInstantAngle || m_rotationAngleY > m_leftInstantAngle)
		{
			targetRotation.x = 0f;
			targetRotation.z = 0f;
			transform.rotation = targetRotation;
			m_rotationAngleY = 0;
		}
	}
	
	
	void FixedUpdate()
	{
        bool lightActive = GameMgr.GetInstance().GetStorageMgr().Get<bool>("settings", "light_active"); 
		m_lantern.gameObject.SetActive(lightActive);
		if(m_state != TState.ATTACK_BLOCK)
		{
            if (m_state == TState.MOVE)
                m_attackComponent.ToAttack(false);
            m_isGrouned = IsGrounded();
			m_targetPoint.y = transform.position.y;
			Debug.DrawLine(transform.position, transform.position+transform.forward*2f,Color.red);
			Vector3 direction = m_targetPoint - transform.position;
			float sqrDistance = direction.sqrMagnitude;
			Rotation(Time.fixedDeltaTime);
			m_move  = Vector3.zero;
			if(sqrDistance > m_distanceToStop*m_distanceToStop)
			{
				direction = direction.normalized;
				float distanceInc = Trajectories.UniformAccelerateMovement(m_speed,m_accel,Time.fixedDeltaTime);
				if(m_speed < m_maxGroundSpeed)
					m_speed = Trajectories.UniformAccelerateVelocity(m_speed,m_accel,Time.fixedDeltaTime);
				direction*=distanceInc;
                m_animationComponent.IsRunning = true;
                m_animationComponent.IsWalking = true;

                m_move.x = direction.x;
				m_move.z = direction.z;
				m_state = m_state == TState.GO_TO_ATTACK ? m_state: TState.MOVE;
			}
			else
			{
				if(m_state != TState.ATTACK_NO_BLOCK)
				{
                    m_animationComponent.IsRunning = false;
                    m_animationComponent.IsWalking = false;
                    m_speed = m_initialSpeed;
				}
			}
			
			if(!m_isGrouned)
			{
				float distanceGravityInc = Trajectories.UniformAccelerateMovement(m_gravityVelocity,m_gravity,Time.fixedDeltaTime);
				if(m_gravityVelocity < m_maxAirSpeed)
					m_gravityVelocity = Trajectories.UniformAccelerateVelocity(m_gravityVelocity,m_gravity,Time.fixedDeltaTime);
				m_move.y = -distanceGravityInc;
				m_state = TState.MOVE;
			}
			else
			{
				m_gravityVelocity = 0f;
			}
			
			if(m_state == TState.MOVE || m_state == TState.GO_TO_ATTACK)
				m_characterController.Move(m_move);
			
			if(m_state == TState.GO_TO_ATTACK)
			{
                if (m_attackComponent.CheckCanAttack())
                {
                    m_state = TState.ATTACK_BLOCK;
                    m_preparingToAttack = true;
                    ThrowAttack();
                }
                else
                    m_attackComponent.ToAttack(false);
			}
		}
		else
		{
			if(m_preparingToAttack)
			{
				//orientamos el personaje para atacar...
				if(Rotation(Time.fixedDeltaTime))
				{
					ThrowAttack();
				}
			}
		}
	}
	
	
	private bool Rotation(float deltaTime)
	{
		bool result = false;
		transform.rotation = Quaternion.Euler(0.0f,transform.rotation.eulerAngles.y,0.0f);
		if(m_rotationAngleY > 0.1f )
		{
			float rotation = deltaTime * m_rotationSpeed;
			
			m_rotationVector3.x = 0;
			m_rotationVector3.z = 0;
			if(m_rotationAngleY < 180)
			{
				//roto a la derecha
				m_rotationVector3.y = m_rotationAngleY > rotation ? rotation : m_rotationAngleY;
				if (m_rotationAngleY < 5)
					m_rotationAngleY = 0.0f;
				else
					m_rotationAngleY -= rotation;
			}
			else
			{
				//roto a la izquierda
				m_rotationVector3.y = (m_rotationAngleY > (365 - rotation)) ? 365 - m_rotationAngleY : -rotation;
				if (m_rotationAngleY > 360)
					m_rotationAngleY = 0.0f;
				else
					m_rotationAngleY += rotation;
			}
	
			transform.Rotate(m_rotationVector3);
		}
		else
		{
			result = true;
		}
		return result;
	}
	
	
	protected bool IsGrounded()
	{
		bool result = false;
		Ray ray = new Ray(this.transform.position + Vector3.forward*m_characterController.radius*this.transform.localScale.x,Vector3.down);
		Debug.DrawRay(this.transform.position + Vector3.forward*m_characterController.radius*this.transform.localScale.x,Vector3.down*sensorCoeficient,Color.white);
		RaycastHit hit;
		if(Physics.Raycast(ray, out hit, sensorCoeficient))
		{
			result = !Utils.IsInArray(hit.collider.gameObject.tag,m_excludeTagsToGroundObject);
		}
		
		if(!result)
		{
			ray = new Ray(this.transform.position - Vector3.forward*m_characterController.radius*this.transform.localScale.x,Vector3.down);
			Debug.DrawRay(this.transform.position - Vector3.forward*m_characterController.radius*this.transform.localScale.x,Vector3.down*sensorCoeficient,Color.white);
			if(Physics.Raycast(ray, out hit, sensorCoeficient))
			{
				result = !Utils.IsInArray(hit.collider.gameObject.tag,m_excludeTagsToGroundObject); 
			}
		}
		return result;
	}

}
