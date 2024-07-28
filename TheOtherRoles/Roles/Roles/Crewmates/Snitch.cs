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

namespace TheOtherRoles.Roles.Crewmates;
public sealed class Snitch : RoleBase
{
    public PlayerControl snitch;
    public Color color = new Color32(184, 251, 79, byte.MaxValue);
    public enum Mode
    {
        Chat = 0,
        Map = 1,
        ChatAndMap = 2
    }
    public enum Targets
    {
        EvilPlayers = 0,
        Killers = 1
    }

    public Mode mode = Mode.Chat;
    public Targets targets = Targets.EvilPlayers;
    public int taskCountForReveal = 1;

    public bool isRevealed = false;
    public Dictionary<byte, byte> playerRoomMap = new Dictionary<byte, byte>();
    public TMPro.TextMeshPro text = null;
    public bool needsUpdate = true;

    public override void clearAndReload()
    {
        taskCountForReveal = Mathf.RoundToInt(CustomOptionHolder.snitchLeftTasksForReveal.getFloat());
        snitch = null;
        isRevealed = false;
        playerRoomMap = new Dictionary<byte, byte>();
        if (text != null) UnityEngine.Object.Destroy(text);
        text = null;
        needsUpdate = true;
        mode = (Mode)CustomOptionHolder.snitchMode.getSelection();
        targets = (Targets)CustomOptionHolder.snitchTargets.getSelection();
    }
}
