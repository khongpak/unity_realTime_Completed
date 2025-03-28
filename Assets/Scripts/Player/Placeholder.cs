using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;
using GameDevTV.RTS.Units;
using UnityEngine;

namespace GameDevTV.RTS.Player
{
    public class Placeholder : MonoBehaviour, IHideable
    {
        public Transform Transform => transform;
        public bool IsVisible { get; }
        public Owner Owner { get; set; }
        public GameObject ParentObject { get; set; }

        private void Start()
        {
            Bus<PlaceholderSpawnEvent>.Raise(Owner, new PlaceholderSpawnEvent(this));
        }

        public void SetVisible(bool isVisible)
        {
            if (isVisible && ParentObject == null)
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            Bus<PlaceholderDestroyEvent>.Raise(Owner, new PlaceholderDestroyEvent(this));
        }
    }
}