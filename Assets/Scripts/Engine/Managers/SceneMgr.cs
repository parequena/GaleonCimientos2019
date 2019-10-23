using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMgr : MonoBehaviour
{
    public delegate void OnAsyncLoadingProgress(float progress, bool finish);
    public delegate void OnSceneDestroy();

    protected struct TSceneInfo
    {
        public string name;
        public List<string> subScenes;
    }

    /// <summary>
    /// Pila de escenas
    /// </summary>
    private Stack<TSceneInfo> m_stackScenes = new Stack<TSceneInfo>();
    /// <summary>
    /// Número de subscenas almacenadas.
    /// </summary>
    private int m_numSubSceneLoading = 0;
    /// <summary>
    /// Estoy cargando una escena principal asincrona. Si esto sucede no debe cargar más escenas
    /// </summary>
    private bool m_justAsyncLoaderMainScene = false;
    /// <summary>
    /// He intentado cargar una escena per resulta que aún estoy cargando subescenas que me han quedado a medias.
    /// </summary>
    private string m_deferredSceneChange = "";
    private OnAsyncLoadingProgress m_deferredCallback;

    private OnSceneDestroy m_sceneDestroyCallbacks;
    //NEXT_SCENE y SCENE_SECTION alamcenan en la memoria volatil entre escenas
    //la inormación de la siguiente escena a cargar. Esto es util si queremos construir un
    //una pantalla de carga.
    public const string NEXT_SCENE = "next_scene";
    public const string SCENE_SECTION = "scene";

    protected void Awake()
    {
        //Para evitar que se destruya entre escenas.
        DontDestroyOnLoad(this);
        StoreLevelInfoInStack(SceneManager.GetActiveScene().name);
    }

    protected void Update()
    {
        if (m_numSubSceneLoading == 0)
        {
            if (m_deferredSceneChange != null && m_deferredSceneChange != "")
            {
                StartCoroutine(LoadingAsync(m_deferredSceneChange,m_deferredCallback));
                m_deferredSceneChange = "";
            }
        }
    }

    public bool IsLoadingFinish
    {
        get { return !m_justAsyncLoaderMainScene; }
    }

    public void RegisterDestroyScene(OnSceneDestroy destroy)
    {
        m_sceneDestroyCallbacks += destroy;
    }

    public void UnRegisterDestroyScene(OnSceneDestroy destroy)
    {
        m_sceneDestroyCallbacks -= destroy;
    }

    

    /// <summary>
    /// Cambiamos la escena de forma no asincrona
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="next"></param>
    public void ChangeScene(string sceneName, string next = "")
    {
        m_stackScenes.Clear();

        //TODO 1 Si next es distinto de "" lo guardamos en el amacenamiento volatil para que la siguiente escena pueda leerla.
        //Avisamos de que vamso a destruir la escena...
        if (next != "" && next != null)
        {
            GameMgr.GetInstance().GetStorageMgr().SetVolatile(SCENE_SECTION, NEXT_SCENE, next);
        }

        if (m_sceneDestroyCallbacks != null)
            m_sceneDestroyCallbacks();

        //TODO 2 cargamos la escena y la almacenamos en la cima de la pila usando StoreLevelInfoInStack
        SceneManager.LoadScene(sceneName);
        StoreLevelInfoInStack(sceneName);
    }

    /// <summary>
    /// Cambia de escena de manera asincrona.
    /// </summary>
    /// <param name="sceneToLoad"></param>
    /// <param name="onAsyncLoadingProgress"></param>
    public void ChangeAsyncScene(string sceneToLoad, OnAsyncLoadingProgress onAsyncLoadingProgress)
    {
        if (m_numSubSceneLoading == 0)
        {
            //TODO 3 lanzamos la corrutina LoadingAsync
            StartCoroutine(LoadingAsync(sceneToLoad, onAsyncLoadingProgress));
        }
        else
        {
            m_deferredSceneChange = sceneToLoad;
            m_deferredCallback = onAsyncLoadingProgress;
        }
    }

    /// <summary>
    /// Cargamos una subscena de forma aditiva.
    /// </summary>
    /// <param name="subScene"></param>
    /// <param name="asyn"></param>
    public void LoadSubScene(string subScene, bool asyn = false, OnAsyncLoadingProgress onAsyncLoadingProgress = null)
    {
        TSceneInfo sceneInfo = m_stackScenes.Peek();
        if (!sceneInfo.subScenes.Contains(subScene))
        {
            //Carga de la subscena
            if (asyn)
            {
                //TODO 4 lanzamos la corrutina LoadingAdditiveAsync
                StartCoroutine(LoadingAdditiveAsync(subScene, false, onAsyncLoadingProgress));
            }
            else
            {
                SceneManager.LoadScene(subScene, LoadSceneMode.Additive);
                StoreSubSceneInCurrentScene(subScene);
            }
        }
        else // Si ya está cargada, como mucho podemos intentar activarla.
        {
            Scene scene = SceneManager.GetSceneByName(subScene);
            if(IsDesactivate(scene))
            {
                Activate(scene, true);
            }
        }

    }

    /// <summary>
    /// Descargamos una subscena de la escena principal.
    /// </summary>
    /// <param name="subScene"></param>
    /// <param name="clearSubScene"></param>
    public void UnloadSubScene(string subScene, bool clearSubScene)
    {
        TSceneInfo current = m_stackScenes.Peek();
        if (current.subScenes.Contains(subScene))
        {
            if (clearSubScene)
            {
                current.subScenes.Remove(subScene);
                SceneManager.UnloadSceneAsync(subScene);
            }
            else
            {
                Scene scene = SceneManager.GetSceneByName(subScene);
                Activate(scene,false);
            }
        }
            
        //StoreSubSceneInCurrentScene(subScene);
    }

    /******** PILA DE ESCENAS ***********************/
    /// <summary>
    /// Cargamos uan escena de forma aditiva y la apilamos en nuestra pila de escenas, dejando la escena actual desactivada.
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="asyn"></param>
    /// <param name="onAsyncLoadingProgress"></param>
    public void PushScene(string sceneName, bool asyn = false, OnAsyncLoadingProgress onAsyncLoadingProgress = null)
    {
        bool needLoading = true;
        if (m_stackScenes.Count > 0)
        {
            TSceneInfo current = m_stackScenes.Peek();
            if(current.name != sceneName)
                SuspendScene(current);
            else
            {
                needLoading = false;
                Scene scene = SceneManager.GetSceneByName(sceneName);
                if(IsDesactivate(scene))
                {
                    Activate(scene,true);
                }
            }
        }

        if (needLoading)
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);
            if (scene.buildIndex >= 0 && IsDesactivate(scene))
            {
                Activate(scene, true);
                StoreLevelInfoInStack(sceneName);
            }
            else
            {
                if (asyn)
                {
                    StartCoroutine(LoadingAdditiveAsync(sceneName, true, onAsyncLoadingProgress));
                }
                else
                {
                    SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
                    StoreLevelInfoInStack(sceneName);
                }
            }
        }
    }

    /// <summary>
    /// Desapilamos la escena apilada en la cima de la pila.
    /// </summary>
    /// <param name="clearCurrentScene"></param>
    public void PopScene(bool clearCurrentScene)
    {
        Debug.Assert(m_stackScenes.Count > 1, "Error, No hay escena a la cual volver");

        //TODO 5 desapilamos de la cima de la pila
        TSceneInfo current = m_stackScenes.Pop();

        //NOTA: Puede querer destruirla? en principio si...
        if (clearCurrentScene)
            DestroyScene(current);
        else
        {
            Scene scene = SceneManager.GetSceneByName(current.name);
            Activate(scene,false);
        }

        TSceneInfo previousScene = m_stackScenes.Peek();
        BackToLifeScene(previousScene);
    }

    /// <summary>
    /// Obtenemos el número de escenas apiladas.
    /// </summary>
    /// <returns></returns>
    public int GetNumScenesStacked()
    {
        return m_stackScenes.Count;
    }

    protected void BackToLifeScene(TSceneInfo sceneInfo)
    {
        foreach (string subScene in sceneInfo.subScenes)
        {
            Scene scene = SceneManager.GetSceneByName(subScene);
            if (IsDesactivate(scene))
                Activate(scene, true);
        }
        Scene mainScene = SceneManager.GetSceneByName(sceneInfo.name);
        if (IsDesactivate(mainScene))
            Activate(mainScene, true);
    }

    protected void DestroyScene(TSceneInfo sceneInfo)
    {
        foreach (string subscenes in sceneInfo.subScenes)
        {
            SceneManager.UnloadSceneAsync(subscenes);
        }
        if (m_sceneDestroyCallbacks != null)
            m_sceneDestroyCallbacks();
        SceneManager.UnloadSceneAsync(sceneInfo.name);
        /*if (m_sceneDestroyCallbacks != null)
            m_sceneDestroyCallbacks();*/
    }

    protected void SuspendScene(TSceneInfo sceneInfo)
    {
        foreach (string subScene in sceneInfo.subScenes)
        {
            Scene scene = SceneManager.GetSceneByName(subScene);
            Activate(scene, false);
        }

        Scene mainScene = SceneManager.GetSceneByName(sceneInfo.name);
        Activate(mainScene, false);
    }


    protected TSceneInfo StoreLevelInfoInStack(string levelName)
    {
        TSceneInfo info = new TSceneInfo();
        info.name = levelName;
        info.subScenes = new List<string>();
        m_stackScenes.Push(info);
        return info;
    }

    protected void StoreSubSceneInCurrentScene(string subSceneName)
    {
        TSceneInfo info = m_stackScenes.Peek();

        if (!info.subScenes.Contains(subSceneName))
            info.subScenes.Add(subSceneName);
    }

    protected void Activate(Scene scene, bool act)
    {
        if (scene.buildIndex >= 0)
        {
            GameObject[] gameObjects = scene.GetRootGameObjects();
            for (int i = 0; i < gameObjects.Length; ++i)
            {
                gameObjects[i].SetActive(act);
            }
        }
    }

    protected bool IsDesactivate(Scene scene)
    {
        if (scene.buildIndex >= 0)
        {
            GameObject[] gameObjects = scene.GetRootGameObjects();
            for (int i = 0; i < gameObjects.Length; ++i)
            {
                if (gameObjects[i].activeInHierarchy)
                    return false;
            }
            return true;
        }
        return false;
        
    }

    protected IEnumerator LoadingAsync(string sceneName, OnAsyncLoadingProgress onAsyncLoadingProgress)
    {
        m_justAsyncLoaderMainScene = true;
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        //Como vamso a hacer un cambio de escena y las escenas actuales va na ser destruidas
        //no hace falta esperar a cambiar de escena para reescribir nuestra estructura de datos
        //de la escena. Así evitamos que se inicie antes el start de la siguiente escena que 
        //la actualización de los métodos de las corrutinas. "Recordad el flujo de llamadas!!!!!"
        m_stackScenes.Clear();
        StoreLevelInfoInStack(sceneName);
        do
        {
            if (onAsyncLoadingProgress != null)
            {
                onAsyncLoadingProgress(op.progress, op.isDone);
            }
            yield return new WaitForEndOfFrame();
        } while (!op.isDone);
        m_justAsyncLoaderMainScene = false;

    }

    protected IEnumerator LoadingAdditiveAsync(string subScene, bool inStack, OnAsyncLoadingProgress onAsyncLoadingProgress)
    {
        //TODO 6 hacemos la carga aditiva sobre la variale op.
        m_numSubSceneLoading++;
        AsyncOperation op = SceneManager.LoadSceneAsync(subScene, LoadSceneMode.Additive);
        do
        {
            if (onAsyncLoadingProgress != null)
            {
                onAsyncLoadingProgress(op.progress, op.isDone);
            }
            //TODO 7 yield return WaitForEndOfFrame bloquea la corrutina hasta el siguiente frame
            yield return new WaitForEndOfFrame();
        } while (!op.isDone);

        if (inStack)
            StoreLevelInfoInStack(subScene);
        else
            StoreSubSceneInCurrentScene(subScene);
        m_numSubSceneLoading--;
        yield return null;
    }
}
