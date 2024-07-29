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
public sealed class Assassin : RoleBase
{
    public PlayerControl assassin;
    public Color color = Palette.ImpostorRed;

    public PlayerControl assassinMarked;
    public PlayerControl currentTarget;
    public float cooldown = 30f;
    public float traceTime = 1f;
    public bool knowsTargetLocation = false;
    //public  float invisibleDuration = 5f;

    //public  float invisibleTimer = 0f;
    //public  bool isInvisble = false;
    private Sprite markButtonSprite;
    private Sprite killButtonSprite;
    public Arrow arrow = new Arrow(Color.black);
    public Sprite getMarkButtonSprite()
    {
        if (markButtonSprite) return markButtonSprite;
        markButtonSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.AssassinMarkButton.png", 115f);
        return markButtonSprite;
    }

    public Sprite getKillButtonSprite()
    {
        if (killButtonSprite) return killButtonSprite;
        killButtonSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.AssassinAssassinateButton.png", 115f);
        return killButtonSprite;
    }

    public override void clearAndReload()
    {
        assassin = null;
        currentTarget = assassinMarked = null;
        cooldown = CustomOptionHolder.assassinCooldown.getFloat();
        knowsTargetLocation = CustomOptionHolder.assassinKnowsTargetLocation.getBool();
        traceTime = CustomOptionHolder.assassinTraceTime.getFloat();
        //invisibleDuration = CustomOptionHolder.assassinInvisibleDuration.getFloat();
        //invisibleTimer = 0f;
        //isInvisble = false;
        if (arrow?.arrow != null) UnityEngine.Object.Destroy(arrow.arrow);
        arrow = new Arrow(Color.black);
        if (arrow.arrow != null) arrow.arrow.SetActive(false);
    }
}
