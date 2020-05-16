using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace ViridaxGameStudios.AI
{
    public class AIController : Character
    {
        #region Variables
        public int agentID;
        CandiceAIManager candice;
        public BehaviorTree behaviorTree;
        bool playerDead = false;
        public float m_DetectionRadius = 10f;
        public float m_DetectionHeight = 2f;
        public float m_LineOfSight = 180.0f;
        public bool enemyFound = false;//Bolean to know if the enemy has been found    
        public bool isPlayerControlled;
        public bool followPlayer;
        public float oaDistance = 10f;//Obstacle Avoidance Distance. How far the agent will detect objects to avoid.
        /*
         * Pathfinding Variables
         */
        Path path;//The path that the Agent will use to follow.
        const float minPathUpdateTime = .2f;//Minimum time it will take for the agent before attempting to request a new updated path from Candice.
        const float pathUpdateMoveThreshold = .5f;// Minimum distance the target can move by before requesting a new Updated path from Candice.
        public float turnSpeed;//The speed the agent will turn by when pathfinding.
        public float turnDist;//the ditance the agent will turn while moving to the next node.
        public float stoppingDist;//How far away from the target the agent will start to slow down and stop.
        public bool stopBehaviorTree = false;
        Coroutine updatePathCoroutine;
        


        #endregion


        #region Main Methods
        private void Awake()
        {
            
            Animator = GetComponent<Animator>();
            ragdoll = GetComponentsInChildren<Rigidbody>();
            
            if (is3D)
            {
                if (enableRagdoll)
                {
                    EnableRagdoll();
                }
                else
                {
                    DisableRagdoll();
                }
            }
            
            
        }//End Awake()
        public override void Start()
        {
            //Call start from the base class.
            base.Start();
            
            //behaviorTree.Initialise(transform, this);
            
            
            
            //CreateBehaviorTree(bt.GetNodes(), lstFunctions, bt.GetConnections());

            //CreateBehaviorTree(behaviorTreeScript.nodes, lstFunctions, behaviorTreeScript.connections);
            //Instantiate core modules.
            
            if(isPlayerControlled)
            {
                if (healthBar != null)
                    healthBar.SetAgentName("Player");
                movePoint = new GameObject("movePoint");
                SphereCollider col = movePoint.AddComponent<SphereCollider>();
                col.isTrigger = true;
                col.radius = 0.75f;
            }
            else
            {
                if(behaviorTree != null)
                {
                    if(behaviorTree.behaviorTree != null)
                    {
                        behaviorTree.CreateBehaviorTree(this);
                    }
                }
            }
            
            candice = FindObjectOfType<CandiceAIManager>();
            //Subscribe to Events
            CandiceAIManager.instance.OnDestinationReached += onDestinationReached;
            CandiceAIManager.instance.OnCharacterDead += onCharacterDead;
            CandiceAIManager.instance.OnPlayerHealthLow += onPlayerHealthLow;
            CandiceAIManager.instance.OnPlayerDetected += onPlayerDetected;
            try
            {
                candice.RegisterAgent(gameObject, onRegistrationComplete);
            }
            catch(Exception e)
            {
                if (CandiceConfig.enableDebug)
                    Debug.Log("Unable to register agent with Candice. Make sure that there is a GameObject that has the CandiceAIManager script attached.");
            }
            
            //Initialise all actions
            idleAction.Init(Animator, "ToFollow");
            followMoveAction.Init(transform, Animator, "ToIdle");
            patrolMoveAction.Init(transform, Animator, "ToIdle");
            patrolAction.Init(transform, Animator, "ToIdle");
            attackAction.Init(Animator);
            fsm.Start(CharacterStates.STATE_IDLE);
            

        }//End Start()

        
        private void onRegistrationComplete(bool isRegistered, int id)
        {
            if (isRegistered)
            {
                agentID = id;
                if(!isPlayerControlled)
                {
                    if (healthBar != null)
                        healthBar.SetAgentName("AI Agent " + id);
                }
                if (CandiceConfig.enableDebug)
                    Debug.Log("Agent " + agentID + " successfully registered with Candice.");
            }
            else
            {
                if (CandiceConfig.enableDebug)
                    Debug.Log("Unable to register agent with Candice.");
            }
            //Debug.Log("Agent " + agentID + " successfully registered with Candice.");

        }
        // Update is called once per frame
        public override void Update()
        {
            base.Update();
            //Debug.Log("Enemy Found: " + enemyFound);
            
            
            
        }
        void LateUpdate()
        {
            fsm.Update();//Call the update method of the Finite State Machine that will also call the Update method of the corresponding State and Actions.
            if (isPlayerControlled)
            {
                ProcessInput();
            }
            else
            {
                //ProcessState();//Using various conditions, determine what state to be in.
                //objectScanner.ScanForObjects(gameObject.transform.position, m_DetectionRadius);//Scan for objects and ultimately enemies using the m_DetectionRadius.
                CandiceAIManager.ScanForObjects(gameObject.transform.position, m_DetectionRadius, this, onObjectFound);
                if(behaviorTree != null)
                {
                    if (behaviorTree.rootNode != null)
                        behaviorTree.rootNode.Evaluate();
                }
            }
        }
        private void OnAnimatorIK(int layerIndex)
        {
            if (enableHeadLook && headLookTarget != null)
            {
                Animator.SetLookAtPosition(headLookTarget.transform.position);//Enable the agent to look at the target object
                Animator.SetLookAtWeight(headLookIntensity);//Set the intensity of how much the agent will turn their head to look at the target.
            }
        }
        #endregion

        
        #region Override Methods
        public override void CharacterDead()
        {
            candice.CharacterDead(gameObject);
            //Unsubscribe Events
            CandiceAIManager.instance.OnDestinationReached -= onDestinationReached;
            CandiceAIManager.instance.OnCharacterDead -= onCharacterDead;
            CandiceAIManager.instance.OnPlayerHealthLow -= onPlayerHealthLow;
            CandiceAIManager.instance.OnPlayerDetected -= onPlayerDetected;
            base.CharacterDead();
        }

        #endregion

        #region Helper Methods 
        public void onPlayerHealthLow(GameObject player)
        {
            if (CandiceConfig.enableDebug)
                Debug.Log("The players health is low");
        }
        void onCharacterDead(GameObject go)
        {
            if (gameObject.tag.Equals("Player"))
            {
                playerDead = true;
            }
        }
        public void EnableRagdoll()
        {
            if(ragdoll != null && ragdoll.Length > 0)
            {
                foreach (Rigidbody rb in ragdoll)
                {
                    rb.isKinematic = false;
                    rb.useGravity = true;
                    //rb.WakeUp();
                    //rb.gameObject.GetComponent<Collider>().isTrigger = false;
                    rb.gameObject.GetComponent<Collider>().enabled = true;
                }
            }
            Animator.enabled = false;
            GetComponent<Collider>().enabled = false;
            GetComponent<Rigidbody>().useGravity = false;
            //gameObject.GetComponent<Rigidbody>().isKinematic = true;

        }
        void DisableRagdoll()
        {
            Animator.enabled = true;
            if (ragdoll != null && ragdoll.Length > 0)
            {
                foreach (Rigidbody rb in ragdoll)
                {
                    rb.isKinematic = true;
                    rb.useGravity = false;
                    //rb.Sleep();
                    //rb.gameObject.GetComponent<Collider>().isTrigger = true;
                    rb.gameObject.GetComponent<Collider>().enabled = false;
                }
            }
            Animator.enabled = true;
            gameObject.GetComponent<Collider>().enabled = true;
            gameObject.GetComponent<Rigidbody>().useGravity = true;
            gameObject.GetComponent<Rigidbody>().isKinematic = false;
        }
        
        public void StartFinding()
        {
            if(updatePathCoroutine == null)
            {
                //Start moving the agent using Candice's pathfinding module.
                updatePathCoroutine = StartCoroutine(UpdatePath());
            }
        }
        public void StopFinding()
        {
            //Stop moving the agent using Candice's pathfinding module.
            StopCoroutine(UpdatePath());
            StopCoroutine("FollowPath");
        }
        public void ObstacleAvoidance(Transform Target, Transform transform, float size)
        {
            CandiceAIManager.ObstacleAvoidance(Target, transform, size, movementSpeed, is3D, oaDistance);
        }
        void onDestinationReached()
        {
            fsm.ChangeToState(idleState);
            if(isPlayerControlled && movePoint != null)
            {
                movePoint.GetComponent<Collider>().enabled = false;
                if(CandiceConfig.enableDebug)
                    Debug.Log("Dstination reached");
            }
            

        }
        void onObjectFound(bool foundEnemy, GameObject enemy)
        {
            enemyFound = foundEnemy;
            if (enemyFound)
            {
                target = enemy;
                if(target.tag.Equals("Player"))
                {
                    CandiceAIManager.instance.PlayerDetected(target, target);
                    

                }

                //movePoint = enemy.transform.position;
            }
            if (!isPatrolling && !enemyFound)
            {
                target = null;
            }
        }
        void onPlayerDetected(GameObject source, GameObject player)
        {
            //Implement logic here for when the player is detected
            //Debug.Log("Player Detected");
        }
        public void OnPathFound(Vector3[] waypoints, bool pathSuccessful)
        {
            if (pathSuccessful)
            {
                path = new Path(waypoints, transform.position, turnDist, stoppingDist);
                StopCoroutine("FollowPath");
                StartCoroutine("FollowPath");
            }
        }
        IEnumerator UpdatePath()
        {
            if (Time.timeSinceLevelLoad < .3f)
            {
                yield return new WaitForSeconds(.3f);
            }
            CandiceAIManager.RequestPath(new PathRequest(transform.position, target.transform.position, OnPathFound));
            float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
            Vector3 targetPosOld = target.transform.position;

            while (true)
            {
                yield return new WaitForSeconds(minPathUpdateTime);
                if (target != null)
                {
                    if ((target.transform.position - targetPosOld).sqrMagnitude > sqrMoveThreshold)
                    {
                        CandiceAIManager.RequestPath(new PathRequest(transform.position, target.transform.position, OnPathFound));
                        targetPosOld = target.transform.position;
                    }
                }
                if (!isMoving)
                {
                    StopCoroutine(updatePathCoroutine);
                    StopCoroutine("FollowPath");
                    updatePathCoroutine = null;
                }
            }

        }

        IEnumerator FollowPath()
        {
            bool followingPath = true;
            int pathIndex = 0;
            if(is3D)
                transform.LookAt(path.lookPoints[0]);
            float speedPercent = 1;
            while (followingPath)
            {
                
                Vector2 pos2D = new Vector2(transform.position.x, transform.position.z);
                while (path.turnBoundaries[pathIndex].HasCrossedLine(pos2D))
                {
                    if (pathIndex == path.finishLineIndex)
                    {
                        followingPath = false;
                        break;
                    }
                    else
                    {
                        pathIndex++;
                        if(is3D)
                            transform.LookAt(path.lookPoints[pathIndex]);
                    }
                }
                if (followingPath)
                {
                    if (pathIndex >= path.slowDownIndex && stoppingDist > 0)
                    {
                        speedPercent = Mathf.Clamp01(path.turnBoundaries[path.finishLineIndex].DistanceFromPoint(pos2D) / stoppingDist);
                        if (speedPercent < 0.01f)
                        {
                            followingPath = false;
                        }
                    }

                    if(is3D)
                    {
                        Quaternion targetRotation = Quaternion.LookRotation(path.lookPoints[pathIndex] - transform.position);
                        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
                        transform.Translate(Vector3.forward * Time.deltaTime * movementSpeed * speedPercent, Space.Self);
                    }
                    else
                    {
                        //Quaternion targetRotation = Quaternion.LookRotation(path.lookPoints[pathIndex] - transform.position);
                        //transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
                        //Debug.Log(path.lookPoints[pathIndex]);
                        //Vector3 lookPoint = path.lookPoints[pathIndex];
                        //transform.position += path.lookPoints[pathIndex];
                        CandiceAIManager.SimpleMove(transform, path.lookPoints[pathIndex], movementSpeed, is3D);
                        //transform.Translate( Vector3.forward * Time.deltaTime * movementSpeed * speedPercent, Space.Self);
                    }
                    
                }
                yield return null;
            }
        }

        

        public void OnDrawGizmos()
        {
            if (path != null)
            {
                path.DrawWithGizmos();
            }
            
            if (path != null)
            {
                for (int i = 0; i < path.lookPoints.Length; i++)
                {
                    Gizmos.color = Color.black;
                    if (i != 0)
                    {
                        Gizmos.DrawLine(path.lookPoints[i - 1], path.lookPoints[i]);
                    }

                }
            }
        }
        #endregion
        private void OnTriggerEnter(Collider other)
        {
            if(isPatrolling && target != null)
            {
                if (other.gameObject.tag.Equals("PatrolPoint") && other.gameObject.name.Equals(target.gameObject.name))
                {
                    pointReached = true;
                    if (CandiceConfig.enableDebug)
                        Debug.Log("Patrol Point reached");
                }
            }

            if (other.gameObject.name == "movePoint" && isPlayerControlled)
            {
                candice.DestinationReached();
            }

        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if(isPatrolling)
            {
                if (other.gameObject.tag.Equals("PatrolPoint") && other.gameObject.name.Equals(target.gameObject.name))
                {
                    pointReached = true;
                    if (CandiceConfig.enableDebug)
                        Debug.Log("Patrol Point reached");
                }
            }
            if (other.gameObject.name == "movePoint" && isPlayerControlled)
            {
                candice.DestinationReached();
            }


        }
        

    }
    [Serializable]
    public class SpecialAbility
    {
        public string abilityName = "<<Not Set>>";
        public KeyCode abilityKey;
    }

    
}

