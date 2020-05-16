using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViridaxGameStudios.AI;

[System.Serializable]
public abstract class BehaviorNode
{
    public Transform transform;
    public AIController aiController;
    public int patrolCount = 0;
    public int id;
    /* Delegate that returns the state of the node.*/
    public delegate BehaviorStates NodeReturn();

    /* The current state of the node */
    protected BehaviorStates m_nodeState;

    public BehaviorStates nodeState
    {
        get { return m_nodeState; }
    }

    /* The constructor for the node */
    public BehaviorNode() { }
    public void Initialise(Transform transform, AIController aiController) {
        this.transform = transform;
        this.aiController = aiController;
    }

    /* Implementing classes use this method to evaluate the desired set of conditions */
    public abstract BehaviorStates Evaluate();

}