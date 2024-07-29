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

public sealed class AntiTeleport : RoleBase
{
    public List<PlayerControl> antiTeleport = new List<PlayerControl>();
    public Vector3 position;

    public override void clearAndReload()
    {
        antiTeleport = new List<PlayerControl>();
        position = Vector3.zero;
    }

    public void setPosition()
    {
        if (position == Vector3.zero) return;  // Check if this has been set, otherwise first spawn on submerged will fail
        if (antiTeleport.FindAll(x => x.PlayerId == CachedPlayer.LocalPlayer.PlayerId).Count > 0)
        {
            CachedPlayer.LocalPlayer.NetTransform.RpcSnapTo(position);
            if (SubmergedCompatibility.IsSubmerged)
                SubmergedCompatibility.ChangeFloor(position.y > -7);
        }
    }
}
