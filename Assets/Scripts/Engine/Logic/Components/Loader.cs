using UnityEngine;
using System.Collections;

//Carga la siguiente pantalla de forma estática. Dispone de un retardo para simular la arga si la transicion es demaisado corta...
public class Loader : MonoBehaviour
{

	// Use this for initialization
	
	// Update is called once per frame
	protected void Update()
	{
		m_time -= Time.deltaTime;
		if(m_time <= 0.0f)
		{
			//Carga la escena scene que previamente habiamos dejado en next_scene en la pizarra del StorageMng
			string scene = GameMgr.GetInstance().GetStorageMgr().GetVolatile<string>(SceneMgr.SCENE_SECTION,SceneMgr.NEXT_SCENE);
			GameMgr.GetInstance().GetServer<SceneMgr>().ChangeScene(scene);
		}
	}
	
	public float m_time = 1.0f;
}
