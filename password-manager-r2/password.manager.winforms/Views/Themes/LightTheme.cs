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
                ButtonBorder = Color.FromArgb(255, 255, 0, 0),
                TextBoxBorder = Color.FromArgb(255, 0, 255, 0),
                ButtonBackground = Color.FromArgb(120, 0, 0, 155),
                TextBoxBackGround = Color.FromArgb(255, 255, 255, 0),
                ControlForground = Color.FromArgb(255, 0, 0, 0),
                LabelForeGround = Color.FromArgb(255, 255, 0, 155),
            };
        }
    }
}
