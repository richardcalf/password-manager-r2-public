using System.Windows.Media;

namespace password.manager.winforms
{
    public class DarkTheme : Theme
    {
        public Theme ProvideTheme()
        {
            return new Theme
            {
                BackColor = Color.FromArgb(255, 22, 24, 26),
                BorderBrush = Brushes.DarkSlateGray,
                BackGroundBrush = Brushes.DarkGray,
                ForeGroundBrush = Brushes.Black,
                LabelForeGroundBrush = Brushes.Gray
            };
        }
    }
}
