using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

public class Tests
{
    [UnityTest]
    public IEnumerator FindUIDocument()//find ui document
    {
        SceneManager.LoadScene(0);
        yield return null;

        GameObject go = GameObject.Find("UIDocument");
        UIDocument uIDocument = go.GetComponent<UIDocument>();
        Assert.IsNotNull(uIDocument);
    }

    [UnityTest]
    public IEnumerator RegisterSuccess()//verify if you can register 
    {
        yield return SceneManager.LoadSceneAsync("IntroScene");
        GameObject go = GameObject.Find("UIDocument");
        UIDocument uIDocument = go.GetComponent<UIDocument>();

        Button registerButton = uIDocument.rootVisualElement.Q<Button>("RegisterButton");
        TextField usernameTextField = uIDocument.rootVisualElement.Q<TextField>("UsernameTextField");
        TextField passwordTextField = uIDocument.rootVisualElement.Q<TextField>("PasswordTextField");
        Label errorLabel = uIDocument.rootVisualElement.Q<Label>("ErrorLabel");

        usernameTextField.value = "UsernameTest";
        passwordTextField.value = "azerty";

        using (var clicked = new NavigationSubmitEvent {target = registerButton})
            registerButton.SendEvent(clicked);

        yield return new WaitForSeconds(2f);      
        Assert.IsTrue(errorLabel.text == "");
    }

    [UnityTest]
    public IEnumerator LoginSuccess()//verify if you can login
    {
        yield return SceneManager.LoadSceneAsync("IntroScene");

        GameObject go = GameObject.Find("UIDocument");
        UIDocument uIDocument = go.GetComponent<UIDocument>();
        
        Button loginButton = uIDocument.rootVisualElement.Q<Button>("LoginButton");
        TextField usernameTextField = uIDocument.rootVisualElement.Q<TextField>("UsernameTextField");
        TextField passwordTextField = uIDocument.rootVisualElement.Q<TextField>("PasswordTextField");
        Label errorLabel = uIDocument.rootVisualElement.Q<Label>("ErrorLabel");
        

        usernameTextField.value = "test";
        passwordTextField.value = "123456";

        using (var clicked = new NavigationSubmitEvent {target = loginButton})
            loginButton.SendEvent(clicked);

        yield return new WaitForSeconds(2f);  
        Assert.IsTrue(errorLabel.text == "");
    }

    [UnityTest]
    public IEnumerator FindWord()
    {
        yield return SceneManager.LoadSceneAsync("Scene_Gamemode_01");

        GameObject go = GameObject.Find("UIDocument");
        UIDocument uIDocument = go.GetComponent<UIDocument>();
        
        Label lettersLabel = uIDocument.rootVisualElement.Q<Label>("LettersLabel");
        Button validButton = uIDocument.rootVisualElement.Q<Button>("ValidButton");

        
        lettersLabel.text = "who";
        

        using (var clicked = new NavigationSubmitEvent {target = validButton})
            validButton.SendEvent(clicked);
            
        Assert.IsTrue(lettersLabel.text == "");
    }
}
