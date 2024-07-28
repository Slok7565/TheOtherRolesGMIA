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
public sealed class Witch : RoleBase
{
    public PlayerControl witch;
    public Color color = Palette.ImpostorRed;

    public List<PlayerControl> futureSpelled = new List<PlayerControl>();
    public PlayerControl currentTarget;
    public PlayerControl spellCastingTarget;
    public float cooldown = 30f;
    public float spellCastingDuration = 2f;
    public float cooldownAddition = 10f;
    public float currentCooldownAddition = 0f;
    public bool canSpellAnyone = false;
    public bool triggerBothCooldowns = true;
    public bool witchVoteSavesTargets = true;

    private Sprite buttonSprite;
    public Sprite getButtonSprite()
    {
        if (buttonSprite) return buttonSprite;
        buttonSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.SpellButton.png", 115f);
        return buttonSprite;
    }

    private Sprite spelledOverlaySprite;
    public Sprite getSpelledOverlaySprite()
    {
        if (spelledOverlaySprite) return spelledOverlaySprite;
        spelledOverlaySprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.SpellButtonMeeting.png", 225f);
        return spelledOverlaySprite;
    }


    public override void clearAndReload()
    {
        witch = null;
        futureSpelled = new List<PlayerControl>();
        currentTarget = spellCastingTarget = null;
        cooldown = CustomOptionHolder.witchCooldown.getFloat();
        cooldownAddition = CustomOptionHolder.witchAdditionalCooldown.getFloat();
        currentCooldownAddition = 0f;
        canSpellAnyone = CustomOptionHolder.witchCanSpellAnyone.getBool();
        spellCastingDuration = CustomOptionHolder.witchSpellCastingDuration.getFloat();
        triggerBothCooldowns = CustomOptionHolder.witchTriggerBothCooldowns.getBool();
        witchVoteSavesTargets = CustomOptionHolder.witchVoteSavesTargets.getBool();
    }
}
