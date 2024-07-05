using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;



public class APIManager : MonoBehaviour
{
    public Word[] words;
    public bool isValid;

    void Start()
    {
        //System.Action<bool> callback = null
    }

    public IEnumerator SendRequest(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError) 
            {
                Debug.Log("Error: " + webRequest.error);//can be words not found too
                isValid = false;
            }
            else
            {
                Debug.Log(":\nReceived: " + webRequest.downloadHandler.text);
                words = JsonConvert.DeserializeObject<Word[]>(webRequest.downloadHandler.text);
                isValid = true;
            }
        }
    }
}

[System.Serializable]
public class Word
{
    public string definitions;
    public string word;
}