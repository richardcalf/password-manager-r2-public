using password.settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace password.manager.winforms.Views.Themes
{

    public enum Themes
    {
        Light = 0,
        Dark = 1,
        Lavendar = 2,
        Oros = 3,
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
                case Themes.Lavendar:
                    ThemeProvider.SetTheme(visual, new FlamingoTheme().ProvideTheme());
                    break;
                case Themes.Oros:
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
                Themes.Lavendar.ToString(),
                Themes.Oros.ToString()
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

        public static void SetTextBox<T>(T textbox, Theme theme) where T : Control
        {
            textbox.BorderBrush = new SolidColorBrush(theme.TextBoxBorder);
            textbox.Background = new SolidColorBrush(theme.TextBoxBackGround);
            textbox.Foreground = new SolidColorBrush(theme.TextBoxForeGround);
        }

        public static void SetButton(Button button, Theme theme)
        {
            button.BorderBrush = new SolidColorBrush(theme.ButtonBorder);
            button.Background = new SolidColorBrush(theme.ButtonBackground);
            button.Foreground = new SolidColorBrush(theme.ButtonForeground);
        }

        public static void SetListBox(ListBox listbox, Theme theme)
        {
            listbox.BorderBrush = new SolidColorBrush(theme.ListBoxBorder);
            listbox.Background = new SolidColorBrush(theme.ListBoxBackground);
            listbox.Foreground = new SolidColorBrush(theme.ListBoxForeground);
        }

        public static void SetLabel(Label label, Theme theme)
        {
            label.Background = new SolidColorBrush(theme.LabelBackground);
            label.Foreground = new SolidColorBrush(theme.LabelForeground);
        }
    }
}
