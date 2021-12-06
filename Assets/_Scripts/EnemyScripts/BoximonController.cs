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

    private Transform[] wayPoints;
    private int currentPoint = 0;

    private Animator anim;
    private DefensePoint target;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetFloat("Speed", speed);
        StartAttack smScript = anim.GetBehaviour<StartAttack>();
        smScript.controller = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentPoint < wayPoints.Length)
        {
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
        target.TakeDamage(attackPower);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            // notify the gamecontroller that an enemy died, then destroy the enemy
            GameController.GetInstance().activeEnemies--;
            Destroy(gameObject);
        }
    }
}
