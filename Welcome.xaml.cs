using System.Windows;

namespace CyberLikh
{
    public partial class Welcome : Window
    {
        public Welcome()
        {
            InitializeComponent();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string userName = NameInput.Text.Trim();
            if (!string.IsNullOrEmpty(userName))
            {
                REDAPP chatWindow = new REDAPP(userName);
                chatWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Please enter your name before continuing.", "Missing Name",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}