using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBullet : MonoBehaviour, ITargetter
{
    [SerializeField] float speed = 10;
    [SerializeField] float detonationRange = 0.2f;
    [SerializeField] GameObject explosion;
    GameObject target;
    Vector3 targetPosition;

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            targetPosition = target.transform.position;
        }
        if (targetPosition != null)
        {
            Vector3 movement = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            this.transform.position = movement;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (targetPosition != null)
        {
            if (Vector3.Distance(targetPosition, transform.position) < detonationRange)
            {
                Explode();
            }
        }
    }

    public void SetTarget(GameObject target)
    {
        this.target = target;
    }

    private void Explode()
    {
        GameObject go = Instantiate(explosion, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
