using System.Linq;
using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using TheOtherRoles.Objects;
using TheOtherRoles.Players;
using TheOtherRoles.Utilities;
using TheOtherRoles.CustomGameModes;
using AmongUs.Data;
using Hazel;
using JetBrains.Annotations;
using Steamworks;
using TheOtherRoles.Patches;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;
using Reactor.Utilities.Extensions;
using Types = TheOtherRoles.CustomOption.CustomOptionType;
using TheOtherRoles.Roles.Core;
using TheOtherRoles.Helpers;
using TheOtherRoles.Roles.Core.Bases;

namespace TheOtherRoles.Roles
{
    [HarmonyPatch]
    public static class TheOtherRoles
    {
        public static System.Random rnd = new((int)DateTime.Now.Ticks);

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
            trapButtonSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.Trapper_Place_Button.png", 115f);
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
                buttonSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.Bomb_Button_Plant.png", 115f);
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


    }
}
