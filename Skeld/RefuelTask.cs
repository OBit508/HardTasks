﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HardTasks.CustomTask;
using HarmonyLib;
using Hazel;
using UnityEngine;

namespace HardTasks.Skeld
{
    [HarmonyPatch(typeof(RefuelStage))]
    internal class RefuelTask
    {
        public static List<(SpriteRenderer greenLight, SpriteRenderer redLight, ChangeableValue<float> timer)> all = new List<(SpriteRenderer greenLight, SpriteRenderer redLight, ChangeableValue<float> timer)>();
        [HarmonyPatch("Begin")]
        [HarmonyPostfix]
        public static void BeginPostfix(RefuelStage __instance)
        {
            all.Clear();
            Transform background = __instance.transform.GetChild(1);
            Create(background, new Vector3(0, 0, 0), new Vector3(1, 1, 1));
            Create(background, new Vector3(0, 0, 0), new Vector3(-1, 1, 1));
            Create(background, new Vector3(0, 3, 0), new Vector3(1, 1, 1));
            Create(background, new Vector3(0, 3, 0), new Vector3(-1, 1, 1));
            __instance.timer = 0;
            __instance.destGauge.value = 0;
        }
        [HarmonyPatch("FixedUpdate")]
        [HarmonyPrefix]
        public static bool FixedUpdatePrefix(RefuelStage __instance)
        {
            if (__instance.multistageMinigameParent != null)
            {
                __instance.multistageMinigameParent.transform.GetChild(3).gameObject.SetActive(false);
            }
            if (__instance.timer < 6)
            {
                int activeCount = 0;
                foreach ((SpriteRenderer greenLight, SpriteRenderer redLight, ChangeableValue<float> timer) pair in all)
                {
                    pair.timer.Value -= Time.deltaTime;
                    pair.greenLight.color = pair.timer.Value > 0 ? Color.green : new Color(0, 0.3529f, 0, 1);
                    pair.redLight.color = pair.timer.Value <= 0 ? Color.red : new Color(0.3529f, 0, 0, 1);
                    activeCount += pair.timer.Value > 0 ? 1 : 0;
                }
                if (activeCount >= all.Count)
                {
                    __instance.timer += 0.1f;
                }
                else if (__instance.timer - 0.1f > 0)
                {
                    __instance.timer -= 0.1f;
                }
                __instance.destGauge.value = __instance.timer / 6;
                if (__instance.timer == 6)
                {
                    __instance.MyNormTask.NextStep();
                    __instance.StartCoroutine(__instance.CoStartClose());
                }
            }
            return false;
        }
        public static void Create(Transform prefab, Vector3 vec, Vector3 scale)
        {
            Transform background = GameObject.Instantiate<Transform>(prefab, prefab.transform.parent);
            GameObject.Destroy(background.GetComponent<SpriteRenderer>());
            background.transform.localPosition = vec;
            background.transform.localScale = scale;
            ChangeableValue<float> timer = new ChangeableValue<float>(0);
            all.Add((background.GetChild(1).GetChild(2).GetComponent<SpriteRenderer>(), background.GetChild(1).GetChild(1).GetComponent<SpriteRenderer>(), timer));
            background.GetChild(1).GetChild(0).GetComponent<ButtonBehavior>().OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
            background.GetChild(1).GetChild(0).GetComponent<ButtonBehavior>().OnClick.AddListener(new Action(delegate
            {
                timer.Value = 2;
            }));
        }
    }
}
