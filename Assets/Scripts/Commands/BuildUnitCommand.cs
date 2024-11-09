using UnityEngine;
using GameDevTV.RTS.Units;

namespace GameDevTV.RTS.Commands
{
    [CreateAssetMenu(fileName = "Build Unit", menuName = "Buildings/Commands/Build Unit", order = 120)]
    public class BuildUnitCommand : ActionBase
    {
        [field: SerializeField] public AbstractUnitSO Unit { get; private set; }

        public override bool CanHandle(CommandContext context)
        {
            return context.Commandable is BaseBuilding;
        }

        public override void Handle(CommandContext context)
        {
            BaseBuilding building = (BaseBuilding)context.Commandable;
            building.BuildUnit(Unit);
        }
    }
}