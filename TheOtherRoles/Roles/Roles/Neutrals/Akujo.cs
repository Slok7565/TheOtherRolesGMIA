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
public sealed class Akujo : RoleBase
{
    public Color color = new Color32(142, 69, 147, byte.MaxValue);
    public PlayerControl akujo;
    public PlayerControl honmei;
    public List<PlayerControl> keeps;
    public PlayerControl currentTarget;
    public DateTime startTime;

    public float timeLimit = 1300f;
    public bool knowsRoles = true;
    public int timeLeft;
    public int keepsLeft;
    public int numKeeps;

    private Sprite honmeiSprite;
    public Sprite getHonmeiSprite()
    {
        if (honmeiSprite) return honmeiSprite;
        honmeiSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.AkujoHonmeiButton.png", 115f);
        return honmeiSprite;
    }

    private Sprite keepSprite;
    public Sprite getKeepSprite()
    {
        if (keepSprite) return keepSprite;
        keepSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.AkujoKeepButton.png", 115f);
        return keepSprite;
    }

    public void breakLovers(PlayerControl lover)
    {
        if (Lovers.lover1 != null && lover == Lovers.lover1 || Lovers.lover2 != null && lover == Lovers.lover2)
        {
            PlayerControl otherLover = lover.getPartner();
            if (otherLover != null)
            {
                Lovers.clearAndReload();
                otherLover.MurderPlayer(otherLover, MurderResultFlags.Succeeded);
                GameHistory.overrideDeathReasonAndKiller(otherLover, DeadPlayer.CustomDeathReason.LoveStolen);
            }
        }
    }

    public void clearAndReload()
    {
        akujo = null;
        honmei = null;
        keeps = new List<PlayerControl>();
        currentTarget = null;
        startTime = DateTime.UtcNow;
        timeLimit = CustomOptionHolder.akujoTimeLimit.getFloat() + 10f;
        knowsRoles = CustomOptionHolder.akujoKnowsRoles.getBool();
        timeLeft = (int)Math.Ceiling(timeLimit - (DateTime.UtcNow - startTime).TotalSeconds);
        numKeeps = Math.Min((int)CustomOptionHolder.akujoNumKeeps.getFloat(), PlayerControl.AllPlayerControls.Count - 2);
        keepsLeft = numKeeps;
    }
}
