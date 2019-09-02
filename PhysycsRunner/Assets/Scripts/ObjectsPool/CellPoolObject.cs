using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CellPoolObject : PoolObject
{
    [SerializeField] private MeshRenderer _renderer;
    [SerializeField] private Material _material;

    Rigidbody _body;

    public Rigidbody Body
    {
        get
        {
            if (_body == null)
                _body = GetComponent<Rigidbody>();
            
            return _body;
        }
    }
    
    bool _isVisible = false;

    Color _color;

    GroupPoolObject _targetGroup;
    
    void OnEnable()
    {
        transform.rotation = Quaternion.identity;
        Body.velocity = Vector3.zero;
        
        var end = transform.localScale;
            
        transform.localScale = Vector3.zero;
        transform.DOScale(end, 1);
    }

    void OnBecameVisible()
    {
        _isVisible = true;
    }

    void OnBecameInvisible()
    {
        _isVisible = false;
        
        if(_targetGroup == null)
            Destroy();
    }

    public void Join(GroupPoolObject group)
    {
        _targetGroup = group;
    }

    public void DisJoin()
    {
        _targetGroup = null;
        
        if(!_isVisible)
            Destroy();
    }

    public void SetColor(Color c)
    {
        if (!Application.isPlaying)
        {
            var tempMaterial = new Material(_material) {color = c};
            _renderer.sharedMaterial = tempMaterial;
        }
        else
            _renderer.material.color = c;

        _color = c;
    }

    public bool OnSameColor(Color c)
    {
        return c == _color;
    }

    public override void ResetState()
    {
        _targetGroup = null;
    }
}
