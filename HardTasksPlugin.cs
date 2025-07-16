using System;
using AmongUs.GameOptions;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using UnityEngine;
using HarmonyLib;
using Hazel;
using Il2CppInterop.Runtime.Injection;
using HardTasks.CustomTask;
using InnerNet;


namespace ImpossibleTasksV3
{
	[BepInProcess("Among Us.exe")]
	[BepInPlugin(ModId, ModName, ModVersion)]
	public class HardTasksPlugin : BasePlugin
	{
        public const string ModName = "HardTasks";
        public const string Owner = "rafael";
        public const string ModDescription = "";
        public const string ModId = "com." + Owner + "." + ModName;
        public const string ModVersion = "1.0.0";
        public Harmony Harmony { get; } = new Harmony(ModId);
		public override void Load()
		{
			ClassInjector.RegisterTypeInIl2Cpp<CustomUploadTask>();
            Harmony.PatchAll();
		}
	}
}
