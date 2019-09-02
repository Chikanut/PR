using System.Collections.Generic;
using System.Linq;
using Levels.Objects;
using UnityEngine;

public class ObjectsPool
{
    private readonly Transform _objectsParent;
    private readonly ObjectsConfig _objectsConfig;
    private readonly Dictionary<ObjectType, List<PoolObject>> _instantiatedObjectsPerType;

    public ObjectsPool(Transform objectsParent, ObjectsConfig objectsConfig)
    {
        _objectsConfig = objectsConfig;
        _objectsParent = objectsParent;
        _instantiatedObjectsPerType = new Dictionary<ObjectType, List<PoolObject>>();
    }
    
    /// <summary>
    /// Returns existed inactive object of base type.
    /// Instantiates object first if there are no inactive objects of given type yet
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private PoolObject GetObjectOfType(ObjectType type, Transform parent, bool isUnique) {
        PoolObject reusableObject;
        if (_instantiatedObjectsPerType.ContainsKey(type)) {
            var objectsList = _instantiatedObjectsPerType[type];
            reusableObject = isUnique ? objectsList.FirstOrDefault() : 
                objectsList.FirstOrDefault(item => item != null && !item.IsActive);
            //if there are no required objects now
            if (reusableObject == null) {
                reusableObject = InstantiateObjectOfType(type, parent);
                objectsList.Add(reusableObject);
            }
            else if(parent) {
                reusableObject.transform.SetParent(parent);
            }
        }
        else {
            reusableObject = InstantiateObjectOfType(type, parent);
            _instantiatedObjectsPerType[type] = new List<PoolObject>{reusableObject};
        }

        reusableObject.IsActive = true;
        return reusableObject;
    }

    /// <summary>
    /// Returns unique existed object of requested type
    /// Instantiates object first if there are no unique objects of requested type yet
    /// Does not instantiate more than one object
    /// </summary>
    /// <param name="type"></param>
    /// <param name="parent"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetUniqueObjectOfType<T> (ObjectType type, Transform parent = null) where T: PoolObject {
        return (T) GetObjectOfType(type, parent, true);
    }
    
    /// <summary>
    /// Returns existed inactive object of requested type.
    /// Instantiates object first if there are no inactive objects of given requested yet
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public T GetObjectOfType<T>(ObjectType type, Transform parent = null) where T: PoolObject {
        return (T)GetObjectOfType(type, parent, false);
    }
    
    
    /// <summary>
    /// Adds list of objects of requested type,
    /// adds new objects to pool if count is greater than reusable objects count of given type.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public void GetObjectsOfType(ObjectType type, int count) {
        List<PoolObject> objectsList;
        if (_instantiatedObjectsPerType.ContainsKey(type)) {
            objectsList = _instantiatedObjectsPerType[type];
        }
        else {
            objectsList = new List<PoolObject>();
            _instantiatedObjectsPerType[type] = objectsList;
        }
        //if we request more objects of given type that exist in pool at this moment
        var reusableObjects = objectsList.Where(item => !item.IsActive).ToList();
        var reusableObjToActivate = Mathf.Min(count, reusableObjects.Count);
        for (int i = 0; i < reusableObjToActivate; i++) {
            reusableObjects[i].IsActive = true;
            count--;
        }
        for (int i = 0; i < count; i++) {
            objectsList.Add(InstantiateObjectOfType(type, _objectsParent));
        }
    }
    
    
    /// <summary>
    /// For each given object type adds given count of objects to pool
    /// </summary>
    /// <param name="objectsPerType"></param>
    public void GetObjectsSet(Dictionary<ObjectType, int> objectsPerType, Transform parent = null) {
        foreach (var data in objectsPerType) {
            if (data.Value == 1) {
                GetObjectOfType(data.Key, parent, false);
            }
            else {
                GetObjectsOfType(data.Key, data.Value);
            }
        }
    }
      
    /// <summary>
    /// Returns list of all objects of given type, active and inactive
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public List<PoolObject> GetAllObjectsOfType(ObjectType type) {
        if (HasObjectsOfType(type)) {
            return _instantiatedObjectsPerType[type];
        }
        Debug.LogWarning($"There are no objects of type '{type}' yet, returning an empty list");
        return new List<PoolObject>();
    }
    
    /// <summary>
    /// Returns list of all objects of given type that correspond to given state
    /// </summary>
    /// <param name="type"></param>
    /// <param name="inActiveState"></param>
    /// <returns></returns>
    public List<PoolObject> GetAllObjectsOfType(ObjectType type, bool inActiveState) {
        if (HasObjectsOfType(type)) {
            return _instantiatedObjectsPerType[type].Where(item => item.IsActive == inActiveState).ToList();
        }
        Debug.LogWarning($"There are no objects of type '{type}' yet, returning an empty list");
        return new List<PoolObject>();
    }

    /// <summary>
    /// Returns dictionary with keys of requested objects type and values of objects list
    /// </summary>
    /// <param name="types"></param>
    /// <returns></returns>
    public Dictionary<ObjectType, List<PoolObject>> GetObjectsSetOfTypes(List<ObjectType> types) {
        Dictionary<ObjectType, List<PoolObject>> objectsOfType = new Dictionary<ObjectType, List<PoolObject>>();
        foreach (var type in types) {
            objectsOfType.Add(type, GetAllObjectsOfType(type));
        }
        return objectsOfType;
    }
    
    /// <summary>
    /// Returns dictionary with keys of requested objects type and values of objects list
    /// where objects correspond to given state
    /// </summary>
    /// <param name="types"></param>
    /// <param name="inActiveState"></param>
    /// <returns></returns>
    public Dictionary<ObjectType, List<PoolObject>> GetObjectsSetOfTypes(List<ObjectType> types, bool inActiveState) {
        Dictionary<ObjectType, List<PoolObject>> objectsOfType = new Dictionary<ObjectType, List<PoolObject>>();
        foreach (var type in types) {
            objectsOfType.Add(type, GetAllObjectsOfType(type, inActiveState));
        }
        return objectsOfType;
    }
    
    /// <summary>
    /// Indicates if this objects pool contains any objects of given type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public bool HasObjectsOfType(ObjectType type) {
        return _instantiatedObjectsPerType.ContainsKey(type);
    }
    
    /// <summary>
    /// Instantiates object of given type
    /// </summary>
    /// <param name="objectName"></param>
    /// <returns></returns>
    private PoolObject InstantiateObjectOfType(ObjectType type, Transform parent) {
        return Object.Instantiate(_objectsConfig.GetObjectOriginal(type), parent ? parent : _objectsParent, false);
    }
    
    /// <summary>
    /// Returns dictionary of all objects per type, active and inactive
    /// </summary>
    /// <returns></returns>
    public Dictionary<ObjectType, List<PoolObject>> GetAllCurrentObjectsPerType() {
        return _instantiatedObjectsPerType;
    }
    
    /// <summary>
    /// Returns dictionary of all objects per type, objects are filtered by their active state
    /// </summary>
    /// <param name="inActiveState"></param>
    /// <returns></returns>
    public Dictionary<ObjectType, List<PoolObject>> GetAllCurrentObjectsPerType(bool inActiveState) {
        return GetObjectsSetOfTypes(_instantiatedObjectsPerType.Keys.ToList(), inActiveState);
    }
    
    /// <summary>
    /// Deactivates all current active objects
    /// </summary>
    public void DeactivateAllObjects() {
        foreach (var data in GetAllCurrentObjectsPerType(true)) {
            foreach (var poolObject in data.Value) {
                poolObject.Destroy();
            }
        }
    }
    
}
