using System.Windows.Controls;
using System.Windows.Media;

namespace password.manager.winforms
{

    public class ThemeProvider
    {
        public static void SetTheme(Visual visual, Theme theme)
        {
            SolidColorBrush mainBackground = new SolidColorBrush(theme.BackColor);
            SolidColorBrush buttonBorder = new SolidColorBrush(theme.ButtonBorder);
            SolidColorBrush textBoxBorder = new SolidColorBrush(theme.TextBoxBorder);
            SolidColorBrush buttonBackground = new SolidColorBrush(theme.ButtonBackground);
            SolidColorBrush textBoxBackground = new SolidColorBrush(theme.TextBoxBackGround);
            SolidColorBrush foregroundColor = new SolidColorBrush(theme.ControlForground);
            SolidColorBrush labelForgroundColor = new SolidColorBrush(theme.LabelForeGround);
            if (visual is System.Windows.Controls.Grid)
            {
                var item = visual as Grid;
                item.Background = mainBackground;
            }
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(visual); i++)
            {
                Visual childVisual = (Visual)VisualTreeHelper.GetChild(visual, i);
                if (childVisual is System.Windows.Controls.TextBox)
                {
                    var textbox = childVisual as TextBox;
                    textbox.BorderBrush = textBoxBorder;
                    textbox.Background = textBoxBackground;
                    textbox.Foreground = foregroundColor;
                }
                if (childVisual is System.Windows.Controls.PasswordBox)
                {
                    var textbox = childVisual as PasswordBox;
                    textbox.BorderBrush = textBoxBorder;
                    textbox.Background = textBoxBackground;
                    textbox.Foreground = foregroundColor;
                }
                if (childVisual is System.Windows.Controls.Button)
                {
                    var button = childVisual as Button;
                    button.BorderBrush = buttonBorder;
                    button.Background = buttonBackground;
                    button.Foreground = foregroundColor;
                }
                if (childVisual is System.Windows.Controls.ListBox)
                {
                    var listbox = childVisual as ListBox;
                    listbox.BorderBrush = textBoxBorder;
                    listbox.Background = textBoxBackground;
                    listbox.Foreground = foregroundColor;
                }
                if (childVisual is System.Windows.Controls.Label)
                {
                    var label = childVisual as Label;
                    label.Background = mainBackground;
                    label.Foreground = labelForgroundColor;
                }
            }
        }
    }
}
