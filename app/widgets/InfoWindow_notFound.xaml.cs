using System;
using System.Windows;
using Microsoft.Xna.Framework;

namespace app.widgets
{
    public partial class InfoWindow_notFound : Window
    {
        public InfoWindow_notFound(string reasonText, Vector2 start, Vector2 end)
        {
            InitializeComponent();

            ReasonLbl.Text = reasonText;
            firstPoint.Text = "(" + Convert.ToString(start.X + 1) + ", " + Convert.ToString(start.Y + 1) + ")";
            secondPoint.Text = "(" + Convert.ToString(end.X + 1) + ", " + Convert.ToString(end.Y + 1) + ")";
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
