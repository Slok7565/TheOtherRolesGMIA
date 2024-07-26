﻿using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using System;
using UnityEngine.Windows.Speech;
using static UnityEngine.GraphicsBuffer;
using TheOtherRoles.Roles;

namespace TheOtherRoles.Patches
{
    [HarmonyPatch]
    public static class TransportationToolPatches
    {
        /* 
         * Moving Plattform / Zipline / Ladders move the player out of bounds, thus we want to disable functions of the mod if the player is currently using one of these.
         * Save the players anti tp position before using it.
         * 
         * Zipline can also break camo, fix that one too.
         */

        public static bool isUsingTransportation(PlayerControl pc)
        {
            return pc.inMovingPlat || pc.onLadder;
        }

        // Zipline:
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ZiplineBehaviour), nameof(ZiplineBehaviour.Use), new Type[] { typeof(PlayerControl), typeof(bool) })]
        public static void prefix3(ZiplineBehaviour __instance, PlayerControl player, bool fromTop)
        {
            AntiTeleport.position = Players.CachedPlayer.LocalPlayer.transform.position;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ZiplineBehaviour), nameof(ZiplineBehaviour.Use), new Type[] { typeof(PlayerControl), typeof(bool) })]
        public static void postfix(ZiplineBehaviour __instance, PlayerControl player, bool fromTop)
        {
            // Fix camo:
            __instance.StartCoroutine(Effects.Lerp(fromTop ? __instance.downTravelTime : __instance.upTravelTime, new System.Action<float>((p) => {
                HandZiplinePoolable hand;
                __instance.playerIdHands.TryGetValue(player.PlayerId, out hand);
                if (hand != null)
                {
                    if (Camouflager.camouflageTimer <= 0 && !Helpers.MushroomSabotageActive())
                    {
                        if (player == Morphling.morphling && Morphling.morphTimer > 0)
                        {
                            hand.SetPlayerColor(Morphling.morphTarget.CurrentOutfit, PlayerMaterial.MaskType.None);
                            // Also set hat color, cause the line destroys it...
                            player.RawSetHat(Morphling.morphTarget.Data.DefaultOutfit.HatId, Morphling.morphTarget.Data.DefaultOutfit.ColorId);
                        }
                        else if (player == MimicK.mimicK && MimicK.victim != null)
                        {
                            hand.SetPlayerColor(MimicK.victim.CurrentOutfit, PlayerMaterial.MaskType.None);
                            player.RawSetHat(MimicK.victim.Data.DefaultOutfit.HatId, MimicK.victim.Data.DefaultOutfit.ColorId);
                        }
                        else if (player == MimicA.mimicA && MimicK.mimicK != null && MimicA.isMorph)
                        {
                            hand.SetPlayerColor(MimicK.mimicK.CurrentOutfit, PlayerMaterial.MaskType.None);
                            player.RawSetHat(MimicK.mimicK.Data.DefaultOutfit.HatId, MimicK.mimicK.Data.DefaultOutfit.ColorId);
                        }
                        else
                        {
                            hand.SetPlayerColor(player.CurrentOutfit, PlayerMaterial.MaskType.None);
                        }
                    }
                    else
                    {
                        PlayerMaterial.SetColors(6, hand.handRenderer);
                    }

                    if ((Ninja.ninja != null && Ninja.ninja == player && Ninja.stealthed) || (Sprinter.sprinter != null && Sprinter.sprinter == player && Sprinter.sprinting)) hand.gameObject.SetActive(false);
                }
            })));
        }

        // Save the position of the player prior to starting the climb / gap platform
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.ClimbLadder))]
        public static void prefix()
        {
            AntiTeleport.position = Players.CachedPlayer.LocalPlayer.transform.position;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.ClimbLadder))]
        public static void postfix2(PlayerPhysics __instance, Ladder source, byte climbLadderSid)
        {
            // Fix camo:
            var player = __instance.myPlayer;
            __instance.StartCoroutine(Effects.Lerp(5.0f, new System.Action<float>((p) => {
                if (Camouflager.camouflageTimer <= 0 && !Helpers.MushroomSabotageActive())
                {
                    if (player == Morphling.morphling && Morphling.morphTimer > 0.1f)
                        player.RawSetHat(Morphling.morphTarget.Data.DefaultOutfit.HatId, Morphling.morphTarget.Data.DefaultOutfit.ColorId);
                    else if (player == MimicK.mimicK && MimicK.victim != null && MimicK.victim.Data != null)
                        player.RawSetHat(MimicK.victim.Data.DefaultOutfit.HatId, MimicK.victim.Data.DefaultOutfit.ColorId);
                    else if (player == MimicA.mimicA && MimicK.mimicK != null && MimicA.isMorph)
                        player.RawSetHat(MimicK.mimicK.Data.DefaultOutfit.HatId, MimicK.mimicK.Data.DefaultOutfit.ColorId);
                }
            })));
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(MovingPlatformBehaviour), nameof(MovingPlatformBehaviour.UsePlatform))]
        public static void prefix2()
        {
            AntiTeleport.position = Players.CachedPlayer.LocalPlayer.transform.position;
        }
    }
}