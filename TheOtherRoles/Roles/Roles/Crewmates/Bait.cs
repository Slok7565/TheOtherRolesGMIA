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

public sealed class Bait : RoleBase
{
    public PlayerControl bait;
    public Color color = new Color32(0, 247, 255, byte.MaxValue);

    public bool highlightAllVents = false;
    public float reportDelay = 0f;
    public bool showKillFlash = true;

    public bool reported = false;

    public void clearAndReload()
    {
        bait = null;
        reported = false;
        highlightAllVents = CustomOptionHolder.baitHighlightAllVents.getBool();
        reportDelay = CustomOptionHolder.baitReportDelay.getFloat();
        showKillFlash = CustomOptionHolder.baitShowKillFlash.getBool();
    }
}
