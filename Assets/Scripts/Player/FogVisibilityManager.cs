using System.Collections.Generic;
using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;
using GameDevTV.RTS.Units;
using UnityEngine;

namespace GameDevTV.RTS.Player
{
    [RequireComponent(typeof(Camera))]
    public class FogVisibilityManager : MonoBehaviour
    {
        private Camera fogOfWarCamera;

        private Texture2D visionTexture;
        private Rect textureRect;

        private HashSet<AbstractCommandable> aliveNotOwnedUnits = new(1000);

        private void Awake()
        {
            fogOfWarCamera = GetComponent<Camera>();
            visionTexture = new Texture2D(fogOfWarCamera.targetTexture.width, fogOfWarCamera.targetTexture.height);
            textureRect = new Rect(0, 0, visionTexture.width, visionTexture.height);

            Bus<UnitSpawnEvent>.RegisterForAll(HandleUnitSpawn);
            Bus<UnitDeathEvent>.RegisterForAll(HandleUnitDeath);
        }

        private void OnDestroy()
        {
            Bus<UnitSpawnEvent>.UnregisterForAll(HandleUnitSpawn);
            Bus<UnitDeathEvent>.UnregisterForAll(HandleUnitDeath);
        }

        private void LateUpdate()
        {
            ReadPixelsToVisionTexture();

            foreach(AbstractCommandable commandable in aliveNotOwnedUnits)
            {
                SetUnitVisibilityStatus(commandable);
            }
        }

        private void ReadPixelsToVisionTexture()
        {
            RenderTexture previousRenderTexture = RenderTexture.active;

            RenderTexture.active = fogOfWarCamera.targetTexture;
            visionTexture.ReadPixels(textureRect, 0, 0);
            RenderTexture.active = previousRenderTexture;
        }

        private void SetUnitVisibilityStatus(AbstractCommandable commandable)
        {
            Vector3 screenPoint = fogOfWarCamera.WorldToScreenPoint(commandable.transform.position);
            Color visibilityColor = visionTexture.GetPixel((int)screenPoint.x, (int)screenPoint.y);
            Debug.Log($"Determined {commandable.name} is {(visibilityColor.r > 0.9f ? "Visible!" : "Not Visible!")}");
        }

        private void HandleUnitSpawn(UnitSpawnEvent evt)
        {
            if (evt.Unit.Owner != Owner.Player1)
            {
                aliveNotOwnedUnits.Add(evt.Unit);
            }
        }

        private void HandleUnitDeath(UnitDeathEvent evt)
        {
            aliveNotOwnedUnits.Remove(evt.Unit);
        }
    }
}