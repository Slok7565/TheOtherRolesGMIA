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

namespace TheOtherRoles.Roles.Crewmates;
public sealed class Swapper : RoleBase
{
    public PlayerControl swapper;
    public Color color = new Color32(134, 55, 86, byte.MaxValue);
    private Sprite spriteCheck;
    public bool canCallEmergency = false;
    public bool canOnlySwapOthers = false;
    public int charges;
    public float rechargeTasksNumber;
    public float rechargedTasks;

    public byte playerId1 = byte.MaxValue;
    public byte playerId2 = byte.MaxValue;

    public Sprite getCheckSprite()
    {
        if (spriteCheck) return spriteCheck;
        spriteCheck = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.SwapperCheck.png", 150f);
        return spriteCheck;
    }

    public override void clearAndReload()
    {
        swapper = null;
        playerId1 = byte.MaxValue;
        playerId2 = byte.MaxValue;
        canCallEmergency = CustomOptionHolder.swapperCanCallEmergency.getBool();
        canOnlySwapOthers = CustomOptionHolder.swapperCanOnlySwapOthers.getBool();
        charges = Mathf.RoundToInt(CustomOptionHolder.swapperSwapsNumber.getFloat());
        rechargeTasksNumber = Mathf.RoundToInt(CustomOptionHolder.swapperRechargeTasksNumber.getFloat());
        rechargedTasks = Mathf.RoundToInt(CustomOptionHolder.swapperRechargeTasksNumber.getFloat());
    }
}
