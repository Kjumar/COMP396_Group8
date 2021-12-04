using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// since each enemy and tower may need to check where this objective is, we enfore that it is singleton

public class DefensePoint : MonoBehaviour
{
    private static DefensePoint instance;

    [SerializeField] private int health = 100;
    [SerializeField] UIHealthBar healthBar;

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
    }
}
