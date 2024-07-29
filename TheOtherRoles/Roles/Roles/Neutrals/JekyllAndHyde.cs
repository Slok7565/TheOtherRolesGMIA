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
using TheOtherRoles.Utilities;

namespace TheOtherRoles.Roles.Neutral;
public sealed class JekyllAndHyde : RoleBase
{
    public Color color = Color.grey;
    public PlayerControl jekyllAndHyde;
    public PlayerControl formerJekyllAndHyde;
    public PlayerControl currentTarget;

    public enum Status
    {
        None,
        Jekyll,
        Hyde,
    }

    public Status status;
    public int counter = 0;
    public int numberToWin = 3;
    public float suicideTimer = 40f;
    public bool reset = true;
    public float cooldown = 18f;
    public int numUsed;
    public bool oddIsJekyll;
    public bool triggerWin = false;
    public int numCommonTasks;
    public int numLongTasks;
    public int numShortTasks;
    public int numTasks;

    public bool isOdd(int n)
    {
        return n % 2 == 1;
    }

    public bool isJekyll()
    {
        if (status == Status.None)
        {
            var alive = PlayerControl.AllPlayerControls.GetFastEnumerator().ToArray().Where(x =>
            {
                return !x.Data.IsDead;
            });
            bool ret = oddIsJekyll ? isOdd(alive.Count()) : !isOdd(alive.Count());
            return ret;
        }
        return status == Status.Jekyll;
    }

    public int getNumDrugs()
    {
        var p = jekyllAndHyde;
        int counter = p.Data.Tasks.ToArray().Where(t => t.Complete).Count();
        return (int)Math.Floor((float)counter / numTasks);
    }

    public void clearAndReload()
    {
        jekyllAndHyde = null;
        formerJekyllAndHyde = null;
        currentTarget = null;
        status = Status.None;
        counter = 0;
        triggerWin = false;
        numUsed = 0;
        numTasks = (int)CustomOptionHolder.jekyllAndHydeNumTasks.getFloat();
        numCommonTasks = (int)CustomOptionHolder.jekyllAndHydeCommonTasks.getFloat();
        numShortTasks = (int)CustomOptionHolder.jekyllAndHydeShortTasks.getFloat();
        numLongTasks = (int)CustomOptionHolder.jekyllAndHydeLongTasks.getFloat();
        reset = CustomOptionHolder.jekyllAndHydeResetAfterMeeting.getBool();
        numberToWin = (int)CustomOptionHolder.jekyllAndHydeNumberToWin.getFloat();
        cooldown = CustomOptionHolder.jekyllAndHydeCooldown.getFloat();
        suicideTimer = CustomOptionHolder.jekyllAndHydeSuicideTimer.getFloat();
    }
}
