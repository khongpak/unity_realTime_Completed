using GameDevTV.RTS.Player;
using GameDevTV.RTS.Units;
using UnityEngine;

namespace GameDevTV.RTS.Commands
{
    [CreateAssetMenu(fileName = "Set Rally Point", menuName = "Buildings/Commands/Set Rally Point", order = 121)]
    public class SetRallyPointCommand : BaseCommand
    {
        [field: SerializeField] public LayerMask IgnoreObjectLayers { get; private set; }

        public override bool CanHandle(CommandContext context)
        {
            return context.Commandable is BaseBuilding;
        }

        public override void Handle(CommandContext context)
        {
            BaseBuilding building = context.Commandable as BaseBuilding;

            RallyPoint rallyPoint;
            if (context.Hit.collider.gameObject == building.gameObject)
            {
                rallyPoint = new RallyPoint(false, Vector3.zero, null);
            }
            else if ((IgnoreObjectLayers.value & (1 << context.Hit.collider.gameObject.layer)) == 0 && FogVisibilityManager.Instance.IsVisible(context.Hit.collider.transform.position))
            {
                rallyPoint = new RallyPoint(true, context.Hit.point, context.Hit.collider.gameObject);
            }
            else
            {
                rallyPoint = new RallyPoint(true, context.Hit.point, null);
            }

            building.RallyPoint = rallyPoint;
        }

        public override bool IsLocked(CommandContext context) => false;
    }
}
