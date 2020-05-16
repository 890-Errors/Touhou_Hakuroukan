using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ViridaxGameStudios.AI
{
    public class AttackAction : FSMAction
    {
        Animator animator;
        string finishEvent;
        public AttackAction(FSMState owner, Character aiController) : base(owner, aiController)
        {

        }
        public void Init(Animator animator, string finishEvent = null)
        {
            this.animator = animator;
            this.finishEvent = finishEvent;
        }
        public override void OnEnter()
        {
            animator.SetBool("isAttacking", true);
        }

        public override void OnExit()
        {
            animator.SetBool("isAttacking", false);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }

        
    }
}

