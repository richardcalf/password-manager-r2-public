using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using password.encryption;

namespace password.manager.winforms
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void decryptButton_Click(object sender, RoutedEventArgs e)
        {
            Crypto crypt = new Crypto(Crypto.CryptoTypes.encTypeTripleDES);
            decryptedTextBox.Text = crypt.Decrypt(encryptedTextBox.Text);
        }

        private void encryptButton_Click(object sender, RoutedEventArgs e)
        {
            Crypto crypt = new Crypto(Crypto.CryptoTypes.encTypeTripleDES);
            encryptedTextBox.Text = crypt.Encrypt(plainTextBox.Text);
        }
    }
}
