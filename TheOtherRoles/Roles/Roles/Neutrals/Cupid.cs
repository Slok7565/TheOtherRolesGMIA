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
using TheOtherRoles.Roles.Modifier;

namespace TheOtherRoles.Roles.Neutral;
public sealed class Cupid : RoleBase
{
    public PlayerControl cupid;
    public Color color = new Color32(246, 152, 150, byte.MaxValue);

    public PlayerControl lovers1;
    public PlayerControl lovers2;
    public PlayerControl shielded;
    public PlayerControl currentTarget;
    public PlayerControl shieldTarget;
    public DateTime startTime = DateTime.UtcNow;
    public bool isShieldOn = false;
    public int timeLeft;
    public float timeLimit;

    private Sprite arrowSprite;
    public Sprite getArrowSprite()
    {
        if (arrowSprite) return arrowSprite;
        arrowSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.CupidButton.png", 115f);
        return arrowSprite;
    }

    public void breakLovers(PlayerControl lover)
    {
        if (Lovers.lover1 != null && lover == Lovers.lover1 || Lovers.lover2 != null && lover == Lovers.lover2)
        {
            PlayerControl otherLover = lover.getPartner();
            if (otherLover != null && !otherLover.Data.IsDead)
            {
                Lovers.clearAndReload();
                otherLover.MurderPlayer(otherLover, MurderResultFlags.Succeeded);
                GameHistory.overrideDeathReasonAndKiller(otherLover, DeadPlayer.CustomDeathReason.LoveStolen);
            }
        }
    }

    public void clearAndReload(bool resetLovers = true)
    {
        cupid = null;
        if (resetLovers)
        {
            lovers1 = null;
            lovers2 = null;
        }
        shielded = null;
        currentTarget = null;
        shieldTarget = null;
        startTime = DateTime.UtcNow;
        timeLimit = CustomOptionHolder.cupidTimeLimit.getFloat() + 10f;
        timeLeft = (int)Math.Ceiling(timeLimit - (DateTime.UtcNow - startTime).TotalSeconds);
        isShieldOn = CustomOptionHolder.cupidShield.getBool();
    }
}
