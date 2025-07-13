using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HardTasks.Skeld
{
    
    internal class WeaponsTask
    {
        [HarmonyPatch(typeof(WeaponsMinigame))]
        public class WeaponsMinigamePatch
        {
            public static Sprite sprite = Utils.LoadSprite("HardTasks.Resources.ReloadButton", 200);
            public static bool CanShot;
            [HarmonyPatch("Begin")]
            [HarmonyPostfix]
            public static void BeginPostfix(WeaponsMinigame __instance)
            {
                CanShot = true;
                __instance.transform.GetChild(5).gameObject.SetActive(false);
                ButtonBehavior button = GameObject.Instantiate<ButtonBehavior>(__instance.transform.GetChild(1).GetComponent<ButtonBehavior>(), __instance.transform);
                button.transform.localScale = new Vector3(1, 1, 0.8f);
                button.transform.localPosition = new Vector3(2.893f, 0, 0);
                button.spriteRenderer.sprite = sprite;
                button.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
                button.OnClick.AddListener(new Action(delegate
                {
                    if (!CanShot)
                    {
                        CanShot = true;
                    }
                }));
            }
            [HarmonyPatch("BreakApart")]
            [HarmonyPrefix]
            public static bool BreakApartPrefix()
            {
                if (CanShot)
                {
                    CanShot = false;
                    return true;
                }
                return false;
            }
            [HarmonyPatch("BreakApart")]
            [HarmonyPostfix]
            public static void BreakApartPostfix(WeaponsMinigame __instance)
            {
                __instance.ScoreText.text = TranslationController.Instance.GetString(StringNames.AstDestroyed).Replace("{0}", __instance.MyNormTask.TaskStep.ToString()) + "\nNeed to Reload: " + (!CanShot).ToString();
            }
        }
        [HarmonyPatch(typeof(NormalPlayerTask), "Initialize")]
        public class NormalPlayerTaskPatch
        {
            public static void Postfix(NormalPlayerTask __instance)
            {
                if (__instance.TaskType == TaskTypes.ClearAsteroids)
                {
                    __instance.MaxStep = 30;
                }
            }
        }
    }
}
