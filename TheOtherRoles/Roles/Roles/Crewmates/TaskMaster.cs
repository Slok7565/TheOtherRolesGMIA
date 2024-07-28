using System;
using TheOtherRoles.Utilities;
using TheOtherRoles.Players;
using Hazel;
using TheOtherRoles.Objects;
using TheOtherRoles.Roles.Core;
using TheOtherRoles.Roles.Core.Bases;
using TheOtherRoles.Helpers;
using UnityEngine;
using static TheOtherRoles.CustomOption;
using System.Linq;
using TheOtherRoles.Patches;
using Reactor.Utilities.Extensions;
using System.Collections.Generic;

namespace TheOtherRoles.Roles.Crewmates;
public sealed class TaskMaster : RoleBase
{
    public PlayerControl taskMaster = null;
    public bool becomeATaskMasterWhenCompleteAllTasks = false;
    public Color color = new Color32(225, 86, 75, byte.MaxValue);
    public bool isTaskComplete = false;
    public byte clearExTasks = 0;
    public byte allExTasks = 0;
    public byte oldTaskMasterPlayerId = byte.MaxValue;
    public bool triggerTaskMasterWin = false;

    public void clearAndReload()
    {
        taskMaster = null;
        becomeATaskMasterWhenCompleteAllTasks = CustomOptionHolder.taskMasterBecomeATaskMasterWhenCompleteAllTasks.getBool();
        isTaskComplete = false;
        clearExTasks = 0;
        allExTasks = 0;
        oldTaskMasterPlayerId = byte.MaxValue;
        triggerTaskMasterWin = false;
    }

    public bool isTaskMaster(byte playerId)
    {
        return taskMaster != null && taskMaster.PlayerId == playerId;
    }
}
