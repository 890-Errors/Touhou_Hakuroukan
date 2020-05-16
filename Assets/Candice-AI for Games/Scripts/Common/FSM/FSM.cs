using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ViridaxGameStudios.AI
{
    //This is the main class for the FInite State Machine Engine that controls all states and actions.
    public class FSM
    {
        private readonly string name;
        private FSMState currentState;
        private readonly Dictionary<string, FSMState> stateMap;
        public string Name
        {
            get{return name;}
        }

//The constructor initialises the class and gives the the FSM a unique name or id.
        public FSM(string name)
        {
            this.name = name;
            this.currentState = null;
            stateMap = new Dictionary<string, FSMState>();
        }
        public FSMState GetCurrentState
        {
            get{ return currentState; }
        }
        //This initialises the FSM. We can set a starting state here.
        public void Start(string stateName)
        {
            if(!stateMap.ContainsKey(stateName))
            {
                Debug.LogWarning("The FSM doesn't contain: " + stateName);
                return;
            }
            ChangeToState(stateMap[stateName]);

        }

//This changes the state of the object. This also calls the exit state before doing the next state.
        public void ChangeToState(FSMState state)
        {
            if(this.currentState != null)
            {
                ExitState(this.currentState);
            }
            this.currentState = state;
            EnterState(this.currentState);
        }
//This changes the state of the object. It is not advisable to call this to change state.
        private void EnterState(FSMState state)
        {
            ProcessStateAction (state, delegate(FSMAction action)
            {
                action.OnEnter();
            });
        }
        private void ExitState(FSMState state)
        {
            FSMState currentStateOnInvoke = this.currentState;
            ProcessStateAction (state, delegate(FSMAction action)
            {
                if(this.currentState != currentStateOnInvoke)
                {
                    Debug.LogError("State cannot be changed on exit of the specified state");
                }
                action.OnExit();
            });

        }
//Call this under a MonoBehaviour's Update
        public void Update()
        {
            if(this.currentState == null)
            {
                return;
            }
            ProcessStateAction(this.currentState, delegate(FSMAction action)
            {
                action.OnUpdate();
            });
        }
        public FSMState AddState(string name)
        {
            if(stateMap.ContainsKey(name))
            {
                Debug.LogWarning("The FSM already contains " + name);
                return null;
            }
            FSMState newState = new FSMState(name, this);
            stateMap[name] = newState;
            return newState;
        }
//This handles the events that is bound to a state and changes the state.
        public void SendEvent(string eventId)
        {
            FSMState transitionState = ResolveTransition(eventId);
            if(transitionState == null)
            {
                Debug.LogWarning ("The current state has no transition for event " + eventId);
            }
            else
            {
                ChangeToState(transitionState);

            }
        }
        //This gets the next state from the current state.
        private FSMState ResolveTransition(string eventId)
        {
            FSMState transitionState = this.currentState.GetTransition (eventId);
            if(transitionState == null)
            {
                return null;
            }
            else
            {
                return transitionState;
            }
        }
        private delegate void StateActionProcessor (FSMAction action);
        //this gets all the actions inside the state and loops them.
        private void ProcessStateAction(FSMState state, StateActionProcessor actionProcessor)
        {
            FSMState currentStateOnInvoke = this.currentState;
            IEnumerable<FSMAction> actions = state.GetActions();

            foreach(FSMAction action in actions)
            {
                if(this.currentState != currentStateOnInvoke)
                {
                    break;
                }
                actionProcessor(action);
            }
        }

    }

    

}

