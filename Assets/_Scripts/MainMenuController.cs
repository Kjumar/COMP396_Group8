using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject singleplayerMenu;
    [SerializeField] GameObject multiplayerMenu;
    [SerializeField] GameObject howToPlayMenu;

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

    public void OpenMultiplayerMenu()
    {
        mainMenu.SetActive(false);
        multiplayerMenu.SetActive(true);
    }

    public void OpenHowToPlayMenu()
    {
        mainMenu.SetActive(false);
        howToPlayMenu.SetActive(true);
    }

    public void ReturnToMain()
    {
        mainMenu.SetActive(true);
        singleplayerMenu.SetActive(false);
        multiplayerMenu.SetActive(false);
        howToPlayMenu.SetActive(false);
    }
}
