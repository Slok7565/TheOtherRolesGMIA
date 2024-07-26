using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Iced.Intel;
using TheOtherRoles.Role;
using TheOtherRoles.TheOtherRoles.Core;
using UnityEngine;

namespace TheOtherRoles.TheOtherRoles.Roles.Modifier;

public sealed class Lovers : ModifierBase
{
    public static Color color = new Color32(232, 57, 185, byte.MaxValue);

    public static bool bothDie = true;
    public static bool enableChat = true;
    // Lovers save if next to be exiled is a lover, because RPC of ending game comes before RPC of exiled
    public static bool notAckedExiledIsLover = false;

    public static bool existing()
    {
        return lover1 != null && lover2 != null && !lover1.Data.Disconnected && !lover2.Data.Disconnected;
    }

    public static bool existingAndAlive()
    {
        return existing() && !lover1.Data.IsDead && !lover2.Data.IsDead && !notAckedExiledIsLover; // ADD NOT ACKED IS LOVER
    }

    public PlayerControl otherLover(PlayerControl oneLover)
    {
        if (!existingAndAlive()) return null;
        if (oneLover == lover1) return lover2;
        if (oneLover == lover2) return lover1;
        return null;
    }

    public bool existingWithKiller()
    {
        return existing() && (lover1 == Jackal.jackal || lover2 == Jackal.jackal
                           || lover1 == Sidekick.sidekick || lover2 == Sidekick.sidekick
                           || lover1.Data.Role.IsImpostor || lover2.Data.Role.IsImpostor);
    }


    public override void clearAndReload()
    {
        lover1 = null;
        lover2 = null;
        notAckedExiledIsLover = false;
        bothDie = CustomOptionHolder.modifierLoverBothDie.getBool();
        enableChat = CustomOptionHolder.modifierLoverEnableChat.getBool();
    }

    public PlayerControl getPartner(PlayerControl player)
    {
        return LoverPair.GetPartner(player);
    }
    
}
public class LoverPair
{
    private static HashSet<LoverPair> allLovers = new();

    public byte Lover1 { get; }
    public byte Lover2 { get; }

    public LoverPair(byte lover1, byte lover2)
    {
        Lover1 = lover1;
        Lover2 = lover2;
        allLovers.Add(this);
    }

    public static bool IsLover(PlayerControl pc)=> IsLover(pc.PlayerId);
    public static bool IsLover(byte playerId)=> allLovers.Any(pair => pair.Lover1 == playerId || pair.Lover2 == playerId);
    

    public static PlayerControl GetPartner(PlayerControl lover)
    {
        foreach (var pair in allLovers)
        {
            if (pair.Lover1 == lover.PlayerId)
                return Helpers.playerById(pair.Lover2);
            else if (pair.Lover2 == lover.PlayerId)
                return Helpers.playerById(pair.Lover1);
        }
        return null;
    }
}
