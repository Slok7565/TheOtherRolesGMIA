using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheOtherRoles.Patches;
using TheOtherRoles.Role;
using TheOtherRoles.TheOtherRoles.Core.Interfaces;
using TheOtherRoles.TheOtherRoles.Core;
using UnityEngine;

namespace TheOtherRoles.Players;

public static class ExtendPlayerControl
{
    public static void setDefaultOutFit(this PlayerControl target, bool enforceNightVisionUpdate = true)
    {
        if (MushroomSabotageActive())
        {
            var instance = ShipStatus.Instance.CastFast<FungleShipStatus>().specialSabotage;
            MushroomMixupSabotageSystem.CondensedOutfit condensedOutfit = instance.currentMixups[target.PlayerId];
            GameData.PlayerOutfit playerOutfit = instance.ConvertToPlayerOutfit(condensedOutfit);
            target.MixUpOutfit(playerOutfit);
        }
        else
            target.setOutFit(target.Data.PlayerName, target.Data.DefaultOutfit.ColorId, target.Data.DefaultOutfit.HatId, target.Data.DefaultOutfit.VisorId, target.Data.DefaultOutfit.SkinId, target.Data.DefaultOutfit.PetId, enforceNightVisionUpdate);
    }

    public static void setOutFit(this PlayerControl target, string playerName, int colorId, string hatId, string visorId, string skinId, string petId, bool enforceNightVisionUpdate = true)
    {
        target.RawSetColor(colorId);
        target.RawSetVisor(visorId, colorId);
        target.RawSetHat(hatId, colorId);
        target.RawSetName(hidePlayerName(CachedPlayer.LocalPlayer.PlayerControl, target) ? "" : playerName);


        SkinViewData nextSkin = null;
        try { nextSkin = ShipStatus.Instance.CosmeticsCache.GetSkin(skinId); } catch { return; };

        PlayerPhysics playerPhysics = target.MyPhysics;
        AnimationClip clip = null;
        var spriteAnim = playerPhysics.myPlayer.cosmetics.skin.animator;
        var currentPhysicsAnim = playerPhysics.Animations.Animator.GetCurrentAnimation();


        if (currentPhysicsAnim == playerPhysics.Animations.group.RunAnim) clip = nextSkin.RunAnim;
        else if (currentPhysicsAnim == playerPhysics.Animations.group.SpawnAnim) clip = nextSkin.SpawnAnim;
        else if (currentPhysicsAnim == playerPhysics.Animations.group.EnterVentAnim) clip = nextSkin.EnterVentAnim;
        else if (currentPhysicsAnim == playerPhysics.Animations.group.ExitVentAnim) clip = nextSkin.ExitVentAnim;
        else if (currentPhysicsAnim == playerPhysics.Animations.group.IdleAnim) clip = nextSkin.IdleAnim;
        else clip = nextSkin.IdleAnim;
        float progress = playerPhysics.Animations.Animator.m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        playerPhysics.myPlayer.cosmetics.skin.skin = nextSkin;
        playerPhysics.myPlayer.cosmetics.skin.UpdateMaterial();

        spriteAnim.Play(clip, 1f);
        spriteAnim.m_animator.Play("a", 0, progress % 1);
        spriteAnim.m_animator.Update(0f);

        target.RawSetPet(petId, colorId);

        if (enforceNightVisionUpdate) SurveillanceMinigamePatch.enforceNightVision(target);
        Chameleon.update();  // so that morphling and camo wont make the chameleons visible
    }
    public static bool isNeutral(this PlayerControl player)
    {
        RoleInfo roleInfo = RoleInfo.getRoleInfoForPlayer(player, false).FirstOrDefault();
        if (roleInfo != null)
            return roleInfo.isNeutral;
        return false;
    }

    public static bool isKiller(this PlayerControl player)
    {
        return player.Data.Role.IsImpostor || player.GetRoleClass() is IKiller;
        //return player.Data.Role.IsImpostor || 
        //    (isNeutral(player) && 
        //    player != Arsonist.arsonist && 
        //    player != Vulture.vulture && 
        //    player != Lawyer.lawyer && 
        //    player != Pursuer.pursuer &&
        //    player != Opportunist.opportunist &&
        //    player != Akujo.akujo &&
        //    player != PlagueDoctor.plagueDoctor &&
        //    player != Cupid.cupid);

    }

    public static bool isEvil(this PlayerControl player)
    {
        return player.Data.Role.IsImpostor || isNeutral(player);
    }
    public static int GetClientId(this PlayerControl control)
    {
        for (int i = 0; i < AmongUsClient.Instance.allClients.Count; i++)
        {
            InnerNet.ClientData data = AmongUsClient.Instance.allClients[i];
            if (data.Character == control)
                return data.Id;
        }
        return -1;
    }
    public static bool hasImpVision(this PlayerControl player) => player.GetRoleClass().HasImpVision();
    public static bool hasImpVision(GameData.PlayerInfo player) => CustomRoleManager.GetByPlayerId(player.PlayerId).HasImpVision();

    public static void setInvisible(this PlayerControl player, Color color)
    {

        if (player.MyPhysics?.myPlayer.cosmetics.currentBodySprite.BodySprite != null)
            player.MyPhysics.myPlayer.cosmetics.currentBodySprite.BodySprite.color = color;

        if (player.MyPhysics?.myPlayer.cosmetics.skin?.layer != null)
            player.MyPhysics.myPlayer.cosmetics.skin.layer.color = color;

        if (player.cosmetics.hat != null)
            player.cosmetics.hat.SpriteColor = color;

        if (player.GetPet() != null)
            player.GetPet().ForEachRenderer(true, (Il2CppSystem.Action<SpriteRenderer>)((render) => render.color = color));

        if (player.cosmetics.visor != null)
            player.cosmetics.visor.Image.color = color;

        if (player.cosmetics.colorBlindText != null)
            player.cosmetics.colorBlindText.color = color;
    }

    //return player.Role.IsImpostor
    //    || ((Jackal.jackal != null && Jackal.jackal.PlayerId == player.PlayerId || Jackal.formerJackals.Any(x => x.PlayerId == player.PlayerId)) && Jackal.hasImpostorVision)
    //    || (Sidekick.sidekick != null && Sidekick.sidekick.PlayerId == player.PlayerId && Sidekick.hasImpostorVision)
    //    || (Spy.spy != null && Spy.spy.PlayerId == player.PlayerId && Spy.hasImpostorVision)
    //    || (Thief.thief != null && Thief.thief.PlayerId == player.PlayerId && Thief.hasImpostorVision)
    //    || (Madmate.madmate.Any(x => x.PlayerId == player.PlayerId) && Madmate.hasImpostorVision)
    //    || (CreatedMadmate.createdMadmate != null && CreatedMadmate.createdMadmate.PlayerId == player.PlayerId && CreatedMadmate.hasImpostorVision)
    //    || (Moriarty.moriarty != null && Moriarty.moriarty.PlayerId == player.PlayerId)
    //    || (JekyllAndHyde.jekyllAndHyde != null && !JekyllAndHyde.isJekyll() && JekyllAndHyde.jekyllAndHyde.PlayerId == player.PlayerId)
    //    || (Fox.fox != null && Fox.fox.PlayerId == player.PlayerId);


}
