using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Enemy : MonoBehaviour
{
    public int health;
    public float speed;
    Animator animator;
    BoxCollider2D collider;
    Controller2D controller;
    Vector3 velocity;

    float gravity = -50;

    //Patrolling Variables
    [Header("Patrolling")]
    public Vector3[] localWaypoints;
    Vector3[] globalWaypoints;
    float nextMoveTime;
    int fromWaypointIndex = 0;
    float percentBetweenWaypoints;
    public bool cyclic;
    public float waitTime;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<Controller2D>();
        animator = GetComponent<Animator>();
        collider = GetComponent<BoxCollider2D>();

        globalWaypoints = new Vector3[localWaypoints.Length];
        for (int i = 0; i < localWaypoints.Length; i++)
        {
            globalWaypoints[i] = localWaypoints[i] + transform.position;
            Debug.Log("Global: " + globalWaypoints[i]);
            Debug.Log("Local: " + localWaypoints[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {   
        if (health <= 0)
        {
            StartCoroutine(Die());
        }
        else
        {
            Move();
        }
    }

    void Move()
    {
        velocity = EnemyPatrol();
        //velocity.y += gravity * Time.deltaTime;
        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }
        controller.Move(velocity * Time.deltaTime);

    }

    private void LateUpdate()
    {
        animator.SetBool("IsDead", false);
    }

    IEnumerator Die()
    {
        animator.SetBool("IsDead", true);
        collider.enabled = false;
        float elapsed = 0f;
        float duration = AnimationLength("Crab_Death");

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            yield return 0;
        }
        Destroy(gameObject);
    }

    float AnimationLength(string name)
    {
        float time = 0;
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;

        for (int i = 0; i < ac.animationClips.Length; i++)
            if (ac.animationClips[i].name == name)
                time = ac.animationClips[i].length;

        return time;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log(damage + " damage taken.");
    }

    Vector3 EnemyPatrol()
    {
        if (globalWaypoints.Length != 0)
        {
            if (Time.time < nextMoveTime)
            {
                return Vector3.zero;
            }
            fromWaypointIndex %= globalWaypoints.Length;
            int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;
            float distanceBetweenWaypoints = Vector3.Distance(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex]);
            percentBetweenWaypoints += Time.deltaTime * (speed / distanceBetweenWaypoints);
            percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);

            //Debug.Log(percentBetweenWaypoints);

            Vector3 newPos = Vector3.Lerp(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex], percentBetweenWaypoints);

            if (percentBetweenWaypoints >= 1)
            {
                percentBetweenWaypoints = 0;
                fromWaypointIndex++;

                if (!cyclic)
                {
                    if (fromWaypointIndex >= globalWaypoints.Length - 1)
                    {
                        fromWaypointIndex = 0;
                        System.Array.Reverse(globalWaypoints);
                    }
                }
                nextMoveTime = Time.time + waitTime;
            }

            return newPos - transform.position;
        }
        else
        {
            return Vector3.zero;
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (localWaypoints != null)
        { 
            float size = 0.3f;

            for (int i = 0; i < localWaypoints.Length; i++)
            {
                Vector3 globalWaypointPos = Application.isPlaying ? globalWaypoints[i] : localWaypoints[i] + transform.position;
                Gizmos.DrawLine(globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
                Gizmos.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);

            }
        }
        if (Application.isPlaying)
        {
            Gizmos.DrawWireCube(collider.transform.position, collider.size);
        }
    }
}
