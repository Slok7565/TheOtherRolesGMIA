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
public sealed class TimeMaster : RoleBase
{
    public PlayerControl timeMaster;
    public Color color = new Color32(112, 142, 239, byte.MaxValue);

    public bool reviveDuringRewind = false;
    public float rewindTime = 3f;
    public float shieldDuration = 3f;
    public float cooldown = 30f;

    public bool shieldActive = false;
    public bool isRewinding = false;

    private Sprite buttonSprite;
    public Sprite getButtonSprite()
    {
        if (buttonSprite) return buttonSprite;
        buttonSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.TimeShieldButton.png", 115f);
        return buttonSprite;
    }

    public override void clearAndReload()
    {
        timeMaster = null;
        isRewinding = false;
        shieldActive = false;
        rewindTime = CustomOptionHolder.timeMasterRewindTime.getFloat();
        shieldDuration = CustomOptionHolder.timeMasterShieldDuration.getFloat();
        cooldown = CustomOptionHolder.timeMasterCooldown.getFloat();
    }
}
