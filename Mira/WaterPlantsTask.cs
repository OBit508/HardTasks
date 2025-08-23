using BepInEx.Unity.IL2CPP.Utils.Collections;
using Discord;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HardTasks.Mira
{
    [HarmonyPatch(typeof(WaterPlantsGame))]
    internal class WaterPlantsTask
    {
        [HarmonyPatch("WaterPlant")]
        [HarmonyPrefix]
        public static bool WaterPlantPrefix(WaterPlantsGame __instance, [HarmonyArgument(0)] int num)
        {
            if (__instance.Watered(num))
            {
                return false;
            }
            if (Enumerable.All<int>(Enumerable.Range(0, 4), new Func<int, bool>(__instance.Watered)))
            {
                __instance.MyNormTask.NextStep();
                __instance.StartCoroutine(__instance.CoStartClose(0.75f));
            }
            if (Constants.ShouldPlaySfx())
            {
                SoundManager.Instance.PlaySound(__instance.WaterPlantSound, false, 1f, null);
            }
            __instance.StartCoroutine(CoGrowPlant(__instance, num).WrapToIl2Cpp());
            if (global::Controller.currentTouchType == global::Controller.TouchType.Joystick)
            {
                __instance.waterParticles.Play();
            }
            return false;
        }
        public static System.Collections.IEnumerator CoGrowPlant(WaterPlantsGame waterPlants, [HarmonyArgument(0)] int num)
        {
            SpriteRenderer plant = waterPlants.Plants[num];
            if (Constants.ShouldPlaySfx())
            {
                SoundManager.Instance.PlaySound(waterPlants.PlantGrowSounds[new System.Random().Next(0, waterPlants.PlantGrowSounds.Count)], false, 1f, null).pitch = FloatRange.Next(0.9f, 1.1f);
            }
            for (float timer = 0f; timer < 5f; timer += Time.deltaTime)
            {
                float num2 = timer / 2f;
                plant.material.SetFloat("_Desat", (1f - num2) * 0.8f);
                plant.transform.localScale = new Vector3(0.8f, Mathf.Lerp(0.8f, 1.1f, num2), 1f);
                yield return null;
            }
            plant.material.SetFloat("_Desat", 0f);
            if (Constants.ShouldPlaySfx())
            {
                SoundManager.Instance.PlaySound(waterPlants.PlantFinishedSounds[new System.Random().Next(0, waterPlants.PlantFinishedSounds.Count)], false, 1f, null).pitch = FloatRange.Next(0.9f, 1.1f);
            }
            for (float timer = 0f; timer < 0.1f; timer += Time.deltaTime)
            {
                float num3 = timer / 0.1f;
                plant.transform.localScale = new Vector3(Mathf.Lerp(0.8f, 1.1f, num3), Mathf.Lerp(1.1f, 0.95f, num3), 1f);
                yield return null;
            }
            for (float timer = 0f; timer < 0.1f; timer += Time.deltaTime)
            {
                float num4 = timer / 0.1f;
                plant.transform.localScale = new Vector3(Mathf.Lerp(1.1f, 1f, num4), Mathf.Lerp(0.95f, 1f, num4), 1f);
                waterPlants.Watered(num, true);
                yield return null;
            }
            yield break;
        }
        [HarmonyPatch("Begin")]
        [HarmonyPostfix]
        public static void BeginPostfix(WaterPlantsGame __instance)
        {
            __instance.WaterCan.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.2f);
        }
    }
}
