using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// since each enemy and tower may need to check where this objective is, we enfore that it is singleton

public class DefensePoint : MonoBehaviour
{
    private static DefensePoint instance;

    [SerializeField] private int health = 100;
    [SerializeField] UIHealthBar healthBar;
    [SerializeField] GameObject gameOverScreen;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
        gameOverScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static DefensePoint GetInstance()
    {
        return instance;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        healthBar.SetHealth(health);
        if (health <= 0)
        {
            gameOverScreen.SetActive(true);
            Time.timeScale = 0f;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
