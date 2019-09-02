using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Visartech.Progress;

public class SettingsPopup : Popup
{
    [SerializeField] Button _back;    
    [SerializeField] Toggle _on;

    Action<bool> _onChange;

    protected override void Awake()
    {
        base.Awake();
        
       
       _back.onClick.AddListener(Back);
    }

    void OnEnable()
    {
        _on.isOn = Progress.Game.MusicEnabled;
        _on.onValueChanged.AddListener(OnMusicSwitch);
        

    }

    public void Init(Action<bool> onChange)
    {
        _onChange = onChange;
    }

    void OnMusicSwitch(bool isOn)
    {
        _onChange?.Invoke(isOn);
    }

    void Back()
    {
        _on.onValueChanged.RemoveAllListeners();
        
        PlayHideAnimation(() => SetActive(false));
    }
}
