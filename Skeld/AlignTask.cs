using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx.Unity.IL2CPP.Utils;
using UnityEngine;

namespace HardTasks.Skeld
{
    [HarmonyPatch(typeof(AlignGame))]
    internal class AlignTask
    {
        public static int steps;
        public static bool moving;
        [HarmonyPatch("Begin")]
        [HarmonyPostfix]
        public static void BeginPostfix()
        {
            steps = 0;
            moving = false;
        }
        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        public static bool UpdatePrefix(AlignGame __instance)
        {
            if (steps < 3 && __instance.col.transform.localPosition.y > -0.2f && __instance.col.transform.localPosition.y < 0.2f && !moving)
            {
                float targetPosition = __instance.YRange.Next();
                System.Collections.IEnumerator move()
                {
                    moving = true;
                    while (Vector2.Distance(__instance.col.transform.localPosition, new Vector3(2.01f, 2.13f, 0)) < 0.2f)
                    {
                        yield return new WaitForSeconds(0.05f);
                        Vector3 pos = Vector3.MoveTowards(__instance.col.transform.localPosition, new Vector3(2.01f, 2.13f, 0), 5 * Time.deltaTime);
                        pos.z = 0;
                        __instance.col.transform.localPosition = pos;
                    }
                    moving = false;
                    steps++;
                    yield return null;
                }
                __instance.StartCoroutine(move());
                return false;
            }
            return !moving;
        }
        public static float GetXFromY(float y)
        {
            float x1 = 1.55f;
            float x2 = 2.01f;
            float y1 = 0f;
            float y2 = 2.13f;

            return x1 + ((x2 - x1) / (y2 - y1)) * (y - y1);
        }
    }
}
