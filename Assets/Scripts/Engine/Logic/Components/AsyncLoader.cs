using UnityEngine;
using System.Collections;

/// <summary>
/// Componente que permite cargar el siguiente nivel desde una pantalla de carga dinámica de forma asíncrona. Se presupone que el nivel que ha puesto la pantalla de carga debe
/// haber establecido previamente la variable SCENE_SECTION/NEXT_SCENE con el nombre de la escena a cargar.
/// </summary>
public class AsyncLoader : MonoBehaviour
{
    private bool loadingInit = false;
    private SceneMgr.OnAsyncLoadingProgress onAsyncLoadingProgress;
    protected void Update () {

        if (!loadingInit)
        {
            loadingInit = true;
            
            Load();
        }
    }
    
    protected void Load()
    {
        
        string sceneToLoad = GameMgr.GetInstance().GetStorageMgr().GetVolatile<string>(SceneMgr.SCENE_SECTION, SceneMgr.NEXT_SCENE);
        SceneMgr sceneMgr = GameMgr.GetInstance().GetServer<SceneMgr>();
        // TODO 1: hacer carga asincrona ChangeAsyncScene de la escena guardada en SceneMgr.SCENE_SECTION y SceneMgr.NEXT_SCENE
        sceneMgr.ChangeAsyncScene(sceneToLoad, onAsyncLoadingProgress);
    }

    protected void ProgressLoading(float progress, bool finish)
    {
        if (onAsyncLoadingProgress != null)
            onAsyncLoadingProgress(progress, finish);
    }

    public void RegisterProgressCallback(SceneMgr.OnAsyncLoadingProgress progress)
    {
        onAsyncLoadingProgress += progress;
    }

    public void UnregisterProgressCallback(SceneMgr.OnAsyncLoadingProgress progress)
    {
        onAsyncLoadingProgress -= progress;
    }

}
