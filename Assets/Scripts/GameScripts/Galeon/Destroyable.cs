using UnityEngine;
using System.Collections;

/// <summary>
/// Un GameObject Es destruible si tiene el Componente Destroyable y escucha el mensaje DestroyGameObject.
/// </summary>
public class Destroyable : MonoBehaviour
{
 
	public bool m_desactive = true;

	//Mensaje
	void DestroyGameObject()
	{
        //TODO 1 destruir usando el SpawnerMgr.
    }
 
}
