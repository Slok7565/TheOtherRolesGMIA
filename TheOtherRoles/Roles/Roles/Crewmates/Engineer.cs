using System;
using TheOtherRoles.Utilities;
using TheOtherRoles.Players;
using Hazel;
using TheOtherRoles.Objects;
using TheOtherRoles.Roles.Core;
using TheOtherRoles.Roles.Core.Bases;
using TheOtherRoles.Helpers;
using UnityEngine;
using static TheOtherRoles.CustomOption;
using System.Linq;
using TheOtherRoles.Patches;

namespace TheOtherRoles.Roles.Crewmates;
public sealed class Engineer : RoleBase
{
    public PlayerControl engineer;
    public Color color = new Color32(0, 40, 245, byte.MaxValue);
    private Sprite buttonSprite;

    public int remainingFixes = 1;
    public bool highlightForImpostors = true;
    public bool highlightForTeamJackal = true;

    public Sprite getButtonSprite()
    {
        if (buttonSprite) return buttonSprite;
        buttonSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.RepairButton.png", 115f);
        return buttonSprite;
    }

    public override void clearAndReload()
    {
        engineer = null;
        remainingFixes = Mathf.RoundToInt(CustomOptionHolder.engineerNumberOfFixes.getFloat());
        highlightForImpostors = CustomOptionHolder.engineerHighlightForImpostors.getBool();
        highlightForTeamJackal = CustomOptionHolder.engineerHighlightForTeamJackal.getBool();
    }
}

