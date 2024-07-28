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
using TheOtherRoles.Utilities;

namespace TheOtherRoles.Roles.Impostor;
public sealed class EvilHacker : RoleBase
{
    public PlayerControl evilHacker;
    public Color color = Palette.ImpostorRed;
    public bool canHasBetterAdmin = false;
    public bool canCreateMadmate = false;
    public bool canCreateMadmateFromJackal;
    public bool canInheritAbility;
    public bool canSeeDoorStatus;
    public PlayerControl fakeMadmate;
    public PlayerControl currentTarget;

    private Sprite buttonSprite;
    private Sprite madmateButtonSprite;

    public Sprite getButtonSprite()
    {
        if (buttonSprite) return buttonSprite;
        byte mapId = GameOptionsManager.Instance.currentNormalGameOptions.MapId;
        UseButtonSettings button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.PolusAdminButton]; // Polus
        if (Helpers.isSkeld() || mapId == 3) button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.AdminMapButton]; // Skeld || Dleks
        else if (Helpers.isMira()) button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.MIRAAdminButton]; // Mira HQ
        else if (Helpers.isAirship()) button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.AirshipAdminButton]; // Airship
        else if (Helpers.isFungle()) button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.AdminMapButton];
        buttonSprite = button.Image;
        return buttonSprite;
    }

    public Sprite getMadmateButtonSprite()
    {
        if (madmateButtonSprite) return madmateButtonSprite;
        madmateButtonSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.SidekickButton.png", 115f);
        return madmateButtonSprite;
    }

    public bool isInherited()
    {
        return canInheritAbility && evilHacker != null && evilHacker.Data.IsDead && CachedPlayer.LocalPlayer.PlayerControl.Data.Role.IsImpostor;
    }

    public void clearAndReload()
    {
        evilHacker = null;
        currentTarget = null;
        fakeMadmate = null;
        canCreateMadmate = CustomOptionHolder.evilHackerCanCreateMadmate.getBool();
        canHasBetterAdmin = CustomOptionHolder.evilHackerCanHasBetterAdmin.getBool();
        canCreateMadmateFromJackal = CustomOptionHolder.evilHackerCanCreateMadmateFromJackal.getBool();
        canInheritAbility = CustomOptionHolder.evilHackerCanInheritAbility.getBool();
        canSeeDoorStatus = CustomOptionHolder.evilHackerCanSeeDoorStatus.getBool();
    }
}
