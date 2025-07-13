using HarmonyLib;
using Il2CppInterop.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HardTasks.Skeld
{
    [HarmonyPatch(typeof(CourseMinigame))]
    internal class CourseTask
    {
        public static List<Transform> Asteroids = new List<Transform>();
        public static float timer;
        [HarmonyPatch("FixedUpdate")]
        [HarmonyPrefix]
        public static void FixedUpdatePrefix(CourseMinigame __instance)
        {
            foreach (Transform t in Asteroids)
            {
                t.Rotate(0f, 0f, 70f * Time.deltaTime);
                Collider2D[] colliders = Physics2D.OverlapPointAll(t.position);
                bool flag = false;
                foreach (Collider2D c in colliders)
                {
                    if (c == __instance.Ship)
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag || t.transform.localPosition.y <= -1.1f)
                {
                    if (flag)
                    {
                        CourseTask.Reset(__instance);
                    }
                    Asteroids.Remove(t);
                    GameObject.Destroy(t.gameObject);
                    break;
                }
            }
            timer += Time.deltaTime;
            if (timer >= 0.5f)
            {
                Asteroid asteroid = GameObject.Instantiate<Asteroid>(Utils.GetMinigamePrefab(TaskTypes.ClearAsteroids).Cast<WeaponsMinigame>().asteroidPool.Prefab.Cast<Asteroid>(), __instance.transform);
                SpriteRenderer renderer = asteroid.GetComponent<SpriteRenderer>();
                renderer.sprite = asteroid.AsteroidImages[new System.Random().Next(0, asteroid.AsteroidImages.Count - 1)];
                renderer.maskInteraction = SpriteMaskInteraction.None;
                renderer.transform.localScale = new Vector3(0.3f, 0.3f, 1);
                renderer.transform.localPosition = new Vector3(UnityEngine.Random.RandomRange(-2f, 2f), 1.1f, -1);
                renderer.gameObject.AddComponent<Rigidbody2D>().gravityScale = 0.1f;
                GameObject.Destroy(asteroid);
                Asteroids.Add(renderer.transform);
                timer = 0;
            }
        }
        [HarmonyPatch("Begin")]
        [HarmonyPrefix]
        public static void BeginPrefix(CourseMinigame __instance)
        {
            timer = 0;
            Asteroids.Clear();
            __instance.NumPoints = 15;
        }
        public static void Reset(CourseMinigame minigame)
        {
            foreach (CourseStarBehaviour star in minigame.GetComponentsInChildren<CourseStarBehaviour>(true))
            {
                GameObject.Destroy(star.gameObject);
            }
            List<CourseStarBehaviour> list = new List<CourseStarBehaviour>() { null };
            minigame.Ship.transform.localPosition = minigame.PathPoints[0];
            for (int i = 1; i < minigame.PathPoints.Count; i++)
            {
                CourseStarBehaviour star = GameObject.Instantiate<CourseStarBehaviour>(minigame.StarPrefab, minigame.transform);
                star.transform.localPosition = minigame.PathPoints[i];
                list.Add(star);
            }
            minigame.Destination = list[1];
            minigame.Stars = list.ToArray();
            minigame.MyNormTask.Data = new byte[] { 0, 0, 0, 0 };
        }
    }
}
