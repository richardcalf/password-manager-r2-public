using System.Windows.Controls;
using System.Windows.Media;

namespace password.manager.winforms
{

    public class ThemeProvider
    {
        public static void SetTheme(Visual visual, Theme theme)
        {
            Color backColor = theme.BackColor;
            SolidColorBrush borderColor = theme.BorderBrush;
            SolidColorBrush backGroundColor = theme.BackGroundBrush;
            SolidColorBrush foregroundColor = theme.ForeGroundBrush;
            SolidColorBrush labelForgroundColor = theme.LabelForeGroundBrush;
            if (visual is System.Windows.Controls.Grid)
            {
                var item = visual as Grid;
                item.Background = new SolidColorBrush(backColor);
            }
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(visual); i++)
            {
                Visual childVisual = (Visual)VisualTreeHelper.GetChild(visual, i);
                if (childVisual is System.Windows.Controls.TextBox)
                {
                    var textbox = childVisual as TextBox;
                    textbox.BorderBrush = borderColor;
                    textbox.Background = backGroundColor;
                    textbox.Foreground = foregroundColor;
                }
                if (childVisual is System.Windows.Controls.PasswordBox)
                {
                    var textbox = childVisual as PasswordBox;
                    textbox.BorderBrush = borderColor;
                    textbox.Background = backGroundColor;
                    textbox.Foreground = foregroundColor;
                }
                if (childVisual is System.Windows.Controls.Button)
                {
                    var button = childVisual as Button;
                    button.BorderBrush = borderColor;
                    button.Background = backGroundColor;
                    button.Foreground = foregroundColor;
                }
                if (childVisual is System.Windows.Controls.ListBox)
                {
                    var listbox = childVisual as ListBox;
                    listbox.BorderBrush = borderColor;
                    listbox.Background = backGroundColor;
                    listbox.Foreground = foregroundColor;
                }
                if (childVisual is System.Windows.Controls.Label)
                {
                    var label = childVisual as Label;
                    label.Background = new SolidColorBrush(backColor);
                    label.Foreground = labelForgroundColor;
                }
            }
        }
    }
}
