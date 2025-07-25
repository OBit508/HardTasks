using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HardTasks.Skeld
{
    [HarmonyPatch(typeof(VentCleaningMinigame))]
    internal class VentTask
    {
        [HarmonyPatch("OpenVent")]
        [HarmonyPostfix]
        public static void OpenVentPostfix(VentCleaningMinigame __instance)
        {
            for (int i = 0; i<= 10; i++)
            {
                __instance.SpawnDirt();
                __instance.numberOfDirts++;
            }
        }
        [HarmonyPatch("CleanUp")]
        [HarmonyPrefix]
        public static bool CleanUpPrefix(VentCleaningMinigame __instance, [HarmonyArgument(0)] VentDirt ventDirt)
        {
            SpriteRenderer rend = ventDirt.GetComponent<SpriteRenderer>();
            Color color = rend.color;
            color.a -= 0.2f;
            rend.color = color;
            return rend.color.a <= 0;
        }
    }
}
