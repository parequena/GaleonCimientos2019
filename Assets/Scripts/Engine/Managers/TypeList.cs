using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//Clase que almacena los tipo de datos permitidos por el storageMng.
public class TypeList 
{
	public TypeList(params System.Type[] typePermited)
	{
		m_types.Clear();
		foreach(System.Type type in typePermited)
		{
			m_types.Add(type);
		}
	}
	
	public bool AvailableDataTypes(System.Type type)
	{
		bool ret = false;
		for( int i = 0; !ret && i < m_types.Count; ++i)
		{
			if(m_types[i] == type)
			{
				ret = true;
			}
		}
		return ret;
	}
	
	protected List<System.Type> m_types = new List<System.Type>();
}
