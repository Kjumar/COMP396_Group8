using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingCoin : MonoBehaviour
{
    public int value = 1;
    [SerializeField] float floatingHeight = 0.3f;
    [SerializeField] float turningSpeed = 180f;

    private void Start()
    {
        transform.position = transform.position + new Vector3(0, floatingHeight, 0);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0f, turningSpeed, 0f, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (BankController.instance != null)
            {
                BankController.instance.Deposit(value);
            }

            Destroy(gameObject);
        }
    }
}
