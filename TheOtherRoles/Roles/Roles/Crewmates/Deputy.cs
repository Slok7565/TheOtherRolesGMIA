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
using System.Collections.Generic;

namespace TheOtherRoles.Roles.Crewmates;
public sealed class Deputy : RoleBase
{
    public PlayerControl deputy;
    public Color color = Sheriff.color;

    public PlayerControl currentTarget;
    public List<byte> handcuffedPlayers = new List<byte>();
    public int promotesToSheriff; // No: 0, Immediately: 1, After Meeting: 2
    public bool keepsHandcuffsOnPromotion;
    public float handcuffDuration;
    public float remainingHandcuffs;
    public float handcuffCooldown;
    public bool knowsSheriff;
    public bool stopsGameEnd;
    public Dictionary<byte, float> handcuffedKnows = new Dictionary<byte, float>();

    private Sprite buttonSprite;
    private Sprite handcuffedSprite;

    public Sprite getButtonSprite()
    {
        if (buttonSprite) return buttonSprite;
        buttonSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.DeputyHandcuffButton.png", 115f);
        return buttonSprite;
    }

    public Sprite getHandcuffedButtonSprite()
    {
        if (handcuffedSprite) return handcuffedSprite;
        handcuffedSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.DeputyHandcuffed.png", 115f);
        return handcuffedSprite;
    }

    // Can be used to enable / disable the handcuff effect on the target's buttons
    public void setHandcuffedKnows(bool active = true, byte playerId = byte.MaxValue)
    {
        if (playerId == byte.MaxValue)
            playerId = CachedPlayer.LocalPlayer.PlayerId;

        if (active && playerId == CachedPlayer.LocalPlayer.PlayerId)
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.ShareGhostInfo, SendOption.Reliable, -1);
            writer.Write(CachedPlayer.LocalPlayer.PlayerId);
            writer.Write((byte)RPCProcedure.GhostInfoTypes.HandcuffNoticed);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        if (active)
        {
            handcuffedKnows.Add(playerId, handcuffDuration);
            handcuffedPlayers.RemoveAll(x => x == playerId);
        }

        if (playerId == CachedPlayer.LocalPlayer.PlayerId)
        {
            HudManagerStartPatch.setAllButtonsHandcuffedStatus(active);
            SoundEffectsManager.play("deputyHandcuff");
        }

    }

    public void clearAndReload()
    {
        deputy = null;
        currentTarget = null;
        handcuffedPlayers = new List<byte>();
        handcuffedKnows = new Dictionary<byte, float>();
        HudManagerStartPatch.setAllButtonsHandcuffedStatus(false, true);
        promotesToSheriff = CustomOptionHolder.deputyGetsPromoted.getSelection();
        remainingHandcuffs = CustomOptionHolder.deputyNumberOfHandcuffs.getFloat();
        handcuffCooldown = CustomOptionHolder.deputyHandcuffCooldown.getFloat();
        keepsHandcuffsOnPromotion = CustomOptionHolder.deputyKeepsHandcuffs.getBool();
        handcuffDuration = CustomOptionHolder.deputyHandcuffDuration.getFloat();
        knowsSheriff = CustomOptionHolder.deputyKnowsSheriff.getBool();
        stopsGameEnd = CustomOptionHolder.deputyStopsGameEnd.getBool();
    }
}
