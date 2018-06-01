using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpaceshipAI : MonoBehaviour {

    private Transform target;

    //Vector3 storeTarget;
    Vector3 newTargetPos;

    //bool savePos;
    bool ovverideTarget;

    Vector3 acceleration;
    Vector3 velocity;

    public float maxSpeed = 80f;

    //public float WingSpan = 12f;
    float storeMaxSpeed;
    float targetSpeed;

    Rigidbody rigidBody;
    Transform obstacle;


    public List<Vector3> EscapeDirections = new List<Vector3>();


	// Use this for initialization
	void Start () {
        storeMaxSpeed = maxSpeed;
        targetSpeed = storeMaxSpeed;
        rigidBody = GetComponent<Rigidbody>();

        target = GameObject.FindGameObjectWithTag("AITarget").transform;
	}
	
	// Update is called once per frame
	void Update () {
        Debug.DrawLine(transform.position, target.position);
        Vector3 forces = MoveTowardsTarget(target.position);
        acceleration = forces;

        velocity += 2 * acceleration * Time.deltaTime;

        if(velocity.magnitude>maxSpeed)
        {
            velocity = velocity.normalized * maxSpeed;
        }
        rigidBody.velocity = velocity;

        Quaternion desiredRotation = Quaternion.LookRotation(velocity);
        //transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime*0);


        //ObstacleAvoidance(transform.forward, 0);
        //ObstacleAvoidance(transform.forward, WingSpan);
        //ObstacleAvoidance(transform.forward, -WingSpan);

        if (ovverideTarget)
        {
            target.position = newTargetPos;
        }

	}

    Vector3 MoveTowardsTarget(Vector3 target)
    {
        Vector3 distance = target - transform.position;

        if (distance.magnitude < 5)
        {
            return distance.normalized * -maxSpeed;
            
            Debug.Log(distance.normalized * -maxSpeed);
        }
        else
        {
            return distance.normalized * maxSpeed;
            Debug.Log(distance.normalized * maxSpeed);
        }

    }

    //void ObstacleAvoidance(Vector3 direction, float offsetX)
    //{
    //    RaycastHit[] hit = Rays(direction, offsetX);

    //    for (int i = 0; i < hit.Length - 1; i++)
    //    {
    //        if (hit[i].transform.root.gameObject != this.gameObject)
    //        {
    //            if (!savePos)
    //            {
    //                storeTarget = target.position;
    //                obstacle = hit[i].transform;
    //                savePos = true;
    //            }

    //            FindEscapeDirection(hit[i].collider);

    //        }
    //    }
    //    if (EscapeDirections.Count > 0)
    //    {
    //        if (!ovverideTarget)
    //        {
    //            newTargetPos = getClosests();
    //            ovverideTarget = true;
    //        }
    //    }
    //    float distance = Vector3.Distance(transform.position, target.position);
    //    Debug.Log(distance);

    //    if (distance < WingSpan)
    //    {
    //        if (savePos)
    //        {
    //            target.position = storeTarget;
    //            savePos = false;
    //        }
    //        ovverideTarget = false;
    //        EscapeDirections.Clear();
    //    }

    //}


    //Vector3 getClosests()
    //{
    //    Vector3 clos = EscapeDirections[0];
    //    float distance = Vector3.Distance(transform.position, EscapeDirections[0]);


    //    foreach(Vector3 dir in EscapeDirections)
    //    {
    //        float tempDistance = Vector3.Distance(transform.position, dir);
    //        if(tempDistance<distance)
    //        {
    //            distance = tempDistance;
    //            clos = dir;
    //        }
    //    }
    //    return clos;
    //}

    //void FindEscapeDirection(Collider col)
    //{

    //    //sửa lại hướng bắn của AI khi cách player 1 khoang WingSpan
    //    RaycastHit hitUp;
    //    if (Physics.Raycast(col.transform.position, col.transform.up, out hitUp, col.bounds.extents.y * WingSpan))
    //    {

    //    }
    //    else
    //    {
    //        Vector3 dir = col.transform.position + new Vector3(0, col.bounds.extents.y * 2 + WingSpan, 0);
    //        if (!EscapeDirections.Contains(dir))
    //        {
    //            EscapeDirections.Add(dir);
    //        }
    //    }
    //    RaycastHit hitDown;
    //    if (Physics.Raycast(col.transform.position, -col.transform.up, out hitDown, col.bounds.extents.y * WingSpan))
    //    {

    //    }
    //    else
    //    {
    //        Vector3 dir = col.transform.position + new Vector3(0, -col.bounds.extents.y * 2 - WingSpan, 0);
    //        if (!EscapeDirections.Contains(dir))
    //        {
    //            EscapeDirections.Add(dir);
    //        }
    //    }

    //    RaycastHit hitRight;
    //    if (Physics.Raycast(col.transform.position, col.transform.right, out hitRight, col.bounds.extents.x * WingSpan))
    //    {

    //    }
    //    else
    //    {
    //        Vector3 dir = col.transform.position + new Vector3(-col.bounds.extents.x * 2 + WingSpan, 0, 0);
    //        if (!EscapeDirections.Contains(dir))
    //        {
    //            EscapeDirections.Add(dir);
    //        }
    //    }


    //    RaycastHit hitLeft;
    //    if (Physics.Raycast(col.transform.position, -col.transform.right, out hitLeft, col.bounds.extents.x * WingSpan))
    //    {

    //    }
    //    else
    //    {
    //        Vector3 dir = col.transform.position + new Vector3(-col.bounds.extents.x * 2 - WingSpan, 0, 0);
    //        if (!EscapeDirections.Contains(dir))
    //        {
    //            EscapeDirections.Add(dir);
    //        }
    //    }

    //}

    //RaycastHit[] Rays(Vector3 direction, float offsetX)
    //{
    //    Ray ray = new Ray(transform.position + new Vector3(offsetX, 0, 0), direction);
    //    Debug.DrawRay(transform.position + new Vector3(offsetX, 0, 0), direction * 10 * maxSpeed, Color.red);

    //    float distanceToLookAhead = maxSpeed * 100;
    //    RaycastHit[] hits = Physics.SphereCastAll(ray, 100, distanceToLookAhead);
    //    return hits;
    //}

}
