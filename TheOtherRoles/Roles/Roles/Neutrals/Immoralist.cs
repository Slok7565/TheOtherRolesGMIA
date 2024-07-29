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
public sealed class Immoralist : RoleBase
{
    public PlayerControl immoralist;
    public Color color = Fox.color;

    public List<Arrow> arrows = new();
    public float updateTimer = 0f;
    public float arrowUpdateInterval = 1f;

    private Sprite buttonSprite;
    public Sprite getButtonSprite()
    {
        if (buttonSprite) return buttonSprite;
        buttonSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.SuicideButton.png", 115f);
        return buttonSprite;
    }

    public void arrowUpdate()
    {
        updateTimer -= Time.fixedDeltaTime;

        if (updateTimer <= 0.0f)
        {
            foreach (Arrow arrow in arrows)
                if (arrow?.arrow != null)
                {
                    arrow.arrow.SetActive(false);
                    UnityEngine.Object.Destroy(arrow.arrow);
                }

            arrows = new List<Arrow>();

            foreach (PlayerControl p in CachedPlayer.AllPlayers)
            {
                if (p.Data.IsDead) continue;
                Arrow arrow;
                if (p == Fox.fox)
                {
                    arrow = new Arrow(Fox.color);
                    arrow.arrow.SetActive(true);
                    arrow.Update(p.transform.position);
                    arrows.Add(arrow);
                }
            }
            updateTimer = arrowUpdateInterval;
        }
        else
            arrows.Do(x => x.Update());
    }

    public void clearAndReload()
    {
        immoralist = null;
        foreach (Arrow arrow in arrows)
            if (arrow?.arrow != null)
            {
                arrow.arrow.SetActive(false);
                UnityEngine.Object.Destroy(arrow.arrow);
            }
        arrows = new List<Arrow>();
    }
}
