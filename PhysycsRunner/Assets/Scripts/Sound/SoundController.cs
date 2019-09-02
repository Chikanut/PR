using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;
using Visartech.Progress;

public class SoundController : MonoBehaviour {

    public const float _maxSoundDistance = 33;

    [SerializeField] private AudioSource _effectsSource;
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioMixer _masterMixer;

    private SoundsConfig _soundsConfig;
    public static SoundController Instance = null;

    private AudioClipType _currentMusicClip;

    public AudioClipType CurrentMusicClip => _currentMusicClip;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else if (Instance != this) {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    public void Init(SoundsConfig soundsConfig) {
        _soundsConfig = soundsConfig;
        
        _masterMixer.SetFloat("EffectsVolume", Progress.Game.SoundEnabled ? 0f : -80f);

        MusicEnabled = !MusicEnabled;
        MusicEnabled = !MusicEnabled;
    }

    public bool SoundEnabled {
        get { return Progress.Game.SoundEnabled; }
        set {
            if(Progress.Game.SoundEnabled == value) return;
            _masterMixer.SetFloat("EffectsVolume", value ? 0f : -80f);
            Progress.Game.SoundEnabled = value;
        }
    }

    public bool MusicEnabled {
        get { return Progress.Game.MusicEnabled; }
        set {
            if (Progress.Game.MusicEnabled == value) return;
            _masterMixer.DOSetFloat("MusicVolume", value ? 0f : -80f, 0.5f).SetEase(Ease.OutCubic);
            Progress.Game.MusicEnabled = value;
           
        }
    }
    
    public void SetMusicLowpassCutoff(bool isActive) {
        _masterMixer.DOSetFloat("LowpassCutoff", isActive ? 1400 : 22000, 0.3f).SetEase(Ease.OutCubic);
    }

    public void PlayAudioClip(AudioClipType clipType, bool is3D, Vector3 position = default(Vector3)) {
        if(!SoundEnabled) return;
        var clip = _soundsConfig.GetClip(clipType);
        if (clip == null) {
            Debug.LogError("Audio clip for " + clipType + " was not found");
            return;
        }
        if (is3D) {
            CreateAndPlay3DAudioClip(clip, position);
        }
        else {
            CreateAndPlay2DAudioClip(clip);
        }
    }

    private void CreateAndPlay3DAudioClip(AudioClipConfig clipConfig, Vector3 position) {
        var audioSource = new GameObject("3D Audio Source").AddComponent<AudioSource>();
        audioSource.transform.position = position;
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialize = true;
        audioSource.clip = clipConfig.Clip;
        audioSource.volume = clipConfig.Volume;
        audioSource.spatialBlend = 1;
        audioSource.maxDistance = _maxSoundDistance;
        audioSource.outputAudioMixerGroup = clipConfig.OutputAudioMixerGroup;
        audioSource.Play();
        Destroy(audioSource.gameObject, clipConfig.Clip.length);
    }
    
    private void CreateAndPlay2DAudioClip(AudioClipConfig clipConfig) {
        var audioSource = new GameObject("2D Audio Source").AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialize = false;
        audioSource.clip = clipConfig.Clip;
        audioSource.volume = clipConfig.Volume;
        audioSource.outputAudioMixerGroup = clipConfig.OutputAudioMixerGroup;
        audioSource.Play();
        Destroy(audioSource.gameObject, clipConfig.Clip.length);
    }
    
    public void PlayAudioClipDelayed(AudioClipType clipType, float delay, bool is3D, Vector3 position = default(Vector3)) {
        if(!SoundEnabled) return;
        var clip = _soundsConfig.GetClip(clipType);
        if (clip == null) {
            Debug.LogError("Audio clip for " + clipType + " was not found");
            return;
        }
        StartCoroutine(PlayAudioClipDelayed(clip, delay, is3D, position:position));
    }

    private IEnumerator PlayAudioClipDelayed(AudioClipConfig clipConfig, float delay, bool is3D, Vector3 position) {
        yield return new WaitForSeconds(delay);
        if (is3D) {
            CreateAndPlay3DAudioClip(clipConfig, position);
        }
        else {
            CreateAndPlay2DAudioClip(clipConfig);
        }
    }

    public void PlayMusic(AudioClipType clipType) {
        var clip = _soundsConfig.GetClip(clipType);
        if (clip == null) {
            Debug.LogError("Audio clip for " + clipType + " was not found");
            return;
        }
        if (_musicSource.clip == clip.Clip) {
            return;
        }
        _musicSource.Stop();
        _musicSource.clip = clip.Clip;
        _musicSource.volume = clip.Volume;
        _musicSource.outputAudioMixerGroup = clip.OutputAudioMixerGroup;
        _currentMusicClip = clipType;
        _musicSource.Play();
    }
}
