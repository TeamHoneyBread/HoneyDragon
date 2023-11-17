﻿namespace RPG.Combat.AI.BehaviourTree.Node
{
    public class Inverter : DecoratorNode
    {
        protected override void OnAwake()
        {
        }

        protected override void OnEnd()
        {
        }

        protected override void OnStart()
        {
        }

        protected override NodeState OnUpdate()
        {
            var stat = Child.Evaluate();

            if (stat == NodeState.Success)
                return NodeState.Failure;

            else if(stat == NodeState.Failure) 
                return NodeState.Success;

            return stat;
        }
    }
}
