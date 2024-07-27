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

namespace TheOtherRoles.Roles.Impostor;
public sealed class SerialKiller : RoleBase, IImpostor
{
    public static readonly RoleInfo Info =
    RoleInfo.Create(
        typeof(SerialKiller),
        player => new SerialKiller(player),
        03_00100,
        Palette.ImpostorRed,
        CreateOpt,
        RoleId.SerialKiller,
        CustomOptionType.Impostor

);
    public SerialKiller(PlayerControl player)
    : base(
        Info,
        player
    )
    { }
    public bool isCountDown = false;

    private Sprite buttonSprite;
    public static CustomOption serialKillerKillCooldown;
    public static CustomOption serialKillerSuicideTimer;
    public static CustomOption serialKillerResetTimer;
    static void CreateOpt()
    {
        serialKillerKillCooldown = CustomOption.Create(Info, 10, "serialKillerKillCooldown", 15f, 2.5f, 60f, 2.5f, null, "unitSeconds");
        serialKillerSuicideTimer = CustomOption.Create(Info, 11, "serialKillerSuicideTimer", 40f, 2.5f, 60f, 2.5f, null, "unitSeconds");
        serialKillerResetTimer = CustomOption.Create(Info, 12, "serialKillerResetTimer" ,true);
    }

    public Sprite getButtonSprite()
    {
        if (buttonSprite) return buttonSprite;
        buttonSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.SuicideButton.png", 115f);
        return buttonSprite;
    }
    void SendRPC(byte targetId)
    {
        var sender = CreateSender();
        sender.Writer.Write(targetId);
    }
    public override void ReceiveRPC(MessageReader reader = null)
    {
        Player.MurderPlayer(Player, MurderResultFlags.Succeeded);
        GameHistory.overrideDeathReasonAndKiller(Player, DeadPlayer.CustomDeathReason.Suicide);
    }

    public override void clearAndReload()
    {
        killCooldown = serialKillerKillCooldown.getFloat();
        suicideTimer = Mathf.Max(serialKillerSuicideTimer.getFloat(), killCooldown + 2.5f);
        resetTimer = serialKillerResetTimer.getBool();
        isCountDown = false;
    }
    public CustomButton serialKillerButton;
    public override void setCustomButtonCooldowns()
    {
        serialKillerButton.MaxTimer = suicideTimer;

    }
    public override void CreateButton(HudManager __instance)
    {
        serialKillerButton = new CustomButton(
    () => { },
    () => { return !Player.Data.IsDead && isCountDown; },
    () => { return true; },
    () =>
    {
        Player.SetKillTimer(killCooldown);
        if (resetTimer)
        {
            serialKillerButton.Timer = suicideTimer;
        }
    },
    getButtonSprite(),
    CustomButton.ButtonPositions.upperRowLeft,
    __instance,
    KeyCode.F,
    true,
    suicideTimer,
    () =>
    {
        byte targetId = Player.PlayerId;
        SendRPC(targetId);
        ReceiveRPC();
    },
    abilityTexture: true
);
        //UnityEngine.Object.Destroy(serialKillerButton.actionButton.buttonLabelText);
        //serialKillerButton.actionButton.buttonLabelText = UnityEngine.Object.Instantiate(__instance.AbilityButton.buttonLabelText, serialKillerButton.actionButton.transform);
        serialKillerButton.showButtonText = true;
        serialKillerButton.buttonText = ModTranslation.getString("serialKillerSuicideText");
        serialKillerButton.isEffectActive = true;


    }
    public float killCooldown = 15f;
    public float suicideTimer = 40f;
    public bool resetTimer = true;
    public void OnMurderPlayerAsKiller(PlayerControl target)
    {
        Player.SetKillTimer(killCooldown);
        serialKillerButton.Timer = suicideTimer;
        isCountDown = true;
    }
}
