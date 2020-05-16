using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class Sequence : BehaviorNode
{
    /** Children nodes that belong to this sequence */
    private List<BehaviorNode> m_nodes = new List<BehaviorNode>();

    /** Must provide an initial set of children nodes to work */
    public Sequence()
    {
        
    }
    public void SetNodes(List<BehaviorNode> nodes)
    {
        m_nodes = nodes;
    }
    public void AddNode(BehaviorNode node)
    {
        m_nodes.Add(node);
    }
    /* If any child node returns a failure, the entire node fails. Whence all  
     * nodes return a success, the node reports a success. */
    public override BehaviorStates Evaluate()
    {
        bool anyChildRunning = false;

        foreach (BehaviorNode node in m_nodes)
        {
            switch (node.Evaluate())
            {
                case BehaviorStates.FAILURE:
                    m_nodeState = BehaviorStates.FAILURE;
                    return m_nodeState;
                case BehaviorStates.SUCCESS:
                    continue;
                case BehaviorStates.RUNNING:
                    anyChildRunning = true;
                    continue;
                default:
                    m_nodeState = BehaviorStates.SUCCESS;
                    return m_nodeState;
            }
        }
        m_nodeState = anyChildRunning ? BehaviorStates.RUNNING : BehaviorStates.SUCCESS;
        return m_nodeState;
    }
}
