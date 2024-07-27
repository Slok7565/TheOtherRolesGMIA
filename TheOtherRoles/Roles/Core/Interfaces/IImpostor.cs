using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmongUs.GameOptions;

namespace TheOtherRoles.Roles.Core.Interfaces;
/// <summary>
/// 内鬼的接口<br/>
/// <see cref="IKiller"/>的继承
/// </summary>
public interface IImpostor : IKiller, ISchrodingerCatOwner
{
    /// インポスターは基本サボタージュボタンを使える
    //bool IKiller.CanUseSabotageButton() => true;

    ///// <summary>
    ///// 是否可以成为绝境者
    ///// </summary>
    //public bool CanBeLastImpostor => true;
    /////// <summary>
    ///// 击杀猫猫的时候职业发生了变化<br/>
    ///// </summary>
    //SchrodingerCat.TeamType ISchrodingerCatOwner.SchrodingerCatChangeTo => SchrodingerCat.TeamType.Mad;

    ///// <summary>
    ///// 该位置的选项改为猫猫的设置<br/>
    ///// デフォルト<see cref="SchrodingerCat.ApplyMadCatOptions"/>
    ///// </summary>
    //void ISchrodingerCatOwner.ApplySchrodingerCatOptions(IGameOptions option)
    //{
    //    //SchrodingerCat.ApplyMadCatOptions(option);
    //}
}
