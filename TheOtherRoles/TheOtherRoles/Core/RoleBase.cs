using AmongUs.GameOptions;
using Hazel;
using Il2CppSystem.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using TheOtherRoles.Modules;
using UnityEngine;
using static TheOtherRoles.ModTranslation;

namespace TheOtherRoles.TheOtherRoles.Core;

public abstract class RoleBase : IDisposable
{
    #region 玩家相关处理
    /// <summary>
    /// 玩家本体
    /// </summary>
    public PlayerControl Player { get; set; }

    protected Func<HasTask> hasTasks;
    /// <summary>
    /// 是否拥有任务
    /// </summary>
    public HasTask HasTasks => hasTasks.Invoke();


    public RoleBase(
        RoleInfo roleInfo,

        PlayerControl player,
        Func<HasTask> hasTasks = null

    )
    {
        this.hasTasks = hasTasks ?? (roleInfo.types == CustomOption.CustomOptionType.Crewmate ? () => HasTask.True : () => HasTask.False);
        Player = player;
        CustomRoleManager.AllActiveRoles.Add(Player.PlayerId, this);

    }
#pragma warning disable CA1816
    public void Dispose()
    {
        OnDestroy();
        CustomRoleManager.AllActiveRoles.Remove(Player.PlayerId);
        Player = null;
    }
#pragma warning restore CA1816
    public bool Is(PlayerControl player)
    {
        return player.PlayerId == Player.PlayerId;
    }
    public virtual void clearAndReload()
    { }
    /// <summary>
    /// 创建实例后立刻调用的函数
    /// </summary>
    public virtual void Add()
    { }
    /// <summary>
    /// 实例被销毁时调用的函数
    /// </summary>
    public virtual void OnDestroy()
    { }
    #endregion
    #region RPC相关处理
    /// <summary>
    /// RoleBase 专用 RPC 发送类
    /// 会自动发送 PlayerId
    /// </summary>
    public class RoleRPCSender : IDisposable
    {
        public MessageWriter Writer;
        public RoleRPCSender(RoleBase role)
        {
            Writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.RoleIdSync, SendOption.Reliable, -1);
            Writer.Write(role.Player.PlayerId);
        }
        public void Dispose()
        {
            AmongUsClient.Instance.FinishRpcImmediately(Writer);
        }
    }
    /// <summary>
    /// 创建一个待发送的 RPC
    /// PlayerId 是自动添加的，所以您可以忽略
    /// </summary>
    /// <param name="rpcType">发送的 RPC 类型</param>
    /// <returns>用于发送的 RoleRPCSender</returns>
    protected RoleRPCSender CreateSender()
    {
        return new RoleRPCSender(this);
    }
    /// <summary>
    /// 接受到 RPC 时的函数
    /// 任何职业收到任何类型的 RPC 时都会调用，所以您需要判断是否是您需要的 RPC 类型
    /// RoleRPCSender 中的 PlayerId 在传递前会被删除，所以您不需要知道它
    /// </summary>
    /// <param name="reader">收到 RPC 内容</param>
    /// <param name="rpcType">收到 RPC 类型</param>
    public virtual void ReceiveRPC(MessageReader reader)
    { }
    #endregion
    /// <summary>
    /// 是否允许钻管
    /// </summary>
    public virtual bool CanVent() => Player?.Data?.Role?.CanVent ?? false;
    /// <summary>
    /// 是否允许在管道移动
    /// </summary>
    public virtual bool CanMoveInVents() =>  false;
    /// <summary>
    /// 是否用于内鬼视野
    /// </summary>
    public virtual bool HasImpVision() => Player?.Data?.Role?.IsImpostor ?? false;
    /// <summary>
    /// 驱逐后的处理
    /// </summary>
    public virtual Action OnWrapUp(GameData.PlayerInfo exiled, ref bool DecideWinner) => null;
    /// <summary>
    /// 覆盖驱逐文本
    /// </summary>
    public virtual string OverrideExileText(PlayerControl player, StringNames id) => default;
    /// <summary>
    /// 是否可以召开会议
    /// </summary>
    public virtual bool CanUseMeetingButton() => true;
    /// <summary>
    /// 覆盖会议界面文本
    /// </summary>
    public virtual string OverrideMeetingBtnText() => default;
    /// <summary>
    /// 创建按钮
    /// </summary>
    public virtual void CreateButton(HudManager __instance) {}

    public virtual void setCustomButtonCooldowns() { }
    protected enum GeneralOption
    {
        Cooldown,
        KillCooldown,
        CanVent,
        CanKill,
        VentCooldown,
        ImpostorVision,
        CanUseSabotage,
        CanCreateMadmate,
        CanKillAllies,
        CanKillSelf,
        ShapeshiftDuration,
        ShapeshiftCooldown,
        SkillDuration,
        SkillCooldown,
        SkillLimit,
    }
}
