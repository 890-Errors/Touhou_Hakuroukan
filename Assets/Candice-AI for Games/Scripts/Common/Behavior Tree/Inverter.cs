using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class Inverter : BehaviorNode
{
    /* Child node to evaluate */
    private BehaviorNode m_node;

    public BehaviorNode node
    {
        get { return m_node; }
    }

    /* The constructor requires the child node that this inverter decorator 
     * wraps*/
    public Inverter(BehaviorNode node)
    {
        m_node = node;
    }

    /* Reports a success if the child fails and 
     * a failure if the child succeeds. Running will report 
     * as running */
    public override BehaviorStates Evaluate()
    {
        switch (m_node.Evaluate())
        {
            case BehaviorStates.FAILURE:
                m_nodeState = BehaviorStates.SUCCESS;
                return m_nodeState;
            case BehaviorStates.SUCCESS:
                m_nodeState = BehaviorStates.FAILURE;
                return m_nodeState;
            case BehaviorStates.RUNNING:
                m_nodeState = BehaviorStates.RUNNING;
                return m_nodeState;
        }
        m_nodeState = BehaviorStates.SUCCESS;
        return m_nodeState;
    }
}
