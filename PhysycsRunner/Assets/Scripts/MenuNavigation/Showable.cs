using UnityEngine;

public abstract class Showable : MonoBehaviour {
    public virtual void SetActive(bool isActive) {
        gameObject.SetActive(isActive);
    }

    public virtual void OnButtonClicked() {
    }

    public virtual bool IsActive => gameObject.activeSelf;

    public virtual int RootIndex => transform.GetSiblingIndex();
}