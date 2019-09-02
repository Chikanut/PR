using System;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomPropertyDrawer(typeof(SortingLayerAttribute))]
public class SortingLayerAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.Integer)
        {
            EditorGUI.LabelField(position, "The property has to be a layer for LayerAttribute to work!");
            return;
        }

        var currentLayer = GetSortingLayerNum(property.intValue);

        var selectedLayer = EditorGUI.Popup(position, label.text, currentLayer, GetSortingLayerNames());

        property.intValue = SortingLayer.NameToID(GetSortingLayerNames()[selectedLayer]);
    }

    public int GetSortingLayerNum(int layerCode)
    {
        var sortingLayers = GetSortingLayerNames();

        for (int i = 0; i < sortingLayers.Length; i++)
        {
            if (SortingLayer.NameToID(sortingLayers[i]).Equals(layerCode))
                return i;
        }

        return 0;
    }

    public string[] GetSortingLayerNames()
    {
        Type internalEditorUtilityType = typeof(InternalEditorUtility);
        PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
        var sortingLayers = (string[])sortingLayersProperty.GetValue(null, new object[0]);
        return sortingLayers;
    }
}
