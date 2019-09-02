using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace EditorExtensions.Editor {
    public static class EnumGenerator {
    
        public static void GenerateEnumFromStringList(string fileName, string enumName, IEnumerable<string> enumEntries) {
            if (!IsValidFileName(fileName) || !IsValidFileName(enumName)) {
                throw new FormatException("File name or enum name contain invalid characters");
            }
            string filePathAndName = "Assets/Scripts/ObjectsPool/Configs/" + fileName + ".cs";    
            using (StreamWriter streamWriter = new StreamWriter(filePathAndName)) {
                streamWriter.WriteLine($"public enum  {enumName} " + "{");
                foreach (var entry in enumEntries) {
                    streamWriter.WriteLine("\t" + entry + ",");
                }
                streamWriter.WriteLine("}");
                streamWriter.Close();
            }
            AssetDatabase.Refresh();
        }

        private static bool IsValidFileName(string fileName) {
            return !string.IsNullOrEmpty(fileName) &&
                   fileName.IndexOfAny(Path.GetInvalidFileNameChars()) < 0;
        }
        
    }
}
