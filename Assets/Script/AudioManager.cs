using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using DG.Tweening;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    ObjectPool<GameObject> audioSound;
    LobbyUIManager lobbyUIManager;
    UIManager uIManager;
    [SerializeField] AudioSource musicAudioSource;
    [SerializeField] AudioSource soundAudioSource;
    [SerializeField] List<AudioClip> clips;
    bool paused;
    public float musicValue;
    public float soundValue;
    int scene;

    void Awake()
    {
        EventManager.buttonClicked += PlayAudioClip;
        scene = -1;

        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
        //CreateAudioSoundObjectPool();
    }

    public void ChangeUIManager(int num)
    {
        //on scene load change ui manager for settings
        //called from GameManager
        if (num == 1)
        {
            uIManager = GameObject.Find("UIDocument").GetComponent<UIManager>();
            scene = 1;
        }
        else
        {
            lobbyUIManager = GameObject.Find("UIDocument").GetComponent<LobbyUIManager>();
            scene = 0;
        }
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
            //slider value = musicValue
            musicAudioSource.Play();
            paused = false;
            
        }
        else
        {
            //musicValue = slider value
            //go to 0
            musicAudioSource.Pause();
            paused = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (scene == 0 && lobbyUIManager != null)
        {
            musicAudioSource.volume = lobbyUIManager.musicSlider.value;
            musicValue = musicAudioSource.volume;



            soundAudioSource.volume = lobbyUIManager.audioSlider.value;
            soundValue = soundAudioSource.volume;
        }
        else if (scene == 1 && uIManager != null)
        {
            musicAudioSource.volume = uIManager.musicSlider.value;
            musicValue = musicAudioSource.volume;



            soundAudioSource.volume = uIManager.audioSlider.value;
            soundValue = soundAudioSource.volume;
        }
    }
    /*
    public void CreateAudioSoundObjectPool() {
        audioSound = new ObjectPool<GameObject>(() => {
            return CreateAudioSound();                      //Creation Function
        }, audioSound => {
            audioSound.SetActive(true);                     //On Get
        }, audioSound => {
            audioSound.SetActive(false);                    //On Release
        }, audioSound => {
            Destroy(audioSound);                            //On Destroy
        }, false,                                           //Check Collection
        5,                                                  //Initial Array Size (to avoid recreations)
        20                                                  //Max Array Size
        );
    }

    GameObject CreateAudioSound()
    {
        return Instantiate(GameManager.instance.GenerateBox());
    }

    public GameObject GetAudioSound()
    {
        return audioSound.Get();
    }

    public void ReleaseAudioSound(GameObject go)
    {
        audioSound.Release(go);
    }
    */
}