using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LobbyUIManager : MonoBehaviour
{
    VisualElement root;
    VisualElement settingsTab;
    public Slider musicSlider;
    Button singleplayerButton;
    Button multiplayerButton;
    Button settingsButton;
    Button returnButton;
    Label connectingLabel;

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

        singleplayerButton.RegisterCallback<ClickEvent>(evt => ConnectingScreen());
        settingsButton.RegisterCallback<ClickEvent>(evt => OpenCloseSettings(settingsButton));
        returnButton.RegisterCallback<ClickEvent>(evt => OpenCloseSettings(returnButton));

        musicSlider.value = 0.05f;
    }

    void ConnectingScreen()
    {
        singleplayerButton.style.opacity = 0;
        multiplayerButton.style.opacity = 0;
        connectingLabel.style.opacity = 100;
        PlayfabManager.instance.Login();
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
}