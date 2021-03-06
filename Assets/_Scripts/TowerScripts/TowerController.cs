using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerController : MonoBehaviour
{
    [SerializeField] float range = 3;
    [SerializeField] LayerMask targetLayer;
    [SerializeField] Transform cannonHead;

    [SerializeField] Transform bulletSpawnPoint;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float attackSpeed = 1; // attacks per second
    private float attackTimer;

    [Header("FX")]
    [SerializeField] ParticleSystem fireParticles;

    Transform target = null;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
        if (target != null)
        {
            RotateCannon(target.position);

            if (attackTimer <= 0)
            {
                attackTimer = attackSpeed;

                Fire();
            }
        }
    }

    private void Fire()
    {
        GameObject projectile = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
        ITargetter targetter = projectile.GetComponent<ITargetter>();

        if (targetter != null)
        {
            targetter.SetTarget(target.gameObject);
        }

        if (fireParticles != null)
        {
            fireParticles.Simulate(0, true, true);
            fireParticles.Play();
        }
    }

    private void RotateCannon(Vector3 targetPos)
    {
        Vector3 vecToTarget = targetPos - cannonHead.position;
        Vector3 rotatedVec = Vector3.RotateTowards(cannonHead.forward, vecToTarget.normalized, 10 * Time.deltaTime, 0f);
        cannonHead.localRotation = Quaternion.LookRotation(rotatedVec);
    }

    private void FixedUpdate()
    {
        // if the tower isn't already aiming at something, search for a new target within the given range
        // this is done in the physics update to save on resources
        if (target == null || Vector3.Distance(target.position, transform.position) > range)
        {
            target = null;
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, range, targetLayer);

            Transform closest = null;
            float closestDistance = 0;
            Transform objectivePos = DefensePoint.GetInstance().transform; // we want to target the enemy closest to the objective

            foreach (Collider hitCollider in hitColliders)
            {
                if (closest == null)
                {
                    closest = hitCollider.gameObject.transform;
                    closestDistance = Vector3.Distance(closest.position, objectivePos.position);
                }
                else
                {
                    float targetDistance = Vector3.Distance(hitCollider.gameObject.transform.position, objectivePos.position);
                    if (targetDistance < closestDistance)
                    {
                        closest = hitCollider.gameObject.transform;
                        closestDistance = targetDistance;
                    }
                }
            }

            target = closest;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
