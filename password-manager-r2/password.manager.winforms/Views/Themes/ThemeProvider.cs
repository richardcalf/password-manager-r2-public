using System.IO;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;

namespace password.manager.winforms
{

    public class ThemeProvider
    {
        private static void SetTextBox<T>(T textbox, Theme theme) where T : Control
        {
            textbox.BorderBrush = new SolidColorBrush(theme.TextBoxBorder);
            textbox.Background = new SolidColorBrush(theme.TextBoxBackGround);
            textbox.Foreground = new SolidColorBrush(theme.TextBoxForeGround);
        }
        public static void SetTheme(Visual visual, Theme theme)
        {
            if (visual is System.Windows.Controls.Grid)
            {
                var item = visual as Grid;
                item.Background = new SolidColorBrush(theme.BackColor);
            }
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(visual); i++)
            {
                Visual childVisual = (Visual)VisualTreeHelper.GetChild(visual, i);
                if (childVisual is System.Windows.Controls.TextBox)
                {
                    var textbox = childVisual as TextBox;
                    SetTextBox(textbox, theme);
                }
                if (childVisual is System.Windows.Controls.PasswordBox)
                {
                    var textbox = childVisual as PasswordBox;
                    SetTextBox(textbox, theme);
                }
                if (childVisual is System.Windows.Controls.Button)
                {
                    var button = childVisual as Button;
                    button.BorderBrush = new SolidColorBrush(theme.ButtonBorder);
                    button.Background = new SolidColorBrush(theme.ButtonBackground);
                    button.Foreground = new SolidColorBrush(theme.ButtonForeground);
                }
                if (childVisual is System.Windows.Controls.ListBox)
                {
                    var listbox = childVisual as ListBox;
                    listbox.BorderBrush = new SolidColorBrush(theme.ListBoxBorder);
                    listbox.Background = new SolidColorBrush(theme.ListBoxBackground);
                    listbox.Foreground = new SolidColorBrush(theme.ListBoxForeground);
                }
                if (childVisual is System.Windows.Controls.Label)
                {
                    var label = childVisual as Label;
                    label.Background = new SolidColorBrush(theme.LabelBackground);
                    label.Foreground = new SolidColorBrush(theme.LabelForeground);
                }
            }
        }
    }
}
