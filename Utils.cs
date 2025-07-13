using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    }
}
