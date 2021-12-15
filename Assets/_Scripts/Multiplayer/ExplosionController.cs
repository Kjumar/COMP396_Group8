using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    [SerializeField] int damage = 2;
    [SerializeField] float life = 5;
    [SerializeField] float tickRate = 1;
    [SerializeField] float effectRadius = 4;
    [SerializeField] LayerMask targetMask;
    [SerializeField] AudioSource soundFX;
    private float tickTimer = 0;

    private void Update()
    {
        tickTimer -= Time.deltaTime;
        if (tickTimer <= 0)
        {
            tickTimer = tickRate;
            Collider[] colliders = Physics.OverlapSphere(transform.position, effectRadius, targetMask);

            for (int i = 0; i < colliders.Length; i++)
            {
                IDamageable damageable = colliders[i].gameObject.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(damage);
                }
            }

            if (colliders.Length > 0)
            {
                soundFX.PlayOneShot(soundFX.clip);
            }
        }

        life -= Time.deltaTime;
        if (life <= 0)
        {
            Destroy(gameObject);
        }
    }
}
