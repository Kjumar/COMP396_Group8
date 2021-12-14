using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public float speed = 50f;
    public int inflictDamage = 10;
    public float bulletDuration = 2f;
    public float bulletTimer;

    GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        bulletTimer = bulletDuration;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;

        bulletTimer -= Time.deltaTime;
        if(bulletTimer <= 0f)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        var hit = collision.gameObject;
        if (hit.tag == "Enemy")
        {
            var health = hit.GetComponent<BoximonController>();
            health.TakeDamage(inflictDamage);
        }
        Destroy(this.gameObject);
    }
}
