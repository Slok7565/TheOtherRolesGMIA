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
using System.Collections.Generic;

namespace TheOtherRoles.Roles.Crewmates;
public sealed class Seer : RoleBase
{
    public PlayerControl seer;
    public Color color = new Color32(97, 178, 108, byte.MaxValue);
    public List<Vector3> deadBodyPositions = new List<Vector3>();

    public float soulDuration = 15f;
    public bool limitSoulDuration = false;
    public int mode = 0;

    private Sprite soulSprite;
    public Sprite getSoulSprite()
    {
        if (soulSprite) return soulSprite;
        soulSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.Soul.png", 500f);
        return soulSprite;
    }

    public override void clearAndReload()
    {
        seer = null;
        deadBodyPositions = new List<Vector3>();
        limitSoulDuration = CustomOptionHolder.seerLimitSoulDuration.getBool();
        soulDuration = CustomOptionHolder.seerSoulDuration.getFloat();
        mode = CustomOptionHolder.seerMode.getSelection();
    }
}
