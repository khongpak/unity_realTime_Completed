using GameDevTV.RTS.Environment;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Linq;
using GameDevTV.RTS.Utilities;

namespace GameDevTV.RTS.Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Move to GatherableSupply", story: "[Agent] moves to [Supply] or nearby not busy supply.", category: "Action/Navigation", id: "b9248f874f11b1a358e671809522dbfc")]
    public partial class MoveToGatherableSupplyAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<GatherableSupply> Supply;
        [SerializeReference] public BlackboardVariable<float> SearchRadius = new(7f);

        private NavMeshAgent agent;
        private LayerMask suppliesMask;

        protected override Status OnStart()
        {
            if (!Agent.Value.TryGetComponent(out agent))
            {
                return Status.Failure;
            }

            suppliesMask = LayerMask.GetMask("Supplies");

            Vector3 targetPosition = GetTargetPosition();

            agent.SetDestination(targetPosition);
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (agent.remainingDistance >= agent.stoppingDistance)
            {
                return Status.Running;
            }

            if (!Supply.Value.IsBusy && Supply.Value.Amount > 0)
            {
                return Status.Success;
            }

            Collider[] colliders = Physics.OverlapSphere(
                agent.transform.position,
                SearchRadius,
                suppliesMask
            ).Where(collider => 
                    collider.TryGetComponent(out GatherableSupply supply)
                    && !supply.IsBusy
                    && supply.Supply.Equals(Supply.Value.Supply)
            ).ToArray();

            if (colliders.Length > 0)
            {
                Array.Sort(colliders, new ClosestColliderComparer(agent.transform.position));

                Supply.Value = colliders[0].GetComponent<GatherableSupply>();
                agent.SetDestination(GetTargetPosition());
                return Status.Running;
            }

            return Status.Failure;
        }

        private Vector3 GetTargetPosition()
        {
            Vector3 targetPosition;
            if (Supply.Value.TryGetComponent(out Collider collider))
            {
                targetPosition = collider.ClosestPoint(agent.transform.position);
            }
            else
            {
                targetPosition = Supply.Value.transform.position;
            }

            return targetPosition;
        }
    }
}