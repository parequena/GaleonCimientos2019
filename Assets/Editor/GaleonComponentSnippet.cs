using UnityEditor;
using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Text;

/// <summary>
/// Galeon component snippet.
/// </summary>
/// <author>
/// David Llanso & Ismael Sagredo.
/// </author>
public class GaleonComponentSnippet  
{
	[MenuItem("Assets/Create/Galeon Component Script")]
    static void CreateComponent()
    {

        string name = "MyGaleonScript";

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "")
        {
            path = "Assets";
        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeInstanceID)), "");
        }

        string filePath = EditorUtility.SaveFilePanel("create galeon file", path, name, "cs");
        if (filePath == "") return;

        using (StreamWriter outfile =
            new StreamWriter(filePath))
        {
            outfile.WriteLine("using UnityEngine;");
            outfile.WriteLine("using System.Collections;");
            outfile.WriteLine("");
            outfile.WriteLine("public class " + Path.GetFileNameWithoutExtension(filePath) + " : AComponent {");
            outfile.WriteLine(" ");
            outfile.WriteLine(" ");
            outfile.WriteLine(" \t// Call when component is create (only once)");
            outfile.WriteLine(" \t/*protected override void Awake() {");
            outfile.WriteLine(" \t\tbase.Awake();");
            outfile.WriteLine(" \t}*/");
            outfile.WriteLine(" ");
            outfile.WriteLine(" \t// Use this for initialization");
            outfile.WriteLine(" \tprotected override void Start() {");
            outfile.WriteLine(" \tbase.Start();");
            outfile.WriteLine(" \t}");
            outfile.WriteLine(" ");
            outfile.WriteLine(" ");
            outfile.WriteLine(" \t// Tick is called once per frame");
            outfile.WriteLine(" \tprotected override void Update() {");
            outfile.WriteLine(" \t\tbase.Update();");
            outfile.WriteLine(" \t}");
            outfile.WriteLine(" \t// End is called when component is destroy");
            outfile.WriteLine(" \t/*protected override void OnDestroy() {");
            outfile.WriteLine(" \tbase.OnDestroy();");
            outfile.WriteLine(" \t}*/");

            outfile.WriteLine("}");
        }
        AssetDatabase.Refresh();
    }
}
