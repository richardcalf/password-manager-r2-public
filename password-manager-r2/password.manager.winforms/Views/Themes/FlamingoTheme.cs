using System.Windows.Media;

namespace password.manager.winforms
{
    public class FlamingoTheme : Theme 
    {
        public Theme ProvideTheme()
        {
            return new Theme
            {
                BackColor = Color.FromArgb(255, 224, 43, 115)
            };
        }
    }
}
