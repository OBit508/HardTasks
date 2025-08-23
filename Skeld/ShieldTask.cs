using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HardTasks.Skeld
{
    [HarmonyPatch(typeof(ShieldMinigame))]
    internal class ShieldTask
    {
        public static float timer;
        public static HorizontalGauge Gauge;
        [HarmonyPatch("Begin")]
        [HarmonyPostfix]
        public static void BeginPostfix(ShieldMinigame __instance)
        {
            HorizontalGauge gaugePrefab;
            if (Utils.GetMinigamePrefab(TaskTypes.UploadData) != null)
            {
                gaugePrefab = Utils.GetMinigamePrefab(TaskTypes.UploadData).TryCast<UploadDataGame>().Gauge;
            }
            else
            {
                gaugePrefab = Utils.GetMinigamePrefab(TaskTypes.ProcessData).TryCast<ProcessDataMinigame>().Gauge;
            }
            timer = 1.7f;
            Gauge = GameObject.Instantiate<HorizontalGauge>(gaugePrefab, __instance.transform);
            Gauge.transform.localPosition = new Vector3(0, -2.5f, 0);
            for (int i = 0; i < __instance.Shields.Count; i++)
            {
                __instance.Shields[i].color = __instance.OnColor;
            }
            __instance.shields = 0;
        }
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        public static void UpdatePostfix(ShieldMinigame __instance)
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                for (int i = 0; i < __instance.Shields.Count; i++)
                {
                    __instance.Shields[i].color = __instance.OffColor;
                }
                __instance.shields = 0;
                timer = 2;
            }
            Gauge.Value = timer / 2;
        }
    }
}
