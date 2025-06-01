using System;
using System.Linq;
using GameDevTV.RTS.Commands;
using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;
using GameDevTV.RTS.Player;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace GameDevTV.RTS.Units
{
    public abstract class AbstractCommandable : MonoBehaviour, ISelectable, IDamageable, IHideable
    {
        [field: SerializeField] public bool IsSelected { get; protected set; }
        [field: SerializeField] public int CurrentHealth { get; protected set; }
        [field: SerializeField] public int MaxHealth { get; protected set; }
        [field: SerializeField] public Owner Owner { get; set; }
        [field: SerializeField] public bool IsVisible { get; private set; } = true;
        public Transform Transform => this == null ? null : transform;
        [field: SerializeField] public BaseCommand[] AvailableCommands { get; private set; }
        [field: SerializeField] public AbstractUnitSO UnitSO { get; private set; }
        [SerializeField] protected DecalProjector decalProjector;
        [SerializeField] protected Transform VisionTransform;
        [SerializeField] protected Renderer MinimapRenderer;

        public delegate void HealthUpdatedEvent(AbstractCommandable commandable, int lastHealth, int newHealth);
        public event HealthUpdatedEvent OnHealthUpdated;
        
        public event IHideable.VisibilityChangeEvent OnVisibilityChanged;

        private BaseCommand[] initialCommands;
        private Renderer[] renderers = Array.Empty<Renderer>();
        private ParticleSystem[] particleSystems = Array.Empty<ParticleSystem>();

        private static int COLOR_ID = Shader.PropertyToID("_BaseColor");

        protected virtual void Awake()
        {
            UnitSO = UnitSO.Clone() as AbstractUnitSO;

            renderers = GetComponentsInChildren<Renderer>();
            particleSystems = GetComponentsInChildren<ParticleSystem>();
        }

        protected virtual void Start()
        {
            if (UnitSO.SightConfig != null && VisionTransform != null)
            {
                float size = UnitSO.SightConfig.SightRadius * 2;
                VisionTransform.localScale = new Vector3(size, size, size);
                VisionTransform.gameObject.SetActive(Owner == Owner.Player1);
            }

            initialCommands = UnitSO.Prefab.GetComponent<AbstractCommandable>().AvailableCommands;
            SetCommandOverrides(null);

            if (MinimapRenderer != null)
            {
                MinimapRenderer.material.SetColor(COLOR_ID, Owner == Owner.Player1 ? Color.green : Color.red);
            }

            if (UnitSO.PopulationConfig != null)
            {
                Bus<PopulationEvent>.Raise(Owner, new PopulationEvent(
                    Owner,
                    UnitSO.PopulationConfig.PopulationCost,
                    UnitSO.PopulationConfig.PopulationSupply
                ));
            }

            Bus<UpgradeResearchedEvent>.OnEvent[Owner] += HandleUpgradeResearched;
        }

        protected virtual void OnDestroy()
        {
            Bus<UpgradeResearchedEvent>.OnEvent[Owner] -= HandleUpgradeResearched;

            if (UnitSO.PopulationConfig != null)
            {
                Bus<PopulationEvent>.Raise(Owner, new PopulationEvent(
                    Owner,
                    -UnitSO.PopulationConfig.PopulationCost,
                    -UnitSO.PopulationConfig.PopulationSupply
                ));
            }
        }

        public virtual void Select()
        {
            if (decalProjector != null)
            {
                decalProjector.gameObject.SetActive(true);
            }

            IsSelected = true;
            Bus<UnitSelectedEvent>.Raise(Owner, new UnitSelectedEvent(this));
        }

        public virtual void Deselect()
        {
            if (decalProjector != null)
            {
                decalProjector.gameObject.SetActive(false);
            }

            IsSelected = false;
            SetCommandOverrides(null);

            Bus<UnitDeselectedEvent>.Raise(Owner, new UnitDeselectedEvent(this));
        }

        public void SetCommandOverrides(BaseCommand[] commands)
        {
            if (commands == null || commands.Length == 0)
            {
                AvailableCommands = initialCommands;
            }
            else
            {
                AvailableCommands = commands;
            }

            if (IsSelected)
            {
                Bus<UnitSelectedEvent>.Raise(Owner, new UnitSelectedEvent(this));
            }
        }

        public void TakeDamage(int damage)
        {
            int lastHealth = CurrentHealth;
            CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, CurrentHealth);

            OnHealthUpdated?.Invoke(this, lastHealth, CurrentHealth);
            if (CurrentHealth == 0)
            {
                Die();
            }
        }

        public void Die()
        {
            Destroy(gameObject);
        }

        public void Heal(int amount)
        {
            int lastHealth = CurrentHealth;
            CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, MaxHealth);
            OnHealthUpdated?.Invoke(this, lastHealth, CurrentHealth);
        }

        public void SetVisible(bool isVisible)
        {
            if (isVisible == IsVisible) return;

            IsVisible = isVisible;
            OnVisibilityChanged?.Invoke(this, isVisible);

            if (IsVisible)
            {
                OnGainVisibility();
            }
            else
            {
                OnLoseVisibility();
            }
        }

        protected virtual void OnGainVisibility()
        {
            foreach(Renderer renderer in renderers)
            {
                renderer.enabled = true;
            }

            foreach(ParticleSystem particleSystem in particleSystems)
            {
                particleSystem.gameObject.SetActive(true);
            }
        }

        protected virtual void OnLoseVisibility()
        {
            foreach (Renderer renderer in renderers)
            {
                renderer.enabled = false;
            }

            foreach (ParticleSystem particleSystem in particleSystems)
            {
                particleSystem.gameObject.SetActive(false);
            }
        }

        private void HandleUpgradeResearched(UpgradeResearchedEvent evt)
        {
            if (evt.Owner == Owner && UnitSO.Upgrades.Contains(evt.Upgrade))
            {
                evt.Upgrade.Apply(UnitSO);
            }
        }
    }
}
