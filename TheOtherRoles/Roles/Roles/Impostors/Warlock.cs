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

namespace TheOtherRoles.Roles.Impostor;
public sealed class Warlock : RoleBase
{

    public PlayerControl warlock;
    public Color color = Palette.ImpostorRed;

    public PlayerControl currentTarget;
    public PlayerControl curseVictim;
    public PlayerControl curseVictimTarget;

    public float cooldown = 30f;
    public float rootTime = 5f;

    private Sprite curseButtonSprite;
    private Sprite curseKillButtonSprite;

    public Sprite getCurseButtonSprite()
    {
        if (curseButtonSprite) return curseButtonSprite;
        curseButtonSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.CurseButton.png", 115f);
        return curseButtonSprite;
    }

    public Sprite getCurseKillButtonSprite()
    {
        if (curseKillButtonSprite) return curseKillButtonSprite;
        curseKillButtonSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.CurseKillButton.png", 115f);
        return curseKillButtonSprite;
    }

    public override void clearAndReload()
    {
        warlock = null;
        currentTarget = null;
        curseVictim = null;
        curseVictimTarget = null;
        cooldown = CustomOptionHolder.warlockCooldown.getFloat();
        rootTime = CustomOptionHolder.warlockRootTime.getFloat();
    }

    public void resetCurse()
    {
        HudManagerStartPatch.warlockCurseButton.Timer = HudManagerStartPatch.warlockCurseButton.MaxTimer;
        HudManagerStartPatch.warlockCurseButton.Sprite = Warlock.getCurseButtonSprite();
        HudManagerStartPatch.warlockCurseButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
        currentTarget = null;
        curseVictim = null;
        curseVictimTarget = null;
    }
}
