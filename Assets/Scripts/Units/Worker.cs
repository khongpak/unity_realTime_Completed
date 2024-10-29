using GameDevTV.RTS.Behavior;
using GameDevTV.RTS.Environment;
using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;
using Unity.Behavior;
using UnityEngine;

namespace GameDevTV.RTS.Units
{
    public class Worker : AbstractUnit
    {
        public bool HasSupplies 
        {
            get 
            {
                if (graphAgent != null && graphAgent.GetVariable("SupplyAmountHeld", out BlackboardVariable<int> heldVariable))
                {
                    return heldVariable.Value > 0;
                }

                return false;
            }
        }

        protected override void Start()
        {
            base.Start();
            if (graphAgent.GetVariable("GatherSuppliesEvent", out BlackboardVariable<GatherSuppliesEventChannel> eventChannelVariable))
            {
                eventChannelVariable.Value.Event += HandleGatherSupplies;
            }
        }

        public void Gather(GatherableSupply supply)
        {
            graphAgent.SetVariableValue("Supply", supply);
            graphAgent.SetVariableValue("TargetGameObject", supply.gameObject);
            graphAgent.SetVariableValue("Command", UnitCommands.Gather);
        }

        public void ReturnSupplies(GameObject commandPost)
        {
            graphAgent.SetVariableValue("CommandPost", commandPost);
            graphAgent.SetVariableValue("Command", UnitCommands.ReturnSupplies);
        }

        private void HandleGatherSupplies(GameObject self, int amount, SupplySO supply)
        {
            Bus<SupplyEvent>.Raise(new SupplyEvent(amount, supply));
        }
    }
}
