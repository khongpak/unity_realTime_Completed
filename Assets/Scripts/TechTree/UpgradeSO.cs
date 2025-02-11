using GameDevTV.RTS.Units;
using UnityEngine;

namespace GameDevTV.RTS.TechTree
{
    public abstract class UpgradeSO : UnlockableSO, IModifier
    {
        [field: SerializeField] public string PropertyPath { get; private set; }

        public abstract void Apply(AbstractUnitSO unit);
    }
}