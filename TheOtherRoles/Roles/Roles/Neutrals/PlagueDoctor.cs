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

namespace TheOtherRoles.Roles.Neutral;
public sealed class PlagueDoctor : RoleBase
{
    public PlayerControl plagueDoctor;
    public Color color = new Color32(255, 192, 0, byte.MaxValue);

    public Dictionary<int, PlayerControl> infected;
    public Dictionary<int, float> progress;
    public Dictionary<int, bool> dead;
    public TMPro.TMP_Text statusText = null;
    public bool triggerPlagueDoctorWin = false;

    public PlayerControl currentTarget;
    public int numInfections = 0;
    public bool meetingFlag = false;

    public float infectCooldown = 10f;
    public int maxInfectable;
    public float infectDistance = 1f;
    public float infectDuration = 5f;
    public float immunityTime = 10f;
    public bool infectKiller = true;
    public bool canWinDead = true;

    public Sprite plagueDoctorIcon;

    public Sprite getSyringeIcon()
    {
        if (plagueDoctorIcon) return plagueDoctorIcon;
        plagueDoctorIcon = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.InfectButton.png", 115f);
        return plagueDoctorIcon;
    }

    public void updateDead()
    {
        foreach (var pc in PlayerControl.AllPlayerControls.GetFastEnumerator())
            dead[pc.PlayerId] = pc.Data.IsDead;
    }

    public bool hasInfected()
    {
        bool flag = false;
        foreach (var item in progress)
            if (item.Value != 0f)
            {
                flag = true;
                break;
            }
        return flag;
    }

    public string getProgressString(float progress)
    {
        // Go from green -> yellow -> red based on infection progress
        Color color;
        var prog = progress / infectDuration;
        if (prog < 0.5f)
            color = Color.Lerp(Color.green, Color.yellow, prog * 2);
        else
            color = Color.Lerp(Color.yellow, Color.red, prog * 2 - 1);

        float progPercent = prog * 100;
        return OtherHelper.cs(color, $"{progPercent.ToString("F1")}%");
    }

    public void checkWinStatus()
    {
        bool winFlag = true;
        foreach (PlayerControl p in CachedPlayer.AllPlayers)
        {
            if (p.Data.IsDead) continue;
            if (p == plagueDoctor) continue;
            if (!infected.ContainsKey(p.PlayerId))
            {
                winFlag = false;
                break;
            }
        }

        if (winFlag)
        {
            MessageWriter winWriter = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.PlagueDoctorWin, SendOption.Reliable, -1);
            AmongUsClient.Instance.FinishRpcImmediately(winWriter);
            RPCProcedure.plagueDoctorWin();
        }
    }

    public void clearAndReload()
    {
        plagueDoctor = null;
        infectCooldown = CustomOptionHolder.plagueDoctorInfectCooldown.getFloat();
        maxInfectable = Mathf.RoundToInt(CustomOptionHolder.plagueDoctorNumInfections.getFloat());
        infectDistance = CustomOptionHolder.plagueDoctorDistance.getFloat();
        infectDuration = CustomOptionHolder.plagueDoctorDuration.getFloat();
        immunityTime = CustomOptionHolder.plagueDoctorImmunityTime.getFloat();
        infectKiller = CustomOptionHolder.plagueDoctorInfectKiller.getBool();
        canWinDead = CustomOptionHolder.plagueDoctorWinDead.getBool();
        meetingFlag = false;
        triggerPlagueDoctorWin = false;
        numInfections = maxInfectable;
        currentTarget = null;
        infected = new Dictionary<int, PlayerControl>();
        progress = new Dictionary<int, float>();
        dead = new Dictionary<int, bool>();
        if (statusText != null) UnityEngine.Object.Destroy(statusText);
        statusText = null;
    }
}
