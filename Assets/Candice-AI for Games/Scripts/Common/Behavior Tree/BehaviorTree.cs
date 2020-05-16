using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
namespace ViridaxGameStudios.AI
{
    [CreateAssetMenu(fileName = "New Behavior Tree", menuName = "Behavior Tree")]
    public class BehaviorTree : ScriptableObject
    {
        public BehaviorNode rootNode;
        public CandiceBehaviorTree behaviorTree;
        List<MethodInfo> lstFunctions;

        public void Initialise()
        {
            List<Type> behaviorTypes = new List<Type>();
            lstFunctions = new List<MethodInfo>();
            MethodInfo[] arrMethodInfos;

            behaviorTypes.Add(typeof(DefaultBehaviors));
            foreach (Type type in behaviorTypes)
            {
                arrMethodInfos = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
                lstFunctions.AddRange(arrMethodInfos);
            }
        }


        public BehaviorNode CreateBehaviorTree(AIController agent)
        {
            Initialise();
            rootNode = null;
            CandiceBehaviorNode _rootNode = null;
            List<CandiceBehaviorNode> nodes = behaviorTree.GetNodes();
            foreach (CandiceBehaviorNode item in nodes)
            {
                if (item.isRoot)
                {
                    _rootNode = item;

                }
            }

            switch (_rootNode.type)
            {
                case CandiceAIManager.NODE_TYPE_SELECTOR:
                    rootNode = new Selector();
                    rootNode.id = _rootNode.id;
                    rootNode.Initialise(agent.transform, agent);
                    (rootNode as Selector).SetNodes(GetChildren(behaviorTree, _rootNode));
                    break;
                case CandiceAIManager.NODE_TYPE_SEQUENCE:
                    rootNode = new Sequence();
                    rootNode.id = _rootNode.id;
                    rootNode.Initialise(agent.transform, agent);
                    (rootNode as Sequence).SetNodes(GetChildren(behaviorTree, _rootNode));
                    break;
            }


            return rootNode;
        }
        List<BehaviorNode> GetChildren(CandiceBehaviorTree bt, CandiceBehaviorNode node)
        {
            List<BehaviorNode> children = new List<BehaviorNode>();
            CandiceBehaviorNode _node = null;
            foreach (int id in node.childrenIDs)
            {
                BehaviorNode newNode = null;
                if (GetNodeWithID(id, bt.GetNodes()) != null)
                {
                    _node = bt.GetNodes()[id];
                    switch (_node.type)
                    {
                        case CandiceAIManager.NODE_TYPE_SELECTOR:
                            newNode = new Selector();
                            (newNode as Selector).SetNodes(GetChildren(bt, _node));
                            break;
                        case CandiceAIManager.NODE_TYPE_SEQUENCE:
                            newNode = new Sequence();
                            (newNode as Sequence).SetNodes(GetChildren(bt, _node));
                            break;
                        case CandiceAIManager.NODE_TYPE_ACTION:
                            BehaviorAction action = new BehaviorAction((BehaviorAction.ActionNodeDelegate)lstFunctions[_node.function].CreateDelegate(typeof(BehaviorAction.ActionNodeDelegate)), rootNode);
                            newNode = action;
                            break;
                    }
                    children.Add(newNode);
                }
            }
            return children;
        }

        CandiceBehaviorNode GetNodeWithID(int id, List<CandiceBehaviorNode> nodes)
        {
            CandiceBehaviorNode node = null;
            bool isFound = false;
            int count = 0;
            while (!isFound && count < nodes.Count)
            {
                if (nodes[count].id == id)
                {
                    node = nodes[count];
                    isFound = true;
                }
                count++;
            }
            return node;
        }


    }
}
