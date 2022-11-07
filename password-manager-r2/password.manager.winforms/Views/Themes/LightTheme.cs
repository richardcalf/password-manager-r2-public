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
                BorderBrush = Brushes.DarkSlateGray,
                BackGroundBrush = Brushes.White,
                ForeGroundBrush = Brushes.Black,
                LabelForeGroundBrush = Brushes.Black
            };
        }
    }
}
