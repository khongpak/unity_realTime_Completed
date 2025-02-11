using System;
using System.Collections.Generic;
using System.Linq;
using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;
using GameDevTV.RTS.Units;
using UnityEngine;

namespace GameDevTV.RTS.TechTree
{
    [CreateAssetMenu(fileName = "Tech Tree", menuName = "Tech Tree/Tech Tree", order = 1)]
    public class TechTreeSO : ScriptableObject
    {
        [SerializeField] private List<UnlockableSO> allUnlockables = new();
        public IEnumerable<UnlockableSO> AllUnlockables => allUnlockables.ToList();

        private Dictionary<Owner, Dictionary<UnlockableSO, Dependency>> techTrees;

        private void OnEnable()
        {
            if (techTrees == null)
            {
                BuildTechTrees();
            }

            Bus<BuildingSpawnEvent>.RegisterForAll(HandleBuildingSpawn);
        }

        private void OnDisable()
        {
            techTrees = null;
            Bus<BuildingSpawnEvent>.UnregisterForAll(HandleBuildingSpawn);
        }

        private void HandleBuildingSpawn(BuildingSpawnEvent evt)
        {
            foreach(KeyValuePair<UnlockableSO, Dependency> keyValuePair in techTrees[evt.Owner])
            {
                keyValuePair.Value.UnlockDependency(evt.Building.BuildingSO);
            }
        }

        private void BuildTechTrees()
        {
            techTrees = new Dictionary<Owner, Dictionary<UnlockableSO, Dependency>>();

            foreach(Owner owner in Enum.GetValues(typeof(Owner)))
            {
                techTrees.Add(owner, new Dictionary<UnlockableSO, Dependency>());

                foreach(UnlockableSO unlockableSO in allUnlockables)
                {
                    techTrees[owner].Add(unlockableSO, new Dependency(unlockableSO));
                }
            }
        }

        private readonly struct Dependency
        {
            public HashSet<UnlockableSO> Dependencies { get; }
            private readonly Dictionary<UnlockableSO, int> metDependencies;

            public Dependency(UnlockableSO unlockable)
            {
                Dependencies = new HashSet<UnlockableSO>(unlockable.UnlockRequirements);
                metDependencies = new Dictionary<UnlockableSO, int>(Dependencies.Count);
            }

            public void UnlockDependency(UnlockableSO dependency)
            {
                Debug.Log($"Attempting to unlock dependency {dependency.Name}");

                if (Dependencies.Contains(dependency) && !metDependencies.TryAdd(dependency, 1))
                {
                    metDependencies[dependency]++;
                }

                if (metDependencies.ContainsKey(dependency))
                {
                    Debug.Log($"Met dependencies for {dependency.Name}: {metDependencies[dependency]}");
                }
            }
        }
    }
}