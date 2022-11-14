using System.Windows.Media;

namespace password.manager.winforms
{
    public class DarkTheme : Theme
    {
        public Theme ProvideTheme()
        {
            return new Theme
            {
                BackColor = Color.FromArgb(255, 35, 35, 38),

                TextBoxBorder = Color.FromArgb(255, 0, 0, 0),
                TextBoxBackGround = Color.FromArgb(255, 134, 135, 134),
                TextBoxForeGround = Color.FromArgb(255, 0, 0, 0),

                ButtonBorder = Color.FromArgb(255, 119, 120, 119),
                ButtonBackground = Color.FromArgb(255, 84, 84, 83),
                ButtonForeground = Color.FromArgb(255, 232, 232, 232),

                ListBoxBorder = Color.FromArgb(255, 171, 173, 179),
                ListBoxBackground = Color.FromArgb(255, 134, 135, 134),
                ListBoxForeground = Color.FromArgb(255, 0, 0, 0),

                LabelBackground = BackColor,
                LabelForeground = Color.FromArgb(255, 232, 232, 232),
            };
        }
    }
}
