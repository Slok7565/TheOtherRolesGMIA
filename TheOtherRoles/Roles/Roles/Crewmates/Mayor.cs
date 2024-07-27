using System;
using TheOtherRoles.Utilities;
using TheOtherRoles.Players;
using Hazel;
using TheOtherRoles.Objects;
using TheOtherRoles.Roles.Core;
using TheOtherRoles.Roles.Core.Bases;
using TheOtherRoles.Helpers;
using UnityEngine;
using static TheOtherRoles.CustomOption;
using System.Linq;
using TheOtherRoles.Patches;

namespace TheOtherRoles.Roles.Crewmates;
public sealed class Mayor : RoleBase
{
    public static readonly RoleInfo Info =
    RoleInfo.Create(
        typeof(Mayor),
        player => new Mayor(player),
        04_00100,
        new Color32(32, 77, 66, byte.MaxValue),
        CreateOpt,
        RoleId.Mayor,
        CustomOptionType.Crewmate

);
    public Mayor(PlayerControl player)
    : base(
        Info,
        player
    )
    { }
    public static CustomOption OptmayorCanSeeVoteColors;
    public static CustomOption OptmayorTasksNeededToSeeVoteColors;
    public static CustomOption OptmayorMeetingButton;
    public static CustomOption OptmayorMaxRemoteMeetings;
    public static CustomOption OptmayorChooseSingleVote;

    static void CreateOpt()
    {
        OptmayorCanSeeVoteColors = CustomOption.Create(Info, 10, "mayorCanSeeVoteColor", false);
        OptmayorTasksNeededToSeeVoteColors = CustomOption.Create(Info, 11, "mayorTasksNeededToSeeVoteColors", 5f, 0f, 20f, 1f, null, "unitScrews");
        OptmayorMeetingButton = CustomOption.Create(Info, 12, "mayorMeetingButton", true);
        OptmayorMaxRemoteMeetings = CustomOption.Create(Info, 13, "mayorMaxRemoteMeetings", 1f, 1f, 5f, 1f, OptmayorMeetingButton, "unitShots");
        OptmayorChooseSingleVote = CustomOption.Create(Info, 14, "mayorChooseSingleVote", new string[] { "optionOff", "mayorBeforeVoting", "mayorUntilMeetingEnd" });


    }
    public Minigame emergency = null;
    public Sprite emergencySprite = null;
    public int remoteMeetingsLeft = 1;

    public bool canSeeVoteColors = false;
    public int tasksNeededToSeeVoteColors;
    public bool meetingButton = true;
    public int mayorChooseSingleVote;

    public bool voteTwice = true;

    public Sprite getMeetingSprite()
    {
        if (emergencySprite) return emergencySprite;
        emergencySprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.EmergencyButton.png", 550f);
        return emergencySprite;
    }

    public override void clearAndReload()
    {
        emergency = null;
        emergencySprite = null;
        remoteMeetingsLeft = Mathf.RoundToInt(OptmayorMaxRemoteMeetings.getFloat());
        canSeeVoteColors = OptmayorCanSeeVoteColors.getBool();
        tasksNeededToSeeVoteColors = (int)OptmayorTasksNeededToSeeVoteColors.getFloat();
        meetingButton = OptmayorMeetingButton.getBool();
        mayorChooseSingleVote = OptmayorChooseSingleVote.getSelection();
        voteTwice = true;
    }
    public static CustomButton mayorMeetingButton;

    public override void CreateButton(HudManager __instance)
    {
        mayorMeetingButton = new CustomButton(
   () => {
       CachedPlayer.LocalPlayer.NetTransform.Halt(); // Stop current movement
        remoteMeetingsLeft--;
       RolesHelper.handleVampireBiteOnBodyReport(); // Manually call Vampire handling, since the CmdReportDeadBody Prefix won't be called
       RolesHelper.HandleUndertakerDropOnBodyReport();
       RolesHelper.handleTrapperTrapOnBodyReport();
       RPCProcedure.uncheckedCmdReportDeadBody(CachedPlayer.LocalPlayer.PlayerId, Byte.MaxValue);

       MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.UncheckedCmdReportDeadBody, Hazel.SendOption.Reliable, -1);
       writer.Write(CachedPlayer.LocalPlayer.PlayerId);
       writer.Write(Byte.MaxValue);
       AmongUsClient.Instance.FinishRpcImmediately(writer);
       mayorMeetingButton.Timer = 1f;
   },
   () => { return !CachedPlayer.LocalPlayer.Data.IsDead && meetingButton; },
   () => {
       mayorMeetingButton.actionButton.OverrideText(ModTranslation.getString("mayorEmergencyLeftText") + " (" + remoteMeetingsLeft + ")");
       bool sabotageActive = false;
       foreach (PlayerTask task in CachedPlayer.LocalPlayer.PlayerControl.myTasks.GetFastEnumerator())
           if (task.TaskType == TaskTypes.FixLights || task.TaskType == TaskTypes.RestoreOxy || task.TaskType == TaskTypes.ResetReactor || task.TaskType == TaskTypes.ResetSeismic || task.TaskType == TaskTypes.FixComms || task.TaskType == TaskTypes.StopCharles
               || SubmergedCompatibility.IsSubmerged && task.TaskType == SubmergedCompatibility.RetrieveOxygenMask)
               sabotageActive = true;
       return !sabotageActive && CachedPlayer.LocalPlayer.PlayerControl.CanMove && (remoteMeetingsLeft > 0);
   },
   () => { mayorMeetingButton.Timer = mayorMeetingButton.MaxTimer; },
   getMeetingSprite(),
   CustomButton.ButtonPositions.lowerRowRight,
   __instance,
   KeyCode.F,
   true,
   0f,
   () => { },
   false,
   ModTranslation.getString("mayorEmergencyMeetingText")
);

    }
    public void mayorToggleVoteTwice(MeetingHud __instance)
    {
        __instance.playerStates[0].Cancel();  // This will stop the underlying buttons of the template from showing up
        if (__instance.state == MeetingHud.VoteStates.Results || Player.Data.IsDead) return;
        if (mayorChooseSingleVote == 1)
        { // Only accept changes until the mayor voted
            var mayorPVA = __instance.playerStates.FirstOrDefault(x => x.TargetPlayerId == Player.PlayerId);
            if (mayorPVA != null && mayorPVA.DidVote)
            {
                SoundEffectsManager.play("fail");
                return;
            }
        }

        voteTwice = !voteTwice;

        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.MayorSetVoteTwice, Hazel.SendOption.Reliable, -1);
        writer.Write(voteTwice);
        AmongUsClient.Instance.FinishRpcImmediately(writer);

        MeetingHudPatch.meetingExtraButtonLabel.text = OtherHelper.cs(Info.color, ModTranslation.getString("mayorDoubleVote") + (voteTwice ? OtherHelper.cs(Color.green, ModTranslation.getString("mayorDoubleVoteOn")) : OtherHelper.cs(Color.red, ModTranslation.getString("mayorDoubleVoteOff"))));
    }

    public override void setCustomButtonCooldowns()
    {
        mayorMeetingButton.MaxTimer = GameManager.Instance.LogicOptions.GetEmergencyCooldown();

    }
}

