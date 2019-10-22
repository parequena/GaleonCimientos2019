using UnityEngine;
using System.Collections;
/// <summary>
/// Input controller. Clase base para los InputController que se registra automaticamente en el InputMgr
/// </summary>
public class InputController : MonoBehaviour {

	// Use this for initialization
	protected virtual void Awake(){
        // TODO 1: Res registramos como input especifico para el juego
       
    }
	
	protected void Start () {
		
	}
	
	// Update is called once per frame
	protected virtual void Update () {
	}
	
	void OnDestroy() 
	{
		InputMgr inputMgr = GameMgr.GetInstance().GetServer<InputMgr>();
		if(inputMgr != null)
			inputMgr.ClearInput();
	}

}
