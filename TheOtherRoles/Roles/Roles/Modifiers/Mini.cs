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

namespace TheOtherRoles.Roles.Modifier;

public sealed class Mini : RoleBase
{
    public PlayerControl mini;
    public Color color = Color.yellow;
    public const float defaultColliderRadius = 0.2233912f;
    public const float defaultColliderOffset = 0.3636057f;

    public float growingUpDuration = 400f;
    public bool isGrowingUpInMeeting = true;
    public DateTime timeOfGrowthStart = DateTime.UtcNow;
    public DateTime timeOfMeetingStart = DateTime.UtcNow;
    public float ageOnMeetingStart = 0f;
    public bool triggerMiniLose = false;

    public override void clearAndReload()
    {
        mini = null;
        triggerMiniLose = false;
        growingUpDuration = CustomOptionHolder.modifierMiniGrowingUpDuration.getFloat();
        isGrowingUpInMeeting = CustomOptionHolder.modifierMiniGrowingUpInMeeting.getBool();
        timeOfGrowthStart = DateTime.UtcNow;
    }

    public float growingProgress()
    {
        float timeSinceStart = (float)(DateTime.UtcNow - timeOfGrowthStart).TotalMilliseconds;
        return Mathf.Clamp(timeSinceStart / (growingUpDuration * 1000), 0f, 1f);
    }

    public bool isGrownUp()
    {
        return growingProgress() == 1f;
    }

}
