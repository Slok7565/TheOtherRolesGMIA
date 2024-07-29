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
public sealed class Yasuna : RoleBase
{
    public PlayerControl yasuna;
    public Color color = new Color32(90, 255, 25, byte.MaxValue);
    public byte specialVoteTargetPlayerId = byte.MaxValue;
    private int _remainingSpecialVotes = 1;
    private Sprite targetSprite;

    public void clearAndReload()
    {
        yasuna = null;
        _remainingSpecialVotes = Mathf.RoundToInt(CustomOptionHolder.yasunaNumberOfSpecialVotes.getFloat());
        specialVoteTargetPlayerId = byte.MaxValue;
    }

    public Sprite getTargetSprite(bool isImpostor)
    {
        if (targetSprite) return targetSprite;
        targetSprite = ResourcesHelper.loadSpriteFromResources(isImpostor ? "TheOtherRoles.Resources.EvilYasunaTargetIcon.png" : "TheOtherRoles.Resources.YasunaTargetIcon.png", 150f);
        return targetSprite;
    }

    public int remainingSpecialVotes(bool isVote = false)
    {
        if (yasuna == null)
            return 0;

        if (isVote)
            _remainingSpecialVotes = Mathf.Max(0, _remainingSpecialVotes - 1);
        return _remainingSpecialVotes;
    }

    public bool isYasuna(byte playerId)
    {
        return yasuna != null && yasuna.PlayerId == playerId;
    }
}
