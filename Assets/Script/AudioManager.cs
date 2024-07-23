using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    ObjectPool<GameObject> audioSound;
    LobbyUIManager lobbyUIManager;
    UIManager uIManager;
    [SerializeField] AudioSource musicAudioSource;
    [SerializeField] AudioSource soundAudioSource;

    void Awake()
    {
        lobbyUIManager = GameObject.Find("UIDocument").GetComponent<LobbyUIManager>();

        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
        CreateAudioSoundObjectPool();
    }

    public void ChangeUIManager()
    {
        //on scene load change ui manager for settings
        //called from GameManager
    }

    public void TestSoundEffect()
    {
        soundAudioSource.Play();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if scene lobby
        musicAudioSource.volume = lobbyUIManager.musicSlider.value;
        //scene gameplay slider value == scene lobby slider value
        //or if scene gameplay
    }

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
}