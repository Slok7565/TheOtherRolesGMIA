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
public sealed class Watcher : RoleBase
{
    public PlayerControl nicewatcher;
    public PlayerControl evilwatcher;
    public Color color = Palette.Purple;

    public void clear(byte playerId)
    {
        if (nicewatcher != null && nicewatcher.PlayerId == playerId) nicewatcher = null;
        else if (evilwatcher != null && evilwatcher.PlayerId == playerId) evilwatcher = null;
    }

    public void clearAndReload()
    {
        nicewatcher = null;
        evilwatcher = null;
    }
}
