using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Microsoft.Xna.Framework;

namespace Routing
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public static bool onlyNumeric(string text)
        {
            Regex regex = new Regex("[^0-9]+"); //regex that allows numeric input only
            return !regex.IsMatch(text);
        }

        private void textBox1_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !onlyNumeric(e.Text);
        }

        private void GameMouseMove(object sender, MouseEventArgs e)
        {
            Vector2 mousePos = new Vector2((float)e.GetPosition(Game).X, (float)e.GetPosition(Game).Y);
            Game.setMousePos(mousePos);
        }

        private void GameMouseLeave(object sender, MouseEventArgs e)
        {
            Game.setMousePos(new Vector2(-1, -1));
        }
    }
}
