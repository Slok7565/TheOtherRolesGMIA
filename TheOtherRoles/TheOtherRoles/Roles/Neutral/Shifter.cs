using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheOtherRoles.Patches;
using TheOtherRoles.TheOtherRoles.Core;
using UnityEngine;
using TheOtherRoles.Objects;

using static TheOtherRoles.CustomOption;
using Hazel;
using TheOtherRoles.Players;
using TheOtherRoles.Role;
using AmongUs.GameOptions;
using static TheOtherRoles.Role.TheOtherRoles;
using TheOtherRoles.Utilities;

namespace TheOtherRoles.TheOtherRoles.Roles.Neutral;
public sealed class Shifter : RoleBase
{
    public static readonly RoleInfo Info =
    RoleInfo.Create(
        typeof(Shifter),
        player => new Shifter(player),
        05_00200,
        new Color32(102, 102, 102, byte.MaxValue),
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
    public List<int> pastShifters = new List<int>();

    public PlayerControl futureShift;
    public PlayerControl currentTarget;
    public PlayerControl killer;
    public DeadPlayer.CustomDeathReason deathReason;
    public bool shiftModifiers = false;
    public bool isNeutral = false;
    public bool shiftPastShifters = false;

    private Sprite buttonSprite;
    CustomButton shifterShiftButton;
    void SendRpc()
    {
        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.RoleIdSync, Hazel.SendOption.Reliable, -1);
        writer.Write(futureShift.PlayerId);
        AmongUsClient.Instance.FinishRpcImmediately(writer);
    }
    public override void ReceiveRPC(MessageReader reader)
    {
        var targetId = reader.ReadByte();
        DoShift(targetId);
    }
    void DoShift(byte targetId){
        
        PlayerControl oldShifter = Player;
        PlayerControl player = Helpers.playerById(targetId);
        if (player == null || oldShifter == null) return;


        futureShift = null;
        if (!isNeutral) clearAndReload();

        // Suicide (exile) when impostor or impostor variants
        if (!isNeutral && (player.Data.Role.IsImpostor
            || Helpers.isNeutral(player)
            || Madmate.madmate.Any(x => x.PlayerId == player.PlayerId)
            || player == CreatedMadmate.createdMadmate) && !oldShifter.Data.IsDead)
        {
            oldShifter.Exiled();
            GameHistory.overrideDeathReasonAndKiller(oldShifter, DeadPlayer.CustomDeathReason.Shift, player);
            if (PlayerControl.AllPlayerControls.ToArray().Any(p => )oldShifter == Lawyer.target && AmongUsClient.Instance.AmHost && Lawyer.lawyer != null)
            {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.LawyerPromotesToPursuer, Hazel.SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.lawyerPromotesToPursuer();
            }
            return;
        }

        // Switch shield
        if (Medic.shielded != null && Medic.shielded == player)
        {
            Medic.shielded = oldShifter;
        }
        else if (Medic.shielded != null && Medic.shielded == oldShifter)
        {
            Medic.shielded = player;
        }

        if (Madmate.madmate.Any(x => x.PlayerId == player.PlayerId))
        {
            Madmate.madmate.Add(oldShifter);
            Madmate.madmate.Remove(player);
        }

        if (Shifter.shiftModifiers)
        {


            // Switch Lovers
            if (Lovers.lover1 != null && oldShifter == Lovers.lover1) Lovers.lover1 = player;
            else if (Lovers.lover1 != null && player == Lovers.lover1) Lovers.lover1 = oldShifter;

            if (Lovers.lover2 != null && oldShifter == Lovers.lover2) Lovers.lover2 = player;
            else if (Lovers.lover2 != null && player == Lovers.lover2) Lovers.lover2 = oldShifter;

            if (Cupid.lovers1 != null && oldShifter == Cupid.lovers1) Cupid.lovers1 = player;
            else if (Cupid.lovers1 != null && player == Cupid.lovers1) Cupid.lovers1 = oldShifter;

            if (Cupid.lovers2 != null && oldShifter == Cupid.lovers2) Cupid.lovers2 = player;
            else if (Cupid.lovers2 != null && player == Cupid.lovers2) Cupid.lovers2 = oldShifter;

            // Switch Anti-Teleport
            if (AntiTeleport.antiTeleport.Any(x => x.PlayerId == player.PlayerId))
            {
                AntiTeleport.antiTeleport.Add(oldShifter);
                AntiTeleport.antiTeleport.Remove(player);
            }
            // Switch Bloody
            if (Bloody.bloody.Any(x => x.PlayerId == player.PlayerId))
            {
                Bloody.bloody.Add(oldShifter);
                Bloody.bloody.Remove(player);
            }
            // Switch Mini
            if (Mini.mini == player) Mini.mini = oldShifter;
            // Switch Tiebreaker
            if (Tiebreaker.tiebreaker == player) Tiebreaker.tiebreaker = oldShifter;
            // Switch Chameleon
            if (Chameleon.chameleon.Any(x => x.PlayerId == player.PlayerId))
            {
                Chameleon.chameleon.Add(oldShifter);
                Chameleon.chameleon.Remove(player);
            }
            // Switch Sunglasses
            if (Sunglasses.sunglasses.Any(x => x.PlayerId == player.PlayerId))
            {
                Sunglasses.sunglasses.Add(oldShifter);
                Sunglasses.sunglasses.Remove(player);
            }
            if (Vip.vip.Any(x => x.PlayerId == player.PlayerId))
            {
                Vip.vip.Add(oldShifter);
                Vip.vip.Remove(player);
            }
            if (Invert.invert.Any(x => x.PlayerId == player.PlayerId))
            {
                Invert.invert.Add(oldShifter);
                Invert.invert.Remove(player);
            }
        }

        // Shift role

        if (Mayor.mayor != null && Mayor.mayor == player)
            Mayor.mayor = oldShifter;
        if (Portalmaker.portalmaker != null && Portalmaker.portalmaker == player)
            Portalmaker.portalmaker = oldShifter;
        if (Engineer.engineer != null && Engineer.engineer == player)
            Engineer.engineer = oldShifter;
        if (Sheriff.sheriff != null && Sheriff.sheriff == player)
        {
            if (Sheriff.formerDeputy != null && Sheriff.formerDeputy == Sheriff.sheriff) Sheriff.formerDeputy = oldShifter;  // Shifter also shifts info on promoted deputy (to get handcuffs)
            Sheriff.sheriff = oldShifter;
        }
        if (Deputy.deputy != null && Deputy.deputy == player)
            Deputy.deputy = oldShifter;
        if (Lighter.lighter != null && Lighter.lighter == player)
            Lighter.lighter = oldShifter;
        if (Detective.detective != null && Detective.detective == player)
            Detective.detective = oldShifter;
        if (TimeMaster.timeMaster != null && TimeMaster.timeMaster == player)
            TimeMaster.timeMaster = oldShifter;
        if (Medic.medic != null && Medic.medic == player)
            Medic.medic = oldShifter;
        if (Swapper.swapper != null && Swapper.swapper == player)
            Swapper.swapper = oldShifter;
        if (Seer.seer != null && Seer.seer == player)
            Seer.seer = oldShifter;
        if (Hacker.hacker != null && Hacker.hacker == player)
            Hacker.hacker = oldShifter;
        if (Tracker.tracker != null && Tracker.tracker == player)
            Tracker.tracker = oldShifter;
        if (Snitch.snitch != null && Snitch.snitch == player)
            Snitch.snitch = oldShifter;
        if (Spy.spy != null && Spy.spy == player)
            Spy.spy = oldShifter;
        if (SecurityGuard.securityGuard != null && SecurityGuard.securityGuard == player)
            SecurityGuard.securityGuard = oldShifter;
        if (Guesser.niceGuesser != null && Guesser.niceGuesser == player)
            Guesser.niceGuesser = oldShifter;
        if (Bait.bait != null && Bait.bait == player)
        {
            Bait.bait = oldShifter;
            if (Bait.bait.Data.IsDead) Bait.reported = true;
        }
        if (Medium.medium != null && Medium.medium == player)
            Medium.medium = oldShifter;
        if (Watcher.nicewatcher != null && Watcher.nicewatcher == player)
            Watcher.nicewatcher = oldShifter;
        if (FortuneTeller.fortuneTeller != null && FortuneTeller.fortuneTeller == player)
            FortuneTeller.fortuneTeller = oldShifter;
        if (Sherlock.sherlock != null && Sherlock.sherlock == player)
            Sherlock.sherlock = oldShifter;
        if (Sprinter.sprinter != null && Sprinter.sprinter == player)
            Sprinter.sprinter = oldShifter;
        if (Veteran.veteran != null && Veteran.veteran == player)
            Veteran.veteran = oldShifter;
        if (Yasuna.yasuna != null && Yasuna.yasuna == player)
            Yasuna.yasuna = oldShifter;
        if (player == TaskMaster.taskMaster)
            TaskMaster.taskMaster = oldShifter;
        if (Teleporter.teleporter != null && Teleporter.teleporter == player)
            Teleporter.teleporter = oldShifter;

        if (Prophet.prophet != null && Prophet.prophet == player)
            Prophet.prophet = oldShifter;


        if (Lawyer.lawyer != null && Lawyer.target == player)
        {
            Lawyer.target = oldShifter;
        }

        player.GetRoleClass().Player = oldShifter;

        if (isNeutral)
        {
            Player = player;
            pastShifters.Add(oldShifter.PlayerId);
            if (player.Data.Role.IsImpostor)
            {
                FastDestroyableSingleton<RoleManager>.Instance.SetRole(player, RoleTypes.Crewmate);
                FastDestroyableSingleton<RoleManager>.Instance.SetRole(oldShifter, RoleTypes.Impostor);
                if (deathReason != DeadPlayer.CustomDeathReason.Disconnect && !oldShifter.Data.IsDead) // In case the Chain-Shifter revives
                {
                    oldShifter.Exiled();
                    GameHistory.overrideDeathReasonAndKiller(oldShifter, deathReason, killer);
                }
            }
        }

        // Set cooldowns to max for both players
        if (CachedPlayer.LocalPlayer.PlayerControl == oldShifter || CachedPlayer.LocalPlayer.PlayerControl == player)
            CustomButton.ResetAllCooldowns();


    }
    public override void OnMeetingStart(GameData.PlayerInfo meetingTarget)
    {
        if (isNeutral && Player.Data.IsDead && futureShift != null &&futureShift.Data.Role.IsImpostor)
        {
            var dp = GameHistory.deadPlayers.FirstOrDefault(x => x.player.PlayerId == Player.PlayerId);
            killer = dp.killerIfExisting;
            deathReason = dp.deathReason;
        }
    }
    public override void OnExile(ExileController __instance, GameData.PlayerInfo exiled, bool tie)
    {
        if (AmongUsClient.Instance.AmHost) return;
        if (futureShift != null)
        { // We need to send the RPC from the host here, to make sure that the order of shifting and erasing is correct (for that reason the futureShifted and futureErased are being synced)
            PlayerControl oldShifter = Player;
            byte oldTaskMasterPlayerId = (futureShift.GetRoleClass() as TaskMaster)?.isTaskComplete ?? false  ? futureShift.PlayerId : byte.MaxValue;
            SendRpc();
            DoShift(futureShift.PlayerId);

            if (oldShifter.GetRoleClass() is TaskMaster)
            {
                byte clearTasks = 0;
                for (int i = 0; i < oldShifter.Data.Tasks.Count; ++i)
                {
                    if (oldShifter.Data.Tasks[i].Complete)
                        ++clearTasks;
                }
                bool allTasksCompleted = clearTasks == oldShifter.Data.Tasks.Count;
                byte[] taskTypeIds = allTasksCompleted ? TaskMasterTaskHelper.GetTaskMasterTasks(oldShifter) : null;
                MessageWriter writer2 = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.TaskMasterSetExTasks, Hazel.SendOption.Reliable, -1);
                writer2.Write(oldShifter.PlayerId);
                writer2.Write(oldTaskMasterPlayerId);
                if (taskTypeIds != null)
                    writer2.Write(taskTypeIds);
                AmongUsClient.Instance.FinishRpcImmediately(writer2);
                RPCProcedure.taskMasterSetExTasks(oldShifter.PlayerId, oldTaskMasterPlayerId, taskTypeIds);
            }
        }
        futureShift = null;

    }
    public Sprite getButtonSprite()
    {
        if (buttonSprite) return buttonSprite;
        buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.ShiftButton.png", 115f);
        return buttonSprite;
    }

    public override void setCustomButtonCooldowns()
    {
        shifterShiftButton.MaxTimer = 0f;

    }
    public override void CreateButton(HudManager __instance)
    {
        shifterShiftButton = new CustomButton(
            () => {
                if (RoleInfo.getRoleInfoForPlayer(currentTarget).Any(r => r.roleId is RoleId.Veteran)  && (currentTarget.GetRoleClass() as Veteran).alertActive && isNeutral)
                {
                    Helpers.checkMurderAttemptAndKill(currentTarget, Player);
                    return;
                }

                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.RoleIdSync, Hazel.SendOption.Reliable, -1);
                writer.Write(currentTarget.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.setFutureShifted(currentTarget.PlayerId);
                SoundEffectsManager.play("shifterShift");
            },
            () => { return futureShift == null && !Player.Data.IsDead; },
            () => { return currentTarget && futureShift == null && CachedPlayer.LocalPlayer.PlayerControl.CanMove; },
            () => { },
            getButtonSprite(),
            CustomButton.ButtonPositions.lowerRowRight,
            __instance,
            KeyCode.F,
            buttonText: ModTranslation.getString("ShiftText"),
            abilityTexture: true
        );


    }
    public static CustomOption shifterIsNeutralRate;
    public static CustomOption shifterShiftsModifiers;
    public static CustomOption shifterPastShifters;



    static void CreateOpt()
    {
        shifterIsNeutralRate = CustomOption.Create(Info, 10,  "shifterIsNeutralRate", CustomOptionHolder.rates);
        shifterShiftsModifiers = CustomOption.Create(Info, 11, "shifterShiftsModifiers", false);
        shifterPastShifters = CustomOption.Create(Info, 12, "shifterPastShifters", false);



    }
    public override void clearAndReload()
    {
        pastShifters = new List<int>();
        currentTarget = null;
        futureShift = null;
        killer = null;
        deathReason = DeadPlayer.CustomDeathReason.Disconnect; // Get something unreachable here
        shiftModifiers =  shifterShiftsModifiers.getBool();
        shiftPastShifters = shifterPastShifters.getBool();
        isNeutral = false;
    }
    void shifterSetTarget()
    {
        if (Player != CachedPlayer.LocalPlayer.PlayerControl) return;
        List<PlayerControl> blockShift = null;
        if (isNeutral && !shiftPastShifters)
        {
            blockShift = new List<PlayerControl>();
            foreach (var playerId in pastShifters)
            {
                blockShift.Add(Helpers.playerById((byte)playerId));
            }
        }

        currentTarget = PlayerControlFixedUpdatePatch.setTarget(untargetablePlayers: blockShift);
        if (futureShift == null) PlayerControlFixedUpdatePatch.setPlayerOutline(currentTarget, Info.color);
    }

}

