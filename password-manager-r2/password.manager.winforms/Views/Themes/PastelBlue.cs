using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace password.manager.winforms.Views.Themes
{
    public class PastelBlue : Theme
    {
        public Theme ProvideTheme()
        {
            return new Theme
            {
                BackColor = Color.FromArgb(255, 44, 82, 232),

                TextBoxBorder = Color.FromArgb(255, 91, 117, 222),
                TextBoxBackGround = Color.FromArgb(255, 154, 169, 227),
                TextBoxForeGround = Color.FromArgb(255, 13, 16, 31),

                ButtonBorder = Color.FromArgb(255, 87, 111, 189),
                ButtonBackground = Color.FromArgb(255, 90, 140, 230),
                ButtonForeground = Color.FromArgb(255, 9, 9, 13),

                ListBoxBorder = Color.FromArgb(255, 87, 111, 189),
                ListBoxBackground = Color.FromArgb(255, 154, 169, 227),
                ListBoxForeground = Color.FromArgb(255, 13, 16, 31),

                LabelBackground = BackColor,
                LabelForeground = Color.FromArgb(255, 200, 200, 200)
            };
        }
    }
}
