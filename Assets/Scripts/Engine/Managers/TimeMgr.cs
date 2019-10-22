using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//Delegado que nos permite definir la interfaz del metodo al que vamso a llamar cuando se nos establezca una alarma
//el delegado recibe como parametro el tiempo que ha trasncurrido y el objeto que previamente sed le habia pasado.
public delegate void AlarmDelegate(float time, object data);

/// <summary>
/// Time mgr. PErmite deifnir alarmas, tanto de un unico tiempo como periodicas.
/// </summary>
public class TimeMgr : MonoBehaviour
{
	
	protected TDataTimeMgr Contains(MonoBehaviour component, string name)
	{
		TDataTimeMgr result = null;
		for(int i = 0 ; result == null && i < m_alarms.Count; ++i)
		{
			TDataTimeMgr data = m_alarms[i];
			if(component.GetInstanceID() == data.m_component.GetInstanceID() && name == data.m_alarmName)
			{
				result = data;
			}
		}
		return result;
	}
	public void SetAlarm(MonoBehaviour component,string name, AlarmDelegate func, Object data, float time, bool periodic = false)
	{
		TDataTimeMgr alarm = null;
		if((alarm = Contains(component,name) ) == null)
		{
			TDataTimeMgr dataMgr = new TDataTimeMgr();
			dataMgr.m_component = component;
			dataMgr.m_alarmName = name;
			dataMgr.func = func;
			dataMgr.m_timeToExpired = time;
			dataMgr.m_time = time;
			dataMgr.periodic = periodic;
			dataMgr.data = data;
			m_alarms.Add(dataMgr);
		}
		else
		{
			//Cambio los valores de la alarma
			alarm.m_timeToExpired = time;
			alarm.m_time = time;
			alarm.func = func;
			alarm.periodic = periodic;
		}
	}
	
	public void CancelAlarm(MonoBehaviour component,string name)
	{
		TDataTimeMgr alarm = null;
		if((alarm = Contains(component,name ) ) != null)
		{
			m_alarms.Remove(alarm);
		}
	}
	
	public bool ExistAlarm(MonoBehaviour component,string name)
	{
		//TDataTimeMgr alarm = null;
		return Contains(component,name ) != null;
	}
	
	public float GetTimeToExpired(MonoBehaviour component,string name)
	{
		float time = -1;
		TDataTimeMgr alarm = null;
		if((alarm = Contains(component,name ) ) != null)
		{
			time = alarm.m_timeToExpired;
		}
		return time;
	}
	
	
	
	protected void Update()
	{
		for(int i = 0 ; i < m_alarms.Count; ++i)
		{
			TDataTimeMgr data = m_alarms[i];
			data.m_timeToExpired -= Time.deltaTime;
			if(data.m_timeToExpired <= 0f)
			{
				if(data.m_component != null)
				{
					data.func(data.m_time - data.m_timeToExpired,data.data);
					if(data.periodic)
					{
						data.m_timeToExpired = data.m_time;
					}
					else
					{
						m_alarms.Remove(data);
					}
				}
			}
		}
	}

	
	protected class TDataTimeMgr
	{
		public MonoBehaviour m_component;
		public string m_alarmName;
		public AlarmDelegate func;
		public float m_timeToExpired;
		public float m_time;
		public bool periodic;
		public object data;
	}
	
	List<TDataTimeMgr> m_alarms = new List<TDataTimeMgr>();

	
}
