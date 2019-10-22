using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

/// <summary>
/// Script que añade al menú Create la creación del ScriptableObject dentro de la carpeta Resources/Config.
/// </summary>
public class GameMngConfigCreate
{
    
    //TODO 1 hacer que aparezca en el menu de creación de unity.
    static void CreateGameMgrConfiguration()
    {
        GameMgrConfig gmc = new GameMgrConfig();
        string path = "Assets/Resources/" + GameMgr.CONFIGURATION_FOLDER;
        ScriptableObjectMgr.Save<GameMgrConfig>(gmc, path+"/"+ GameMgr.CONFIGURATION_FILE+".asset");
    }
	
}
