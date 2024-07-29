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
public sealed class Trickster : RoleBase
{
    public PlayerControl trickster;
    public Color color = Palette.ImpostorRed;
    public float placeBoxCooldown = 30f;
    public float lightsOutCooldown = 30f;
    public float lightsOutDuration = 10f;
    public float lightsOutTimer = 0f;

    private Sprite placeBoxButtonSprite;
    private Sprite lightOutButtonSprite;
    private Sprite tricksterVentButtonSprite;

    public Sprite getPlaceBoxButtonSprite()
    {
        if (placeBoxButtonSprite) return placeBoxButtonSprite;
        placeBoxButtonSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.PlaceJackInTheBoxButton.png", 115f);
        return placeBoxButtonSprite;
    }

    public Sprite getLightsOutButtonSprite()
    {
        if (lightOutButtonSprite) return lightOutButtonSprite;
        lightOutButtonSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.LightsOutButton.png", 115f);
        return lightOutButtonSprite;
    }

    public Sprite getTricksterVentButtonSprite()
    {
        if (tricksterVentButtonSprite) return tricksterVentButtonSprite;
        tricksterVentButtonSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.TricksterVentButton.png", 115f);
        return tricksterVentButtonSprite;
    }

    public override void clearAndReload()
    {
        trickster = null;
        lightsOutTimer = 0f;
        placeBoxCooldown = CustomOptionHolder.tricksterPlaceBoxCooldown.getFloat();
        lightsOutCooldown = CustomOptionHolder.tricksterLightsOutCooldown.getFloat();
        lightsOutDuration = CustomOptionHolder.tricksterLightsOutDuration.getFloat();
        JackInTheBox.UpdateStates(); // if the role is erased, we might have to update the state of the created objects
    }

}
