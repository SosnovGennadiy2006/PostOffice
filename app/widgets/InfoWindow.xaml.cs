using System;
using System.Windows;
using app.logics;

namespace app.widgets
{
    public partial class InfoWindow : Window
    {
        public InfoWindow(ref PathInfo path)
        {
            InitializeComponent();

            distanceIndicator.Content = Convert.ToString(path.totalDistance);
            costIndicator.Content = Convert.ToString(path.totalCost);
            timeIndicator.Content = Convert.ToString(path.totalTime);

            firstPoint.Text = "(" + Convert.ToString(path.first().X + 1) + "," + Convert.ToString(path.first().Y + 1) + ")";
            secondPoint.Text = "(" + Convert.ToString(path.last().X + 1) + "," + Convert.ToString(path.last().Y + 1) + ")";

            string pathString = "";

            for (int i = 0; i < path.path.Count - 1; i++)
            {
                pathString += path.toStringByIndex(i);
                pathString += " -> ";
            }

            pathString += path.toStringByIndex(path.path.Count - 1);

            PathTextArea.Text = pathString;
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
