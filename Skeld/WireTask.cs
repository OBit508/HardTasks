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

        [HarmonyPatch("Begin")]
        [HarmonyPostfix]
        public static void BeginPostfix(WireMinigame __instance)
        {
            WireMinigame prefab = __instance.MyNormTask.GetMinigamePrefab().Cast<WireMinigame>();
            WireMinigame.colors = new Color[] { Color.gray, Color.gray, Color.gray, Color.gray, Color.gray, Color.gray, Color.gray, Color.gray, Color.gray };
            __instance.ActualWires = new sbyte[] { -1, -1, -1, -1, -1, -1, -1, -1, -1 };
            __instance.Symbols = new Sprite[] { null, null, null, null, null, null, null, null, null };
            __instance.ExpectedWires = new sbyte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            for (int i = 2; i < __instance.transform.GetChildCount(); i++)
            {
                __instance.transform.GetChild(i).gameObject.SetActive(false);
            }
            __instance.LeftLights = new SpriteRenderer[] { };
            __instance.LeftNodes = new Wire[] { };
            __instance.RightLights = new SpriteRenderer[] { };
            __instance.RightNodes = new WireNode[] { };
            Create(__instance, prefab).localPosition = new Vector3(0, 0.5f, 0);
            Create(__instance, prefab).localPosition = Vector3.zero;
            Create(__instance, prefab).localPosition = new Vector3(0, -0.5f, 0);
            Create(__instance, prefab).localPosition = new Vector3(0, -1, 0);
            Create(__instance, prefab).localPosition = new Vector3(0, -1.5f, 0);
            Create(__instance, prefab).localPosition = new Vector3(0, -2, 0);
            Create(__instance, prefab).localPosition = new Vector3(0, -2.5f, 0);
            Create(__instance, prefab).localPosition = new Vector3(0, -3, 0);
            Create(__instance, prefab).localPosition = new Vector3(0, -3.5f, 0);
            List<sbyte> ids = new List<sbyte>() { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
            for (int i = 0; i < 9; i++)
            {
                sbyte id = ids[new System.Random().Next(0, ids.Count - 1)];
                ids.Remove(id);
                __instance.ExpectedWires[i] = (sbyte)id;
            }
        }
        [HarmonyPatch("CheckRightSide")]
        [HarmonyPrefix]
        public static bool CheckRightSidePrefix(WireMinigame __instance, ref WireNode __result, Vector2 pos)
        {
            Collider2D amTouching = __instance.myController.amTouching;
            int wireId = (int)amTouching.transform.parent.GetComponent<Wire>().WireId;
            for (int i = 0; i < __instance.RightNodes.Length; i++)
            {
                WireNode wireNode = __instance.RightNodes[i];
                if (wireNode.hitbox.OverlapPoint(pos) && __instance.ExpectedWires[wireId] == wireNode.WireId)
                {
                    __result = wireNode;
                }
            }
            if (!__result)
            {
                __result = null;
            }
            return false;
        }
        public static Transform Create(WireMinigame original, WireMinigame prefab)
        {
            Transform parent = new GameObject("Parent").transform;
            parent.SetParent(original.transform);
            original.LeftLights = original.LeftLights.Concat(new SpriteRenderer[] { GameObject.Instantiate<SpriteRenderer>(prefab.LeftLights[0], parent, true) }).ToArray();
            Wire wire = GameObject.Instantiate<Wire>(prefab.LeftNodes[0], parent, true);
            wire.ResetLine(Vector3.zero, true);
            wire.SetColor(Color.gray, null);
            wire.WireId = (sbyte)original.LeftNodes.Count;
            original.LeftNodes = original.LeftNodes.Concat(new Wire[] { wire }).ToArray();
            original.RightLights = original.RightLights.Concat(new SpriteRenderer[] { GameObject.Instantiate<SpriteRenderer>(prefab.RightLights[0], parent, true) }).ToArray();
            WireNode node = GameObject.Instantiate<WireNode>(prefab.RightNodes[0], parent, true);
            node.SetColor(Color.gray, null);
            node.WireId = (sbyte)original.RightNodes.Count;
            original.RightNodes = original.RightNodes.Concat(new WireNode[] { node }).ToArray();
            return parent;
        }
    }
}
