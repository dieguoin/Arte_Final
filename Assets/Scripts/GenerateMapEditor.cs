using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(GenerateMap))]
public class GenerateMapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();


        GenerateMap myMapEditor = (GenerateMap)target;
        if (GUILayout.Button("Create Scene"))
        {
            myMapEditor.Create();
        }
    }
}
