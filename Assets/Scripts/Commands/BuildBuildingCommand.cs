using System.Linq;
using GameDevTV.RTS.Units;
using UnityEngine;

namespace GameDevTV.RTS.Commands
{
    [CreateAssetMenu(fileName = "Build Building", menuName = "Units/Commands/Build Building")]
    public class BuildBuildingCommand : ActionBase
    {
        [field: SerializeField] public BuildingSO Building { get; private set; }
        [field: SerializeField] public BuildingRestrictionSO[] Restrictions { get; private set; }

        public override bool CanHandle(CommandContext context)
        {
            if (context.Commandable is not IBuildingBuilder) return false;

            if (context.Hit.collider != null)
            {
                return context.Hit.collider.TryGetComponent(out BaseBuilding building)
                    && Building == building.BuildingSO
                    && (building.Progress.State == BuildingProgress.BuildingState.Paused
                        || building.Progress.State == BuildingProgress.BuildingState.Destroyed
                    );
            }

            return AllRestrictionsPass(context.Hit.point);
        }

        public override void Handle(CommandContext context)
        {
            IBuildingBuilder builder = (IBuildingBuilder)context.Commandable;

            if (context.Hit.collider != null && context.Hit.collider.TryGetComponent(out BaseBuilding building))
            {
                builder.ResumeBuilding(building);
            }
            else if (AllRestrictionsPass(context.Hit.point))
            {
                builder.Build(Building, context.Hit.point);
            }
        }

        private bool AllRestrictionsPass(Vector3 point) => 
            Restrictions.Length == 0 || Restrictions.All(restriction => restriction.CanPlace(point));
    }
}
