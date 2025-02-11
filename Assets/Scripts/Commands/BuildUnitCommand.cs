using UnityEngine;
using GameDevTV.RTS.Units;
using GameDevTV.RTS.Player;
using GameDevTV.RTS.TechTree;

namespace GameDevTV.RTS.Commands
{
    [CreateAssetMenu(fileName = "Build Unit", menuName = "Buildings/Commands/Build Unit", order = 120)]
    public class BuildUnitCommand : BaseCommand
    {
        [SerializeField] private TechTreeSO techTree;
        [field: SerializeField] public AbstractUnitSO Unit { get; private set; }

        public override bool CanHandle(CommandContext context)
        {
            return context.Commandable is BaseBuilding && HasEnoughSupplies(context);
        }

        public override void Handle(CommandContext context)
        {
            if (!HasEnoughSupplies(context)) return;

            BaseBuilding building = (BaseBuilding)context.Commandable;
            building.BuildUnit(Unit);
        }

        public override bool IsLocked(CommandContext context) => 
            !HasEnoughSupplies(context) || !techTree.IsUnlocked(context.Owner, Unit);

        private bool HasEnoughSupplies(CommandContext context)
        {
            return Unit.Cost.Minerals <= Supplies.Minerals[context.Owner] && Unit.Cost.Gas <= Supplies.Gas[context.Owner];
        }
    }
}