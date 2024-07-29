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
public sealed class Morphling : RoleBase
{
    public PlayerControl morphling;
    public Color color = Palette.ImpostorRed;
    private Sprite sampleSprite;
    private Sprite morphSprite;

    public float cooldown = 30f;
    public float duration = 10f;

    public PlayerControl currentTarget;
    public PlayerControl sampledTarget;
    public PlayerControl morphTarget;
    public float morphTimer = 0f;

    public void resetMorph()
    {
        morphTarget = null;
        morphTimer = 0f;
        if (morphling == null) return;
        morphling.setDefaultOutFit();
    }

    public override void clearAndReload()
    {
        resetMorph();
        morphling = null;
        currentTarget = null;
        sampledTarget = null;
        morphTarget = null;
        morphTimer = 0f;
        cooldown = CustomOptionHolder.morphlingCooldown.getFloat();
        duration = CustomOptionHolder.morphlingDuration.getFloat();
    }

    public Sprite getSampleSprite()
    {
        if (sampleSprite) return sampleSprite;
        sampleSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.SampleButton.png", 115f);
        return sampleSprite;
    }

    public Sprite getMorphSprite()
    {
        if (morphSprite) return morphSprite;
        morphSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.MorphButton.png", 115f);
        return morphSprite;
    }
}
