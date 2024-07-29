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

namespace TheOtherRoles.Roles.Crewmates;
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
        investigateIcon = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.SherlockInvestigate.png", 115f);
        return investigateIcon;
    }
    public Sprite getWatchIcon()
    {
        if (watchIcon) return watchIcon;
        watchIcon = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.SherlockWatch.png", 115f);
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
