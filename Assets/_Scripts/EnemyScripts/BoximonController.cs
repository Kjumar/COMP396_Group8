using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoximonController : MonoBehaviour, Pathable
{
    [SerializeField] int health = 100;
    [SerializeField] float speed = 4;
    [SerializeField] float turnSpeedRad = 8;

    private Transform[] wayPoints;
    private int currentPoint = 0;

    // Start is called before the first frame update
    void Start()
    {
        
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
}
