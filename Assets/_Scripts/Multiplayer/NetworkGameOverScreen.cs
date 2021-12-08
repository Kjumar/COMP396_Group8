using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class NetworkGameOverScreen : MonoBehaviour
{
    [SerializeField] NetworkManager netManager;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void ReturnToMain()
    {
        netManager.client.Disconnect();

        LoadScene("MainMenu");
    }
}
