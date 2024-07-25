using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheOtherRoles.TheOtherRoles.Core;
using TheOtherRoles.TheOtherRoles.Core.Interfaces;
using TheOtherRoles.TheOtherRoles.Roles.Neutral;
using UnityEngine;
using static TheOtherRoles.CustomOption;

namespace TheOtherRoles.TheOtherRoles.Roles.Impostor;
public sealed class SerialKiller : RoleBase, IImpostor
{
    public static readonly RoleInfo Info =
    RoleInfo.Create(
        typeof(Jester),
        player => new Jester(player),
        03_00100,
        Palette.ImpostorRed,
        CreateOpt,
        "jester",
        RoleId.Jester,
        CustomOptionType.Neutral

);
    public SerialKiller(PlayerControl player)
    : base(
        Info,
        player
    )
    { }


    public float killCooldown = 15f;
    public float suicideTimer = 40f;
    public bool resetTimer = true;

    public bool isCountDown = false;

    private Sprite buttonSprite;
    public Sprite getButtonSprite()
    {
        if (buttonSprite) return buttonSprite;
        buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.SuicideButton.png", 115f);
        return buttonSprite;
    }

    public override void clearAndReload()
    {
        killCooldown = CustomOptionHolder.serialKillerKillCooldown.getFloat();
        suicideTimer = Mathf.Max(CustomOptionHolder.serialKillerSuicideTimer.getFloat(), killCooldown + 2.5f);
        resetTimer = CustomOptionHolder.serialKillerResetTimer.getBool();
        isCountDown = false;
    }
}
