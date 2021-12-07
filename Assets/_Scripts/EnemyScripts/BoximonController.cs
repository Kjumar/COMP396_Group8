using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoximonController : MonoBehaviour, IPathable, IDamageable
{
    [SerializeField] int health = 10;
    [SerializeField] float speed = 4;
    [SerializeField] float turnSpeedRad = 8;
    [SerializeField] float attackRange = 1;
    [SerializeField] int attackPower = 5;

    [Header("Drops on Death")]
    [SerializeField] GameObject drop;
    [SerializeField] int minAmount = 0;
    [SerializeField] int maxAmount = 2;

    private Transform[] wayPoints;
    private int currentPoint = 0;

    private Animator anim;
    private DefensePoint target;

    GameObject playerInRange = null;
    LayerMask playerLayer;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetFloat("Speed", speed);
        StartAttack smScript = anim.GetBehaviour<StartAttack>();
        smScript.controller = this;

        playerLayer = LayerMask.GetMask(new string[] { "Player" });
    }

    private void FixedUpdate()
    {
        // check if a player is too close to the enemy
        Collider[] colliders = Physics.OverlapSphere(transform.position, attackRange, playerLayer);

        if (colliders.Length > 0)
        {
            playerInRange = colliders[0].gameObject;
        }
        else
        {
            playerInRange = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // This should've been implemented as an FSM
        // the sates are: ChasePlayer, FollowPath, and AttackObjective
        if (playerInRange != null)
        {
            anim.SetTrigger("Attack 01");
            anim.SetFloat("Speed", 0);

            Vector3 C2T = playerInRange.transform.position - this.transform.position;
            Vector3 rotatedVec = Vector3.RotateTowards(this.transform.forward, C2T.normalized, turnSpeedRad * Time.deltaTime, 0.0f);
            this.transform.rotation = Quaternion.LookRotation(rotatedVec);
        }
        else if (currentPoint < wayPoints.Length)
        {
            anim.SetFloat("Speed", speed);
            MoveTowards(wayPoints[currentPoint].position);
            
            if (Vector3.Distance(this.transform.position, wayPoints[currentPoint].position) < 0.1f)
            {
                currentPoint += 1;
            }
        }
        else if (target != null)
        {
            if (Vector3.Distance(transform.position, target.transform.position) < attackRange)
            {
                anim.SetTrigger("Attack 01");
                anim.SetFloat("Speed", 0);
            }
            else
            {
                MoveTowards(target.transform.position);
            }
        }
        else
        {
            target = DefensePoint.GetInstance();
        }
    }

    private void MoveTowards(Vector3 target)
    {
        Vector3 movement = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        this.transform.position = movement;

        Vector3 C2T = target - this.transform.position;

        Vector3 rotatedVec = Vector3.RotateTowards(this.transform.forward, C2T.normalized, turnSpeedRad * Time.deltaTime, 0.0f);
        this.transform.rotation = Quaternion.LookRotation(rotatedVec);
    }

    public void SetPath(Transform[] wayPoints)
    {
        this.wayPoints = wayPoints;
    }

    // AttackTarget gets called by the animator script whenever the attack animation starts
    // to trigger this function, use anim.SetTrigger("Attack 01")
    public void AttackTarget()
    {
        // the way the states are ordered, the enemies will prioritize hitting players (players can essentially body block for the objective)
        // since this gets triggered by the animator, I'm differentiating between attacking players and the objective here
        if (playerInRange != null)
        {
            playerInRange.GetComponent<IDamageable>().TakeDamage(attackPower);
        }
        else if (target != null)
        {
            target.TakeDamage(attackPower);
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            SpawnDrops();

            // notify the gamecontroller that an enemy died, then destroy the enemy
            GameController.GetInstance().activeEnemies--;
            Destroy(gameObject);
        }
    }

    private void SpawnDrops()
    {
        if (drop != null)
        {
            int amount = Random.Range(minAmount, maxAmount);
            if (amount > 0)
            {
                GameObject go = Instantiate(drop, transform.position, drop.transform.rotation);

                FloatingCoin fc = go.GetComponent<FloatingCoin>();
                if (fc != null)
                {
                    fc.value = amount;
                }
            }
        }
    }
}
