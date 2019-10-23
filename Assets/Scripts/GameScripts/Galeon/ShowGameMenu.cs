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
        GameMgr.GetInstance().GetServer<InputMgr>().RegisterReturn(OnReturnPressed);
    }
	
	protected void OnReturnPressed()
	{
        //muestro el menu..
        //TODO 2 if type == TPushSceneType.PUSH_SCENE => PushScene else ReturnScene
        if (m_type == TPushSceneType.PUSH_SCENE)
        {
            Debug.LogError("apilamos la escena de menu ");
            GameMgr.GetInstance().GetServer<SceneMgr>().PushScene(m_menuSceneName);
        }
        else
        {
            Debug.LogError("Desapilamos la escena en la cima de la pila");
            GameMgr.GetInstance().GetServer<SceneMgr>().PopScene(true);
        }

    }

    protected void OnDestroy() 
	{
        //TODO 3 desregistrar el return.
        InputMgr input = GameMgr.GetInstance().GetServer<InputMgr>();
        if (input != null)
            Debug.LogError("desregistramos");

        GameMgr.GetInstance().GetServer<InputMgr>().UnRegisterReturn(OnReturnPressed);

    }

    /*protected virtual void Tick(float deltaTime){}
	protected virtual void Init(){}
	
	protected virtual void End() {}*/
}
