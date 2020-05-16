using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ViridaxGameStudios.AI
{
    public class IdleAction : FSMAction
    {
        private Animator animator;
        private string finishEvent;
        public IdleAction(FSMState owner, Character aiController) : base(owner, aiController)
        {

        }

        public void Init(Animator animator, string finishEvent = null)
        {
            this.animator = animator;
            this.finishEvent = finishEvent;
        }

        public override void OnEnter()
        {
            //base.OnEnter();
            //animator.SetBool("isIdle", true);
        }

        public override void OnExit()
        {
            //base.OnExit();
            //animator.SetBool("isIdle", false);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }
    }
}

