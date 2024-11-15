using GameDevTV.RTS.Units;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

namespace GameDevTV.RTS.Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Build Building", story: "[Self] builds [BuildingSO] at [TargetLocation] .", category: "Action/Units", id: "32f744f391217b3bc542115e266d16c0")]
    public partial class BuildBuildingAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Self;
        [SerializeReference] public BlackboardVariable<BuildingSO> BuildingSO;
        [SerializeReference] public BlackboardVariable<Vector3> TargetLocation;
        [SerializeReference] public BlackboardVariable<BaseBuilding> BuildingUnderConstruction;

        private float startBuildTime;
        private BaseBuilding completedBuilding;
        private Vector3 startPosition;
        private Vector3 endPosition;

        protected override Status OnStart()
        {
            if (!HasValidInputs()) return Status.Failure;

            startBuildTime = Time.time;
            GameObject building = GameObject.Instantiate(BuildingSO.Value.Prefab);

            if (!building.TryGetComponent(out completedBuilding) 
                || completedBuilding.MainRenderer == null) return Status.Failure;

            Renderer buildingRenderer = completedBuilding.MainRenderer;

            BuildingUnderConstruction.Value = completedBuilding;

            startPosition = TargetLocation.Value - Vector3.up * buildingRenderer.bounds.size.y;
            endPosition = TargetLocation.Value;
            completedBuilding.transform.position = startPosition;
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            float normalizedTime = (Time.time - startBuildTime) / BuildingSO.Value.BuildTime;

            completedBuilding.transform.position = Vector3.Lerp(startPosition, endPosition, normalizedTime);

            return normalizedTime >= 1 ? Status.Success : Status.Running;
        }

        protected override void OnEnd()
        {
            if (CurrentStatus == Status.Success)
            {
                completedBuilding.enabled = true;
            }
        }

        private bool HasValidInputs()
        {
            return Self.Value != null
                && BuildingSO.Value != null
                && BuildingSO.Value.Prefab != null;
        }
    }
}