using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.Tilemaps;

public class SeijaAI : MonoBehaviour
{
    public Transform target;
    public float speed = 200f;
    public float nextWaypointDistance = 3f;
    public float updatePathTime = 0.5f;
    public float stopDistance = 3f;
    private RaycastHit2D[] hits;

    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;

    Seeker seeker;
    Rigidbody2D rb;
    Collider2D cd;

    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<Collider2D>();

        seeker.StartPath(rb.position, target.position, OnPathComplete);
        //每隔段时间更新一下寻路路径
        InvokeRepeating("UpdatePath", 0f, updatePathTime);
    }

    void UpdatePath()
    {
        if (path.IsDone())
            seeker.StartPath(rb.position, target.position, OnPathComplete);
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (path == null) return;
        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        //计算移动方向
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        if (Vector2.Distance(rb.position, (Vector2)target.position) >= stopDistance
            || Physics2D.RaycastAll(rb.position, (Vector2)target.position, stopDistance)[1].collider is TilemapCollider2D)
        {
            rb.AddForce(force);
        }

        rb.GetComponent<SpriteRenderer>().flipX = force.x <=0 ? true : false;
    }
}
