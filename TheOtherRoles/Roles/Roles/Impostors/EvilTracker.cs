using System.Collections.Generic;
using TheOtherRoles.Helpers;
using TheOtherRoles.Roles.Neutral;
using UnityEngine;
using TheOtherRoles.Objects;
using TheOtherRoles.Players;
using TheOtherRoles.Roles.Core.Bases;
using TheOtherRoles.Roles.Crewmates;
using TheOtherRoles.Utilities;

namespace TheOtherRoles.Roles.Impostor;
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
        trackerButtonSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.TrackerButton.png", 115f);
        return trackerButtonSprite;
    }

    public Sprite getArrowSprite()
    {
        if (!arrowSprite)
            arrowSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.Arrow.png", 300f);
        return arrowSprite;
    }

    public void arrowUpdate()
    {
        updateTimer -= Time.fixedDeltaTime;

        if (updateTimer <= 0.0f)
        {

            foreach (Arrow arrow in arrows)
                if (arrow != null && arrow.arrow != null)
                {
                    arrow.arrow.SetActive(false);
                    UnityEngine.Object.Destroy(arrow.arrow);
                }

            arrows = new List<Arrow>();

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
