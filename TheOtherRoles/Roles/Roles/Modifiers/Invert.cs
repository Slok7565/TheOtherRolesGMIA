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

public sealed class Invert : RoleBase
{
    public List<PlayerControl> invert = new List<PlayerControl>();
    public int meetings = 3;

    public override void clearAndReload()
    {
        invert = new List<PlayerControl>();
        meetings = (int)CustomOptionHolder.modifierInvertDuration.getFloat();
    }
}
