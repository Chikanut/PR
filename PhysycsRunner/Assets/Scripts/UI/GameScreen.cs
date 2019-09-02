using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameScreen : MenuScreen
{
    [SerializeField] TextMeshProUGUI _levelText;
    [SerializeField] float _textAppearTime;
    [SerializeField] float _textShowTime;
    [SerializeField] Ease _textShowEase;
    
    [SerializeField] Image _progress;

    Transform _target;
    float _startDistance;
    float _targetDistance;

    Sequence _sequence;
    
    public void Init(Transform target)
    {
        _target = target;
    }

    public void SetNewDistance(float distance)
    {       
        _targetDistance = distance;
        _startDistance = _target.position.z;
    }

    public void ShowLevel(int lvl)
    {
        _levelText.text = "Level " + lvl.ToString();
        _sequence = DOTween.Sequence();
        _sequence.Insert(0, _levelText.DOFade(1, _textAppearTime)).SetEase(_textShowEase);
        _sequence.Insert(_textShowTime + _textAppearTime, _levelText.DOFade(0, _textAppearTime)).SetEase(_textShowEase);
    }

    void Update()
    {
        _progress.fillAmount = (_target.position.z - _startDistance) / _targetDistance;

        if(_visualiseTouch)
            TouchVisualisation();
    }

    [SerializeField] bool _visualiseTouch;
    [SerializeField] RectTransform _startPoint;
    [SerializeField] RectTransform _endPoint;
    bool _pressed;
    bool _stretching;
    Vector2 _startPos;
    Vector2 _endPos;
    
    void TouchVisualisation()
    {
        if ((Input.GetMouseButtonDown(0) || Input.touchCount > 0) && !_stretching)
        {
            _endPos = _startPos = Input.touchCount == 0 ? (Vector2)Input.mousePosition : Input.GetTouch(0).position;
            _stretching = true;
            
            _startPoint.gameObject.SetActive(true);
            _endPoint.gameObject.SetActive(true);
            
            if (Input.touchCount > 0)
                _pressed = true;
        }

        if (_stretching)
        {
            _endPos = Input.touchCount == 0 ? (Vector2) Input.mousePosition : Input.GetTouch(0).position;

            _startPoint.position = _startPos;
            _endPoint.position = _endPos;
        }

        if (!_stretching || (!Input.GetMouseButtonUp(0) && (Input.touchCount != 0 || !_pressed))) return;
        
        _stretching = false;
        _pressed = false;
            
        _startPoint.gameObject.SetActive(false);
        _endPoint.gameObject.SetActive(false);
    }

    void OnDisable()
    {
        _sequence.Kill();
    }
}
