using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViridaxGameStudios.AI;

namespace ViridaxGameStudios
{
    public class PatrolAction : FSMAction
    {
        Transform transform;
        Animator animator;
        string finishEvent;
        new Character aiController;
        int patrolCount = 0;
        public PatrolAction(FSMState owner, Character aiController) : base(owner, aiController)
        {
            this.aiController = aiController;
        }
        public void Init(Transform transform, Animator animator, string finishEvent = null)
        {
            this.transform = transform;
            this.animator = animator;
            this.finishEvent = finishEvent;

        }
        public override void OnEnter()
        {
            base.OnEnter();
            aiController.target = aiController.patrolPoints[patrolCount];

        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            Patrol();
        }

        private void Patrol()
        {
            //
            //Method Name : void Patrol()
            //Purpose     : This method allows the character to progress towards the patrol points.
            //Re-use      : none
            //Input       : none
            //Output      : none
            //
            if (aiController.pointReached)
            {
                if (aiController.patrolInOrder)
                {
                    if (patrolCount < aiController.patrolPoints.Count - 1)
                    {
                        patrolCount++;
                    }
                    else
                    {
                        patrolCount = 0;
                    }
                }
                else
                {
                    Random rnd = new Random();
                    patrolCount = Random.Range(0, aiController.patrolPoints.Count);
                }
                aiController.target = aiController.patrolPoints[patrolCount];
                aiController.pointReached = false;
            }
            else
            {
                aiController.target = aiController.patrolPoints[patrolCount];
                //LookAt(patrolTarget);
                //transform.position += transform.forward * MovementSpeed * Time.deltaTime;
            }
        }
    }
}

