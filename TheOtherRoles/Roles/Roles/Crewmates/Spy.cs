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
public sealed class Spy : RoleBase
{
    public PlayerControl spy;
    public Color color = Palette.ImpostorRed;

    public bool impostorsCanKillAnyone = true;
    public bool canEnterVents = false;
    public bool hasImpostorVision = false;

    public override void clearAndReload()
    {
        spy = null;
        impostorsCanKillAnyone = CustomOptionHolder.spyImpostorsCanKillAnyone.getBool();
        canEnterVents = CustomOptionHolder.spyCanEnterVents.getBool();
        hasImpostorVision = CustomOptionHolder.spyHasImpostorVision.getBool();
    }
}
