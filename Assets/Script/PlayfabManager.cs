namespace PlayfabManagerNamespace
{
    using System.Collections;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using UnityEngine;
    using PlayFab;
    using PlayFab.ClientModels;
    using PlayFab.DataModels;
    using GameManagerNamespace;
    using WordsManagerNamespace;

    public class PlayfabManager : MonoBehaviour
    {
        IntroUIManager introUIManager;
        LobbyUIManager lobbyUIManager;
        UIManager uIManager;
        WordsManager wordsManager;
        public static PlayfabManager instance;
        string playerUsername = null;

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
        /**
        <summary>
        Generate an error report if any of the PlayfabManager fonctions fail.
        </summary>
        <param name="error">Error report generate by a PlayfabManager fonction</param>
        <returns></returns>
        **/
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
            var request = new UpdateUserTitleDisplayNameRequest
            {
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
            var request = new RegisterPlayFabUserRequest
            {
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
            var request = new LoginWithPlayFabRequest
            {
                Username = username,
                Password = password,
                InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
                {
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
            var request = new UpdatePlayerStatisticsRequest
            {
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
            var request = new GetLeaderboardRequest
            {
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
        /**
        <summary>
        Get a string from playfab with different words depending on the category chosen.
        </summary>
        <param name="key">Key word to find the category choosen</param>
        <returns></returns>
        **/
        public async Task GetCategoryAsync(string key)
        {
            var tsk = new TaskCompletionSource<string>();

            PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(), result =>
            {
                string category = result.Data[key];
                tsk.SetResult(category);

            }, OnError);

            string categoryResult = await tsk.Task;

            wordsManager = GameObject.Find("WordsManager").GetComponent<WordsManager>();
            wordsManager.AddWordsToCategoryList(categoryResult);

            uIManager = GameObject.Find("UIDocument").GetComponent<UIManager>();
            uIManager.SetCategoryLabel(key);

            Debug.Log(categoryResult);
        }

        public void Logout()
        {
            PlayFabClientAPI.ForgetAllCredentials();
        }
    }
}