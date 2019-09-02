using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Levels.Objects
{
    [System.Serializable]
    public class ObjectInfo
    {
        public string Name;
        public PoolObject Object;
    }

    [CreateAssetMenu(fileName = "ObjectsConfig", menuName = "Configs/ObjectsConfig")]
    public class ObjectsConfig : ScriptableObject
    {
        public List<ObjectInfo> ObjectsList = new List<ObjectInfo>();

        #if UNITY_EDITOR
        public ObjectInfo IsObjectInConfig(PoolObject obj)
        {
            return ObjectsList.Find(anyObj =>
                anyObj.Object.GetInstanceID() == PrefabUtility
                    .GetCorrespondingObjectFromOriginalSource(obj).GetInstanceID());
        }
        #endif

        public PoolObject GetObjectOriginal(ObjectType objName)
        {
            return ObjectsList.FirstOrDefault(obj => obj.Name == objName.ToString())?.Object;
        }
    }
}
