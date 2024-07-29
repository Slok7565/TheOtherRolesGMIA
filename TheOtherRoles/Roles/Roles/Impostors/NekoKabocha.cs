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

namespace TheOtherRoles.Roles.Impostor;
public sealed class NekoKabocha : RoleBase
{
    public PlayerControl nekoKabocha;
    public Color color = Palette.ImpostorRed;

    public bool revengeCrew = true;
    public bool revengeImpostor = true;
    public bool revengeNeutral = true;
    public bool revengeExile = false;

    public PlayerControl meetingKiller = null;
    public PlayerControl otherKiller;

    public void clearAndReload()
    {
        nekoKabocha = null;
        meetingKiller = null;
        otherKiller = null;
        revengeCrew = CustomOptionHolder.nekoKabochaRevengeCrew.getBool();
        revengeImpostor = CustomOptionHolder.nekoKabochaRevengeImpostor.getBool();
        revengeNeutral = CustomOptionHolder.nekoKabochaRevengeNeutral.getBool();
        revengeExile = CustomOptionHolder.nekoKabochaRevengeExile.getBool();
    }
}
