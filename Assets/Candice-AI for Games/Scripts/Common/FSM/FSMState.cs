using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ViridaxGameStudios.AI
{
    public class FSMState
    {
        private List<FSMAction> actions;
        private string name;
        private Dictionary<string, FSMState> transitionMap;
        private FSM owner;

        public string Name { get { return name; } }

        //Initialises a new instance of the state
        public FSMState(string name, FSM owner)
        {
            this.name = name;
            this.owner = owner;
            this.transitionMap = new Dictionary<string, FSMState>();
            this.actions = new List<FSMAction>();
        }

        //Adds the transition
        public void AddTransition(string id, FSMState destinationState)
        {
            if(transitionMap.ContainsKey(id))
            {
                Debug.LogError(string.Format("state {0} already contains transitions for {1}",this.name,id));
                return;
            }
            transitionMap[id] = destinationState;
        }
        //Gets the transition
        public FSMState GetTransition(string eventId)
        {
            if(transitionMap.ContainsKey(eventId))
            {
                return transitionMap[eventId];
            }
            return null;
        }
        //Adds the action
        public void AddAction(FSMAction action)
        {
            if(actions.Contains(action))
            {
                Debug.LogWarning("this state already contains " + action);
            }
            actions.Add(action);
        }
        //this gets the actions of this state
        public IEnumerable<FSMAction>GetActions()
        {
            return actions;
        }
        //send the event
        public void SendEvent(string eventId)
        {
            this.owner.SendEvent(eventId);
        }

    }
}

