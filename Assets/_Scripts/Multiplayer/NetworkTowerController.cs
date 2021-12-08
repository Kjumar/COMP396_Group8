using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkTowerController : NetworkBehaviour
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
    [SerializeField] AudioSource sfx;

    Transform target = null;
    private int scanInterval = 6; // amount fo physics frames to skip
    private int scanSkip = 6;

    private Transform objectivePos;

    // Start is called before the first frame update
    void Start()
    {
        NetworkDefensePoint defensePoint = FindObjectOfType<NetworkDefensePoint>();
        if (defensePoint != null)
        {
            objectivePos = defensePoint.transform;
        }
        else
        {
            objectivePos = transform;
        }
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

                if (fireParticles != null)
                {
                    fireParticles.Simulate(0, true, true);
                    fireParticles.Play();
                }
            }
        }
    }

    //[Command]
    private void Fire()
    {
        if (sfx != null) sfx.PlayOneShot(sfx.clip);

        GameObject projectile = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
        ITargetter targetter = projectile.GetComponent<ITargetter>();

        if (targetter != null)
        {
            targetter.SetTarget(target.gameObject);
        }

        // NetworkServer.Spawn(projectile);
    }

    private void RotateCannon(Vector3 targetPos)
    {
        Vector3 vecToTarget = targetPos - cannonHead.position;
        Vector3 rotatedVec = Vector3.RotateTowards(cannonHead.forward, vecToTarget.normalized, 10 * Time.deltaTime, 0f);
        cannonHead.localRotation = Quaternion.LookRotation(rotatedVec);
    }

    private void FixedUpdate()
    {
        scanSkip--;

        if (scanSkip <= 0)
        {
            scanSkip = scanInterval;

            // if the tower isn't already aiming at something, search for a new target within the given range
            // this is done in the physics update to save on resources
            if (target == null || Vector3.Distance(target.position, transform.position) > range)
            {
                target = null;
                Collider[] hitColliders = Physics.OverlapSphere(transform.position, range, targetLayer);

                Transform closest = null;
                float closestDistance = 0;

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
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
