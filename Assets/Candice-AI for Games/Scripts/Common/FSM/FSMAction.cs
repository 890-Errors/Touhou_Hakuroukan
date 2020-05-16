using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ViridaxGameStudios.AI
{
    public class FSMAction
    {
        private readonly FSMState owner;
        public Character aiController;
        public FSMAction(FSMState owner, Character aiController)
        {
            this.owner = owner;
            this.aiController = aiController;
        }
        public FSMState GetOwner()
        {
            return owner;
        }
        //Starts the action
        public virtual void OnEnter()
        {

        }
        //Updates the action
        public virtual void OnUpdate()
        {

        }
        //Finishes the action
        public virtual void OnExit()
        {

        }
    }
}

