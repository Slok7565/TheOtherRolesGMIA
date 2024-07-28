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
using TheOtherRoles.Roles.Neutral;

namespace TheOtherRoles.Roles.Crewmates;
public sealed class Prophet : RoleBase
{
    public PlayerControl prophet;
    public Color32 color = new Color32(255, 204, 127, byte.MaxValue);

    public float cooldown = 30f;
    public bool powerCrewAsRed = false;
    public bool neutralAsRed = true;
    public bool canCallEmergency = false;
    public int examineNum = 3;
    public int examinesToBeRevealed = 1;
    public int examinesLeft;
    public bool revealProphet = true;
    public bool isRevealed = false;
    public List<Arrow> arrows = new List<Arrow>();

    public Dictionary<PlayerControl, bool> examined = new Dictionary<PlayerControl, bool>();
    public PlayerControl currentTarget;

    private Sprite buttonSprite;
    public Sprite getButtonSprite()
    {
        if (buttonSprite) return buttonSprite;
        buttonSprite = ResourcesHelper.loadSpriteFromResources("TheOtherRoles.Resources.SeerButton.png", 115f);
        return buttonSprite;
    }

    public bool isKiller(PlayerControl p)
    {
        return Helpers.isKiller(p)
            || (p == Sheriff.sheriff || p == Deputy.deputy || p == Veteran.veteran || p == Mayor.mayor || p == Swapper.swapper || p == Guesser.niceGuesser || p == Yasuna.yasuna) && powerCrewAsRed || Helpers.isNeutral(p) && neutralAsRed;
    }

    public void clearAndReload()
    {
        prophet = null;
        currentTarget = null;
        isRevealed = false;
        examined = new Dictionary<PlayerControl, bool>();
        revealProphet = CustomOptionHolder.prophetIsRevealed.getBool();
        cooldown = CustomOptionHolder.prophetCooldown.getFloat();
        examineNum = Mathf.RoundToInt(CustomOptionHolder.prophetNumExamines.getFloat());
        powerCrewAsRed = CustomOptionHolder.prophetPowerCrewAsRed.getBool();
        neutralAsRed = CustomOptionHolder.prophetNeutralAsRed.getBool();
        canCallEmergency = CustomOptionHolder.prophetCanCallEmergency.getBool();
        examinesToBeRevealed = Math.Min(examineNum, Mathf.RoundToInt(CustomOptionHolder.prophetExaminesToBeRevealed.getFloat()));
        examinesLeft = examineNum;
        if (arrows != null)
            foreach (Arrow arrow in arrows)
                if (arrow?.arrow != null)
                    UnityEngine.Object.Destroy(arrow.arrow);
        arrows = new List<Arrow>();
    }
}
