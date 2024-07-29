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
public sealed class Detective : RoleBase
{
    public PlayerControl detective;
    public Color color = new Color32(45, 106, 165, byte.MaxValue);

    public float footprintIntervall = 1f;
    public float footprintDuration = 1f;
    public bool anonymousFootprints = false;
    public float reportNameDuration = 0f;
    public float reportColorDuration = 20f;
    public float timer = 6.2f;

    public override void clearAndReload()
    {
        detective = null;
        anonymousFootprints = CustomOptionHolder.detectiveAnonymousFootprints.getBool();
        footprintIntervall = CustomOptionHolder.detectiveFootprintIntervall.getFloat();
        footprintDuration = CustomOptionHolder.detectiveFootprintDuration.getFloat();
        reportNameDuration = CustomOptionHolder.detectiveReportNameDuration.getFloat();
        reportColorDuration = CustomOptionHolder.detectiveReportColorDuration.getFloat();
        timer = 6.2f;
    }
}
    }
