using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class NetworkPauseScreen : MonoBehaviour
{
    [SerializeField] NetworkManager netManager;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void ReturnToMain()
    {
        netManager.StopClient();
        netManager.StopHost();
        SceneManager.LoadScene("MainMenu");
    }

    public void Resume()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        gameObject.SetActive(false);
    }
}
