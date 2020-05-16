using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ViridaxGameStudios.AI
{
    public class ObstacleAvoidance
    {

        public ObstacleAvoidance()
        {
            string[] className = (this.ToString()).Split('.');
            if (CandiceConfig.enableDebug)
                Debug.Log(className[className.Length - 1] + ": Initialised.");
        }
        public void Move(Transform Target, Transform transform, float size, float movementSpeed, bool is3D, float distance)
        {
            //
            //Method Name : void Move(Transform Target, Transform transform, float size)
            //Purpose     : This method moves the agent while avoiding immediate obstacles.
            //Re-use      : none
            //Input       : Transform Target, Transform transform, float size
            //Output      : void
            //
            if(!is3D)
            {
                Move2D(Target, transform, size, movementSpeed, distance);
                return;
            }
            Vector3 dir = (Target.position - transform.position).normalized;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, distance))
            {
                if (hit.transform != transform && hit.transform != Target.transform)
                {
                    Debug.DrawLine(transform.position, hit.point, Color.red);
                    dir += hit.normal * 50;
                }
            }

            Vector3 left = transform.position;
            Vector3 right = transform.position;

            left.x -= size;
            right.x += size;
            if (Physics.Raycast(left, transform.forward, out hit, distance))
            {
                if (hit.transform != transform && hit.transform != Target.transform)
                {
                    Debug.DrawLine(left, hit.point, Color.red);
                    dir += hit.normal * 50;

                }
            }

            if (Physics.Raycast(right, transform.forward, out hit, distance))
            {
                if (hit.transform != transform && hit.transform != Target.transform)
                {
                    Debug.DrawLine(right, hit.point, Color.red);
                    dir += hit.normal * 50;
                }
            }
            Quaternion rot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime);
            transform.position += transform.forward * movementSpeed * Time.deltaTime;
        }

        public void Move2D(Transform Target, Transform transform, float size, float movementSpeed, float distance)
        {
            
            Vector2 dir = (Target.position - transform.position).normalized;
            RaycastHit2D hit;
            if ((hit = Physics2D.Raycast(transform.position, transform.forward, distance)))
            {
                Debug.Log("OA 2D");
                if (hit.transform != transform && hit.transform != Target.transform)
                {
                    Debug.DrawLine(transform.position, hit.point, Color.red);
                    dir += hit.normal * 50;
                }
            }

            Vector3 left = transform.position;
            Vector3 right = transform.position;

            left.x -= size;
            right.x += size;
            if ((hit = Physics2D.Raycast(left, transform.forward, distance)))
            {
                if (hit.transform != transform && hit.transform != Target.transform)
                {
                    Debug.DrawLine(left, hit.point, Color.red);
                    dir += hit.normal * 50;

                }
            }

            if ((hit = Physics2D.Raycast(right, transform.forward, distance)))
            {
                if (hit.transform != transform && hit.transform != Target.transform)
                {
                    Debug.DrawLine(right, hit.point, Color.red);
                    dir += hit.normal * 50;
                }
            }
            //Quaternion rot = Quaternion.LookRotation(dir);
            //transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime);
            transform.position += new Vector3(dir.x,dir.y) * movementSpeed * Time.deltaTime;


        }
    }
}

