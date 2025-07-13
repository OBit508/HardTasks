using HarmonyLib;
using ImpossibleTasksV3.CustomTask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImpossibleTasksV3.Skeld
{
    [HarmonyPatch(typeof(UploadDataGame))]
    internal static class UploadTask
    {
        [HarmonyPatch("Begin")]
        [HarmonyPostfix]
        public static void OnBegin(UploadDataGame __instance)
        {
            __instance.gameObject.AddComponent<CustomUploadTask>().Game = __instance;
            __instance.transform.GetChild(8).gameObject.SetActive(false);
        }
        [HarmonyPatch("Click")]
        [HarmonyPrefix]
        public static bool StartUpload(UploadDataGame __instance)
        {
            __instance.StartCoroutine(__instance.DoRun());
            __instance.Status.gameObject.SetActive(true);
            __instance.Gauge.transform.localScale = new UnityEngine.Vector3(0.05f, 1, 1);
            __instance.Button.gameObject.SetActive(false);
            __instance.GetComponent<CustomUploadTask>().doPercent = true;
            return false;
        }
    }
}
