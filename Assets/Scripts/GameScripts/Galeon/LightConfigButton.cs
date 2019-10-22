using UnityEngine;
using System.Collections;
using UnityEngine.UI;



/// <summary>
/// Guarda la configuración de la luz en el almacenamiento persistente
/// </summary>
public class LightConfigButton : MonoBehaviour
{
 
 
	public Text m_text;
 	// Call when component is create (only once)
 	/*protected override void Awake() {
 		base.Awake();
 	}*/
 
 	// Use this for initialization
 	protected void Start() {
		if(!GameMgr.GetInstance().GetStorageMgr().ExistFile())
		{
			GameMgr.GetInstance().GetStorageMgr().Store();
		}

        GameMgr.GetInstance().GetStorageMgr().Load();
        if (!GameMgr.GetInstance().GetStorageMgr().Contains("settings", "light_active"))
        {
            GameMgr.GetInstance().GetStorageMgr().Set("settings", "light_active", false);
            GameMgr.GetInstance().GetStorageMgr().Store();
        }

        GameMgr.GetInstance().GetStorageMgr().Store();
		bool lightActive = GameMgr.GetInstance().GetStorageMgr().Get<bool>("settings","light_active");

        if (lightActive)
        {
            m_text.text = "Light On";
        }
        else
        {
            m_text.text = "Light Off";
        }
    }


    public void OnButtonBeginPressed()
	{
        bool lightActive = GameMgr.GetInstance().GetStorageMgr().Get<bool>("settings", "light_active");
        lightActive = !lightActive;
        GameMgr.GetInstance().GetStorageMgr().Set("settings", "light_active", lightActive);
        GameMgr.GetInstance().GetStorageMgr().Store();

        if (lightActive)
        {
            m_text.text = "Light On";
        }
        else
        {
            m_text.text = "Light Off";
        }
    }
}
