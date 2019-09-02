using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuScreen : MenuScreen
{
    [SerializeField] Button _play;
    [SerializeField] Button _settings;
    
    Action _onPlay;
    Action _onSettings;

    void Awake()
    {
        _play.onClick.AddListener(OnPlay);
        _settings.onClick.AddListener(OnSettings);
    }

    public void Init(Action onPlay, Action onSettings)
    {
        _onPlay = onPlay;
        _onSettings = onSettings;
    }

    void OnPlay()
    {
        PlayHideAnimation(_onPlay);
    }

    void OnSettings()
    {
        _onSettings?.Invoke();
    }
}
