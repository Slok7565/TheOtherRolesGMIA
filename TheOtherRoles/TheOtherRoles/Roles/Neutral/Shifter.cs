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
public sealed class Shifter : RoleBase
{
    public static readonly RoleInfo Info =
    RoleInfo.Create(
        typeof(Shifter),
        player => new Shifter(player),
        05_00100,
        new Color32(236, 98, 165, byte.MaxValue),
        CreateOpt,
        RoleId.Shifter,
        CustomOptionType.Neutral

);
    public Shifter(PlayerControl player)
    : base(
        Info,
        player
    )
    { }
    public static CustomOption ShifterCanCallEmergency;
    public static CustomOption ShifterHasImpostorVision;
    public static CustomOption ShifterCanVent;

    public bool triggerShifterWin = false;
    public bool canCallEmergency = true;
    public bool hasImpostorVision = false;
    public bool canUseVents = false;
    public override bool CanVent() => canUseVents;
    public override bool HasImpVision() => hasImpostorVision;
    public override Action OnWrapUp(GameData.PlayerInfo exiled, ref bool DecidedWinner)
    {
        if (exiled == null || Player.PlayerId != exiled.PlayerId) return null;
        triggerShifterWin = true;
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
        return canCallEmergency? default : ModTranslation.getString("ShifterMeetingButton");
    }
    static void CreateOpt()
    {
        ShifterCanCallEmergency = CustomOption.Create(Info, 10, "ShifterCanCallEmergency", true);
        ShifterHasImpostorVision = CustomOption.Create(Info, 11,  "ShifterHasImpostorVision", false);
        ShifterCanVent = CustomOption.Create(Info, 12, "ShifterCanVent", false);

    }
    public override void clearAndReload()
    {
        triggerShifterWin = false;
        canCallEmergency = ShifterCanCallEmergency.getBool();
        hasImpostorVision = ShifterHasImpostorVision.getBool();
        canUseVents = ShifterCanVent.getBool();
    }
}

