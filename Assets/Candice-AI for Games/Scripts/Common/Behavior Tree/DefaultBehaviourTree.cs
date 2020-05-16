using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ViridaxGameStudios.AI
{
    public class DefaultBehaviourTree : MonoBehaviour
    {
        AIController aiController;
        private Selector rootNode;
        private Sequence canSeeEnemySequence;
        private Sequence patrolSequence;
        private Sequence isDeadSequence;

        private BehaviorAction initNode;
        private BehaviorAction canSeeEnemyNode;
        private Selector attackOrChaseSelector;
        private Sequence attackSequence;
        private BehaviorAction withinAttackRange;
        private BehaviorAction attackNode;
        private BehaviorAction idleNode;
        private BehaviorAction isDeadNode;
        private BehaviorAction dieNode;

        private BehaviorAction moveNode;
        private BehaviorAction lookAtNode;
        private BehaviorAction isPatrollingNode;
        private BehaviorAction patrolNode;
        DefaultBehaviors paladinBehaviours;
        

        public void Initialise(AIController aiController)
        {
            this.aiController = aiController;
        }
        // Start is called before the first frame update
        void Start()
        {
            aiController = GetComponent<AIController>();
            rootNode = new Selector();
            rootNode.Initialise(transform, aiController);

            initNode = new BehaviorAction(DefaultBehaviors.InitVariables, rootNode);
            moveNode = new BehaviorAction(DefaultBehaviors.MoveTo, rootNode);
            lookAtNode = new BehaviorAction(DefaultBehaviors.LookAt, rootNode);
            canSeeEnemyNode = new BehaviorAction(DefaultBehaviors.EnemyDetected, rootNode);
            withinAttackRange = new BehaviorAction(DefaultBehaviors.WithinAttackRange, rootNode);
            attackNode = new BehaviorAction(DefaultBehaviors.Attack, rootNode);
            attackSequence = new Sequence();
            attackSequence.SetNodes(new List<BehaviorNode> {withinAttackRange, lookAtNode, attackNode });
            isDeadNode = new BehaviorAction(DefaultBehaviors.IsDead, rootNode);
            dieNode = new BehaviorAction(DefaultBehaviors.Die, rootNode);

            attackOrChaseSelector = new Selector();
            attackOrChaseSelector.SetNodes( new List<BehaviorNode> { attackSequence, new Inverter(lookAtNode), moveNode });
            canSeeEnemySequence = new Sequence();
            canSeeEnemySequence.SetNodes(new List<BehaviorNode> { canSeeEnemyNode, attackOrChaseSelector });
            isDeadSequence = new Sequence();
            isDeadSequence.SetNodes(new List<BehaviorNode> { isDeadNode, dieNode });

            isPatrollingNode = new BehaviorAction(DefaultBehaviors.IsPatrolling, rootNode);
            patrolNode = new BehaviorAction(DefaultBehaviors.Patrol, rootNode);
            patrolSequence = new Sequence();
            patrolSequence.SetNodes(new List<BehaviorNode> { isPatrollingNode, patrolNode, lookAtNode, moveNode });

            idleNode = new BehaviorAction(DefaultBehaviors.Idle, rootNode);

            rootNode.SetNodes(new List<BehaviorNode>{
            new Inverter(initNode), isDeadSequence, canSeeEnemySequence, patrolSequence, idleNode
            });
            
        }

        // Update is called once per frame
        void Update()
        {
            if(!aiController.stopBehaviorTree)
                Evaluate();
        }

        public void Evaluate()
        {
            rootNode.Evaluate();

        }

        
    }

}
