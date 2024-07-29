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
using Reactor.Utilities.Extensions;
using System.Collections.Generic;
using HarmonyLib;

namespace TheOtherRoles.Roles.Crewmates;
public sealed class FortuneTeller : RoleBase
{
    public PlayerControl fortuneTeller;
    public PlayerControl divineTarget;
    public Color color = new Color32(175, 198, 241, byte.MaxValue);

    public enum DivineResults
    {
        BlackWhite,
        Team,
        Role,
    }

    public int numTasks;
    public DivineResults divineResult;
    public float duration;
    public float distance;

    public bool endGameFlag = false;
    public bool meetingFlag = false;

    public Dictionary<byte, float> progress = new Dictionary<byte, float>();
    public Dictionary<byte, bool> playerStatus = new Dictionary<byte, bool>();
    public bool divinedFlag = false;
    public int numUsed = 0;

    public List<Arrow> arrows = new List<Arrow>();
    public float updateTimer = 0f;

    public bool isCompletedNumTasks(PlayerControl p)
    {
        var (tasksCompleted, tasksTotal) = TasksHandler.taskInfo(p.Data);
        return tasksCompleted >= numTasks;
    }

    public void setDivinedFlag(PlayerControl player, bool flag)
    {
        if (player == fortuneTeller)
            divinedFlag = flag;
    }

    public bool canDivine(byte index)
    {
        bool status = true;
        if (playerStatus.ContainsKey(index))
            status = playerStatus[index];
        return progress.ContainsKey(index) && progress[index] >= duration || !status;
    }

    private TMPro.TMP_Text text;
    public void fortuneTellerMessage(string message, float duration, Color color)
    {
        RoomTracker roomTracker = FastDestroyableSingleton<HudManager>.Instance?.roomTracker;
        if (roomTracker != null)
        {
            GameObject gameObject = UnityEngine.Object.Instantiate(roomTracker.gameObject);

            gameObject.transform.SetParent(FastDestroyableSingleton<HudManager>.Instance.transform);
            UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<RoomTracker>());

            // Use local position to place it in the player's view instead of the world location
            gameObject.transform.localPosition = new Vector3(0, -1.8f, gameObject.transform.localPosition.z);
            gameObject.transform.localScale *= 1.5f;

            text = gameObject.GetComponent<TMPro.TMP_Text>();
            text.text = message;
            text.color = color;

            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(duration, new Action<float>((p) =>
            {
                if (p == 1f && text != null && text.gameObject != null)
                    UnityEngine.Object.Destroy(text.gameObject);
            })));
        }
    }

    public void divine(PlayerControl p)
    {
        string msg = "";
        Color color = Color.white;

        if (divineResult == DivineResults.BlackWhite)
            if (!Helpers.isNeutral(p) && !p.Data.Role.IsImpostor)
            {
                msg = string.Format(ModTranslation.getString("divineMessageIsCrew"), p.Data.PlayerName);
                color = Color.white;
            }
            else
            {
                msg = string.Format(ModTranslation.getString("divineMessageIsntCrew"), p.Data.PlayerName);
                color = Palette.ImpostorRed;
            }

        else if (divineResult == DivineResults.Team)
            if (!Helpers.isNeutral(p) && !p.Data.Role.IsImpostor)
            {
                msg = string.Format(ModTranslation.getString("divineMessageTeamCrew"), p.Data.PlayerName);
                color = Color.white;
            }
            else if (Helpers.isNeutral(p))
            {
                msg = string.Format(ModTranslation.getString("divineMessageTeamNeutral"), p.Data.PlayerName);
                color = Color.yellow;
            }
            else
            {
                msg = string.Format(ModTranslation.getString("divineMessageTeamImp"), p.Data.PlayerName);
                color = Palette.ImpostorRed;
            }

        else if (divineResult == DivineResults.Role)
            msg = $"{p.Data.PlayerName} was The {string.Join(" ", RoleInfo.getRoleInfoForPlayer(p, false).Select(x => OtherHelper.cs(x.color, x.name)))}";

        if (!string.IsNullOrWhiteSpace(msg))
            fortuneTellerMessage(msg, 7f, color);

        if (Constants.ShouldPlaySfx()) SoundManager.Instance.PlaySound(DestroyableSingleton<HudManager>.Instance.TaskCompleteSound, false, 0.8f);
        numUsed += 1;

        // ռ����g�Ф������Ȥǰk�𤵤��I��������饤����Ȥ�֪ͨ
        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.FortuneTellerUsedDivine, SendOption.Reliable, -1);
        writer.Write(PlayerControl.LocalPlayer.PlayerId);
        writer.Write(p.PlayerId);
        AmongUsClient.Instance.FinishRpcImmediately(writer);
        RPCProcedure.fortuneTellerUsedDivine(PlayerControl.LocalPlayer.PlayerId, p.PlayerId);
    }

    public void clearAndReload()
    {
        meetingFlag = true;
        duration = CustomOptionHolder.fortuneTellerDuration.getFloat();
        List<Arrow> arrows = new List<Arrow>();
        numTasks = (int)CustomOptionHolder.fortuneTellerNumTasks.getFloat();
        distance = CustomOptionHolder.fortuneTellerDistance.getFloat();
        divineResult = (DivineResults)CustomOptionHolder.fortuneTellerResults.getSelection();
        fortuneTeller = null;
        playerStatus = new Dictionary<byte, bool>();
        progress = new Dictionary<byte, float>();
        numUsed = 0;
        divinedFlag = false;
        divineTarget = null;
    }

    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.OnDestroy))]
    class IntroCutsceneOnDestroyPatch
    {
        public void Prefix(IntroCutscene __instance)
        {
            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(16.2f, new Action<float>((p) =>
            {
                if (p == 1f)
                    meetingFlag = false;
            })));
        }
    }
}
