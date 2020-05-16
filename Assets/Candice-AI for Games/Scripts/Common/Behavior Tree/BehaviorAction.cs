using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorAction : BehaviorNode
{
    /* Method signature for the action. */
    public delegate BehaviorStates ActionNodeDelegate(BehaviorNode rootNode);
    BehaviorNode rootNode;
    /* The delegate that is called to evaluate this node */
    private ActionNodeDelegate m_action;

    /* Because this node contains no logic itself, 
     * the logic must be passed in in the form of  
     * a delegate. As the signature states, the action 
     * needs to return a NodeStates enum */
    public BehaviorAction(ActionNodeDelegate action, BehaviorNode rootNode)
    {
        this.rootNode = rootNode;
        m_action = action;
    }

    /* Evaluates the node using the passed in delegate and  
     * reports the resulting state as appropriate */
    public override BehaviorStates Evaluate()
    {
        switch (m_action(rootNode))
        {
            case BehaviorStates.SUCCESS:
                m_nodeState = BehaviorStates.SUCCESS;
                return m_nodeState;
            case BehaviorStates.FAILURE:
                m_nodeState = BehaviorStates.FAILURE;
                return m_nodeState;
            case BehaviorStates.RUNNING:
                m_nodeState = BehaviorStates.RUNNING;
                return m_nodeState;
            default:
                m_nodeState = BehaviorStates.FAILURE;
                return m_nodeState;
        }
    }
}
