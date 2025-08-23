using HardTasks.CustomTask;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using Rewired;
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
        [HarmonyPatch(typeof(DivertPowerMinigame))]
        public class DivertPowerMinigamePatch
        {
            public static List<int> ids = new List<int>();
            [HarmonyPatch("Begin")]
            public static void Postfix(DivertPowerMinigame __instance)
            {
                ids.Clear();
                for (int i = 0; i < __instance.SliderOrder.Count; i++)
                {
                    ids.Add(i);
                }
                int id = ids[new System.Random().Next(0, ids.Count - 1)];
                ids.Remove(id);
                __instance.sliderId = id;
                __instance.glyphDisplay.UpdateGlyphDisplay();
                __instance.glyphDisplay.transform.SetParent(__instance.Sliders[__instance.sliderId].transform, false);
            }
            [HarmonyPatch("FixedUpdate")]
            public static bool Prefix(DivertPowerMinigame __instance)
            {
                __instance.myController.Update();
                if (__instance.sliderId >= 0)
                {
                    float axisRaw = ReInput.players.GetPlayer(0).GetAxisRaw(__instance.inputJoystick);
                    Collider2D collider2D = __instance.Sliders[__instance.sliderId];
                    Vector2 vector = collider2D.transform.localPosition;
                    if (Mathf.Abs(axisRaw) > 0.01f)
                    {
                        __instance.prevHadInput = true;
                        vector.y = __instance.SliderY.Clamp(vector.y + axisRaw * Time.deltaTime * 2f);
                        collider2D.transform.localPosition = vector;
                    }
                    else
                    {
                        if (__instance.prevHadInput && __instance.SliderY.max - vector.y < 0.05f)
                        {
                            if (ids.Count == 0)
                            {
                                __instance.MyNormTask.NextStep();
                                __instance.StartCoroutine(__instance.CoStartClose());
                            }
                            else
                            {
                                collider2D.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);
                                int id = ids[new System.Random().Next(0, ids.Count - 1)];
                                ids.Remove(id);
                                __instance.sliderId = id;
                                __instance.glyphDisplay.UpdateGlyphDisplay();
                                __instance.glyphDisplay.transform.SetParent(__instance.Sliders[__instance.sliderId].transform, false);
                                return false;
                            }
                        }
                        __instance.prevHadInput = false;
                    }
                }
                float num = 0f;
                for (int i = 0; i < __instance.Sliders.Length; i++)
                {
                    num += __instance.SliderY.ReverseLerp(__instance.Sliders[i].transform.localPosition.y) / (float)__instance.Sliders.Length;
                }
                for (int j = 0; j < __instance.Sliders.Length; j++)
                {
                    float num2 = __instance.SliderY.ReverseLerp(__instance.Sliders[j].transform.localPosition.y);
                    float num3 = num2 / num / 1.6f;
                    __instance.Gauges[j].value = num3 + (Mathf.PerlinNoise((float)j, Time.time * 51f) - 0.5f) * 0.04f;
                    Color color = Color.Lerp(Color.gray, Color.yellow, num2 * num2);
                    color.a = (float)((num3 < 0.1f) ? 0 : 1);
                    Vector2 textureOffset = __instance.Wires[j].material.GetTextureOffset("_MainTex");
                    textureOffset.x -= Time.fixedDeltaTime * 3f * Mathf.Lerp(0.1f, 2f, num3);
                    __instance.Wires[j].material.SetTextureOffset("_MainTex", textureOffset);
                    __instance.Wires[j].material.SetColor("_Color", color);
                }
                if (__instance.sliderId < 0)
                {
                    return false;
                }
                Collider2D collider2D2 = __instance.Sliders[__instance.sliderId];
                Vector2 vector2 = collider2D2.transform.localPosition;
                DragState dragState = __instance.myController.CheckDrag(collider2D2);
                if (dragState == DragState.Dragging)
                {
                    Vector2 vector3 = (Vector3)__instance.myController.DragPosition - collider2D2.transform.parent.position;
                    vector3.y = __instance.SliderY.Clamp(vector3.y);
                    vector2.y = vector3.y;
                    collider2D2.transform.localPosition = vector2;
                    return false;
                }
                if (dragState != DragState.Released)
                {
                    return false;
                }
                if (__instance.SliderY.max - vector2.y < 0.05f)
                {
                    __instance.MyNormTask.NextStep();
                    __instance.StartCoroutine(__instance.CoStartClose(0.75f));
                    __instance.sliderId = -1;
                    collider2D2.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);
                }
                return false;
            }
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
