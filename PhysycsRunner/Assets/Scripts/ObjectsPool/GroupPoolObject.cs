using System.Collections.Generic;
using System.Linq;
using Helpers;
using UnityEngine;

public class GroupPoolObject : PoolObject
{
    [System.Serializable]
    public class CellCoordinate
    {
        public ObjectType CellType;
        public Vector3 Position;
    }

    [SerializeField] private Color _color = Color.gray;
    [SerializeField] private bool _playerPass;
    public Color Color
    {
        get { return _color; }
        set
        {
            _color = value;
        }
    }

    LevelColors _currentCollors;

    public bool PlayerPass
    {
        get { return _playerPass; }
        set { _playerPass = value; }
    }

    [SerializeField] List<CellCoordinate> _cellsCoordinates = new List<CellCoordinate>();
    [SerializeField] List<CellPoolObject> _cellsObjects = new List<CellPoolObject>();

    ObjectsPool _pool;
    
    public List<CellCoordinate> CellsCoordinates => _cellsCoordinates;

    [System.Serializable]
    public class GroupInfo : PoolObjectInfo
    {
        public CellCoordinate[] CellsCoordinates;
        public bool PlayerPass;
    }

    public override string SerializeSettings()
    {
        var groupInfo = new GroupInfo()
        {
            CellsCoordinates = _cellsCoordinates.ToArray(),
            PlayerPass = _playerPass
        };

        return XMLHelper.Serialize<GroupInfo>(groupInfo);
    }
    
    public override void AcceptSettings(string info)
    {
        var groupInfo = XMLHelper.Deserialize<GroupInfo>(info);

        _cellsCoordinates = groupInfo.CellsCoordinates.ToList();
        _playerPass = groupInfo.PlayerPass;
        
        SpawnCells();
    }

    public void Init(ObjectsPool pool, LevelColors c)
    {
        _currentCollors = c;
        _pool = pool;
    }

    void SpawnCells()
    {
        if (_pool == null)
        {
            Debug.LogError("Group " + gameObject.name + " dont have pool access!");
            return;
        }

        for (var i = 0; i < _cellsCoordinates.Count; i++)
        {           
            var obj = _pool.GetObjectOfType<CellPoolObject>(_cellsCoordinates[i].CellType);
            obj.transform.SetParent(transform);
            obj.transform.localPosition = _cellsCoordinates[i].Position;
            Color = PlayerPass ? _currentCollors.LightColor : _currentCollors.DarkColor;
            obj.SetColor(Color);
            
            _cellsObjects.Add(obj);
            
            obj.Join(this);
        }
    }

    public void AddCell(Vector3 pos, ObjectType type)
    {
        for (var i = 0; i < _cellsCoordinates.Count; i++)
        {
            if(_cellsCoordinates[i].Position == pos)
                return;
        }
        
        _cellsCoordinates.Add(new CellCoordinate()
        {
            CellType = type,
            Position = pos
        });
    }

    public void RemoveCell(Vector3 pos)
    {
        for (var i = 0; i < _cellsCoordinates.Count; i++)
        {
            if (_cellsCoordinates[i].Position != pos)
                continue;
            
            _cellsCoordinates.RemoveAt(i);
            
            break;
        }          
    }

    public override void ResetState()
    {
        for (int i = 0; i < _cellsObjects.Count; i++)
        {
            _cellsObjects[i].DisJoin();
            _cellsObjects[i].transform.SetParent(transform.parent);
        }
        
        _cellsObjects.Clear();
    }
}
