using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ViridaxGameStudios.AI
{
    [Serializable]
    public class CandiceBehaviorTree
    {
        public string name;
        private List<CandiceBehaviorNode> nodes;

        public void SetNodes(List<CandiceBehaviorNode> _nodes)
        {
            nodes = new List<CandiceBehaviorNode>();
            foreach(CandiceBehaviorNode node in _nodes)
            {
                CandiceBehaviorNode newNode = new CandiceBehaviorNode(node.id, node.type, node.childrenIDs, node.function, node.isRoot, node.x, node.y, node.width, node.height, node.number);
                nodes.Add(newNode);
            }
        }
        public List<CandiceBehaviorNode> GetNodes()
        {
            return nodes;
        }
        public void AddNode(CandiceBehaviorNode node)
        {
            nodes.Add(node);
        }
    }
}

