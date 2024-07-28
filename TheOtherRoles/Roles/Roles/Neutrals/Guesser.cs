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
public sealed class Guesser : RoleBase
{
    public PlayerControl niceGuesser;
    public PlayerControl evilGuesser;
    public Color color = new Color32(255, 255, 0, byte.MaxValue);

    public int remainingShotsEvilGuesser = 2;
    public int remainingShotsNiceGuesser = 2;

    public bool isGuesser(byte playerId)
    {
        if (niceGuesser != null && niceGuesser.PlayerId == playerId || evilGuesser != null && evilGuesser.PlayerId == playerId) return true;
        return false;
    }

    public void clear(byte playerId)
    {
        if (niceGuesser != null && niceGuesser.PlayerId == playerId) niceGuesser = null;
        else if (evilGuesser != null && evilGuesser.PlayerId == playerId) evilGuesser = null;
    }

    public int remainingShots(byte playerId, bool shoot = false)
    {
        int remainingShots = remainingShotsEvilGuesser;
        if (niceGuesser != null && niceGuesser.PlayerId == playerId)
        {
            remainingShots = remainingShotsNiceGuesser;
            if (shoot) remainingShotsNiceGuesser = Mathf.Max(0, remainingShotsNiceGuesser - 1);
        }
        else if (shoot)
            remainingShotsEvilGuesser = Mathf.Max(0, remainingShotsEvilGuesser - 1);
        return remainingShots;
    }

    public override void clearAndReload()
    {
        niceGuesser = null;
        evilGuesser = null;
        remainingShotsEvilGuesser = Mathf.RoundToInt(CustomOptionHolder.guesserNumberOfShots.getFloat());
        remainingShotsNiceGuesser = Mathf.RoundToInt(CustomOptionHolder.guesserNumberOfShots.getFloat());
    }
}
