using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

namespace GameDevTV.RTS.Units
{
    [RequireComponent(typeof(NavMeshAgent), typeof(BehaviorGraphAgent))]
    public abstract class AbstractUnit : AbstractCommandable, IMoveable
    {
        public float AgentRadius => agent.radius;
        [SerializeField] private DamageableSensor DamageableSensor;
        private NavMeshAgent agent;
        protected BehaviorGraphAgent graphAgent;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            graphAgent = GetComponent<BehaviorGraphAgent>();
            graphAgent.SetVariableValue("Command", UnitCommands.Stop);
        }

        protected override void Start()
        {
            base.Start();
            CurrentHealth = UnitSO.Health;
            MaxHealth = UnitSO.Health;
            Bus<UnitSpawnEvent>.Raise(new UnitSpawnEvent(this));

            if (DamageableSensor != null)
            {
                DamageableSensor.OnUnitEnter += HandleUnitEnter;
                DamageableSensor.OnUnitExit += HandleUnitExit;
            }
        }

        public void MoveTo(Vector3 position)
        {
            graphAgent.SetVariableValue("TargetLocation", position);
            graphAgent.SetVariableValue("Command", UnitCommands.Move);
        }

        public void Stop()
        {
            SetCommandOverrides(null);
            graphAgent.SetVariableValue("Command", UnitCommands.Stop);
        }

        private void HandleUnitEnter(IDamageable damageable)
        {
            Debug.Log($"Detected unit enter! {DamageableSensor.Damageables.Count} nearby damageables!");
        }

        private void HandleUnitExit(IDamageable damageable)
        {
            Debug.Log($"Detected unit exit! {DamageableSensor.Damageables.Count} nearby damageables!");
        }

        private void OnDestroy()
        {
            Bus<UnitDeathEvent>.Raise(new UnitDeathEvent(this));
        }
    }
}
