using UnityEngine;
using GameDevTV.RTS.Units;
using GameDevTV.RTS.Player;
using GameDevTV.RTS.TechTree;

namespace GameDevTV.RTS.Commands
{
    [CreateAssetMenu(fileName = "Build Unit", menuName = "Buildings/Commands/Build Unit", order = 120)]
    public class BuildUnitCommand : BaseCommand, IUnlockableCommand
    {
        [field: SerializeField] public AbstractUnitSO Unit { get; private set; }

        public override bool CanHandle(CommandContext context)
        {
            return context.Commandable is BaseBuilding && HasEnoughSupplies(context);
        }

        public override void Handle(CommandContext context)
        {
            BaseBuilding building = (BaseBuilding)context.Commandable;

            if (!HasEnoughSupplies(context) || (building.QueueSize == 0 && !HasEnoughSupplies(context))) return;

            building.BuildUnlockable(Unit);
        }

        public override bool IsLocked(CommandContext context) =>
            !HasEnoughSupplies(context) || !Unit.TechTree.IsUnlocked(context.Owner, Unit) || (context.Commandable is BaseBuilding building && building.QueueSize == 0 && !HasEnoughPopulation(context));

        public UnlockableSO[] GetUnmetDependencies(Owner owner)
        {
            return Unit.TechTree.GetUnmetDependencies(owner, Unit);
        }

        private bool HasEnoughSupplies(CommandContext context)
        {
            return Unit.Cost.Minerals <= Supplies.Minerals[context.Owner] && Unit.Cost.Gas <= Supplies.Gas[context.Owner];
        }

        private bool HasEnoughPopulation(CommandContext context)
        {
            if (Unit.PopulationConfig == null) return true;

            int newPopulation = Unit.PopulationConfig.PopulationCost + Supplies.Population[context.Owner];

            return newPopulation <= Supplies.PopulationLimit[context.Owner];
        }
    }
}
