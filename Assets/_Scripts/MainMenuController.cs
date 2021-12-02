using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject singleplayerMenu;

    // Start is called before the first frame update
    void Start()
    {
        ReturnToMain();
    }

    // button functions
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void OpenSingleplayerMenu()
    {
        mainMenu.SetActive(false);
        singleplayerMenu.SetActive(true);
    }

    public void OpenSettingsMenu()
    {
        mainMenu.SetActive(false);
    }

    public void ReturnToMain()
    {
        mainMenu.SetActive(true);
        singleplayerMenu.SetActive(false);
    }
}
