using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace HardTasks.Skeld
{
    [HarmonyPatch(typeof(CardSlideGame))]
    internal class CardTask
    {
        public static int steps;
        [HarmonyPatch("Begin")]
        [HarmonyPostfix]
        public static void BeginPostfix()
        {
            steps = 0;
        }
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        public static void UpdatePostfix(CardSlideGame __instance)
        {
            if (__instance.State == CardSlideGame.TaskStages.Inserted)
            {
                string s = "s";
                if (steps == 9)
                {
                    s = "";
                }
                __instance.StatusText.text = "swipe the card " + (10 - steps) + " more time" + s;
            }
        }
        [HarmonyPatch("PutCardBack")]
        [HarmonyPrefix]
        public static bool PutCardBackPrefix(CardSlideGame __instance)
        {
            steps++;
            if (steps < 10)
            {
                __instance.State = CardSlideGame.TaskStages.Inserted;
            }
            return steps == 10;
        }
    }
}
