using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetFloatingCoin : NetworkBehaviour
{
    public int value = 1;
    [SerializeField] float floatingHeight = 0.3f;
    [SerializeField] float turningSpeed = 180f;
    private NetworkBankController bank;

    // Start is called before the first frame update
    private void Start()
    {
        transform.position = transform.position + new Vector3(0, floatingHeight, 0);
        bank = FindObjectOfType<NetworkBankController>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0f, turningSpeed, 0f, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isServer)
        {
            return;
        }

        if (other.gameObject.CompareTag("Player"))
        {
            if (bank != null)
            {
                bank.Deposit(value);
            }

            NetworkServer.Destroy(gameObject);
        }
    }
}
