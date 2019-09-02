using System;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public abstract class MenuScreen : Showable
{

    [SerializeField] private UiPanel _panel;
    protected RectTransform RectTransform;

    protected virtual void Awake() {
        RectTransform = GetComponent<RectTransform>();
    }

    
    public virtual void PlayShowAnimation(Action onFinish) {
        if(_panel != null)
           _panel.ShowUiComponents(onFinish);
        else
            onFinish?.Invoke();
    }

    public virtual void PlayHideAnimation(Action onFinish) {
        if(_panel != null)
            _panel.HideUiComponents(onFinish);
        else
            onFinish?.Invoke();
    }

}
