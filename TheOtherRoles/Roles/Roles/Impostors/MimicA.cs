using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheOtherRoles.Roles.Core;
using TheOtherRoles.Helpers;
using TheOtherRoles.Roles.Core.Interfaces;
using TheOtherRoles.Roles.Neutral;
using UnityEngine;
using static TheOtherRoles.CustomOption;
using TheOtherRoles.Objects;
using Hazel;
using TheOtherRoles.Players;
using System.Xml.Serialization;
using TheOtherRoles.Roles.Core.Bases;
using TheOtherRoles.Utilities;

namespace TheOtherRoles.Roles.Impostor;
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
        morphButtonSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.MorphButton.png", 115f);
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

        updateTimer -= Time.fixedDeltaTime;

        if (updateTimer <= 0.0f)
        {

            foreach (Arrow arrow1 in arrows)
                if (arrow1 != null && arrow1.arrow != null)
                {
                    arrow1.arrow.SetActive(false);
                    UnityEngine.Object.Destroy(arrow1.arrow);
                }

            //if (MimicA.mimicA == null) return;

            arrows = new List<Arrow>();
            if (MimicK.mimicK.Data.IsDead || MimicK.mimicK == null) return;
            Arrow arrow = new Arrow(Palette.ImpostorRed);
            arrow.arrow.SetActive(true);
            arrow.Update(MimicK.mimicK.transform.position);
            arrows.Add(arrow);

            updateTimer = arrowUpdateInterval;
        }
    }

    public void clearAndReload()
    {
        mimicA?.setDefaultOutFit();
        mimicA = null;
        isMorph = false;
        if (arrows != null)
            foreach (Arrow arrow in arrows)
                if (arrow?.arrow != null)
                    UnityEngine.Object.Destroy(arrow.arrow);
        arrows = new List<Arrow>();
    }
}
