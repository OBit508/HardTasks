using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HardTasks.Skeld
{
    [HarmonyPatch(typeof(LeafMinigame))]
    internal class LeafTask
    {
        public static Collider2D Collider;
        public static float timer;
        public static AnimationClip[] active;
        public static AnimationClip[] inactive;
        public static Sprite sprite = Utils.LoadSprite("HardTasks.Resources.LeafButton", 200);
        [HarmonyPatch("Begin")]
        [HarmonyPostfix]
        public static void BeginPostfix(LeafMinigame __instance)
        {
            __instance.MyNormTask.MaxStep = 16;
            timer = 0;
            Collider = new GameObject("Collider").AddComponent<BoxCollider2D>();
            Collider.gameObject.layer = __instance.transform.GetChild(2).gameObject.layer;
            Collider.transform.SetParent(__instance.transform);
            Collider.transform.localScale = new Vector3(1, 2, 1);
            Collider.transform.localPosition = new Vector3(-2, 0, 0);
            active = __instance.Active;
            inactive = __instance.Inactive;
            ButtonBehavior button = GameObject.Instantiate<ButtonBehavior>(__instance.transform.GetChild(1).GetComponent<ButtonBehavior>(), __instance.transform);
            button.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            button.transform.localPosition = new Vector3(-1.985f, 1.84f, -5);
            button.spriteRenderer.sprite = sprite;
            button.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
            button.OnClick.AddListener(new Action(delegate
            {
                if (timer <= 0)
                {
                    timer = 1.2f;
                    __instance.Arrows[0].Play(active[0]);
                    __instance.Arrows[1].Play(active[1]);
                }
            }));
            foreach (Collider2D leaf in __instance.Leaves)
            {
                GameObject.Destroy(leaf.gameObject);
            }
            __instance.Leaves = new Collider2D[] { };
            for (int i = 0; i < 16 - __instance.MyNormTask.TaskStep; i++)
            {
                LeafBehaviour leafBehaviour = GameObject.Instantiate<LeafBehaviour>(__instance.LeafPrefab);
                leafBehaviour.transform.SetParent(__instance.transform);
                leafBehaviour.Parent = __instance;
                Vector2 vector = __instance.ValidArea.Next();
                leafBehaviour.transform.localPosition = new Vector3(vector.x, vector.y, -1f);
                __instance.Leaves = __instance.Leaves.Concat(new Collider2D[] { leafBehaviour.GetComponent<Collider2D>() }).ToArray();
            }
        }
        [HarmonyPatch("FixedUpdate")]
        [HarmonyPostfix]
        public static void FixedUpdatePostfix(LeafMinigame __instance)
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
                __instance.Inactive = active;
                __instance.Active = active;
                if (timer <= 0)
                {
                    __instance.Arrows[0].Play(inactive[0]);
                    __instance.Arrows[1].Play(inactive[1]);
                }
            }
            else
            {
                __instance.Inactive = inactive;
                __instance.Active = inactive;
            }
            Collider.gameObject.SetActive(timer <= 0);
        }
    }
}
