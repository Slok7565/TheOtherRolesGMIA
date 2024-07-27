using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheOtherRoles.Roles.Core.Interfaces;
/// <summary>
/// 中立职业的接口
/// </summary>
public interface INeutral
{
    /// <summary>
    /// 独自胜利
    /// </summary>
    public bool WinAlone => true;

    

}

/// <summary>
/// 带有击杀按钮的中立职业的接口
/// </summary>
public interface INeutralKiller : INeutral, IKiller, ISchrodingerCatOwner
{
    //SchrodingerCat.TeamType ISchrodingerCatOwner.SchrodingerCatChangeTo => SchrodingerCat.TeamType.None;
}
