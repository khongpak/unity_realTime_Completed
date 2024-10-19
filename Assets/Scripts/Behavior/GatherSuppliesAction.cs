using GameDevTV.RTS.Environment;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

namespace GameDevTV.RTS.Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Gather Supplies", story: "[Unit] gathers [Amount] supplies from [GatherableSupplies] .", category: "Action/Units", id: "3b941d7ae99d1e36b7d806875379c977")]
    public partial class GatherSuppliesAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Unit;
        [SerializeReference] public BlackboardVariable<int> Amount;
        [SerializeReference] public BlackboardVariable<GatherableSupply> GatherableSupplies;

        private float enterTime;

        protected override Status OnStart()
        {
            enterTime = Time.time;

            GatherableSupplies.Value.BeginGather();
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (GatherableSupplies.Value.Supply.BaseGatherTime + enterTime <= Time.time)
            {
                Amount.Value = GatherableSupplies.Value.EndGather();
                return Status.Success;
            }

            return Status.Running;
        }
    }
}
