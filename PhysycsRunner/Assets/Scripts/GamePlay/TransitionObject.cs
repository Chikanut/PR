using DG.Tweening;
using UnityEngine;

public class TransitionObject : PoolObject
{
    [SerializeField] LayerMask _playerLayer;
    [SerializeField] Vector3 _checkZoneSize;

    Renderer[] _renderers;
    bool _activated;

    void Awake()
    {
        _renderers = GetComponentsInChildren<Renderer>();
    }

    void OnEnable()
    {
        Show();
    }

    public void Update()
    {       
        var _colliders = Physics.OverlapBox(transform.position, _checkZoneSize, Quaternion.identity, _playerLayer);

        if (_colliders.Length <= 0) return;
        
        foreach (var coll in _colliders)
            CheckCollider(coll);
    }
    
    void CheckCollider(Collider collision)
    {
        if (_playerLayer == (_playerLayer | (1 << collision.gameObject.layer)))
        {
            var player = collision.gameObject.GetComponent<Player>();
            if (player)
                player.SetColor(GameManager.Instance.CurrentColors.LightColor);
            
            Activate();
        }
        else
        {
            var cell = collision.gameObject.GetComponent<CellPoolObject>();
            if (cell)
                cell.SetColor(GameManager.Instance.CurrentColors.LightColor);
        }
    }

    void Show()
    {
        for (int i = 0; i < _renderers.Length; i++)
        {
            _renderers[i].material.color = GameManager.Instance.CurrentColors.LightColor;
        }
        
        var end = transform.localScale.x;
            
        transform.localScale = Vector3.zero;
        transform.DOScale(end, 1);
    }

    void Activate()
    {
        if(_activated)
            return;

        GameManager.Instance.OnFinished();
            
        _activated = true;
    }
    
    public override void ResetState()
    {
        _activated = false;
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.black;

        Gizmos.DrawWireCube(transform.position, _checkZoneSize);
    }
}
