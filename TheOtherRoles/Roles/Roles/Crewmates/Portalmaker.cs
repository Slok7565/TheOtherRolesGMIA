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

namespace TheOtherRoles.Roles.Crewmates;
public sealed class Portalmaker : RoleBase
{
    public PlayerControl portalmaker;
    public Color color = new Color32(69, 69, 169, byte.MaxValue);

    public float cooldown;
    public float usePortalCooldown;
    public bool logOnlyHasColors;
    public bool logShowsTime;
    public bool canPortalFromAnywhere;

    private Sprite placePortalButtonSprite;
    private Sprite usePortalButtonSprite;
    private Sprite usePortalSpecialButtonSprite1;
    private Sprite usePortalSpecialButtonSprite2;
    private Sprite logSprite;

    public Sprite getPlacePortalButtonSprite()
    {
        if (placePortalButtonSprite) return placePortalButtonSprite;
        placePortalButtonSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.PlacePortalButton.png", 115f);
        return placePortalButtonSprite;
    }

    public Sprite getUsePortalButtonSprite()
    {
        if (usePortalButtonSprite) return usePortalButtonSprite;
        usePortalButtonSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.UsePortalButton.png", 115f);
        return usePortalButtonSprite;
    }

    public Sprite getUsePortalSpecialButtonSprite(bool first)
    {
        if (first)
        {
            if (usePortalSpecialButtonSprite1) return usePortalSpecialButtonSprite1;
            usePortalSpecialButtonSprite1 = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.UsePortalSpecialButton1.png", 115f);
            return usePortalSpecialButtonSprite1;
        }
        else
        {
            if (usePortalSpecialButtonSprite2) return usePortalSpecialButtonSprite2;
            usePortalSpecialButtonSprite2 = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.UsePortalSpecialButton2.png", 115f);
            return usePortalSpecialButtonSprite2;
        }
    }

    public Sprite getLogSprite()
    {
        if (logSprite) return logSprite;
        logSprite = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.DoorLogsButton].Image;
        return logSprite;
    }

    public override void clearAndReload()
    {
        portalmaker = null;
        cooldown = CustomOptionHolder.portalmakerCooldown.getFloat();
        usePortalCooldown = CustomOptionHolder.portalmakerUsePortalCooldown.getFloat();
        logOnlyHasColors = CustomOptionHolder.portalmakerLogOnlyColorType.getBool();
        logShowsTime = CustomOptionHolder.portalmakerLogHasTime.getBool();
        canPortalFromAnywhere = CustomOptionHolder.portalmakerCanPortalFromAnywhere.getBool();
    }
}

