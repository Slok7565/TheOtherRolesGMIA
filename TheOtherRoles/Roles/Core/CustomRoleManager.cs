using AmongUs.GameOptions;
using HarmonyLib;
using Hazel;
using Il2CppSystem.Diagnostics.Tracing;
using Il2CppSystem.Text;
using Rewired.Utils.Platforms.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using TheOtherRoles.Modules;
using TheOtherRoles.Roles.Core;
using UnityEngine.Playables;
using static TheOtherRoles.Helpers.EnumHelper;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.ParticleSystem.PlaybackState;

namespace TheOtherRoles.Roles.Core;

public static class CustomRoleManager
{
    /// <summary>所有角色（不包括附加）</summary>
    public static readonly RoleId[] AllRoles = GetAllValues<RoleId>().Where(role => role < RoleId.NotAssigned).ToArray();
    /// <summary>所有附加</summary>
    public static readonly RoleId[] AllModifiers = GetAllValues<RoleId>().Where(role => role > RoleId.NotAssigned).ToArray();
    /// <summary>可以在标准模式下出现的所有角色</summary>
    public static readonly RoleId[] AllStandardRoles = AllRoles.Concat(AllModifiers).ToList().ToArray();
    /// <summary>所有职业类型</summary>

    public static Type[] AllRolesClassType;
    public static Dictionary<RoleId, RoleInfo> AllRolesInfo = new(AllRoles.Length);
    public static Dictionary<RoleId, RoleInfo> AllModifiersInfo = new(AllModifiers.Length);
    public static Dictionary<byte, RoleBase> AllActiveRoles = new();
    public static Dictionary<byte, List<ModifierBase>> AllActiveModifiers = new();

    public static RoleInfo GetRoleInfo(this RoleId role) => AllRolesInfo.ContainsKey(role) ? AllRolesInfo[role] : null;

    public static RoleBase GetRoleClass(this PlayerControl player) => GetByPlayerId(player.PlayerId);
    public static RoleBase GetByPlayerId(byte playerId) => AllActiveRoles.TryGetValue(playerId, out var roleBase) ? roleBase : null;
    public static List<ModifierBase> GetModifierClasses(this PlayerControl player) => GetMByPlayerId(player.PlayerId);
    public static List<ModifierBase> GetMByPlayerId(byte playerId) => AllActiveModifiers.TryGetValue(playerId, out var roleBase) ? roleBase : null;
    public static void Do<T>(this List<T> list, Action<T> action) => list.ToArray().Do(action);



    public static void CreateInstance(RoleId role, PlayerControl player)
    {
        if (AllRolesInfo.TryGetValue(role, out var roleInfo))
        {
            roleInfo.CreateRoleInstance(player).Add();
            roleInfo.CreateRoleInstance(player).clearAndReload();
        }
        else if (AllModifiersInfo.TryGetValue(role, out var roleInfo1))
        {
            roleInfo1.CreateModifierInstance(player).clearAndReload();
        }
    }
    public static void DispatchRpc(MessageReader reader)
    {
        var playerId = reader.ReadByte();
        GetByPlayerId(playerId)?.ReceiveRPC(reader);
    }
}
public enum RoleId
{
    Jester,
    Mayor,
    Portalmaker,
    Engineer,
    Sheriff,
    Deputy,
    Lighter,
    Godfather,
    Mafioso,
    Janitor,
    Detective,
    TimeMaster,
    Medic,
    Swapper,
    Seer,
    Sprinter,
    Morphling,
    Camouflager,
    Hacker,
    Tracker,
    Vampire,
    Catcher,
    Snitch,
    Jackal,
    Sidekick,
    Eraser,
    FortuneTeller,
    Bait,
    Veteran,
    Sherlock,
    Spy,
    Trickster,
    Cleaner,
    Warlock,
    SecurityGuard,
    Arsonist,
    EvilGuesser,
    NiceGuesser,
    NiceWatcher,
    EvilWatcher,
    BountyHunter,
    Vulture,
    Medium,
    Shifter,
    Yasuna,
    Prophet,
    TaskMaster,
    Teleporter,
    EvilYasuna,
    //Trapper,
    Lawyer,
    //Prosecutor,
    Pursuer,
    Moriarty,
    PlagueDoctor,
    Akujo,
    Cupid,
    JekyllAndHyde,
    Fox,
    Immoralist,
    Witch,
    Assassin,
    Ninja,
    NekoKabocha,
    Thief,
    SerialKiller,
    EvilTracker,
    MimicK,
    MimicA,
    BomberA,
    BomberB,
    EvilHacker,
    Undertaker,
    Trapper,
    Blackmailer,
    Opportunist,
    Madmate,
    //Bomber,
    Crewmate,
    Impostor,
    NotAssigned,
    // Modifier ---
    Lover,
    //Bait, Bait is no longer a modifier
    Bloody,
    AntiTeleport,
    Tiebreaker,
    Sunglasses,
    Mini,
    Vip,
    Invert,
    Chameleon,
    //Shifter
}
public enum HasTask
{
    True,
    False,
    ForRecompute
}
public enum CountTypes
{
    OutOfGame = CustomWinner.None,
    None = CustomWinner.None,
    Crew = CustomWinner.Crewmate,
    Impostor = CustomWinner.Impostor,
    Jackal = CustomWinner.Jackal,
}
enum CustomWinner
{
    None,
    Impostor,
    Crewmate,
    LoverTeam,
    LoverSolo,
    Jester,
    Jackal,
    Mini,
    Arsonist,
    Vulture,
    LawyerSolo,
    AdditionalLawyerBonus,
    AdditionalAlivePursuer,
    AdditionalLawyerStolen,
    Opportunist,
    Fox,
    Moriarty,
    Akujo,
    PlagueDoctor,
    JekyllAndHyde,
    CupidLovers,
    EveryoneDied
    //ProsecutorWin
}

