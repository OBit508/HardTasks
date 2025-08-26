using BepInEx.Unity.IL2CPP.Utils;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Il2CppSystem.Net.WebClient;

namespace HardTasks.Mira
{
    [HarmonyPatch(typeof(ProcessDataMinigame))]
    internal class ProcessDataTask
    {
        public static List<GenericPopup> PopUps = new List<GenericPopup>();
        public static string[] strings = new string[]
        {
            "ERROR",
            ":)",
            "Hi",
            "Free minecraft\ndownload updated\n2025!!!!!",
            "Brazil!!!",
            ":/",
            "Close me",
            "...",
            "abcdefghijklmnopqrstuvwxyz",
            "amogus",
            "Hard",
            "You are sus",
            "SUS!!!",
            "I think you are the imposter"
        };

        public static void Create_Pop_Up(Transform transform)
        {
            GenericPopup popup = GameObject.Instantiate<GenericPopup>(DiscordManager.Instance.discordPopup, transform);
            PopUps.Add(popup);
            popup.transform.GetChild(0).gameObject.SetActive(false);
            PassiveButton close = popup.transform.GetChild(2).GetComponent<PassiveButton>();
            close.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
            close.OnClick.AddListener(new Action(delegate
            {
                PopUps.Remove(popup);
                GameObject.Destroy(popup.gameObject);
            }));
            popup.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
            popup.transform.localPosition = new Vector3(UnityEngine.Random.RandomRange(-3, 3), UnityEngine.Random.RandomRange(-1.5f, 1.5f), -735);
            popup.Show(strings[new System.Random().Next(0, strings.Count() - 1)]);
        }
        [HarmonyPatch("Begin")]
        [HarmonyPostfix]
        public static void BeginPostfix()
        {
            PopUps.Clear();
        }
        [HarmonyPatch("StartStopFill")]
        [HarmonyPrefix]
        public static bool StartStopFillPrefix(ProcessDataMinigame __instance)
        {
            __instance.StartButton.enabled = false;
            __instance.StartCoroutine(CoDoAnimation(__instance));
            __instance.StartCoroutine(CoDoAnimation(__instance));
            return false;
        }
        public static System.Collections.IEnumerator CoStartPopUpLoop(ProcessDataMinigame processData)
        {
            while (true)
            {
                yield return new WaitForSeconds(1);
                Create_Pop_Up(processData.transform);
            }
        }
        public static System.Collections.IEnumerator CoDoAnimation(ProcessDataMinigame processData)
        {
            processData.LeftFolder.Play(processData.OpenFolderClip, 1f);
            yield return processData.Transition();
            processData.StartCoroutine(DoText(processData));
            float timer = 0;
            while (timer < processData.Duration)
            {
                if (PopUps.Count <= 0)
                {
                    float num = timer / processData.Duration;
                    processData.Gauge.Value = num;
                    processData.PercentText.text = Mathf.RoundToInt(num * 100f).ToString() + "%";
                    processData.scenery.SetParallax(processData.SceneRange.Lerp(num));
                    yield return null;
                    timer += Time.deltaTime;
                }
            }
            processData.running = false;
            processData.EstimatedText.text = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.WeatherComplete);
            processData.RightFolder.Play(processData.CloseFolderClip, 1f);
            processData.MyNormTask.NextStep();
            yield return processData.CoStartClose(0.75f);
            yield break;
        }
        public static System.Collections.IEnumerator DoText(ProcessDataMinigame processData)
        {
            StringBuilder txt = new StringBuilder(DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.Processing) + ": ");
            int len = txt.Length;
            while (processData.running)
            {
                if (PopUps.Count <= 0)
                {
                    txt.Append(processData.DocTopics[new System.Random().Next(0, processData.DocTopics.Count - 1)]);
                    txt.Append("_");
                    txt.Append(processData.DocTypes[new System.Random().Next(0, processData.DocTypes.Count - 1)]);
                    txt.Append(processData.DocExtensions[new System.Random().Next(0, processData.DocExtensions.Count - 1)]);
                    processData.EstimatedText.text = txt.ToString();
                    yield return Effects.Wait(FloatRange.Next(0.025f, 0.15f));
                    txt.Length = len;
                }
            }
            yield break;
        }
    }
}
