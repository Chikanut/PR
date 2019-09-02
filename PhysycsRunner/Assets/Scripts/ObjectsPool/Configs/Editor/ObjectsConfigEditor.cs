using System.CodeDom.Compiler;
using System.Linq;
using EditorExtensions.Editor;
using UnityEditor;
using UnityEngine;
using Levels.Objects;
using UnityEditorInternal;

[CustomEditor(typeof(ObjectsConfig))]
public class ObjectsConfigEditor : Editor
{
    readonly CodeDomProvider _provider = CodeDomProvider.CreateProvider("C#");
    ReorderableList _objectsInfo;
    Texture2D backgroundImage;
    int _selectedObject = -1;

    void OnEnable()
    {
        InitializeList(); 
    }

    public virtual void SetHightLightBackgroundImage(Color begin, Color end)
    {
        backgroundImage = new Texture2D(3, 1);
        backgroundImage.SetPixel(0, 0, begin);
        backgroundImage.SetPixel(2, 0, end);
        backgroundImage.hideFlags = HideFlags.DontSave;
        backgroundImage.wrapMode = TextureWrapMode.Clamp;
        backgroundImage.Apply();
    }

    void InitializeList()
    {
        var colors = new Color[] { Color.red, Color.blue, Color.white, Color.yellow, Color.green };

        _objectsInfo = new ReorderableList(serializedObject,
            serializedObject.FindProperty("ObjectsList"),
            false, true, true, true)
        {
            drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width / 2 - 15, EditorGUIUtility.singleLineHeight),
                    "Name");
                EditorGUI.LabelField(
                    new Rect(rect.x + rect.width / 2, rect.y, rect.width / 2, EditorGUIUtility.singleLineHeight),
                    "Object");
            }
        };


        _objectsInfo.drawElementCallback = 
            (Rect rect, int index, bool isActive, bool isFocused) => {
                var element = _objectsInfo.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;

                var prevName = element.FindPropertyRelative("Name").stringValue;
                var prevObject = element.FindPropertyRelative("Object").objectReferenceValue;

                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y, rect.width/2 - 15, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("Name"), GUIContent.none);

                EditorGUI.PropertyField(
                    new Rect(rect.x + rect.width/2, rect.y, rect.width/2, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("Object"), GUIContent.none);
                
                if(prevName != element.FindPropertyRelative("Name").stringValue)
                    CheckName(index, prevName);
                if(prevObject != element.FindPropertyRelative("Object").objectReferenceValue)
                    CheckObject(index);

                if (element.FindPropertyRelative("Object").objectReferenceValue != null &&
                    string.IsNullOrEmpty(element.FindPropertyRelative("Name").stringValue))
                {
                    element.FindPropertyRelative("Name").stringValue =
                        element.FindPropertyRelative("Object").objectReferenceValue.name;
                }
            };

        if (_objectsInfo.count > 0)
        {
            _objectsInfo.drawElementBackgroundCallback = (rect, index, active, focused) =>
            {
                var element = _objectsInfo.serializedProperty.GetArrayElementAtIndex(index);
                var prevName = element.FindPropertyRelative("Name").stringValue;

                var nameColor = string.IsNullOrEmpty(prevName)
                    ? colors[0]
                    : (_provider.IsValidIdentifier(prevName) ? colors[2] : colors[3]);
                var objectColor = element.FindPropertyRelative("Object").objectReferenceValue == null
                    ? colors[0]
                    : colors[2];

                if (nameColor == colors[2] && objectColor == colors[2])
                    nameColor = objectColor = colors[4];

                if (index == _selectedObject)
                    nameColor = objectColor = colors[1];

                SetHightLightBackgroundImage(nameColor, objectColor);

                EditorGUI.DrawTextureTransparent(rect, backgroundImage, ScaleMode.ScaleAndCrop);
            };
        }

        _objectsInfo.onAddCallback = (ReorderableList l) => {
           var index = l.serializedProperty.arraySize;
            l.serializedProperty.arraySize++;
            l.index = index;
            var element = l.serializedProperty.GetArrayElementAtIndex(index);
            element.FindPropertyRelative("Name").stringValue = "";
            element.FindPropertyRelative("Object").objectReferenceValue = null;
        };

        _objectsInfo.onSelectCallback = (ReorderableList l) => { _selectedObject = l.index; };
    }

    void CheckName(int index, string prevName)
    {
        var arr = _objectsInfo.serializedProperty;
        var objName = arr.GetArrayElementAtIndex(index).FindPropertyRelative("Name").stringValue;

        if(string.IsNullOrEmpty(objName))
            return;

        for (int i = 0; i < arr.arraySize; i++)
        {
            if (index == i ||
                !objName.Equals(arr.GetArrayElementAtIndex(i).FindPropertyRelative("Name").stringValue))
                continue;

            if (EditorUtility.DisplayDialog("Warning!", 
                "There is already element with "+objName+" name", "Raname"))
                _objectsInfo.serializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative("Name").stringValue = "";
        }
    }

    void CheckObject(int index)
    {
        var arr = _objectsInfo.serializedProperty;
        var obj = arr.GetArrayElementAtIndex(index).FindPropertyRelative("Object").objectReferenceValue;

        for (int i = 0; i < arr.arraySize; i++)
        {
            if (index == i || obj == null ||
                !obj.Equals(arr.GetArrayElementAtIndex(i).FindPropertyRelative("Object").objectReferenceValue))
                continue;

            if (EditorUtility.DisplayDialog("Warning!",
                "There is already element with " + obj.name + " object", "Ok"))
                _objectsInfo.serializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative("Object").objectReferenceValue = null;
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        _objectsInfo?.DoLayoutList();
        serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("Save changes"))
        {
                EnumGenerator.GenerateEnumFromStringList("PoolObjectTypes", "ObjectType",((ObjectsConfig) target).ObjectsList.Select(obj => obj.Name)
                    .Where(objName => _provider.IsValidIdentifier (objName)));
        }
    }
}
