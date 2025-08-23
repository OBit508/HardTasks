using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace HardTasks.Mira
{
    [HarmonyPatch(typeof(CrystalMinigame))]
    internal class CrystalTask
    {
        public static Dictionary<Transform, ChangeableValue<Vector3>> Slots = new Dictionary<Transform, ChangeableValue<Vector3>>();
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void StartPostfix(CrystalMinigame __instance)
        {
            Slots.Clear();
            for (int i = 0; i < __instance.CrystalSlots.Count; i++)
            {
                __instance.CrystalSlots[i].localPosition = new Vector3(UnityEngine.Random.Range(-2.4f, 2.4f), UnityEngine.Random.Range(-2.28f, 2.28f), -0.025f);
                __instance.CrystalSlots[i].gameObject.AddComponent<SpriteRenderer>().sprite = __instance.CrystalPieces[i].GetComponent<SpriteRenderer>().sprite;
                __instance.CrystalSlots[i].gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.3f);
                Slots.Add(__instance.CrystalSlots[i], new ChangeableValue<Vector3>(new Vector3(UnityEngine.Random.Range(-2.4f, 2.4f), UnityEngine.Random.Range(-2.28f, 2.28f), -0.025f)));
            }
        }
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        public static void UpdatePostfix(CrystalMinigame __instance)
        {
            foreach (KeyValuePair<Transform, ChangeableValue<Vector3>> pair in Slots)
            {
                if (Vector2.Distance(pair.Value.Value, pair.Key.localPosition) <= 0.3f)
                {
                    pair.Value.Value = new Vector3(UnityEngine.Random.Range(-2.4f, 2.4f), UnityEngine.Random.Range(-2.28f, 2.28f), -0.025f);
                }
                pair.Key.localPosition = Vector2.MoveTowards(pair.Key.localPosition, pair.Value.Value, Time.deltaTime * 4.5f);
            }
        }
    }
}
