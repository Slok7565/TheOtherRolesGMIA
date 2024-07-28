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
using static TheOtherRoles.Roles.TheOtherRoles;
using TheOtherRoles.Patches;

namespace TheOtherRoles.Roles.Impostor;
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
        bomberButtonSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.PlantBombButton.png", 115f);
        return bomberButtonSprite;
    }
    public Sprite getReleaseButtonSprite()
    {
        if (releaseButtonSprite) return releaseButtonSprite;
        releaseButtonSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.ReleaseButton.png", 115f);
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
        updateTimer -= Time.fixedDeltaTime;

        if (updateTimer <= 0.0f)
        {

            foreach (Arrow arrow in arrows)
                if (arrow != null)
                {
                    arrow.arrow.SetActive(false);
                    UnityEngine.Object.Destroy(arrow.arrow);
                }

            arrows = new List<Arrow>();
            /*if (BomberB.bomberB == null || BomberB.bomberB.Data.IsDead) return;
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
