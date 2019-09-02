using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public abstract class Popup : Showable {

    protected RectTransform RectTransform;
    protected RectTransform MainPanel;
    protected Image Background; //Andy Removed all background animations

    private Vector3 _shownPosition;
    protected float BackgroundShownAlpha;
    
    protected virtual void Awake() {
        RectTransform = GetComponent<RectTransform>();
        MainPanel = transform.GetChild(0).GetComponent<RectTransform>();
        Background = GetComponent<Image>();
        _shownPosition = MainPanel.anchoredPosition;
        BackgroundShownAlpha = Background.color.a;
    }
      
    public virtual void PlayShowAnimation(Action onFinish) {
        Background.color = new Color(Background.color.r, Background.color.g, Background.color.b, 0f);
        MainPanel.localPosition = new Vector3(MainPanel.localPosition.x, -RectTransform.rect.height, MainPanel.localPosition.z);
        var sequence = DOTween.Sequence();
        sequence.Append(DOTween
            .To(() => MainPanel.anchoredPosition3D, p => MainPanel.anchoredPosition3D = p, _shownPosition, 0.2f)
            .SetEase(Ease.OutCubic));
        sequence.Join(Background.DOFade(BackgroundShownAlpha, 0.3f).SetEase(Ease.OutCubic));
        sequence.onComplete = ()=> {
            onFinish?.Invoke();
        };
    }

    public virtual void PlayHideAnimation(Action onFinish) {
        var sequence = DOTween.Sequence();
        sequence.Append(MainPanel.DOLocalMove(
                new Vector3(RectTransform.localPosition.x, -RectTransform.rect.height, RectTransform.localPosition.z),
                0.3f).SetEase(Ease.OutCubic));
        sequence.Join(Background.DOFade(0f, 0.3f).SetEase(Ease.OutCubic));
        sequence.onComplete = () => {
            onFinish?.Invoke();
        };
    }
}
