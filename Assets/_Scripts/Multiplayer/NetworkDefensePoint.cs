using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkDefensePoint : NetworkBehaviour
{
    [SyncVar]
    [SerializeField] private int health = 100;
    [SerializeField] UIHealthBar healthBar;
    [SerializeField] GameObject gameOverScreen;

    // Start is called before the first frame update
    void Start()
    {
        gameOverScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            gameOverScreen.SetActive(true);
            Time.timeScale = 0f;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void TakeDamage(int damage)
    {
        RpcTakeDamage(damage);
    }

    [ClientRpc]
    public void RpcTakeDamage(int damage)
    {
        health -= damage;
        healthBar.SetHealth(health);
    }

    public int GetHealth()
    {
        return health;
    }
}
