using System;
using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class APIManager : MonoBehaviour
{
    public Word[] words;
    public delegate void Callback(bool isValid);

    public IEnumerator SendRequest(string url, Callback isValidCallback)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError) //word found
            {
                Debug.Log("Error: " + webRequest.error);

                if (isValidCallback != null)
                {
                    isValidCallback(false);
                }
            }
            else//word not found
            {
                Debug.Log(":\nReceived: " + webRequest.downloadHandler.text);
                words = JsonConvert.DeserializeObject<Word[]>(webRequest.downloadHandler.text);

                if (isValidCallback != null)
                {
                    isValidCallback(true);
                }
            }
        }
    }
}

[Serializable]
public class Word
{
    public string definitions;
    public string word;
}