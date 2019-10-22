using UnityEngine;
using System.Collections;

/// <summary>
/// Componente que gestiona el Ataque de una Entidad. 
/// </summary>
public class AttackComponent : MonoBehaviour
{
	
	public float m_timeToAttack;
	
	[System.Serializable]
	public class TWeapons
	{
		public string Weapon;
		public AnimatorOverrideController AnimationController;
		public float Damage; //daño por cada impacto. Si es automatica el daño indicado es por segundo...
		public float TimeBetweenAttacks; //Tiempo entre ataques. Si es  0 el tiempo dependera de la animacion
		//si es < 0, es un ataque automatico (tipo metralleta, la animacion se pleyea una vez y se espera que esta sea en loop)
		// si el tiempo es > 0 entonces es la cadencia de disparo. Se le suma al tiempo de la animacion.
		public float max_distance; //la distancia maxima a la que el arma es efectiva.
		public GameObject gameObject;
	}
	
	public TWeapons[] m_weapons;
	public int m_activeWeapon = 0;
	
	protected PlayerAnimation m_animationComponent;
	protected bool m_attack = false;
	protected bool m_attackBlocked = false;
	protected GameObject m_enemy;
	protected bool m_canAttack = false;
	// Use this for initialization
	protected void Update()
	{
		if(m_attack)
		{
			//Debug.Log(m_weapons[m_activeWeapon].AnimAttackName+" state "+state+" time "+time+" timeToFinish "+timeToFinish+" duration "+duration );
			//Sigo disparando
			if(m_weapons[m_activeWeapon].TimeBetweenAttacks < 0f)
			{
				//Ataque automatico, le sigo quitando vida...
				if(m_enemy != null)
					m_enemy.SendMessage("Damage",m_weapons[m_activeWeapon].Damage*Time.deltaTime,SendMessageOptions.DontRequireReceiver);
			}
		}
	}
	
	void SetAttackTarget(GameObject enemy)
	{
		//comprobamos que podemos atacar ocn el arma. Si no podemos atacar, nos moveremos hasta que la distancia sea la necesaria y luego atacaremos...
		
		if(!m_attackBlocked)
			m_enemy = enemy;
	}

    public void ToAttack(bool attack)
    {
        m_attack = attack;
        m_animationComponent.IsAttacking = attack;
    }
	
	public bool CheckCanAttack(GameObject enemy)
	{
		m_enemy = enemy;
		return CheckCanAttack();
	}
	
	public bool CheckCanAttack()
	{
		Vector3 direction = m_enemy.transform.position - transform.position;
		bool result = false;
		float sqrDistance = direction.sqrMagnitude;
		float max = m_weapons[m_activeWeapon].max_distance;
		max *= max;
		if(sqrDistance <= max)
		{
			result = true;
		}
		return result;
	}
	
	void AttackToTarget(GameObject enemy)
	{
		SetAttackTarget(enemy);
		TryToAttack();
	}
	
	void EnemyDeath()
	{
        ToAttack(false);
    }
	
	public void TryToAttack()
	{
		//Si esta atacando esperamso a que termine de atacar...
		if(!m_attackBlocked)
		{
			//Mando un mensaje al enemigo diciendole que le he hecho pupa.. (Nota, un arma automatica depende el daño del tiempo, lo hacemos en el tick)
			m_attackBlocked = true;
			if(m_weapons[m_activeWeapon].TimeBetweenAttacks >= 0f)
			{
				
				m_enemy.SendMessage("Damage",m_weapons[m_activeWeapon].Damage,SendMessageOptions.DontRequireReceiver);
			}
			else
			{
				//Si es un ataque automatico, entonces debemos liberar al player
				SendMessage("OnFinishAttackAnim",true);
				m_attackBlocked = false;
			}
            //Enciendo luz del arma
            //Pongo el sonido
            //Pongo la animacion
            ToAttack(true);
            //Hasta que la animacion no termine para el player el jugador esta bloqueado. 
            //Nota: Una excepcion podria ser si te disparan y cancelan la animacion. Pero ne principio el personaje no podra hacer ninguna accion
            //mientras ejecuta la animacio nde ataque.

            //pongo destellito
            //mando mensaje de FinishAttack
        }
		else
		{
			SendMessage("OnFinishAttackAnim",false);
		}
	}

	void OnAnimationFinish(string state)
	{
		//Cuando termina la animacion le decimos al controlador que ya puede decidir otra accion 
		// Si el disparo es automatico seguira disparando hasta que cambie la orden.
		//Nota: Recargar? por ahora no...

		if(m_weapons[m_activeWeapon].TimeBetweenAttacks < 0)
		{
			//Ataque automatico, sigo quitando daño pero dejo de estar bloqueado.
			m_attackBlocked = false;
            ToAttack(true);
        }
		else 
		{
            ToAttack(false);
            //Existe un tiempo adicional al de la animacion
            if (m_weapons[m_activeWeapon].TimeBetweenAttacks < 0.01)
			{
				//No tengo que esperar mas el ataque ha finalizado
				m_attackBlocked = false;
			}
			else
			{
				//No puedo volver a aceptar otro mensaje de ataque porque aun estoy bloqueado, pero ya no ataco (no es ataque automatico)
				m_attackBlocked = true;

                GameMgr.GetInstance().GetServer<TimeMgr>().SetAlarm(this, "WeaponTimeWaiting", OnWaeponTimeFinish, null, m_weapons[m_activeWeapon].TimeBetweenAttacks, false);

            }
        }
		SendMessage("OnFinishAttackAnim",m_attack);
	}
	
	protected void OnWaeponTimeFinish(float time, object data)
	{
		Debug.Log("OnWaeponTimeFinish time "+time);
		m_attackBlocked = false;
	}
	
	
	protected void Start()
	{
		m_animationComponent = GetComponent<PlayerAnimation>();
        Debug.Assert(m_animationComponent != null,"Error: el componente animacion no existe");
        m_animationComponent.RegisterOnAnimationEvent=OnAnimationFinish;

        SendMessage("OnChangeWeapon", m_activeWeapon,SendMessageOptions.DontRequireReceiver);

        // TODO 1 : Registramos RegisterChangeWeapon en el input controller del inputmgr con el event ChangeWeapon



        foreach (TWeapons wp in m_weapons)
		{
			wp.gameObject.SetActive(false);
		}
		m_weapons[m_activeWeapon].gameObject.SetActive(true);
        m_animationComponent.ChangeOverrideAnimatorController(m_weapons[m_activeWeapon].AnimationController);
    }
	
	
	public void ChangeWeapon(int index)
	{
        Debug.Assert(index >= 0 && index < m_weapons.Length,"Error, la nueva entrada de arma no existe en el inventario... "+index);
		m_weapons[m_activeWeapon].gameObject.SetActive(false);
		m_activeWeapon = index;
		SendMessage("OnChangeWeapon", m_activeWeapon,SendMessageOptions.DontRequireReceiver);
		m_weapons[m_activeWeapon].gameObject.SetActive(true);
        m_animationComponent.ChangeOverrideAnimatorController(m_weapons[m_activeWeapon].AnimationController);
	}
}
