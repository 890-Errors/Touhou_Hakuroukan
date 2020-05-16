using System;
using UnityEngine;
namespace ViridaxGameStudios.AI
{
    public class DefaultBehaviors
    {
        public static BehaviorStates Idle(BehaviorNode rootNode)
        {
            BehaviorStates state = BehaviorStates.SUCCESS;
            rootNode.aiController.Animator.SetTrigger("idle");
            return state;
        }
        public static BehaviorStates InitVariables(BehaviorNode rootNode)
        {
            BehaviorStates state = BehaviorStates.SUCCESS;
            rootNode.aiController.isMoving = false;
            return state;
        }
        public static BehaviorStates LookAt(BehaviorNode rootNode)
        {
            BehaviorStates state = BehaviorStates.SUCCESS;

            try
            {
                if (rootNode.aiController.moveType == MoveType.MOVE_SIMPLE && rootNode.aiController.is3D)
                {
                    rootNode.aiController.transform.LookAt(rootNode.aiController.target.transform);
                }
            }
            catch (Exception e)
            {
                state = BehaviorStates.FAILURE;
            }
            return state;
        }
        public static BehaviorStates MoveTo(BehaviorNode rootNode)
        {
            rootNode.aiController.isMoving = true;
            rootNode.aiController.Animator.SetTrigger("run");
            //rootNode.aiController.Animator.SetFloat("characterSpeed", rootNode.aiController.movementSpeed);
            BehaviorStates state = BehaviorStates.FAILURE;
            try
            {
                //LookAt(rootNode.aiController.target.transform, rootNode.aiController.transform);
                if(rootNode.aiController.moveType == MoveType.MOVE_SIMPLE)
                {
                    CandiceAIManager.SimpleMove(rootNode.transform, rootNode.aiController.target.transform, rootNode.aiController.movementSpeed, rootNode.aiController.is3D);
                }
                else if(rootNode.aiController.moveType == MoveType.MOVE_OA)
                {
                    CandiceAIManager.ObstacleAvoidance(rootNode.aiController.target.transform, rootNode.transform, rootNode.transform.localScale.x, rootNode.aiController.movementSpeed, rootNode.aiController.is3D, rootNode.aiController.oaDistance);
                }
                else if(rootNode.aiController.moveType == MoveType.MOVE_PATHFIND)
                {
                    if(rootNode.aiController.pathfindSource == Character.PATHFIND_SOURCE_CANDICE)
                    {
                        rootNode.aiController.StartFinding();
                    }
                    else if (rootNode.aiController.pathfindSource == Character.PATHFIND_SOURCE_NAVMESH)
                    {
                        rootNode.aiController.StartMoveNavMesh(rootNode.aiController.target);
                    }
                }
                state = BehaviorStates.SUCCESS;
                if (CandiceConfig.enableDebug) ;
                    //Debug.DrawLine(rootNode.aiController.transform.position, rootNode.aiController.target.transform.position, Color.green);
            }
            catch(Exception e)
            {
                state = BehaviorStates.FAILURE;
                if (CandiceConfig.enableDebug)
                    Debug.LogError("Error: " + e.Message);
            }
            
            return state;
        }

        public static BehaviorStates IsDead(BehaviorNode rootNode)
        {
            BehaviorStates state = BehaviorStates.FAILURE;
            try
            {
                if (rootNode.aiController.isDead)
                    state = BehaviorStates.SUCCESS;
            }
            catch (Exception e)
            {
                state = BehaviorStates.FAILURE;
                if (CandiceConfig.enableDebug)
                    Debug.LogError("Error: " + e.Message);
            }
            return state;
        }

        public static BehaviorStates Die(BehaviorNode rootNode)
        {
            BehaviorStates state = BehaviorStates.FAILURE;
            try
            {
                rootNode.aiController.stopBehaviorTree = true;
                if (rootNode.aiController.enableRagdollOnDeath)
                    rootNode.aiController.EnableRagdoll();
                else
                    rootNode.aiController.Animator.SetTrigger("die");
                state = BehaviorStates.SUCCESS;
            }
            catch (Exception e)
            {
                state = BehaviorStates.FAILURE;
                if (CandiceConfig.enableDebug)
                    Debug.LogError("Error: " + e.Message);
            }
            return state;
        }

        
        public static BehaviorStates Attack(BehaviorNode rootNode)
        {
            BehaviorStates state = BehaviorStates.SUCCESS;
            try
            {
                if (rootNode.aiController.hasAttackAnimation)
                {
                    rootNode.aiController.Animator.SetTrigger("attack");
                }
                    
                else
                {
                    if (rootNode.aiController.attackType == Character.ATTACK_TYPE_MELEE)
                        rootNode.aiController.Attack();
                    else
                        rootNode.aiController.AttackRange();
                }
            }
            catch(Exception e)
            {
                
                state = BehaviorStates.FAILURE;
            }
            
            
            //rootNode.aiController.Animator.SetBool();
            return state;
        }

        public static BehaviorStates Patrol(BehaviorNode rootNode)
        {
            BehaviorStates state = BehaviorStates.FAILURE;
            try
            {
                if (rootNode.aiController.pointReached)
                {
                    if (rootNode.aiController.patrolInOrder)
                    {
                        if (rootNode.patrolCount < rootNode.aiController.patrolPoints.Count - 1)
                        {
                            rootNode.patrolCount++;
                        }
                        else
                        {
                            rootNode.patrolCount = 0;
                        }
                    }
                    else
                    {
                        UnityEngine.Random rnd = new UnityEngine.Random();
                        rootNode.patrolCount = UnityEngine.Random.Range(0, rootNode.aiController.patrolPoints.Count);
                    }
                    rootNode.aiController.target = rootNode.aiController.patrolPoints[rootNode.patrolCount];
                    rootNode.aiController.pointReached = false;
                }
                else
                {
                    rootNode.aiController.target = rootNode.aiController.patrolPoints[rootNode.patrolCount];
                    //LookAt(patrolTarget);
                    //transform.position += transform.forward * MovementSpeed * Time.deltaTime;
                }
                state = BehaviorStates.SUCCESS;
            }
            catch (Exception e)
            {
                state = BehaviorStates.FAILURE;
                if (CandiceConfig.enableDebug)
                    Debug.LogError("Error: " + e.Message);
            }
            return state;
        }

        public static BehaviorStates EnemyDetected(BehaviorNode rootNode)
        {
            if (rootNode.aiController.enemyFound)
                return BehaviorStates.SUCCESS;
            else
                return BehaviorStates.FAILURE;
        }
        public static BehaviorStates WithinAttackRange(BehaviorNode rootNode)
        {
            float distance = float.MaxValue;
            try
            {
                distance = Vector3.Distance(rootNode.aiController.transform.position, rootNode.aiController.target.transform.position);
            }
            catch (Exception e)
            {

            }
            if (distance <= rootNode.aiController.m_AttackRange)
                return BehaviorStates.SUCCESS;
            else
                return BehaviorStates.FAILURE;
        }
        public static BehaviorStates IsPatrolling(BehaviorNode rootNode)
        {
            if (rootNode.aiController.isPatrolling)
                return BehaviorStates.SUCCESS;
            else
                return BehaviorStates.FAILURE;
        }
        public static BehaviorStates IsPlayerValid(BehaviorNode rootNode)
        {
            GameObject [] arrPlayer = GameObject.FindGameObjectsWithTag("Player");

            if (arrPlayer.Length > 0)
            {
                rootNode.aiController.target = arrPlayer[0];
                return BehaviorStates.SUCCESS;
            }
                
            else
                return BehaviorStates.FAILURE;
        }
    }

}
