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
public sealed class Blackmailer : RoleBase
{
    public PlayerControl blackmailer;
    public Color color = Palette.ImpostorRed;
    public Color blackmailedColor = Palette.White;

    public bool alreadyShook = false;
    public PlayerControl blackmailed;
    public PlayerControl currentTarget;
    public float cooldown = 30f;
    private Sprite blackmailButtonSprite;
    private Sprite overlaySprite;

    public Sprite getBlackmailOverlaySprite()
    {
        if (overlaySprite) return overlaySprite;
        overlaySprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.BlackmailerOverlay.png", 100f);
        return overlaySprite;
    }

    public Sprite getBlackmailButtonSprite()
    {
        if (blackmailButtonSprite) return blackmailButtonSprite;
        blackmailButtonSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.BlackmailerBlackmailButton.png", 115f);
        return blackmailButtonSprite;
    }

    public void clearAndReload()
    {
        blackmailer = null;
        currentTarget = null;
        blackmailed = null;
        alreadyShook = false;
        cooldown = CustomOptionHolder.blackmailerCooldown.getFloat();
    }
}
