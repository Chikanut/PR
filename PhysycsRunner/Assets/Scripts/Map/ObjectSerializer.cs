using System.Collections.Generic;
using CBX.TileMapping.Unity;
using Helpers;
using Levels.Objects;
using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Levels
{
    [System.Serializable]
    public class ChildInfo
    {
        public string ObjectName;
        public Vector3 LocalPosition;
        public string Settings;
    }

    [System.Serializable]
    public class PatternObjectInfo
    {
        public List<ChildInfo> Childs;
        public Vector3 Size;
    }

    public class ObjectSerializer
    {
#if UNITY_EDITOR
        const string ObjectsConfigPath = "Assets/_Configs/ObjectsConfig.asset";

        public static string SerializeObject(Transform targetObject)
        {
            var objectsConfig = AssetDatabase.LoadAssetAtPath<ObjectsConfig>(ObjectsConfigPath);
            var patternSettings = targetObject.GetComponent<TileMap>();

            var childCount = targetObject.childCount;
            var childsInfo = new List<ChildInfo>();

            PoolObject child = null;
            ObjectInfo childInfo = null;

            for (var i = 0; i < childCount; i++)
            {
                child = targetObject.GetChild(i).GetComponent<PoolObject>();
                childInfo = objectsConfig.IsObjectInConfig(child);

                if (child == null || childInfo == null || child.GetComponent<CellPoolObject>() != null)
                    continue;

                var info = new ChildInfo()
                {
                    ObjectName = childInfo.Name,
                    LocalPosition = child.transform.position,
                    Settings = child.SerializeSettings()
                };

                childsInfo.Add(info);
            }

            var objectInfo = new PatternObjectInfo()
            {
                Childs = childsInfo,
                Size = new Vector3(patternSettings.Rows,0, patternSettings.Columns)
            };

            var serealizedInfo = XMLHelper.Serialize<PatternObjectInfo>(objectInfo);

            return serealizedInfo;
        }
#endif
        public static PatternObjectInfo DeserializeObject(TextAsset info)
        {
            return XMLHelper.Deserialize<PatternObjectInfo>(info.text);
        }
       
    }
}

