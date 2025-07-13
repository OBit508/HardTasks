using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Rewired.UI.ControlMapper.ControlMapper;

namespace HardTasks.Skeld
{
    [HarmonyPatch(typeof(EmptyGarbageMinigame), "Begin")]
    internal class GarbageTask
    {
        public static void Postfix(EmptyGarbageMinigame __instance)
        {
            List<Transform> levers = new List<Transform>();
            for (int i = 0; i < 7; i++)
            {
                levers.Add(CreateLever(__instance, i, 0));
                levers.Add(CreateLever(__instance, i, 1));
            }
            Transform k = levers[new System.Random().Next(0, levers.Count - 1)];
            Transform lever = __instance.VibratePivot.GetChild(2);
            lever.localScale = new Vector3(0.4f, 0.4f, 1);
            lever.localPosition = k.localPosition;
            GameObject.Destroy(k.gameObject);
        }
        public static Transform CreateLever(EmptyGarbageMinigame minigame, int i, int r)
        {
            Transform lever = GameObject.Instantiate<Transform>(minigame.VibratePivot.GetChild(2), minigame.VibratePivot);
            lever.transform.localPosition = new Vector3(1.5f + (r * 0.5f), 0.8f - (i * 0.5f), -1);
            lever.localScale = new Vector3(0.4f, 0.4f, 1);
            return lever;
        }
    }
}
