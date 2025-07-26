using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace HardTasks.Skeld
{
    [HarmonyPatch(typeof(SimonSaysGame), "Begin")]
    internal class SimonSaysTask
    {
        public static List<Sprite> buttons;
        public static List<Sprite> Buttons
        {
            get
            {
                if (buttons == null)
                {
                    buttons = new List<Sprite>();
                    for (int i = 0; i <= 8; i++)
                    {
                        buttons.Add(Utils.LoadSprite("HardTasks.Resources.SimonSays." + i.ToString(), 180));
                    }
                }
                return buttons;
            }
        }
        public static void Postfix(SimonSaysGame __instance)
        {
            for (int i = 0; i < __instance.Buttons.Count; i++)
            {
                __instance.Buttons[i].sprite = Buttons[i];
            }
            for (int i = 0; i < 200; i++)
            {
                SpriteRenderer button1 = __instance.Buttons[new System.Random().Next(0, __instance.Buttons.Count - 1)];
                SpriteRenderer button2 = __instance.Buttons[new System.Random().Next(0, __instance.Buttons.Count - 1)];
                Vector3 vec = button1.transform.localPosition;
                button1.transform.localPosition = button2.transform.localPosition;
                button2.transform.localPosition = vec;
            }
        }
    }
}
