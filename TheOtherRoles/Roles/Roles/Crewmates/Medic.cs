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
using static TheOtherRoles.Roles.TheOtherRoles;

namespace TheOtherRoles.Roles.Crewmates;
public sealed class Medic : RoleBase
{
    public PlayerControl medic;
    public PlayerControl shielded;
    public PlayerControl futureShielded;

    public Color color = new Color32(126, 251, 194, byte.MaxValue);
    public bool usedShield;

    public int showShielded = 0;
    public bool showAttemptToShielded = false;
    public bool showAttemptToMedic = false;
    public bool setShieldAfterMeeting = false;
    public bool showShieldAfterMeeting = false;
    public bool meetingAfterShielding = false;

    public Color shieldedColor = new Color32(0, 221, 255, byte.MaxValue);
    public PlayerControl currentTarget;

    private Sprite buttonSprite;
    public Sprite getButtonSprite()
    {
        if (buttonSprite) return buttonSprite;
        buttonSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.ShieldButton.png", 115f);
        return buttonSprite;
    }

    public bool shieldVisible(PlayerControl target)
    {
        bool hasVisibleShield = false;

        bool isMorphedMorphling = target == Morphling.morphling && Morphling.morphTarget != null && Morphling.morphTimer > 0f;
        bool isMimicKShield = target == MimicK.mimicK && MimicK.victim != null;
        bool isMimicAMorph = target == MimicA.mimicA && MimicA.isMorph;
        if (shielded != null && (target == shielded && !isMorphedMorphling && !isMimicKShield && !isMimicAMorph || isMorphedMorphling && Morphling.morphTarget == shielded || isMimicAMorph && MimicK.mimicK == shielded))
        {
            hasVisibleShield = showShielded == 0 || Helpers.shouldShowGhostInfo() // Everyone or Ghost info
                || showShielded == 1 && (CachedPlayer.LocalPlayer.PlayerControl == shielded || CachedPlayer.LocalPlayer.PlayerControl == medic) // Shielded + Medic
                || showShielded == 2 && CachedPlayer.LocalPlayer.PlayerControl == medic; // Medic only
                                                                                         // Make shield invisible till after the next meeting if the option is set (the medic can already see the shield)
            hasVisibleShield = hasVisibleShield && (meetingAfterShielding || !showShieldAfterMeeting || CachedPlayer.LocalPlayer.PlayerControl == medic || Helpers.shouldShowGhostInfo());
        }
        return hasVisibleShield;
    }

    public override void clearAndReload()
    {
        medic = null;
        shielded = null;
        futureShielded = null;
        currentTarget = null;
        usedShield = false;
        showShielded = CustomOptionHolder.medicShowShielded.getSelection();
        showAttemptToShielded = CustomOptionHolder.medicShowAttemptToShielded.getBool();
        showAttemptToMedic = CustomOptionHolder.medicShowAttemptToMedic.getBool();
        setShieldAfterMeeting = CustomOptionHolder.medicSetOrShowShieldAfterMeeting.getSelection() == 2;
        showShieldAfterMeeting = CustomOptionHolder.medicSetOrShowShieldAfterMeeting.getSelection() == 1;
        meetingAfterShielding = false;
    }
}
