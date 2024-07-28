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
public sealed class Catcher : RoleBase
{
    public PlayerControl catcher;
    public Color color = new Color32(71, 95, 250, byte.MaxValue);
    public PlayerControl neartarget;
    public PlayerControl target;
    private Sprite CatchButton;
    public float catchchance;
    public float catchcooldown;

    public Sprite getCheckSprite()
    {
        if (CatchButton) return CatchButton;
        CatchButton = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.SwapperCheck.png", 150f);
        return CatchButton;
    }

    public void clearAndReload()
    {
        catcher = null;
        target = null;
        neartarget = null;
        catchchance = CustomOptionHolder.catchChance.getFloat();
        catchcooldown = CustomOptionHolder.catchCooldown.getFloat();
    }
}
