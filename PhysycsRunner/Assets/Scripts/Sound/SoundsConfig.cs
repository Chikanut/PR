using System;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "SoundsConfig", menuName = "Configs/SoundsConfig")]
public class SoundsConfig : ScriptableObject {
    public AudioTypeAudioClipDictionary ClipsDictionary;

    public AudioClipConfig GetClip(AudioClipType type)
    {
        return ClipsDictionary[type.ToString()];
    }

    public IEnumerable<string> GetAvailableNames()
    {
        return ClipsDictionary.Keys.AsEnumerable();
    }
}

[Serializable]
public class AudioTypeAudioClipDictionary : SerializableDictionaryBase<string, AudioClipConfig> { }

[Serializable]
public class AudioClipConfig
{
    public AudioClip Clip => Clips.Length < 2 ? Clips[0] : Clips[Random.Range(0, Clips.Length)];
    public AudioClip[] Clips;
    [Range(0, 1)]
    public float Volume;
    public AudioMixerGroup OutputAudioMixerGroup;
}