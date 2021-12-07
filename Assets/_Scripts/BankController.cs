using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BankController : MonoBehaviour
{
    // singleton
    public static BankController instance;

    [SerializeField] int bank = 100; // starting amount
    [SerializeField] Text bankText;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        bankText.text = bank.ToString();
    }

    public bool Pay(int amount)
    {
        if (amount <= bank)
        {
            bank -= amount;
            bankText.text = bank.ToString();
            return true;
        }
        return false;
    }

    public void Deposit(int amount)
    {
        bank += amount;
        bankText.text = bank.ToString();
    }
}
