using System.Collections;
using System.Collections.Generic;
using UnityEditor.DeviceSimulation;
using UnityEngine;
using UnityEngine.UIElements;

public class LobbyUIManager : MonoBehaviour
{
    [SerializeField] VisualTreeAsset elementList;
    VisualElement root;
    VisualElement settingsTab;
    VisualElement uiHolder;
    VisualElement categoryList;
    Slider musicSlider;
    Slider audioSlider;
    Button singleplayerButton;
    Button multiplayerButton;
    Button settingsButton;
    Button returnButton;
    Button animalsButton;
    Button countryButton;
    Button leg_FruitButton;
    Button capitalesButton;
    Button flowersButton;
    Button brandButton;
    Label connectingLabel;
    Label playerUsername;
    ListView leaderbordList;
    List<string> leaderbord = new List<string>();

    public float music;
    public float sfx;

    // Start is called before the first frame update
    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        settingsTab = root.Q<VisualElement>("SettingsTab");
        uiHolder = root.Q<VisualElement>("UIHolder");
        categoryList = root.Q<VisualElement>("CategoryList");
        singleplayerButton = root.Q<Button>("SingleplayerButton");
        multiplayerButton = root.Q<Button>("MultiplayerButton");
        connectingLabel = root.Q<Label>("ConnectingLabel");
        settingsButton = root.Q<Button>("SettingsButton");
        returnButton = root.Q<Button>("ReturnButton");
        musicSlider = root.Q<Slider>("MusicSlider");
        audioSlider = root.Q<Slider>("AudioSlider");
        playerUsername = root.Q<Label>("PlayerUsernameLabel");
        leaderbordList = root.Q<ListView>("LeaderbordList");

        animalsButton = root.Q<Button>("AnimalsButton");
        countryButton = root.Q<Button>("CountryButton");
        leg_FruitButton = root.Q<Button>("Fruit_LegumeButton");
        capitalesButton = root.Q<Button>("CapitalesButton");
        flowersButton = root.Q<Button>("FlowersButton");
        brandButton = root.Q<Button>("BrandsButton");

        ShowUsername();
        HideCategoryList();

        singleplayerButton.RegisterCallback<ClickEvent>(evt => {
            GameManager.instance.LaunchGamemode_1();
            EventManager.ButtonClicked(0);
        });
        multiplayerButton.RegisterCallback<ClickEvent>(evt => {
            ShowCategoryList();
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
        animalsButton.RegisterCallback<ClickEvent>(evt => {
            GameManager.instance.LaunchGamemode_2("animals");
            EventManager.ButtonClicked(0);
        });
        countryButton.RegisterCallback<ClickEvent>(evt => {
            GameManager.instance.LaunchGamemode_2("country");
            EventManager.ButtonClicked(0);
        });
        leg_FruitButton.RegisterCallback<ClickEvent>(evt => {
            GameManager.instance.LaunchGamemode_2("vegetables_fruits");
            EventManager.ButtonClicked(0);
        });
        capitalesButton.RegisterCallback<ClickEvent>(evt => {
            GameManager.instance.LaunchGamemode_2("capitales");
            EventManager.ButtonClicked(0);
        });
        flowersButton.RegisterCallback<ClickEvent>(evt => {
            GameManager.instance.LaunchGamemode_2("flowers");
            EventManager.ButtonClicked(0);
        });
        brandButton.RegisterCallback<ClickEvent>(evt => {
            GameManager.instance.LaunchGamemode_2("brands");
            EventManager.ButtonClicked(0);
        });

        SetSlidersValue();
        
        audioSlider.RegisterValueChangedCallback(evt => {
            EventManager.SFXVolumeChange(audioSlider.value);
            EventManager.ButtonClicked(0);
        });
        musicSlider.RegisterValueChangedCallback(evt => {
            EventManager.MusicVolumeChange(musicSlider.value);
        });
    }

    void SetSlidersValue()
    {
        musicSlider.value = AudioManager.instance.MusicSliderValue();
        audioSlider.value = AudioManager.instance.SFXSliderValue();
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

    void HideCategoryList()
    {
        singleplayerButton.pickingMode = PickingMode.Position;
        multiplayerButton.pickingMode = PickingMode.Position;
        
        uiHolder.style.opacity = 100;
        uiHolder.pickingMode = PickingMode.Position;

        categoryList.style.opacity = 0;
        categoryList.pickingMode = PickingMode.Ignore;

        animalsButton.pickingMode = PickingMode.Ignore;
        countryButton.pickingMode = PickingMode.Ignore;
        leg_FruitButton.pickingMode = PickingMode.Ignore; 
        capitalesButton.pickingMode = PickingMode.Ignore; 
        flowersButton.pickingMode = PickingMode.Ignore; 
        brandButton.pickingMode = PickingMode.Ignore;
    }

    void ShowCategoryList()
    {
        singleplayerButton.pickingMode = PickingMode.Ignore;
        multiplayerButton.pickingMode = PickingMode.Ignore;
        
        uiHolder.style.opacity = 0;
        uiHolder.pickingMode = PickingMode.Ignore;

        categoryList.style.opacity = 100;
        categoryList.pickingMode = PickingMode.Position;

        animalsButton.pickingMode = PickingMode.Position;
        countryButton.pickingMode = PickingMode.Position;
        leg_FruitButton.pickingMode = PickingMode.Position; 
        capitalesButton.pickingMode = PickingMode.Position; 
        flowersButton.pickingMode = PickingMode.Position; 
        brandButton.pickingMode = PickingMode.Position;
    }
}