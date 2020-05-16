using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace ViridaxGameStudios.AI
{
    [DisallowMultipleComponent()]
    [Serializable]
    public abstract class Character : MonoBehaviour
    {
        #region Variables
        //The main finite state machine that controls all states of this AI
        protected FSM fsm;
        //The different states that the State Machine will use.
        protected FSMState idleState;
        protected FSMState followState;
        protected FSMState fleeState;
        protected FSMState patrolState;
        protected FSMState attackState;
        protected FSMState guardState;
        protected FSMState dameState;
        protected FSMState deadState;
        //The different actions that will allow the AI to do things.
        protected IdleAction idleAction;
        protected MoveAction followMoveAction;
        protected MoveAction patrolMoveAction;
        protected AttackAction attackAction;
        protected PatrolAction patrolAction;


        [Header("Base Statistics")]
        [Range(1f,10f)]
        [Tooltip("This is used to calculate the attack damage based on your Strength, Intelligence and Faith.")]
        [SerializeField] public float m_StatsMultiplier = 1.75f;
        [Range(1f, 100f)]
        [Tooltip("")]
        [SerializeField]
        public int m_StatLevelUpIncrease = 5;
        [Range(1f, 100f)]
        [Tooltip("This directly affects the HitPoints and physical damage.")]
        [SerializeField] public int m_Strength = 3;
        [Range(1f, 100f)]
        [Tooltip("This directly affects the HitPoints and mental damage.")]
        [SerializeField] public int m_Intelligence = 3;
        [Range(1f, 100f)]
        [Tooltip("This directly affects the HitPoints and spiritual damage.")]
        [SerializeField] public int m_Faith = 3;
        [Tooltip("The faction that this character belongs to.")]
        public Faction faction;
        public bool healthLow;
        public float healthLowThreshold = 10f;
        public bool is3D = true;
        [Space(10)]
        [Header("Additional Statistics")]
        [Tooltip("The maximum range that the character can be in order to attack a target.")]
        public float m_AttackRange = 3f;
        public const float m_DefaultAttackRange = 3f;
        public float m_DamageAngle = 90.0f;
        public bool hasAttackAnimation = true;
        public float attacksPerSecond = 1f;
        public int attackType = 0;
        public bool isAttacking = false;
        public GameObject attackProjectile;
        public Transform spawnPosition;
        public const int ATTACK_TYPE_MELEE = 0;
        public const int ATTACK_TYPE_RANGE = 1;
        [Tooltip("All the Tags that the character will consider as an enemy. NOTE: The default reaction is to attack.")]
        public List<string> enemyTags = new List<string>();
        public List<string> allyTags = new List<string>();
        
        public bool enableHeadLook = false;
        public float headLookIntensity = 1f;
        CharacterStats stats;
        public int level = 0;
        public float attackDamage = 0;
        public Animator Animator { get; set; }
        public float HitPoints { get; set; }
        //public float MovementSpeed { get; set; }
        public float movementSpeed = 7;
        float rotationSpeed = 30;
        public bool IsStunned { get; set; } = false;

        public KeyCode keyUp;
        public KeyCode keyDown;
        public KeyCode keyLeft;
        public KeyCode keyRight;
        public KeyCode keyAttack;
        public KeyCode keyAttack2;
        public KeyCode keyMove;
        public KeyCode special1;
        public KeyCode special2;
        public KeyCode special3;
        public bool clickToMove;
        public bool isMoving = false;
        public bool isDead = false;
        public int pathfindSource;//Integer representing the pathfinding source.
        public int moveType;//Integer representing a move type from of the agent.


        //Constant variables describing the different pathfinding sources.
        public const int PATHFIND_SOURCE_CANDICE = 0;
        public const int PATHFIND_SOURCE_NAVMESH = 1;
        [Tooltip("The points in the gameworld where you want the character to patrol. They can be anything, even empty gameObjects. Note: Ensure each patrol point is tagged as 'PatrolPoint'")]
        public List<GameObject> patrolPoints = new List<GameObject>();
        [Tooltip("Whether or not the character should patrol each point in order of the list. False will allow the character to patrol randomly.")]
        public bool patrolInOrder = true;
        public bool isPatrolling = false;
        public bool pointReached = false;//Boolean to know if the agent reached to targeted patrol point.
        public GameObject rig;
        public Rigidbody[] ragdoll;
        public bool enableRagdoll = false;
        public bool enableRagdollOnDeath;
        public Camera cam;
        public bool autoAttack = true;
        public GameObject movePoint;
        public GameObject headLookTarget;//The object that the AI's head will look towards.
        public GameObject target;//The target that the AI will follow (e.g enemy, patrol point etc).
        public bool destroyOnDeath;
        public float destoyOnDeathDelay = 10f;
        public NavMeshAgent navMeshAgent;
        public HealthBarScript healthBar; 
        #endregion



        private void Awake()
        {
            
        }
        // Start is called before the first frame update
        public virtual void Start()
        {
            stats = new CharacterStats(m_StatsMultiplier, m_StatLevelUpIncrease, m_Strength, m_Intelligence, m_Faith, movementSpeed);
            navMeshAgent = GetComponent<NavMeshAgent>();
            //Create the Finite State Machine
            fsm = new FSM("AI Controller FSM");

            //Create the states
            idleState = fsm.AddState(CharacterStates.STATE_IDLE);
            followState = fsm.AddState(CharacterStates.STATE_FOLLOW);
            attackState = fsm.AddState(CharacterStates.STATE_ATTACK);
            patrolState = fsm.AddState(CharacterStates.STATE_PATROL);
            //Create the actions
            idleAction = new IdleAction(idleState, this);
            followMoveAction = new MoveAction(followState, this);
            patrolMoveAction = new MoveAction(patrolState, this);
            patrolAction = new PatrolAction(patrolState, this);
            attackAction = new AttackAction(attackState, this);

            //Add actions to the states
            idleState.AddAction(idleAction);
            followState.AddAction(followMoveAction);
            attackState.AddAction(attackAction);
            patrolState.AddAction(patrolAction);
            patrolState.AddAction(patrolMoveAction);
            //Add transitions to the states
            idleState.AddTransition("ToFollow", followState);
            followState.AddTransition("ToIdle", idleState);
            followState.AddTransition("ToAttack", attackState);
            attackState.AddTransition("ToIdle", idleState);
            attackState.AddTransition("ToFollow", followState);
            patrolState.AddTransition("ToIdle", idleState);
            patrolState.AddTransition("ToFollow", followState);
            HitPoints = stats.MaxHitPoints;
            if (healthBar != null)
                healthBar.SetMaxHealth(stats.MaxHitPoints);
        }

        // Update is called once per frame
        public virtual void Update()
        {
            UpdateStats();
        }

        private void UpdateStats()
        {
            stats.AttackRange = m_AttackRange;
            stats.DamageAngle = m_DamageAngle;
            stats.StatsMultiplier = m_StatsMultiplier;
            level = stats.Level;
            attackDamage = stats.AttackDamage;
        }
        protected void ProcessInput()
        { 
            if (clickToMove)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit))
                    {
                        movePoint.GetComponent<Collider>().enabled = true;
                        movePoint.transform.position = hit.point;
                        target = movePoint;
                        if (fsm.GetCurrentState.Name != followState.Name)
                        {
                            fsm.ChangeToState(followState);
                        }
                        else
                        {
                            navMeshAgent.SetDestination(hit.point);
                        }

                        //
                    }
                }
            }
            else
            {
                float translation = (Input.GetAxis("Vertical") * movementSpeed) * Time.deltaTime;
                float rotation = (Input.GetAxis("Horizontal") * rotationSpeed) * Time.deltaTime;

                if(is3D)
                {
                    transform.Translate(0, 0, translation);
                    transform.Rotate(0, rotation, 0);
                }
                else
                {
                    transform.Translate(rotation, translation, 0);
                }
                
                if (translation == 0)
                {
                    Animator.SetBool("isRunning", false);
                }
                else
                {
                    Animator.SetBool("isRunning", true);
                }
            }

            if (Input.GetKey(keyAttack))
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (fsm.GetCurrentState.Name != attackState.Name)
                    {
                        transform.LookAt(hit.point);
                        fsm.ChangeToState(attackState);
                    }
                }
            }
            if (Input.GetKeyDown(keyAttack2))
            {
                //Functionality for attack 2
            }
            if (Input.GetKeyDown(special1))
            {
                //Functionality for special 1
            }
            if (Input.GetKeyDown(special2))
            {
                //Functionality for special 2
            }
            if (Input.GetKeyDown(special3))
            {
                //Functionality for special 3
            }
        }
        public void StartMoveNavMesh(GameObject point)
        {
            //Move the agent using the NavMeshAgent
            movePoint.transform.position = point.transform.position;
            movePoint.GetComponent<SphereCollider>().enabled = true;
            navMeshAgent.isStopped = false;
            navMeshAgent.SetDestination(point.transform.position);
        }
        public void StopMoveNavMesh()
        {
            //Stop moving the agent
            navMeshAgent.isStopped = true;
        }
        public virtual void ReceiveDamage(float damage)
        {
            //
            //Method Name : void ReceiveDamage(float damage)
            //Purpose     : This method receives damage from various sources and applies it to the character.
            //Re-use      : none
            //Input       : float damage
            //Output      : none
            //
            IsStunned = true;
            if (HitPoints - damage <= 0)
            {
                HitPoints = 0;
                isDead = true;
                //CharacterDead() method should be called after the death animation has finished playing using an Animation Event. 
                //Alternatively, you can implement your own logic here to suit your needs.
            }
            else
            {
                HitPoints -= damage;
                if (HitPoints < healthLowThreshold)
                    healthLow = true;
                else
                    healthLow = false;
            }
            healthBar.SetHealth(HitPoints);
            if (CandiceConfig.enableDebug)
                Debug.Log("Hit with: " + damage + " damage. New Health: " + HitPoints);
            //if (CandiceConfig.enableDebug)
            //Debug.Log("Hit with: " + damage + " damage. New Health: " + HitPoints);

        }
        public virtual void ResumeFromDamage()
        {
            //
            //Method Name : void ResumeFromDamage()
            //Purpose     : This method allows the character to resume its normal functionality after the damage animation has played.
            //Re-use      : none
            //Input       : none
            //Output      : none
            //
            IsStunned = false;
        }
        public virtual void CharacterDead()
        {
            //
            //Method Name : void CharacterDead()
            //Purpose     : This method destroys the character GameObject as soon as the death animation has finished playing.
            //Re-use      : none
            //Input       : none
            //Output      : none
            //
            if (destroyOnDeath)
            {
                Destroy(gameObject, destoyOnDeathDelay);
            }
        }
        public void Attack()
        {
            if (hasAttackAnimation)
                DealDamage();
            else if (!isAttacking)
            {
                isAttacking = true;
                StartCoroutine(DealTimedDamage(1f / attacksPerSecond));
                
            }
        }
        public IEnumerator DealTimedDamage(float time)
        {
            yield return new WaitForSecondsRealtime(time);
            DealDamage();
        }
        public void DealDamage()
        {
            //
            //Method Name : void Attack()
            //Purpose     : This method is called by the attack animation event. Deals the required damage to all targets in range..
            //Re-use      : none
            //Input       : none
            //Output      : none
            //
            
            if (is3D)
            {
                RaycastHit[] hits;
                hits = Physics.SphereCastAll(transform.position, m_AttackRange, transform.forward);//Send a shere cast to see if it hits anything
                foreach (RaycastHit hit in hits)//for each object hit by the spherecast
                {
                    bool isHittable = false;
                    foreach (string tag in enemyTags)
                    {
                        //if the hit object is an enemy
                        if (tag.Equals(hit.transform.gameObject.tag))
                        {
                            isHittable = true;
                        }
                    }
                    //if the object is hittable
                    if (isHittable)
                    {
                        float distance = Vector3.Distance(transform.position, hit.transform.position);
                        float angle = Vector3.Angle(hit.transform.position - transform.position, transform.forward);
                        if (angle <= m_DamageAngle / 2 && distance <= m_AttackRange)//If the object is within the attack range and within the damage angle.
                        {
                            hit.transform.gameObject.SendMessage("ReceiveDamage", attackDamage);//send the damage to the hit object. The hit object needs to have a script with the method ReceiveDamage(float damage);
                        }
                    }
                }
            }
            else
            {
                RaycastHit2D[] hits;
                hits = Physics2D.CircleCastAll(new Vector2(transform.position.x, transform.position.y), m_AttackRange, transform.up);//Send a shere cast to see if it hits anything
                foreach (RaycastHit2D hit in hits)//for each object hit by the spherecast
                {
                    bool isHittable = false;
                    foreach (string tag in enemyTags)
                    {
                        //if the hit object is an enemy
                        if (tag.Equals(hit.transform.gameObject.tag))
                        {
                            isHittable = true;
                        }
                    }
                    //if the object is hittable
                    if (isHittable)
                    {
                        float distance = Vector3.Distance(transform.position, hit.transform.position);
                        if (distance <= m_AttackRange)//If the object is within the attack range and within the damage angle.
                        {
                            hit.transform.gameObject.SendMessage("ReceiveDamage", attackDamage);//send the damage to the hit object. The hit object needs to have a script with the method ReceiveDamage(float damage);
                        }
                    }
                }
            }

            isAttacking = false;
        }
        public void FinishAttack()
        {
            if (!autoAttack)
            {

                //fsm.GetCurrentState.SendEvent("ToIdle");
                fsm.ChangeToState(idleState);
            }
        }

        public void AttackRange()
        {
            //
            //Method Name : void AttackRange()
            //Purpose     : This method is called by the attack animation event. Deals the required damage to all targets in range..
            //Re-use      : none
            //Input       : none
            //Output      : none
            //
            GameObject projectile = Instantiate(attackProjectile, spawnPosition.position, Quaternion.identity);

            //arrow.transform.position = spawnPosition.position;
            SimpleAIController ai = projectile.GetComponent<SimpleAIController>();
            ai.target = target;
            ai.Fire(gameObject);
        }
    }
    public class CharacterStates
    {
        //The different states that the character can be in. 
        //Note: A character can only be in one state at a time.
        public const string STATE_IDLE = "IdleState";
        public const string STATE_DAME = "DameState";
        public const string STATE_ATTACK = "AttackState";
        public const string STATE_FOLLOW = "FollowState";
        public const string STATE_PATROL = "PatrolState";
        public const string STATE_GUARD = "GuardState";
        public const string STATE_DEAD = "DeadState";
    }

    public class MoveType
    {
        public const int MOVE_SIMPLE = 0;
        public const int MOVE_OA = 1;
        public const int MOVE_PATHFIND = 2;
    }

}
