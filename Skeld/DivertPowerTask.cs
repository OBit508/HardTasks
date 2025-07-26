using HardTasks.CustomTask;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HardTasks.Skeld
{
    internal class DivertPower
    {
        public static Dictionary<Console, ArrowBehaviour> consoles = new Dictionary<Console, ArrowBehaviour>();
        public static DivertPowerTask GetFirst()
        {
            foreach (PlayerTask task in PlayerControl.LocalPlayer.myTasks)
            {
                DivertPowerTask divertPowerTask = task.TryCast<DivertPowerTask>();
                if (divertPowerTask != null)
                {
                    return divertPowerTask;
                }
            }
            return null;
        }
        public static bool IsDivertPowerTaskConsole(Console console)
        {
            foreach (TaskSet set in console.ValidTasks)
            {
                if (set.taskType == TaskTypes.DivertPower)
                {
                    return true;
                }
            }
            return false;
        }
        public static Console GetClosestDivertPowerConsole()
        {
            float dis = 10;
            Console closest = null;
            foreach (KeyValuePair<Console, ArrowBehaviour> pair in consoles)
            {
                float currentDis = Vector2.Distance(pair.key.transform.position, PlayerControl.LocalPlayer.transform.position);
                if (dis > currentDis)
                {
                    dis = currentDis;
                    closest = pair.key;
                }
            }
            return closest;
        }
        [HarmonyPatch(typeof(AcceptDivertPowerGame), "Start")]
        public class AcceptDivertPowerGamePatch
        {
            public static void Postfix(AcceptDivertPowerGame __instance)
            {
                DivertPowerTask task = GetFirst();
                if (task == null)
                {
                    return;
                }
                __instance.Console = GetClosestDivertPowerConsole();
                __instance.MyNormTask = task;
                __instance.MyTask = __instance.MyNormTask;
                __instance.Switch.GetComponent<ButtonBehavior>().OnClick.AddListener(new Action(delegate
                {
                    foreach (KeyValuePair<Console, ArrowBehaviour> pair in consoles)
                    {
                        if (pair.key == __instance.Console)
                        {
                            consoles.Remove(pair.key);
                            GameObject.Destroy(pair.value.gameObject);
                        }
                    }
                }));
            }
        }
        [HarmonyPatch(typeof(DivertPowerTask))]
        public class DivertPowerTaskPatch
        {
            [HarmonyPatch("ValidConsole")]
            [HarmonyPrefix]
            public static bool ValidConsolePrefix(DivertPowerTask __instance, [HarmonyArgument(0)] Console console, ref bool __result)
            {
                __result = false;
                if (__instance.taskStep == 0)
                {
                    __result = console.TaskTypes.Contains(TaskTypes.DivertPower);
                }
                else if (__instance.taskStep == 1)
                {
                    __result = consoles.ContainsKey(console);
                }
                return false;
            }
            [HarmonyPatch("AppendTaskText")]
            [HarmonyPrefix]
            public static bool AppendTaskTextPrefix(DivertPowerTask __instance, [HarmonyArgument(0)] Il2CppSystem.Text.StringBuilder sb)
            {
                __instance.Arrow.gameObject.SetActive(false);
                ShipStatus ship = ShipStatus.Instance;
                int consoles = 0;
                StringNames mapName = StringNames.MapNameSkeld;
                foreach (Console console in ship.AllConsoles)
                {
                    if (DivertPower.IsDivertPowerTaskConsole(console))
                    {
                        consoles++;
                    }
                }
                if (ship.name == "MiraShip(Clone)")
                {
                    mapName = StringNames.MapNameMira;
                }
                else if (ship.name == "PolusShip(Clone)")
                {
                    mapName = StringNames.MapNamePolus;
                }
                else if (ship.name == "Airship(Clone)")
                {
                    mapName = StringNames.MapNameAirship;
                }
                else if (ship.name == "FungleShip(Clone)")
                {
                    mapName = StringNames.MapNameFungle;
                }
                if (__instance.TaskStep == 0)
                {
                    sb.AppendLine(TranslationController.Instance.GetString(SystemTypes.Electrical) + ": Divert Power (0/" + (consoles + 1).ToString() + ")");
                }
                else if (__instance.TaskStep == 1)
                {
                    sb.AppendLine("<color=#FFFF00FF>" + TranslationController.Instance.GetString(mapName) + ": Accept Diverted Power (" + (1 + consoles - DivertPower.consoles.Count).ToString() + "/" + (consoles + 1).ToString() + ")</color>");
                }
                else
                {
                    sb.AppendLine("<color=#00DD00FF>" + TranslationController.Instance.GetString(mapName) + ": Divert Power (" + (consoles + 1).ToString() + "/" + (consoles + 1).ToString() + ")</color>");
                }
                return false;
            }
        }
        [HarmonyPatch(typeof(NormalPlayerTask))]
        public class NormalPlayerTaskPatch
        {
            [HarmonyPatch("Initialize")]
            [HarmonyPostfix]
            public static void InitializePostfix(NormalPlayerTask __instance)
            {
                DivertPowerTask task = __instance.TryCast<DivertPowerTask>();
                if (task != null && task.Owner == PlayerControl.LocalPlayer)
                {
                    consoles.Clear();
                    foreach (Console console in ShipStatus.Instance.AllConsoles)
                    {
                        if (!console.TaskTypes.Contains(TaskTypes.DivertPower) && IsDivertPowerTaskConsole(console))
                        {
                            ArrowBehaviour arrow = GameObject.Instantiate<ArrowBehaviour>(task.Arrow, task.Arrow.transform.parent);
                            arrow.target = console.transform.position;
                            arrow.gameObject.SetActive(false);
                            consoles.Add(console, arrow);
                        }
                    }
                }
            }
            [HarmonyPatch("NextStep")]
            [HarmonyPrefix]
            public static bool NextStepPrefix(NormalPlayerTask __instance)
            {
                DivertPowerTask task = __instance.TryCast<DivertPowerTask>();
                if (task != null && task.Owner == PlayerControl.LocalPlayer)
                {
                    if (task.TaskStep == 0)
                    {
                        foreach (KeyValuePair<Console, ArrowBehaviour> pair in consoles)
                        {
                            pair.value.gameObject.SetActive(true);
                        }
                        return true;
                    }
                    return consoles.Count == 0;
                }
                return true;
            }
        }
    }
}
