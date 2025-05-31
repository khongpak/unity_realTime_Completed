using System.Collections.Generic;
using GameDevTV.RTS.UI.Components;
using GameDevTV.RTS.Units;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameDevTV.RTS.UI.Containers
{
    public class MultipleUnitsSelectedUI : MonoBehaviour, IUIElement<HashSet<AbstractCommandable>>
    {
        [SerializeField] private UIUnitSelectedButton[] selectedUnitButtons;

        private HashSet<AbstractCommandable> selectedUnits;

        public void EnableFor(HashSet<AbstractCommandable> items)
        {
            gameObject.SetActive(true);

            if (selectedUnitButtons.Length < items.Count)
            {
                Debug.LogWarning(
                    $"Too many units were passed to MultipleUnitsSelectedUI! Ensure now more than {selectedUnitButtons.Length} " +
                    "units are selected at a time, or update the UI to handle more units!"
                );
            }

            int i = 0;
            foreach (AbstractCommandable commandable in items)
            {
                selectedUnitButtons[i].EnableFor(commandable, () => HandleClick(commandable));
                i++;
            }

            for (; i < selectedUnitButtons.Length; i++)
            {
                selectedUnitButtons[i].Disable();
            }

            selectedUnits = items;
        }

        public void Disable() => gameObject.SetActive(false);

        private void HandleClick(AbstractCommandable clickedCommandable)
        {
            if (Keyboard.current.shiftKey.isPressed)
            {
                clickedCommandable.Deselect();
            }
            else
            {
                selectedUnits.Remove(clickedCommandable);

                // commandable.Deselect() will trigger an event that will trigger EnableFor to be called again, which
                // will update selectedUnits, so we need a copy to iterate over safely.
                AbstractCommandable[] commandables = new AbstractCommandable[selectedUnits.Count];
                selectedUnits.CopyTo(commandables);

                foreach (AbstractCommandable commandable in commandables)
                {
                    commandable.Deselect();
                }

                clickedCommandable.Select();
            }
        }
    }
}
