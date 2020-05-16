using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class Selector : BehaviorNode
{
    /** The child nodes for this selector */
    protected List<BehaviorNode> m_nodes = new List<BehaviorNode>();


    /** The constructor requires a lsit of child nodes to be  
     * passed in*/
    public Selector()
    {

    }
    public void SetNodes(List<BehaviorNode> nodes)
    {
        m_nodes = nodes;
    }
    public List<BehaviorNode> GetNodes()
    {
        return m_nodes;
    }
    public void AddNode(BehaviorNode node)
    {
        m_nodes.Add(node);
    }
    /* If any of the children reports a success, the selector will 
     * immediately report a success upwards. If all children fail, 
     * it will report a failure instead.*/
    public override BehaviorStates Evaluate()
    {
        foreach (BehaviorNode node in m_nodes)
        {
            switch (node.Evaluate())
            {
                case BehaviorStates.FAILURE:
                    continue;
                case BehaviorStates.SUCCESS:
                    m_nodeState = BehaviorStates.SUCCESS;
                    return m_nodeState;
                case BehaviorStates.RUNNING:
                    m_nodeState = BehaviorStates.RUNNING;
                    return m_nodeState;
                default:
                    continue;
            }
        }
        m_nodeState = BehaviorStates.FAILURE;
        return m_nodeState;
    }
}