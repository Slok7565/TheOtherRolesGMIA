using System.Collections.Generic;
using TheOtherRoles.Helpers;
using UnityEngine;
using TheOtherRoles.Objects;
using TheOtherRoles.Roles.Core.Bases;

namespace TheOtherRoles.Roles.Neutral;
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
        brainwashIcon = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.BrainwashButton.png", 115f);
        return brainwashIcon;
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
