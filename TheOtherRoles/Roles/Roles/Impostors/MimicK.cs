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
using static TheOtherRoles.Roles.TheOtherRoles;

namespace TheOtherRoles.Roles.Impostor;
public sealed class MimicK : RoleBase
{
    public PlayerControl mimicK;
    public Color color = Palette.ImpostorRed;

    public bool ifOneDiesBothDie = true;
    public bool hasOneVote = true;
    public bool countAsOne = true;

    public string name = "";

    public List<Arrow> arrows = new();
    public float updateTimer = 0f;
    public float arrowUpdateInterval = 0.5f;

    public PlayerControl victim;

    public void arrowUpdate()
    {
        //if (MimicK.mimicK == null || MimicA.mimicA == null) return;
        if (arrows.FirstOrDefault()?.arrow != null)
            if (mimicK == null || MimicA.mimicA == null)
            {
                foreach (Arrow arrows in arrows) arrows.arrow.SetActive(false);
                return;
            }
        if (CachedPlayer.LocalPlayer.PlayerControl != mimicK || mimicK == null) return;
        if (mimicK.Data.IsDead)
        {
            if (arrows.FirstOrDefault().arrow != null) UnityEngine.Object.Destroy(arrows.FirstOrDefault().arrow);
            return;
        }
        updateTimer -= Time.fixedDeltaTime;

        if (updateTimer <= 0.0f)
        {

            foreach (Arrow arrow1 in arrows)
                if (arrow1 != null && arrow1.arrow != null)
                {
                    arrow1.arrow.SetActive(false);
                    UnityEngine.Object.Destroy(arrow1.arrow);
                }

            //if (MimicK.mimicK == null) return;

            arrows = new List<Arrow>();

            /*foreach (PlayerControl p in CachedPlayer.AllPlayers)
            {
                if (p.Data.IsDead) continue;
                Arrow arrow;
                if (p == MimicA.mimicA)
                {
                    arrow = MimicA.isMorph ? new Arrow(Palette.White) : new Arrow(Palette.ImpostorRed);
                    arrow.arrow.SetActive(true);
                    arrow.Update(p.transform.position);
                    arrows.Add(arrow);
                }
            }*/

            if (MimicA.mimicA.Data.IsDead || MimicA.mimicA == null) return;
            Arrow arrow;
            arrow = MimicA.isMorph ? new Arrow(Palette.White) : new Arrow(Palette.ImpostorRed);
            arrow.arrow.SetActive(true);
            arrow.Update(MimicA.mimicA.transform.position);
            arrows.Add(arrow);

            updateTimer = arrowUpdateInterval;
        }
    }

    public void clearAndReload()
    {
        mimicK?.setDefaultOutFit();
        if (MimicA.mimicA != null)
        {
            MimicA.isMorph = false;
            MimicA.mimicA.setDefaultOutFit();
        }

        mimicK = null;
        victim = null;
        ifOneDiesBothDie = CustomOptionHolder.mimicIfOneDiesBothDie.getBool();
        hasOneVote = CustomOptionHolder.mimicHasOneVote.getBool();
        countAsOne = CustomOptionHolder.mimicCountAsOne.getBool();

        if (arrows != null)
            foreach (Arrow arrow in arrows)
                if (arrow?.arrow != null)
                    UnityEngine.Object.Destroy(arrow.arrow);
        arrows = new List<Arrow>();
    }
}
