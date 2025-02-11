using GameDevTV.RTS.Units;
using UnityEngine;

namespace GameDevTV.RTS.TechTree
{
    [CreateAssetMenu(fileName = "Additive Int Modifier", menuName = "Tech Tree/Modifiers/Additive Int Modifier", order = 160)]
    public class AdditiveIntModifierSO : UpgradeSO
    {
        [field: SerializeField] public int Amount { get; private set; }

        public override void Apply(AbstractUnitSO unit)
        {
            Debug.Log($"{Name} is applying {Amount} to {PropertyPath}.");
        }
    }
}