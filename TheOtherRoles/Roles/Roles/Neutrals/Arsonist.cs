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
public sealed class Arsonist : RoleBase
{
    public PlayerControl arsonist;
    public Color color = new Color32(238, 112, 46, byte.MaxValue);

    public float cooldown = 30f;
    public float duration = 3f;
    public bool triggerArsonistWin = false;

    public PlayerControl currentTarget;
    public PlayerControl douseTarget;
    public List<PlayerControl> dousedPlayers = new List<PlayerControl>();

    private Sprite douseSprite;
    public Sprite getDouseSprite()
    {
        if (douseSprite) return douseSprite;
        douseSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.DouseButton.png", 115f);
        return douseSprite;
    }

    private Sprite igniteSprite;
    public Sprite getIgniteSprite()
    {
        if (igniteSprite) return igniteSprite;
        igniteSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.IgniteButton.png", 115f);
        return igniteSprite;
    }

    public bool dousedEveryoneAlive()
    {
        return CachedPlayer.AllPlayers.All(x => { return x.PlayerControl == Arsonist.arsonist || x.Data.IsDead || x.Data.Disconnected || Arsonist.dousedPlayers.Any(y => y.PlayerId == x.PlayerId); });
    }

    public override void clearAndReload()
    {
        arsonist = null;
        currentTarget = null;
        douseTarget = null;
        triggerArsonistWin = false;
        dousedPlayers = new List<PlayerControl>();
        foreach (PoolablePlayer p in TORMapOptions.playerIcons.Values)
            if (p != null && p.gameObject != null) p.gameObject.SetActive(false);
        cooldown = CustomOptionHolder.arsonistCooldown.getFloat();
        duration = CustomOptionHolder.arsonistDuration.getFloat();
    }
}
