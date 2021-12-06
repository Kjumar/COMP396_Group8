using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void ReturnToMain()
    {
        LoadScene("MainMenu");
    }
}
