using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using GameManagerNamespace;

public class LobbyUIManager : MonoBehaviour
{
    [SerializeField] VisualTreeAsset elementList;
    VisualElement root;
    VisualElement settingsTab;
    VisualElement uiHolder;
    VisualElement categoryList;
    VisualElement visualInfoGamemode1;
    VisualElement visualInfoGamemode2;
    VisualElement backgroundDay;
    VisualElement backgroundSunset;
    VisualElement backgroundNight;
    VisualElement currentBackground;
    VisualElement defaultOutline;
    VisualElement kenneyOutline;
    VisualElement simpleOutline;
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
    Button infoGamemode1;
    Button infoGamemode2;
    Button dayButton;
    Button sunsetButton;
    Button nightButton;
    Button returnCategoryButton;
    Button defaultButton;
    Button kenneyButton;
    Button simpleButton;
    Label playerUsername;
    ListView leaderbordList;
    bool clicked1;
    bool clicked2;
    const string BACKGROUND = "CurrentBackground";
    const string BOXSSKIN = "Skin";
    List<string> leaderbord = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        settingsTab = root.Q<VisualElement>("SettingsTab");
        uiHolder = root.Q<VisualElement>("UIHolder");
        categoryList = root.Q<VisualElement>("CategoryList");
        singleplayerButton = root.Q<Button>("SingleplayerButton");
        multiplayerButton = root.Q<Button>("MultiplayerButton");
        settingsButton = root.Q<Button>("SettingsButton");
        returnButton = root.Q<Button>("ReturnButton");
        musicSlider = root.Q<Slider>("MusicSlider");
        audioSlider = root.Q<Slider>("AudioSlider");
        playerUsername = root.Q<Label>("PlayerUsernameLabel");
        leaderbordList = root.Q<ListView>("LeaderbordList");
        categoryList = root.Q<VisualElement>("CategoryList");
        categoryList = root.Q<VisualElement>("CategoryList");
        visualInfoGamemode1 = root.Q<VisualElement>("VisualInfoGamemode1");
        visualInfoGamemode2 = root.Q<VisualElement>("VisualInfoGamemode2");
        backgroundDay = root.Q<VisualElement>("BackgroundDay");
        backgroundSunset = root.Q<VisualElement>("BackgroundSunset");
        backgroundNight = root.Q<VisualElement>("BackgroundNight");
        defaultOutline = root.Q<VisualElement>("DefaultOutline");
        kenneyOutline = root.Q<VisualElement>("KenneyOutline");
        simpleOutline = root.Q<VisualElement>("SimpleOutline");

        defaultButton = root.Q<Button>("DefaultButton");
        kenneyButton = root.Q<Button>("KenneyButton");
        simpleButton = root.Q<Button>("SimpleButton");
        animalsButton = root.Q<Button>("AnimalsButton");
        countryButton = root.Q<Button>("CountryButton");
        leg_FruitButton = root.Q<Button>("Fruit_LegumeButton");
        capitalesButton = root.Q<Button>("CapitalesButton");
        flowersButton = root.Q<Button>("FlowersButton");
        brandButton = root.Q<Button>("BrandsButton");
        infoGamemode1 = root.Q<Button>("InfoGamemode1");
        infoGamemode2 = root.Q<Button>("InfoGamemode2");
        dayButton = root.Q<Button>("DayButton");
        sunsetButton = root.Q<Button>("SunsetButton");
        nightButton = root.Q<Button>("NightButton");
        returnCategoryButton = root.Q<Button>("ReturnCategoryButton");

        ShowUsername();
        HideCategoryList();
        StopAllCoroutines();

        infoGamemode1.RegisterCallback<ClickEvent>(evt => {
            ShowHideInfo(infoGamemode1);
            EventManager.ButtonClicked(0);
        });
        infoGamemode2.RegisterCallback<ClickEvent>(evt => {
            ShowHideInfo(infoGamemode2);
            EventManager.ButtonClicked(0);
        });
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
        returnCategoryButton.RegisterCallback<ClickEvent>(evt => {
            HideCategoryList();
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
            GameManager.instance.LaunchGamemode_2("vegetables&fruits");
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

        SetBoxsSkin();

        defaultButton.RegisterCallback<ClickEvent>(evt => {
            ChangeBoxsSkin("default");
            EventManager.ButtonClicked(0);
        });
        kenneyButton.RegisterCallback<ClickEvent>(evt => {
            ChangeBoxsSkin("kenney");
            EventManager.ButtonClicked(0);
        });
        simpleButton.RegisterCallback<ClickEvent>(evt => {
            ChangeBoxsSkin("simple");
            EventManager.ButtonClicked(0);
        });

        SetBackground();

        dayButton.RegisterCallback<ClickEvent>(evt => {
            StartCoroutine(ChangeBackground(backgroundDay));
            EventManager.ButtonClicked(0);
        });
        sunsetButton.RegisterCallback<ClickEvent>(evt => {
            StartCoroutine(ChangeBackground(backgroundSunset));
            EventManager.ButtonClicked(0);
        });
        nightButton.RegisterCallback<ClickEvent>(evt => {
            StartCoroutine(ChangeBackground(backgroundNight));
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

    void SetBoxsSkin()
    {
        string boxsskin = PlayerPrefs.GetString(BOXSSKIN, "default");

        if (boxsskin == "default")
        {
            defaultOutline.style.opacity = 100;
            kenneyOutline.style.opacity = 0;
            simpleOutline.style.opacity = 0;
        }
        else if (boxsskin == "kenney")
        {
            defaultOutline.style.opacity = 0;
            kenneyOutline.style.opacity = 100;
            simpleOutline.style.opacity = 0;
        }
        else if (boxsskin == "simple")
        {
            defaultOutline.style.opacity = 0;
            kenneyOutline.style.opacity = 0;
            simpleOutline.style.opacity = 100;
        }
    }

    void ChangeBoxsSkin(string box)
    {
        PlayerPrefs.SetString(BOXSSKIN, box);

        if (box == "default")
        {
            defaultOutline.style.opacity = 100;
            kenneyOutline.style.opacity = 0;
            simpleOutline.style.opacity = 0;
        }
        else if (box == "kenney")
        {
            defaultOutline.style.opacity = 0;
            kenneyOutline.style.opacity = 100;
            simpleOutline.style.opacity = 0;
        }
        else if (box == "simple")
        {
            defaultOutline.style.opacity = 0;
            kenneyOutline.style.opacity = 0;
            simpleOutline.style.opacity = 100;
        }
    }

    void SetBackground()
    {
        string savedBackground = PlayerPrefs.GetString(BACKGROUND, backgroundDay.name);

        if (savedBackground == "BackgroundDay")
        {
            backgroundDay.style.opacity = 1;
            backgroundSunset.style.opacity = 0;
            backgroundNight.style.opacity = 0;
            currentBackground = backgroundDay;
        }
        else if (savedBackground == "BackgroundSunset")
        {
            backgroundDay.style.opacity = 0;
            backgroundSunset.style.opacity = 1;
            backgroundNight.style.opacity = 0;
            currentBackground = backgroundSunset;
        }
        else if (savedBackground == "BackgroundNight")
        {
            backgroundDay.style.opacity = 0;
            backgroundSunset.style.opacity = 0;
            backgroundNight.style.opacity = 1;
            currentBackground = backgroundNight;
        }
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

    IEnumerator ChangeBackground(VisualElement newBackground)
    {
        dayButton.pickingMode = PickingMode.Ignore;
        sunsetButton.pickingMode = PickingMode.Ignore;
        nightButton.pickingMode = PickingMode.Ignore;

        currentBackground.BringToFront();

        DOFade(currentBackground, 0f, 3f);
        DOFade(newBackground, 1f, 3f);

        currentBackground = newBackground;
        PlayerPrefs.SetString(BACKGROUND, currentBackground.name);

        yield return new WaitForSeconds(3f);

        dayButton.pickingMode = PickingMode.Position;
        sunsetButton.pickingMode = PickingMode.Position;
        nightButton.pickingMode = PickingMode.Position;
    }

    public void DOFade(VisualElement target, float endValue, float duration)
    {
        float startValue = target.resolvedStyle.opacity;
        DOTween.To(() => startValue, x =>
        {
            startValue = x;
            target.style.opacity = startValue;
        },  endValue, duration);
    }

    void ShowHideInfo(Button button)
    {
        if (button == infoGamemode1)
        {
            if (clicked1 == true && clicked2 == false)
            {
                visualInfoGamemode1.AddToClassList("VisualInfoGamemodeHidden");
                clicked1 = false;
            }
            else if (clicked1 == false && clicked2 == true)
            {
                visualInfoGamemode1.RemoveFromClassList("VisualInfoGamemodeHidden");
                visualInfoGamemode2.AddToClassList("VisualInfoGamemode2Hidden");
                clicked2 = false;
                clicked1 = true;
            }
            else 
            {
                visualInfoGamemode1.RemoveFromClassList("VisualInfoGamemodeHidden");
                clicked1 = true;
            }
        }
        else
        {
            if (clicked2 == true && clicked1 == false)
            {
                visualInfoGamemode2.AddToClassList("VisualInfoGamemode2Hidden");
                clicked2 = false;
            }
            else if (clicked2 == false && clicked1 == true)
            {
                visualInfoGamemode2.RemoveFromClassList("VisualInfoGamemode2Hidden");
                visualInfoGamemode1.AddToClassList("VisualInfoGamemodeHidden");
                clicked1 = false;
                clicked2 = true;
            }
            else 
            {
                visualInfoGamemode2.RemoveFromClassList("VisualInfoGamemode2Hidden");
                clicked2 = true;
            }
        }

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