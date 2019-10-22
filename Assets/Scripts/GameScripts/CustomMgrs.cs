using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Clase que se puede usar para crear los Managers especificos de neustro juego
/// Tiene acceso al GameMgr para poder registrar 
/// </summary>
public class CustomMgrs : GameMgr.ProjectSpecificMgrs
{
    public CustomMgrs(GameMgr gameMgr) : base(gameMgr)
    {
        m_configurableMgr = new Dictionary<System.Type, Component>();
        //ejemplo para añadir un server que no sea MonoBehaviour.
        //Ejemplo para añadir un server de unity MonoBehaviour.
        //this.AddServerInGameMgr<MyComponent>();
    }


    public void Register(Component c)
    {
        if (!m_configurableMgr.ContainsKey(c.GetType()))
        {
            m_configurableMgr.Add(c.GetType(), c);
        }
    }

    public void UnRegister<T>() where T : MonoBehaviour
    {
        m_configurableMgr.Remove(typeof(T));
    }

    public T GetServer<T>() where T : MonoBehaviour
    {
        return (T)m_configurableMgr[typeof(T)];
    }


    public Dictionary<System.Type, Component> m_configurableMgr;

}


