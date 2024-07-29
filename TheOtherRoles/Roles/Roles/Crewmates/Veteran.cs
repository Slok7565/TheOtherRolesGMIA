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
public sealed class Veteran : RoleBase
{
    public PlayerControl veteran;
    public Color color = new Color32(255, 77, 0, byte.MaxValue);

    public float alertDuration = 3f;
    public float cooldown = 30f;

    public int remainingAlerts = 5;

    public bool alertActive = false;

    private Sprite buttonSprite;
    public Sprite getButtonSprite()
    {
        if (buttonSprite) return buttonSprite;
        buttonSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.AlertButton.png", 115f);
        return buttonSprite;
    }

    public void clearAndReload()
    {
        veteran = null;
        alertActive = false;
        alertDuration = CustomOptionHolder.veteranAlertDuration.getFloat();
        cooldown = CustomOptionHolder.veteranCooldown.getFloat();
        remainingAlerts = Mathf.RoundToInt(CustomOptionHolder.veteranAlertNumber.getFloat());
    }
}
