using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HardTasks.Skeld
{
    [HarmonyPatch(typeof(WireMinigame))]
    internal class WireTask
    {
        public static int wiresCount = 14;
        public static Sprite background = Utils.LoadSprite("HardTasks.Resources.WireTaskBackground", 100);
        [HarmonyPatch("Begin")]
        [HarmonyPrefix]
        public static void BeginPrefix(WireMinigame __instance)
        {
            WireMinigame prefab = Utils.GetMinigamePrefab(TaskTypes.FixWiring).Cast<WireMinigame>();
            __instance.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = background;
            WireMinigame.colors = new Color[wiresCount];
            __instance.ActualWires = new sbyte[wiresCount];
            __instance.Symbols = new Sprite[wiresCount];
            __instance.ExpectedWires = new sbyte[wiresCount];
            List<sbyte> ids = new List<sbyte>();
            for (int i = 2; i < __instance.transform.GetChildCount(); i++)
            {
                __instance.transform.GetChild(i).gameObject.SetActive(false);
            }
            __instance.LeftLights = new SpriteRenderer[] { };
            __instance.LeftNodes = new Wire[] { };
            __instance.RightLights = new SpriteRenderer[] { };
            __instance.RightNodes = new WireNode[] { };
            float num = 1.05f;
            for (int i = 0; i < wiresCount; i++)
            {
                WireMinigame.colors[i] = Color.HSVToRGB((float)i / (float)wiresCount, 1f, 1f);
                __instance.ActualWires[i] = -1;
                num -= 0.35f;
                Transform wire = Create(__instance, prefab, WireMinigame.colors[i]);
                wire.localPosition = new Vector3(0, num, 0);
                ids.Add((sbyte)i);
            }
            for (int i = 0; i < wiresCount; i++)
            {
                sbyte id = ids[new System.Random().Next(0, ids.Count - 1)];
                ids.Remove(id);
                __instance.ExpectedWires[i] = (sbyte)id;
            }
        }
        public static Transform Create(WireMinigame original, WireMinigame prefab, Color color)
        {
            Transform parent = new GameObject("Parent").transform;
            parent.SetParent(original.transform);
            original.LeftLights = original.LeftLights.Concat(new SpriteRenderer[] { GameObject.Instantiate<SpriteRenderer>(prefab.LeftLights[0], parent, true) }).ToArray();
            Wire wire = GameObject.Instantiate<Wire>(prefab.LeftNodes[0], parent, true);
            wire.ResetLine(Vector3.zero, true);
            wire.SetColor(color, null);
            wire.WireId = (sbyte)original.LeftNodes.Count;
            wire.hitbox.Cast<CircleCollider2D>().radius = 1.5f / (float)wiresCount + 0.1f;
            original.LeftNodes = original.LeftNodes.Concat(new Wire[] { wire }).ToArray();
            original.RightLights = original.RightLights.Concat(new SpriteRenderer[] { GameObject.Instantiate<SpriteRenderer>(prefab.RightLights[0], parent, true) }).ToArray();
            WireNode node = GameObject.Instantiate<WireNode>(prefab.RightNodes[0], parent, true);
            node.SetColor(color, null);
            node.WireId = (sbyte)original.RightNodes.Count;
            node.hitbox.Cast<CircleCollider2D>().radius = 1.5f / (float)wiresCount + 0.1f;
            original.RightNodes = original.RightNodes.Concat(new WireNode[] { node }).ToArray();
            int i = 0;
            return parent;
        }
    }
}
