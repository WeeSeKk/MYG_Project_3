using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LobbyUIManager : MonoBehaviour
{
    [SerializeField] VisualTreeAsset elementList;
    VisualElement root;
    VisualElement settingsTab;
    public Slider musicSlider;
    public Slider audioSlider;
    Button singleplayerButton;
    Button multiplayerButton;
    Button settingsButton;
    Button returnButton;
    Label connectingLabel;
    Label playerUsername;
    ListView leaderbordList;
    List<string> leaderbord = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        settingsTab = root.Q<VisualElement>("SettingsTab");
        singleplayerButton = root.Q<Button>("SingleplayerButton");
        multiplayerButton = root.Q<Button>("MultiplayerButton");
        connectingLabel = root.Q<Label>("ConnectingLabel");
        settingsButton = root.Q<Button>("SettingsButton");
        returnButton = root.Q<Button>("ReturnButton");
        musicSlider = root.Q<Slider>("MusicSlider");
        audioSlider = root.Q<Slider>("AudioSlider");
        playerUsername = root.Q<Label>("PlayerUsernameLabel");
        leaderbordList = root.Q<ListView>("LeaderbordList");

        ShowUsername();

        singleplayerButton.RegisterCallback<ClickEvent>(evt => {
            ConnectingScreen();
            EventManager.ButtonClicked(0);
        });
        settingsButton.RegisterCallback<ClickEvent>(evt => {
            OpenCloseSettings(settingsButton);
            EventManager.ButtonClicked(0);
        });
        returnButton.RegisterCallback<ClickEvent>(evt => {
            OpenCloseSettings(returnButton);
            EventManager.ButtonClicked(0);
        });
        audioSlider.RegisterCallback<ClickEvent>(evt => {
            EventManager.ButtonClicked(0);
        });

        musicSlider.value = 0.05f;
        audioSlider.value = 0.5f;
    }

    void ConnectingScreen()
    {
        singleplayerButton.style.opacity = 0;
        multiplayerButton.style.opacity = 0;
        connectingLabel.style.opacity = 100;
        GameManager.instance.LaunchGamemode_1();
    }

    public void ShowUsername()
    {
        playerUsername.text = PlayfabManager.instance.Player_Username();
    }

    void OpenCloseSettings(Button button)
    {
        if (button == settingsButton)
        {
           settingsTab.RemoveFromClassList("SettingsTabHidden");
        }
        else
        {
            settingsTab.AddToClassList("SettingsTabHidden");
        }
    }

    void CallLeaderboard()
    {
        PlayfabManager.instance.GetLeaderboard();
    }

    public void AddLeaderboardList(string info)//add the found word to the list 
    {
        if (!leaderbord.Contains(info))
        {
            leaderbord.Add(info);
        }

        UpdateLeaderboardList();
    }

    void UpdateLeaderboardList()
    {
        leaderbordList.Clear();
        leaderbordList.itemsSource = leaderbord;
        leaderbordList.makeItem = () => elementList.CloneTree();
        leaderbordList.bindItem = (element, index) =>
        {
            var label = element.Q<Label>();
            label.text = $"{leaderbord[index]}";
        };
        leaderbordList.fixedItemHeight = 60;
        leaderbordList.Rebuild();
    }
}