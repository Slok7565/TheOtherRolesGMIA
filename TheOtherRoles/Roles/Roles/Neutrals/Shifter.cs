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
public sealed class Shifter : RoleBase, INeutral
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
        var sender = CreateSender();
        sender.Writer.Write(futureShift.PlayerId);
    }
    public override void ReceiveRPC(MessageReader reader)
    {
        var targetId = reader.ReadByte();
        DoShift(targetId);
    }
    void DoShift(byte targetId){
        
        PlayerControl oldShifter = Player;
        PlayerControl target = PlayerHelper.playerById(targetId);
        if (target == null || oldShifter == null) return;
        futureShift = null;
        if (!isNeutral)
        {
            Dispose();
            if ((target.Data.Role.IsImpostor || target.isNeutral()|| Madmate.madmate.Any(x => x.PlayerId == target.PlayerId)
                || target == CreatedMadmate.createdMadmate) && !oldShifter.Data.IsDead)
            {
                oldShifter.Exiled();
                GameHistory.overrideDeathReasonAndKiller(oldShifter, DeadPlayer.CustomDeathReason.Shift, target);
                if (PlayerControl.AllPlayerControls.ToArray().Any(p => )oldShifter == Lawyer.target && AmongUsClient.Instance.AmHost && Lawyer.lawyer != null)
                {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.LawyerPromotesToPursuer, Hazel.SendOption.Reliable, -1);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.lawyerPromotesToPursuer();
                }
                return;
            }
            
        }
        var ownrc = Player.GetRoleClass();
        var tarrc = target.GetRoleClass();

        tarrc.Player = oldShifter;
        CustomRoleManager.AllActiveRoles.Remove(Player.PlayerId);
        CustomRoleManager.AllActiveRoles.Add(Player.PlayerId, tarrc);
        CustomRoleManager.AllActiveRoles.Remove(targetId);
        CustomRoleManager.AllActiveRoles.Add(targetId, ownrc);

        // Switch shield
        if (Medic.shielded != null && Medic.shielded == target)
        {
            Medic.shielded = oldShifter;
        }
        else if (Medic.shielded != null && Medic.shielded == oldShifter)
        {
            Medic.shielded = target;
        }

        if (Madmate.madmate.Any(x => x.PlayerId == target.PlayerId))
        {
            Madmate.madmate.Add(oldShifter);
            Madmate.madmate.Remove(target);
        }

        if (Shifter.shiftModifiers)
        {


            // Switch Lovers
            if (Lovers.lover1 != null && oldShifter == Lovers.lover1) Lovers.lover1 = target;
            else if (Lovers.lover1 != null && target == Lovers.lover1) Lovers.lover1 = oldShifter;

            if (Lovers.lover2 != null && oldShifter == Lovers.lover2) Lovers.lover2 = target;
            else if (Lovers.lover2 != null && target == Lovers.lover2) Lovers.lover2 = oldShifter;

            if (Cupid.lovers1 != null && oldShifter == Cupid.lovers1) Cupid.lovers1 = target;
            else if (Cupid.lovers1 != null && target == Cupid.lovers1) Cupid.lovers1 = oldShifter;

            if (Cupid.lovers2 != null && oldShifter == Cupid.lovers2) Cupid.lovers2 = target;
            else if (Cupid.lovers2 != null && target == Cupid.lovers2) Cupid.lovers2 = oldShifter;

            // Switch Anti-Teleport
            if (AntiTeleport.antiTeleport.Any(x => x.PlayerId == target.PlayerId))
            {
                AntiTeleport.antiTeleport.Add(oldShifter);
                AntiTeleport.antiTeleport.Remove(target);
            }
            // Switch Bloody
            if (Bloody.bloody.Any(x => x.PlayerId == target.PlayerId))
            {
                Bloody.bloody.Add(oldShifter);
                Bloody.bloody.Remove(target);
            }
            // Switch Mini
            if (Mini.mini == target) Mini.mini = oldShifter;
            // Switch Tiebreaker
            if (Tiebreaker.tiebreaker == target) Tiebreaker.tiebreaker = oldShifter;
            // Switch Chameleon
            if (Chameleon.chameleon.Any(x => x.PlayerId == target.PlayerId))
            {
                Chameleon.chameleon.Add(oldShifter);
                Chameleon.chameleon.Remove(target);
            }
            // Switch Sunglasses
            if (Sunglasses.sunglasses.Any(x => x.PlayerId == target.PlayerId))
            {
                Sunglasses.sunglasses.Add(oldShifter);
                Sunglasses.sunglasses.Remove(target);
            }
            if (Vip.vip.Any(x => x.PlayerId == target.PlayerId))
            {
                Vip.vip.Add(oldShifter);
                Vip.vip.Remove(target);
            }
            if (Invert.invert.Any(x => x.PlayerId == target.PlayerId))
            {
                Invert.invert.Add(oldShifter);
                Invert.invert.Remove(target);
            }
        }

        // Shift role
        if (Sheriff.sheriff != null && Sheriff.sheriff == target)
        {
            if (Sheriff.formerDeputy != null && Sheriff.formerDeputy == Sheriff.sheriff) Sheriff.formerDeputy = oldShifter;  // Shifter also shifts info on promoted deputy (to get handcuffs)
            Sheriff.sheriff = oldShifter;
        }
        if (Bait.bait != null && Bait.bait == target)
        {
            Bait.bait = oldShifter;
            if (Bait.bait.Data.IsDead) Bait.reported = true;
        }

        if (Lawyer.lawyer != null && Lawyer.target == target)
        {
            Lawyer.target = oldShifter;
        }

        if (isNeutral)
        {
            Player = target;
            pastShifters.Add(oldShifter.PlayerId);
            if (target.Data.Role.IsImpostor)
            {
                FastDestroyableSingleton<RoleManager>.Instance.SetRole(target, RoleTypes.Crewmate);
                FastDestroyableSingleton<RoleManager>.Instance.SetRole(oldShifter, RoleTypes.Impostor);
                if (deathReason != DeadPlayer.CustomDeathReason.Disconnect && !oldShifter.Data.IsDead) // In case the Chain-Shifter revives
                {
                    oldShifter.Exiled();
                    GameHistory.overrideDeathReasonAndKiller(oldShifter, deathReason, killer);
                }
            }
        }
        else
            Dispose();

        // Set cooldowns to max for both players
        if (CachedPlayer.LocalPlayer.PlayerControl == oldShifter || CachedPlayer.LocalPlayer.PlayerControl == target)
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
                blockShift.Add(PlayerHelper.playerById((byte)playerId));
            }
        }

        currentTarget = PlayerControlFixedUpdatePatch.setTarget(untargetablePlayers: blockShift);
        if (futureShift == null) PlayerControlFixedUpdatePatch.setPlayerOutline(currentTarget, Info.color);
    }

}

