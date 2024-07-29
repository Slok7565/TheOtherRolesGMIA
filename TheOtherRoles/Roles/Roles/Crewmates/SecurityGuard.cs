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
public sealed class SecurityGuard : RoleBase
{
    public PlayerControl securityGuard;
    public Color color = new Color32(195, 178, 95, byte.MaxValue);

    public float cooldown = 30f;
    public int remainingScrews = 7;
    public int totalScrews = 7;
    public int ventPrice = 1;
    public int camPrice = 2;
    public int placedCameras = 0;
    public float duration = 10f;
    public int maxCharges = 5;
    public int rechargeTasksNumber = 3;
    public int rechargedTasks = 3;
    public int charges = 1;
    public bool cantMove = true;
    public Vent ventTarget = null;
    public Minigame minigame = null;

    private Sprite closeVentButtonSprite;
    public Sprite getCloseVentButtonSprite()
    {
        if (closeVentButtonSprite) return closeVentButtonSprite;
        closeVentButtonSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.CloseVentButton.png", 115f);
        return closeVentButtonSprite;
    }

    private Sprite placeCameraButtonSprite;
    public Sprite getPlaceCameraButtonSprite()
    {
        if (placeCameraButtonSprite) return placeCameraButtonSprite;
        placeCameraButtonSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.PlaceCameraButton.png", 115f);
        return placeCameraButtonSprite;
    }

    private Sprite animatedVentSealedSprite;
    private float lastPPU;
    public Sprite getAnimatedVentSealedSprite()
    {
        float ppu = 185f;
        if (SubmergedCompatibility.IsSubmerged) ppu = 120f;
        if (lastPPU != ppu)
        {
            animatedVentSealedSprite = null;
            lastPPU = ppu;
        }
        if (animatedVentSealedSprite) return animatedVentSealedSprite;
        animatedVentSealedSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.AnimatedVentSealed.png", ppu);
        return animatedVentSealedSprite;
    }

    private Sprite VentSealedSprite;
    public Sprite getVentSealedSprite()
    {
        if (VentSealedSprite) return VentSealedSprite;
        VentSealedSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.VentSealed.png", 160f);
        return VentSealedSprite;
    }

    private Sprite fungleVentSealedSprite;
    public Sprite getFungleVentSealedSprite()
    {
        if (fungleVentSealedSprite) return fungleVentSealedSprite;
        fungleVentSealedSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.FungleVentSealed.png", 160f);
        return fungleVentSealedSprite;
    }

    private Sprite submergedCentralUpperVentSealedSprite;
    public Sprite getSubmergedCentralUpperSealedSprite()
    {
        if (submergedCentralUpperVentSealedSprite) return submergedCentralUpperVentSealedSprite;
        submergedCentralUpperVentSealedSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.CentralUpperBlocked.png", 145f);
        return submergedCentralUpperVentSealedSprite;
    }

    private Sprite submergedCentralLowerVentSealedSprite;
    public Sprite getSubmergedCentralLowerSealedSprite()
    {
        if (submergedCentralLowerVentSealedSprite) return submergedCentralLowerVentSealedSprite;
        submergedCentralLowerVentSealedSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.CentralLowerBlocked.png", 145f);
        return submergedCentralLowerVentSealedSprite;
    }

    private Sprite camSprite;
    public Sprite getCamSprite()
    {
        if (camSprite) return camSprite;
        camSprite = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.CamsButton].Image;
        return camSprite;
    }

    private Sprite logSprite;
    public Sprite getLogSprite()
    {
        if (logSprite) return logSprite;
        logSprite = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.DoorLogsButton].Image;
        return logSprite;
    }

    public override void clearAndReload()
    {
        securityGuard = null;
        ventTarget = null;
        minigame = null;
        duration = CustomOptionHolder.securityGuardCamDuration.getFloat();
        maxCharges = Mathf.RoundToInt(CustomOptionHolder.securityGuardCamMaxCharges.getFloat());
        rechargeTasksNumber = Mathf.RoundToInt(CustomOptionHolder.securityGuardCamRechargeTasksNumber.getFloat());
        rechargedTasks = Mathf.RoundToInt(CustomOptionHolder.securityGuardCamRechargeTasksNumber.getFloat());
        charges = Mathf.RoundToInt(CustomOptionHolder.securityGuardCamMaxCharges.getFloat()) / 2;
        placedCameras = 0;
        cooldown = CustomOptionHolder.securityGuardCooldown.getFloat();
        totalScrews = remainingScrews = Mathf.RoundToInt(CustomOptionHolder.securityGuardTotalScrews.getFloat());
        camPrice = Mathf.RoundToInt(CustomOptionHolder.securityGuardCamPrice.getFloat());
        ventPrice = Mathf.RoundToInt(CustomOptionHolder.securityGuardVentPrice.getFloat());
        cantMove = CustomOptionHolder.securityGuardNoMove.getBool();
    }
}
