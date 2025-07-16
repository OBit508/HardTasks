using HardTasks.CustomTask;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardTasks.Patches
{
    [HarmonyPatch(typeof(PlayerControl), "Start")]
    internal class PlayerControlPatch
    {
        public static void Postfix(PlayerControl __instance)
        { 
        }
    }
}
