using UnityEngine;
using UnityEditor;


[CustomPropertyDrawer(typeof(StorageMgrConfig))]
public class StorageMgrConfigPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //property.serializedObject.targetObject.
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = indent + 1;
        EditorGUI.BeginProperty(position, label, property);
        EditorGUI.PropertyField(new Rect(position.x, position.y, 300, position.height), property.FindPropertyRelative("m_storageFileName"), new GUIContent("Storage File Name", "Nombre del fichero donde guardaremos los datos de guardado"));
        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }

    //public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
}


[CustomPropertyDrawer(typeof(InputMgrConfig))]
public class InputMgrConfigPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        var indent = EditorGUI.indentLevel;
        EditorGUI.LabelField(new Rect(position.x, position.y, 180, 16f), label);
        EditorGUI.indentLevel = indent + 1;
        SerializedProperty soPointAndClickActive = property.FindPropertyRelative("m_pointAndClickActive");
        bool soPointAndClickActiveBool = soPointAndClickActive.boolValue;
        EditorGUI.PropertyField(new Rect(position.x, position.y+16f, 300, 16f), soPointAndClickActive, new GUIContent("PointAndClick Active", "Activamos el point and click?"));
        m_height = 0;
        if (soPointAndClickActiveBool)
        {
            EditorGUI.indentLevel = indent + 2;
            EditorGUI.PropertyField(new Rect(position.x, position.y + 32f, 300, 16f), property.FindPropertyRelative("m_buttonIdToPointAndClick"), new GUIContent("Button Id To PointAndClick", "Boton del ratón que hace el point and click"));
            EditorGUI.indentLevel = indent + 1;
            m_height = 32;
        }
        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label) + m_height;
    }

    protected float m_height;
}