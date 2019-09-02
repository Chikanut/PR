using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class UiPanel : MonoBehaviour {

    [SerializeField] private UiComponent[] _uiComponents;

    public float TransitionTime { get; private set; }

    private UiComponent _componentWithLongestTransition;
    
    private void Awake() {
        TransitionTime = _uiComponents.Max(c => c.TransitionTime);
        _componentWithLongestTransition = _uiComponents.First(c => c.TransitionTime == TransitionTime);
    }
    
    public void ShowUiComponents(Action onFinish = null) {
        foreach (var component in _uiComponents) {
            var action = component == _componentWithLongestTransition ? onFinish : null;
            component.Show(action);
        }
    }

    public void HideUiComponents(Action onFinish = null) {        
        foreach (var component in _uiComponents) {
            var action = component == _componentWithLongestTransition ? onFinish : null;
            component.Hide(action);
        }
    }
}
