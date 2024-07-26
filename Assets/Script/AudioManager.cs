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

        LoadVolumeValue();

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

    void Update()
    {

    }

    void SetMusicVolume(float value)
    {
        musicValue = value;
        audioMixer.SetFloat(MUSIC_VOLUME, Mathf.Log10(value) * 20);
    }

    void SetSFXVolume(float value)
    {
        soundValue = value;
        audioMixer.SetFloat(SFX_VOLUME, Mathf.Log10(value) * 20);
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
        SetMusicVolume(PlayerPrefs.GetFloat(MUSIC_VOLUME));
        SetSFXVolume(PlayerPrefs.GetFloat(SFX_VOLUME));
    }

    void SaveVolumeValue(string type, float volume)
    {
        if (type == "Music")
        {
            Debug.Log("test");
            //PlayerPrefs.SetFloat(MUSIC_VOLUME, volume);
            PlayerPrefs.SetInt(MUSIC_VOLUME, 10);
        }
        else if (type == "SFX")
        {
            //PlayerPrefs.SetFloat(SFX_VOLUME, volume);
            PlayerPrefs.SetInt(MUSIC_VOLUME, 10);
        }
    }
}