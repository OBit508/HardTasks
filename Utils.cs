using HardTasks.Skeld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HardTasks
{
    internal class Utils
    {
        public static Sprite LoadSprite(string resource, float PixelPerUnit)
        {
            resource = resource + ".png";
            System.IO.Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource);
            Texture2D texture2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
            manifestResourceStream.CopyTo(memoryStream);
            texture2D.LoadImage(memoryStream.ToArray());
            Sprite sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f), PixelPerUnit);
            sprite.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            return sprite;
        }
        public static Minigame GetMinigamePrefab(TaskTypes type)
        {
            foreach (PlayerTask task in ShipStatus.Instance.GetAllTasks())
            {
                if (task.TaskType == type)
                {
                    return task.GetMinigamePrefab();
                }
            }
            return null;
        }
        public static DivertPowerTask GetTaskByTarget(SystemTypes type)
        {
            foreach (PlayerTask task in PlayerControl.LocalPlayer.myTasks)
            {
                DivertPowerTask divertPowerTask = task.TryCast<DivertPowerTask>();
                if (divertPowerTask != null && divertPowerTask.TargetSystem == type)
                {
                    return divertPowerTask;
                }
            }
            return null;
        }
        public static bool IsDivertPowerTaskConsole(Console console)
        {
            foreach (TaskSet set in console.ValidTasks)
            {
                if (set.taskType == TaskTypes.DivertPower)
                {
                    return true;
                }
            }
            return false;
        }
        public static Console GetClosestDivertPowerConsole(Minigame minigame, SystemTypes type)
        {
            float dis = 10;
            Console closest = null;
            foreach ((Console console, ArrowBehaviour arrow) pair in DivertPower.tasks[type])
            {
                if (IsDivertPowerTaskConsole(pair.console))
                {
                    float currentDis = Vector2.Distance(pair.console.transform.position, PlayerControl.LocalPlayer.transform.position);
                    if (dis > currentDis)
                    {
                        dis = currentDis;
                        closest = pair.console;
                    }
                }
            }
            return closest;
        }
    }
}
