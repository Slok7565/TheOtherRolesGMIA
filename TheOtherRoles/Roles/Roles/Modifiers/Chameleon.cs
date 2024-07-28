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
using AmongUs.Data;
using TheOtherRoles.Roles.Crewmates;
using TheOtherRoles.Roles.Impostor;

namespace TheOtherRoles.Roles.Modifier;

public sealed class Chameleon : RoleBase
{
    public List<PlayerControl> chameleon = new List<PlayerControl>();
    public float minVisibility = 0.2f;
    public float holdDuration = 1f;
    public float fadeDuration = 0.5f;
    public Dictionary<byte, float> lastMoved;

    public override void clearAndReload()
    {
        chameleon = new List<PlayerControl>();
        lastMoved = new Dictionary<byte, float>();
        holdDuration = CustomOptionHolder.modifierChameleonHoldDuration.getFloat();
        fadeDuration = CustomOptionHolder.modifierChameleonFadeDuration.getFloat();
        minVisibility = CustomOptionHolder.modifierChameleonMinVisibility.getSelection() / 10f;
    }

    public float visibility(byte playerId)
    {
        if (playerId == Ninja.ninja?.PlayerId && Ninja.stealthed || playerId == Sprinter.sprinter?.PlayerId && Sprinter.sprinting) return 1f;
        float visibility = 1f;
        if (lastMoved != null && lastMoved.ContainsKey(playerId))
        {
            var tStill = Time.time - lastMoved[playerId];
            if (tStill > holdDuration)
                if (tStill - holdDuration > fadeDuration) visibility = minVisibility;
                else visibility = (1 - (tStill - holdDuration) / fadeDuration) * (1 - minVisibility) + minVisibility;
        }
        if (PlayerControl.LocalPlayer.Data.IsDead && visibility < 0.1f)
            visibility = 0.1f;
        return visibility;
    }

    public void update()
    {
        foreach (var chameleonPlayer in chameleon)
        {
            //if (chameleonPlayer == Assassin.assassin && Assassin.isInvisble) continue;  // Dont make Assassin visible...
            if (chameleonPlayer == Ninja.ninja && Ninja.stealthed || chameleonPlayer == Sprinter.sprinter && Sprinter.sprinting) continue;
            // check movement by animation
            PlayerPhysics playerPhysics = chameleonPlayer.MyPhysics;
            var currentPhysicsAnim = playerPhysics.Animations.Animator.GetCurrentAnimation();
            if (currentPhysicsAnim != playerPhysics.Animations.group.IdleAnim)
                lastMoved[chameleonPlayer.PlayerId] = Time.time;
            // calculate and set visibility
            float visibility = Chameleon.visibility(chameleonPlayer.PlayerId);
            float petVisibility = visibility;
            if (chameleonPlayer.Data.IsDead)
            {
                visibility = 0.5f;
                petVisibility = 1f;
            }

            try
            {  // Sometimes renderers are missing for weird reasons. Try catch to avoid exceptions
                chameleonPlayer.cosmetics.currentBodySprite.BodySprite.color = chameleonPlayer.cosmetics.currentBodySprite.BodySprite.color.SetAlpha(visibility);
                if (DataManager.Settings.Accessibility.ColorBlindMode) chameleonPlayer.cosmetics.colorBlindText.color = chameleonPlayer.cosmetics.colorBlindText.color.SetAlpha(visibility);
                chameleonPlayer.SetHatAndVisorAlpha(visibility);
                chameleonPlayer.cosmetics.skin.layer.color = chameleonPlayer.cosmetics.skin.layer.color.SetAlpha(visibility);
                chameleonPlayer.cosmetics.nameText.color = chameleonPlayer.cosmetics.nameText.color.SetAlpha(visibility);
                foreach (var rend in chameleonPlayer.cosmetics.currentPet.renderers) rend.color = rend.color.SetAlpha(petVisibility);
                foreach (var shadowRend in chameleonPlayer.cosmetics.currentPet.shadows) shadowRend.color = shadowRend.color.SetAlpha(petVisibility);

                //if (chameleonPlayer.cosmetics.skin.layer.color == chameleonPlayer.cosmetics.skin.layer.color.SetAlpha(visibility) && visibility == minVisibility) TheOtherRolesPlugin.Logger.LogMessage("Chameleon");
                //chameleonPlayer.cosmetics.currentPet.renderers[0].color = chameleonPlayer.cosmetics.currentPet.renderers[0].color.SetAlpha(petVisibility);
                //chameleonPlayer.cosmetics.currentPet.shadows[0].color = chameleonPlayer.cosmetics.currentPet.shadows[0].color.SetAlpha(petVisibility);
            }
            catch { }
        }

    }
}
