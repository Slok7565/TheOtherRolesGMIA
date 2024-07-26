using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheOtherRoles.Players;
using TheOtherRoles.Role;
using TheOtherRoles.Utilities;
using UnityEngine;

namespace TheOtherRoles.Helpers;

public static class DeadBodyHelper
{
    public static void SetDeadBodyOutline(DeadBody target, Color? color)
    {
        if (target == null || target.bodyRenderers[0] == null) return;
        target.bodyRenderers[0].material.SetFloat("_Outline", color == null ? 0f : 1f);
        if (color != null) target.bodyRenderers[0].material.SetColor("_OutlineColor", color.Value);
    }


    public static DeadBody setDeadTarget(float maxDistance = 0f, PlayerControl targetingPlayer = null)
    {
        DeadBody result = null;
        float closestDistance = float.MaxValue;

        if (!MapUtilities.CachedShipStatus) return null;

        if (targetingPlayer == null) targetingPlayer = CachedPlayer.LocalPlayer.PlayerControl;
        if (targetingPlayer.Data.IsDead) return null;

        maxDistance = maxDistance == 0f ? 1f : maxDistance + 0.1f;

        Vector2 truePosition = targetingPlayer.GetTruePosition() - new Vector2(-0.2f, -0.22f);

        bool flag = GameOptionsManager.Instance.currentNormalGameOptions.GhostsDoTasks
                    && (!AmongUsClient.Instance || !AmongUsClient.Instance.IsGameOver);

        Collider2D[] allocs = Physics2D.OverlapCircleAll(truePosition, maxDistance,
            LayerMask.GetMask("Players", "Ghost"));


        foreach (Collider2D collider2D in allocs)
        {
            if (!flag || collider2D.tag != "DeadBody") continue;
            DeadBody component = collider2D.GetComponent<DeadBody>();

            if (!(Vector2.Distance(truePosition, component.TruePosition) <=
                  maxDistance)) continue;

            float distance = Vector2.Distance(truePosition, component.TruePosition);

            if (!(distance < closestDistance)) continue;

            result = component;
            closestDistance = distance;
        }

        if (result && Undertaker.undertaker == targetingPlayer)
            SetDeadBodyOutline(result, Undertaker.color);

        return result;
    }

    public static void HandleUndertakerDropOnBodyReport()
    {
        if (Undertaker.undertaker == null) return;
        var position = Undertaker.DraggedBody != null
            ? Undertaker.DraggedBody.transform.position
            : Vector3.zero;
        Undertaker.DropBody(position);
    }

}
