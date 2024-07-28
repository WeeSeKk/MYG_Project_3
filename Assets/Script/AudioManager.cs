using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEditor.Rendering;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField] AudioSource musicAudioSource;
    [SerializeField] AudioSource soundAudioSource;
    [SerializeField] List<AudioClip> clips;
    [SerializeField] AudioMixer audioMixer;
    bool paused;
    public float musicValue;
    public float soundValue;
    const string MUSIC_VOLUME = "MusicVolume";
    const string SFX_VOLUME = "SFXVolume";

    void Awake()
    {
        EventManager.buttonClicked += PlayAudioClip;
        EventManager.musicVolulmeChange += SetMusicVolume;
        EventManager.sfxVolulmeChange += SetSFXVolume;

        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        LoadVolumeValue();
    }

    void SetMusicVolume(float value)
    {
        musicValue = value;
        audioMixer.SetFloat(MUSIC_VOLUME, Mathf.Log10(value) * 20);
        SaveVolumeValue("Music", value);
    }

    void SetSFXVolume(float value)
    {
        soundValue = value;
        audioMixer.SetFloat(SFX_VOLUME, Mathf.Log10(value) * 20);
        SaveVolumeValue("SFX", value);
    }

    public void PlayAudioClip(int num)
    {
        soundAudioSource.clip = clips[num];
        soundAudioSource.Play();
    }

    public void PauseMusic()
    {
        if (paused)
        {
            musicAudioSource.Play();
            paused = false;
        }
        else
        {
            musicAudioSource.Pause();
            paused = true;
        }
    }

    void LoadVolumeValue()
    {
        float defaultVolume = 0.5f; 
        float musicVolume = IntToFloat(PlayerPrefs.GetInt(MUSIC_VOLUME, FloatToInt(defaultVolume)));
        float sfxVolume = IntToFloat(PlayerPrefs.GetInt(SFX_VOLUME, FloatToInt(defaultVolume)));
        
        SetMusicVolume(musicVolume);
        SetSFXVolume(sfxVolume);
    }

    void SaveVolumeValue(string type, float volume)
    {
        int volumeint = FloatToInt(volume);

        if (type == "Music")
        {
            PlayerPrefs.SetInt(MUSIC_VOLUME, volumeint);
        }
        if (type == "SFX")
        {
            PlayerPrefs.SetInt(SFX_VOLUME, volumeint);
        }
    }

    public float MusicSliderValue()
    {
        float defaultVolume = 0.5f; 
        return IntToFloat(PlayerPrefs.GetInt(MUSIC_VOLUME, FloatToInt(defaultVolume)));
    }

    public float SFXSliderValue()
    {
        float defaultVolume = 0.5f; 
        return IntToFloat(PlayerPrefs.GetInt(SFX_VOLUME, FloatToInt(defaultVolume)));
    }

    int FloatToInt(float value)
    {
        return (int)(value * 1000);
    }

    float IntToFloat(int value)
    {
        return value / 1000.0f;
    }
}