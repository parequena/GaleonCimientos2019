using UnityEngine;
using System.Collections;
/// <summary>
/// Componente que da vida a un NPC o un juegador. Cuando la vida lanza el mensaje Death que debe ser escuchado por algun Componenete.
/// </summary>
public class LifeComponent : MonoBehaviour
{
 
 	public float m_lifeMax;
	private float m_life;

	void OnEnable()
	{
		m_life = m_lifeMax;
	}
 
	void Damage(float damage)
	{
		m_life -= damage;
		if(m_life <= 0f)
		{
			SendMessage("Death");
		}
        
	}
	
	void AddLife(float newLife)
	{
		m_life += newLife;
		m_life = Mathf.Min(m_life,m_lifeMax);
	}
	
	void ResetLife(float newLife)
	{
		m_life = m_lifeMax;
	}

}
