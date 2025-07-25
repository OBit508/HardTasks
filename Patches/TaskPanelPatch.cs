using HardTasks.CustomTask;
using HardTasks.Skeld;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using Il2CppSystem.Text;
using System.Threading.Tasks;

namespace HardTasks.Patches
{
    [HarmonyPatch(typeof(TaskPanelBehaviour), "SetTaskText")]
    internal class TaskPanelPatch
    {
        public static bool Prefix(TaskPanelBehaviour __instance, [HarmonyArgument(0)] string str)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                foreach (PlayerTask task in PlayerControl.LocalPlayer.myTasks)
                {
                    DivertPowerTask divertPowerTask = task.TryCast<DivertPowerTask>();
                    if (divertPowerTask != null)
                    {
                        ShipStatus ship = ShipStatus.Instance;
                        int consoles = 0;
                        StringNames mapName = StringNames.MapNameSkeld;
                        foreach (Console console in ship.AllConsoles)
                        {
                            if (DivertPower.IsDivertPowerTaskConsole(console))
                            {
                                consoles++;
                            }
                        }
                        if (ship.name == "MiraShip(Clone)")
                        {
                            mapName = StringNames.MapNameMira;
                        }
                        else if (ship.name == "PolusShip(Clone)")
                        {
                            mapName = StringNames.MapNamePolus;
                        }
                        else if (ship.name == "Airship(Clone)")
                        {
                            mapName = StringNames.MapNameAirship;
                        }
                        else if (ship.name == "FungleShip(Clone)")
                        {
                            mapName = StringNames.MapNameFungle;
                        }
                        if (divertPowerTask.TaskStep == 0)
                        {
                            sb.AppendLine(TranslationController.Instance.GetString(SystemTypes.Electrical) + ": Divert Power (0/" + (consoles + 1).ToString() + ")");
                        }
                        else if (divertPowerTask.TaskStep == 1)
                        {
                            sb.AppendLine("<color=#FFFF00FF>" + TranslationController.Instance.GetString(mapName) + ": Accept Diverted Power (" + (1 + consoles - DivertPower.consoles.Count).ToString() + "/" + (consoles + 1).ToString() + ")</color>");
                        }
                        else
                        {
                            sb.AppendLine("<color=#00DD00FF>" + TranslationController.Instance.GetString(mapName) + ": Divert Power (" + (consoles + 1).ToString() + "/" + (consoles + 1).ToString() + ")</color>");
                        }
                    }
                    else
                    {
                        task.AppendTaskText(sb);
                    }
                }
                PlayerControl.LocalPlayer.Data.Role.AppendTaskHint(sb);
                __instance.taskText.text = sb.ToString();
            }
            catch
            {
            }
            return false;
        }
    }
}
