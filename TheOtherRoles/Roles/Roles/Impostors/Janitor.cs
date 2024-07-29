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
public sealed class Janitor : RoleBase
{
    public PlayerControl janitor;
    public Color color = Palette.ImpostorRed;

    public float cooldown = 30f;

    private Sprite buttonSprite;
    public Sprite getButtonSprite()
    {
        if (buttonSprite) return buttonSprite;
        buttonSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.CleanButton.png", 115f);
        return buttonSprite;
    }

    public override void clearAndReload()
    {
        janitor = null;
        cooldown = CustomOptionHolder.janitorCooldown.getFloat();
    }
}
