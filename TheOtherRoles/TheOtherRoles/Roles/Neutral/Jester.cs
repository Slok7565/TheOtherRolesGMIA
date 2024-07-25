using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheOtherRoles.Patches;
using TheOtherRoles.TheOtherRoles.Core;
using UnityEngine;
using static TheOtherRoles.CustomOption;

namespace TheOtherRoles.TheOtherRoles.Roles.Neutral;
public sealed class Jester : RoleBase
{
    public static readonly RoleInfo Info =
    RoleInfo.Create(
        typeof(Jester),
        player => new Jester(player),
        03_00100,
        new Color32(236, 98, 165, byte.MaxValue),
        CreateOpt,
        "jester",
        RoleId.Jester,
        CustomOptionType.Neutral

);
    public Jester(PlayerControl player)
    : base(
        Info,
        player
    )
    { }
    public static CustomOption jesterSpawnRate;
    public static CustomOption jesterCanCallEmergency;
    public static CustomOption jesterHasImpostorVision;
    public static CustomOption jesterCanVent;

    public bool triggerJesterWin = false;
    public bool canCallEmergency = true;
    public bool hasImpostorVision = false;
    public bool canUseVents = false;
    public override bool CanVent() => canUseVents;
    public override bool HasImpVision() => hasImpostorVision;
    public override Action OnWrapUp(GameData.PlayerInfo exiled, ref bool DecidedWinner)
    {
        if (exiled == null || Player.PlayerId != exiled.PlayerId) return null;
        triggerJesterWin = true;
        DecidedWinner = true;
        return () =>
        {

        };
    }
    public override string OverrideExileText(PlayerControl player, StringNames id)
    {
        return player == Player && id is StringNames.ImpostorsRemainP or StringNames.ImpostorsRemainS ?
            "":default;
    }
    public override bool CanUseMeetingButton() => canCallEmergency;
    public override string OverrideMeetingBtnText()
    {
        return canCallEmergency? default : ModTranslation.getString("jesterMeetingButton");
    }
    static void CreateOpt()
    {
        jesterCanCallEmergency = CustomOption.Create(Info, 10, "jesterCanCallEmergency", true, jesterSpawnRate);
        jesterHasImpostorVision = CustomOption.Create(62, CustomOptionType.Neutral, "jesterHasImpostorVision", false, jesterSpawnRate);
        jesterCanVent = CustomOption.Create(6088, CustomOptionType.Neutral, "jesterCanVent", false, jesterSpawnRate);

    }
    public override void clearAndReload()
    {
        triggerJesterWin = false;
        canCallEmergency = jesterCanCallEmergency.getBool();
        hasImpostorVision = jesterHasImpostorVision.getBool();
        canUseVents = jesterCanVent.getBool();
    }
}

