using UnityEngine;
using System.Collections;

public class TopDownCamera : MonoBehaviour
{
	
	
	public GameObject m_target;
	public float m_height = 10f;
	public float m_speed = 2f;
	
	private Vector3 m_vector3 = new Vector3(0f,0f,0f);
	
	
	
	protected void Update()
	{
		float verticalDistanceToTarget = Mathf.Abs(m_target.transform.position.y - this.transform.position.y);
		Vector3 direction = m_target.transform.position-this.transform.position;
		//float aSquare = direction.sqrMagnitude - (verticalDistanceToTarget*verticalDistanceToTarget);
		
		float angle = Vector3.Angle(transform.forward,Vector3.down);
		angle *= Mathf.Deg2Rad;
		float horizontalDistance =  verticalDistanceToTarget * (Mathf.Sin(angle) / Mathf.Cos(angle));
		Vector3 groundProyectation = transform.position;
		groundProyectation.y = m_target.transform.position.y;
		
		Debug.DrawRay(transform.position,transform.forward*direction.magnitude,Color.green);
		Debug.DrawRay(transform.position,Vector3.down*verticalDistanceToTarget,Color.blue);
		
		
		Vector3 groundProyectationForward = transform.forward;
		groundProyectationForward. y = 0f;
		Debug.DrawRay(groundProyectation,groundProyectationForward.normalized*horizontalDistance,Color.black);
		
		//Vector3 pointTarget  = new Vector3(x,transform.position.y,z-horizontalDistance);
		//direction = pointTarget - groundProyectation;
		
		direction.y = 0f;
		direction.x = m_target.transform.position.x - groundProyectation.x;
		direction.z = m_target.transform.position.z - groundProyectation.z-horizontalDistance;
		if(direction.sqrMagnitude > 0.1f)
		{
			direction = direction.normalized;
			
		
			transform.position = transform.position + direction*m_speed*Time.deltaTime;
			
		}
		/*else if(direction.sqrMagnitude < horizontalDistance*horizontalDistance)
		{
			direction = direction.normalized;
			
		
			transform.position = transform.position - direction*2f*Time.deltaTime;
			
		}*/
		m_vector3.x = transform.position.x;
		m_vector3.y = m_height;
		m_vector3.z = transform.position.z;
		transform.position = m_vector3;
	}
	
}
