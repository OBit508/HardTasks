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
        public static List<int> Ids = new List<int>();
        [HarmonyPatch("Update")]
        public static void Postfix(SampleMinigame __instance)
        {
            __instance.TimePerStep = 30;
            for (int i = 0; i < __instance.Tubes.Count; i++)
            {
                __instance.Tubes[i].color = Ids.Contains(i) ? Color.red : Color.blue;
            }
        }
        [HarmonyPatch("AnomalyId", MethodType.Setter)]
        public static bool Prefix(SampleMinigame __instance, ref int __value)
        {
            Ids = new List<int>() { 0, 1, 2, 3, 4 };
            Ids.Remove(__value);
            for (int i = 0; i < 2; i++)
            {
                int id = Ids[new System.Random().Next(0, Ids.Count - 1)];
                Ids.Remove(id);
            }
            __instance.MyNormTask.Data[1] = (byte)__value;
            return false;
        }
    }
}
