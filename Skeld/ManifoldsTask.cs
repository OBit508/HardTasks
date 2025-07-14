using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HardTasks.Skeld
{
    [HarmonyPatch(typeof(UnlockManifoldsMinigame), "Begin")]
    internal class ManifoldsTask
    {
        public static List<Sprite> sprites;
        public static void Postfix(UnlockManifoldsMinigame __instance)
        {
            if (sprites == null)
            {
                sprites = new List<Sprite>();
                for (int i = 0; i <= 10; i++)
                {
                    sprites.Add(Utils.LoadSprite("HardTasks.Resources.Manifolds." + i.ToString(), 100));
                }
            }
            __instance.transform.GetChild(0).transform.localScale = new Vector3(1.4f, 1.4f, 1.4f);
            __instance.transform.GetChild(1).localPosition = new Vector3(-3.88f, 1.392f, -5);
            __instance.transform.GetChild(1).localScale = new Vector3(0.95f, 0.95f, 0.95f);
            __instance.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
            for (int i = 0; i <= 10; i++)
            {
                CreateButton(__instance);
            }
            Action reset = new Action(delegate
            {
                for (int i = 0; i < 200; i++)
                {
                    SpriteRenderer button1 = __instance.Buttons[new System.Random().Next(0, __instance.Buttons.Count - 1)];
                    SpriteRenderer button2 = __instance.Buttons[new System.Random().Next(0, __instance.Buttons.Count - 1)];
                    Vector3 vec = button1.transform.localPosition;
                    button1.transform.localPosition = button2.transform.localPosition;
                    button2.transform.localPosition = vec;
                }
            });
            for (int i = 0; i < __instance.Buttons.Count; i++)
            {
                float num = -2.55f;
                float num2 = 0.77f;
                int num3 = i % 7;
                int num4 = i / 7;
                float num5 = num + 0.85f * (float)num3;
                float num6 = num2 - 0.77f * (float)num4;
                SpriteRenderer button = __instance.Buttons[i];
                button.transform.localPosition = new Vector3(num5, num6);
                button.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
                button.GetComponent<PassiveButton>().OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
                button.GetComponent<PassiveButton>().OnClick.AddListener(new Action(delegate
                {
                    if (i == __instance.buttonCounter)
                    {
                        __instance.buttonCounter++;
                        button.color = Color.green;
                    }
                    else
                    {
                        __instance.buttonCounter = 0;
                        __instance.StartCoroutine(__instance.ResetAll());
                    }
                    SoundManager.Instance.PlaySound(__instance.PressButtonSound, false);
                    reset();
                }));
            }
            reset();
        }
        public static void CreateButton(UnlockManifoldsMinigame minigame)
        {
            SpriteRenderer rend = GameObject.Instantiate<SpriteRenderer>(minigame.Buttons[0], minigame.transform);
            rend.sprite = sprites[minigame.Buttons.Count - 10];
            minigame.Buttons = minigame.Buttons.Concat(new SpriteRenderer[] { rend }).ToArray();
            minigame.ControllerSelectable.Add(rend.GetComponent<PassiveButton>());
        }
    }
}
