using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth = 100;

    [SerializeField] Slider healthBar;

    private void Start()
    {
        if (healthBar == null)
        {
            healthBar = GetComponent<Slider>();
        }

        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;
    }

    public void SetMaxHealth(int health)
    {
        maxHealth = health;
        healthBar.maxValue = maxHealth;
    }

    public void SetHealth(int health)
    {
        currentHealth = health;

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
        healthBar.value = currentHealth;
    }
}
