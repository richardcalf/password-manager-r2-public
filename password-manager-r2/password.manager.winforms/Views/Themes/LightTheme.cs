using System.Windows.Controls;
using System.Windows.Media;

namespace password.manager.winforms
{
    public class LightTheme : Theme
    {
        public Theme ProvideTheme()
        {
            return new Theme
            {
                BackColor = Color.FromArgb(255, 255, 255, 255),

                TextBoxBorder = Color.FromArgb(255, 171, 173, 179),
                TextBoxBackGround = Color.FromArgb(255, 255, 255, 255),
                TextBoxForeGround = Color.FromArgb(255,0, 0, 0),

                ButtonBorder = Color.FromArgb(255, 112, 112, 112),
                ButtonBackground = Color.FromArgb(255, 221, 221, 221),
                ButtonForeground = Color.FromArgb(255, 0, 0, 0),

                ListBoxBorder = Color.FromArgb(255, 171, 173, 179),
                ListBoxBackground  = Color.FromArgb(255, 255, 255, 255),
                ListBoxForeground = Color.FromArgb(255, 0, 0, 0),

                LabelBackground = Color.FromArgb(255, 255, 255, 255),
                LabelForeground = Color.FromArgb(255, 0, 0, 0),
            };
        }
    }
}
