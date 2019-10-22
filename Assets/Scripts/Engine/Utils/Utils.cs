using UnityEngine;
using System.Collections;
using System;
using System.Diagnostics;
//using UnityEditor;

public class Utils 
{
	/*public static GameObject LoadPrefabFromFile(string path)
    {
        path += ".prefab";

        GameObject toInstantiate = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;

        return toInstantiate;
    }*/
	
	public static bool IsInArray(string tag, string [] arr)
	{
		bool found = false;
		for(int i = 0; !found && i < arr.Length; ++i )
		{
			if( tag == arr[i])
			{
				found = true;
			}
		}
		
		return found;
	}
}
