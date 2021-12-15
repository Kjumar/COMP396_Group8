using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public float speed = 50f;
    public int inflictDamage = 1;
    public float bulletDuration = 2f;

    GameObject target;

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;

        bulletDuration -= Time.deltaTime;
        if(bulletDuration <= 0f)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        var hit = collision.gameObject;
        if (hit.tag == "Enemy")
        {
            var health = hit.GetComponent<IDamageable>();
            if (health != null)
            {
                health.TakeDamage(inflictDamage);
            }
        }
        Destroy(this.gameObject);
    }
}
