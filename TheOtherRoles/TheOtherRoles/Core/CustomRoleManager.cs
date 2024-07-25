using AmongUs.GameOptions;
using HarmonyLib;
using Hazel;
using Il2CppSystem.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using TheOtherRoles.Modules;
using TheOtherRoles.TheOtherRoles.Core;
using static TheOtherRoles.ModTranslation;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.ParticleSystem.PlaybackState;

namespace TheOtherRoles.TheOtherRoles.Core;

public static class CustomRoleManager
{
    public static Type[] AllRolesClassType;
    public static Dictionary<RoleId, RoleInfo> AllRolesInfo = new(CustomRolesHelper.AllRoles.Length);
    public static Dictionary<byte, RoleBase> AllActiveRoles = new();

    public static RoleInfo GetRoleInfo(this RoleId role) => AllRolesInfo.ContainsKey(role) ? AllRolesInfo[role] : null;
    public static RoleBase GetRoleClass(this PlayerControl player) => GetByPlayerId(player.PlayerId);
    public static RoleBase GetByPlayerId(byte playerId) => AllActiveRoles.TryGetValue(playerId, out var roleBase) ? roleBase : null;
    public static void Do<T>(this List<T> list, Action<T> action) => list.ToArray().Do(action);
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
