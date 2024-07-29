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
public sealed class Pursuer : RoleBase
{
    public PlayerControl pursuer;
    public PlayerControl target;
    public Color color = Lawyer.color;
    public List<PlayerControl> blankedList = new List<PlayerControl>();
    public int blanks = 0;
    public Sprite blank;
    public bool notAckedExiled = false;

    public float cooldown = 30f;
    public int blanksNumber = 5;

    public Sprite getTargetSprite()
    {
        if (blank) return blank;
        blank = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.PursuerButton.png", 115f);
        return blank;
    }

    public override void clearAndReload()
    {
        pursuer = null;
        target = null;
        blankedList = new List<PlayerControl>();
        blanks = 0;
        notAckedExiled = false;

        cooldown = CustomOptionHolder.pursuerCooldown.getFloat();
        blanksNumber = Mathf.RoundToInt(CustomOptionHolder.pursuerBlanksNumber.getFloat());
    }
}
