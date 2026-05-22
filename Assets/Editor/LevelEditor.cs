using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MauerGenerator))]
public class LevelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Zeichnet den Standard-Inspector (die Variablen)
        DrawDefaultInspector();

        MauerGenerator generator = (MauerGenerator)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Editor Tools", EditorStyles.boldLabel);

        if (GUILayout.Button("Mauer bauen"))
        {
            generator.GenerateWall();
        }
    }
}