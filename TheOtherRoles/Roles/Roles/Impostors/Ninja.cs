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
using HarmonyLib;
using static TheOtherRoles.Roles.TheOtherRoles;
using TheOtherRoles.Roles.Crewmates;

namespace TheOtherRoles.Roles.Impostor;
public sealed class Ninja : RoleBase
{
    public PlayerControl ninja;
    public Color color = Palette.ImpostorRed;
    public float stealthCooldown = 30f;
    public float stealthDuration = 15f;
    public float killPenalty = 10f;
    public float speedBonus = 1.25f;
    public float fadeTime = 0.5f;
    public bool canUseVents = false;
    public bool canBeTargeted;
    public float addition = 0f;

    public bool penalized = false;
    public bool stealthed = false;
    public DateTime stealthedAt = DateTime.UtcNow;

    private Sprite buttonSprite;
    public Sprite getButtonSprite()
    {
        if (buttonSprite) return buttonSprite;
        buttonSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.NinjaButton.png", 115f);
        return buttonSprite;
    }

    public bool isStealthed(PlayerControl player)
    {
        if (Ninja.ninja != null && !Ninja.ninja.Data.IsDead && Ninja.ninja == player)
            return Ninja.stealthed;
        return false;
    }

    public float stealthFade(PlayerControl player)
    {
        if (Ninja.ninja == player && fadeTime > 0 && !Ninja.ninja.Data.IsDead)
            return Mathf.Min(1.0f, (float)(DateTime.UtcNow - Ninja.stealthedAt).TotalSeconds / fadeTime);
        return 1.0f;
    }

    public bool isPenalized(PlayerControl player)
    {
        if (Ninja.ninja == player && !Ninja.ninja.Data.IsDead)
            return Ninja.penalized;
        return false;
    }

    public void setStealthed(PlayerControl player, bool stealthed = true)
    {
        if (Ninja.ninja == player && Ninja.ninja != null)
        {
            Ninja.stealthed = stealthed;
            Ninja.stealthedAt = DateTime.UtcNow;
        }
    }

    public void OnKill(PlayerControl target)
    {
        if (Ninja.stealthed)
        {
            Ninja.addition += Ninja.killPenalty;
            if (CachedPlayer.LocalPlayer.PlayerControl == Ninja.ninja)
            {
                Ninja.penalized = true;
                CachedPlayer.LocalPlayer.PlayerControl.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + Ninja.addition);
                Helpers.checkMurderAttemptAndKill(Ninja.ninja, target, false, false);
            }
        }
    }

    public void setOpacity(PlayerControl player, float opacity)
    {
        var color = Color.Lerp(Palette.ClearWhite, Palette.White, opacity);
        try
        {
            // Block setting opacity if the Chameleon skill is active
            if (Chameleon.chameleon.Any(x => x.PlayerId == player.PlayerId) && Chameleon.visibility(player.PlayerId) < 1f && !stealthed) return;
            ExtendPlayerControl.setInvisible(player, color);
        }
        catch { }
    }

    public void clearAndReload()
    {
        setOpacity(ninja, 1.0f);
        ninja = null;
        stealthCooldown = CustomOptionHolder.ninjaStealthCooldown.getFloat();
        stealthDuration = CustomOptionHolder.ninjaStealthDuration.getFloat();
        killPenalty = CustomOptionHolder.ninjaKillPenalty.getFloat();
        speedBonus = CustomOptionHolder.ninjaSpeedBonus.getFloat();
        fadeTime = CustomOptionHolder.ninjaFadeTime.getFloat();
        canUseVents = CustomOptionHolder.ninjaCanVent.getBool();
        canBeTargeted = CustomOptionHolder.ninjaCanBeTargeted.getBool();

        penalized = false;
        stealthed = false;
        stealthedAt = DateTime.UtcNow;
    }

    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
    public sealed class PlayerPhysicsNinjaPatch : RoleBase
    {
        public void Postfix(PlayerPhysics __instance)
        {
            if (__instance.AmOwner && __instance.myPlayer.CanMove && GameData.Instance && isStealthed(__instance.myPlayer))
                __instance.body.velocity *= speedBonus + 1;

            if (__instance.myPlayer == Ninja.ninja)
            {
                var ninja = __instance.myPlayer;
                if (ninja == null || ninja.Data.IsDead) return;

                bool canSee =
                    PlayerControl.LocalPlayer.Data.IsDead ||
                    PlayerControl.LocalPlayer.Data.Role.IsImpostor ||
                    Lighter.canSeeInvisible && PlayerControl.LocalPlayer == Lighter.lighter;

                var opacity = canSee ? 0.1f : 0.0f;

                if (isStealthed(ninja))
                {
                    opacity = Math.Max(opacity, 1.0f - stealthFade(ninja));
                    ninja.cosmetics.currentBodySprite.BodySprite.material.SetFloat("_Outline", 0f);
                }
                else
                    opacity = Math.Max(opacity, stealthFade(ninja));

                setOpacity(ninja, opacity);
            }
        }
    }
}
