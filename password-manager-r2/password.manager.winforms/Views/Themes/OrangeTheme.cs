using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace password.manager.winforms.Views.Themes
{
    class OrangeTheme : Theme
    {
        public Theme ProvideTheme()
        {
            return new Theme
            {
                BackColor = Color.FromArgb(255, 255, 165, 0)
            };
        }
    }
}
