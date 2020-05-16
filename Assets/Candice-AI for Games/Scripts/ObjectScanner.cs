using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ViridaxGameStudios.AI
{
    public class ObjectScanner
    {
        AIController aiController;
        public Action<bool, GameObject> callback;
        public ObjectScanner(AIController aiController, Action<bool, GameObject> _callback)
        {
            this.aiController = aiController;
            callback = _callback;
            string [] className = (this.ToString()).Split('.');
            Debug.Log(className[className.Length-1] + ": Initialised.");
        }
        public void ScanForObjects(Vector3 center, float radius)
        {
            //Array that will store all collided objects
            Collider[] hitColliders = Physics.OverlapSphere(center, radius);
            GameObject priorityEnemy = null;
            int lowestLevel = int.MaxValue;
            //Loop though each object
            foreach (Collider collider in hitColliders)

            {
                GameObject go = collider.gameObject;
                float distance = Vector3.Distance(aiController.transform.position, go.transform.position);
                float angle = Vector3.Angle(go.transform.position - aiController.transform.position, aiController.transform.forward);
                Debug.Log("Angle: " + angle);
                //Check if the object is in the enemy tag list
                if (aiController.enemyTags.Contains(go.tag) && angle <= aiController.m_LineOfSight/2 && !aiController.isDead)
                {
                    //Character character = go.GetComponent<Character>();
                    /*if (character.level < lowestLevel)
                    {
                        priorityEnemy = go;
                        lowestLevel = character.level;
                    }*/
                    priorityEnemy = go;
                }

            }
            ProcessObjects(priorityEnemy);


        }
        void ProcessObjects(GameObject priorityEnemy)
        {
            bool enemyFound = false;
            if (priorityEnemy != null)
            {
                //aiController.target = priorityEnemy;
                enemyFound = true;
            }
            else
            {
                //aiController.target = null;
                enemyFound = false;
            }
            callback(enemyFound, priorityEnemy);
        }
    }
}

