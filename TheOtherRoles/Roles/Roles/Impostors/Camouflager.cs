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
public sealed class Camouflager : RoleBase
{
    public PlayerControl camouflager;
    public Color color = Palette.ImpostorRed;

    public float cooldown = 30f;
    public float duration = 10f;
    public float camouflageTimer = 0f;

    private Sprite buttonSprite;
    public Sprite getButtonSprite()
    {
        if (buttonSprite) return buttonSprite;
        buttonSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.CamoButton.png", 115f);
        return buttonSprite;
    }

    public void resetCamouflage()
    {
        camouflageTimer = 0f;
        foreach (PlayerControl p in CachedPlayer.AllPlayers)
            /*if ((p == Ninja.ninja && Ninja.stealthed) || (p == Sprinter.sprinter && Sprinter.sprinting))
                continue;*/
            p.setDefaultOutFit();
    }

    public override void clearAndReload()
    {
        resetCamouflage();
        camouflager = null;
        camouflageTimer = 0f;
        cooldown = CustomOptionHolder.camouflagerCooldown.getFloat();
        duration = CustomOptionHolder.camouflagerDuration.getFloat();
    }
}
