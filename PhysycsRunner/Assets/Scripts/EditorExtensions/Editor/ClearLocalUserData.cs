using UnityEditor;
using UnityEngine;

public static class ClearLocalUserData {
    
    [MenuItem("Tools/Progress/Clear Local UserData")]
    public static void ClearLocalData() {
        PlayerPrefs.DeleteAll();
    }
    
}
