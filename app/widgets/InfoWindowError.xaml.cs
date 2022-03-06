using System.Windows;

namespace app.widgets
{
    public partial class InfoWindowError : Window
    {
        public InfoWindowError(string text)
        {
            InitializeComponent();

            MainTextLbl.Content = text;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
