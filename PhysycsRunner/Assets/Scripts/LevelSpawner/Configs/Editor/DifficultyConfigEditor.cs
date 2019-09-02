using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(DifficultyConfig))]
public class DifficultyConfigEditor : Editor {
    
    private ReorderableList _list;
    private ReorderableList _innerList;
    
    private void OnEnable() {   
        _list = new ReorderableList(serializedObject, 
            serializedObject.FindProperty("LevelConfigs"), 
            true, true, true, true);

        _list.drawElementCallback = 
            (rect, index, isActive, isFocused) => {
                var element = _list.serializedProperty.GetArrayElementAtIndex(index);  
                rect.y += 2;
                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y, EditorGUIUtility.currentViewWidth - 55, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("Distance"), GUIContent.none);

                if (isFocused) {
                    _innerList = new ReorderableList(serializedObject, 
                        element.FindPropertyRelative("TextAssets"), 
                        true, true, true, true);
                    
                    _innerList.drawElementCallback = (innerListRect, innerListIndex, innerListIsActive, innerListIsFocused) => {
                        var innerListElement = _innerList.serializedProperty.GetArrayElementAtIndex(innerListIndex);
                        rect.y += 2;
                        EditorGUI.PropertyField(new Rect(innerListRect.x, innerListRect.y, rect.width - 30, EditorGUIUtility.singleLineHeight),
                            innerListElement, GUIContent.none);                                                        
                    };
                    _innerList.onSelectCallback = list => {
                        var prefab = list.serializedProperty.GetArrayElementAtIndex(list.index).objectReferenceValue as GameObject;
                        if (prefab)
                            EditorGUIUtility.PingObject(prefab.gameObject);
                    };
                    _innerList.onCanRemoveCallback = list => list.count > 1;
                    _innerList.drawHeaderCallback = innerListRect => {
                        EditorGUI.LabelField(innerListRect, "Text Assets");
                    };
                }
            };

        _list.onCanRemoveCallback = list => list.count > 1;
        _list.onRemoveCallback = list => {
            if (EditorUtility.DisplayDialog("Warning!", 
                "Are you sure you want to delete this difficulty entry?", "Yes", "No")) {
                _innerList = null;
                ReorderableList.defaultBehaviours.DoRemoveButton(list);
            }
        };
        _list.drawHeaderCallback = rect => {
            EditorGUI.LabelField(
                new Rect(rect.x + 30, rect.y, 100, EditorGUIUtility.singleLineHeight),
                 new GUIContent("Distance"));
        };   
    }
    
    public override void OnInspectorGUI() {
        serializedObject.Update();
        _list.DoLayoutList();
        _innerList?.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}
