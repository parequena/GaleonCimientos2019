using UnityEngine;
using System.Collections;


/// <summary>
/// Gestiona el cambio de escena desde la UI de Unity (sólo disponible a partir de la 4.6)
/// </summary>
public class ChangeSceneUI : MonoBehaviour {

    public string m_nextSceneAfterTransitionScene;
    public bool m_return;


    /// <summary>
    /// El método es llamado por el sistema de eventos de la UI y se le pasa por parámetro la escena que quiere cargar. Adicionalmente peude tener establecida como variable pública
    /// en el inspector una siguiente escena a cargar, en caso de que la escena de carga sea una pantalla de carga.
    /// </summary>
    /// <param name="sceneToGo"></param>
    public void OnChangeScene(string sceneToGo)
    {
        //TODO 1 cambiamos de escena o hacemos desapilamos la escena en función de los que indique el inspector.
        if (m_return)
            Debug.LogError("Desapilando la escena en la cima de la pila");
        else
            Debug.LogError("Cambiando de escena a "+ sceneToGo);
    }

    /// <summary>
    /// El método es llamado por el sistema de eventos de la UI para cerrar la aplciación.
    /// </summary>
    /// <param name="quit"></param>
    public void OnQuitScene(bool quit)
    {
        // TODO 2: Aplication Quit.
        if (quit)
            Application.Quit();

    }
}
