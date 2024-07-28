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
using HarmonyLib;
using static TheOtherRoles.Roles.TheOtherRoles;

namespace TheOtherRoles.Roles.Crewmates;
public sealed class Sprinter : RoleBase
{
    public PlayerControl sprinter;
    public Color color = new Color32(128, 128, 255, byte.MaxValue);

    public float sprintCooldown = 30f;
    public float sprintDuration = 15f;
    public float fadeTime = 0.5f;

    public bool sprinting = false;

    public DateTime sprintAt = DateTime.UtcNow;

    private Sprite buttonSprite;
    public Sprite getButtonSprite()
    {
        if (buttonSprite) return buttonSprite;
        buttonSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.SprintButton.png", 115f);
        return buttonSprite;
    }

    public float sprintFade(PlayerControl player)
    {
        if (Sprinter.sprinter == player && fadeTime > 0 && !Sprinter.sprinter.Data.IsDead)
            return Mathf.Min(1.0f, (float)(DateTime.UtcNow - Sprinter.sprintAt).TotalSeconds / fadeTime);
        return 1.0f;
    }

    public bool isSprinting()
    {
        if (CachedPlayer.LocalPlayer.PlayerControl == Sprinter.sprinter && !Sprinter.sprinter.Data.IsDead)
            return Sprinter.sprinting;
        return false;
    }

    public void setSprinting(PlayerControl player, bool sprinting = true)
    {
        if (player == Sprinter.sprinter && !Sprinter.sprinter.Data.IsDead)
        {
            Sprinter.sprinting = sprinting;
            Sprinter.sprintAt = DateTime.UtcNow;
        }
    }

    public void setOpacity(PlayerControl player, float opacity)
    {
        var color = Color.Lerp(Palette.ClearWhite, Palette.White, opacity);
        try
        {
            if (Chameleon.chameleon.Any(x => x.PlayerId == player.PlayerId) && Chameleon.visibility(player.PlayerId) < 1f && !sprinting) return;
            ExtendPlayerControl.setInvisible(player, color);
        }
        catch { }
    }

    public void clearAndReload()
    {
        setOpacity(sprinter, 1.0f);
        sprinter = null;
        sprinting = false;
        sprintCooldown = CustomOptionHolder.sprinterCooldown.getFloat();
        sprintDuration = CustomOptionHolder.sprinterDuration.getFloat();
        fadeTime = CustomOptionHolder.sprinterFadeTime.getFloat();
    }

    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
    public sealed class PlayerPhysicsSprinterPatch : RoleBase
    {
        public void Postfix(PlayerPhysics __instance)
        {
            if (__instance.myPlayer == Sprinter.sprinter)
            {
                var sprinter = __instance.myPlayer;
                if (sprinter == null || sprinter.Data.IsDead) return;

                bool canSee =
                    PlayerControl.LocalPlayer.Data.IsDead ||
                    CachedPlayer.LocalPlayer.PlayerControl == Sprinter.sprinter ||
                    Lighter.canSeeInvisible && PlayerControl.LocalPlayer == Lighter.lighter;

                var opacity = canSee ? 0.1f : 0.0f;

                if (Sprinter.sprinting)
                {
                    opacity = Math.Max(opacity, 1.0f - sprintFade(sprinter));
                    sprinter.cosmetics.currentBodySprite.BodySprite.material.SetFloat("_Outline", 0f);
                }
                else
                    opacity = Math.Max(opacity, sprintFade(sprinter));

                setOpacity(sprinter, opacity);
            }
        }
    }
}
