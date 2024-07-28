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

namespace TheOtherRoles.Roles.Neutral;
public sealed class Lawyer : RoleBase
{
    public PlayerControl lawyer;
    public PlayerControl target;
    public Color color = new Color32(134, 153, 25, byte.MaxValue);
    public Sprite targetSprite;
    //public  bool triggerProsecutorWin = false;
    //public  bool isProsecutor = false;
    public bool targetKnows = true;
    public bool triggerLawyerWin = false;
    public int meetings = 0;

    public bool winsAfterMeetings = false;
    public int neededMeetings = 4;
    public float vision = 1f;
    public bool lawyerTargetKnows = true;
    public bool lawyerKnowsRole = false;
    public bool targetCanBeJester = false;
    public bool targetWasGuessed = false;

    public Sprite getTargetSprite()
    {
        if (targetSprite) return targetSprite;
        targetSprite = ResourcesHelper.loadSpriteFromResources("", 150f);
        return targetSprite;
    }

    public void clearAndReload(bool clearTarget = true)
    {
        lawyer = null;
        if (clearTarget)
        {
            target = null;
            targetWasGuessed = false;
        }
        triggerLawyerWin = false;
        meetings = 0;
        //isProsecutor = false;
        //triggerProsecutorWin = false;
        vision = CustomOptionHolder.lawyerVision.getFloat();
        lawyerKnowsRole = CustomOptionHolder.lawyerKnowsRole.getBool();
        lawyerTargetKnows = CustomOptionHolder.lawyerTargetKnows.getBool();
        targetCanBeJester = CustomOptionHolder.lawyerTargetCanBeJester.getBool();
        winsAfterMeetings = CustomOptionHolder.lawyerWinsAfterMeetings.getBool();
        neededMeetings = Mathf.RoundToInt(CustomOptionHolder.lawyerNeededMeetings.getFloat());
        targetKnows = CustomOptionHolder.lawyerTargetKnows.getBool();
    }
}
