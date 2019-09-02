using System;
using System.Collections.Generic;
using Levels;
using UnityEngine;

public interface ISpawner
{
    void Init(Action onPatternSpawned);
    void ChangeColor(LevelColors c);
    void ResetObjects();
    void OnDistanceChanged(float v);
    void SetNewPattern(PatternObjectInfo info);
}

public class PatternSpawner : ISpawner
{
    public PatternSpawner(ObjectsPool pool)
    {
        _pool = pool;
    }

    const int SpawnRadius = 25;
    const int ClearRadius = 25;
    const int StartDistance = 25;

    private Vector3 _currentDirection = Vector3.forward;
    LevelColors _currentColor;
    private Action _onPatternSpawned;
    readonly ObjectsPool _pool; 
    readonly HashSet<PoolObject> _spawnedObjects = new HashSet<PoolObject>();
    readonly HashSet<PoolObject> _excludedObjects = new HashSet<PoolObject>();
    
    float _currentDistance = 0;
    Vector3 _spawnPoint = Vector3.zero;
    PatternObjectInfo _currentPattern;

    public void Init(Action onPatternSpawned)
    {
        _onPatternSpawned = onPatternSpawned;
    }

    public void ChangeColor(LevelColors c)
    {
        _currentColor = c;
    }

    public void ResetObjects()
    {
        _currentPattern = null;

        foreach (var obj in _spawnedObjects)
            obj.Destroy();

        _spawnedObjects.Clear();
    }

    public void OnDistanceChanged(float value)
    {
        _currentDistance = value + StartDistance;
        
        SpawnPattern();

        if (_currentPattern == null || _currentPattern.Childs.Count == 0)
            _onPatternSpawned?.Invoke();

#if UNITY_EDITOR     
        Debug.DrawLine(_spawnPoint, _spawnPoint + _currentDirection * SpawnRadius, Color.red);
        var dist = _currentDistance + SpawnRadius;
        Debug.DrawLine(Vector3.zero, _currentDirection * dist, Color.blue);
#endif
        
        ClearUnusedObjects(_spawnedObjects, ClearRadius);
    }

    public void SetNewPattern(PatternObjectInfo info)
    {
        if (_currentPattern == null)
            _spawnPoint = _currentDirection * (_currentDistance + GetPatternStep(info));
        else
            _spawnPoint += _currentDirection * (GetPatternStep(info) + GetPatternStep(_currentPattern));
        
        _currentPattern = info;
    }

    float GetPatternStep(PatternObjectInfo info)
    {
        return new Vector3(info.Size.x * _currentDirection.x, info.Size.y * _currentDirection.y, info.Size.z * _currentDirection.z).magnitude / 2;
    }

    void SpawnPattern()
    {
        if (_currentPattern == null)
            return; 

        for (var i = 0; i < _currentPattern.Childs.Count; i++)
        {
            var dist = _currentDistance + SpawnRadius;
            var point = _spawnPoint + _currentPattern.Childs[i].LocalPosition;

            if (!(Vector3.Distance(Vector3.zero, _currentDirection * dist) >
                  Vector3.Distance(point, Vector3.zero))) continue;
            
            var obj = SpawnPatternObject(_currentPattern.Childs[i],
                _spawnPoint + _currentPattern.Childs[i].LocalPosition);

            if (obj != null && !obj.SelfDestroy)
                _spawnedObjects.Add(obj);

            _currentPattern.Childs.RemoveAt(i);
        }
    }

    PoolObject SpawnPatternObject(ChildInfo info, Vector3 pos)
    {
        var type = ObjectType.Group;

        if (!Enum.TryParse(info.ObjectName, out type))
            return null;
            
        var obj = _pool.GetObjectOfType<PoolObject>(type);
            
        if (obj == null)
        {
            Debug.LogError("There is no " + info.ObjectName + " object in pool!");

            return null;
        }
        
        obj.transform.position = pos;


        if (type == ObjectType.Group)
        {
            var group = (obj as GroupPoolObject);

            if (group != null)
                group.Init(_pool, _currentColor);
        }

        obj.AcceptSettings(info.Settings);
        
        return obj;
    }
    
    void ClearUnusedObjects(HashSet<PoolObject> targetList, float checkDistance)
    {
        _excludedObjects.Clear();
        var dist = _currentDistance - ClearRadius;
        
#if UNITY_EDITOR  
        Debug.DrawLine(Vector3.zero, _currentDirection * dist, Color.cyan);
#endif
        
        foreach (var obj in targetList)
        {
            var point = obj.transform.position;
            
            Vector3 toTarget = (point - _currentDirection * dist).normalized;

            if (!(Vector3.Dot(toTarget, _currentDirection) < 0)) continue;
            
            if (!(Vector3.Distance(Vector3.zero, _currentDirection * dist) >
                  Vector3.Distance(point, Vector3.zero))) continue;
                
            obj.Destroy();
            _excludedObjects.Add(obj);
        }

        targetList.ExceptWith(_excludedObjects);
    }
}
