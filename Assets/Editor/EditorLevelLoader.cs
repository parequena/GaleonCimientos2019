using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEditor.SceneManagement;

public class EditorLevelLoader : EditorWindow
{
    //TODO 1: Enlazar en el el menu con menuItem.
    [MenuItem("Master Tools/Editor Level Loader", priority = 6)]
    public static void Init()
    {
        EditorLevelLoader editor = (EditorLevelLoader)EditorWindow.GetWindow(typeof(EditorLevelLoader));
        editor.Create();
    }

    //Aqui inicializazo lo que tenga que inicializar :)
    public void Create()
    {

    }

    //Llama al pintado de la GUI del editor.
    void OnGUI()
    {
        //Por cada escena...
        GUILayout.BeginVertical();
        GUILayout.Label("Current Scene: ", GUILayout.MaxWidth(100));
        GUILayout.Label("     " + EditorSceneManager.GetActiveScene().name, EditorStyles.boldLabel);

        //TODO 2: recorrer las escenas de EditorBuildSettings.scenes accediendo
        //con el path al nombre de la escena.
        foreach (var scene in EditorBuildSettings.scenes)
        {

            string[] subfolders = scene.path.Split('/');
            string sceneName = subfolders[subfolders.Length - 1];
            if (GUILayout.Button(new GUIContent(sceneName)))
            {
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                EditorSceneManager.OpenScene(scene.path);
            }
        }


        GUILayout.Space(25);
        GUILayout.Label("Play Controls:", GUILayout.MaxWidth(100));
        if (GUILayout.Button(new GUIContent("Play ")))
        {
            //TODO 3.a Iniciar la aplicación
            EditorApplication.isPlaying = true;

        }
        if (GUILayout.Button(new GUIContent("Play and Close EditorLevelLoader")))
        {
            //TODO 3.a Iniciar la aplicación
            EditorApplication.isPlaying = true;

            this.Close();
        }
        GUILayout.EndVertical();
    }
}
