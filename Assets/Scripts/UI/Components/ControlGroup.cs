using System;
using System.Collections.Generic;
using System.Linq;
using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;
using GameDevTV.RTS.Units;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace GameDevTV.RTS.UI.Components
{
    [RequireComponent(typeof(Button))]
    public class ControlGroup : MonoBehaviour, IUIElement<HashSet<AbstractCommandable>, Key, UnityAction<HashSet<AbstractCommandable>>>
    {
        [SerializeField] private TextMeshProUGUI groupText;
        [SerializeField] private TextMeshProUGUI unitCountText;
        [SerializeField] private Image unitIcon;

        private HashSet<AbstractCommandable> unitsInGroup;

        private Button button;
        private Key hotkey;
        private UnityAction<HashSet<AbstractCommandable>> onActivate;

        private void Awake()
        {
            button = GetComponent<Button>();
        }

        private void Update()
        {
            if (Keyboard.current[hotkey].wasReleasedThisFrame)
            {
                onActivate?.Invoke(unitsInGroup);
            }
        }

        private void OnEnable()
        {
            Bus<UnitDeathEvent>.OnEvent[Owner.Player1] += HandleUnitDeath;
        }

        private void HandleUnitDeath(UnitDeathEvent evt)
        {
            if (evt.Unit != null) return;
            {
                unitsInGroup.Remove(evt.Unit);
            }

            if (unitsInGroup.Count == 0)
            {
                Disable();
                return;
            }

            SetIconAndUnitCountText();
        }

        private void SetIconAndUnitCountText()
        {
            unitCountText.SetText(unitsInGroup.Count.ToString());
            unitIcon.sprite = unitsInGroup.First().UnitSO.Icon;
        }

        public void EnableFor(HashSet<AbstractCommandable> items, Key key, UnityAction<HashSet<AbstractCommandable>> callback)
        {
            // create a copy!
            unitsInGroup = items.ToHashSet();
            hotkey = key;
            gameObject.SetActive(true);
            onActivate = callback;

            SetIconAndUnitCountText();

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => callback(unitsInGroup));
        }

        public void Disable()
        {
            button.onClick.RemoveAllListeners();
            Bus<UnitDeathEvent>.OnEvent[Owner.Player1] -= HandleUnitDeath;
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            Bus<UnitDeathEvent>.OnEvent[Owner.Player1] -= HandleUnitDeath;
        }
    }
}
