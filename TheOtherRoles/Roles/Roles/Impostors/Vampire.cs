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
public sealed class Vampire : RoleBase
{
    public PlayerControl vampire;
    public Color color = Palette.ImpostorRed;

    public float delay = 10f;
    public float cooldown = 30f;
    public bool canKillNearGarlics = true;
    public bool localPlacedGarlic = false;
    public bool garlicsActive = true;

    public PlayerControl currentTarget;
    public PlayerControl bitten;
    public bool targetNearGarlic = false;

    private Sprite buttonSprite;
    public Sprite getButtonSprite()
    {
        if (buttonSprite) return buttonSprite;
        buttonSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.VampireButton.png", 115f);
        return buttonSprite;
    }

    private Sprite garlicButtonSprite;
    public Sprite getGarlicButtonSprite()
    {
        if (garlicButtonSprite) return garlicButtonSprite;
        garlicButtonSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.GarlicButton.png", 115f);
        return garlicButtonSprite;
    }

    public override void clearAndReload()
    {
        vampire = null;
        bitten = null;
        targetNearGarlic = false;
        localPlacedGarlic = false;
        currentTarget = null;
        garlicsActive = CustomOptionHolder.vampireSpawnRate.getSelection() > 0;
        delay = CustomOptionHolder.vampireKillDelay.getFloat();
        cooldown = CustomOptionHolder.vampireCooldown.getFloat();
        canKillNearGarlics = CustomOptionHolder.vampireCanKillNearGarlics.getBool();
    }
}
