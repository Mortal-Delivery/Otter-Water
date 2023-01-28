using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMove : MonoBehaviour
{

    // Declare variables for AISpawner manager script
    private AI_Spawner p_AIManager;

    // Moving and Turning
    private bool p_hasTarget = false;
    private bool p_isTurning;

    // Current Waypoint
    private Vector3 p_wayPoint;
    private Vector3 p_lastWaypoint = new Vector3 (0f, 0f, 0f);

    // Animation Speed
    private Animator p_animator;
    private float p_speed;
    private float p_scale;

    private Collider p_collider;

    public bool useRandomTarget;



    // Start is called before the first frame update
    void Start()
    {
        //Get the AISpawner from its parent
        p_AIManager = transform.parent.GetComponentInParent<AI_Spawner>();
        p_animator = GetComponent<Animator>();

        SetUpNPC();
    }

    void SetUpNPC()
    {
        // Randomly scales NPC
        float p_scale = Random.Range(0f, 4f);
        transform.localScale += new Vector3(p_scale * 1.5f, p_scale, p_scale);

        if (transform.GetComponent<Collider>() != null && transform.GetComponent<Collider>().enabled == true)
        {
            p_collider = transform.GetComponent<Collider>();
        }
        else if (transform.GetComponentInChildren<Collider>() != null && transform.GetComponentInChildren<Collider>().enabled == true)
        {
            p_collider = transform.GetComponentInChildren<Collider>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // if waypoint has been found, move towards it
        // otherwise, find a waypoint.
        if (!p_hasTarget)
        {
            p_hasTarget = CanFindTarget();
        }
        else 
        {
            // Rotate NPC to face wthe current waypoint
            RotateNPC(p_wayPoint, p_speed);
            // Straight line towards the waypoint
            transform.position = Vector3.MoveTowards(transform.position, p_wayPoint, p_speed * Time.deltaTime);

            //check if collided
            CollidedNPC();
        }

        if (transform.position == p_wayPoint)
        {
            p_hasTarget = false;
        }
    }

    // Changes NPC's directions if collision
    void CollidedNPC()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, transform.localScale.z))
        {
            // if collider has hit waypoint or itself, ignore.
            if (hit.collider == p_collider | hit.collider.tag == "Waypoint")
            {
                return;
            }
            int randomNum = Random.Range(1, 100);
            if (randomNum < 40)
            {
                p_hasTarget = false;
            }

            // Debug Proof its working
            Debug.Log(hit.collider.transform.parent.name + " " + hit.collider.transform.parent.position);
        }
    }

    Vector3 GetWaypoint(bool isRandom)
    {
        if (isRandom) // if true, get a random location
        {
            return p_AIManager.RandomPosition();
        }
        else // else, get a random waypoint
        {
            return p_AIManager.RandomWaypoint();
        }
    }

    bool CanFindTarget(float start = 1f, float end = 7f)
    {
        p_wayPoint = p_AIManager.RandomWaypoint();
        // Don't repeat the same waypoint
        if (p_lastWaypoint == p_wayPoint)
        {
            // Get a new waypoint
            p_wayPoint = GetWaypoint(true);
            return false;
        }
        else 
        {
            // Update last waypoint
            p_lastWaypoint = p_wayPoint;
            // Random speed for movement and animation
            p_speed = Random.Range(start, end);
            p_animator.speed = p_speed;
            // We found a WP
            return true;
        }
    }

    // Rotates NPC to face the new waypoint
    void RotateNPC (Vector3 waypoint, float currentSpeed)
    {
        // Increase turn speed
        float TurnSpeed = currentSpeed * Random.Range(1f, 3f);

        // new direction to look at
        Vector3 LookAt = waypoint - this.transform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(LookAt), TurnSpeed * Time.deltaTime);
    }

}
