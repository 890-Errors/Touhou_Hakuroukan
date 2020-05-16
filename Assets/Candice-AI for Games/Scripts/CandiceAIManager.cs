using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

namespace ViridaxGameStudios.AI
{
    [DisallowMultipleComponent()]
    public class CandiceAIManager : MonoBehaviour
    {
        public bool enableDebug;//   
        public static CandiceAIManager instance;
        private static ObstacleAvoidance obstacleAvoidance;//Obstacle avoidance module to allow the agent to move and evade obstacles.
        private Queue<PathResult> results = new Queue<PathResult>();//Data strucure containing a collection of all paths requested by all AI Agents/Controllers
        private PathFinding pathFinding;//Pathfinding module that does the actual calculations to find a path.
        private Grid grid;//The grid that contains all the nodes
        
        
        Queue<RegistrationRequest> registrationQueue = new Queue<RegistrationRequest>();

        public Dictionary<int, GameObject> agents = new Dictionary<int, GameObject>();
        int agentCount = 0;

        public static string[] arrNodeTypes = { "Selector", "Sequence", "Inverter", "Action" };
        public static string[] arrFunctions = { "None", "MoveTo", "LookAt", "Attack", "EnemyDetected" };
        public const int NODE_TYPE_SELECTOR = 0;
        public const int NODE_TYPE_SEQUENCE = 1;
        public const int NODE_TYPE_INVERTER = 2;
        public const int NODE_TYPE_ACTION = 3;

        #region Events
        public event Action<GameObject, GameObject> OnPlayerDetected = delegate { };
        public event Action<GameObject> OnPlayerHealthLow = delegate { };
        public event Action OnDestinationReached = delegate { };
        public event Action<GameObject> OnCharacterDead = delegate { };

        public void PlayerDetected(GameObject source, GameObject player)
        {
            OnPlayerDetected(source, player);
        }
        public void CharacterDead(GameObject go)
        {
            OnCharacterDead(go);
        }
        public void PlayerHealthLow(GameObject player)
        {
            OnPlayerHealthLow(player);
        }
        public void DestinationReached()
        {
            OnDestinationReached();
        }
        #endregion

        private void Awake()
        {
            Initialise();
        }
        
        private void Start()
        {
            
        }
        private void Initialise()
        {
            CandiceConfig.enableDebug = enableDebug;
            instance = this;
            grid = GetComponent<Grid>();
            pathFinding = new PathFinding(grid);
            obstacleAvoidance = new ObstacleAvoidance();
        }
        private void Update()
        {
            CandiceConfig.enableDebug = enableDebug;
            if (results.Count > 0)
            {
                int itemsInQueue = results.Count;
                lock (results)
                {
                    for (int i = 0; i < itemsInQueue; i++)
                    {
                        PathResult result = results.Dequeue();
                        result.callback(result.path, result.success);
                    }
                }
            }

            if(registrationQueue.Count > 0)
            {
                int itemsInQueue = registrationQueue.Count;
                lock (registrationQueue)
                {
                    for (int i = 0; i < itemsInQueue; i++)
                    {
                        RegistrationRequest rr = registrationQueue.Dequeue();
                        bool isRegistered;
                        try
                        {
                            agentCount++;
                            agents.Add(agentCount, rr.agent);
                            isRegistered = true;
                        }
                        catch(Exception e)
                        {
                            isRegistered = false;
                        }
                        rr.callback(isRegistered, agentCount);
                    }
                }
            }
        }
        #region A* Pathfinding
        //This method is called by the AI agents in order to receive a path to their goal, using the Pathfinding module.
        public static void RequestPath(PathRequest request)
        {
            ThreadStart threadStart = delegate
            {
                instance.pathFinding.FindPath(request, instance.FinishedProcessingPath);
            };
            threadStart.Invoke();
        }
        public void FinishedProcessingPath(PathResult result)
        {
            lock (results)
            {
                //Add the result to the queue.
                results.Enqueue(result);
            }

        }
        #endregion
        #region SimpleMovement
        public static void SimpleMove(Transform transform, Vector3 target, float movementSpeed, bool is3D)
        {
            if (is3D)
            {
                transform.LookAt(target);
                transform.position += transform.forward * movementSpeed * Time.deltaTime;
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, target, movementSpeed * Time.deltaTime);
            }
        }
        public static void SimpleMove(Transform transform, Transform target, float movementSpeed, bool is3D)
        {
            SimpleMove(transform, target.position, movementSpeed, is3D);
            
        }
        #endregion
        #region Obstacle Avoidance
        public static void ObstacleAvoidance(Transform Target, Transform transform, float size, float movementSpeed, bool is3D, float _distance)
        {
            if(is3D)
            {
                RaycastHit hit;
                float distance = Vector3.Distance(transform.position, Target.transform.position);
                if (Physics.Raycast(transform.position, Target.transform.position, out hit))
                {
                    if (hit.transform != Target.transform)
                    {
                        obstacleAvoidance.Move(Target, transform, size, movementSpeed, is3D, _distance);
                    }
                    else
                    {
                        SimpleMove(transform, Target.transform, movementSpeed, is3D);
                    }
                }
                else
                {
                    obstacleAvoidance.Move(Target, transform, size, movementSpeed, is3D, _distance);
                }
            }
            else
            {
                RaycastHit2D hit;
                float distance = Vector3.Distance(transform.position, Target.transform.position);
                if (hit = Physics2D.Raycast(transform.position, Target.transform.position))
                {
                    if (hit.transform != Target.transform)
                    {
                        obstacleAvoidance.Move(Target, transform, size, movementSpeed, is3D, _distance);
                    }
                    else
                    {
                        SimpleMove(transform, Target.transform, movementSpeed, is3D);
                    }
                }
                else
                {
                    obstacleAvoidance.Move(Target, transform, size, movementSpeed, is3D, _distance);
                }
            }
            
            
        }
        #endregion
        #region Object Detection
        public static void ScanForObjects(Vector3 center, float radius, AIController aiController, Action<bool, GameObject> _callback)
        {
            GameObject priorityEnemy = null;
            if (aiController.is3D)
            {
                //Array that will store all collided objects
                //Collider[] hitColliders = Physics.OverlapSphere(center, radius);
                Vector3 halfExtents = new Vector3(radius, aiController.m_DetectionHeight, radius);
                Collider[] hitColliders = Physics.OverlapBox(center, halfExtents);

                //Loop though each object
                foreach (Collider collider in hitColliders)
                {
                    GameObject go = collider.gameObject;
                    float distance = Vector3.Distance(aiController.transform.position, go.transform.position);
                    float angle = Vector3.Angle(go.transform.position - aiController.transform.position, aiController.transform.forward);
                    //Check if the object is in the enemy tag list
                    if (aiController.enemyTags.Contains(go.tag) && angle <= aiController.m_LineOfSight / 2 && !aiController.isDead)
                    {
                        priorityEnemy = go;
                    }

                }
            }
            else
            {
                //Array that will store all collided objects
                Collider2D[] hitColliders = Physics2D.OverlapCircleAll(new Vector2(center.x, center.y), radius);

                //Loop though each object
                foreach (Collider2D collider in hitColliders)
                {
                    GameObject go = collider.gameObject;
                    float distance = Vector3.Distance(aiController.transform.position, go.transform.position);
                    //float angle = Vector3.Angle(go.transform.position - aiController.transform.position, aiController.transform.forward);
                    //Check if the object is in the enemy tag list
                    if (aiController.enemyTags.Contains(go.tag))
                    {
                        priorityEnemy = go;
                    }

                }
            }
            
            ProcessObjects(priorityEnemy, _callback);

        }

        static void ProcessObjects(GameObject priorityEnemy, Action<bool, GameObject> callback)
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
        #endregion

        public void RegisterAgent(GameObject agent, Action<bool,int> callback)
        {
            instance.registrationQueue.Enqueue(new RegistrationRequest(agent, callback));
        }
    }
    public struct PathResult
    {
        public Vector3[] path;
        public bool success;
        public Action<Vector3[], bool> callback;

        public PathResult(Vector3[] path, bool success, Action<Vector3[], bool> callback)
        {
            this.path = path;
            this.success = success;
            this.callback = callback;
        }
    }
    public struct PathRequest
    {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[], bool> callback;

        public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback)
        {
            pathStart = _start;
            pathEnd = _end;
            callback = _callback;
        }
    }

    public struct RegistrationRequest
    {
        public GameObject agent;
        public Action<bool, int> callback;

        public RegistrationRequest(GameObject _agent, Action<bool, int> _callback)
        {
            agent = _agent;
            callback = _callback;
        }
    }

}
