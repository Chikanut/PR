using System.CodeDom.Compiler;
using System.Linq;
using EditorExtensions.Editor;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SoundsConfig))]
public class SoundsConfigEditor : Editor
{
    private readonly CodeDomProvider _provider = CodeDomProvider.CreateProvider("C#");

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Save changes"))
        {
            EnumGenerator.GenerateEnumFromStringList("AudioClipTypes", "AudioClipType", ((SoundsConfig)target).GetAvailableNames()
                .Where(objName => _provider.IsValidIdentifier(objName)));
        }
    }
}
