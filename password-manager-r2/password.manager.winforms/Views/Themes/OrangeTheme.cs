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
                BackColor = Color.FromArgb(255, 255, 135, 15),

                TextBoxBorder = Color.FromArgb(255, 222, 190, 131),
                TextBoxBackGround = Color.FromArgb(255, 237, 205, 145),
                TextBoxForeGround = Color.FromArgb(255, 54, 52, 48),

                ButtonBorder = Color.FromArgb(255, 240, 122, 38),
                ButtonBackground = Color.FromArgb(255, 245, 167, 22),
                ButtonForeground = Color.FromArgb(255, 54, 52, 48),

                ListBoxBorder = Color.FromArgb(255, 171, 173, 179),
                ListBoxBackground = Color.FromArgb(255, 237, 205, 145),
                ListBoxForeground = Color.FromArgb(255, 54, 52, 48),

                LabelBackground = BackColor,
                LabelForeground = Color.FromArgb(255, 54, 52, 48)
            };
        }
    }
}
