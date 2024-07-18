using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LobbyUIManager : MonoBehaviour
{
    VisualElement root;
    Button singleplayerButton;
    Button multiplayerButton;
    Button settingsButton;
    Button returnButton;
    Label connectingLabel;

    // Start is called before the first frame update
    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        singleplayerButton = root.Q<Button>("SingleplayerButton");
        multiplayerButton = root.Q<Button>("MultiplayerButton");
        connectingLabel = root.Q<Label>("ConnectingLabel");
        settingsButton = root.Q<Button>("SettingsButton");
        returnButton = root.Q<Button>("ReturnButton");

        singleplayerButton.RegisterCallback<ClickEvent>(evt => ConnectingScreen());
        settingsButton.RegisterCallback<ClickEvent>(evt => OpenCloseSettings(settingsButton));
        returnButton.RegisterCallback<ClickEvent>(evt => OpenCloseSettings(returnButton));
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
            //open settings
        }
        else
        {
            //close settings
        }
    }
}