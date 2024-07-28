using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheOtherRoles.Roles.Core;
using TheOtherRoles.Helpers;
using TheOtherRoles.Roles.Core.Interfaces;
using TheOtherRoles.Roles.Neutral;
using UnityEngine;
using static TheOtherRoles.CustomOption;
using TheOtherRoles.Objects;
using Hazel;
using TheOtherRoles.Players;
using System.Xml.Serialization;
using TheOtherRoles.Roles.Core.Bases;

namespace TheOtherRoles.Roles.Modifier;

public sealed class Madmate : RoleBase
{
    public Color color = Palette.ImpostorRed;
    public List<PlayerControl> madmate = new List<PlayerControl>();
    public bool hasTasks;
    public bool canDieToSheriff;
    public bool canVent;
    public bool hasImpostorVision;
    public bool canFixComm;
    public bool canSabotage;
    public int commonTasks;
    public int shortTasks;
    public int longTasks;

    public string fullName { get { return ModTranslation.getString("madmate"); } }
    public string prefix { get { return ModTranslation.getString("madmatePrefix"); } }

    public bool tasksComplete(PlayerControl player)
    {
        if (!hasTasks) return false;

        int counter = 0;
        int totalTasks = commonTasks + longTasks + shortTasks;
        if (totalTasks == 0) return true;
        foreach (var task in player.Data.Tasks)
            if (task.Complete)
                counter++;
        return counter == totalTasks;
    }

    public void clearAndReload()
    {
        hasTasks = CustomOptionHolder.madmateAbility.getBool();
        madmate = new List<PlayerControl>();
        canDieToSheriff = CustomOptionHolder.madmateCanDieToSheriff.getBool();
        canVent = CustomOptionHolder.madmateCanEnterVents.getBool();
        hasImpostorVision = CustomOptionHolder.madmateHasImpostorVision.getBool();
        canFixComm = CustomOptionHolder.madmateCanFixComm.getBool();
        canSabotage = CustomOptionHolder.madmateCanSabotage.getBool();
        shortTasks = (int)CustomOptionHolder.madmateShortTasks.getFloat();
        commonTasks = (int)CustomOptionHolder.madmateCommonTasks.getFloat();
        longTasks = (int)CustomOptionHolder.madmateLongTasks.getFloat();
    }
}
