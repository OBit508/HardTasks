using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HardTasks.Skeld
{
    internal class DivertPower
    {
        public static Dictionary<SystemTypes, List<(Console console, ArrowBehaviour arrow)>> tasks = new Dictionary<SystemTypes, List<(Console console, ArrowBehaviour arrow)>>();
        [HarmonyPatch(typeof(AcceptDivertPowerGame), "Start")]
        public class AcceptDivertPowerGamePatch
        {
            public static void Postfix(AcceptDivertPowerGame __instance)
            {
                DivertPowerTask task = null;
                foreach (SystemTypes type in tasks.Keys)
                {
                    task = Utils.GetTaskByTarget(type);
                    if (task != null)
                    {
                        break;
                    }
                }
                if (task == null)
                {
                    return;
                }
                __instance.Console = Utils.GetClosestDivertPowerConsole(__instance, task.TargetSystem);
                __instance.MyNormTask = task;
                __instance.MyTask = __instance.MyNormTask;
                __instance.Switch.GetComponent<ButtonBehavior>().OnClick.AddListener(new Action(delegate
                {
                    foreach ((Console console, ArrowBehaviour arrow) pair in tasks[task.TargetSystem])
                    {
                        if (pair.console == __instance.Console)
                        {
                            tasks[task.TargetSystem].Remove(pair);
                            GameObject.Destroy(pair.arrow.gameObject);
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
                    foreach ((Console console, ArrowBehaviour arrow) pair in tasks[__instance.TargetSystem])
                    {
                        if (console == pair.console)
                        {
                            __result = true;
                        }
                    }
                }
                return false;
            }
            [HarmonyPatch("AppendTaskText")]
            [HarmonyPrefix]
            public static bool AppendTaskTextPrefix(DivertPowerTask __instance, [HarmonyArgument(0)] StringBuilder sb)
            {
                __instance.Arrow.gameObject.SetActive(false);
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
                if (task != null)
                {
                    task.arrowSuspended = true;
                    task.Arrow.gameObject.SetActive(false);
                    if (!tasks.Keys.Contains(task.TargetSystem))
                    {
                        List<(Console, ArrowBehaviour)> list = new List<(Console, ArrowBehaviour)>();
                        foreach (Console console in ShipStatus.Instance.AllConsoles)
                        {
                            if (!console.TaskTypes.Contains(TaskTypes.DivertPower) && Utils.IsDivertPowerTaskConsole(console))
                            {
                                ArrowBehaviour arrow = GameObject.Instantiate<ArrowBehaviour>(task.Arrow, task.Arrow.transform.parent);
                                arrow.target = console.transform.position;
                                arrow.gameObject.SetActive(false);
                                list.Add((console, arrow));
                            }
                        }
                        tasks.Add(task.TargetSystem, list);
                    }
                    else
                    {
                        tasks[task.TargetSystem].Clear();
                        foreach (Console console in ShipStatus.Instance.AllConsoles)
                        {
                            if (!console.TaskTypes.Contains(TaskTypes.DivertPower) && Utils.IsDivertPowerTaskConsole(console))
                            {
                                ArrowBehaviour arrow = GameObject.Instantiate<ArrowBehaviour>(task.Arrow, task.Arrow.transform.parent);
                                arrow.target = console.transform.position;
                                arrow.gameObject.SetActive(false);
                                tasks[task.TargetSystem].Add((console, arrow));
                            }
                        }
                    }
                }
            }
            [HarmonyPatch("NextStep")]
            [HarmonyPrefix]
            public static bool NextStepPrefix(NormalPlayerTask __instance)
            {
                DivertPowerTask task = __instance.TryCast<DivertPowerTask>();
                if (task != null)
                {
                    if (task.TaskStep == 0)
                    {
                        foreach ((Console console, ArrowBehaviour arrow) pair in tasks[task.TargetSystem])
                        {
                            pair.arrow.gameObject.SetActive(true);
                        }
                        return true;
                    }
                    return tasks[task.TargetSystem].Count == 0;
                }
                return true;
            }
        }
    }
}
