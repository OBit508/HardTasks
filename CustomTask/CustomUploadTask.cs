using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ImpossibleTasksV3.CustomTask
{
    internal class CustomUploadTask : MonoBehaviour
    {
        public UploadDataGame Game;
        public float timer;
        public float timer2;
        public List<GenericPopup> PopUps = new List<GenericPopup>();
        public bool doPercent;
        public int value;
        public string[] strings = new string[] 
        {
            "ERROR",
            ":)",
            "Hi",
            "Pop Up",
            "This Upload is funny",
            ":/",
            "Close me",
            "...",
            "abcdefghijklmnopqrstuvwxyz"
        };

        public void Create_Pop_Up()
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
        public void Update()
        {
            if (doPercent)
            {
                if (Game.Gauge.transform.localScale.x < 0.85f)
                {
                    timer += Time.deltaTime;
                    if (timer >= 0.02f)
                    {
                        Game.Gauge.transform.localScale = new Vector3(Game.Gauge.transform.localScale.x + 0.1f, 1, 1);
                        timer = 0;
                    }
                }
                else
                {
                    if (value >= 100)
                    {
                        Game.MyNormTask.NextStep();
                        Game.StartCoroutine(Game.CoStartClose());
                        GameObject.Destroy(this);
                        return;
                    }
                    timer += Time.deltaTime;
                    if (timer >= 1f)
                    {
                        Create_Pop_Up();
                        timer = 0;
                    }
                    if (PopUps.Count <= 0)
                    {
                        timer2 += Time.deltaTime;
                        if (timer2 >= 0.15f && value < 100)
                        {
                            Game.Gauge.Value += 0.01f;
                            value++;
                            timer2 = 0;
                        }
                    }
                }
            }
        }
    }
}
