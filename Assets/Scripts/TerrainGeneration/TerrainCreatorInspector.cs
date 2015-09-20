using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainCreator))]
public class TerrainCreatorInspector : Editor
{

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        DrawDefaultInspector();
        if (EditorGUI.EndChangeCheck() && Application.isPlaying)
            (target as TerrainCreator).Refresh();
    }
}
