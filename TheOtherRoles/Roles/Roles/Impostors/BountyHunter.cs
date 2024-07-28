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
public sealed class BountyHunter : RoleBase
{
    public PlayerControl bountyHunter;
    public Color color = Palette.ImpostorRed;

    public Arrow arrow;
    public float bountyDuration = 30f;
    public bool showArrow = true;
    public float bountyKillCooldown = 0f;
    public float punishmentTime = 15f;
    public float arrowUpdateIntervall = 10f;

    public float arrowUpdateTimer = 0f;
    public float bountyUpdateTimer = 0f;
    public PlayerControl bounty;
    public TMPro.TextMeshPro cooldownText;

    public override void clearAndReload()
    {
        arrow = new Arrow(color);
        bountyHunter = null;
        bounty = null;
        arrowUpdateTimer = 0f;
        bountyUpdateTimer = 0f;
        if (arrow != null && arrow.arrow != null) UnityEngine.Object.Destroy(arrow.arrow);
        arrow = null;
        if (cooldownText != null && cooldownText.gameObject != null) UnityEngine.Object.Destroy(cooldownText.gameObject);
        cooldownText = null;
        foreach (PoolablePlayer p in TORMapOptions.playerIcons.Values)
            if (p != null && p.gameObject != null) p.gameObject.SetActive(false);


        bountyDuration = CustomOptionHolder.bountyHunterBountyDuration.getFloat();
        bountyKillCooldown = CustomOptionHolder.bountyHunterReducedCooldown.getFloat();
        punishmentTime = CustomOptionHolder.bountyHunterPunishmentTime.getFloat();
        showArrow = CustomOptionHolder.bountyHunterShowArrow.getBool();
        arrowUpdateIntervall = CustomOptionHolder.bountyHunterArrowUpdateIntervall.getFloat();
    }
}
