using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq;
using UnityEngine.U2D;

namespace HardTasks.Skeld
{
    [HarmonyPatch(typeof(SweepMinigame), "Begin")]
    internal class SweepTask
    {
        public static Sprite background = Utils.LoadSprite("HardTasks.Resources.SweepTaskBackground", 100);
        public static void Postfix(SweepMinigame __instance)
        {
            __instance.SpinRate = 180;
            __instance.BackButton.transform.localPosition = new Vector3(-3.95f, 2.55f, -5);
            __instance.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = background;
            SweepMinigame Prefab = __instance.MyNormTask.GetMinigamePrefab().Cast<SweepMinigame>();
            __instance.ControllerSelectable.Clear();
            __instance.Gauges = new HorizontalGauge[] { };
            __instance.Lights = new SpriteRenderer[] { };
            __instance.Shadows = new SpriteRenderer[] { };
            __instance.Spinners = new SpriteRenderer[] { };
            for (int i = 2; i < __instance.transform.GetChildCount(); i++)
            {
                __instance.transform.GetChild(i).gameObject.SetActive(false);
            }
            Create(__instance, Prefab, Color.yellow).localPosition = new Vector3(-1.75f, -0.2f, 0);
            Create(__instance, Prefab, Palette.Blue).localPosition = new Vector3(0.75f, -0.2f, 0);
            Create(__instance, Prefab, Color.green).localPosition = new Vector3(3.25f, -0.2f, 0);
            Create(__instance, Prefab, Palette.Purple).localPosition = new Vector3(-1.75f, -2.06f, 0);
            Create(__instance, Prefab, Palette.ImpostorRed).localPosition = new Vector3(0.75f, -2.06f, 0);
            Create(__instance, Prefab, Palette.Orange).localPosition = new Vector3(3.25f, -2.06f, 0);
        }
        public static Transform Create(SweepMinigame original, SweepMinigame prefab, Color color)
        {
            Transform parent = new GameObject("Parent").transform;
            parent.SetParent(original.transform);
            ButtonBehavior button = GameObject.Instantiate<ButtonBehavior>(prefab.ControllerSelectable[0].Cast<ButtonBehavior>(), parent, true);
            button.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
            int c = original.ControllerSelectable.Count;
            button.OnClick.AddListener(new Action(delegate
            {
                original.HitButton(c);
            }));
            button.transform.localPosition = new Vector3(-1.38f, 0.55f, 0);
            original.ControllerSelectable.Add(button);
            HorizontalGauge gauge = GameObject.Instantiate<HorizontalGauge>(prefab.Gauges[0], parent, true);
            gauge.transform.localScale = new Vector3(0.35f, 0.6f, 0.6f);
            gauge.transform.localPosition = new Vector3(-0.49f, 1.81f, 0.2f);
            gauge.transform.GetChild(1).GetComponent<SpriteRenderer>().color = color;
            original.Gauges = original.Gauges.Concat(new HorizontalGauge[] { gauge }).ToArray();
            original.Lights = original.Lights.Concat(new SpriteRenderer[] { GameObject.Instantiate<SpriteRenderer>(prefab.Lights[0], parent, true) }).ToArray();
            SpriteRenderer shadow = GameObject.Instantiate<SpriteRenderer>(prefab.Shadows[0], parent, true);
            shadow.color = new Color(color.r * 0.5f, color.g * 0.5f, color.b * 0.5f, 1);
            original.Shadows = original.Shadows.Concat(new SpriteRenderer[] { shadow }).ToArray();
            SpriteRenderer spinner = GameObject.Instantiate<SpriteRenderer>(prefab.Spinners[0], parent, true);
            spinner.color = color;
            original.Spinners = original.Spinners.Concat(new SpriteRenderer[] { spinner }).ToArray();
            parent.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            return parent;
        }
    }
}
