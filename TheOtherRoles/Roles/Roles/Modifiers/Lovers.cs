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

namespace TheOtherRoles.Roles.Modifier;

public sealed class Lovers : ModifierBase
{
    public static readonly RoleInfo Info =
    RoleInfo.Create(
        typeof(Lovers),
        player => new Lovers(player),
        06_00100,
        new Color32(232, 57, 185, byte.MaxValue),
        CreateOpt,
        RoleId.Lover,
        CustomOptionType.Modifier

);
    public static CustomOption modifierLoverImpLoverRate;
    public static CustomOption modifierLoverBothDie;
    public static CustomOption modifierLoverEnableChat;

    static void CreateOpt()
    {
        modifierLoverImpLoverRate = CustomOption.Create(Info, 10, "loversImpLoverRate", CustomOptionHolder.rates);
        modifierLoverBothDie = CustomOption.Create(Info, 11, "loversBothDie", true);
        modifierLoverEnableChat = CustomOption.Create(Info, 12, "loversEnableChat", true);


    }
    public Lovers(PlayerControl player)
    : base(Info, player)
    {  }
    public List<byte> LoverList;
    public static bool bothDie = true;
    public static bool enableChat = true;
    // Lovers save if next to be exiled is a lover, because RPC of ending game comes before RPC of exiled
    public bool notAckedExiledIsLover = false;


    public bool existingAndAlive()
    {
        return LoverList.All(p => !PlayerHelper.playerById(p).Data.IsDead) && !notAckedExiledIsLover; // ADD NOT ACKED IS LOVER
    }

    

    public bool existingWithKiller()
    {
        return LoverList.Any(p => PlayerHelper.playerById(p).GetRoleClass() is Jackal or Sidekick
        || PlayerHelper.playerById(p).Data.Role.IsImpostor);
    }


    public override void clearAndReload()
    {
        notAckedExiledIsLover = false;
        bothDie = modifierLoverBothDie.getBool();
        enableChat = modifierLoverEnableChat.getBool();
    }
    public PlayerControl getPartner(PlayerControl oneLover)
    {
        if (!existingAndAlive()) return null;
        foreach (var id in LoverList.Where(p => PlayerHelper.playerById(p) != oneLover))
            return PlayerHelper.playerById(id);
        return null;
    }
    public bool isPartner(PlayerControl target) => getPartner(Player) == target;

    public override bool HidePlayerName(PlayerControl target) => !isPartner(target);
}

