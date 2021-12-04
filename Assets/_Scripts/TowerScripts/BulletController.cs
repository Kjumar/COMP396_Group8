using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour, ITargetter
{
    [SerializeField] float speed = 10;
    [SerializeField] int damage = 5;
    GameObject target;

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            Vector3 movement = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
            this.transform.position = movement;

            Vector3 C2T = target.transform.position - this.transform.position;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == target)
        {
            IDamageable damageable = target.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }

    public void SetTarget(GameObject target)
    {
        this.target = target;
    }
}
