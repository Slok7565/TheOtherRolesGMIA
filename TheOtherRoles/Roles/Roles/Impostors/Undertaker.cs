using System.Linq;
using TheOtherRoles.Helpers;
using UnityEngine;
using Hazel;
using TheOtherRoles.Players;
using TheOtherRoles.Roles.Core.Bases;

namespace TheOtherRoles.Roles.Impostor;
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
        dragButtonSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.DragButton.png", 115f);
        return dragButtonSprite;
    }

    public Sprite getDropButtonSprite()
    {
        if (dropButtonSprite) return dropButtonSprite;
        dropButtonSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.DropButton.png", 115f);
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
