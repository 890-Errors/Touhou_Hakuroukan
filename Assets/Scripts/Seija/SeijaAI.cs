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
    public GameObject Mahoubakudan;
    public Seija seija;
    private RaycastHit2D[] hits;

    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;
    bool isPathing = true;

    Seeker seeker;
    Rigidbody2D rb;
    Collider2D cd;

    // Start is called before the first frame update
    void Start()
    {
        seija = GetComponent<Seija>();
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<Collider2D>();

        seeker.StartPath(rb.position, target.position, OnPathComplete);
        //每隔段时间更新一下寻路路径
        InvokeRepeating("UpdatePath", 0f, updatePathTime);
        StartCoroutine("PlantBomb");
    }

    void UpdatePath()
    {
        if (path?.IsDone() ?? false)
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

        if ((Vector2.Distance(rb.position, (Vector2)target.position) >= stopDistance
            || Physics2D.RaycastAll(rb.position, (Vector2)target.position)[1].collider is TilemapCollider2D
            )
            && isPathing)
        {
            rb.AddForce(force);
        }

        rb.GetComponent<SpriteRenderer>().flipX = target.position.x < transform.position.x ? true : false;
    }

    IEnumerator PlantBomb()
    {
        while (this.enabled)
        {
            yield return new WaitForSeconds(3.0f);
            if (rb.velocity.magnitude >= 0.001f
                && Vector2.Distance(transform.position, target.position) <= 15)
            {
                isPathing = false;
                seija.emitter.enabled = false;
                yield return new WaitForSeconds(.5f);
                if (!this.enabled) break;
                Instantiate(Mahoubakudan, transform);
                seija.emitter.enabled = true;
                isPathing = true;
            }
        }
    }
}
