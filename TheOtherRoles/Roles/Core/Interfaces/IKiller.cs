using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TheOtherRoles.TheOtherRoles.Core.Interfaces;
/// <summary>
/// 带有击杀按钮的职业的接口
/// </summary>
public interface IKiller
{
    public void OnCheckMurderAsKiller() { }
    public void OnCheckMurderAsTarget() { }
    public void OnMurderPlayerAsKiller(PlayerControl target) { }
    public void OnMurderPlayerAsTarget() { }


}

