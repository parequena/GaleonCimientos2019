using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// Trajectories. Algunos algoritmos para trajectorias. Bezier, velocidad uniformemente acelerada, tiro parabolico, etc.
/// </summary>
public class Trajectories 
{
	
	public static Vector2 Bezier(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float time, float timeMax)
	{
		
		//float step = time / timeMax;
		float t = time / timeMax;

		float _1_t = 1 - t;
		float _1_t_exp2 = _1_t*_1_t;
		float _1_t_exp3 = _1_t_exp2 * _1_t;
		float t_exp2 = t * t;
		float t_exp3 = t_exp2 * t;
		Vector2 point = _1_t_exp3 * p0 + 3* _1_t_exp2 * t * p1 + 3 * _1_t * t_exp2 * p2 + t_exp3 * p3;
		return point;
	}

	public static Vector2 Bezier(Vector2 p0, Vector2 p1, Vector2 p2, float time, float TimeMax)
	{
		float t = time / TimeMax;

		float _1_t = 1 - t;
		float _1_t_exp2 = _1_t*_1_t;
		float t_exp2 = t * t;
		Vector2 point = _1_t_exp2 * p0 + 2 * _1_t * t * p1 +  t_exp2 * p2 ;
		return point;
	}
	
	
	public static float UniformAccelerateVelocity(float v0, float a, float deltaT)
	{
		return v0 + a*deltaT;
	}
	
	public static float UniformAccelerateMovement(float v0, float a, float deltaT)
	{
		return v0*deltaT + 0.5f*a*deltaT*deltaT;
	}
	
	public static Vector2 ParabolicShoot(float v0x, float v0y, float angle, float g, float deltaT)
	{
		Vector2 increment = new Vector2();
		
		increment.x = v0x*Mathf.Cos(Mathf.Deg2Rad*angle)*deltaT;
		increment.y = v0y*Mathf.Sin(Mathf.Deg2Rad*angle)*deltaT - 0.5f*g*deltaT*deltaT;
		return increment;
	}
	
	public static float ParabolicShootMaxDistance(float v0, float angle, float g)
	{
		return (v0*v0 * 2f*Mathf.Sin(Mathf.Deg2Rad*angle)*Mathf.Cos(Mathf.Deg2Rad*angle)) / g;
	}
	
	public static float ParabolicShootTimeInMovement(float v0, float angle, float g)
	{
		return ( 2f*v0 * Mathf.Sin(Mathf.Deg2Rad*angle) ) / g;
	}
	
	public static float ParabolicShootMaxHeight(float v0, float angle, float g)
	{
		float sin = Mathf.Sin(Mathf.Deg2Rad*angle);
		return (v0*v0 *sin*sin) / (2f*g);
	}
	
}



