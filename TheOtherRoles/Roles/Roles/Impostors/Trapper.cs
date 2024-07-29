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

namespace TheOtherRoles.Roles.Impostor;
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
        trapButtonSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.TrapperButton.png", 115f);
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
