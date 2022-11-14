using System.Windows.Media;

namespace password.manager.winforms
{
    public class FlamingoTheme : Theme 
    {
        public Theme ProvideTheme()
        {
            return new Theme
            {
                BackColor = Color.FromArgb(255, 209, 90, 224),

                TextBoxBorder = Color.FromArgb(255, 203, 145, 219),
                TextBoxBackGround = Color.FromArgb(255, 222, 181, 235),
                TextBoxForeGround = Color.FromArgb(255, 108, 37, 128),

                ButtonBorder = Color.FromArgb(255, 182, 38, 222),
                ButtonBackground = Color.FromArgb(255, 205, 142, 222),
                ButtonForeground = Color.FromArgb(255, 108, 37, 128),

                ListBoxBorder = Color.FromArgb(255, 171, 173, 179),
                ListBoxBackground = Color.FromArgb(255, 222, 181, 235),
                ListBoxForeground = Color.FromArgb(255, 108, 37, 128),

                LabelBackground = BackColor,
                LabelForeground = Color.FromArgb(255, 108, 37, 128)
            };
        }
    }
}
