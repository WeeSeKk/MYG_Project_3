using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using PlayfabManagerNamespace;

public class IntroUIManager : MonoBehaviour
{
    VisualElement root;
    VisualElement loginRegisterHolder;
    TextField usernameTextField;
    TextField passwordTextField;
    Button registerButton;
    Button loginButton;
    Label errorLabel;
    Label infoLabel;

    // Start is called before the first frame update
    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        usernameTextField = root.Q<TextField>("UsernameTextField");
        passwordTextField = root.Q<TextField>("PasswordTextField");
        registerButton = root.Q<Button>("RegisterButton");
        loginButton = root.Q<Button>("LoginButton");
        errorLabel = root.Q<Label>("ErrorLabel");
        loginRegisterHolder = root.Q<VisualElement>("LoginRegisterHolder");
        infoLabel = root.Q<Label>("InfoLabel");

       

        registerButton.RegisterCallback<ClickEvent>(evt => {
            EventManager.ButtonClicked(0);
            PlayfabManager.instance.OnRegister(usernameTextField.text, passwordTextField.text);//if inputfield are correct
            StartCoroutine(WaitingScreen());
        });
        loginButton.RegisterCallback<ClickEvent>(evt => {
            EventManager.ButtonClicked(0);
            PlayfabManager.instance.OnLogin(usernameTextField.text, passwordTextField.text);
            StartCoroutine(WaitingScreen());
        });
    }

    public IEnumerator ShowError(string errorMessage)
    {
        if (errorMessage == "Invalid input parameters")
        {
            errorLabel.text = "Password as to be at least 6 characters long";
            yield return new WaitForSeconds(2f);
            errorLabel.text = "";
        }
        else
        {
            errorLabel.text = errorMessage;
            yield return new WaitForSeconds(2f);
            errorLabel.text = "";
        } 
    }

    IEnumerator WaitingScreen()
    {
        loginRegisterHolder.style.opacity = 0;
        infoLabel.style.opacity = 100;

        infoLabel.text = "Connecting ";
        yield return new WaitForSeconds(0.5f);
        infoLabel.text = "Connecting .";
        yield return new WaitForSeconds(0.5f);
        infoLabel.text = "Connecting ..";
        yield return new WaitForSeconds(0.5f);
        infoLabel.text = "Connecting ...";
        yield return new WaitForSeconds(0.5f);
    }

    public void HideWaitingScreen()
    {
        StopAllCoroutines();
        loginRegisterHolder.style.opacity = 100;
        infoLabel.style.opacity = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
