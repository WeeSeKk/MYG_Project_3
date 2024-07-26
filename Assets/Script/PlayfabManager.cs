using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEditor.PackageManager;
using PlayFab.DataModels;

public class PlayfabManager : MonoBehaviour
{
    IntroUIManager introUIManager;
    LobbyUIManager lobbyUIManager;
    public static PlayfabManager instance;
    string playerUsername = null;
    string category;

    void Awake()
    {
        introUIManager = GameObject.Find("UIDocument").GetComponent<IntroUIManager>();

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

    public string Player_Username()
    {
        return playerUsername;
    }

    void OnError(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
        if (introUIManager != null)
        {
            StartCoroutine(introUIManager.ShowError(error.ErrorMessage));
            introUIManager.HideWaitingScreen();
        }
    }

    void UpdateUsername(string username)
    {
        var request = new UpdateUserTitleDisplayNameRequest {
            DisplayName = username
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameUpdate, OnError);
    }

    void OnDisplayNameUpdate(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log("Username Changed");
    }

    public void OnRegister(string username, string password)
    {
        var request = new RegisterPlayFabUserRequest {
            Username = username,
            Password = password,
            RequireBothUsernameAndEmail = false
        };

        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSucces, OnError);
    }

    void OnRegisterSucces(RegisterPlayFabUserResult result)
    {
        Debug.Log("Register Success" + result);
        playerUsername = result.Username;
        UpdateUsername(playerUsername);
        introUIManager.HideWaitingScreen();
        StartCoroutine(introUIManager.ShowError("Register Completed"));
    }

    public void OnLogin(string username, string password)
    {
        var request = new LoginWithPlayFabRequest {
            Username = username,
            Password = password,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams {
                GetPlayerProfile = true
            }
        };

        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSucces, OnError);
    }

    void OnLoginSucces(LoginResult result)
    {
        Debug.Log("Login Success");
        if (playerUsername == null)
        {
            playerUsername = result.InfoResultPayload.PlayerProfile.DisplayName;
        }
        GameManager.instance.LaunchLobby();
    }

    public void SendLeaderboard(int score)
    {
        var request = new UpdatePlayerStatisticsRequest {
            Statistics = new List<StatisticUpdate> {
                new StatisticUpdate {
                    StatisticName = "High Score",
                    Value = score
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderbordUpdate, OnError);
    }

    void OnLeaderbordUpdate(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("leaderbord updated");
    }

    public void GetLeaderboard()
    {
        var request = new GetLeaderboardRequest {
            StatisticName = "High Score",
            StartPosition = 0,
            MaxResultsCount = 10
        };
        PlayFabClientAPI.GetLeaderboard(request, OnleaderbordGet, OnError);
    }

    void OnleaderbordGet(GetLeaderboardResult result)
    {
        lobbyUIManager = GameObject.Find("UIDocument").GetComponent<LobbyUIManager>();
        foreach (var item in result.Leaderboard)
        {
            lobbyUIManager.AddLeaderboardList(item.DisplayName + " " + item.StatValue);
        }
    }

    public void GetCategory()
    {
        PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(), OnCategoryGet, OnError);
    }

    void OnCategoryGet(GetTitleDataResult result)
    {
        if (result.Data == null || result.Data.ContainsKey("animals") == false)
        {
            Debug.LogError("FUCK");
        }

        category = result.Data["animals"];
    }

    public string Category()
    {
        return category;
    }
}