using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reactor.Utilities.Extensions;
using static TheOtherRoles.Roles.TheOtherRoles;
using TheOtherRoles.Patches;
using TheOtherRoles.Players;
using TheOtherRoles.Roles;
using TheOtherRoles.Roles.Roles.Modifier;
using TheOtherRoles.Utilities;
using UnityEngine;
using TheOtherRoles.Modules;
using TheOtherRoles.Roles.Core;
using static TheOtherRoles.Helpers.SabotageHelper;
namespace TheOtherRoles.Helpers;

public static class PlayerHelper
{
    public static PlayerControl playerById(byte id)
    {
        foreach (PlayerControl player in CachedPlayer.AllPlayers)
            if (player.PlayerId == id)
                return player;
        return null;
    }

    public static Dictionary<byte, PlayerControl> allPlayersById()
    {
        Dictionary<byte, PlayerControl> res = new Dictionary<byte, PlayerControl>();
        foreach (PlayerControl player in CachedPlayer.AllPlayers)
            res.Add(player.PlayerId, player);
        return res;
    }


    public static bool hidePlayerName(PlayerControl source, PlayerControl target)
    {
        return source.GetRoleClass().HidePlayerName(target) || source.GetModifierClasses().Any(s => s.HidePlayerName(target));
        if (Camouflager.camouflageTimer > 0f || MushroomSabotageActive()) return true; // No names are visible
        if (!source.Data.Role.IsImpostor && Ninja.isStealthed(target) && Ninja.ninja == target) return true; // Hide Ninja nametags from non-impostors
        if (Sprinter.sprinting && Sprinter.sprinter == target && source != Sprinter.sprinter) return true; // Hide Sprinter nametags
        if (Fox.stealthed && Fox.fox == target && source != Fox.fox) return true; // Hide Fox nametags
        if (SurveillanceMinigamePatch.nightVisionIsActive) return true;
        //else if (Assassin.isInvisble && Assassin.assassin == target) return true;
        else if (!TORMapOptions.hidePlayerNames) return false; // All names are visible
        else if (source == null || target == null) return true;
        else if (source == target) return false; // Player sees his own name
        else if (source.Data.Role.IsImpostor && (target.Data.Role.IsImpostor || target == Spy.spy || target == Sidekick.sidekick && Sidekick.wasTeamRed || target == Jackal.jackal && Jackal.wasTeamRed)) return false; // Members of team Impostors see the names of Impostors/Spies
        else if ((source == Jackal.jackal || source == Sidekick.sidekick) && (target == Jackal.jackal || target == Sidekick.sidekick || target == Jackal.fakeSidekick)) return false; // Members of team Jackal see the names of each other
        else if (Deputy.knowsSheriff && (source == Sheriff.sheriff || source == Deputy.deputy) && (target == Sheriff.sheriff || target == Deputy.deputy)) return false; // Sheriff & Deputy see the names of each other
        else if ((source == Fox.fox || source == Immoralist.immoralist) && (target == Fox.fox || target == Immoralist.immoralist)) return false; // Members of team Fox see the names of each other

        return true;
    }


    public static void showFlash(Color color, float duration = 1f, string message = "")
    {
        if (FastDestroyableSingleton<HudManager>.Instance == null || FastDestroyableSingleton<HudManager>.Instance.FullScreen == null) return;
        FastDestroyableSingleton<HudManager>.Instance.FullScreen.gameObject.SetActive(true);
        FastDestroyableSingleton<HudManager>.Instance.FullScreen.enabled = true;
        // Message Text
        TMPro.TextMeshPro messageText = UnityEngine.Object.Instantiate(FastDestroyableSingleton<HudManager>.Instance.KillButton.cooldownTimerText, FastDestroyableSingleton<HudManager>.Instance.transform);
        messageText.text = message;
        messageText.enableWordWrapping = false;
        messageText.transform.localScale = Vector3.one * 0.5f;
        messageText.transform.localPosition += new Vector3(0f, 2f, -69f);
        messageText.gameObject.SetActive(true);
        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(duration, new Action<float>((p) =>
        {
            var renderer = FastDestroyableSingleton<HudManager>.Instance.FullScreen;

            if (p < 0.5)
                if (renderer != null)
                    renderer.color = new Color(color.r, color.g, color.b, Mathf.Clamp01(p * 2 * 0.75f));
                else
                if (renderer != null)
                    renderer.color = new Color(color.r, color.g, color.b, Mathf.Clamp01((1 - p) * 2 * 0.75f));
            if (p == 1f && renderer != null) renderer.enabled = false;
            if (p == 1f) messageText.gameObject.Destroy();
        })));
    }


}
