using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
namespace ViridaxGameStudios.AI
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class PlayerController : MonoBehaviour
    {
        float speed = 7.0f;
        float rotationSpeed = 100.0f;
        public Camera cam;
        private NavMeshAgent navMeshAgent;
        // Start is called before the first frame update
        void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    navMeshAgent.SetDestination(hit.point);
                }
            }
            /*
            float translation = (Input.GetAxis("Vertical") * speed) * Time.deltaTime;
            float rotation = (Input.GetAxis("Horizontal") * rotationSpeed) * Time.deltaTime;
            transform.Translate(0, 0, translation);
            transform.Rotate(0, rotation, 0);
            */

        }

        public void ReceiveDamage(float damage)
        {
            if (CandiceConfig.enableDebug)
                Debug.Log("PLAYER_CONTROLLER: Player hit with " + damage + " damage!");
        }
    }

}
