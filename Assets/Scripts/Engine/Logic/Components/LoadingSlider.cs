using UnityEngine;
using System.Collections;
using UnityEngine.UI;


/// <summary>
/// componente que maneja un Slider de unity y lo utiliza como una barra de carga.
/// </summary>
public class LoadingSlider : MonoBehaviour
{
    public AsyncLoader asyncLoader;

    private SceneMgr m_gameMgr;
    private Slider m_slider;
    private float m_progress = 0.0f;
    // private SceneMgr.OnAsyncLoadingProgress m_progress;


    // Use this for initialization
    protected void Start ()
    {
        // Init the progress.
        // m_progress(0, false);

        // Get the game manager.
        m_gameMgr = GameMgr.GetInstance().GetServer<SceneMgr>();

        //TODO 1: registrar el evento ProgressLoading en el AsyncLoader
        asyncLoader.RegisterProgressCallback(ProgressLoading);

        m_slider = GetComponent<Slider>();
        m_slider.value = 0f;
    }

    // Update is called once per frame
    protected void Update ()
    {

        // ProgressLoading(m_progress, m_progress >= 0.99);

	}

    protected void ProgressLoading(float progress, bool finish)
    {
        m_slider.value = progress;
        // m_progress(progress, finish);
        //TODO 2: deregistrar el evento cuando termina la carga (finish)
        // If its ended, "destroy" it.
        if (finish) asyncLoader.UnregisterProgressCallback(ProgressLoading);
    }

    protected void OnDestroy()
    {
        //TODO 3: deregistrar el evento cuando se destruye el objeto. Comprobar que asyncLoader sea distinto de null por sanidad :)
        if(asyncLoader != null) // If we have an object, destroy it.
        {
            asyncLoader.UnregisterProgressCallback(ProgressLoading);
            Destroy(asyncLoader);
        }
    }
}
