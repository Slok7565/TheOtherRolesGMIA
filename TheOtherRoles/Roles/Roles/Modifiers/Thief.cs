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

public sealed class Thief : RoleBase
{
    public PlayerControl thief;
    public Color color = new Color32(71, 99, 45, byte.MaxValue);
    public PlayerControl currentTarget;
    public PlayerControl formerThief;

    public float cooldown = 30f;

    public bool suicideFlag = false;  // Used as a flag for suicide

    public bool hasImpostorVision;
    public bool canUseVents;
    public bool canKillSheriff;
    public bool canStealWithGuess;

    public override void clearAndReload()
    {
        thief = null;
        suicideFlag = false;
        currentTarget = null;
        formerThief = null;
        hasImpostorVision = CustomOptionHolder.thiefHasImpVision.getBool();
        cooldown = CustomOptionHolder.thiefCooldown.getFloat();
        canUseVents = CustomOptionHolder.thiefCanUseVents.getBool();
        canKillSheriff = CustomOptionHolder.thiefCanKillSheriff.getBool();
        canStealWithGuess = CustomOptionHolder.thiefCanStealWithGuess.getBool();
    }

    public bool isFailedThiefKill(PlayerControl target, PlayerControl killer, RoleInfo targetRole)
    {
        return killer == Thief.thief && !target.Data.Role.IsImpostor && !new List<RoleInfo> { RoleInfo.jackal, canKillSheriff ? RoleInfo.sheriff : null, RoleInfo.sidekick, RoleInfo.moriarty, RoleInfo.jekyllAndHyde }.Contains(targetRole);
    }
}
