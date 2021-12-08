using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class NetworkBankController : NetworkBehaviour
{
    [SyncVar]
    [SerializeField] int bank = 100; // starting amount
    [SerializeField] Text bankText;
    [SerializeField] AudioSource sfx;

    public override void OnStartClient()
    {
        bankText.text = bank.ToString();
    }

    // Start is called before the first frame update
    void Start()
    {
        bankText.text = bank.ToString();
    }

    private void FixedUpdate()
    {
        UpdateText();
    }

    public bool CanPay(int amount)
    {
        if (amount <= bank)
        {
            return true;
        }
        return false;
    }


    public void Pay(int amount)
    {
        bank -= amount;
        UpdateText();
    }

    public void Deposit(int amount)
    {
        PlaySFX();

        bank += amount;
        UpdateText();
    }

    [ClientRpc]
    private void PlaySFX()
    {
        if (sfx != null) sfx.PlayOneShot(sfx.clip);
    }

    public void Withdraw(int amount)
    {
        bank -= amount;
        UpdateText();
    }

    void UpdateText()
    {
        bankText.text = bank.ToString();
    }

    public int GetBalance()
    {
        return bank;
    }
}
