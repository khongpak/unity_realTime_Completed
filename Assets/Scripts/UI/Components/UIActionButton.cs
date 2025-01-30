using System.Collections.Generic;
using System.Linq;
using GameDevTV.RTS.Commands;
using GameDevTV.RTS.Units;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameDevTV.RTS.UI.Components
{
    [RequireComponent(typeof(Button))]
    public class UIActionButton : MonoBehaviour, IUIElement<BaseCommand, IEnumerable<AbstractCommandable>, UnityAction>, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image icon;
        [SerializeField] private Tooltip tooltip;

        private bool isActive;
        private RectTransform rectTransform;
        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            rectTransform = GetComponent<RectTransform>();
            Disable();
        }

        public void EnableFor(BaseCommand command, IEnumerable<AbstractCommandable> selectedUnits, UnityAction onClick)
        {
            button.onClick.RemoveAllListeners();
            SetIcon(command.Icon);
            button.interactable = selectedUnits.Any((unit) => !command.IsLocked(new CommandContext(unit, new RaycastHit())));
            button.onClick.AddListener(onClick);
            isActive = true;

            if (tooltip != null)
            {
                tooltip.SetText(GetTooltipText(command));
            }
        }

        public void Disable()
        {
            SetIcon(null);
            button.interactable = false;
            button.onClick.RemoveAllListeners();
            isActive = false;
            CancelInvoke();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (isActive)
            {
                Invoke(nameof(ShowTooltip), tooltip.HoverDelay);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (tooltip != null)
            {
                tooltip.Hide();
            }
            CancelInvoke();
        }

        private void ShowTooltip()
        {
            if (tooltip != null)
            {
                tooltip.Show();
                tooltip.RectTransform.position = new Vector2(
                    rectTransform.position.x + rectTransform.rect.width / 2f,
                    rectTransform.position.y + rectTransform.rect.height / 2f
                );
            }
        }

        private void SetIcon(Sprite icon)
        {
            if (icon == null)
            {
                this.icon.enabled = false;
            }
            else
            {
                this.icon.sprite = icon;
                this.icon.enabled = true;
            }
        }

        private string GetTooltipText(BaseCommand command)
        {
            string tooltipText = command.Name + "\n";

            SupplyCostSO supplyCost = null;
            if (command is BuildUnitCommand unitCommand)
            {
                supplyCost = unitCommand.Unit.Cost;
            }
            else if (command is BuildBuildingCommand buildingCommand)
            {
                supplyCost = buildingCommand.Building.Cost;
            }

            if (supplyCost != null)
            {
                if (supplyCost.Minerals > 0)
                {
                    tooltipText += $"{supplyCost.Minerals} Minerals.";
                }
                if (supplyCost.Gas > 0)
                {
                    tooltipText += $"{supplyCost.Gas} Gas.";
                }
            }

            return tooltipText;
        }
    }
}
