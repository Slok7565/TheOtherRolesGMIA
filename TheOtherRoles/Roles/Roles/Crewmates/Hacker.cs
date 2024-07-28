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
public sealed class Hacker : RoleBase
{
    public PlayerControl hacker;
    public Minigame vitals = null;
    public Minigame doorLog = null;
    public Color color = new Color32(117, 250, 76, byte.MaxValue);

    public float cooldown = 30f;
    public float duration = 10f;
    public float toolsNumber = 5f;
    public bool onlyColorType = false;
    public float hackerTimer = 0f;
    public int rechargeTasksNumber = 2;
    public int rechargedTasks = 2;
    public int chargesVitals = 1;
    public int chargesAdminTable = 1;
    public bool cantMove = true;

    private Sprite buttonSprite;
    private Sprite vitalsSprite;
    private Sprite logSprite;
    private Sprite adminSprite;

    public Sprite getButtonSprite()
    {
        if (buttonSprite) return buttonSprite;
        buttonSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.HackerButton.png", 115f);
        return buttonSprite;
    }

    public Sprite getVitalsSprite()
    {
        if (vitalsSprite) return vitalsSprite;
        vitalsSprite = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.VitalsButton].Image;
        return vitalsSprite;
    }

    public Sprite getLogSprite()
    {
        if (logSprite) return logSprite;
        logSprite = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.DoorLogsButton].Image;
        return logSprite;
    }

    public Sprite getAdminSprite()
    {
        byte mapId = GameOptionsManager.Instance.currentNormalGameOptions.MapId;
        UseButtonSettings button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.PolusAdminButton]; // Polus
        if (Helpers.isSkeld() || mapId == 3) button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.AdminMapButton]; // Skeld || Dleks
        else if (Helpers.isMira()) button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.MIRAAdminButton]; // Mira HQ
        else if (Helpers.isAirship()) button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.AirshipAdminButton]; // Airship
        else if (Helpers.isFungle()) button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.AdminMapButton];
        adminSprite = button.Image;
        return adminSprite;
    }

    public override void clearAndReload()
    {
        hacker = null;
        vitals = null;
        doorLog = null;
        hackerTimer = 0f;
        adminSprite = null;
        cooldown = CustomOptionHolder.hackerCooldown.getFloat();
        duration = CustomOptionHolder.hackerHackeringDuration.getFloat();
        onlyColorType = CustomOptionHolder.hackerOnlyColorType.getBool();
        toolsNumber = CustomOptionHolder.hackerToolsNumber.getFloat();
        rechargeTasksNumber = Mathf.RoundToInt(CustomOptionHolder.hackerRechargeTasksNumber.getFloat());
        rechargedTasks = Mathf.RoundToInt(CustomOptionHolder.hackerRechargeTasksNumber.getFloat());
        chargesVitals = Mathf.RoundToInt(CustomOptionHolder.hackerToolsNumber.getFloat()) / 2;
        chargesAdminTable = Mathf.RoundToInt(CustomOptionHolder.hackerToolsNumber.getFloat()) / 2;
        cantMove = CustomOptionHolder.hackerNoMove.getBool();
    }
}
