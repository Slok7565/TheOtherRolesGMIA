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
public sealed class Sheriff : RoleBase
{
    public PlayerControl sheriff;
    public Color color = new Color32(248, 205, 70, byte.MaxValue);

    public float cooldown = 30f;
    public bool canKillNeutrals = false;
    public bool spyCanDieToSheriff = false;

    public PlayerControl currentTarget;

    public PlayerControl formerDeputy;  // Needed for keeping handcuffs + shifting
    public PlayerControl formerSheriff;  // When deputy gets promoted...

    public void replaceCurrentSheriff(PlayerControl deputy)
    {
        if (!formerSheriff) formerSheriff = sheriff;
        sheriff = deputy;
        currentTarget = null;
        cooldown = CustomOptionHolder.sheriffCooldown.getFloat();
    }

    public override void clearAndReload()
    {
        sheriff = null;
        currentTarget = null;
        formerDeputy = null;
        formerSheriff = null;
        cooldown = CustomOptionHolder.sheriffCooldown.getFloat();
        canKillNeutrals = CustomOptionHolder.sheriffCanKillNeutrals.getBool();
        spyCanDieToSheriff = CustomOptionHolder.spyCanDieToSheriff.getBool();
    }
}
