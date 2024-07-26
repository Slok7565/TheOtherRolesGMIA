using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmongUs.GameOptions;
using Hazel;
using TheOtherRoles.CustomGameModes;
using TheOtherRoles.Patches;
using TheOtherRoles.Players;
using TheOtherRoles.Roles;
using TheOtherRoles.Roles.Core;
using TheOtherRoles.Utilities;
using UnityEngine;
using static TheOtherRoles.Roles.TheOtherRoles;
using static TheOtherRoles.Helpers.OtherHelper;
using static TheOtherRoles.Helpers.ResourcesHelper;

namespace TheOtherRoles.Helpers;

public static class RolesHelper
{
    public static void handleVampireBiteOnBodyReport()
    {
        // Murder the bitten player and reset bitten (regardless whether the kill was successful or not)
        checkMurderAttemptAndKill(Vampire.vampire, Vampire.bitten, true, false);
        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.VampireSetBitten, SendOption.Reliable, -1);
        writer.Write(byte.MaxValue);
        writer.Write(byte.MaxValue);
        AmongUsClient.Instance.FinishRpcImmediately(writer);
        RPCProcedure.vampireSetBitten(byte.MaxValue, byte.MaxValue);
    }

    public static void handleTrapperTrapOnBodyReport()
    {
        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.TrapperMeetingFlag, SendOption.Reliable, -1);
        AmongUsClient.Instance.FinishRpcImmediately(writer);
        RPCProcedure.trapperMeetingFlag();
    }

    public static IEnumerator BlackmailShhh()
    {
        yield return HudManager.Instance.CoFadeFullScreen(Color.clear, new Color(0f, 0f, 0f, 0.98f));
        var TempPosition = HudManager.Instance.shhhEmblem.transform.localPosition;
        var TempDuration = HudManager.Instance.shhhEmblem.HoldDuration;
        HudManager.Instance.shhhEmblem.transform.localPosition = new Vector3(
            HudManager.Instance.shhhEmblem.transform.localPosition.x,
            HudManager.Instance.shhhEmblem.transform.localPosition.y,
            HudManager.Instance.FullScreen.transform.position.z + 1f);
        HudManager.Instance.shhhEmblem.TextImage.text = ModTranslation.getString("blackmailerBlackmailText");
        HudManager.Instance.shhhEmblem.HoldDuration = 2.5f;
        yield return HudManager.Instance.ShowEmblem(true);
        HudManager.Instance.shhhEmblem.transform.localPosition = TempPosition;
        HudManager.Instance.shhhEmblem.HoldDuration = TempDuration;
        yield return HudManager.Instance.CoFadeFullScreen(new Color(0f, 0f, 0f, 0.98f), Color.clear);
        yield return null;
    }

    public static void enableCursor(bool initalSetCursor)
    {
        if (initalSetCursor)
        {
            Sprite sprite = loadSpriteFromResources("TheOtherRoles.Resources.Cursor.png", 115f);
            Cursor.SetCursor(sprite.texture, Vector2.zero, CursorMode.Auto);
            return;
        }
        if (TheOtherRolesPlugin.ToggleCursor.Value)
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        else
        {
            Sprite sprite = loadSpriteFromResources("TheOtherRoles.Resources.Cursor.png", 115f);
            Cursor.SetCursor(sprite.texture, Vector2.zero, CursorMode.Auto);
        }
    }

    public static void refreshRoleDescription(PlayerControl player)
    {
        List<RoleInfo> infos = RoleInfo.getRoleInfoForPlayer(player);
        List<string> taskTexts = new(infos.Count);

        foreach (var roleInfo in infos)
            taskTexts.Add(getRoleString(roleInfo));

        var toRemove = new List<PlayerTask>();
        foreach (PlayerTask t in player.myTasks.GetFastEnumerator())
        {
            var textTask = t.TryCast<ImportantTextTask>();
            if (textTask == null) continue;

            var currentText = textTask.Text;

            if (taskTexts.Contains(currentText)) taskTexts.Remove(currentText); // TextTask for this RoleInfo does not have to be added, as it already exists
            else toRemove.Add(t); // TextTask does not have a corresponding RoleInfo and will hence be deleted
        }

        foreach (PlayerTask t in toRemove)
        {
            t.OnRemove();
            player.myTasks.Remove(t);
            UnityEngine.Object.Destroy(t.gameObject);
        }

        // Add TextTask for remaining RoleInfos
        foreach (string title in taskTexts)
        {
            var task = new GameObject("RoleTask").AddComponent<ImportantTextTask>();
            task.transform.SetParent(player.transform, false);
            task.Text = title;
            player.myTasks.Insert(0, task);
        }

        if (Madmate.madmate.Any(x => x.PlayerId == player.PlayerId) || player == CreatedMadmate.createdMadmate)
        {
            var task = new GameObject("RoleTask").AddComponent<ImportantTextTask>();
            task.transform.SetParent(player.transform, false);
            task.Text = cs(Madmate.color, $"{Madmate.fullName}: " + ModTranslation.getString("madmateShortDesc"));
            player.myTasks.Insert(0, task);
        }
    }
    internal static string getRoleString(RoleInfo roleInfo)
    {
        if (roleInfo.roleId == RoleId.Jackal)
        {
            var getSidekickText = Jackal.canCreateSidekick ? ModTranslation.getString("jackalWithSidekick") : ModTranslation.getString("jackalShortDesc");
            return cs(roleInfo.color, $"{roleInfo.name}: {getSidekickText}");
        }

        if (roleInfo.roleId == RoleId.Invert)
            return cs(roleInfo.color, $"{roleInfo.name}: {roleInfo.shortDescription} ({Invert.meetings})");

        return cs(roleInfo.color, $"{roleInfo.name}: {roleInfo.shortDescription}");
    }
    public static bool hasFakeTasks(this PlayerControl player)
    {
        return player.GetRoleClass().HasTasks || player == Jackal.jackal || player == Sidekick.sidekick || player == Arsonist.arsonist || player == Opportunist.opportunist || player == Vulture.vulture || Jackal.formerJackals.Any(x => x == player) || player == Moriarty.moriarty || player == Moriarty.formerMoriarty
            || Madmate.madmate.Any(x => x.PlayerId == player.PlayerId) && !Madmate.hasTasks ||
            player == CreatedMadmate.createdMadmate && !CreatedMadmate.hasTasks || player == Akujo.akujo || player == PlagueDoctor.plagueDoctor || player == JekyllAndHyde.formerJekyllAndHyde || player == Cupid.cupid;
    }

    public static bool canBeErased(this PlayerControl player)
    {
        return player != Jackal.jackal && player != Sidekick.sidekick && !Jackal.formerJackals.Any(x => x == player);
    }

    public static bool shouldShowGhostInfo()
    {
        return CachedPlayer.LocalPlayer.PlayerControl != null && CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead && TORMapOptions.ghostsSeeInformation || AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Ended;
    }

    public static void clearAllTasks(this PlayerControl player)
    {
        if (player == null) return;
        foreach (var playerTask in player.myTasks.GetFastEnumerator())
        {
            playerTask.OnRemove();
            UnityEngine.Object.Destroy(playerTask.gameObject);
        }
        player.myTasks.Clear();

        if (player.Data != null && player.Data.Tasks != null)
            player.Data.Tasks.Clear();
    }

    public static bool roleCanUseVents(this PlayerControl player)
    {
        bool roleCouldUse = false;
        if (player.GetRoleClass().CanVent())
            roleCouldUse = true;
        if (Engineer.engineer != null && Engineer.engineer == player)
            roleCouldUse = true;
        else if (Jackal.canUseVents && Jackal.jackal != null && Jackal.jackal == player)
            roleCouldUse = true;
        else if (Sidekick.canUseVents && Sidekick.sidekick != null && Sidekick.sidekick == player)
            roleCouldUse = true;
        else if (Spy.canEnterVents && Spy.spy != null && Spy.spy == player)
            roleCouldUse = true;
        else if (Vulture.canUseVents && Vulture.vulture != null && Vulture.vulture == player)
            roleCouldUse = true;
        else if (Madmate.canVent && Madmate.madmate.Any(x => x.PlayerId == player.PlayerId))
            roleCouldUse = true;
        else if (CreatedMadmate.canEnterVents && CreatedMadmate.createdMadmate != null && CreatedMadmate.createdMadmate == player)
            roleCouldUse = true;
        else if (Moriarty.moriarty != null && Moriarty.moriarty == player)
            roleCouldUse = true;
        else if (JekyllAndHyde.jekyllAndHyde != null && !JekyllAndHyde.isJekyll() && JekyllAndHyde.jekyllAndHyde == player)
            roleCouldUse = true;
        else if (Thief.canUseVents && Thief.thief != null && Thief.thief == player)
            roleCouldUse = true;
        else if (player.Data?.Role != null && player.Data.Role.CanVent)
            if (Janitor.janitor != null && Janitor.janitor == CachedPlayer.LocalPlayer.PlayerControl)
                roleCouldUse = false;
            else if (Mafioso.mafioso != null && Mafioso.mafioso == CachedPlayer.LocalPlayer.PlayerControl && Godfather.godfather != null && !Godfather.godfather.Data.IsDead)
                roleCouldUse = false;
            else if (Ninja.ninja != null && Ninja.ninja == CachedPlayer.LocalPlayer.PlayerControl && Ninja.canUseVents == false)
                roleCouldUse = false;
            else if (Undertaker.undertaker != null && Undertaker.undertaker == CachedPlayer.LocalPlayer.PlayerControl && Undertaker.DraggedBody != null && Undertaker.disableVent)
                roleCouldUse = false;
            else
                roleCouldUse = true;
        return roleCouldUse;
    }

    public static MurderAttemptResult checkMuderAttempt(PlayerControl killer, PlayerControl target, bool blockRewind = false, bool ignoreBlank = false, bool ignoreIfKillerIsDead = false)
    {
        var targetRole = RoleInfo.getRoleInfoForPlayer(target, false).FirstOrDefault();

        // Modified vanilla checks
        if (AmongUsClient.Instance.IsGameOver) return MurderAttemptResult.SuppressKill;
        if (killer == null || killer.Data == null || killer.Data.IsDead && !ignoreIfKillerIsDead || killer.Data.Disconnected) return MurderAttemptResult.SuppressKill; // Allow non Impostor kills compared to vanilla code
        if (target == null || target.Data == null || target.Data.IsDead || target.Data.Disconnected) return MurderAttemptResult.SuppressKill; // Allow killing players in vents compared to vanilla code

        if (GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek) return MurderAttemptResult.PerformKill;

        // Handle first kill attempt
        if (TORMapOptions.shieldFirstKill && TORMapOptions.firstKillPlayer == target) return MurderAttemptResult.SuppressKill;

        // Handle blank shot
        if (!ignoreBlank && Pursuer.blankedList.Any(x => x.PlayerId == killer.PlayerId))
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.SetBlanked, SendOption.Reliable, -1);
            writer.Write(killer.PlayerId);
            writer.Write((byte)0);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.setBlanked(killer.PlayerId, 0);

            return MurderAttemptResult.BlankKill;
        }

        // Block impostor shielded kill
        if (Medic.shielded != null && Medic.shielded == target)
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(killer.NetId, (byte)CustomRPC.ShieldedMurderAttempt, SendOption.Reliable, -1);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.shieldedMurderAttempt();
            SoundEffectsManager.play("fail");
            return MurderAttemptResult.SuppressKill;
        }

        // Block impostor not fully grown mini kill
        else if (Mini.mini != null && target == Mini.mini && !Mini.isGrownUp())
            return MurderAttemptResult.SuppressKill;

        // Block Time Master with time shield kill
        else if (TimeMaster.shieldActive && TimeMaster.timeMaster != null && TimeMaster.timeMaster == target)
        {
            if (!blockRewind)
            { // Only rewind the attempt was not called because a meeting startet 
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(killer.NetId, (byte)CustomRPC.TimeMasterRewindTime, SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.timeMasterRewindTime();
            }
            return MurderAttemptResult.SuppressKill;
        }

        else if (Cupid.cupid != null && Cupid.shielded == target && !Cupid.cupid.Data.IsDead)
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.CupidSuicide, SendOption.Reliable, -1);
            writer.Write(Cupid.cupid.PlayerId);
            writer.Write(true);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.cupidSuicide(Cupid.cupid.PlayerId, true);
            return MurderAttemptResult.BlankKill;
        }

        // Kill the killer if Veteran is on alert
        else if (Veteran.veteran != null && Veteran.alertActive && Veteran.veteran == target)
            //MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(killer.NetId, (byte)CustomRPC.VeteranKill, Hazel.SendOption.Reliable, -1);
            //AmongUsClient.Instance.FinishRpcImmediately(writer);
            //RPCProcedure.veteranKill(killer.PlayerId);
            return MurderAttemptResult.ReverseKill;

        // Thief if hit crew only kill if setting says so, but also kill the thief.
        else if (Thief.isFailedThiefKill(target, killer, targetRole))
        {
            Thief.suicideFlag = true;
            return MurderAttemptResult.SuppressKill;
        }

        // Block hunted with time shield kill
        else if (Hunted.timeshieldActive.Contains(target.PlayerId))
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(killer.NetId, (byte)CustomRPC.HuntedRewindTime, SendOption.Reliable, -1);
            writer.Write(target.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.huntedRewindTime(target.PlayerId);

            return MurderAttemptResult.SuppressKill;
        }

        if (TransportationToolPatches.isUsingTransportation(target) && !blockRewind && killer == Vampire.vampire)
            return MurderAttemptResult.DelayVampireKill;
        else if (TransportationToolPatches.isUsingTransportation(target))
            return MurderAttemptResult.SuppressKill;

        return MurderAttemptResult.PerformKill;
    }

    public static void MurderPlayer(PlayerControl killer, PlayerControl target, bool showAnimation)
    {
        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.UncheckedMurderPlayer, SendOption.Reliable, -1);
        writer.Write(killer.PlayerId);
        writer.Write(target.PlayerId);
        writer.Write(showAnimation ? byte.MaxValue : 0);
        AmongUsClient.Instance.FinishRpcImmediately(writer);
        RPCProcedure.uncheckedMurderPlayer(killer.PlayerId, target.PlayerId, showAnimation ? byte.MaxValue : (byte)0);
    }

    public static MurderAttemptResult checkMurderAttemptAndKill(PlayerControl killer, PlayerControl target, bool isMeetingStart = false, bool showAnimation = true, bool ignoreBlank = false, bool ignoreIfKillerIsDead = false)
    {
        // The local player checks for the validity of the kill and performs it afterwards (different to vanilla, where the host performs all the checks)
        // The kill attempt will be shared using a custom RPC, hence combining modded and unmodded versions is impossible

        MurderAttemptResult murder = checkMuderAttempt(killer, target, isMeetingStart, ignoreBlank, ignoreIfKillerIsDead);
        if (murder == MurderAttemptResult.PerformKill)
            MurderPlayer(killer, target, showAnimation);
        else if (murder == MurderAttemptResult.DelayVampireKill)
            HudManager.Instance.StartCoroutine(Effects.Lerp(10f, new Action<float>((p) =>
            {
                if (!TransportationToolPatches.isUsingTransportation(target) && Vampire.bitten != null)
                {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.VampireSetBitten, SendOption.Reliable, -1);
                    writer.Write(byte.MaxValue);
                    writer.Write(byte.MaxValue);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.vampireSetBitten(byte.MaxValue, byte.MaxValue);
                    MurderPlayer(killer, target, showAnimation);
                }
            })));

        if (murder == MurderAttemptResult.ReverseKill)
            checkMurderAttemptAndKill(target, killer, isMeetingStart);
        return murder;
    }



}
