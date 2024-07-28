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
public sealed class Lighter : RoleBase
{
    public PlayerControl lighter;
    public Color color = new Color32(238, 229, 190, byte.MaxValue);

    public float lighterModeLightsOnVision = 2f;
    public float lighterModeLightsOffVision = 0.75f;
    public float flashlightWidth = 0.75f;
    public bool canSeeInvisible = true;

    public override void clearAndReload()
    {
        lighter = null;
        flashlightWidth = CustomOptionHolder.lighterFlashlightWidth.getFloat();
        lighterModeLightsOnVision = CustomOptionHolder.lighterModeLightsOnVision.getFloat();
        lighterModeLightsOffVision = CustomOptionHolder.lighterModeLightsOffVision.getFloat();
        canSeeInvisible = CustomOptionHolder.lighterCanSeeInvisible.getBool();
    }
}
