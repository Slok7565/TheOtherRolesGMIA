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
public sealed class Jackal : RoleBase
{
    public PlayerControl jackal;
    public Color color = new Color32(0, 180, 235, byte.MaxValue);
    public PlayerControl fakeSidekick;
    public PlayerControl currentTarget;
    public List<PlayerControl> formerJackals = new List<PlayerControl>();

    public float cooldown = 30f;
    public float createSidekickCooldown = 30f;
    public bool canUseVents = true;
    public bool canCreateSidekick = true;
    public Sprite buttonSprite;
    public bool jackalPromotedFromSidekickCanCreateSidekick = true;
    public bool canCreateSidekickFromImpostor = true;
    public bool hasImpostorVision = false;
    public bool wasTeamRed;
    public bool wasImpostor;
    public bool wasSpy;
    public bool canSabotageLights;

    public Sprite getSidekickButtonSprite()
    {
        if (buttonSprite) return buttonSprite;
        buttonSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.SidekickButton.png", 115f);
        return buttonSprite;
    }

    public void removeCurrentJackal()
    {
        if (!formerJackals.Any(x => x.PlayerId == jackal.PlayerId)) formerJackals.Add(jackal);
        jackal = null;
        currentTarget = null;
        fakeSidekick = null;
        cooldown = CustomOptionHolder.jackalKillCooldown.getFloat();
        createSidekickCooldown = CustomOptionHolder.jackalCreateSidekickCooldown.getFloat();
    }

    public override void clearAndReload()
    {
        jackal = null;
        currentTarget = null;
        fakeSidekick = null;
        cooldown = CustomOptionHolder.jackalKillCooldown.getFloat();
        createSidekickCooldown = CustomOptionHolder.jackalCreateSidekickCooldown.getFloat();
        canUseVents = CustomOptionHolder.jackalCanUseVents.getBool();
        canCreateSidekick = CustomOptionHolder.jackalCanCreateSidekick.getBool();
        jackalPromotedFromSidekickCanCreateSidekick = CustomOptionHolder.jackalPromotedFromSidekickCanCreateSidekick.getBool();
        canCreateSidekickFromImpostor = CustomOptionHolder.jackalCanCreateSidekickFromImpostor.getBool();
        formerJackals.Clear();
        hasImpostorVision = CustomOptionHolder.jackalAndSidekickHaveImpostorVision.getBool();
        wasTeamRed = wasImpostor = wasSpy = false;
        canSabotageLights = CustomOptionHolder.jackalCanSabotageLights.getBool();
    }

}

