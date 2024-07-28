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

public sealed class Sunglasses : RoleBase
{
    public List<PlayerControl> sunglasses = new List<PlayerControl>();
    public int vision = 1;

    public override void clearAndReload()
    {
        sunglasses = new List<PlayerControl>();
        vision = CustomOptionHolder.modifierSunglassesVision.getSelection() + 1;
    }
}
