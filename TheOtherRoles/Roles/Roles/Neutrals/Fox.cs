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
using HarmonyLib;
using static TheOtherRoles.Roles.TheOtherRoles;
using TheOtherRoles.Roles.Crewmates;

namespace TheOtherRoles.Roles.Neutral;
public sealed class Fox : RoleBase
{
    public PlayerControl fox;
    public Color color = new Color32(167, 87, 168, byte.MaxValue);

    public enum TaskType
    {
        Serial,
        Parallel
    }

    public List<Arrow> arrows = new();
    public float updateTimer = 0f;
    public float arrowUpdateInterval = 0.5f;
    public bool crewWinsByTasks = false;
    public bool impostorWinsBySabotage = true;
    public float stealthCooldown;
    public float stealthDuration;
    public int numTasks;
    public float stayTime;

    public bool stealthed = false;
    public DateTime stealthedAt = DateTime.UtcNow;
    public float fadeTime = 1f;

    public int numRepair = 0;
    public bool canCreateImmoralist;
    public PlayerControl currentTarget;
    public TaskType taskType;

    private Sprite hideButtonSprite;
    private Sprite repairButtonSprite;
    private Sprite immoralistButtonSprite;

    public Sprite getHideButtonSprite()
    {
        if (hideButtonSprite) return hideButtonSprite;
        hideButtonSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.FoxHideButton.png", 115f);
        return hideButtonSprite;
    }

    public Sprite getRepairButtonSprite()
    {
        if (repairButtonSprite) return repairButtonSprite;
        repairButtonSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.RepairButton.png", 115f);
        return repairButtonSprite;
    }

    public Sprite getImmoralistButtonSprite()
    {
        if (immoralistButtonSprite) return immoralistButtonSprite;
        immoralistButtonSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.FoxImmoralistButton.png", 115f);
        return immoralistButtonSprite;
    }

    public float stealthFade()
    {
        if (fox != null && !fox.Data.IsDead)
            return Mathf.Min(1.0f, (float)(DateTime.UtcNow - stealthedAt).TotalSeconds / fadeTime);
        return 1.0f;
    }

    public void setStealthed(bool stealthed = true)
    {
        Fox.stealthed = stealthed;
        stealthedAt = DateTime.UtcNow;
    }

    public void setOpacity(PlayerControl player, float opacity)
    {
        var color = Color.Lerp(Palette.ClearWhite, Palette.White, opacity);
        try
        {
            if (Chameleon.chameleon.Any(x => x.PlayerId == player.PlayerId) && Chameleon.visibility(player.PlayerId) < 1f && !stealthed) return;
            ExtendPlayerControl.setInvisible(player, color);
        }
        catch { }
    }

    public bool tasksComplete()
    {
        if (fox == null) return false;
        if (fox.Data.IsDead) return false;
        int counter = 0;
        int totalTasks = 1;
        foreach (var task in fox.Data.Tasks)
            if (task.Complete)
                counter++;
        return counter == totalTasks;
    }

    public void arrowUpdate()
    {
        updateTimer -= Time.fixedDeltaTime;

        if (updateTimer <= 0.0f)
        {

            foreach (Arrow arrow in arrows)
                if (arrow?.arrow != null)
                {
                    arrow.arrow.SetActive(false);
                    UnityEngine.Object.Destroy(arrow.arrow);
                }

            arrows = new List<Arrow>();

            foreach (PlayerControl p in CachedPlayer.AllPlayers)
            {
                if (p.Data.IsDead) continue;
                Arrow arrow;
                // float distance = Vector2.Distance(p.transform.position, PlayerControl.LocalPlayer.transform.position);
                if (p.Data.Role.IsImpostor || p == Jackal.jackal || p == Sheriff.sheriff || p == JekyllAndHyde.jekyllAndHyde || p == Moriarty.moriarty || p == Thief.thief)
                {
                    if (p.Data.Role.IsImpostor)
                        arrow = new Arrow(Palette.ImpostorRed);
                    else if (p == Jackal.jackal)
                        arrow = new Arrow(Jackal.color);
                    else if (p == Sheriff.sheriff)
                        arrow = new Arrow(Palette.White);
                    else if (p == JekyllAndHyde.jekyllAndHyde)
                        arrow = new Arrow(JekyllAndHyde.color);
                    else if (p == Moriarty.moriarty)
                        arrow = new Arrow(Moriarty.color);
                    else
                        arrow = new Arrow(Thief.color);
                    arrow.arrow.SetActive(true);
                    arrow.Update(p.transform.position);
                    arrows.Add(arrow);
                }
            }

            updateTimer = arrowUpdateInterval;
        }
        else
            arrows.Do(x => x.Update());
    }

    public void clearAndReload()
    {
        setOpacity(fox, 1.0f);
        fox = null;
        currentTarget = null;
        stealthed = false;
        stealthedAt = DateTime.UtcNow;
        crewWinsByTasks = CustomOptionHolder.foxCrewWinsByTasks.getBool();
        impostorWinsBySabotage = CustomOptionHolder.foxImpostorWinsBySabotage.getBool();
        stealthCooldown = CustomOptionHolder.foxStealthCooldown.getFloat();
        stealthDuration = CustomOptionHolder.foxStealthDuration.getFloat();
        canCreateImmoralist = CustomOptionHolder.foxCanCreateImmoralist.getBool();
        numTasks = (int)CustomOptionHolder.foxNumTasks.getFloat();
        numRepair = (int)CustomOptionHolder.foxNumRepairs.getFloat();
        stayTime = (int)CustomOptionHolder.foxStayTime.getFloat();
        taskType = (TaskType)CustomOptionHolder.foxTaskType.getSelection();
        foreach (Arrow arrow in arrows)
            if (arrow?.arrow != null)
            {
                arrow.arrow.SetActive(false);
                UnityEngine.Object.Destroy(arrow.arrow);
            }
        arrows = new List<Arrow>();
        Immoralist.clearAndReload();
    }

    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
    public sealed class PlayerPhysicsFoxPatch : RoleBase
    {
        public void Postfix(PlayerPhysics __instance)
        {
            if (fox != null && fox == __instance.myPlayer)
            {
                var fox = __instance.myPlayer;
                if (fox == null || fox.Data.IsDead) return;

                bool canSee =
                    CachedPlayer.LocalPlayer.PlayerControl == fox ||
                    CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead ||
                    CachedPlayer.LocalPlayer.PlayerControl == Lighter.lighter && Lighter.canSeeInvisible ||
                    CachedPlayer.LocalPlayer.PlayerControl == Immoralist.immoralist;

                var opacity = canSee ? 0.1f : 0.0f;

                if (stealthed)
                {
                    opacity = Math.Max(opacity, 1.0f - stealthFade());
                    fox.cosmetics?.currentBodySprite?.BodySprite.material.SetFloat("_Outline", 0f);
                }
                else
                    opacity = Math.Max(opacity, stealthFade());

                setOpacity(fox, opacity);
            }
        }
    }
}
