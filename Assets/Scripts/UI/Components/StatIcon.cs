using System.Linq;
using GameDevTV.RTS.Units;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameDevTV.RTS.UI.Components
{
    public class StatIcon : MonoBehaviour, IUIElement<AbstractCommandable>, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private TextMeshProUGUI upgradeLabel;
        [SerializeField] private TextMeshProUGUI amountLabel;
        [SerializeField] private IconType iconType;
        [SerializeField] private Tooltip tooltip;
        [SerializeField] private Image icon;

        public void EnableFor(AbstractCommandable item)
        {
            gameObject.SetActive(true);
            int upgradeCount = item.UnitSO.Upgrades.Count(
                upgradeSO => item.UnitSO.TechTree.IsResearched(item.Owner, upgradeSO)
                             && upgradeSO.PropertyPath.Contains(iconType.ToString())
            );

            int amount = 0;

            if (iconType == IconType.Attack)
            {
                if (item.UnitSO == null || item.UnitSO is not UnitSO unitSO || unitSO.AttackConfig == null)
                {
                    gameObject.SetActive(false);
                    return;
                }

                amount = unitSO.AttackConfig.Damage;
                if (tooltip != null)
                {
                    tooltip.SetText($"{amount} Damage");
                }

                if (icon != null)
                {
                    icon.sprite = unitSO.AttackConfig.Icon;
                }
            }

            upgradeLabel.SetText(upgradeCount.ToString());
            amountLabel.SetText(amount.ToString());
        }

        public void Disable() {}

        public void OnPointerEnter(PointerEventData _)
        {
            Invoke(nameof(ShowTooltip), tooltip.HoverDelay);
        }

        public void OnPointerExit(PointerEventData _)
        {
            CancelInvoke();
            tooltip.gameObject.SetActive(false);
        }

        private void ShowTooltip()
        {
            tooltip.gameObject.SetActive(true);
        }

        private enum IconType
        {
            Attack
        }
    }
}
