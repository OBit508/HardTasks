using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardTasks
{
    [HarmonyPatch(typeof(PingTracker), "Update")]
    internal class PingTrackerPatch
    {
        public static void Postfix(PingTracker __instance)
        {
            __instance.text.text += "\nHardTasks 1.0.0\n<size=1.5>https://github.com/OBit508/HardTasks</size>";
            __instance.text.horizontalAlignment = TMPro.HorizontalAlignmentOptions.Center;
        }
    }
}
