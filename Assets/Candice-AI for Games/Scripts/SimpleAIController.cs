using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ViridaxGameStudios.AI
{
    public class SimpleAIController : MonoBehaviour
    {
        [Tooltip("The target to follow")]
        public GameObject target;
        public Rigidbody rb;
        [Tooltip("Whether the agent should follow the target. If false, it will just move to the point and stop.")]
        public bool followTarget = false;
        [Tooltip("If true, the agent will attempt to evade all obstacles.")]
        public bool obstacleAvoidance = false;
        [Tooltip("The speed the object will move at")]
        public float speed = 10f;
        public float stoppingDistance = 0;
        public bool destroyOnCollision = false;
        [Tooltip("How much time (in seconds) must elapse before the object destroys itself, after colliding.")]
        public float colisionDelay = 3f;
        public bool destroyAfterDelay;
        [Tooltip("How much time (in seconds) must elapse before the object destroys itself.")]
        public float delay = 3f;
        public bool stopOnCollision = false;
        public bool orbit = false;
        float t = 0;
        public bool rotate = false;
        public float rotationSpeed = 1f;
        
        private Vector3 point;
        bool pointSet = false;
        public bool fireTrue = false;
        bool stop = false;
        Vector3 direction;
        float timeElapsed = 0f;
        int pointIncPerSec = 1;
        GameObject source;
        public bool is3D = true;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }
        void Start()
        {

            //transform.rotation = Quaternion.LookRotation(rb.velocity);
        }
        // Update is called once per frame
        void Update()
        {
            timeElapsed += pointIncPerSec * Time.deltaTime;
            if (destroyAfterDelay && timeElapsed > delay)
            {
                Destroy(gameObject);
            }
            
            if (fireTrue)
            {
                //transform.rotation = Quaternion.LookRotation(rb.velocity);
                if (rotate)
                    transform.Rotate(new Vector3(0, 0, 1), rotationSpeed);
                else if (orbit == true)
                    Orbit();
                else if (obstacleAvoidance)
                    CandiceAIManager.ObstacleAvoidance(target.transform, transform, transform.localScale.x, speed, is3D, 10);
                //ObstacleAvoidance(target.transform);
                else if (followTarget)
                    Seek();
                else
                {
                    if (!pointSet)
                    {
                        point = target.transform.position;
                        transform.LookAt(target.transform);
                        pointSet = true;
                    }
                    Move();
                }
            }

        }
        public void Fire(GameObject source)
        {
            this.source = source;
            fireTrue = true;
            Vector3 pos = target.transform.position; ;

            Collider col = target.transform.GetComponent<Collider>();
            if (col != null)
            {
                if (col is CapsuleCollider)
                {
                    pos = target.transform.TransformPoint((col as CapsuleCollider).center);
                }
                else if (col is BoxCollider)
                {
                    pos = target.transform.TransformPoint((col as BoxCollider).center);
                }
                else if (col is SphereCollider)
                {
                    pos = target.transform.TransformPoint((col as SphereCollider).center);
                }
            }




            transform.LookAt(pos);
            direction = transform.forward;
            rb.velocity = direction * speed;
        }
        private void Move()
        {
            rb.velocity = direction * speed;
            //transform.position += transform.forward * speed * Time.deltaTime;
        }
        private void Seek()
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance > stoppingDistance)
            {
                Quaternion targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 12 * Time.deltaTime);
                transform.position += transform.forward * speed * Time.deltaTime;
            }

        }
        private void Orbit()
        {
            float newX = Mathf.Cos(t);
            float newZ = Mathf.Sin(t);
            if (target != null)
                transform.position = new Vector3(newX, target.transform.position.y, newZ);
            else
                transform.position = new Vector3(newX, transform.position.y, newZ);
            t += 0.03f;
        }
        void OnTriggerEnter(Collider collider)
        {
            DealDamage(collider.gameObject);
            //Check if destroyOnCollision is enabled and check if collided object is the target. 
            if (destroyOnCollision && collider.gameObject == target.gameObject)
            {
                Debug.Log("Collided with: " + collider.gameObject.name);
                StartCoroutine(DestroyAfterCollisionDelay());


                if (stopOnCollision)
                {
                    fireTrue = false;
                    gameObject.transform.SetParent(collider.gameObject.transform);
                    rb.constraints = RigidbodyConstraints.FreezeAll;
                }
                
            }
        }
        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject != source && fireTrue)
            {
                //Debug.Log("Collided with: " + collision.gameObject.name);
                //Debug.Log("Fire True: " + fireTrue);
                DealDamage(collision.gameObject);
                if (stopOnCollision)
                {
                    fireTrue = false;
                    gameObject.transform.SetParent(collision.gameObject.transform);
                    rb.constraints = RigidbodyConstraints.FreezeAll;
                }
                if (destroyOnCollision)
                {
                    StartCoroutine(DestroyAfterCollisionDelay());
                    
                }
            }
        }
        void DealDamage(GameObject go)
        {
            try
            {
                go.SendMessage("ReceiveDamage", 20f);
            }
            catch (Exception e)
            {

            }

        }
        IEnumerator DestroyAfterCollisionDelay()
        {
            yield return new WaitForSeconds(colisionDelay);
            Destroy(gameObject);

        }
        private void ObstacleAvoidance(Transform Target)
        {
            Vector3 dir = (Target.position - transform.position).normalized;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, 20))
            {
                if (hit.transform != transform && hit.transform != Target.transform)
                {
                    Debug.DrawLine(transform.position, hit.point, Color.red);
                    dir += hit.normal * 50;
                }
            }

            Vector3 left = transform.position;
            Vector3 right = transform.position;

            left.x -= 2;
            right.x += 2;
            if (Physics.Raycast(left, transform.forward, out hit, 20))
            {
                if (hit.transform != transform && hit.transform != Target.transform)
                {
                    Debug.DrawLine(left, hit.point, Color.red);
                    dir += hit.normal * 50;

                }
            }

            if (Physics.Raycast(right, transform.forward, out hit, 20))
            {
                if (hit.transform != transform && hit.transform != Target.transform)
                {
                    Debug.DrawLine(right, hit.point, Color.red);
                    dir += hit.normal * 50;
                }
            }
            Quaternion rot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime);
            transform.position += transform.forward * 5 * Time.deltaTime;

        }

    }

}
