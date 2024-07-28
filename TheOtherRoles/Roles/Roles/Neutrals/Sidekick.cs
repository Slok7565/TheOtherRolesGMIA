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

namespace TheOtherRoles.Roles.Neutral;
public sealed class Sidekick : RoleBase
{
    public PlayerControl sidekick;
    public Color color = new Color32(0, 180, 235, byte.MaxValue);

    public PlayerControl currentTarget;

    public bool wasTeamRed;
    public bool wasImpostor;
    public bool wasSpy;

    public float cooldown = 30f;
    public bool canUseVents = true;
    public bool canKill = true;
    public bool promotesToJackal = true;
    public bool hasImpostorVision = false;
    public bool canSabotageLights;

    public override void clearAndReload()
    {
        sidekick = null;
        currentTarget = null;
        cooldown = CustomOptionHolder.jackalKillCooldown.getFloat();
        canUseVents = CustomOptionHolder.sidekickCanUseVents.getBool();
        canKill = CustomOptionHolder.sidekickCanKill.getBool();
        promotesToJackal = CustomOptionHolder.sidekickPromotesToJackal.getBool();
        hasImpostorVision = CustomOptionHolder.jackalAndSidekickHaveImpostorVision.getBool();
        wasTeamRed = wasImpostor = wasSpy = false;
        canSabotageLights = CustomOptionHolder.sidekickCanSabotageLights.getBool();
    }
}
