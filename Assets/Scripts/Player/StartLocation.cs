using System.Collections;
using GameDevTV.RTS.Environment;
using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;
using GameDevTV.RTS.Units;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace GameDevTV.RTS.Player
{
    public class StartLocation : MonoBehaviour
    {
        [SerializeField] private Owner player;
        [SerializeField] private BuildingSO startingBuilding;
        [SerializeField] private UnitSupplyData[] startingUnits;
        [SerializeField] private StartingSupplies[] startingSupplies;

        private IEnumerator Start()
        {
            GameObject buildingGO = Instantiate(startingBuilding.Prefab, transform.position, transform.rotation);
            BaseBuilding buildingInstance = buildingGO.GetComponent<BaseBuilding>();
            buildingInstance.Owner = player;
            buildingInstance.enabled = true;
            buildingInstance.Heal(startingBuilding.Health);

            yield return null; // Collider bounds are not always set the same frame as it's instantiated. Wait 1 frame to spawn units.
            Bounds bounds = buildingInstance.GetComponent<Collider>().bounds;
            for (int i = 0; i < startingUnits.Length; i++)
            {
                for (int count = 0; count < startingUnits[i].NumberToSpawn; count++)
                {
                    Vector3 spawnLocation = new(bounds.min.x + (i + count) / 2f, bounds.min.y, bounds.min.z);
                    GameObject unitGO = Instantiate(startingUnits[i].UnitSO.Prefab, spawnLocation, Quaternion.Euler(0, Random.value * 180, 0));
                    AbstractCommandable commandable = unitGO.GetComponent<AbstractCommandable>();
                    commandable.Owner = player;
                    Bus<PopulationEvent>.Raise(player,
                        new PopulationEvent(
                            player,
                            commandable.UnitSO.PopulationConfig.PopulationCost,
                            commandable.UnitSO.PopulationConfig.PopulationSupply
                    ));
                }
            }

            foreach (StartingSupplies startingSupply in startingSupplies)
            {
                Bus<SupplyEvent>.Raise(player, new SupplyEvent(player, startingSupply.StartingAmount, startingSupply.SupplySO));
            }

            Destroy(GetComponentInChildren<DecalProjector>());
            enabled = false;
        }

        [System.Serializable]
        private struct UnitSupplyData
        {
            [field: SerializeField] public AbstractUnitSO UnitSO { get; private set; }
            [field: SerializeField] public int NumberToSpawn { get; private set; }
        }

        [System.Serializable]
        private struct StartingSupplies
        {
            [field: SerializeField] public SupplySO SupplySO { get; private set; }
            [field: SerializeField] public int StartingAmount { get; private set; }
        }
    }
}
