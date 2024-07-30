using System;
using System.Threading.Tasks;
using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class APIManager : MonoBehaviour
{
    public Word[] words;
    public delegate void Callback(bool isValid);

    public async Task SendRequestAsync(string url, Callback isValidCallback)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            var operation = webRequest.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();
            }
                
            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                isValidCallback?.Invoke(false);
            }
            else
            {
                words = JsonConvert.DeserializeObject<Word[]>(webRequest.downloadHandler.text);
                isValidCallback?.Invoke(true);
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