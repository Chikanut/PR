using System;
using DG.Tweening;
using UnityEngine;

public interface IUiComponent {
    void Show(Action onFinish);
    void Hide(Action onFinish);
}

public class UiComponent : MonoBehaviour, IUiComponent {

//    [SerializeField] private RectTransform _transform;
    [SerializeField] private CanvasGroup _group;
    [SerializeField] private float _transitionTime;
    [SerializeField] private Ease _ease;

    public bool isHidden
    {
        get
        {
            return _group.alpha == 0;
        }
    }

    public float TransitionTime => _transitionTime;
    
    public virtual void Show(Action onFinish = null) {
        var sequence = DOTween.Sequence();
        sequence.Insert(0, _group.DOFade(1, _transitionTime)).SetEase(_ease);
        sequence.onComplete = () => {
            onFinish?.Invoke();
        };
    }

    public virtual void Hide(Action onFinish = null) {
        var sequence = DOTween.Sequence();
        sequence.Insert(0, _group.DOFade(0, _transitionTime)).SetEase(_ease);;
        sequence.onComplete = () => {
            onFinish?.Invoke();
        };
    }
}
