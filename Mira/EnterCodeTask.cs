using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HardTasks.Mira
{
    [HarmonyPatch(typeof(EnterCodeMinigame))]
    internal class EnterCodeTask
    {
        [HarmonyPatch("Begin")]
        public static void Postfix(EnterCodeMinigame __instance)
        {
            __instance.targetNumber = new System.Random().Next(999999, 2147483647);
            __instance.TargetText.text = __instance.targetNumber.ToString();
        }
        [HarmonyPatch("EnterDigit")]
        public static bool Prefix(EnterCodeMinigame __instance, [HarmonyArgument(0)] int i)
        {
            if (__instance.animating)
            {
                return false;
            }
            if (__instance.done)
            {
                return false;
            }
            if (__instance.NumberText.text.Length >= __instance.TargetText.text.Length)
            {
                if (Constants.ShouldPlaySfx())
                {
                    SoundManager.Instance.PlaySound(__instance.RejectSound, false, 1f, null);
                }
                return false;
            }
            if (Constants.ShouldPlaySfx())
            {
                SoundManager.Instance.PlaySound(__instance.NumberSound, false, 1f, null).pitch = Mathf.Lerp(0.8f, 1.2f, (float)i / 9f);
            }
            __instance.numString += i.ToString();
            __instance.number = __instance.number * 10 + i;
            __instance.NumberText.text = __instance.numString;
            return false;
        }
    }
}
