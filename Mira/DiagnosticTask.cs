using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HardTasks.Mira
{
    internal class DiagnosticTask
    {
        public static bool HasError;
        [HarmonyPatch(typeof(DiagnosticGame))]
        public static class DiagnosticGamePatch 
        {
            [HarmonyPatch("Begin")]
            [HarmonyPostfix]
            public static void BeginPostfix(DiagnosticGame __instance)
            {
                if (__instance.MyNormTask.Data == null || __instance.MyNormTask.Data.Count <= 0)
                {
                    __instance.MyNormTask.Data = new byte[] { 0, 0, 0, 0 };
                }
                for (int i = 0; i < __instance.Targets.Count; i++)
                {
                    SpriteRenderer target = __instance.Targets[i];
                    if (__instance.MyNormTask.Data[i] == 0)
                    {
                        target.gameObject.SetActive(false);
                        target.color = Color.red;
                    }
                    else
                    {
                        target.color = __instance.goodBarColor;
                    }
                    target.GetComponent<PassiveButton>().OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
                    target.GetComponent<PassiveButton>().OnClick.AddListener(new Action(delegate
                    {
                        target.color = __instance.goodBarColor;
                        __instance.MyNormTask.Data[i] = 1;
                        HasError = false;
                    }));
                }
            }
            [HarmonyPatch("Update")]
            [HarmonyPrefix]
            public static bool UpdatePrefix(DiagnosticGame __instance)
            {
                void Update()
                {
                    if (__instance.MyNormTask.TaskTimer < 70)
                    {
                        __instance.Targets[3].gameObject.SetActive(true);
                    }
                    if (__instance.MyNormTask.TaskTimer < 45)
                    {
                        __instance.Targets[2].gameObject.SetActive(true);
                    }
                    if (__instance.MyNormTask.TaskTimer < 30)
                    {
                        __instance.Targets[0].gameObject.SetActive(true);
                    }
                    if (__instance.MyNormTask.TaskTimer < 20)
                    {
                        __instance.Targets[1].gameObject.SetActive(true);
                    }
                    bool flag = __instance.MyNormTask.TimerStarted == NormalPlayerTask.TimerState.Finished;
                    foreach (byte b in __instance.MyNormTask.Data)
                    {
                        if (b == 0)
                        {
                            flag = false;
                        }
                    }
                    if (flag)
                    {
                        __instance.MyNormTask.NextStep();
                        __instance.StartCoroutine(__instance.CoStartClose());
                    }
                }
                switch (__instance.MyNormTask.TimerStarted)
                {
                    case NormalPlayerTask.TimerState.NotStarted:
                        __instance.Gauge.gameObject.SetActive(false);
                        return false;
                    case NormalPlayerTask.TimerState.Started:
                        {
                            Update();
                            __instance.Gauge.gameObject.SetActive(true);
                            __instance.Gauge.MaxValue = __instance.TimePerStep;
                            __instance.Gauge.value = __instance.MyNormTask.TaskTimer;
                            int num = (int)(100f * __instance.MyNormTask.TaskTimer / __instance.TimePerStep);
                            if (num != __instance.lastPercent && Constants.ShouldPlaySfx())
                            {
                                __instance.lastPercent = num;
                                SoundManager.Instance.PlaySound(__instance.TickSound, false, 0.8f, null);
                            }
                            __instance.Text.text = num.ToString() + "%";
                            return false;
                        }
                    case NormalPlayerTask.TimerState.Finished:
                        Update();
                        __instance.Gauge.gameObject.SetActive(true);
                        __instance.Gauge.MaxValue = 1f;
                        __instance.Gauge.value = 1f;
                        return false;
                    default:
                        return false;
                }
            }
            [HarmonyPatch(typeof(NormalPlayerTask), "FixedUpdate")]
            public static class NormalPlayerTaskPatch
            {
                public static bool Prefix(NormalPlayerTask __instance)
                {
                    if (__instance.TaskType == TaskTypes.RunDiagnostics && __instance.TimerStarted != NormalPlayerTask.TimerState.NotStarted)
                    {
                        if (__instance.Data == null || __instance.Data.Count <= 0)
                        {
                            __instance.Data = new byte[] { 0, 0, 0, 0 };
                        }
                        if (__instance.TaskTimer < 70 && __instance.Data[3] == 0)
                        {
                            HasError = true;
                        }
                        if (__instance.TaskTimer < 45 && __instance.Data[2] == 0)
                        {
                            HasError = true;
                        }
                        if (__instance.TaskTimer < 30 && __instance.Data[0] == 0)
                        {
                            HasError = true;
                        }
                        if (__instance.TaskTimer < 20 && __instance.Data[1] == 0)
                        {
                            HasError = true;
                        }
                        return !HasError;
                    }
                    return true;
                }
            }
        }
    }
}
