    using UnityEngine;
    using System;
    using System.Reflection;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;

namespace Visartech.Progress
{
    /// <summary>
    /// Base class to automatic saving and loading game data.
    /// Uses Reflection and Xml to save all non-static private and public fields.
    /// </summary>
    /// <typeparam name="T">Type of class to save</typeparam>
    [Serializable]
    public class ProgressBase<T> : MonoBehaviour where T : ProgressBase<T>
    {

        private static T p_instance;

        private const BindingFlags Flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;

        public static T instance
        {
            get
            {
                if (p_instance == null)
                {
                    GameObject go = new GameObject("_progress");
                    p_instance = go.AddComponent<T>();
                    DontDestroyOnLoad(go);
                    LoadAllFields(p_instance);
                }

                return p_instance;
            }
        }

        public static T GetInstance()
        {
            return instance;
        }

        void OnApplicationQuit()
        {
            Save();
            PlayerPrefs.Save();
        }

        void OnApplicationPause(bool pauseStatus)
        {
            Save();
        }

        private static void LoadAllFields(T _instance)
        {
            FieldInfo[] fields = _instance.GetType().GetFields(Flags);
            foreach (FieldInfo fInfo in fields)
            {
                if (fInfo.FieldType.IsArray)
                    LoadArrayField(fInfo);
                else
                    LoadField(fInfo);
            }
        }

        private static void LoadField(FieldInfo fieldInfo)
        {
            string key = fieldInfo.FieldType.ToString() + "+" + fieldInfo.Name;
            if (PlayerPrefs.HasKey(key))
            {
                var obj = Deserialize(PlayerPrefs.GetString(key), fieldInfo.FieldType);
                fieldInfo.SetValue(instance, obj);
            }
            else
            {
                CreateDefaultField(fieldInfo);
            }
        }

        private static void LoadArrayField(FieldInfo fieldInfo)
        {
            string key = fieldInfo.FieldType.ToString() + "+" + fieldInfo.Name;
            if (PlayerPrefs.HasKey(key))
            {
                Array obj = Deserialize(PlayerPrefs.GetString(key), fieldInfo.FieldType) as Array;
                Array array = fieldInfo.GetValue(instance) as Array;
                CreateDefaultField(fieldInfo, Mathf.Max(obj.Length, array.Length));
                for (int i = obj.GetLowerBound(0); i <= Mathf.Min(obj.GetUpperBound(0), array.GetUpperBound(0)); i++)
                {
                    array.SetValue(obj.GetValue(i), i);
                }

                fieldInfo.SetValue(instance, array);
            }
            else
                CreateDefaultField(fieldInfo);
        }

        private static void CreateDefaultField(FieldInfo fieldInfo, int arrayLength = -1)
        {
            if (fieldInfo.FieldType.IsArray)
            {
                if (arrayLength < 0)
                    arrayLength = (fieldInfo.GetValue(instance) as Array).Length;
                Array array = Array.CreateInstance(fieldInfo.FieldType.GetElementType(), arrayLength);
                for (int i = array.GetLowerBound(0); i <= array.GetUpperBound(0); i++)
                    array.SetValue(Activator.CreateInstance(fieldInfo.FieldType.GetElementType()), i);
                fieldInfo.SetValue(instance, array);
            }
            else
                fieldInfo.SetValue(instance, Activator.CreateInstance(fieldInfo.FieldType));
        }

        /// <summary>
        /// Force save field info 
        /// </summary>
        /// <param name="fieldInfo">Field to save</param>
        protected void SaveField(FieldInfo fieldInfo)
        {
            string key = fieldInfo.FieldType.ToString() + "+" + fieldInfo.Name;
            string value = Serialize(fieldInfo.GetValue(instance), fieldInfo.FieldType);
            PlayerPrefs.SetString(key, value);
        }

        /// <summary>
        /// Force save field info 
        /// </summary>
        /// <param name="fieldName">name of field</param>
        protected void SaveField(string fieldName)
        {
            FieldInfo[] fields = instance.GetType().GetFields(Flags);
            foreach (FieldInfo fInfo in fields)
            {
                if (fInfo.Name == fieldName)
                {
                    SaveField(fInfo);
                    return;
                }
            }
        }

        /// <summary>
        /// Clears all fields and set default values
        /// </summary>
        protected void ClearAllFields()
        {
            FieldInfo[] fields = instance.GetType().GetFields(Flags);
            foreach (FieldInfo fInfo in fields)
                CreateDefaultField(fInfo, fInfo.FieldType.IsArray ? (fInfo.GetValue(instance) as Array).Length : -1);
        }

        /// <summary>
        /// Save all fields by type
        /// </summary>
        /// <typeparam name="_type">Type of field to save</typeparam>
        public static void Save<_type>()
        {
            FieldInfo[] fields = instance.GetType().GetFields(Flags);
            foreach (FieldInfo fInfo in fields)
            {
                if (fInfo.FieldType == typeof(_type))
                    instance.SaveField(fInfo);
            }
        }

        /// <summary>
        /// Force save all fields
        /// </summary>
        public static void Save()
        {
            FieldInfo[] fields = instance.GetType().GetFields(Flags);
            foreach (FieldInfo fInfo in fields)
                instance.SaveField(fInfo);
        }

        private static string Serialize(object details)
        {
            return Serialize(details, details.GetType());
        }

        private static string Serialize<T>(object details)
        {
            return Serialize(details, typeof(T));
        }

        private static string Serialize(object details, System.Type type)
        {
            XmlSerializer serializer = new XmlSerializer(type);
            MemoryStream stream = new MemoryStream();
            serializer.Serialize(stream, details);
            StreamReader reader = new StreamReader(stream);
            stream.Position = 0;
            string retSrt = reader.ReadToEnd();
            stream.Flush();
            stream = null;
            reader = null;
            return retSrt;
        }

        private static T Deserialize<T>(string details)
        {
            return (T) Deserialize(details, typeof(T));
        }

        private static object Deserialize(string details, System.Type type)
        {
            XmlSerializer serializer = new XmlSerializer(type);
            XmlReader reader = XmlReader.Create(new StringReader(details));
            return serializer.Deserialize(reader);
        }
    }
}