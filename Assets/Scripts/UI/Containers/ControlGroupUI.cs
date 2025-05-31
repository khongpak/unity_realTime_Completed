using System.Collections.Generic;
using System.Linq;
using GameDevTV.RTS.UI.Components;
using GameDevTV.RTS.Units;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameDevTV.RTS.UI.Containers
{
    public class ControlGroupUI : MonoBehaviour, IUIElement<HashSet<AbstractCommandable>>
    {
        [SerializeField] private ControlGroupKeyboardHotkey[] controlGroupHotkeys;

        private HashSet<AbstractCommandable> selectedUnits;

        public void EnableFor(HashSet<AbstractCommandable> items)
        {
            selectedUnits = items;
        }

        public void Disable() {}

        private void Update()
        {
            if (!Keyboard.current.ctrlKey.isPressed) return;

            foreach (ControlGroupKeyboardHotkey groupHotKey in controlGroupHotkeys)
            {
                if (Keyboard.current[groupHotKey.Key].wasReleasedThisFrame && selectedUnits.Count > 0)
                {
                    groupHotKey.Group.EnableFor(selectedUnits, groupHotKey.Key, SelectUnits);
                }
            }
        }

        private void SelectUnits(HashSet<AbstractCommandable> units)
        {
            // toList creates a copy, so we don't mutate this collection while iterating over it.
            foreach (ISelectable selectable in selectedUnits.ToList())
            {
                selectable.Deselect();
            }

            foreach (ISelectable selectable in units)
            {
                selectable.Select();
            }
        }

        [System.Serializable]
        private struct ControlGroupKeyboardHotkey
        {
            [field: SerializeField] public Key Key { get; private set; }
            [field: SerializeField] public ControlGroup Group { get; private set; }
        }
    }
}
