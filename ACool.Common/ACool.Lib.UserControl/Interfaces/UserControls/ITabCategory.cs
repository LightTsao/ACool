using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ACool.Definition.Interface.UserControls
{
    /// <summary>
    /// 用途1 用來產生Tab的依據
    /// </summary>
    public interface ITabCategory
    {
        string TabName { get; set; }
        Control TabContextControl { get; set; }
        Action<Control> ToolClick { get; set; }
    }
}
