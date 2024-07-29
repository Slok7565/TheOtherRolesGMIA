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
using static TheOtherRoles.Roles.TheOtherRoles;
using TheOtherRoles.Roles.Impostor;
using TheOtherRoles.Roles.Modifier;
using TheOtherRoles.Roles.Neutral;

namespace TheOtherRoles.Roles.Crewmates;
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
        soulSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.Soul.png", 500f);
        return soulSprite;
    }

    private Sprite question;
    public Sprite getQuestionSprite()
    {
        if (question) return question;
        question = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.MediumButton.png", 115f);
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
