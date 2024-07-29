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
public sealed class Tracker : RoleBase
{
    public PlayerControl tracker;
    public Color color = new Color32(100, 58, 220, byte.MaxValue);
    public List<Arrow> localArrows = new List<Arrow>();

    public float updateIntervall = 5f;
    public bool resetTargetAfterMeeting = false;
    public bool canTrackCorpses = false;
    public float corpsesTrackingCooldown = 30f;
    public float corpsesTrackingDuration = 5f;
    public float corpsesTrackingTimer = 0f;
    public int trackingMode = 0;
    public List<Vector3> deadBodyPositions = new List<Vector3>();

    public PlayerControl currentTarget;
    public PlayerControl tracked;
    public bool usedTracker = false;
    public float timeUntilUpdate = 0f;
    public Arrow arrow = new Arrow(Color.blue);

    public GameObject DangerMeterParent;
    public DangerMeter Meter;

    private Sprite trackCorpsesButtonSprite;
    public Sprite getTrackCorpsesButtonSprite()
    {
        if (trackCorpsesButtonSprite) return trackCorpsesButtonSprite;
        trackCorpsesButtonSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.PathfindButton.png", 115f);
        return trackCorpsesButtonSprite;
    }

    private Sprite buttonSprite;
    public Sprite getButtonSprite()
    {
        if (buttonSprite) return buttonSprite;
        buttonSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.TrackerButton.png", 115f);
        return buttonSprite;
    }

    public void resetTracked()
    {
        currentTarget = tracked = null;
        timeUntilUpdate = 0f;
        usedTracker = false;
        if (arrow?.arrow != null) UnityEngine.Object.Destroy(arrow.arrow);
        arrow = new Arrow(Color.blue);
        if (arrow.arrow != null) arrow.arrow.SetActive(false);
        if (DangerMeterParent)
        {
            Meter.gameObject.Destroy();
            DangerMeterParent.Destroy();
        }
    }

    public override void clearAndReload()
    {
        tracker = null;
        resetTracked();
        timeUntilUpdate = 0f;
        updateIntervall = CustomOptionHolder.trackerUpdateIntervall.getFloat();
        resetTargetAfterMeeting = CustomOptionHolder.trackerResetTargetAfterMeeting.getBool();
        if (localArrows != null)
            foreach (Arrow arrow in localArrows)
                if (arrow?.arrow != null)
                    UnityEngine.Object.Destroy(arrow.arrow);
        deadBodyPositions = new List<Vector3>();
        corpsesTrackingTimer = 0f;
        corpsesTrackingCooldown = CustomOptionHolder.trackerCorpsesTrackingCooldown.getFloat();
        corpsesTrackingDuration = CustomOptionHolder.trackerCorpsesTrackingDuration.getFloat();
        canTrackCorpses = CustomOptionHolder.trackerCanTrackCorpses.getBool();
        trackingMode = CustomOptionHolder.trackerTrackingMethod.getSelection();
    }
}
