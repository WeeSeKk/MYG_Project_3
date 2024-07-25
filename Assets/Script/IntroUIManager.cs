using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class IntroUIManager : MonoBehaviour
{
    VisualElement root;
    TextField usernameTextField;
    TextField passwordTextField;
    Button registerButton;
    Button loginButton;
    Label errorLabel;

    // Start is called before the first frame update
    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        usernameTextField = root.Q<TextField>("UsernameTextField");
        passwordTextField = root.Q<TextField>("PasswordTextField");
        registerButton = root.Q<Button>("RegisterButton");
        loginButton = root.Q<Button>("LoginButton");
        errorLabel = root.Q<Label>("ErrorLabel");



        registerButton.RegisterCallback<ClickEvent>(evt => {
            EventManager.ButtonClicked(0);
            PlayfabManager.instance.OnRegister(usernameTextField.text, passwordTextField.text);//if inputfield are correct
        });
        loginButton.RegisterCallback<ClickEvent>(evt => {
            EventManager.ButtonClicked(0);
            PlayfabManager.instance.OnLogin(usernameTextField.text, passwordTextField.text);
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
