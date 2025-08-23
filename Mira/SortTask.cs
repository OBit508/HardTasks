using HardTasks.CustomTask;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Rewired.UI.ControlMapper.ControlMapper;

namespace HardTasks.Mira
{
    [HarmonyPatch(typeof(SortMinigame), "Start")]
    internal class SortTask
    {
        public static void Postfix(SortMinigame __instance)
        {
            SortGameObject[] Objects = __instance.MyNormTask.MinigamePrefab.TryCast<SortMinigame>().Objects;
            foreach (SortGameObject obj in __instance.Objects)
            {
                GameObject.Destroy(obj.gameObject);
            }
            List<SortGameObject> newObjects = new List<SortGameObject>();
            for (int i = 0; i < 25; i++)
            {
                SortGameObject objectPrefab = Objects[new System.Random().Next(0, Objects.Count() - 1)];
                SortGameObject newObject = GameObject.Instantiate<SortGameObject>(objectPrefab, __instance.transform);
                newObject.transform.localPosition = new Vector3(UnityEngine.Random.Range(-2.8f, 2.8f), objectPrefab.transform.localPosition.y, objectPrefab.transform.localPosition.z);
                newObject.transform.localScale /= 2;
                newObjects.Add(newObject);
            }
            __instance.Objects = newObjects.ToArray();
        }
    }
}
