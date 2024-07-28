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
public sealed class Teleporter : RoleBase
{
    public PlayerControl teleporter;
    public Color color = new Color32(164, 249, 255, byte.MaxValue);
    private Sprite teleportButtonSprite;
    public float teleportCooldown = 30f;
    public float sampleCooldown = 30f;
    public int teleportNumber = 5;
    public PlayerControl target1;
    public PlayerControl target2;
    public PlayerControl currentTarget;

    public Sprite getButtonSprite()
    {
        if (teleportButtonSprite) return teleportButtonSprite;
        teleportButtonSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.TeleporterButton.png", 115f);
        return teleportButtonSprite;
    }

    public void clearAndReload()
    {
        teleporter = null;
        target1 = null;
        target2 = null;
        currentTarget = null;
        teleportCooldown = CustomOptionHolder.teleporterCooldown.getFloat();
        teleportNumber = (int)CustomOptionHolder.teleporterTeleportNumber.getFloat();
        sampleCooldown = CustomOptionHolder.teleporterSampleCooldown.getFloat();
    }
}
