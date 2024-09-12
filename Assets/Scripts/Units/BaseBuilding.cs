using System.Collections;
using UnityEngine;

namespace GameDevTV.RTS.Units
{
    public class BaseBuilding : AbstractCommandable
    {
        public void BuildUnit(UnitSO unit)
        {
            StartCoroutine(DoBuildUnit(unit));
        }

        private IEnumerator DoBuildUnit(UnitSO unit)
        {
            Debug.Log("starting the coroutine!");
            yield return new WaitForSeconds(unit.BuildTime);
            Debug.Log("build time has elapsed! instantiating the unit!");
            Instantiate(unit.Prefab, transform.position, Quaternion.identity);
        }
    }
}
