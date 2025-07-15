using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardTasks.Skeld
{
    [HarmonyPatch(typeof(MedScanMinigame), "Begin")]
    internal class ScanTask
    {
        public static void Prefix(MedScanMinigame __instance)
        {
            __instance.ScanDuration = 40;
        }
    }
}
