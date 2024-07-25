using System.Linq;
using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using TheOtherRoles.Objects;
using TheOtherRoles.Players;
using TheOtherRoles.Utilities;
using TheOtherRoles.CustomGameModes;
using TheOtherRoles.Role.TheOtherRoles;
using AmongUs.Data;
using Hazel;
using JetBrains.Annotations;
using Steamworks;
using TheOtherRoles.Patches;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;
using Reactor.Utilities.Extensions;
using Types = TheOtherRoles.CustomOption.CustomOptionType;
using TheOtherRoles.TheOtherRoles.Core;

namespace TheOtherRoles.Role
{
    [HarmonyPatch]
    public static class TheOtherRoles
    {
        public static System.Random rnd = new((int)DateTime.Now.Ticks);


        public sealed class Portalmaker : RoleBase
        {
            public PlayerControl portalmaker;
            public Color color = new Color32(69, 69, 169, byte.MaxValue);

            public float cooldown;
            public float usePortalCooldown;
            public bool logOnlyHasColors;
            public bool logShowsTime;
            public bool canPortalFromAnywhere;

            private Sprite placePortalButtonSprite;
            private Sprite usePortalButtonSprite;
            private Sprite usePortalSpecialButtonSprite1;
            private Sprite usePortalSpecialButtonSprite2;
            private Sprite logSprite;

            public Sprite getPlacePortalButtonSprite()
            {
                if (placePortalButtonSprite) return placePortalButtonSprite;
                placePortalButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.PlacePortalButton.png", 115f);
                return placePortalButtonSprite;
            }

            public Sprite getUsePortalButtonSprite()
            {
                if (usePortalButtonSprite) return usePortalButtonSprite;
                usePortalButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.UsePortalButton.png", 115f);
                return usePortalButtonSprite;
            }

            public Sprite getUsePortalSpecialButtonSprite(bool first)
            {
                if (first)
                {
                    if (usePortalSpecialButtonSprite1) return usePortalSpecialButtonSprite1;
                    usePortalSpecialButtonSprite1 = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.UsePortalSpecialButton1.png", 115f);
                    return usePortalSpecialButtonSprite1;
                }
                else
                {
                    if (usePortalSpecialButtonSprite2) return usePortalSpecialButtonSprite2;
                    usePortalSpecialButtonSprite2 = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.UsePortalSpecialButton2.png", 115f);
                    return usePortalSpecialButtonSprite2;
                }
            }

            public Sprite getLogSprite()
            {
                if (logSprite) return logSprite;
                logSprite = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.DoorLogsButton].Image;
                return logSprite;
            }

            public override void clearAndReload()
            {
                portalmaker = null;
                cooldown = CustomOptionHolder.portalmakerCooldown.getFloat();
                usePortalCooldown = CustomOptionHolder.portalmakerUsePortalCooldown.getFloat();
                logOnlyHasColors = CustomOptionHolder.portalmakerLogOnlyColorType.getBool();
                logShowsTime = CustomOptionHolder.portalmakerLogHasTime.getBool();
                canPortalFromAnywhere = CustomOptionHolder.portalmakerCanPortalFromAnywhere.getBool();
            }


        }

        public sealed class Mayor : RoleBase
        {
            public PlayerControl mayor;
            public Color color = new Color32(32, 77, 66, byte.MaxValue);
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
                emergencySprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.EmergencyButton.png", 550f);
                return emergencySprite;
            }

            public override void clearAndReload()
            {
                mayor = null;
                emergency = null;
                emergencySprite = null;
                remoteMeetingsLeft = Mathf.RoundToInt(CustomOptionHolder.mayorMaxRemoteMeetings.getFloat());
                canSeeVoteColors = CustomOptionHolder.mayorCanSeeVoteColors.getBool();
                tasksNeededToSeeVoteColors = (int)CustomOptionHolder.mayorTasksNeededToSeeVoteColors.getFloat();
                meetingButton = CustomOptionHolder.mayorMeetingButton.getBool();
                mayorChooseSingleVote = CustomOptionHolder.mayorChooseSingleVote.getSelection();
                voteTwice = true;
            }
        }

        public sealed class Engineer : RoleBase
        {
            public PlayerControl engineer;
            public Color color = new Color32(0, 40, 245, byte.MaxValue);
            private Sprite buttonSprite;

            public int remainingFixes = 1;
            public bool highlightForImpostors = true;
            public bool highlightForTeamJackal = true;

            public Sprite getButtonSprite()
            {
                if (buttonSprite) return buttonSprite;
                buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.RepairButton.png", 115f);
                return buttonSprite;
            }

            public override void clearAndReload()
            {
                engineer = null;
                remainingFixes = Mathf.RoundToInt(CustomOptionHolder.engineerNumberOfFixes.getFloat());
                highlightForImpostors = CustomOptionHolder.engineerHighlightForImpostors.getBool();
                highlightForTeamJackal = CustomOptionHolder.engineerHighlightForTeamJackal.getBool();
            }
        }

        public sealed class Godfather : RoleBase
        {
            public PlayerControl godfather;
            public Color color = Palette.ImpostorRed;

            public override void clearAndReload()
            {
                godfather = null;
            }
        }

        public sealed class Mafioso : RoleBase
        {
            public PlayerControl mafioso;
            public Color color = Palette.ImpostorRed;

            public override void clearAndReload()
            {
                mafioso = null;
            }
        }


        public sealed class Janitor : RoleBase
        {
            public PlayerControl janitor;
            public Color color = Palette.ImpostorRed;

            public float cooldown = 30f;

            private Sprite buttonSprite;
            public Sprite getButtonSprite()
            {
                if (buttonSprite) return buttonSprite;
                buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.CleanButton.png", 115f);
                return buttonSprite;
            }

            public override void clearAndReload()
            {
                janitor = null;
                cooldown = CustomOptionHolder.janitorCooldown.getFloat();
            }
        }

        public sealed class Sheriff : RoleBase
        {
            public PlayerControl sheriff;
            public Color color = new Color32(248, 205, 70, byte.MaxValue);

            public float cooldown = 30f;
            public bool canKillNeutrals = false;
            public bool spyCanDieToSheriff = false;

            public PlayerControl currentTarget;

            public PlayerControl formerDeputy;  // Needed for keeping handcuffs + shifting
            public PlayerControl formerSheriff;  // When deputy gets promoted...

            public void replaceCurrentSheriff(PlayerControl deputy)
            {
                if (!formerSheriff) formerSheriff = sheriff;
                sheriff = deputy;
                currentTarget = null;
                cooldown = CustomOptionHolder.sheriffCooldown.getFloat();
            }

            public override void clearAndReload()
            {
                sheriff = null;
                currentTarget = null;
                formerDeputy = null;
                formerSheriff = null;
                cooldown = CustomOptionHolder.sheriffCooldown.getFloat();
                canKillNeutrals = CustomOptionHolder.sheriffCanKillNeutrals.getBool();
                spyCanDieToSheriff = CustomOptionHolder.spyCanDieToSheriff.getBool();
            }
        }

        public sealed class Deputy : RoleBase
        {
            public PlayerControl deputy;
            public Color color = Sheriff.color;

            public PlayerControl currentTarget;
            public List<byte> handcuffedPlayers = new List<byte>();
            public int promotesToSheriff; // No: 0, Immediately: 1, After Meeting: 2
            public bool keepsHandcuffsOnPromotion;
            public float handcuffDuration;
            public float remainingHandcuffs;
            public float handcuffCooldown;
            public bool knowsSheriff;
            public bool stopsGameEnd;
            public Dictionary<byte, float> handcuffedKnows = new Dictionary<byte, float>();

            private Sprite buttonSprite;
            private Sprite handcuffedSprite;

            public Sprite getButtonSprite()
            {
                if (buttonSprite) return buttonSprite;
                buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.DeputyHandcuffButton.png", 115f);
                return buttonSprite;
            }

            public Sprite getHandcuffedButtonSprite()
            {
                if (handcuffedSprite) return handcuffedSprite;
                handcuffedSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.DeputyHandcuffed.png", 115f);
                return handcuffedSprite;
            }

            // Can be used to enable / disable the handcuff effect on the target's buttons
            public void setHandcuffedKnows(bool active = true, byte playerId = byte.MaxValue)
            {
                if (playerId == byte.MaxValue)
                    playerId = CachedPlayer.LocalPlayer.PlayerId;

                if (active && playerId == CachedPlayer.LocalPlayer.PlayerId)
                {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.ShareGhostInfo, SendOption.Reliable, -1);
                    writer.Write(CachedPlayer.LocalPlayer.PlayerId);
                    writer.Write((byte)RPCProcedure.GhostInfoTypes.HandcuffNoticed);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }

                if (active)
                {
                    handcuffedKnows.Add(playerId, handcuffDuration);
                    handcuffedPlayers.RemoveAll(x => x == playerId);
                }

                if (playerId == CachedPlayer.LocalPlayer.PlayerId)
                {
                    HudManagerStartPatch.setAllButtonsHandcuffedStatus(active);
                    SoundEffectsManager.play("deputyHandcuff");
                }

            }

            public void clearAndReload()
            {
                deputy = null;
                currentTarget = null;
                handcuffedPlayers = new List<byte>();
                handcuffedKnows = new Dictionary<byte, float>();
                HudManagerStartPatch.setAllButtonsHandcuffedStatus(false, true);
                promotesToSheriff = CustomOptionHolder.deputyGetsPromoted.getSelection();
                remainingHandcuffs = CustomOptionHolder.deputyNumberOfHandcuffs.getFloat();
                handcuffCooldown = CustomOptionHolder.deputyHandcuffCooldown.getFloat();
                keepsHandcuffsOnPromotion = CustomOptionHolder.deputyKeepsHandcuffs.getBool();
                handcuffDuration = CustomOptionHolder.deputyHandcuffDuration.getFloat();
                knowsSheriff = CustomOptionHolder.deputyKnowsSheriff.getBool();
                stopsGameEnd = CustomOptionHolder.deputyStopsGameEnd.getBool();
            }
        }

        public sealed class Lighter : RoleBase
        {
            public PlayerControl lighter;
            public Color color = new Color32(238, 229, 190, byte.MaxValue);

            public float lighterModeLightsOnVision = 2f;
            public float lighterModeLightsOffVision = 0.75f;
            public float flashlightWidth = 0.75f;
            public bool canSeeInvisible = true;

            public override void clearAndReload()
            {
                lighter = null;
                flashlightWidth = CustomOptionHolder.lighterFlashlightWidth.getFloat();
                lighterModeLightsOnVision = CustomOptionHolder.lighterModeLightsOnVision.getFloat();
                lighterModeLightsOffVision = CustomOptionHolder.lighterModeLightsOffVision.getFloat();
                canSeeInvisible = CustomOptionHolder.lighterCanSeeInvisible.getBool();
            }
        }

        public sealed class Detective : RoleBase
        {
            public PlayerControl detective;
            public Color color = new Color32(45, 106, 165, byte.MaxValue);

            public float footprintIntervall = 1f;
            public float footprintDuration = 1f;
            public bool anonymousFootprints = false;
            public float reportNameDuration = 0f;
            public float reportColorDuration = 20f;
            public float timer = 6.2f;

            public override void clearAndReload()
            {
                detective = null;
                anonymousFootprints = CustomOptionHolder.detectiveAnonymousFootprints.getBool();
                footprintIntervall = CustomOptionHolder.detectiveFootprintIntervall.getFloat();
                footprintDuration = CustomOptionHolder.detectiveFootprintDuration.getFloat();
                reportNameDuration = CustomOptionHolder.detectiveReportNameDuration.getFloat();
                reportColorDuration = CustomOptionHolder.detectiveReportColorDuration.getFloat();
                timer = 6.2f;
            }
        }
    }

    public sealed class TimeMaster : RoleBase
    {
        public PlayerControl timeMaster;
        public Color color = new Color32(112, 142, 239, byte.MaxValue);

        public bool reviveDuringRewind = false;
        public float rewindTime = 3f;
        public float shieldDuration = 3f;
        public float cooldown = 30f;

        public bool shieldActive = false;
        public bool isRewinding = false;

        private Sprite buttonSprite;
        public Sprite getButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.TimeShieldButton.png", 115f);
            return buttonSprite;
        }

        public override void clearAndReload()
        {
            timeMaster = null;
            isRewinding = false;
            shieldActive = false;
            rewindTime = CustomOptionHolder.timeMasterRewindTime.getFloat();
            shieldDuration = CustomOptionHolder.timeMasterShieldDuration.getFloat();
            cooldown = CustomOptionHolder.timeMasterCooldown.getFloat();
        }
    }

    public sealed class Medic : RoleBase
    {
        public PlayerControl medic;
        public PlayerControl shielded;
        public PlayerControl futureShielded;

        public Color color = new Color32(126, 251, 194, byte.MaxValue);
        public bool usedShield;

        public int showShielded = 0;
        public bool showAttemptToShielded = false;
        public bool showAttemptToMedic = false;
        public bool setShieldAfterMeeting = false;
        public bool showShieldAfterMeeting = false;
        public bool meetingAfterShielding = false;

        public Color shieldedColor = new Color32(0, 221, 255, byte.MaxValue);
        public PlayerControl currentTarget;

        private Sprite buttonSprite;
        public Sprite getButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.ShieldButton.png", 115f);
            return buttonSprite;
        }

        public bool shieldVisible(PlayerControl target)
        {
            bool hasVisibleShield = false;

            bool isMorphedMorphling = target == Morphling.morphling && Morphling.morphTarget != null && Morphling.morphTimer > 0f;
            bool isMimicKShield = target == MimicK.mimicK && MimicK.victim != null;
            bool isMimicAMorph = target == MimicA.mimicA && MimicA.isMorph;
            if (shielded != null && (target == shielded && !isMorphedMorphling && !isMimicKShield && !isMimicAMorph || isMorphedMorphling && Morphling.morphTarget == shielded || isMimicAMorph && MimicK.mimicK == shielded))
            {
                hasVisibleShield = showShielded == 0 || Helpers.shouldShowGhostInfo() // Everyone or Ghost info
                    || showShielded == 1 && (CachedPlayer.LocalPlayer.PlayerControl == shielded || CachedPlayer.LocalPlayer.PlayerControl == medic) // Shielded + Medic
                    || showShielded == 2 && CachedPlayer.LocalPlayer.PlayerControl == medic; // Medic only
                // Make shield invisible till after the next meeting if the option is set (the medic can already see the shield)
                hasVisibleShield = hasVisibleShield && (meetingAfterShielding || !showShieldAfterMeeting || CachedPlayer.LocalPlayer.PlayerControl == medic || Helpers.shouldShowGhostInfo());
            }
            return hasVisibleShield;
        }

        public override void clearAndReload()
        {
            medic = null;
            shielded = null;
            futureShielded = null;
            currentTarget = null;
            usedShield = false;
            showShielded = CustomOptionHolder.medicShowShielded.getSelection();
            showAttemptToShielded = CustomOptionHolder.medicShowAttemptToShielded.getBool();
            showAttemptToMedic = CustomOptionHolder.medicShowAttemptToMedic.getBool();
            setShieldAfterMeeting = CustomOptionHolder.medicSetOrShowShieldAfterMeeting.getSelection() == 2;
            showShieldAfterMeeting = CustomOptionHolder.medicSetOrShowShieldAfterMeeting.getSelection() == 1;
            meetingAfterShielding = false;
        }
    }

    public sealed class Catcher : RoleBase
    {
        public PlayerControl catcher;
        public Color color = new Color32(71, 95, 250, byte.MaxValue);
        public PlayerControl neartarget;
        public PlayerControl target;
        private Sprite CatchButton;
        public float catchchance;
        public float catchcooldown;

        public Sprite getCheckSprite()
        {
            if (CatchButton) return CatchButton;
            CatchButton = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.SwapperCheck.png", 150f);
            return CatchButton;
        }

        public void clearAndReload()
        {
            catcher = null;
            target = null;
            neartarget = null;
            catchchance = CustomOptionHolder.catchChance.getFloat();
            catchcooldown = CustomOptionHolder.catchCooldown.getFloat();
        }

    }






    public sealed class Swapper : RoleBase
    {
        public PlayerControl swapper;
        public Color color = new Color32(134, 55, 86, byte.MaxValue);
        private Sprite spriteCheck;
        public bool canCallEmergency = false;
        public bool canOnlySwapOthers = false;
        public int charges;
        public float rechargeTasksNumber;
        public float rechargedTasks;

        public byte playerId1 = byte.MaxValue;
        public byte playerId2 = byte.MaxValue;

        public Sprite getCheckSprite()
        {
            if (spriteCheck) return spriteCheck;
            spriteCheck = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.SwapperCheck.png", 150f);
            return spriteCheck;
        }

        public override void clearAndReload()
        {
            swapper = null;
            playerId1 = byte.MaxValue;
            playerId2 = byte.MaxValue;
            canCallEmergency = CustomOptionHolder.swapperCanCallEmergency.getBool();
            canOnlySwapOthers = CustomOptionHolder.swapperCanOnlySwapOthers.getBool();
            charges = Mathf.RoundToInt(CustomOptionHolder.swapperSwapsNumber.getFloat());
            rechargeTasksNumber = Mathf.RoundToInt(CustomOptionHolder.swapperRechargeTasksNumber.getFloat());
            rechargedTasks = Mathf.RoundToInt(CustomOptionHolder.swapperRechargeTasksNumber.getFloat());
        }
    }

    public sealed class Lovers : RoleBase
    {
        public PlayerControl lover1;
        public PlayerControl lover2;
        public Color color = new Color32(232, 57, 185, byte.MaxValue);

        public bool bothDie = true;
        public bool enableChat = true;
        // Lovers save if next to be exiled is a lover, because RPC of ending game comes before RPC of exiled
        public bool notAckedExiledIsLover = false;

        public bool existing()
        {
            return lover1 != null && lover2 != null && !lover1.Data.Disconnected && !lover2.Data.Disconnected;
        }

        public bool existingAndAlive()
        {
            return existing() && !lover1.Data.IsDead && !lover2.Data.IsDead && !notAckedExiledIsLover; // ADD NOT ACKED IS LOVER
        }

        public PlayerControl otherLover(PlayerControl oneLover)
        {
            if (!existingAndAlive()) return null;
            if (oneLover == lover1) return lover2;
            if (oneLover == lover2) return lover1;
            return null;
        }

        public bool existingWithKiller()
        {
            return existing() && (lover1 == Jackal.jackal || lover2 == Jackal.jackal
                               || lover1 == Sidekick.sidekick || lover2 == Sidekick.sidekick
                               || lover1.Data.Role.IsImpostor || lover2.Data.Role.IsImpostor);
        }

        public bool hasAliveKillingLover(this PlayerControl player)
        {
            if (!Lovers.existingAndAlive() || !existingWithKiller())
                return false;
            return player != null && (player == lover1 || player == lover2);
        }

        public override void clearAndReload()
        {
            lover1 = null;
            lover2 = null;
            notAckedExiledIsLover = false;
            bothDie = CustomOptionHolder.modifierLoverBothDie.getBool();
            enableChat = CustomOptionHolder.modifierLoverEnableChat.getBool();
        }

        public PlayerControl getPartner(this PlayerControl player)
        {
            if (player == null)
                return null;
            if (lover1 == player)
                return lover2;
            if (lover2 == player)
                return lover1;
            return null;
        }
    }

    public sealed class Seer : RoleBase
    {
        public PlayerControl seer;
        public Color color = new Color32(97, 178, 108, byte.MaxValue);
        public List<Vector3> deadBodyPositions = new List<Vector3>();

        public float soulDuration = 15f;
        public bool limitSoulDuration = false;
        public int mode = 0;

        private Sprite soulSprite;
        public Sprite getSoulSprite()
        {
            if (soulSprite) return soulSprite;
            soulSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.Soul.png", 500f);
            return soulSprite;
        }

        public override void clearAndReload()
        {
            seer = null;
            deadBodyPositions = new List<Vector3>();
            limitSoulDuration = CustomOptionHolder.seerLimitSoulDuration.getBool();
            soulDuration = CustomOptionHolder.seerSoulDuration.getFloat();
            mode = CustomOptionHolder.seerMode.getSelection();
        }
    }

    public sealed class Morphling : RoleBase
    {
        public PlayerControl morphling;
        public Color color = Palette.ImpostorRed;
        private Sprite sampleSprite;
        private Sprite morphSprite;

        public float cooldown = 30f;
        public float duration = 10f;

        public PlayerControl currentTarget;
        public PlayerControl sampledTarget;
        public PlayerControl morphTarget;
        public float morphTimer = 0f;

        public void resetMorph()
        {
            morphTarget = null;
            morphTimer = 0f;
            if (morphling == null) return;
            morphling.setDefaultLook();
        }

        public override void clearAndReload()
        {
            resetMorph();
            morphling = null;
            currentTarget = null;
            sampledTarget = null;
            morphTarget = null;
            morphTimer = 0f;
            cooldown = CustomOptionHolder.morphlingCooldown.getFloat();
            duration = CustomOptionHolder.morphlingDuration.getFloat();
        }

        public Sprite getSampleSprite()
        {
            if (sampleSprite) return sampleSprite;
            sampleSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.SampleButton.png", 115f);
            return sampleSprite;
        }

        public Sprite getMorphSprite()
        {
            if (morphSprite) return morphSprite;
            morphSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.MorphButton.png", 115f);
            return morphSprite;
        }
    }

    public sealed class Camouflager : RoleBase
    {
        public PlayerControl camouflager;
        public Color color = Palette.ImpostorRed;

        public float cooldown = 30f;
        public float duration = 10f;
        public float camouflageTimer = 0f;

        private Sprite buttonSprite;
        public Sprite getButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.CamoButton.png", 115f);
            return buttonSprite;
        }

        public void resetCamouflage()
        {
            camouflageTimer = 0f;
            foreach (PlayerControl p in CachedPlayer.AllPlayers)
                /*if ((p == Ninja.ninja && Ninja.stealthed) || (p == Sprinter.sprinter && Sprinter.sprinting))
                    continue;*/
                p.setDefaultLook();
        }

        public override void clearAndReload()
        {
            resetCamouflage();
            camouflager = null;
            camouflageTimer = 0f;
            cooldown = CustomOptionHolder.camouflagerCooldown.getFloat();
            duration = CustomOptionHolder.camouflagerDuration.getFloat();
        }
    }

    public sealed class Hacker : RoleBase
    {
        public PlayerControl hacker;
        public Minigame vitals = null;
        public Minigame doorLog = null;
        public Color color = new Color32(117, 250, 76, byte.MaxValue);

        public float cooldown = 30f;
        public float duration = 10f;
        public float toolsNumber = 5f;
        public bool onlyColorType = false;
        public float hackerTimer = 0f;
        public int rechargeTasksNumber = 2;
        public int rechargedTasks = 2;
        public int chargesVitals = 1;
        public int chargesAdminTable = 1;
        public bool cantMove = true;

        private Sprite buttonSprite;
        private Sprite vitalsSprite;
        private Sprite logSprite;
        private Sprite adminSprite;

        public Sprite getButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.HackerButton.png", 115f);
            return buttonSprite;
        }

        public Sprite getVitalsSprite()
        {
            if (vitalsSprite) return vitalsSprite;
            vitalsSprite = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.VitalsButton].Image;
            return vitalsSprite;
        }

        public Sprite getLogSprite()
        {
            if (logSprite) return logSprite;
            logSprite = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.DoorLogsButton].Image;
            return logSprite;
        }

        public Sprite getAdminSprite()
        {
            byte mapId = GameOptionsManager.Instance.currentNormalGameOptions.MapId;
            UseButtonSettings button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.PolusAdminButton]; // Polus
            if (Helpers.isSkeld() || mapId == 3) button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.AdminMapButton]; // Skeld || Dleks
            else if (Helpers.isMira()) button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.MIRAAdminButton]; // Mira HQ
            else if (Helpers.isAirship()) button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.AirshipAdminButton]; // Airship
            else if (Helpers.isFungle()) button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.AdminMapButton];
            adminSprite = button.Image;
            return adminSprite;
        }

        public override void clearAndReload()
        {
            hacker = null;
            vitals = null;
            doorLog = null;
            hackerTimer = 0f;
            adminSprite = null;
            cooldown = CustomOptionHolder.hackerCooldown.getFloat();
            duration = CustomOptionHolder.hackerHackeringDuration.getFloat();
            onlyColorType = CustomOptionHolder.hackerOnlyColorType.getBool();
            toolsNumber = CustomOptionHolder.hackerToolsNumber.getFloat();
            rechargeTasksNumber = Mathf.RoundToInt(CustomOptionHolder.hackerRechargeTasksNumber.getFloat());
            rechargedTasks = Mathf.RoundToInt(CustomOptionHolder.hackerRechargeTasksNumber.getFloat());
            chargesVitals = Mathf.RoundToInt(CustomOptionHolder.hackerToolsNumber.getFloat()) / 2;
            chargesAdminTable = Mathf.RoundToInt(CustomOptionHolder.hackerToolsNumber.getFloat()) / 2;
            cantMove = CustomOptionHolder.hackerNoMove.getBool();
        }
    }

    public sealed class Tracker : RoleBase
    {
        public PlayerControl tracker;
        public Color color = new Color32(100, 58, 220, byte.MaxValue);
        public List<Arrow> localArrows = new List<Arrow>();

        public float updateIntervall = 5f;
        public bool resetTargetAfterMeeting = false;
        public bool canTrackCorpses = false;
        public float corpsesTrackingCooldown = 30f;
        public float corpsesTrackingDuration = 5f;
        public float corpsesTrackingTimer = 0f;
        public int trackingMode = 0;
        public List<Vector3> deadBodyPositions = new List<Vector3>();

        public PlayerControl currentTarget;
        public PlayerControl tracked;
        public bool usedTracker = false;
        public float timeUntilUpdate = 0f;
        public Arrow arrow = new Arrow(Color.blue);

        public GameObject DangerMeterParent;
        public DangerMeter Meter;

        private Sprite trackCorpsesButtonSprite;
        public Sprite getTrackCorpsesButtonSprite()
        {
            if (trackCorpsesButtonSprite) return trackCorpsesButtonSprite;
            trackCorpsesButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.PathfindButton.png", 115f);
            return trackCorpsesButtonSprite;
        }

        private Sprite buttonSprite;
        public Sprite getButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.TrackerButton.png", 115f);
            return buttonSprite;
        }

        public void resetTracked()
        {
            currentTarget = tracked = null;
            timeUntilUpdate = 0f;
            usedTracker = false;
            if (arrow?.arrow != null) UnityEngine.Object.Destroy(arrow.arrow);
            arrow = new Arrow(Color.blue);
            if (arrow.arrow != null) arrow.arrow.SetActive(false);
            if (DangerMeterParent)
            {
                Meter.gameObject.Destroy();
                DangerMeterParent.Destroy();
            }
        }

        public override void clearAndReload()
        {
            tracker = null;
            resetTracked();
            timeUntilUpdate = 0f;
            updateIntervall = CustomOptionHolder.trackerUpdateIntervall.getFloat();
            resetTargetAfterMeeting = CustomOptionHolder.trackerResetTargetAfterMeeting.getBool();
            if (localArrows != null)
                foreach (Arrow arrow in localArrows)
                    if (arrow?.arrow != null)
                        UnityEngine.Object.Destroy(arrow.arrow);
            deadBodyPositions = new List<Vector3>();
            corpsesTrackingTimer = 0f;
            corpsesTrackingCooldown = CustomOptionHolder.trackerCorpsesTrackingCooldown.getFloat();
            corpsesTrackingDuration = CustomOptionHolder.trackerCorpsesTrackingDuration.getFloat();
            canTrackCorpses = CustomOptionHolder.trackerCanTrackCorpses.getBool();
            trackingMode = CustomOptionHolder.trackerTrackingMethod.getSelection();
        }
    }

    public sealed class Vampire : RoleBase
    {
        public PlayerControl vampire;
        public Color color = Palette.ImpostorRed;

        public float delay = 10f;
        public float cooldown = 30f;
        public bool canKillNearGarlics = true;
        public bool localPlacedGarlic = false;
        public bool garlicsActive = true;

        public PlayerControl currentTarget;
        public PlayerControl bitten;
        public bool targetNearGarlic = false;

        private Sprite buttonSprite;
        public Sprite getButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.VampireButton.png", 115f);
            return buttonSprite;
        }

        private Sprite garlicButtonSprite;
        public Sprite getGarlicButtonSprite()
        {
            if (garlicButtonSprite) return garlicButtonSprite;
            garlicButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.GarlicButton.png", 115f);
            return garlicButtonSprite;
        }

        public override void clearAndReload()
        {
            vampire = null;
            bitten = null;
            targetNearGarlic = false;
            localPlacedGarlic = false;
            currentTarget = null;
            garlicsActive = CustomOptionHolder.vampireSpawnRate.getSelection() > 0;
            delay = CustomOptionHolder.vampireKillDelay.getFloat();
            cooldown = CustomOptionHolder.vampireCooldown.getFloat();
            canKillNearGarlics = CustomOptionHolder.vampireCanKillNearGarlics.getBool();
        }
    }

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

    public sealed class Jackal : RoleBase
    {
        public PlayerControl jackal;
        public Color color = new Color32(0, 180, 235, byte.MaxValue);
        public PlayerControl fakeSidekick;
        public PlayerControl currentTarget;
        public List<PlayerControl> formerJackals = new List<PlayerControl>();

        public float cooldown = 30f;
        public float createSidekickCooldown = 30f;
        public bool canUseVents = true;
        public bool canCreateSidekick = true;
        public Sprite buttonSprite;
        public bool jackalPromotedFromSidekickCanCreateSidekick = true;
        public bool canCreateSidekickFromImpostor = true;
        public bool hasImpostorVision = false;
        public bool wasTeamRed;
        public bool wasImpostor;
        public bool wasSpy;
        public bool canSabotageLights;

        public Sprite getSidekickButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.SidekickButton.png", 115f);
            return buttonSprite;
        }

        public void removeCurrentJackal()
        {
            if (!formerJackals.Any(x => x.PlayerId == jackal.PlayerId)) formerJackals.Add(jackal);
            jackal = null;
            currentTarget = null;
            fakeSidekick = null;
            cooldown = CustomOptionHolder.jackalKillCooldown.getFloat();
            createSidekickCooldown = CustomOptionHolder.jackalCreateSidekickCooldown.getFloat();
        }

        public override void clearAndReload()
        {
            jackal = null;
            currentTarget = null;
            fakeSidekick = null;
            cooldown = CustomOptionHolder.jackalKillCooldown.getFloat();
            createSidekickCooldown = CustomOptionHolder.jackalCreateSidekickCooldown.getFloat();
            canUseVents = CustomOptionHolder.jackalCanUseVents.getBool();
            canCreateSidekick = CustomOptionHolder.jackalCanCreateSidekick.getBool();
            jackalPromotedFromSidekickCanCreateSidekick = CustomOptionHolder.jackalPromotedFromSidekickCanCreateSidekick.getBool();
            canCreateSidekickFromImpostor = CustomOptionHolder.jackalCanCreateSidekickFromImpostor.getBool();
            formerJackals.Clear();
            hasImpostorVision = CustomOptionHolder.jackalAndSidekickHaveImpostorVision.getBool();
            wasTeamRed = wasImpostor = wasSpy = false;
            canSabotageLights = CustomOptionHolder.jackalCanSabotageLights.getBool();
        }

    }

    public sealed class Sidekick : RoleBase
    {
        public PlayerControl sidekick;
        public Color color = new Color32(0, 180, 235, byte.MaxValue);

        public PlayerControl currentTarget;

        public bool wasTeamRed;
        public bool wasImpostor;
        public bool wasSpy;

        public float cooldown = 30f;
        public bool canUseVents = true;
        public bool canKill = true;
        public bool promotesToJackal = true;
        public bool hasImpostorVision = false;
        public bool canSabotageLights;

        public override void clearAndReload()
        {
            sidekick = null;
            currentTarget = null;
            cooldown = CustomOptionHolder.jackalKillCooldown.getFloat();
            canUseVents = CustomOptionHolder.sidekickCanUseVents.getBool();
            canKill = CustomOptionHolder.sidekickCanKill.getBool();
            promotesToJackal = CustomOptionHolder.sidekickPromotesToJackal.getBool();
            hasImpostorVision = CustomOptionHolder.jackalAndSidekickHaveImpostorVision.getBool();
            wasTeamRed = wasImpostor = wasSpy = false;
            canSabotageLights = CustomOptionHolder.sidekickCanSabotageLights.getBool();
        }
    }

    public sealed class Eraser : RoleBase
    {
        public PlayerControl eraser;
        public Color color = Palette.ImpostorRed;

        public List<byte> alreadyErased = new List<byte>();

        public List<PlayerControl> futureErased = new List<PlayerControl>();
        public PlayerControl currentTarget;
        public float cooldown = 30f;
        public bool canEraseAnyone = false;

        private Sprite buttonSprite;
        public Sprite getButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.EraserButton.png", 115f);
            return buttonSprite;
        }

        public override void clearAndReload()
        {
            eraser = null;
            futureErased = new List<PlayerControl>();
            currentTarget = null;
            cooldown = CustomOptionHolder.eraserCooldown.getFloat();
            canEraseAnyone = CustomOptionHolder.eraserCanEraseAnyone.getBool();
            alreadyErased = new List<byte>();
        }
    }

    public sealed class Spy : RoleBase
    {
        public PlayerControl spy;
        public Color color = Palette.ImpostorRed;

        public bool impostorsCanKillAnyone = true;
        public bool canEnterVents = false;
        public bool hasImpostorVision = false;

        public override void clearAndReload()
        {
            spy = null;
            impostorsCanKillAnyone = CustomOptionHolder.spyImpostorsCanKillAnyone.getBool();
            canEnterVents = CustomOptionHolder.spyCanEnterVents.getBool();
            hasImpostorVision = CustomOptionHolder.spyHasImpostorVision.getBool();
        }
    }

    public sealed class Trickster : RoleBase
    {
        public PlayerControl trickster;
        public Color color = Palette.ImpostorRed;
        public float placeBoxCooldown = 30f;
        public float lightsOutCooldown = 30f;
        public float lightsOutDuration = 10f;
        public float lightsOutTimer = 0f;

        private Sprite placeBoxButtonSprite;
        private Sprite lightOutButtonSprite;
        private Sprite tricksterVentButtonSprite;

        public Sprite getPlaceBoxButtonSprite()
        {
            if (placeBoxButtonSprite) return placeBoxButtonSprite;
            placeBoxButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.PlaceJackInTheBoxButton.png", 115f);
            return placeBoxButtonSprite;
        }

        public Sprite getLightsOutButtonSprite()
        {
            if (lightOutButtonSprite) return lightOutButtonSprite;
            lightOutButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.LightsOutButton.png", 115f);
            return lightOutButtonSprite;
        }

        public Sprite getTricksterVentButtonSprite()
        {
            if (tricksterVentButtonSprite) return tricksterVentButtonSprite;
            tricksterVentButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.TricksterVentButton.png", 115f);
            return tricksterVentButtonSprite;
        }

        public override void clearAndReload()
        {
            trickster = null;
            lightsOutTimer = 0f;
            placeBoxCooldown = CustomOptionHolder.tricksterPlaceBoxCooldown.getFloat();
            lightsOutCooldown = CustomOptionHolder.tricksterLightsOutCooldown.getFloat();
            lightsOutDuration = CustomOptionHolder.tricksterLightsOutDuration.getFloat();
            JackInTheBox.UpdateStates(); // if the role is erased, we might have to update the state of the created objects
        }

    }

    public sealed class Cleaner : RoleBase
    {
        public PlayerControl cleaner;
        public Color color = Palette.ImpostorRed;

        public float cooldown = 30f;

        private Sprite buttonSprite;
        public Sprite getButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.CleanButton.png", 115f);
            return buttonSprite;
        }

        public override void clearAndReload()
        {
            cleaner = null;
            cooldown = CustomOptionHolder.cleanerCooldown.getFloat();
        }
    }

    public sealed class Warlock : RoleBase
    {

        public PlayerControl warlock;
        public Color color = Palette.ImpostorRed;

        public PlayerControl currentTarget;
        public PlayerControl curseVictim;
        public PlayerControl curseVictimTarget;

        public float cooldown = 30f;
        public float rootTime = 5f;

        private Sprite curseButtonSprite;
        private Sprite curseKillButtonSprite;

        public Sprite getCurseButtonSprite()
        {
            if (curseButtonSprite) return curseButtonSprite;
            curseButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.CurseButton.png", 115f);
            return curseButtonSprite;
        }

        public Sprite getCurseKillButtonSprite()
        {
            if (curseKillButtonSprite) return curseKillButtonSprite;
            curseKillButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.CurseKillButton.png", 115f);
            return curseKillButtonSprite;
        }

        public override void clearAndReload()
        {
            warlock = null;
            currentTarget = null;
            curseVictim = null;
            curseVictimTarget = null;
            cooldown = CustomOptionHolder.warlockCooldown.getFloat();
            rootTime = CustomOptionHolder.warlockRootTime.getFloat();
        }

        public void resetCurse()
        {
            HudManagerStartPatch.warlockCurseButton.Timer = HudManagerStartPatch.warlockCurseButton.MaxTimer;
            HudManagerStartPatch.warlockCurseButton.Sprite = Warlock.getCurseButtonSprite();
            HudManagerStartPatch.warlockCurseButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            currentTarget = null;
            curseVictim = null;
            curseVictimTarget = null;
        }
    }

    public sealed class SecurityGuard : RoleBase
    {
        public PlayerControl securityGuard;
        public Color color = new Color32(195, 178, 95, byte.MaxValue);

        public float cooldown = 30f;
        public int remainingScrews = 7;
        public int totalScrews = 7;
        public int ventPrice = 1;
        public int camPrice = 2;
        public int placedCameras = 0;
        public float duration = 10f;
        public int maxCharges = 5;
        public int rechargeTasksNumber = 3;
        public int rechargedTasks = 3;
        public int charges = 1;
        public bool cantMove = true;
        public Vent ventTarget = null;
        public Minigame minigame = null;

        private Sprite closeVentButtonSprite;
        public Sprite getCloseVentButtonSprite()
        {
            if (closeVentButtonSprite) return closeVentButtonSprite;
            closeVentButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.CloseVentButton.png", 115f);
            return closeVentButtonSprite;
        }

        private Sprite placeCameraButtonSprite;
        public Sprite getPlaceCameraButtonSprite()
        {
            if (placeCameraButtonSprite) return placeCameraButtonSprite;
            placeCameraButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.PlaceCameraButton.png", 115f);
            return placeCameraButtonSprite;
        }

        private Sprite animatedVentSealedSprite;
        private float lastPPU;
        public Sprite getAnimatedVentSealedSprite()
        {
            float ppu = 185f;
            if (SubmergedCompatibility.IsSubmerged) ppu = 120f;
            if (lastPPU != ppu)
            {
                animatedVentSealedSprite = null;
                lastPPU = ppu;
            }
            if (animatedVentSealedSprite) return animatedVentSealedSprite;
            animatedVentSealedSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.AnimatedVentSealed.png", ppu);
            return animatedVentSealedSprite;
        }

        private Sprite VentSealedSprite;
        public Sprite getVentSealedSprite()
        {
            if (VentSealedSprite) return VentSealedSprite;
            VentSealedSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.VentSealed.png", 160f);
            return VentSealedSprite;
        }

        private Sprite fungleVentSealedSprite;
        public Sprite getFungleVentSealedSprite()
        {
            if (fungleVentSealedSprite) return fungleVentSealedSprite;
            fungleVentSealedSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.FungleVentSealed.png", 160f);
            return fungleVentSealedSprite;
        }

        private Sprite submergedCentralUpperVentSealedSprite;
        public Sprite getSubmergedCentralUpperSealedSprite()
        {
            if (submergedCentralUpperVentSealedSprite) return submergedCentralUpperVentSealedSprite;
            submergedCentralUpperVentSealedSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.CentralUpperBlocked.png", 145f);
            return submergedCentralUpperVentSealedSprite;
        }

        private Sprite submergedCentralLowerVentSealedSprite;
        public Sprite getSubmergedCentralLowerSealedSprite()
        {
            if (submergedCentralLowerVentSealedSprite) return submergedCentralLowerVentSealedSprite;
            submergedCentralLowerVentSealedSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.CentralLowerBlocked.png", 145f);
            return submergedCentralLowerVentSealedSprite;
        }

        private Sprite camSprite;
        public Sprite getCamSprite()
        {
            if (camSprite) return camSprite;
            camSprite = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.CamsButton].Image;
            return camSprite;
        }

        private Sprite logSprite;
        public Sprite getLogSprite()
        {
            if (logSprite) return logSprite;
            logSprite = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.DoorLogsButton].Image;
            return logSprite;
        }

        public override void clearAndReload()
        {
            securityGuard = null;
            ventTarget = null;
            minigame = null;
            duration = CustomOptionHolder.securityGuardCamDuration.getFloat();
            maxCharges = Mathf.RoundToInt(CustomOptionHolder.securityGuardCamMaxCharges.getFloat());
            rechargeTasksNumber = Mathf.RoundToInt(CustomOptionHolder.securityGuardCamRechargeTasksNumber.getFloat());
            rechargedTasks = Mathf.RoundToInt(CustomOptionHolder.securityGuardCamRechargeTasksNumber.getFloat());
            charges = Mathf.RoundToInt(CustomOptionHolder.securityGuardCamMaxCharges.getFloat()) / 2;
            placedCameras = 0;
            cooldown = CustomOptionHolder.securityGuardCooldown.getFloat();
            totalScrews = remainingScrews = Mathf.RoundToInt(CustomOptionHolder.securityGuardTotalScrews.getFloat());
            camPrice = Mathf.RoundToInt(CustomOptionHolder.securityGuardCamPrice.getFloat());
            ventPrice = Mathf.RoundToInt(CustomOptionHolder.securityGuardVentPrice.getFloat());
            cantMove = CustomOptionHolder.securityGuardNoMove.getBool();
        }
    }

    public sealed class Arsonist : RoleBase
    {
        public PlayerControl arsonist;
        public Color color = new Color32(238, 112, 46, byte.MaxValue);

        public float cooldown = 30f;
        public float duration = 3f;
        public bool triggerArsonistWin = false;

        public PlayerControl currentTarget;
        public PlayerControl douseTarget;
        public List<PlayerControl> dousedPlayers = new List<PlayerControl>();

        private Sprite douseSprite;
        public Sprite getDouseSprite()
        {
            if (douseSprite) return douseSprite;
            douseSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.DouseButton.png", 115f);
            return douseSprite;
        }

        private Sprite igniteSprite;
        public Sprite getIgniteSprite()
        {
            if (igniteSprite) return igniteSprite;
            igniteSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.IgniteButton.png", 115f);
            return igniteSprite;
        }

        public bool dousedEveryoneAlive()
        {
            return CachedPlayer.AllPlayers.All(x => { return x.PlayerControl == Arsonist.arsonist || x.Data.IsDead || x.Data.Disconnected || Arsonist.dousedPlayers.Any(y => y.PlayerId == x.PlayerId); });
        }

        public override void clearAndReload()
        {
            arsonist = null;
            currentTarget = null;
            douseTarget = null;
            triggerArsonistWin = false;
            dousedPlayers = new List<PlayerControl>();
            foreach (PoolablePlayer p in TORMapOptions.playerIcons.Values)
                if (p != null && p.gameObject != null) p.gameObject.SetActive(false);
            cooldown = CustomOptionHolder.arsonistCooldown.getFloat();
            duration = CustomOptionHolder.arsonistDuration.getFloat();
        }
    }

    public sealed class Guesser : RoleBase
    {
        public PlayerControl niceGuesser;
        public PlayerControl evilGuesser;
        public Color color = new Color32(255, 255, 0, byte.MaxValue);

        public int remainingShotsEvilGuesser = 2;
        public int remainingShotsNiceGuesser = 2;

        public bool isGuesser(byte playerId)
        {
            if (niceGuesser != null && niceGuesser.PlayerId == playerId || evilGuesser != null && evilGuesser.PlayerId == playerId) return true;
            return false;
        }

        public void clear(byte playerId)
        {
            if (niceGuesser != null && niceGuesser.PlayerId == playerId) niceGuesser = null;
            else if (evilGuesser != null && evilGuesser.PlayerId == playerId) evilGuesser = null;
        }

        public int remainingShots(byte playerId, bool shoot = false)
        {
            int remainingShots = remainingShotsEvilGuesser;
            if (niceGuesser != null && niceGuesser.PlayerId == playerId)
            {
                remainingShots = remainingShotsNiceGuesser;
                if (shoot) remainingShotsNiceGuesser = Mathf.Max(0, remainingShotsNiceGuesser - 1);
            }
            else if (shoot)
                remainingShotsEvilGuesser = Mathf.Max(0, remainingShotsEvilGuesser - 1);
            return remainingShots;
        }

        public override void clearAndReload()
        {
            niceGuesser = null;
            evilGuesser = null;
            remainingShotsEvilGuesser = Mathf.RoundToInt(CustomOptionHolder.guesserNumberOfShots.getFloat());
            remainingShotsNiceGuesser = Mathf.RoundToInt(CustomOptionHolder.guesserNumberOfShots.getFloat());
        }
    }

    public sealed class BountyHunter : RoleBase
    {
        public PlayerControl bountyHunter;
        public Color color = Palette.ImpostorRed;

        public Arrow arrow;
        public float bountyDuration = 30f;
        public bool showArrow = true;
        public float bountyKillCooldown = 0f;
        public float punishmentTime = 15f;
        public float arrowUpdateIntervall = 10f;

        public float arrowUpdateTimer = 0f;
        public float bountyUpdateTimer = 0f;
        public PlayerControl bounty;
        public TMPro.TextMeshPro cooldownText;

        public override void clearAndReload()
        {
            arrow = new Arrow(color);
            bountyHunter = null;
            bounty = null;
            arrowUpdateTimer = 0f;
            bountyUpdateTimer = 0f;
            if (arrow != null && arrow.arrow != null) UnityEngine.Object.Destroy(arrow.arrow);
            arrow = null;
            if (cooldownText != null && cooldownText.gameObject != null) UnityEngine.Object.Destroy(cooldownText.gameObject);
            cooldownText = null;
            foreach (PoolablePlayer p in TORMapOptions.playerIcons.Values)
                if (p != null && p.gameObject != null) p.gameObject.SetActive(false);


            bountyDuration = CustomOptionHolder.bountyHunterBountyDuration.getFloat();
            bountyKillCooldown = CustomOptionHolder.bountyHunterReducedCooldown.getFloat();
            punishmentTime = CustomOptionHolder.bountyHunterPunishmentTime.getFloat();
            showArrow = CustomOptionHolder.bountyHunterShowArrow.getBool();
            arrowUpdateIntervall = CustomOptionHolder.bountyHunterArrowUpdateIntervall.getFloat();
        }
    }

    public sealed class Vulture : RoleBase
    {
        public PlayerControl vulture;
        public Color color = new Color32(139, 69, 19, byte.MaxValue);
        public List<Arrow> localArrows = new List<Arrow>();
        public float cooldown = 30f;
        public int vultureNumberToWin = 4;
        public int eatenBodies = 0;
        public bool triggerVultureWin = false;
        public bool canUseVents = true;
        public bool showArrows = true;
        private Sprite buttonSprite;
        public Sprite getButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.VultureButton.png", 115f);
            return buttonSprite;
        }

        public override void clearAndReload()
        {
            vulture = null;
            vultureNumberToWin = Mathf.RoundToInt(CustomOptionHolder.vultureNumberToWin.getFloat());
            eatenBodies = 0;
            cooldown = CustomOptionHolder.vultureCooldown.getFloat();
            triggerVultureWin = false;
            canUseVents = CustomOptionHolder.vultureCanUseVents.getBool();
            showArrows = CustomOptionHolder.vultureShowArrows.getBool();
            if (localArrows != null)
                foreach (Arrow arrow in localArrows)
                    if (arrow?.arrow != null)
                        UnityEngine.Object.Destroy(arrow.arrow);
            localArrows = new List<Arrow>();
        }
    }


    public sealed class Medium : RoleBase
    {
        public PlayerControl medium;
        public DeadPlayer target;
        public DeadPlayer soulTarget;
        public Color color = new Color32(98, 120, 115, byte.MaxValue);
        public List<Tuple<DeadPlayer, Vector3>> deadBodies = new List<Tuple<DeadPlayer, Vector3>>();
        public List<Tuple<DeadPlayer, Vector3>> futureDeadBodies = new List<Tuple<DeadPlayer, Vector3>>();
        public List<SpriteRenderer> souls = new List<SpriteRenderer>();
        public DateTime meetingStartTime = DateTime.UtcNow;

        public float cooldown = 30f;
        public float duration = 3f;
        public bool oneTimeUse = false;
        public float chanceAdditionalInfo = 0f;

        private Sprite soulSprite;

        enum SpecialMediumInfo
        {
            SheriffSuicide,
            ThiefSuicide,
            ActiveLoverDies,
            PassiveLoverSuicide,
            LawyerKilledByClient,
            JackalKillsSidekick,
            ImpostorTeamkill,
            SubmergedO2,
            WarlockSuicide,
            BodyCleaned,
        }

        public Sprite getSoulSprite()
        {
            if (soulSprite) return soulSprite;
            soulSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.Soul.png", 500f);
            return soulSprite;
        }

        private Sprite question;
        public Sprite getQuestionSprite()
        {
            if (question) return question;
            question = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.MediumButton.png", 115f);
            return question;
        }

        public override void clearAndReload()
        {
            medium = null;
            target = null;
            soulTarget = null;
            deadBodies = new List<Tuple<DeadPlayer, Vector3>>();
            futureDeadBodies = new List<Tuple<DeadPlayer, Vector3>>();
            souls = new List<SpriteRenderer>();
            meetingStartTime = DateTime.UtcNow;
            cooldown = CustomOptionHolder.mediumCooldown.getFloat();
            duration = CustomOptionHolder.mediumDuration.getFloat();
            oneTimeUse = CustomOptionHolder.mediumOneTimeUse.getBool();
            chanceAdditionalInfo = CustomOptionHolder.mediumChanceAdditionalInfo.getSelection() / 10f;
        }

        public string getInfo(PlayerControl target, PlayerControl killer)
        {
            string msg = "";

            List<SpecialMediumInfo> infos = new List<SpecialMediumInfo>();
            // collect fitting death info types.
            // suicides:
            if (killer == target)
            {
                if (target == Sheriff.sheriff || target == Sheriff.formerSheriff) infos.Add(SpecialMediumInfo.SheriffSuicide);
                if (target == Lovers.lover1 || target == Lovers.lover2) infos.Add(SpecialMediumInfo.PassiveLoverSuicide);
                if (target == Thief.thief) infos.Add(SpecialMediumInfo.ThiefSuicide);
                if (target == Warlock.warlock) infos.Add(SpecialMediumInfo.WarlockSuicide);
            }
            else
            {
                if (target == Lovers.lover1 || target == Lovers.lover2) infos.Add(SpecialMediumInfo.ActiveLoverDies);
                if (target.Data.Role.IsImpostor && killer.Data.Role.IsImpostor && Thief.formerThief != killer) infos.Add(SpecialMediumInfo.ImpostorTeamkill);
            }
            if (target == Sidekick.sidekick && (killer == Jackal.jackal || Jackal.formerJackals.Any(x => x.PlayerId == killer.PlayerId))) infos.Add(SpecialMediumInfo.JackalKillsSidekick);
            if (target == Lawyer.lawyer && killer == Lawyer.target) infos.Add(SpecialMediumInfo.LawyerKilledByClient);
            if (Medium.target.wasCleaned) infos.Add(SpecialMediumInfo.BodyCleaned);

            if (infos.Count > 0)
            {
                var selectedInfo = infos[rnd.Next(infos.Count)];
                switch (selectedInfo)
                {
                    case SpecialMediumInfo.SheriffSuicide:
                        msg = ModTranslation.getString("mediumSheriffSuicide");
                        break;
                    case SpecialMediumInfo.WarlockSuicide:
                        msg = ModTranslation.getString("mediumWarlockSuicide");
                        break;
                    case SpecialMediumInfo.ThiefSuicide:
                        msg = ModTranslation.getString("mediumThiefSuicide");
                        break;
                    case SpecialMediumInfo.ActiveLoverDies:
                        msg = ModTranslation.getString("mediumActiveLoverDies");
                        break;
                    case SpecialMediumInfo.PassiveLoverSuicide:
                        msg = ModTranslation.getString("mediumPassiveLoverSuicide");
                        break;
                    case SpecialMediumInfo.LawyerKilledByClient:
                        msg = ModTranslation.getString("mediumLawyerKilledByClient");
                        break;
                    case SpecialMediumInfo.JackalKillsSidekick:
                        msg = ModTranslation.getString("mediumJackalKillsSidekick");
                        break;
                    case SpecialMediumInfo.ImpostorTeamkill:
                        msg = ModTranslation.getString("mediumImpostorTeamKill");
                        break;
                    case SpecialMediumInfo.BodyCleaned:
                        msg = ModTranslation.getString("mediumBodyCleaned");
                        break;
                }
            }
            else
            {
                int randomNumber = rnd.Next(4);
                string typeOfColor = Helpers.isLighterColor(Medium.target.killerIfExisting.Data.DefaultOutfit.ColorId) ? ModTranslation.getString("mediumSoulPlayerLighter") : ModTranslation.getString("mediumSoulPlayerDarker");
                float timeSinceDeath = (float)(Medium.meetingStartTime - Medium.target.timeOfDeath).TotalMilliseconds;
                var roleString = RoleInfo.GetRolesString(Medium.target.player, false, includeHidden: true);

                if (randomNumber == 0)
                    if (!roleString.Contains(RoleInfo.impostor.name) && !roleString.Contains(RoleInfo.crewmate.name)) msg = string.Format(ModTranslation.getString("mediumQuestion1"), RoleInfo.GetRolesString(Medium.target.player, false, includeHidden: true));
                    else msg = string.Format(ModTranslation.getString("mediumQuestion5"), roleString);
                else if (randomNumber == 1) msg = string.Format(ModTranslation.getString("mediumQuestion2"), typeOfColor);
                else if (randomNumber == 2) msg = string.Format(ModTranslation.getString("mediumQuestion3"), Math.Round(timeSinceDeath / 1000));
                else msg = string.Format(ModTranslation.getString("mediumQuestion4"), RoleInfo.GetRolesString(Medium.target.killerIfExisting, false, false, true, includeHidden: true));
            }

            if (rnd.NextDouble() < chanceAdditionalInfo)
            {
                int count = 0;
                string condition = "";
                var alivePlayersList = PlayerControl.AllPlayerControls.ToArray().Where(pc => !pc.Data.IsDead);
                switch (rnd.Next(3))
                {
                    case 0:
                        count = alivePlayersList.Where(pc => pc.Data.Role.IsImpostor || new List<RoleInfo>() { RoleInfo.jackal, RoleInfo.sidekick, RoleInfo.sheriff, RoleInfo.thief }.Contains(RoleInfo.getRoleInfoForPlayer(pc, false).FirstOrDefault())).Count();
                        condition = ModTranslation.getString("mediumKiller") + (count == 1 ? "" : ModTranslation.getString("mediumPlural"));
                        break;
                    case 1:
                        count = alivePlayersList.Where(Helpers.roleCanUseVents).Count();
                        condition = string.Format(ModTranslation.getString("mediumPlayerUseVents"), count == 1 ? "" : ModTranslation.getString("mediumPlural"));
                        break;
                    case 2:
                        count = alivePlayersList.Where(pc => Helpers.isNeutral(pc) && pc != Jackal.jackal && pc != Sidekick.sidekick && pc != Thief.thief).Count();
                        condition = string.Format(ModTranslation.getString("mediumPlayerNeutral"), count == 1 ? "" : ModTranslation.getString("mediumPlural"), count == 1 ? ModTranslation.getString("mediumIs") : ModTranslation.getString("mediumAre"));
                        break;
                    case 3:
                        //count = alivePlayersList.Where(pc =>
                        break;
                }
                msg += $"\n" + ModTranslation.getString("mediumAskPrefix") + $"{count} " + $"{condition} " + string.Format(ModTranslation.getString("mediumStillAlive"), count == 1 ? ModTranslation.getString("mediumWas") : ModTranslation.getString("mediumWere"));
            }

            return string.Format(ModTranslation.getString("mediumSoulPlayerPrefix"), Medium.target.player.Data.PlayerName) + msg;
        }
    }

    public sealed class NekoKabocha : RoleBase
    {
        public PlayerControl nekoKabocha;
        public Color color = Palette.ImpostorRed;

        public bool revengeCrew = true;
        public bool revengeImpostor = true;
        public bool revengeNeutral = true;
        public bool revengeExile = false;

        public PlayerControl meetingKiller = null;
        public PlayerControl otherKiller;

        public void clearAndReload()
        {
            nekoKabocha = null;
            meetingKiller = null;
            otherKiller = null;
            revengeCrew = CustomOptionHolder.nekoKabochaRevengeCrew.getBool();
            revengeImpostor = CustomOptionHolder.nekoKabochaRevengeImpostor.getBool();
            revengeNeutral = CustomOptionHolder.nekoKabochaRevengeNeutral.getBool();
            revengeExile = CustomOptionHolder.nekoKabochaRevengeExile.getBool();
        }
    }

    public sealed class Lawyer : RoleBase
    {
        public PlayerControl lawyer;
        public PlayerControl target;
        public Color color = new Color32(134, 153, 25, byte.MaxValue);
        public Sprite targetSprite;
        //public  bool triggerProsecutorWin = false;
        //public  bool isProsecutor = false;
        public bool targetKnows = true;
        public bool triggerLawyerWin = false;
        public int meetings = 0;

        public bool winsAfterMeetings = false;
        public int neededMeetings = 4;
        public float vision = 1f;
        public bool lawyerTargetKnows = true;
        public bool lawyerKnowsRole = false;
        public bool targetCanBeJester = false;
        public bool targetWasGuessed = false;

        public Sprite getTargetSprite()
        {
            if (targetSprite) return targetSprite;
            targetSprite = Helpers.loadSpriteFromResources("", 150f);
            return targetSprite;
        }

        public void clearAndReload(bool clearTarget = true)
        {
            lawyer = null;
            if (clearTarget)
            {
                target = null;
                targetWasGuessed = false;
            }
            triggerLawyerWin = false;
            meetings = 0;
            //isProsecutor = false;
            //triggerProsecutorWin = false;
            vision = CustomOptionHolder.lawyerVision.getFloat();
            lawyerKnowsRole = CustomOptionHolder.lawyerKnowsRole.getBool();
            lawyerTargetKnows = CustomOptionHolder.lawyerTargetKnows.getBool();
            targetCanBeJester = CustomOptionHolder.lawyerTargetCanBeJester.getBool();
            winsAfterMeetings = CustomOptionHolder.lawyerWinsAfterMeetings.getBool();
            neededMeetings = Mathf.RoundToInt(CustomOptionHolder.lawyerNeededMeetings.getFloat());
            targetKnows = CustomOptionHolder.lawyerTargetKnows.getBool();
        }
    }

    public sealed class Pursuer : RoleBase
    {
        public PlayerControl pursuer;
        public PlayerControl target;
        public Color color = Lawyer.color;
        public List<PlayerControl> blankedList = new List<PlayerControl>();
        public int blanks = 0;
        public Sprite blank;
        public bool notAckedExiled = false;

        public float cooldown = 30f;
        public int blanksNumber = 5;

        public Sprite getTargetSprite()
        {
            if (blank) return blank;
            blank = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.PursuerButton.png", 115f);
            return blank;
        }

        public override void clearAndReload()
        {
            pursuer = null;
            target = null;
            blankedList = new List<PlayerControl>();
            blanks = 0;
            notAckedExiled = false;

            cooldown = CustomOptionHolder.pursuerCooldown.getFloat();
            blanksNumber = Mathf.RoundToInt(CustomOptionHolder.pursuerBlanksNumber.getFloat());
        }
    }

    public sealed class MimicK : RoleBase
    {
        public PlayerControl mimicK;
        public Color color = Palette.ImpostorRed;

        public bool ifOneDiesBothDie = true;
        public bool hasOneVote = true;
        public bool countAsOne = true;

        public string name = "";

        public List<Arrow> arrows = new();
        public float updateTimer = 0f;
        public float arrowUpdateInterval = 0.5f;

        public PlayerControl victim;

        public void arrowUpdate()
        {
            //if (MimicK.mimicK == null || MimicA.mimicA == null) return;
            if (arrows.FirstOrDefault()?.arrow != null)
                if (mimicK == null || MimicA.mimicA == null)
                {
                    foreach (Arrow arrows in arrows) arrows.arrow.SetActive(false);
                    return;
                }
            if (CachedPlayer.LocalPlayer.PlayerControl != mimicK || mimicK == null) return;
            if (mimicK.Data.IsDead)
            {
                if (arrows.FirstOrDefault().arrow != null) UnityEngine.Object.Destroy(arrows.FirstOrDefault().arrow);
                return;
            }
            // ǰ�ե�`�फ��νU�^�r�g��ޥ��ʥ�����
            updateTimer -= Time.fixedDeltaTime;

            // 1��U�^������Arrow�����
            if (updateTimer <= 0.0f)
            {

                // ǰ�ؤ�Arrow�򤹤٤��Ɨ�����
                foreach (Arrow arrow1 in arrows)
                    if (arrow1 != null && arrow1.arrow != null)
                    {
                        arrow1.arrow.SetActive(false);
                        UnityEngine.Object.Destroy(arrow1.arrow);
                    }

                //if (MimicK.mimicK == null) return;

                // Arrowsһ�E
                arrows = new List<Arrow>();

                // ����ݥ����`��λ�ä�ʾ��Arrows���軭
                /*foreach (PlayerControl p in CachedPlayer.AllPlayers)
                {
                    if (p.Data.IsDead) continue;
                    Arrow arrow;
                    if (p == MimicA.mimicA)
                    {
                        arrow = MimicA.isMorph ? new Arrow(Palette.White) : new Arrow(Palette.ImpostorRed);
                        arrow.arrow.SetActive(true);
                        arrow.Update(p.transform.position);
                        arrows.Add(arrow);
                    }
                }*/

                if (MimicA.mimicA.Data.IsDead || MimicA.mimicA == null) return;
                Arrow arrow;
                arrow = MimicA.isMorph ? new Arrow(Palette.White) : new Arrow(Palette.ImpostorRed);
                arrow.arrow.SetActive(true);
                arrow.Update(MimicA.mimicA.transform.position);
                arrows.Add(arrow);

                // �����ީ`�˕r�g�򥻥å�
                updateTimer = arrowUpdateInterval;
            }
        }

        public void clearAndReload()
        {
            mimicK?.setDefaultLook();
            if (MimicA.mimicA != null)
            {
                MimicA.isMorph = false;
                MimicA.mimicA.setDefaultLook();
            }

            mimicK = null;
            victim = null;
            ifOneDiesBothDie = CustomOptionHolder.mimicIfOneDiesBothDie.getBool();
            hasOneVote = CustomOptionHolder.mimicHasOneVote.getBool();
            countAsOne = CustomOptionHolder.mimicCountAsOne.getBool();

            if (arrows != null)
                foreach (Arrow arrow in arrows)
                    if (arrow?.arrow != null)
                        UnityEngine.Object.Destroy(arrow.arrow);
            arrows = new List<Arrow>();
        }
    }

    public sealed class MimicA : RoleBase
    {
        public PlayerControl mimicA;
        public Color color = Palette.ImpostorRed;

        public bool isMorph = false;

        //public  string MimicKName = MimicK.mimicK.Data.PlayerName;

        public Sprite adminButtonSprite;
        public Sprite morphButtonSprite;

        public Sprite getMorphSprite()
        {
            if (morphButtonSprite) return morphButtonSprite;
            morphButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.MorphButton.png", 115f);
            return morphButtonSprite;
        }

        public Sprite getAdminSprite()
        {
            byte mapId = GameOptionsManager.Instance.currentNormalGameOptions.MapId;
            UseButtonSettings button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.PolusAdminButton]; // Polus
            if (Helpers.isSkeld() || mapId == 3) button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.AdminMapButton]; // Skeld || Dleks
            else if (Helpers.isMira()) button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.MIRAAdminButton]; // Mira HQ
            else if (Helpers.isAirship()) button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.AirshipAdminButton]; // Airship
            else if (Helpers.isFungle()) button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.AdminMapButton];
            adminButtonSprite = button.Image;
            return adminButtonSprite;
        }

        public List<Arrow> arrows = new();
        public float updateTimer = 0f;
        public float arrowUpdateInterval = 0.5f;
        public void arrowUpdate()
        {
            //if (MimicA.mimicA == null || MimicK.mimicK == null) return;
            if (arrows.FirstOrDefault()?.arrow != null)
                if (MimicK.mimicK == null || mimicA == null)
                {
                    foreach (Arrow arrows in arrows) arrows.arrow.SetActive(false);
                    return;
                }
            if (CachedPlayer.LocalPlayer.PlayerControl != mimicA) return;

            if (mimicA.Data.IsDead)
            {
                if (arrows.FirstOrDefault().arrow != null) UnityEngine.Object.Destroy(arrows.FirstOrDefault().arrow);
                return;
            }

            // ǰ�ե�`�फ��νU�^�r�g��ޥ��ʥ�����
            updateTimer -= Time.fixedDeltaTime;

            // 1��U�^������Arrow�����
            if (updateTimer <= 0.0f)
            {

                // ǰ�ؤ�Arrow�򤹤٤��Ɨ�����
                foreach (Arrow arrow1 in arrows)
                    if (arrow1 != null && arrow1.arrow != null)
                    {
                        arrow1.arrow.SetActive(false);
                        UnityEngine.Object.Destroy(arrow1.arrow);
                    }

                //if (MimicA.mimicA == null) return;

                // Arrowsһ�E
                arrows = new List<Arrow>();
                if (MimicK.mimicK.Data.IsDead || MimicK.mimicK == null) return;
                Arrow arrow = new Arrow(Palette.ImpostorRed);
                arrow.arrow.SetActive(true);
                arrow.Update(MimicK.mimicK.transform.position);
                arrows.Add(arrow);

                // �����ީ`�˕r�g�򥻥å�
                updateTimer = arrowUpdateInterval;
            }
        }

        public void clearAndReload()
        {
            mimicA?.setDefaultLook();
            mimicA = null;
            isMorph = false;
            if (arrows != null)
                foreach (Arrow arrow in arrows)
                    if (arrow?.arrow != null)
                        UnityEngine.Object.Destroy(arrow.arrow);
            arrows = new List<Arrow>();
        }
    }

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
                msg = $"{p.Data.PlayerName} was The {string.Join(" ", RoleInfo.getRoleInfoForPlayer(p, false).Select(x => Helpers.cs(x.color, x.name)))}";

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

    public sealed class TaskMaster : RoleBase
    {
        public PlayerControl taskMaster = null;
        public bool becomeATaskMasterWhenCompleteAllTasks = false;
        public Color color = new Color32(225, 86, 75, byte.MaxValue);
        public bool isTaskComplete = false;
        public byte clearExTasks = 0;
        public byte allExTasks = 0;
        public byte oldTaskMasterPlayerId = byte.MaxValue;
        public bool triggerTaskMasterWin = false;

        public void clearAndReload()
        {
            taskMaster = null;
            becomeATaskMasterWhenCompleteAllTasks = CustomOptionHolder.taskMasterBecomeATaskMasterWhenCompleteAllTasks.getBool();
            isTaskComplete = false;
            clearExTasks = 0;
            allExTasks = 0;
            oldTaskMasterPlayerId = byte.MaxValue;
            triggerTaskMasterWin = false;
        }

        public bool isTaskMaster(byte playerId)
        {
            return taskMaster != null && taskMaster.PlayerId == playerId;
        }
    }

    public sealed class Yasuna : RoleBase
    {
        public PlayerControl yasuna;
        public Color color = new Color32(90, 255, 25, byte.MaxValue);
        public byte specialVoteTargetPlayerId = byte.MaxValue;
        private int _remainingSpecialVotes = 1;
        private Sprite targetSprite;

        public void clearAndReload()
        {
            yasuna = null;
            _remainingSpecialVotes = Mathf.RoundToInt(CustomOptionHolder.yasunaNumberOfSpecialVotes.getFloat());
            specialVoteTargetPlayerId = byte.MaxValue;
        }

        public Sprite getTargetSprite(bool isImpostor)
        {
            if (targetSprite) return targetSprite;
            targetSprite = Helpers.loadSpriteFromResources(isImpostor ? "TheOtherRoles.Resources.EvilYasunaTargetIcon.png" : "TheOtherRoles.Resources.YasunaTargetIcon.png", 150f);
            return targetSprite;
        }

        public int remainingSpecialVotes(bool isVote = false)
        {
            if (yasuna == null)
                return 0;

            if (isVote)
                _remainingSpecialVotes = Mathf.Max(0, _remainingSpecialVotes - 1);
            return _remainingSpecialVotes;
        }

        public bool isYasuna(byte playerId)
        {
            return yasuna != null && yasuna.PlayerId == playerId;
        }
    }

    public sealed class Trapper : RoleBase
    {
        public PlayerControl trapper;
        public Color color = Palette.ImpostorRed;

        public float minDistance = 0f;
        public float maxDistance;
        public int numTrap;
        public float extensionTime;
        public float killTimer;
        public float cooldown;
        public float trapRange;
        public float penaltyTime;
        public float bonusTime;
        public bool isTrapKill = false;
        public bool meetingFlag;

        public Sprite trapButtonSprite;
        public DateTime placedTime;

        public Sprite getTrapButtonSprite()
        {
            if (trapButtonSprite) return trapButtonSprite;
            trapButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.TrapperButton.png", 115f);
            return trapButtonSprite;
        }

        public void setTrap()
        {
            var pos = CachedPlayer.LocalPlayer.PlayerControl.transform.position;
            byte[] buff = new byte[sizeof(float) * 2];
            Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
            Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));
            MessageWriter writer = AmongUsClient.Instance.StartRpc(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.PlaceTrap, SendOption.Reliable);
            writer.WriteBytesAndSize(buff);
            writer.EndMessage();
            RPCProcedure.placeTrap(buff);
            placedTime = DateTime.UtcNow;
        }

        public void clearAndReload()
        {
            trapper = null;
            numTrap = (int)CustomOptionHolder.trapperNumTrap.getFloat();
            extensionTime = CustomOptionHolder.trapperExtensionTime.getFloat();
            killTimer = CustomOptionHolder.trapperKillTimer.getFloat();
            cooldown = CustomOptionHolder.trapperCooldown.getFloat();
            trapRange = CustomOptionHolder.trapperTrapRange.getFloat();
            penaltyTime = CustomOptionHolder.trapperPenaltyTime.getFloat();
            bonusTime = CustomOptionHolder.trapperBonusTime.getFloat();
            meetingFlag = false;
            Trap.clearAllTraps();
        }
    }

    public sealed class BomberA : RoleBase
    {
        public PlayerControl bomberA;
        public Color color = Palette.ImpostorRed;

        public PlayerControl bombTarget;
        public PlayerControl currentTarget;
        public PlayerControl tmpTarget;

        public Sprite bomberButtonSprite;
        public Sprite releaseButtonSprite;
        public float updateTimer = 0f;
        public List<Arrow> arrows = new();
        public float arrowUpdateInterval = 0.5f;

        public float duration;
        public float cooldown;
        public bool countAsOne;
        public bool showEffects;
        public bool ifOneDiesBothDie;
        public bool hasOneVote;
        public bool alwaysShowArrow;

        public TMPro.TextMeshPro targetText;
        public TMPro.TextMeshPro partnerTargetText;
        public Dictionary<byte, PoolablePlayer> playerIcons = new();

        public Sprite getBomberButtonSprite()
        {
            if (bomberButtonSprite) return bomberButtonSprite;
            bomberButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.PlantBombButton.png", 115f);
            return bomberButtonSprite;
        }
        public Sprite getReleaseButtonSprite()
        {
            if (releaseButtonSprite) return releaseButtonSprite;
            releaseButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.ReleaseButton.png", 115f);
            return releaseButtonSprite;
        }

        public void arrowUpdate()
        {
            if ((BomberA.bombTarget == null || BomberB.bombTarget == null) && !alwaysShowArrow) return;
            if (bomberA.Data.IsDead)
            {
                if (arrows.FirstOrDefault().arrow != null) UnityEngine.Object.Destroy(arrows.FirstOrDefault().arrow);
                return;
            }
            // ǰ�ե�`�फ��νU�^�r�g��ޥ��ʥ�����
            updateTimer -= Time.fixedDeltaTime;

            // 1��U�^������Arrow�����
            if (updateTimer <= 0.0f)
            {

                // ǰ�ؤ�Arrow�򤹤٤��Ɨ�����
                foreach (Arrow arrow in arrows)
                    if (arrow != null)
                    {
                        arrow.arrow.SetActive(false);
                        UnityEngine.Object.Destroy(arrow.arrow);
                    }

                // Arrowsһ�E
                arrows = new List<Arrow>();
                /*if (BomberB.bomberB == null || BomberB.bomberB.Data.IsDead) return;
                // �෽��λ�ä�ʾ��Arrows���軭
                Arrow arrow = new Arrow(Palette.ImpostorRed);
                arrow.arrow.SetActive(true);
                arrow.Update(BomberB.bomberB.transform.position);
                arrows.Add(arrow);*/
                foreach (PlayerControl p in CachedPlayer.AllPlayers)
                {
                    if (p.Data.IsDead) continue;
                    if (p == BomberB.bomberB)
                    {
                        Arrow arrow;
                        arrow = new Arrow(Color.red);
                        arrow.arrow.SetActive(true);
                        arrow.Update(p.transform.position);
                        arrows.Add(arrow);
                    }
                }

                // �����ީ`�˕r�g�򥻥å�
                updateTimer = arrowUpdateInterval;
            }
        }

        public void playerIconsUpdate()
        {
            foreach (PoolablePlayer pp in TORMapOptions.playerIcons.Values) pp.gameObject.SetActive(false);
            //foreach (PoolablePlayer pp in TORMapOptions.playerIcons.Values) pp.gameObject.SetActive(false);
            if (BomberA.bomberA != null && BomberB.bomberB != null && !BomberB.bomberB.Data.IsDead && !BomberA.bomberA.Data.IsDead && !MeetingHud.Instance)
            {
                if (bombTarget != null && TORMapOptions.playerIcons.ContainsKey(bombTarget.PlayerId) && TORMapOptions.playerIcons[bombTarget.PlayerId].gameObject != null)
                {
                    var icon = TORMapOptions.playerIcons[bombTarget.PlayerId];
                    Vector3 bottomLeft = new Vector3(-0.82f, 0.19f, 0) + IntroCutsceneOnDestroyPatch.bottomLeft;
                    icon.gameObject.SetActive(true);
                    icon.transform.localPosition = bottomLeft + new Vector3(-0.25f, 0f, 0);
                    icon.transform.localScale = Vector3.one * 0.4f;
                    if (targetText == null)
                    {
                        targetText = UnityEngine.Object.Instantiate(icon.cosmetics.nameText, icon.cosmetics.nameText.transform.parent);
                        targetText.enableWordWrapping = false;
                        targetText.transform.localScale = Vector3.one * 1.5f;
                        targetText.transform.localPosition += new Vector3(0f, 1.7f, 0);
                    }
                    targetText.text = ModTranslation.getString("bomberYourTarget");
                    targetText.gameObject.SetActive(true);
                    targetText.transform.parent = icon.gameObject.transform;
                }
                // �෽���O�ä������`���åȤ��ʾ����
                if (BomberB.bombTarget != null && TORMapOptions.playerIcons.ContainsKey(BomberB.bombTarget.PlayerId) && TORMapOptions.playerIcons[BomberB.bombTarget.PlayerId].gameObject != null)
                {
                    var icon = TORMapOptions.playerIcons[BomberB.bombTarget.PlayerId];
                    Vector3 bottomLeft = new Vector3(-0.82f, 0.19f, 0) + IntroCutsceneOnDestroyPatch.bottomLeft;
                    icon.gameObject.SetActive(true);
                    icon.transform.localPosition = bottomLeft + new Vector3(1.0f, 0f, 0);
                    icon.transform.localScale = Vector3.one * 0.4f;
                    if (partnerTargetText == null)
                    {
                        partnerTargetText = UnityEngine.Object.Instantiate(icon.cosmetics.nameText, icon.cosmetics.nameText.transform.parent);
                        partnerTargetText.enableWordWrapping = false;
                        partnerTargetText.transform.localScale = Vector3.one * 1.5f;
                        partnerTargetText.transform.localPosition += new Vector3(0f, 1.7f, 0);
                    }
                    partnerTargetText.text = ModTranslation.getString("bomberPartnerTarget");
                    partnerTargetText.gameObject.SetActive(true);
                    partnerTargetText.transform.parent = icon.gameObject.transform;
                }
            }
        }

        public void clearAndReload()
        {
            bomberA = null;
            bombTarget = null;
            currentTarget = null;
            tmpTarget = null;
            playerIcons = new Dictionary<byte, PoolablePlayer>();
            targetText = null;
            partnerTargetText = null;
            foreach (PoolablePlayer pp in TORMapOptions.playerIcons.Values)
                pp?.gameObject?.SetActive(false);
            if (arrows != null)
                foreach (Arrow arrow in arrows)
                    if (arrow?.arrow != null)
                        UnityEngine.Object.Destroy(arrow.arrow);
            arrows = new List<Arrow>();

            duration = CustomOptionHolder.bomberDuration.getFloat();
            cooldown = CustomOptionHolder.bomberCooldown.getFloat();
            countAsOne = CustomOptionHolder.bomberCountAsOne.getBool();
            showEffects = CustomOptionHolder.bomberShowEffects.getBool();
            hasOneVote = CustomOptionHolder.bomberHasOneVote.getBool();
            ifOneDiesBothDie = CustomOptionHolder.bomberIfOneDiesBothDie.getBool();
            alwaysShowArrow = CustomOptionHolder.bomberAlwaysShowArrow.getBool();
        }
    }

    public sealed class BomberB : RoleBase
    {
        public PlayerControl bomberB;
        public Color color = Palette.ImpostorRed;

        public PlayerControl bombTarget;
        public PlayerControl tmpTarget;
        public PlayerControl currentTarget;
        public TMPro.TextMeshPro targetText;
        public TMPro.TextMeshPro partnerTargetText;
        public Dictionary<byte, PoolablePlayer> playerIcons = new();
        public Sprite bomberButtonSprite;
        public Sprite releaseButtonSprite;
        public float updateTimer = 0f;
        public List<Arrow> arrows = new();
        public float arrowUpdateInterval = 0.5f;

        public void playerIconsUpdate()
        {
            foreach (PoolablePlayer pp in TORMapOptions.playerIcons.Values) pp.gameObject.SetActive(false);
            //foreach (PoolablePlayer pp in TORMapOptions.playerIcons.Values) pp.gameObject.SetActive(false);
            if (BomberA.bomberA != null && BomberB.bomberB != null && !BomberB.bomberB.Data.IsDead && !BomberA.bomberA.Data.IsDead && !MeetingHud.Instance)
            {
                if (bombTarget != null && TORMapOptions.playerIcons.ContainsKey(bombTarget.PlayerId) && TORMapOptions.playerIcons[bombTarget.PlayerId].gameObject != null)
                {
                    var icon = TORMapOptions.playerIcons[bombTarget.PlayerId];
                    Vector3 bottomLeft = new Vector3(-0.82f, 0.19f, 0) + IntroCutsceneOnDestroyPatch.bottomLeft;
                    icon.gameObject.SetActive(true);
                    icon.transform.localPosition = bottomLeft + new Vector3(-0.25f, 0f, 0);
                    icon.transform.localScale = Vector3.one * 0.4f;
                    if (targetText == null)
                    {
                        targetText = UnityEngine.Object.Instantiate(icon.cosmetics.nameText, icon.cosmetics.nameText.transform.parent);
                        targetText.enableWordWrapping = false;
                        targetText.transform.localScale = Vector3.one * 1.5f;
                        targetText.transform.localPosition += new Vector3(0f, 1.7f, 0);
                    }
                    targetText.text = ModTranslation.getString("bomberYourTarget");
                    targetText.gameObject.SetActive(true);
                    targetText.transform.parent = icon.gameObject.transform;
                }
                // �෽���O�ä������`���åȤ��ʾ����
                if (BomberA.bombTarget != null && TORMapOptions.playerIcons.ContainsKey(BomberA.bombTarget.PlayerId) && TORMapOptions.playerIcons[BomberA.bombTarget.PlayerId].gameObject != null)
                {
                    var icon = TORMapOptions.playerIcons[BomberA.bombTarget.PlayerId];
                    Vector3 bottomLeft = new Vector3(-0.82f, 0.19f, 0) + IntroCutsceneOnDestroyPatch.bottomLeft;
                    icon.gameObject.SetActive(true);
                    icon.transform.localPosition = bottomLeft + new Vector3(1.0f, 0f, 0);
                    icon.transform.localScale = Vector3.one * 0.4f;
                    if (partnerTargetText == null)
                    {
                        partnerTargetText = UnityEngine.Object.Instantiate(icon.cosmetics.nameText, icon.cosmetics.nameText.transform.parent);
                        partnerTargetText.enableWordWrapping = false;
                        partnerTargetText.transform.localScale = Vector3.one * 1.5f;
                        partnerTargetText.transform.localPosition += new Vector3(0f, 1.7f, 0);
                    }
                    partnerTargetText.text = ModTranslation.getString("bomberPartnerTarget");
                    partnerTargetText.gameObject.SetActive(true);
                    partnerTargetText.transform.parent = icon.gameObject.transform;
                }
            }
        }

        public void arrowUpdate()
        {
            if ((BomberA.bombTarget == null || BomberB.bombTarget == null) && !BomberA.alwaysShowArrow) return;
            if (bomberB.Data.IsDead)
            {
                if (arrows.FirstOrDefault().arrow != null) UnityEngine.Object.Destroy(arrows.FirstOrDefault().arrow);
                return;
            }
            // ǰ�ե�`�फ��νU�^�r�g��ޥ��ʥ�����
            updateTimer -= Time.fixedDeltaTime;

            // 1��U�^������Arrow�����
            if (updateTimer <= 0.0f)
            {

                // ǰ�ؤ�Arrow�򤹤٤��Ɨ�����
                foreach (Arrow arrow in arrows)
                    if (arrow != null)
                    {
                        arrow.arrow.SetActive(false);
                        UnityEngine.Object.Destroy(arrow.arrow);
                    }

                // Arrowsһ�E
                arrows = new List<Arrow>();
                /*if (BomberA.bomberA == null || BomberA.bomberA.Data.IsDead) return;
                // �෽��λ�ä�ʾ��Arrows���軭
                Arrow arrow = new Arrow(Palette.ImpostorRed);

                arrow.arrow.SetActive(true);
                arrow.Update(BomberA.bomberA.transform.position);
                arrows.Add(arrow);*/
                foreach (PlayerControl p in CachedPlayer.AllPlayers)
                {
                    if (p.Data.IsDead) continue;
                    if (p == BomberA.bomberA)
                    {
                        Arrow arrow;
                        arrow = new Arrow(Color.red);
                        arrow.arrow.SetActive(true);
                        arrow.Update(p.transform.position);
                        arrows.Add(arrow);
                    }
                }
                // �����ީ`�˕r�g�򥻥å�
                updateTimer = arrowUpdateInterval;
            }
        }

        public Sprite getBomberButtonSprite()
        {
            if (bomberButtonSprite) return bomberButtonSprite;
            bomberButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.PlantBombButton.png", 115f);
            return bomberButtonSprite;
        }
        public Sprite getReleaseButtonSprite()
        {
            if (releaseButtonSprite) return releaseButtonSprite;
            releaseButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.ReleaseButton.png", 115f);
            return releaseButtonSprite;
        }

        public void clearAndReload()
        {
            bomberB = null;
            bombTarget = null;
            currentTarget = null;
            tmpTarget = null;
            foreach (PoolablePlayer pp in TORMapOptions.playerIcons.Values)
                pp?.gameObject?.SetActive(false);
            if (arrows != null)
                foreach (Arrow arrow in arrows)
                    if (arrow?.arrow != null)
                        UnityEngine.Object.Destroy(arrow.arrow);
            arrows = new List<Arrow>();
            playerIcons = new Dictionary<byte, PoolablePlayer>();
            targetText = null;
            partnerTargetText = null;
        }
    }

    public sealed class CreatedMadmate : RoleBase
    {
        public PlayerControl createdMadmate;

        public bool canEnterVents;
        public bool hasImpostorVision;
        public bool canSabotage;
        public bool canFixComm;
        public bool canDieToSheriff;
        public bool hasTasks;
        public int numTasks;

        public bool tasksComplete(PlayerControl player)
        {
            if (!hasTasks) return false;

            int counter = 0;
            int totalTasks = numTasks;
            if (totalTasks == 0) return true;
            foreach (var task in player.Data.Tasks)
                if (task.Complete)
                    counter++;
            return counter >= totalTasks;
        }

        public void clearAndReload()
        {
            createdMadmate = null;
            canEnterVents = CustomOptionHolder.createdMadmateCanEnterVents.getBool();
            canDieToSheriff = CustomOptionHolder.createdMadmateCanDieToSheriff.getBool();
            hasTasks = CustomOptionHolder.createdMadmateAbility.getBool();
            canSabotage = CustomOptionHolder.createdMadmateCanSabotage.getBool();
            canFixComm = CustomOptionHolder.createdMadmateCanFixComm.getBool();
            numTasks = (int)CustomOptionHolder.createdMadmateCommonTasks.getFloat();
        }
    }

    public sealed class Teleporter : RoleBase
    {
        public PlayerControl teleporter;
        public Color color = new Color32(164, 249, 255, byte.MaxValue);
        private Sprite teleportButtonSprite;
        public float teleportCooldown = 30f;
        public float sampleCooldown = 30f;
        public int teleportNumber = 5;
        public PlayerControl target1;
        public PlayerControl target2;
        public PlayerControl currentTarget;

        public Sprite getButtonSprite()
        {
            if (teleportButtonSprite) return teleportButtonSprite;
            teleportButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.TeleporterButton.png", 115f);
            return teleportButtonSprite;
        }

        public void clearAndReload()
        {
            teleporter = null;
            target1 = null;
            target2 = null;
            currentTarget = null;
            teleportCooldown = CustomOptionHolder.teleporterCooldown.getFloat();
            teleportNumber = (int)CustomOptionHolder.teleporterTeleportNumber.getFloat();
            sampleCooldown = CustomOptionHolder.teleporterSampleCooldown.getFloat();
        }
    }

    public sealed class EvilHacker : RoleBase
    {
        public PlayerControl evilHacker;
        public Color color = Palette.ImpostorRed;
        public bool canHasBetterAdmin = false;
        public bool canCreateMadmate = false;
        public bool canCreateMadmateFromJackal;
        public bool canInheritAbility;
        public bool canSeeDoorStatus;
        public PlayerControl fakeMadmate;
        public PlayerControl currentTarget;

        private Sprite buttonSprite;
        private Sprite madmateButtonSprite;

        public Sprite getButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            byte mapId = GameOptionsManager.Instance.currentNormalGameOptions.MapId;
            UseButtonSettings button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.PolusAdminButton]; // Polus
            if (Helpers.isSkeld() || mapId == 3) button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.AdminMapButton]; // Skeld || Dleks
            else if (Helpers.isMira()) button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.MIRAAdminButton]; // Mira HQ
            else if (Helpers.isAirship()) button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.AirshipAdminButton]; // Airship
            else if (Helpers.isFungle()) button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.AdminMapButton];
            buttonSprite = button.Image;
            return buttonSprite;
        }

        public Sprite getMadmateButtonSprite()
        {
            if (madmateButtonSprite) return madmateButtonSprite;
            madmateButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.SidekickButton.png", 115f);
            return madmateButtonSprite;
        }

        public bool isInherited()
        {
            return canInheritAbility && evilHacker != null && evilHacker.Data.IsDead && CachedPlayer.LocalPlayer.PlayerControl.Data.Role.IsImpostor;
        }

        public void clearAndReload()
        {
            evilHacker = null;
            currentTarget = null;
            fakeMadmate = null;
            canCreateMadmate = CustomOptionHolder.evilHackerCanCreateMadmate.getBool();
            canHasBetterAdmin = CustomOptionHolder.evilHackerCanHasBetterAdmin.getBool();
            canCreateMadmateFromJackal = CustomOptionHolder.evilHackerCanCreateMadmateFromJackal.getBool();
            canInheritAbility = CustomOptionHolder.evilHackerCanInheritAbility.getBool();
            canSeeDoorStatus = CustomOptionHolder.evilHackerCanSeeDoorStatus.getBool();
        }
    }

    public sealed class Blackmailer : RoleBase
    {
        public PlayerControl blackmailer;
        public Color color = Palette.ImpostorRed;
        public Color blackmailedColor = Palette.White;

        public bool alreadyShook = false;
        public PlayerControl blackmailed;
        public PlayerControl currentTarget;
        public float cooldown = 30f;
        private Sprite blackmailButtonSprite;
        private Sprite overlaySprite;

        public Sprite getBlackmailOverlaySprite()
        {
            if (overlaySprite) return overlaySprite;
            overlaySprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.BlackmailerOverlay.png", 100f);
            return overlaySprite;
        }

        public Sprite getBlackmailButtonSprite()
        {
            if (blackmailButtonSprite) return blackmailButtonSprite;
            blackmailButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.BlackmailerBlackmailButton.png", 115f);
            return blackmailButtonSprite;
        }

        public void clearAndReload()
        {
            blackmailer = null;
            currentTarget = null;
            blackmailed = null;
            alreadyShook = false;
            cooldown = CustomOptionHolder.blackmailerCooldown.getFloat();
        }
    }

    public sealed class Sherlock : RoleBase
    {
        public PlayerControl sherlock;
        public Color color = new Color32(248, 205, 70, byte.MaxValue);

        public int numTasks = 2;
        public float cooldown = 10f;
        public float investigateDistance = 5f;

        public int numUsed;
        public int killTimerCounter;

        public List<Tuple<byte, Tuple<byte, Vector3>>> killLog;

        public int numInvestigate = 0;
        public PlayerControl currentTarget;

        private Sprite watchIcon;
        private Sprite investigateIcon;

        public Sprite getInvestigateIcon()
        {
            if (investigateIcon) return investigateIcon;
            investigateIcon = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.SherlockInvestigate.png", 115f);
            return investigateIcon;
        }
        public Sprite getWatchIcon()
        {
            if (watchIcon) return watchIcon;
            watchIcon = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.SherlockWatch.png", 115f);
            return watchIcon;
        }

        private TMPro.TMP_Text text;

        public void investigateMessage(string message, float duration, Color color)
        {

            RoomTracker roomTracker = HudManager.Instance?.roomTracker;

            if (roomTracker != null)

            {
                GameObject gameObject = UnityEngine.Object.Instantiate(roomTracker.gameObject);

                gameObject.transform.SetParent(HudManager.Instance.transform);
                UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<RoomTracker>());

                // Use local position to place it in the player's view instead of the world location
                gameObject.transform.localPosition = new Vector3(0, -1.8f, gameObject.transform.localPosition.z);
                gameObject.transform.localScale *= 1.5f;

                text = gameObject.GetComponent<TMPro.TMP_Text>();
                text.text = message;
                text.color = color;

                HudManager.Instance.StartCoroutine(Effects.Lerp(duration, new Action<float>((p) =>
                {
                    if (p == 1f && text != null && text.gameObject != null)
                        UnityEngine.Object.Destroy(text.gameObject);
                })));
            }
        }

        public int getNumInvestigate()
        {
            int counter = Sherlock.sherlock.Data.Tasks.ToArray().Where(t => t.Complete).Count();
            return (int)Math.Floor((float)counter / numTasks);
        }

        public void clearAndReload()
        {
            sherlock = null;
            numUsed = 0;
            killLog = new();
            numTasks = Mathf.RoundToInt(CustomOptionHolder.sherlockRechargeTasksNumber.getFloat());
            cooldown = CustomOptionHolder.sherlockCooldown.getFloat();
            investigateDistance = CustomOptionHolder.sherlockInvestigateDistance.getFloat();
        }
    }

    public sealed class Veteran : RoleBase
    {
        public PlayerControl veteran;
        public Color color = new Color32(255, 77, 0, byte.MaxValue);

        public float alertDuration = 3f;
        public float cooldown = 30f;

        public int remainingAlerts = 5;

        public bool alertActive = false;

        private Sprite buttonSprite;
        public Sprite getButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.AlertButton.png", 115f);
            return buttonSprite;
        }

        public void clearAndReload()
        {
            veteran = null;
            alertActive = false;
            alertDuration = CustomOptionHolder.veteranAlertDuration.getFloat();
            cooldown = CustomOptionHolder.veteranCooldown.getFloat();
            remainingAlerts = Mathf.RoundToInt(CustomOptionHolder.veteranAlertNumber.getFloat());
        }
    }

    public sealed class JekyllAndHyde : RoleBase
    {
        public Color color = Color.grey;
        public PlayerControl jekyllAndHyde;
        public PlayerControl formerJekyllAndHyde;
        public PlayerControl currentTarget;

        public enum Status
        {
            None,
            Jekyll,
            Hyde,
        }

        public Status status;
        public int counter = 0;
        public int numberToWin = 3;
        public float suicideTimer = 40f;
        public bool reset = true;
        public float cooldown = 18f;
        public int numUsed;
        public bool oddIsJekyll;
        public bool triggerWin = false;
        public int numCommonTasks;
        public int numLongTasks;
        public int numShortTasks;
        public int numTasks;

        public bool isOdd(int n)
        {
            return n % 2 == 1;
        }

        public bool isJekyll()
        {
            if (status == Status.None)
            {
                var alive = PlayerControl.AllPlayerControls.GetFastEnumerator().ToArray().Where(x =>
                {
                    return !x.Data.IsDead;
                });
                bool ret = oddIsJekyll ? isOdd(alive.Count()) : !isOdd(alive.Count());
                return ret;
            }
            return status == Status.Jekyll;
        }

        public int getNumDrugs()
        {
            var p = jekyllAndHyde;
            int counter = p.Data.Tasks.ToArray().Where(t => t.Complete).Count();
            return (int)Math.Floor((float)counter / numTasks);
        }

        public void clearAndReload()
        {
            jekyllAndHyde = null;
            formerJekyllAndHyde = null;
            currentTarget = null;
            status = Status.None;
            counter = 0;
            triggerWin = false;
            numUsed = 0;
            numTasks = (int)CustomOptionHolder.jekyllAndHydeNumTasks.getFloat();
            numCommonTasks = (int)CustomOptionHolder.jekyllAndHydeCommonTasks.getFloat();
            numShortTasks = (int)CustomOptionHolder.jekyllAndHydeShortTasks.getFloat();
            numLongTasks = (int)CustomOptionHolder.jekyllAndHydeLongTasks.getFloat();
            reset = CustomOptionHolder.jekyllAndHydeResetAfterMeeting.getBool();
            numberToWin = (int)CustomOptionHolder.jekyllAndHydeNumberToWin.getFloat();
            cooldown = CustomOptionHolder.jekyllAndHydeCooldown.getFloat();
            suicideTimer = CustomOptionHolder.jekyllAndHydeSuicideTimer.getFloat();
        }
    }

    public sealed class Undertaker : RoleBase
    {
        public PlayerControl undertaker;
        public Color color = Palette.ImpostorRed;

        public DeadBody DraggedBody;
        public DeadBody TargetBody;
        public bool CanDropBody;

        public float speedDecrease = -50f;
        public bool disableVent = true;

        public Sprite dragButtonSprite;
        public Sprite dropButtonSprite;

        public void RpcDropBody(Vector3 position)
        {
            if (undertaker == null) return;
            var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.UndertakerDropBody, SendOption.Reliable, -1);
            writer.Write(position.x);
            writer.Write(position.y);
            writer.Write(position.z);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            DropBody(position);
        }

        public void RpcDragBody(byte playerId)
        {
            if (undertaker == null) return;
            var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.UndertakerDragBody, SendOption.Reliable, -1);
            writer.Write(playerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            DragBody(playerId);
        }

        public void DropBody(Vector3 position)
        {
            if (!DraggedBody) return;
            DraggedBody.transform.position = position;
            DraggedBody = null;
            TargetBody = null;
        }

        public void DragBody(byte playerId)
        {
            if (undertaker == null) return;
            var body = UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == playerId);
            if (body == null) return;
            DraggedBody = body;
        }

        public Sprite getDragButtonSprite()
        {
            if (dragButtonSprite) return dragButtonSprite;
            dragButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.DragButton.png", 115f);
            return dragButtonSprite;
        }

        public Sprite getDropButtonSprite()
        {
            if (dropButtonSprite) return dropButtonSprite;
            dropButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.DropButton.png", 115f);
            return dropButtonSprite;
        }

        public void clearAndReload()
        {
            undertaker = null;
            DraggedBody = null;
            TargetBody = null;

            speedDecrease = CustomOptionHolder.undertakerSpeedDecrease.getFloat();
            disableVent = CustomOptionHolder.undertakerDisableVent.getBool();
        }
    }


    public sealed class Prophet : RoleBase
    {
        public PlayerControl prophet;
        public Color32 color = new Color32(255, 204, 127, byte.MaxValue);

        public float cooldown = 30f;
        public bool powerCrewAsRed = false;
        public bool neutralAsRed = true;
        public bool canCallEmergency = false;
        public int examineNum = 3;
        public int examinesToBeRevealed = 1;
        public int examinesLeft;
        public bool revealProphet = true;
        public bool isRevealed = false;
        public List<Arrow> arrows = new List<Arrow>();

        public Dictionary<PlayerControl, bool> examined = new Dictionary<PlayerControl, bool>();
        public PlayerControl currentTarget;

        private Sprite buttonSprite;
        public Sprite getButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.SeerButton.png", 115f);
            return buttonSprite;
        }

        public bool isKiller(PlayerControl p)
        {
            return Helpers.isKiller(p)
                || (p == Sheriff.sheriff || p == Deputy.deputy || p == Veteran.veteran || p == Mayor.mayor || p == Swapper.swapper || p == Guesser.niceGuesser || p == Yasuna.yasuna) && powerCrewAsRed || Helpers.isNeutral(p) && neutralAsRed;
        }

        public void clearAndReload()
        {
            prophet = null;
            currentTarget = null;
            isRevealed = false;
            examined = new Dictionary<PlayerControl, bool>();
            revealProphet = CustomOptionHolder.prophetIsRevealed.getBool();
            cooldown = CustomOptionHolder.prophetCooldown.getFloat();
            examineNum = Mathf.RoundToInt(CustomOptionHolder.prophetNumExamines.getFloat());
            powerCrewAsRed = CustomOptionHolder.prophetPowerCrewAsRed.getBool();
            neutralAsRed = CustomOptionHolder.prophetNeutralAsRed.getBool();
            canCallEmergency = CustomOptionHolder.prophetCanCallEmergency.getBool();
            examinesToBeRevealed = Math.Min(examineNum, Mathf.RoundToInt(CustomOptionHolder.prophetExaminesToBeRevealed.getFloat()));
            examinesLeft = examineNum;
            if (arrows != null)
                foreach (Arrow arrow in arrows)
                    if (arrow?.arrow != null)
                        UnityEngine.Object.Destroy(arrow.arrow);
            arrows = new List<Arrow>();
        }
    }

    public sealed class EvilTracker : RoleBase
    {
        public PlayerControl evilTracker;
        public Color color = Palette.ImpostorRed;

        public float cooldown = 10f;
        public bool resetTargetAfterMeeting = true;
        public bool canSeeDeathFlash = true;
        public bool canSeeTargetPosition = true;
        public bool canSetTargetOnMeeting = true;
        public bool canSeeTargetTasks = false;

        public float updateTimer = 0f;
        public float arrowUpdateInterval = 0.5f;

        public PlayerControl target;
        public PlayerControl futureTarget;
        public PlayerControl currentTarget;
        public Sprite trackerButtonSprite;
        public Sprite arrowSprite;
        public List<Arrow> arrows = new();
        public Dictionary<string, TMPro.TMP_Text> impostorPositionText;
        public TMPro.TMP_Text targetPositionText;

        public Sprite getEvilTrackerButtonSprite()
        {
            if (trackerButtonSprite) return trackerButtonSprite;
            trackerButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.TrackerButton.png", 115f);
            return trackerButtonSprite;
        }

        public Sprite getArrowSprite()
        {
            if (!arrowSprite)
                arrowSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.Arrow.png", 300f);
            return arrowSprite;
        }

        public void arrowUpdate()
        {
            // ǰ�ե�`�फ��νU�^�r�g��ޥ��ʥ�����
            updateTimer -= Time.fixedDeltaTime;

            // 1��U�^������Arrow�����
            if (updateTimer <= 0.0f)
            {

                // ǰ�ؤ�Arrow�򤹤٤��Ɨ�����
                foreach (Arrow arrow in arrows)
                    if (arrow != null && arrow.arrow != null)
                    {
                        arrow.arrow.SetActive(false);
                        UnityEngine.Object.Destroy(arrow.arrow);
                    }

                // Arrowsһ�E
                arrows = new List<Arrow>();

                // ����ݥ����`��λ�ä�ʾ��Arrows���軭
                int count = 0;
                foreach (PlayerControl p in CachedPlayer.AllPlayers)
                {
                    if (p.Data.IsDead)
                    {
                        if ((p.Data.Role.IsImpostor || p == Spy.spy || p == Sidekick.sidekick && Sidekick.wasTeamRed
                        || p == Jackal.jackal && Jackal.wasTeamRed) && impostorPositionText.ContainsKey(p.Data.PlayerName))
                            impostorPositionText[p.Data.PlayerName].text = "";
                        continue;
                    }
                    Arrow arrow;
                    if (p.Data.Role.IsImpostor && p != CachedPlayer.LocalPlayer.PlayerControl || Spy.spy != null && p == Spy.spy || p == Sidekick.sidekick && Sidekick.wasTeamRed
                        || p == Jackal.jackal && Jackal.wasTeamRed)
                    {
                        arrow = new Arrow(Palette.ImpostorRed);
                        arrow.arrow.SetActive(true);
                        arrow.Update(p.transform.position);
                        arrows.Add(arrow);
                        count += 1;
                        if (!impostorPositionText.ContainsKey(p.Data.PlayerName))
                        {
                            RoomTracker roomTracker = FastDestroyableSingleton<HudManager>.Instance?.roomTracker;
                            if (roomTracker == null) return;
                            GameObject gameObject = UnityEngine.Object.Instantiate(roomTracker.gameObject);
                            UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<RoomTracker>());
                            gameObject.transform.SetParent(FastDestroyableSingleton<HudManager>.Instance.transform);
                            gameObject.transform.localPosition = new Vector3(0, -2.0f + 0.25f * count, gameObject.transform.localPosition.z);
                            gameObject.transform.localScale = Vector3.one * 1.0f;
                            TMPro.TMP_Text positionText = gameObject.GetComponent<TMPro.TMP_Text>();
                            positionText.alpha = 1.0f;
                            impostorPositionText.Add(p.Data.PlayerName, positionText);
                        }
                        PlainShipRoom room = Helpers.getPlainShipRoom(p);
                        impostorPositionText[p.Data.PlayerName].gameObject.SetActive(true);
                        if (room != null)
                            impostorPositionText[p.Data.PlayerName].text = "<color=#FF1919FF>" + $"{p.Data.PlayerName}(" + FastDestroyableSingleton<TranslationController>.Instance.GetString(room.RoomId) + ")</color>";
                        else
                            impostorPositionText[p.Data.PlayerName].text = "";
                    }
                }

                // ���`���åȤ�λ�ä�ʾ��Arrow���軭
                if (target != null && !target.Data.IsDead)
                {
                    Arrow arrow = new(Palette.CrewmateBlue);
                    arrow.arrow.SetActive(true);
                    arrow.Update(target.transform.position);
                    arrows.Add(arrow);
                    if (targetPositionText == null)
                    {
                        RoomTracker roomTracker = FastDestroyableSingleton<HudManager>.Instance?.roomTracker;
                        if (roomTracker == null) return;
                        GameObject gameObject = UnityEngine.Object.Instantiate(roomTracker.gameObject);
                        UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<RoomTracker>());
                        gameObject.transform.SetParent(FastDestroyableSingleton<HudManager>.Instance.transform);
                        gameObject.transform.localPosition = new Vector3(0, -2.0f, gameObject.transform.localPosition.z);
                        gameObject.transform.localScale = Vector3.one * 1.0f;
                        targetPositionText = gameObject.GetComponent<TMPro.TMP_Text>();
                        targetPositionText.alpha = 1.0f;
                    }
                    PlainShipRoom room = Helpers.getPlainShipRoom(target);
                    targetPositionText.gameObject.SetActive(true);
                    if (room != null)
                        targetPositionText.text = "<color=#8CFFFFFF>" + $"{target.Data.PlayerName}(" + FastDestroyableSingleton<TranslationController>.Instance.GetString(room.RoomId) + ")</color>";
                    else
                        targetPositionText.text = "";
                }
                else
                    if (targetPositionText != null)
                    targetPositionText.text = "";

                // �����ީ`�˕r�g�򥻥å�
                updateTimer = arrowUpdateInterval;
            }
        }

        public void clearAndReload()
        {
            evilTracker = null;
            target = null;
            futureTarget = null;
            currentTarget = null;
            if (arrows != null)
                foreach (Arrow arrow in arrows)
                    if (arrow?.arrow != null)
                        UnityEngine.Object.Destroy(arrow.arrow);
            arrows = new List<Arrow>();
            if (impostorPositionText != null)
                foreach (var p in impostorPositionText.Values)
                    if (p != null)
                        UnityEngine.Object.Destroy(p);
            impostorPositionText = new();
            if (targetPositionText != null) UnityEngine.Object.Destroy(targetPositionText);
            targetPositionText = null;

            cooldown = CustomOptionHolder.evilTrackerCooldown.getFloat();
            resetTargetAfterMeeting = CustomOptionHolder.evilTrackerResetTargetAfterMeeting.getBool();
            canSeeDeathFlash = CustomOptionHolder.evilTrackerCanSeeDeathFlash.getBool();
            canSeeTargetPosition = CustomOptionHolder.evilTrackerCanSeeTargetPosition.getBool();
            canSeeTargetTasks = CustomOptionHolder.evilTrackerCanSeeTargetTask.getBool();
            canSetTargetOnMeeting = CustomOptionHolder.evilTrackerCanSetTargetOnMeeting.getBool();
        }
    }

    public sealed class Shifter : RoleBase
    {
        public PlayerControl shifter;
        public List<int> pastShifters = new List<int>();
        public Color color = new Color32(102, 102, 102, byte.MaxValue);

        public PlayerControl futureShift;
        public PlayerControl currentTarget;
        public PlayerControl killer;
        public DeadPlayer.CustomDeathReason deathReason;
        public bool shiftModifiers = false;

        public bool isNeutral = false;
        public bool shiftPastShifters = false;

        private Sprite buttonSprite;
        public Sprite getButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.ShiftButton.png", 115f);
            return buttonSprite;
        }

        public void clearAndReload()
        {
            shifter = null;
            pastShifters = new List<int>();
            currentTarget = null;
            futureShift = null;
            killer = null;
            deathReason = DeadPlayer.CustomDeathReason.Disconnect; // Get something unreachable here
            shiftModifiers = CustomOptionHolder.shifterShiftsModifiers.getBool();
            shiftPastShifters = CustomOptionHolder.shifterPastShifters.getBool();
            isNeutral = false;
        }
    }

    public sealed class Witch : RoleBase
    {
        public PlayerControl witch;
        public Color color = Palette.ImpostorRed;

        public List<PlayerControl> futureSpelled = new List<PlayerControl>();
        public PlayerControl currentTarget;
        public PlayerControl spellCastingTarget;
        public float cooldown = 30f;
        public float spellCastingDuration = 2f;
        public float cooldownAddition = 10f;
        public float currentCooldownAddition = 0f;
        public bool canSpellAnyone = false;
        public bool triggerBothCooldowns = true;
        public bool witchVoteSavesTargets = true;

        private Sprite buttonSprite;
        public Sprite getButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.SpellButton.png", 115f);
            return buttonSprite;
        }

        private Sprite spelledOverlaySprite;
        public Sprite getSpelledOverlaySprite()
        {
            if (spelledOverlaySprite) return spelledOverlaySprite;
            spelledOverlaySprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.SpellButtonMeeting.png", 225f);
            return spelledOverlaySprite;
        }


        public override void clearAndReload()
        {
            witch = null;
            futureSpelled = new List<PlayerControl>();
            currentTarget = spellCastingTarget = null;
            cooldown = CustomOptionHolder.witchCooldown.getFloat();
            cooldownAddition = CustomOptionHolder.witchAdditionalCooldown.getFloat();
            currentCooldownAddition = 0f;
            canSpellAnyone = CustomOptionHolder.witchCanSpellAnyone.getBool();
            spellCastingDuration = CustomOptionHolder.witchSpellCastingDuration.getFloat();
            triggerBothCooldowns = CustomOptionHolder.witchTriggerBothCooldowns.getBool();
            witchVoteSavesTargets = CustomOptionHolder.witchVoteSavesTargets.getBool();
        }
    }

    public sealed class Watcher : RoleBase
    {
        public PlayerControl nicewatcher;
        public PlayerControl evilwatcher;
        public Color color = Palette.Purple;

        public void clear(byte playerId)
        {
            if (nicewatcher != null && nicewatcher.PlayerId == playerId) nicewatcher = null;
            else if (evilwatcher != null && evilwatcher.PlayerId == playerId) evilwatcher = null;
        }

        public void clearAndReload()
        {
            nicewatcher = null;
            evilwatcher = null;
        }
    }

    public sealed class Bait : RoleBase
    {
        public PlayerControl bait;
        public Color color = new Color32(0, 247, 255, byte.MaxValue);

        public bool highlightAllVents = false;
        public float reportDelay = 0f;
        public bool showKillFlash = true;

        public bool reported = false;

        public void clearAndReload()
        {
            bait = null;
            reported = false;
            highlightAllVents = CustomOptionHolder.baitHighlightAllVents.getBool();
            reportDelay = CustomOptionHolder.baitReportDelay.getFloat();
            showKillFlash = CustomOptionHolder.baitShowKillFlash.getBool();
        }
    }

    public sealed class Assassin : RoleBase
    {
        public PlayerControl assassin;
        public Color color = Palette.ImpostorRed;

        public PlayerControl assassinMarked;
        public PlayerControl currentTarget;
        public float cooldown = 30f;
        public float traceTime = 1f;
        public bool knowsTargetLocation = false;
        //public  float invisibleDuration = 5f;

        //public  float invisibleTimer = 0f;
        //public  bool isInvisble = false;
        private Sprite markButtonSprite;
        private Sprite killButtonSprite;
        public Arrow arrow = new Arrow(Color.black);
        public Sprite getMarkButtonSprite()
        {
            if (markButtonSprite) return markButtonSprite;
            markButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.AssassinMarkButton.png", 115f);
            return markButtonSprite;
        }

        public Sprite getKillButtonSprite()
        {
            if (killButtonSprite) return killButtonSprite;
            killButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.AssassinAssassinateButton.png", 115f);
            return killButtonSprite;
        }

        public override void clearAndReload()
        {
            assassin = null;
            currentTarget = assassinMarked = null;
            cooldown = CustomOptionHolder.assassinCooldown.getFloat();
            knowsTargetLocation = CustomOptionHolder.assassinKnowsTargetLocation.getBool();
            traceTime = CustomOptionHolder.assassinTraceTime.getFloat();
            //invisibleDuration = CustomOptionHolder.assassinInvisibleDuration.getFloat();
            //invisibleTimer = 0f;
            //isInvisble = false;
            if (arrow?.arrow != null) UnityEngine.Object.Destroy(arrow.arrow);
            arrow = new Arrow(Color.black);
            if (arrow.arrow != null) arrow.arrow.SetActive(false);
        }
    }

    public sealed class Moriarty : RoleBase
    {
        public PlayerControl moriarty;
        public PlayerControl formerMoriarty;
        public Color color = Color.green;

        public PlayerControl tmpTarget;
        public PlayerControl target;
        public PlayerControl currentTarget;
        public PlayerControl killTarget;
        public List<PlayerControl> brainwashed;

        public int counter;

        public float brainwashTime = 2f;
        public float brainwashCooldown = 30f;
        public int numberToWin = 3;

        public Sprite brainwashIcon;

        public List<Arrow> arrows = new();
        public float updateTimer = 0f;
        public float arrowUpdateInterval = 0.5f;
        public TMPro.TMP_Text targetPositionText;

        public bool triggerMoriartyWin = false;

        public Sprite getBrainwashIcon()
        {
            if (brainwashIcon) return brainwashIcon;
            brainwashIcon = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.BrainwashButton.png", 115f);
            return brainwashIcon;
        }

        public void arrowUpdate()
        {

            // ǰ�ե�`�फ��νU�^�r�g��ޥ��ʥ�����
            updateTimer -= Time.fixedDeltaTime;

            // 1��U�^������Arrow�����
            if (updateTimer <= 0.0f)
            {

                // ǰ�ؤ�Arrow�򤹤٤��Ɨ�����
                foreach (Arrow arrow in arrows)
                    if (arrow != null && arrow.arrow != null)
                    {
                        arrow.arrow.SetActive(false);
                        UnityEngine.Object.Destroy(arrow.arrow);
                    }

                // Arrowsһ�E
                arrows = new List<Arrow>();
                // ���`���åȤ�λ�ä�ʾ��Arrow���軭
                if (target != null && !target.Data.IsDead)
                {
                    Arrow arrow = new(Palette.CrewmateBlue);
                    arrow.arrow.SetActive(true);
                    arrow.Update(target.transform.position);
                    arrows.Add(arrow);
                    if (targetPositionText == null)
                    {
                        RoomTracker roomTracker = HudManager.Instance?.roomTracker;
                        if (roomTracker == null) return;
                        GameObject gameObject = UnityEngine.Object.Instantiate(roomTracker.gameObject);
                        UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<RoomTracker>());
                        gameObject.transform.SetParent(HudManager.Instance.transform);
                        gameObject.transform.localPosition = new Vector3(0, -2.0f, gameObject.transform.localPosition.z);
                        gameObject.transform.localScale = Vector3.one * 1.0f;
                        targetPositionText = gameObject.GetComponent<TMPro.TMP_Text>();
                        targetPositionText.alpha = 1.0f;
                    }
                    PlainShipRoom room = Helpers.getPlainShipRoom(target);
                    targetPositionText.gameObject.SetActive(true);
                    int nearestPlayer = 0;
                    foreach (var p in PlayerControl.AllPlayerControls)
                        if (p != target && !p.Data.IsDead)
                        {
                            float dist = Vector2.Distance(p.transform.position, target.transform.position);
                            if (dist < 7f) nearestPlayer += 1;
                        }
                    if (room != null)
                        targetPositionText.text = "<color=#8CFFFFFF>" + $"{target.Data.PlayerName}({nearestPlayer})(" + DestroyableSingleton<TranslationController>.Instance.GetString(room.RoomId) + ")</color>";
                    else
                        targetPositionText.text = "<color=#8CFFFFFF>" + $"{target.Data.PlayerName}({nearestPlayer})</color>";
                }
                else
                    if (targetPositionText != null)
                    targetPositionText.text = "";

                // �����ީ`�˕r�g�򥻥å�
                updateTimer = arrowUpdateInterval;
            }
        }

        public void clearAndReload()
        {
            moriarty = null;
            formerMoriarty = null;
            tmpTarget = null;
            target = null;
            currentTarget = null;
            killTarget = null;
            brainwashed = new List<PlayerControl>();
            counter = 0;
            triggerMoriartyWin = false;
            brainwashCooldown = CustomOptionHolder.moriartyBrainwashCooldown.getFloat();
            brainwashTime = CustomOptionHolder.moriartyBrainwashTime.getFloat();
            numberToWin = (int)CustomOptionHolder.moriartyNumberToWin.getFloat();

            if (targetPositionText != null) UnityEngine.Object.Destroy(targetPositionText);
            targetPositionText = null;
            if (arrows != null)
                foreach (Arrow arrow in arrows)
                    if (arrow?.arrow != null)
                        UnityEngine.Object.Destroy(arrow.arrow);
            arrows = new List<Arrow>();
        }
    }

    public sealed class Akujo : RoleBase
    {
        public Color color = new Color32(142, 69, 147, byte.MaxValue);
        public PlayerControl akujo;
        public PlayerControl honmei;
        public List<PlayerControl> keeps;
        public PlayerControl currentTarget;
        public DateTime startTime;

        public float timeLimit = 1300f;
        public bool knowsRoles = true;
        public int timeLeft;
        public int keepsLeft;
        public int numKeeps;

        private Sprite honmeiSprite;
        public Sprite getHonmeiSprite()
        {
            if (honmeiSprite) return honmeiSprite;
            honmeiSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.AkujoHonmeiButton.png", 115f);
            return honmeiSprite;
        }

        private Sprite keepSprite;
        public Sprite getKeepSprite()
        {
            if (keepSprite) return keepSprite;
            keepSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.AkujoKeepButton.png", 115f);
            return keepSprite;
        }

        public void breakLovers(PlayerControl lover)
        {
            if (Lovers.lover1 != null && lover == Lovers.lover1 || Lovers.lover2 != null && lover == Lovers.lover2)
            {
                PlayerControl otherLover = lover.getPartner();
                if (otherLover != null)
                {
                    Lovers.clearAndReload();
                    otherLover.MurderPlayer(otherLover, MurderResultFlags.Succeeded);
                    GameHistory.overrideDeathReasonAndKiller(otherLover, DeadPlayer.CustomDeathReason.LoveStolen);
                }
            }
        }

        public void clearAndReload()
        {
            akujo = null;
            honmei = null;
            keeps = new List<PlayerControl>();
            currentTarget = null;
            startTime = DateTime.UtcNow;
            timeLimit = CustomOptionHolder.akujoTimeLimit.getFloat() + 10f;
            knowsRoles = CustomOptionHolder.akujoKnowsRoles.getBool();
            timeLeft = (int)Math.Ceiling(timeLimit - (DateTime.UtcNow - startTime).TotalSeconds);
            numKeeps = Math.Min((int)CustomOptionHolder.akujoNumKeeps.getFloat(), PlayerControl.AllPlayerControls.Count - 2);
            keepsLeft = numKeeps;
        }
    }

    public sealed class Cupid : RoleBase
    {
        public PlayerControl cupid;
        public Color color = new Color32(246, 152, 150, byte.MaxValue);

        public PlayerControl lovers1;
        public PlayerControl lovers2;
        public PlayerControl shielded;
        public PlayerControl currentTarget;
        public PlayerControl shieldTarget;
        public DateTime startTime = DateTime.UtcNow;
        public bool isShieldOn = false;
        public int timeLeft;
        public float timeLimit;

        private Sprite arrowSprite;
        public Sprite getArrowSprite()
        {
            if (arrowSprite) return arrowSprite;
            arrowSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.CupidButton.png", 115f);
            return arrowSprite;
        }

        public void breakLovers(PlayerControl lover)
        {
            if (Lovers.lover1 != null && lover == Lovers.lover1 || Lovers.lover2 != null && lover == Lovers.lover2)
            {
                PlayerControl otherLover = lover.getPartner();
                if (otherLover != null && !otherLover.Data.IsDead)
                {
                    Lovers.clearAndReload();
                    otherLover.MurderPlayer(otherLover, MurderResultFlags.Succeeded);
                    GameHistory.overrideDeathReasonAndKiller(otherLover, DeadPlayer.CustomDeathReason.LoveStolen);
                }
            }
        }

        public void clearAndReload(bool resetLovers = true)
        {
            cupid = null;
            if (resetLovers)
            {
                lovers1 = null;
                lovers2 = null;
            }
            shielded = null;
            currentTarget = null;
            shieldTarget = null;
            startTime = DateTime.UtcNow;
            timeLimit = CustomOptionHolder.cupidTimeLimit.getFloat() + 10f;
            timeLeft = (int)Math.Ceiling(timeLimit - (DateTime.UtcNow - startTime).TotalSeconds);
            isShieldOn = CustomOptionHolder.cupidShield.getBool();
        }
    }

    public sealed class Fox : RoleBase
    {
        public PlayerControl fox;
        public Color color = new Color32(167, 87, 168, byte.MaxValue);

        public enum TaskType
        {
            Serial,
            Parallel
        }

        public List<Arrow> arrows = new();
        public float updateTimer = 0f;
        public float arrowUpdateInterval = 0.5f;
        public bool crewWinsByTasks = false;
        public bool impostorWinsBySabotage = true;
        public float stealthCooldown;
        public float stealthDuration;
        public int numTasks;
        public float stayTime;

        public bool stealthed = false;
        public DateTime stealthedAt = DateTime.UtcNow;
        public float fadeTime = 1f;

        public int numRepair = 0;
        public bool canCreateImmoralist;
        public PlayerControl currentTarget;
        public TaskType taskType;

        private Sprite hideButtonSprite;
        private Sprite repairButtonSprite;
        private Sprite immoralistButtonSprite;

        public Sprite getHideButtonSprite()
        {
            if (hideButtonSprite) return hideButtonSprite;
            hideButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.FoxHideButton.png", 115f);
            return hideButtonSprite;
        }

        public Sprite getRepairButtonSprite()
        {
            if (repairButtonSprite) return repairButtonSprite;
            repairButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.RepairButton.png", 115f);
            return repairButtonSprite;
        }

        public Sprite getImmoralistButtonSprite()
        {
            if (immoralistButtonSprite) return immoralistButtonSprite;
            immoralistButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.FoxImmoralistButton.png", 115f);
            return immoralistButtonSprite;
        }

        public float stealthFade()
        {
            if (fox != null && !fox.Data.IsDead)
                return Mathf.Min(1.0f, (float)(DateTime.UtcNow - stealthedAt).TotalSeconds / fadeTime);
            return 1.0f;
        }

        public void setStealthed(bool stealthed = true)
        {
            Fox.stealthed = stealthed;
            stealthedAt = DateTime.UtcNow;
        }

        public void setOpacity(PlayerControl player, float opacity)
        {
            var color = Color.Lerp(Palette.ClearWhite, Palette.White, opacity);
            try
            {
                if (Chameleon.chameleon.Any(x => x.PlayerId == player.PlayerId) && Chameleon.visibility(player.PlayerId) < 1f && !stealthed) return;
                Helpers.setInvisible(player, color);
            }
            catch { }
        }

        public bool tasksComplete()
        {
            if (fox == null) return false;
            if (fox.Data.IsDead) return false;
            int counter = 0;
            int totalTasks = 1;
            foreach (var task in fox.Data.Tasks)
                if (task.Complete)
                    counter++;
            return counter == totalTasks;
        }

        public void arrowUpdate()
        {
            // ǰ�ե�`�फ��νU�^�r�g��ޥ��ʥ�����
            updateTimer -= Time.fixedDeltaTime;

            // 1��U�^������Arrow�����
            if (updateTimer <= 0.0f)
            {

                // ǰ�ؤ�Arrow�򤹤٤��Ɨ�����
                foreach (Arrow arrow in arrows)
                    if (arrow?.arrow != null)
                    {
                        arrow.arrow.SetActive(false);
                        UnityEngine.Object.Destroy(arrow.arrow);
                    }

                // Arrowsһ�E
                arrows = new List<Arrow>();

                // ����ݥ����`��λ�ä�ʾ��Arrows���軭
                foreach (PlayerControl p in CachedPlayer.AllPlayers)
                {
                    if (p.Data.IsDead) continue;
                    Arrow arrow;
                    // float distance = Vector2.Distance(p.transform.position, PlayerControl.LocalPlayer.transform.position);
                    if (p.Data.Role.IsImpostor || p == Jackal.jackal || p == Sheriff.sheriff || p == JekyllAndHyde.jekyllAndHyde || p == Moriarty.moriarty || p == Thief.thief)
                    {
                        if (p.Data.Role.IsImpostor)
                            arrow = new Arrow(Palette.ImpostorRed);
                        else if (p == Jackal.jackal)
                            arrow = new Arrow(Jackal.color);
                        else if (p == Sheriff.sheriff)
                            arrow = new Arrow(Palette.White);
                        else if (p == JekyllAndHyde.jekyllAndHyde)
                            arrow = new Arrow(JekyllAndHyde.color);
                        else if (p == Moriarty.moriarty)
                            arrow = new Arrow(Moriarty.color);
                        else
                            arrow = new Arrow(Thief.color);
                        arrow.arrow.SetActive(true);
                        arrow.Update(p.transform.position);
                        arrows.Add(arrow);
                    }
                }

                // �����ީ`�˕r�g�򥻥å�
                updateTimer = arrowUpdateInterval;
            }
            else
                arrows.Do(x => x.Update());
        }

        public void clearAndReload()
        {
            setOpacity(fox, 1.0f);
            fox = null;
            currentTarget = null;
            stealthed = false;
            stealthedAt = DateTime.UtcNow;
            crewWinsByTasks = CustomOptionHolder.foxCrewWinsByTasks.getBool();
            impostorWinsBySabotage = CustomOptionHolder.foxImpostorWinsBySabotage.getBool();
            stealthCooldown = CustomOptionHolder.foxStealthCooldown.getFloat();
            stealthDuration = CustomOptionHolder.foxStealthDuration.getFloat();
            canCreateImmoralist = CustomOptionHolder.foxCanCreateImmoralist.getBool();
            numTasks = (int)CustomOptionHolder.foxNumTasks.getFloat();
            numRepair = (int)CustomOptionHolder.foxNumRepairs.getFloat();
            stayTime = (int)CustomOptionHolder.foxStayTime.getFloat();
            taskType = (TaskType)CustomOptionHolder.foxTaskType.getSelection();
            foreach (Arrow arrow in arrows)
                if (arrow?.arrow != null)
                {
                    arrow.arrow.SetActive(false);
                    UnityEngine.Object.Destroy(arrow.arrow);
                }
            arrows = new List<Arrow>();
            Immoralist.clearAndReload();
        }

        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
        public sealed class PlayerPhysicsFoxPatch : RoleBase
        {
            public void Postfix(PlayerPhysics __instance)
            {
                if (fox != null && fox == __instance.myPlayer)
                {
                    var fox = __instance.myPlayer;
                    if (fox == null || fox.Data.IsDead) return;

                    bool canSee =
                        CachedPlayer.LocalPlayer.PlayerControl == fox ||
                        CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead ||
                        CachedPlayer.LocalPlayer.PlayerControl == Lighter.lighter && Lighter.canSeeInvisible ||
                        CachedPlayer.LocalPlayer.PlayerControl == Immoralist.immoralist;

                    var opacity = canSee ? 0.1f : 0.0f;

                    if (stealthed)
                    {
                        opacity = Math.Max(opacity, 1.0f - stealthFade());
                        fox.cosmetics?.currentBodySprite?.BodySprite.material.SetFloat("_Outline", 0f);
                    }
                    else
                        opacity = Math.Max(opacity, stealthFade());

                    setOpacity(fox, opacity);
                }
            }
        }
    }

    public sealed class Immoralist : RoleBase
    {
        public PlayerControl immoralist;
        public Color color = Fox.color;

        public List<Arrow> arrows = new();
        public float updateTimer = 0f;
        public float arrowUpdateInterval = 1f;

        private Sprite buttonSprite;
        public Sprite getButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.SuicideButton.png", 115f);
            return buttonSprite;
        }

        public void arrowUpdate()
        {
            // ǰ�ե�`�फ��νU�^�r�g��ޥ��ʥ�����
            updateTimer -= Time.fixedDeltaTime;

            // 1��U�^������Arrow�����
            if (updateTimer <= 0.0f)
            {
                // ǰ�ؤ�Arrow�򤹤٤��Ɨ�����
                foreach (Arrow arrow in arrows)
                    if (arrow?.arrow != null)
                    {
                        arrow.arrow.SetActive(false);
                        UnityEngine.Object.Destroy(arrow.arrow);
                    }

                // Arrowһ�E
                arrows = new List<Arrow>();

                // ����λ�ä�ʾ��Arrow���軭
                foreach (PlayerControl p in CachedPlayer.AllPlayers)
                {
                    if (p.Data.IsDead) continue;
                    Arrow arrow;
                    if (p == Fox.fox)
                    {
                        arrow = new Arrow(Fox.color);
                        arrow.arrow.SetActive(true);
                        arrow.Update(p.transform.position);
                        arrows.Add(arrow);
                    }
                }
                // �����ީ`�˕r�g�򥻥å�
                updateTimer = arrowUpdateInterval;
            }
            else
                arrows.Do(x => x.Update());
        }

        public void clearAndReload()
        {
            immoralist = null;
            foreach (Arrow arrow in arrows)
                if (arrow?.arrow != null)
                {
                    arrow.arrow.SetActive(false);
                    UnityEngine.Object.Destroy(arrow.arrow);
                }
            arrows = new List<Arrow>();
        }
    }

    public sealed class PlagueDoctor : RoleBase
    {
        public PlayerControl plagueDoctor;
        public Color color = new Color32(255, 192, 0, byte.MaxValue);

        public Dictionary<int, PlayerControl> infected;
        public Dictionary<int, float> progress;
        public Dictionary<int, bool> dead;
        public TMPro.TMP_Text statusText = null;
        public bool triggerPlagueDoctorWin = false;

        public PlayerControl currentTarget;
        public int numInfections = 0;
        public bool meetingFlag = false;

        public float infectCooldown = 10f;
        public int maxInfectable;
        public float infectDistance = 1f;
        public float infectDuration = 5f;
        public float immunityTime = 10f;
        public bool infectKiller = true;
        public bool canWinDead = true;

        public Sprite plagueDoctorIcon;

        public Sprite getSyringeIcon()
        {
            if (plagueDoctorIcon) return plagueDoctorIcon;
            plagueDoctorIcon = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.InfectButton.png", 115f);
            return plagueDoctorIcon;
        }

        public void updateDead()
        {
            foreach (var pc in PlayerControl.AllPlayerControls.GetFastEnumerator())
                dead[pc.PlayerId] = pc.Data.IsDead;
        }

        public bool hasInfected()
        {
            bool flag = false;
            foreach (var item in progress)
                if (item.Value != 0f)
                {
                    flag = true;
                    break;
                }
            return flag;
        }

        public string getProgressString(float progress)
        {
            // Go from green -> yellow -> red based on infection progress
            Color color;
            var prog = progress / infectDuration;
            if (prog < 0.5f)
                color = Color.Lerp(Color.green, Color.yellow, prog * 2);
            else
                color = Color.Lerp(Color.yellow, Color.red, prog * 2 - 1);

            float progPercent = prog * 100;
            return Helpers.cs(color, $"{progPercent.ToString("F1")}%");
        }

        public void checkWinStatus()
        {
            bool winFlag = true;
            foreach (PlayerControl p in CachedPlayer.AllPlayers)
            {
                if (p.Data.IsDead) continue;
                if (p == plagueDoctor) continue;
                if (!infected.ContainsKey(p.PlayerId))
                {
                    winFlag = false;
                    break;
                }
            }

            if (winFlag)
            {
                MessageWriter winWriter = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.PlagueDoctorWin, SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(winWriter);
                RPCProcedure.plagueDoctorWin();
            }
        }

        public void clearAndReload()
        {
            plagueDoctor = null;
            infectCooldown = CustomOptionHolder.plagueDoctorInfectCooldown.getFloat();
            maxInfectable = Mathf.RoundToInt(CustomOptionHolder.plagueDoctorNumInfections.getFloat());
            infectDistance = CustomOptionHolder.plagueDoctorDistance.getFloat();
            infectDuration = CustomOptionHolder.plagueDoctorDuration.getFloat();
            immunityTime = CustomOptionHolder.plagueDoctorImmunityTime.getFloat();
            infectKiller = CustomOptionHolder.plagueDoctorInfectKiller.getBool();
            canWinDead = CustomOptionHolder.plagueDoctorWinDead.getBool();
            meetingFlag = false;
            triggerPlagueDoctorWin = false;
            numInfections = maxInfectable;
            currentTarget = null;
            infected = new Dictionary<int, PlayerControl>();
            progress = new Dictionary<int, float>();
            dead = new Dictionary<int, bool>();
            if (statusText != null) UnityEngine.Object.Destroy(statusText);
            statusText = null;
        }
    }

    public sealed class Opportunist : RoleBase
    {
        public PlayerControl opportunist;
        public Color color = new Color32(0, 255, 00, byte.MaxValue);

        public void clearAndReload()
        {
            opportunist = null;
        }
    }

    public sealed class Ninja : RoleBase
    {
        public PlayerControl ninja;
        public Color color = Palette.ImpostorRed;
        public float stealthCooldown = 30f;
        public float stealthDuration = 15f;
        public float killPenalty = 10f;
        public float speedBonus = 1.25f;
        public float fadeTime = 0.5f;
        public bool canUseVents = false;
        public bool canBeTargeted;
        public float addition = 0f;

        public bool penalized = false;
        public bool stealthed = false;
        public DateTime stealthedAt = DateTime.UtcNow;

        private Sprite buttonSprite;
        public Sprite getButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.NinjaButton.png", 115f);
            return buttonSprite;
        }

        public bool isStealthed(PlayerControl player)
        {
            if (Ninja.ninja != null && !Ninja.ninja.Data.IsDead && Ninja.ninja == player)
                return Ninja.stealthed;
            return false;
        }

        public float stealthFade(PlayerControl player)
        {
            if (Ninja.ninja == player && fadeTime > 0 && !Ninja.ninja.Data.IsDead)
                return Mathf.Min(1.0f, (float)(DateTime.UtcNow - Ninja.stealthedAt).TotalSeconds / fadeTime);
            return 1.0f;
        }

        public bool isPenalized(PlayerControl player)
        {
            if (Ninja.ninja == player && !Ninja.ninja.Data.IsDead)
                return Ninja.penalized;
            return false;
        }

        public void setStealthed(PlayerControl player, bool stealthed = true)
        {
            if (Ninja.ninja == player && Ninja.ninja != null)
            {
                Ninja.stealthed = stealthed;
                Ninja.stealthedAt = DateTime.UtcNow;
            }
        }

        public void OnKill(PlayerControl target)
        {
            if (Ninja.stealthed)
            {
                Ninja.addition += Ninja.killPenalty;
                if (CachedPlayer.LocalPlayer.PlayerControl == Ninja.ninja)
                {
                    Ninja.penalized = true;
                    CachedPlayer.LocalPlayer.PlayerControl.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + Ninja.addition);
                    Helpers.checkMurderAttemptAndKill(Ninja.ninja, target, false, false);
                }
            }
        }

        public void setOpacity(PlayerControl player, float opacity)
        {
            var color = Color.Lerp(Palette.ClearWhite, Palette.White, opacity);
            try
            {
                // Block setting opacity if the Chameleon skill is active
                if (Chameleon.chameleon.Any(x => x.PlayerId == player.PlayerId) && Chameleon.visibility(player.PlayerId) < 1f && !stealthed) return;
                Helpers.setInvisible(player, color);
            }
            catch { }
        }

        public void clearAndReload()
        {
            setOpacity(ninja, 1.0f);
            ninja = null;
            stealthCooldown = CustomOptionHolder.ninjaStealthCooldown.getFloat();
            stealthDuration = CustomOptionHolder.ninjaStealthDuration.getFloat();
            killPenalty = CustomOptionHolder.ninjaKillPenalty.getFloat();
            speedBonus = CustomOptionHolder.ninjaSpeedBonus.getFloat();
            fadeTime = CustomOptionHolder.ninjaFadeTime.getFloat();
            canUseVents = CustomOptionHolder.ninjaCanVent.getBool();
            canBeTargeted = CustomOptionHolder.ninjaCanBeTargeted.getBool();

            penalized = false;
            stealthed = false;
            stealthedAt = DateTime.UtcNow;
        }

        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
        public sealed class PlayerPhysicsNinjaPatch : RoleBase
        {
            public void Postfix(PlayerPhysics __instance)
            {
                if (__instance.AmOwner && __instance.myPlayer.CanMove && GameData.Instance && isStealthed(__instance.myPlayer))
                    __instance.body.velocity *= speedBonus + 1;

                if (__instance.myPlayer == Ninja.ninja)
                {
                    var ninja = __instance.myPlayer;
                    if (ninja == null || ninja.Data.IsDead) return;

                    bool canSee =
                        PlayerControl.LocalPlayer.Data.IsDead ||
                        PlayerControl.LocalPlayer.Data.Role.IsImpostor ||
                        Lighter.canSeeInvisible && PlayerControl.LocalPlayer == Lighter.lighter;

                    var opacity = canSee ? 0.1f : 0.0f;

                    if (isStealthed(ninja))
                    {
                        opacity = Math.Max(opacity, 1.0f - stealthFade(ninja));
                        ninja.cosmetics.currentBodySprite.BodySprite.material.SetFloat("_Outline", 0f);
                    }
                    else
                        opacity = Math.Max(opacity, stealthFade(ninja));

                    setOpacity(ninja, opacity);
                }
            }
        }
    }

    public sealed class Sprinter : RoleBase
    {
        public PlayerControl sprinter;
        public Color color = new Color32(128, 128, 255, byte.MaxValue);

        public float sprintCooldown = 30f;
        public float sprintDuration = 15f;
        public float fadeTime = 0.5f;

        public bool sprinting = false;

        public DateTime sprintAt = DateTime.UtcNow;

        private Sprite buttonSprite;
        public Sprite getButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.SprintButton.png", 115f);
            return buttonSprite;
        }

        public float sprintFade(PlayerControl player)
        {
            if (Sprinter.sprinter == player && fadeTime > 0 && !Sprinter.sprinter.Data.IsDead)
                return Mathf.Min(1.0f, (float)(DateTime.UtcNow - Sprinter.sprintAt).TotalSeconds / fadeTime);
            return 1.0f;
        }

        public bool isSprinting()
        {
            if (CachedPlayer.LocalPlayer.PlayerControl == Sprinter.sprinter && !Sprinter.sprinter.Data.IsDead)
                return Sprinter.sprinting;
            return false;
        }

        public void setSprinting(PlayerControl player, bool sprinting = true)
        {
            if (player == Sprinter.sprinter && !Sprinter.sprinter.Data.IsDead)
            {
                Sprinter.sprinting = sprinting;
                Sprinter.sprintAt = DateTime.UtcNow;
            }
        }

        public void setOpacity(PlayerControl player, float opacity)
        {
            var color = Color.Lerp(Palette.ClearWhite, Palette.White, opacity);
            try
            {
                if (Chameleon.chameleon.Any(x => x.PlayerId == player.PlayerId) && Chameleon.visibility(player.PlayerId) < 1f && !sprinting) return;
                Helpers.setInvisible(player, color);
            }
            catch { }
        }

        public void clearAndReload()
        {
            setOpacity(sprinter, 1.0f);
            sprinter = null;
            sprinting = false;
            sprintCooldown = CustomOptionHolder.sprinterCooldown.getFloat();
            sprintDuration = CustomOptionHolder.sprinterDuration.getFloat();
            fadeTime = CustomOptionHolder.sprinterFadeTime.getFloat();
        }

        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
        public sealed class PlayerPhysicsSprinterPatch : RoleBase
        {
            public void Postfix(PlayerPhysics __instance)
            {
                if (__instance.myPlayer == Sprinter.sprinter)
                {
                    var sprinter = __instance.myPlayer;
                    if (sprinter == null || sprinter.Data.IsDead) return;

                    bool canSee =
                        PlayerControl.LocalPlayer.Data.IsDead ||
                        CachedPlayer.LocalPlayer.PlayerControl == Sprinter.sprinter ||
                        Lighter.canSeeInvisible && PlayerControl.LocalPlayer == Lighter.lighter;

                    var opacity = canSee ? 0.1f : 0.0f;

                    if (Sprinter.sprinting)
                    {
                        opacity = Math.Max(opacity, 1.0f - sprintFade(sprinter));
                        sprinter.cosmetics.currentBodySprite.BodySprite.material.SetFloat("_Outline", 0f);
                    }
                    else
                        opacity = Math.Max(opacity, sprintFade(sprinter));

                    setOpacity(sprinter, opacity);
                }
            }
        }
    }

    public sealed class Thief : RoleBase
    {
        public PlayerControl thief;
        public Color color = new Color32(71, 99, 45, byte.MaxValue);
        public PlayerControl currentTarget;
        public PlayerControl formerThief;

        public float cooldown = 30f;

        public bool suicideFlag = false;  // Used as a flag for suicide

        public bool hasImpostorVision;
        public bool canUseVents;
        public bool canKillSheriff;
        public bool canStealWithGuess;

        public override void clearAndReload()
        {
            thief = null;
            suicideFlag = false;
            currentTarget = null;
            formerThief = null;
            hasImpostorVision = CustomOptionHolder.thiefHasImpVision.getBool();
            cooldown = CustomOptionHolder.thiefCooldown.getFloat();
            canUseVents = CustomOptionHolder.thiefCanUseVents.getBool();
            canKillSheriff = CustomOptionHolder.thiefCanKillSheriff.getBool();
            canStealWithGuess = CustomOptionHolder.thiefCanStealWithGuess.getBool();
        }

        public bool isFailedThiefKill(PlayerControl target, PlayerControl killer, RoleInfo targetRole)
        {
            return killer == Thief.thief && !target.Data.Role.IsImpostor && !new List<RoleInfo> { RoleInfo.jackal, canKillSheriff ? RoleInfo.sheriff : null, RoleInfo.sidekick, RoleInfo.moriarty, RoleInfo.jekyllAndHyde }.Contains(targetRole);
        }
    }

    /*public sealed class Trapper:RoleBase {
    public  PlayerControl trapper;
    public  Color color = new Color32(110, 57, 105, byte.MaxValue);

    public  float cooldown = 30f;
    public  int maxCharges = 5;
    public  int rechargeTasksNumber = 3;
    public  int rechargedTasks = 3;
    public  int charges = 1;
    public  int trapCountToReveal = 2;
    public  List<PlayerControl> playersOnMap = new List<PlayerControl>();
    public  bool anonymousMap = false;
    public  int infoType = 0; // 0 = Role, 1 = Good/Evil, 2 = Name
    public  float trapDuration = 5f;

    private  Sprite trapButtonSprite;

    public  Sprite getButtonSprite() {
        if (trapButtonSprite) return trapButtonSprite;
        trapButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.Trapper_Place_Button.png", 115f);
        return trapButtonSprite;
    }

    public override void clearAndReload() {
        trapper = null;
        cooldown = CustomOptionHolder.trapperCooldown.getFloat();
        maxCharges = Mathf.RoundToInt(CustomOptionHolder.trapperMaxCharges.getFloat());
        rechargeTasksNumber = Mathf.RoundToInt(CustomOptionHolder.trapperRechargeTasksNumber.getFloat());
        rechargedTasks = Mathf.RoundToInt(CustomOptionHolder.trapperRechargeTasksNumber.getFloat());
        charges = Mathf.RoundToInt(CustomOptionHolder.trapperMaxCharges.getFloat()) / 2;
        trapCountToReveal = Mathf.RoundToInt(CustomOptionHolder.trapperTrapNeededTriggerToReveal.getFloat());
        playersOnMap = new List<PlayerControl>();
        anonymousMap = CustomOptionHolder.trapperAnonymousMap.getBool();
        infoType = CustomOptionHolder.trapperInfoType.getSelection();
        trapDuration = CustomOptionHolder.trapperTrapDuration.getFloat();
    }
}*/

    /*public sealed class Bomber:RoleBase {
        public  PlayerControl bomber = null;
        public  Color color = Palette.ImpostorRed;

        public  Bomb bomb = null;
        public  bool isPlanted = false;
        public  bool isActive = false;
        public  float destructionTime = 20f;
        public  float destructionRange = 2f;
        public  float hearRange = 30f;
        public  float defuseDuration = 3f;
        public  float bombCooldown = 15f;
        public  float bombActiveAfter = 3f;

        private  Sprite buttonSprite;

        public  Sprite getButtonSprite() {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.Bomb_Button_Plant.png", 115f);
            return buttonSprite;
        }

        public  void clearBomb(bool flag = true) {
            if (bomb != null) {
                UnityEngine.Object.Destroy(bomb.bomb);
                UnityEngine.Object.Destroy(bomb.background);
                bomb = null;
            }
            isPlanted = false;
            isActive = false;
            if (flag) SoundEffectsManager.stop("bombFuseBurning");
        }

        public override void clearAndReload() {
            clearBomb(false);
            bomber = null;
            bomb = null;
            isPlanted = false;
            isActive = false;
            destructionTime = CustomOptionHolder.bomberBombDestructionTime.getFloat();
            destructionRange = CustomOptionHolder.bomberBombDestructionRange.getFloat() / 10;
            hearRange = CustomOptionHolder.bomberBombHearRange.getFloat() / 10;
            defuseDuration = CustomOptionHolder.bomberDefuseDuration.getFloat();
            bombCooldown = CustomOptionHolder.bomberBombCooldown.getFloat();
            bombActiveAfter = CustomOptionHolder.bomberBombActiveAfter.getFloat();
            Bomb.clearBackgroundSprite();
        }
    }*/

    // Modifier
    /*public sealed class Bait:RoleBase {
        public  List<PlayerControl> bait = new List<PlayerControl>();
        public  Dictionary<DeadPlayer, float> active = new Dictionary<DeadPlayer, float>();
        public  Color color = new Color32(0, 247, 255, byte.MaxValue);

        public  float reportDelayMin = 0f;
        public  float reportDelayMax = 0f;
        public  bool showKillFlash = true;

        public override void clearAndReload() {
            bait = new List<PlayerControl>();
            active = new Dictionary<DeadPlayer, float>();
            reportDelayMin = CustomOptionHolder.modifierBaitReportDelayMin.getFloat();
            reportDelayMax = CustomOptionHolder.modifierBaitReportDelayMax.getFloat();
            if (reportDelayMin > reportDelayMax) reportDelayMin = reportDelayMax;
            showKillFlash = CustomOptionHolder.modifierBaitShowKillFlash.getBool();
        }
    }*/

    public sealed class Bloody : RoleBase
    {
        public List<PlayerControl> bloody = new List<PlayerControl>();
        public Dictionary<byte, float> active = new Dictionary<byte, float>();
        public Dictionary<byte, byte> bloodyKillerMap = new Dictionary<byte, byte>();

        public float duration = 5f;

        public override void clearAndReload()
        {
            bloody = new List<PlayerControl>();
            active = new Dictionary<byte, float>();
            bloodyKillerMap = new Dictionary<byte, byte>();
            duration = CustomOptionHolder.modifierBloodyDuration.getFloat();
        }
    }

    public sealed class AntiTeleport : RoleBase
    {
        public List<PlayerControl> antiTeleport = new List<PlayerControl>();
        public Vector3 position;

        public override void clearAndReload()
        {
            antiTeleport = new List<PlayerControl>();
            position = Vector3.zero;
        }

        public void setPosition()
        {
            if (position == Vector3.zero) return;  // Check if this has been set, otherwise first spawn on submerged will fail
            if (antiTeleport.FindAll(x => x.PlayerId == CachedPlayer.LocalPlayer.PlayerId).Count > 0)
            {
                CachedPlayer.LocalPlayer.NetTransform.RpcSnapTo(position);
                if (SubmergedCompatibility.IsSubmerged)
                    SubmergedCompatibility.ChangeFloor(position.y > -7);
            }
        }
    }

    public sealed class Tiebreaker : RoleBase
    {
        public PlayerControl tiebreaker;

        public bool isTiebreak = false;

        public override void clearAndReload()
        {
            tiebreaker = null;
            isTiebreak = false;
        }
    }

    public sealed class Sunglasses : RoleBase
    {
        public List<PlayerControl> sunglasses = new List<PlayerControl>();
        public int vision = 1;

        public override void clearAndReload()
        {
            sunglasses = new List<PlayerControl>();
            vision = CustomOptionHolder.modifierSunglassesVision.getSelection() + 1;
        }
    }
    public sealed class Mini : RoleBase
    {
        public PlayerControl mini;
        public Color color = Color.yellow;
        public const float defaultColliderRadius = 0.2233912f;
        public const float defaultColliderOffset = 0.3636057f;

        public float growingUpDuration = 400f;
        public bool isGrowingUpInMeeting = true;
        public DateTime timeOfGrowthStart = DateTime.UtcNow;
        public DateTime timeOfMeetingStart = DateTime.UtcNow;
        public float ageOnMeetingStart = 0f;
        public bool triggerMiniLose = false;

        public override void clearAndReload()
        {
            mini = null;
            triggerMiniLose = false;
            growingUpDuration = CustomOptionHolder.modifierMiniGrowingUpDuration.getFloat();
            isGrowingUpInMeeting = CustomOptionHolder.modifierMiniGrowingUpInMeeting.getBool();
            timeOfGrowthStart = DateTime.UtcNow;
        }

        public float growingProgress()
        {
            float timeSinceStart = (float)(DateTime.UtcNow - timeOfGrowthStart).TotalMilliseconds;
            return Mathf.Clamp(timeSinceStart / (growingUpDuration * 1000), 0f, 1f);
        }

        public bool isGrownUp()
        {
            return growingProgress() == 1f;
        }

    }
    public sealed class Vip : RoleBase
    {
        public List<PlayerControl> vip = new List<PlayerControl>();
        public bool showColor = true;

        public override void clearAndReload()
        {
            vip = new List<PlayerControl>();
            showColor = CustomOptionHolder.modifierVipShowColor.getBool();
        }
    }

    public sealed class Invert : RoleBase
    {
        public List<PlayerControl> invert = new List<PlayerControl>();
        public int meetings = 3;

        public override void clearAndReload()
        {
            invert = new List<PlayerControl>();
            meetings = (int)CustomOptionHolder.modifierInvertDuration.getFloat();
        }
    }

    public sealed class Madmate : RoleBase
    {
        public Color color = Palette.ImpostorRed;
        public List<PlayerControl> madmate = new List<PlayerControl>();
        public bool hasTasks;
        public bool canDieToSheriff;
        public bool canVent;
        public bool hasImpostorVision;
        public bool canFixComm;
        public bool canSabotage;
        public int commonTasks;
        public int shortTasks;
        public int longTasks;

        public string fullName { get { return ModTranslation.getString("madmate"); } }
        public string prefix { get { return ModTranslation.getString("madmatePrefix"); } }

        public bool tasksComplete(PlayerControl player)
        {
            if (!hasTasks) return false;

            int counter = 0;
            int totalTasks = commonTasks + longTasks + shortTasks;
            if (totalTasks == 0) return true;
            foreach (var task in player.Data.Tasks)
                if (task.Complete)
                    counter++;
            return counter == totalTasks;
        }

        public void clearAndReload()
        {
            hasTasks = CustomOptionHolder.madmateAbility.getBool();
            madmate = new List<PlayerControl>();
            canDieToSheriff = CustomOptionHolder.madmateCanDieToSheriff.getBool();
            canVent = CustomOptionHolder.madmateCanEnterVents.getBool();
            hasImpostorVision = CustomOptionHolder.madmateHasImpostorVision.getBool();
            canFixComm = CustomOptionHolder.madmateCanFixComm.getBool();
            canSabotage = CustomOptionHolder.madmateCanSabotage.getBool();
            shortTasks = (int)CustomOptionHolder.madmateShortTasks.getFloat();
            commonTasks = (int)CustomOptionHolder.madmateCommonTasks.getFloat();
            longTasks = (int)CustomOptionHolder.madmateLongTasks.getFloat();
        }
    }

    public sealed class Chameleon : RoleBase
    {
        public List<PlayerControl> chameleon = new List<PlayerControl>();
        public float minVisibility = 0.2f;
        public float holdDuration = 1f;
        public float fadeDuration = 0.5f;
        public Dictionary<byte, float> lastMoved;

        public override void clearAndReload()
        {
            chameleon = new List<PlayerControl>();
            lastMoved = new Dictionary<byte, float>();
            holdDuration = CustomOptionHolder.modifierChameleonHoldDuration.getFloat();
            fadeDuration = CustomOptionHolder.modifierChameleonFadeDuration.getFloat();
            minVisibility = CustomOptionHolder.modifierChameleonMinVisibility.getSelection() / 10f;
        }

        public float visibility(byte playerId)
        {
            if (playerId == Ninja.ninja?.PlayerId && Ninja.stealthed || playerId == Sprinter.sprinter?.PlayerId && Sprinter.sprinting) return 1f;
            float visibility = 1f;
            if (lastMoved != null && lastMoved.ContainsKey(playerId))
            {
                var tStill = Time.time - lastMoved[playerId];
                if (tStill > holdDuration)
                    if (tStill - holdDuration > fadeDuration) visibility = minVisibility;
                    else visibility = (1 - (tStill - holdDuration) / fadeDuration) * (1 - minVisibility) + minVisibility;
            }
            if (PlayerControl.LocalPlayer.Data.IsDead && visibility < 0.1f)
                visibility = 0.1f;
            return visibility;
        }

        public void update()
        {
            foreach (var chameleonPlayer in chameleon)
            {
                //if (chameleonPlayer == Assassin.assassin && Assassin.isInvisble) continue;  // Dont make Assassin visible...
                if (chameleonPlayer == Ninja.ninja && Ninja.stealthed || chameleonPlayer == Sprinter.sprinter && Sprinter.sprinting) continue;
                // check movement by animation
                PlayerPhysics playerPhysics = chameleonPlayer.MyPhysics;
                var currentPhysicsAnim = playerPhysics.Animations.Animator.GetCurrentAnimation();
                if (currentPhysicsAnim != playerPhysics.Animations.group.IdleAnim)
                    lastMoved[chameleonPlayer.PlayerId] = Time.time;
                // calculate and set visibility
                float visibility = Chameleon.visibility(chameleonPlayer.PlayerId);
                float petVisibility = visibility;
                if (chameleonPlayer.Data.IsDead)
                {
                    visibility = 0.5f;
                    petVisibility = 1f;
                }

                try
                {  // Sometimes renderers are missing for weird reasons. Try catch to avoid exceptions
                    chameleonPlayer.cosmetics.currentBodySprite.BodySprite.color = chameleonPlayer.cosmetics.currentBodySprite.BodySprite.color.SetAlpha(visibility);
                    if (DataManager.Settings.Accessibility.ColorBlindMode) chameleonPlayer.cosmetics.colorBlindText.color = chameleonPlayer.cosmetics.colorBlindText.color.SetAlpha(visibility);
                    chameleonPlayer.SetHatAndVisorAlpha(visibility);
                    chameleonPlayer.cosmetics.skin.layer.color = chameleonPlayer.cosmetics.skin.layer.color.SetAlpha(visibility);
                    chameleonPlayer.cosmetics.nameText.color = chameleonPlayer.cosmetics.nameText.color.SetAlpha(visibility);
                    foreach (var rend in chameleonPlayer.cosmetics.currentPet.renderers) rend.color = rend.color.SetAlpha(petVisibility);
                    foreach (var shadowRend in chameleonPlayer.cosmetics.currentPet.shadows) shadowRend.color = shadowRend.color.SetAlpha(petVisibility);

                    //if (chameleonPlayer.cosmetics.skin.layer.color == chameleonPlayer.cosmetics.skin.layer.color.SetAlpha(visibility) && visibility == minVisibility) TheOtherRolesPlugin.Logger.LogMessage("Chameleon");
                    //chameleonPlayer.cosmetics.currentPet.renderers[0].color = chameleonPlayer.cosmetics.currentPet.renderers[0].color.SetAlpha(petVisibility);
                    //chameleonPlayer.cosmetics.currentPet.shadows[0].color = chameleonPlayer.cosmetics.currentPet.shadows[0].color.SetAlpha(petVisibility);
                }
                catch { }
            }

        }
    }

    /*public sealed class Shifter:RoleBase {
        public  PlayerControl shifter;

        public  PlayerControl futureShift;
        public  PlayerControl currentTarget;

        private  Sprite buttonSprite;
        public  Sprite getButtonSprite() {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.ShiftButton.png", 115f);
            return buttonSprite;
        }

        public  void shiftRole (PlayerControl player1, PlayerControl player2, bool repeat = true) {
            if (Mayor.mayor != null && Mayor.mayor == player2) {
                if (repeat) shiftRole(player2, player1, false);
                Mayor.mayor = player1;
            } else if (Portalmaker.portalmaker != null && Portalmaker.portalmaker == player2) {
                if (repeat) shiftRole(player2, player1, false);
                Portalmaker.portalmaker = player1;
            } else if (Engineer.engineer != null && Engineer.engineer == player2) {
                if (repeat) shiftRole(player2, player1, false);
                Engineer.engineer = player1;
            } else if (Sheriff.sheriff != null && Sheriff.sheriff == player2) {
                if (repeat) shiftRole(player2, player1, false);
                if (Sheriff.formerDeputy != null && Sheriff.formerDeputy == Sheriff.sheriff) Sheriff.formerDeputy = player1;  // Shifter also shifts info on promoted deputy (to get handcuffs)
                Sheriff.sheriff = player1;
            } else if (Deputy.deputy != null && Deputy.deputy == player2) {
                if (repeat) shiftRole(player2, player1, false);
                Deputy.deputy = player1;
            } else if (Lighter.lighter != null && Lighter.lighter == player2) {
                if (repeat) shiftRole(player2, player1, false);
                Lighter.lighter = player1;
            } else if (Detective.detective != null && Detective.detective == player2) {
                if (repeat) shiftRole(player2, player1, false);
                Detective.detective = player1;
            } else if (TimeMaster.timeMaster != null && TimeMaster.timeMaster == player2) {
                if (repeat) shiftRole(player2, player1, false);
                TimeMaster.timeMaster = player1;
            }  else if (Medic.medic != null && Medic.medic == player2) {
                if (repeat) shiftRole(player2, player1, false);
                Medic.medic = player1;
            } else if (Swapper.swapper != null && Swapper.swapper == player2) {
                if (repeat) shiftRole(player2, player1, false);
                Swapper.swapper = player1;
            } else if (Seer.seer != null && Seer.seer == player2) {
                if (repeat) shiftRole(player2, player1, false);
                Seer.seer = player1;
            } else if (Hacker.hacker != null && Hacker.hacker == player2) {
                if (repeat) shiftRole(player2, player1, false);
                Hacker.hacker = player1;
            } else if (Tracker.tracker != null && Tracker.tracker == player2) {
                if (repeat) shiftRole(player2, player1, false);
                Tracker.tracker = player1;
            } else if (Snitch.snitch != null && Snitch.snitch == player2) {
                if (repeat) shiftRole(player2, player1, false);
                Snitch.snitch = player1;
            } else if (Spy.spy != null && Spy.spy == player2) {
                if (repeat) shiftRole(player2, player1, false);
                Spy.spy = player1;
            } else if (SecurityGuard.securityGuard != null && SecurityGuard.securityGuard == player2) {
                if (repeat) shiftRole(player2, player1, false);
                SecurityGuard.securityGuard = player1;
            } else if (Guesser.niceGuesser != null && Guesser.niceGuesser == player2) {
                if (repeat) shiftRole(player2, player1, false);
                Guesser.niceGuesser = player1;
            } else if (Medium.medium != null && Medium.medium == player2) {
                if (repeat) shiftRole(player2, player1, false);
                Medium.medium = player1;
            } else if (Pursuer.pursuer != null && Pursuer.pursuer == player2) {
                if (repeat) shiftRole(player2, player1, false);
                Pursuer.pursuer = player1;
            } //else if (Trapper.trapper != null && Trapper.trapper == player2) {
                //if (repeat) shiftRole(player2, player1, false);
                //Trapper.trapper = player1;
            //}
        }

        public override void clearAndReload() {
            shifter = null;
            currentTarget = null;
            futureShift = null;
        }
    }*/
}
