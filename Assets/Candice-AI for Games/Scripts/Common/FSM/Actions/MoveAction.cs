using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViridaxGameStudios.AI;
namespace ViridaxGameStudios
{
    public class MoveAction : FSMAction
    {
        private Transform transform;
        private Animator animator;
        private string finishEvent;
        //private float size;
        Vector3[] path;
        Vector3 currentWaypoint;
        int targetIndex;
        bool canMove = false;
        public MoveAction(FSMState owner, Character aiController) :base(owner, aiController)
        {

        }

        public void Init(Transform transform, Animator animator, string finishEvent = null)
        {
            this.transform = transform;
            this.animator = animator;
            this.finishEvent = finishEvent;
            //size = transform.localScale.x;
        }
        public override void OnEnter()
        {
            aiController.isMoving = true;
            animator.SetBool("isRunning", true);
            
            if(MoveType.MOVE_PATHFIND == aiController.moveType)
            {
                switch (aiController.pathfindSource)
                {
                    case Character.PATHFIND_SOURCE_NAVMESH:
                        Debug.Log("Starting NavMesh");
                        aiController.StartMoveNavMesh(aiController.target);
                        break;
                    case Character.PATHFIND_SOURCE_CANDICE:
                        //aiController.StartFinding();

                        break;
                }
            }
            

        }

        public override void OnExit()
        {
            //base.OnExit();
            aiController.isMoving = false;
            animator.SetBool("isRunning", false);
            
            if (MoveType.MOVE_PATHFIND == aiController.moveType)
            {
                switch (aiController.pathfindSource)
                {
                    case Character.PATHFIND_SOURCE_NAVMESH:
                        aiController.StopMoveNavMesh();
                        break;
                    case Character.PATHFIND_SOURCE_CANDICE:
                        //aiController.StopFinding();

                        break;
                }
            }
            

        }

        public override void OnUpdate()
        {
            
            

            if (aiController.target != null)
            {
                //Move(AIController.target.transform);
                switch (aiController.moveType)
                {
                    case MoveType.MOVE_SIMPLE:
                        FollowTarget(aiController.target.transform);
                        break;
                    case MoveType.MOVE_OA:
                        RaycastHit hit;
                        float distance = Vector3.Distance(transform.position, aiController.target.transform.position);
                        if (Physics.Raycast(transform.position, aiController.target.transform.position, out hit))
                        {
                            if (hit.transform != aiController.target.transform)
                            {
                                CandiceAIManager.ObstacleAvoidance(aiController.target.transform, transform, transform.localScale.x, aiController.movementSpeed, aiController.is3D, 10);
                            }
                            else
                            {
                                FollowTarget(aiController.target.transform);
                            }
                        }
                        else
                        {
                            CandiceAIManager.ObstacleAvoidance(aiController.target.transform, transform, transform.localScale.x, aiController.movementSpeed, aiController.is3D, 10);
                        }


                        break;
                }
            }
            
        }
        
        
        
        private void FollowTarget(Transform Target)
        {
            LookAt(Target.gameObject);
            //float distance = Vector3.Distance(transform.position, target.transform.position);
            transform.position += transform.forward * aiController.movementSpeed * Time.deltaTime;
            //Debug.DrawLine(transform.position, Target.position, Color.green);
        }

        protected void LookAt(GameObject Target)
        {
            Quaternion targetRotation = Quaternion.LookRotation(Target.transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 12 * Time.deltaTime);
        }
        protected void LookAt(Vector3 Target)
        {
            Quaternion targetRotation = Quaternion.LookRotation(Target - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 12 * Time.deltaTime);
        }
    }


}
