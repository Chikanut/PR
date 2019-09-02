using UnityEditor;

[CustomEditor(typeof(LocationPallete))]
public class LocationPalleteEditor : Editor {
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("GroupObject"));

        EditorGUILayout.LabelField("Cells");
        EditorList.Show(serializedObject.FindProperty("Cells"), EditorListOption.Buttons);

        serializedObject.ApplyModifiedProperties();
    }


}
