using password.settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace password.manager.winforms.Views.Themes
{

    public enum Themes
    {
        Light = 0,
        Dark = 1,
        Flamingo = 2,
        Orange = 3,
    }
    public class ThemeHelper
    {
        private const string themeSettingName = "Theme";
        public static void ApplyTheme(Visual visual, int themeIndex)
        {
            var theme = (Themes)themeIndex;

            switch (theme)
            {
                case Themes.Light:
                    ThemeProvider.SetTheme(visual, new LightTheme().ProvideTheme());
                    break;
                case Themes.Dark:
                    ThemeProvider.SetTheme(visual, new DarkTheme().ProvideTheme());
                    break;
                case Themes.Flamingo:
                    ThemeProvider.SetTheme(visual, new FlamingoTheme().ProvideTheme());
                    break;
                case Themes.Orange:
                    ThemeProvider.SetTheme(visual, new OrangeTheme().ProvideTheme());
                    break;
                default:
                    ThemeProvider.SetTheme(visual, new LightTheme().ProvideTheme());
                    break;
            }
        }

        public static List<string> GetThemeList()
        {
            return new List<string>
            {
                Themes.Light.ToString(),
                Themes.Dark.ToString(),
                Themes.Flamingo.ToString(),
                Themes.Orange.ToString()
            };
        }

        public static void SaveThemeSetting(int themeIndex)
        {
            Settings.SaveAppSetting(themeSettingName, themeIndex.ToString());
        }

        public static string GetThemeSetting()
        {
            return Settings.GetValueFromSettingKey(themeSettingName);
        }
    }
}
