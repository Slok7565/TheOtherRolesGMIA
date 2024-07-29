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
public sealed class Vulture : RoleBase
{
    public PlayerControl vulture;
    public Color color = new Color32(139, 69, 19, byte.MaxValue);
    public List<Arrow> localArrows = new List<Arrow>();
    public float cooldown = 30f;
    public int vultureNumberToWin = 4;
    public int eatenBodies = 0;
    public bool triggerVultureWin = false;
    public bool canUseVents = true;
    public bool showArrows = true;
    private Sprite buttonSprite;
    public Sprite getButtonSprite()
    {
        if (buttonSprite) return buttonSprite;
        buttonSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.VultureButton.png", 115f);
        return buttonSprite;
    }

    public override void clearAndReload()
    {
        vulture = null;
        vultureNumberToWin = Mathf.RoundToInt(CustomOptionHolder.vultureNumberToWin.getFloat());
        eatenBodies = 0;
        cooldown = CustomOptionHolder.vultureCooldown.getFloat();
        triggerVultureWin = false;
        canUseVents = CustomOptionHolder.vultureCanUseVents.getBool();
        showArrows = CustomOptionHolder.vultureShowArrows.getBool();
        if (localArrows != null)
            foreach (Arrow arrow in localArrows)
                if (arrow?.arrow != null)
                    UnityEngine.Object.Destroy(arrow.arrow);
        localArrows = new List<Arrow>();
    }
}
