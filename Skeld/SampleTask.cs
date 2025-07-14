using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HardTasks.Skeld
{
    [HarmonyPatch(typeof(SampleMinigame))]
    internal class SampleTask
    {
        public static List<SpriteRenderer> liquid = new List<SpriteRenderer>();
        [HarmonyPatch("Begin")]
        [HarmonyPostfix]
        public static void BeginPostfix(SampleMinigame __instance)
        {
            liquid.Clear();
        }
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        public static void UpdatePostfix(SampleMinigame __instance)
        {
            if (__instance.MyNormTask.TimerStarted == NormalPlayerTask.TimerState.Finished && liquid.Count <= 0)
            {
                __instance.AnomalyId = new System.Random().Next(0, 4);
                liquid = __instance.Tubes.ToList();
                liquid.Remove(liquid[__instance.AnomalyId]);
                liquid.Remove(liquid[new System.Random().Next(0, liquid.Count - 1)]);
                liquid.Remove(liquid[new System.Random().Next(0, liquid.Count - 1)]);
                foreach (SpriteRenderer rend in __instance.Tubes)
                {
                    if (liquid.Contains(rend))
                    {
                        rend.color = Color.blue;
                        return;
                    }
                    rend.color = Color.red;
                }
            }
        }
    }
}
