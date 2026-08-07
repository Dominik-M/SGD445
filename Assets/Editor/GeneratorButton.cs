using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GeneratorBehaviour), true)]
public class GeneratorButton : Editor
{
    public override void OnInspectorGUI()
    {
        // Zeichnet den Standard-Inspector (die Variablen)
        DrawDefaultInspector();

        GeneratorBehaviour generator = (GeneratorBehaviour)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Editor Tools", EditorStyles.boldLabel);

        if (GUILayout.Button("Generate"))
        {
            generator.Generate();
        }
    }
}