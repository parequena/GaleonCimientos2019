using UnityEngine;
using System.Collections;

public class ShowGameMenu : MonoBehaviour
{
	
	public string m_menuSceneName;
	public enum TPushSceneType { PUSH_SCENE, POP_SCENE};
	public TPushSceneType m_type = TPushSceneType.PUSH_SCENE;
	public bool m_clearReturnScene = false;
	
	protected void Awake()
	{
        //TODO 1: nos registras en el input al boton return con OnReturnPressed.
    }
	
	protected void OnReturnPressed()
	{
        //muestro el menu..
        //TODO 2 if type == TPushSceneType.PUSH_SCENE => PushScene else ReturnScene
        if (m_type == TPushSceneType.PUSH_SCENE)
            Debug.LogError("apilamos la escena de menu ");
        else
            Debug.LogError("Desapilamos la escena en la cima de la pila");

    }

    protected void OnDestroy() 
	{
        //TODO 3 desregistrar el return.
        InputMgr input = GameMgr.GetInstance().GetServer<InputMgr>();
        if (input != null)
            Debug.LogError("desregistramos");

    }

    /*protected virtual void Tick(float deltaTime){}
	protected virtual void Init(){}
	
	protected virtual void End() {}*/
}
