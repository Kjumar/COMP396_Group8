using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetPlayerBullet : NetworkBehaviour
{
    public int damage = 1;
    public float life = 3;

    private void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        if (life <= 0)
        {
            CmdDestroy();
        }

        life -= Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isServer)
        {
            return;
        }
        if (other.gameObject.transform.CompareTag("Enemy"))
        {
            IDamageable target = other.gameObject.GetComponent<IDamageable>();

            if (target != null)
            {
                target.TakeDamage(damage);
            }
        }

        CmdDestroy();
    }

    [Command]
    private void CmdDestroy()
    {
        NetworkServer.Destroy(gameObject);
    }
}
