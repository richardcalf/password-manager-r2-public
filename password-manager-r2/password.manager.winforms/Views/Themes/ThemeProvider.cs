using password.manager.winforms.Views.Themes;
using System.IO;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;

namespace password.manager.winforms
{

    public class ThemeProvider
    {         
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
                if (childVisual is System.Windows.Controls.Canvas)
                {
                    for (int x = 0; x < VisualTreeHelper.GetChildrenCount(childVisual); x++)
                    {
                        Visual childChildVisual = (Visual)VisualTreeHelper.GetChild(childVisual, x);
                        if (childChildVisual is System.Windows.Controls.TextBox)
                        {
                            ThemeHelper.SetTextBox(childChildVisual as TextBox, theme);
                        }
                        if (childChildVisual is System.Windows.Controls.Button)
                        {
                            ThemeHelper.SetButton(childChildVisual as Button, theme);
                        }
                        if (childChildVisual is System.Windows.Controls.ListBox)
                        {
                            ThemeHelper.SetListBox(childChildVisual as ListBox, theme);
                        }
                        if (childChildVisual is System.Windows.Controls.Label)
                        {
                            ThemeHelper.SetLabel(childChildVisual as Label, theme);
                        }
                    }
                }
                if (childVisual is System.Windows.Controls.TextBox)
                {
                    ThemeHelper.SetTextBox(childVisual as TextBox, theme);
                }
                if (childVisual is System.Windows.Controls.PasswordBox)
                {
                    ThemeHelper.SetTextBox(childVisual as PasswordBox, theme);
                }
                if (childVisual is System.Windows.Controls.Button)
                {
                    ThemeHelper.SetButton(childVisual as Button, theme);
                }
                if (childVisual is System.Windows.Controls.ListBox)
                {
                    ThemeHelper.SetListBox(childVisual as ListBox, theme);
                }
                if (childVisual is System.Windows.Controls.Label)
                {
                    ThemeHelper.SetLabel(childVisual as Label, theme);
                }
            }
        }
    }
}
