﻿using RPG.AI.BehaviourTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RPG.Combat.AI.BehaviourTree.Node
{
    [CreateAssetMenu(menuName ="BehaviourTree/Node/Decorator/Repeater")]
    public class RepeaterNode : DecoratorNode
    {
        public RepeaterNode(NodeBase repeatedNode) 
        {
            Child = repeatedNode;
        }

        protected override void OnEnd()
        {
        }

        protected override void OnStart()
        {
        }

        protected override NodeState OnUpdate()
        {
            Child.Evaluate();
            return NodeState.Running;
        }
    }
}
