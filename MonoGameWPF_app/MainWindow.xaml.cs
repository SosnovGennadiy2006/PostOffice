using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Microsoft.Xna.Framework;
using CellTypes = Routing.MainWindowViewModel.CellTypes;

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
            if (onlyNumeric(e.Text))
            {
                int n = Convert.ToInt32(MapWidthParam.Text + e.Text);
                
                if (n > 25)
                {
                    MapWidthParam.Text = "30";
                    e.Handled = true;
                    return;
                }
                if (n < 5)
                {
                    MapWidthParam.Text = "5";
                    e.Handled = true;
                    return;
                }

                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }
        
        private void textBox2_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (onlyNumeric(e.Text))
            {
                int n = Convert.ToInt32(MapHeightParam.Text + e.Text);
                
                if (n > 25)
                {
                    MapHeightParam.Text = "30";
                    e.Handled = true;
                    return;
                }
                if (n < 5)
                {
                    MapHeightParam.Text = "5";
                    e.Handled = true;
                    return;
                }

                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
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

        private void ChangeMapSizeBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (MapWidthParam.Text != "" && MapHeightParam.Text != "")
            {
                Game.setMapSize(new Vector2(Convert.ToInt32(MapWidthParam.Text), Convert.ToInt32(MapHeightParam.Text)));
            }
            else
            {
                MapWidthParam.Text = "10";
                MapHeightParam.Text = "10";
                Game.setMapSize(new Vector2(10, 10));
            }
        }

        private void Game_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            Game.changeMousePressState(true);
        }

        private void Game_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            Game.changeMousePressState(false);
        }

        private void BasicGroundBtn_OnClick(object sender, RoutedEventArgs e)
        {
            Game.changeHoveredCellType(CellTypes.cell_ground_basic);
        }

        private void WaterGroundBtn_OnClick(object sender, RoutedEventArgs e)
        {
            Game.changeHoveredCellType(CellTypes.cell_ground_water);
        }

        private void BasePlaceBtn_OnClick(object sender, RoutedEventArgs e)
        {
            Game.changeHoveredCellType(CellTypes.cell_base);
            Game.setSelectedRoadType(CellTypes.cell_none);
        }

        private void AirportPlaceBtn_OnClick(object sender, RoutedEventArgs e)
        {
            Game.changeHoveredCellType(CellTypes.cell_airport);
            Game.setSelectedRoadType(CellTypes.cell_none);
        }

        private void TrainStationBtn_OnClick(object sender, RoutedEventArgs e)
        {
            Game.changeHoveredCellType(CellTypes.cell_train_station);
            Game.setSelectedRoadType(CellTypes.cell_none);
        }

        private void CarRoadBtn_OnClick(object sender, RoutedEventArgs e)
        {
            Game.changeHoveredCellType(CellTypes.cell_none);
            Game.setSelectedRoadType(CellTypes.cell_road_car);
        }

        private void RailwayBtn_OnClick(object sender, RoutedEventArgs e)
        {
            Game.changeHoveredCellType(CellTypes.cell_none);
            Game.setSelectedRoadType(CellTypes.cell_road_train);
        }

        private void AirWayBtn_OnClick(object sender, RoutedEventArgs e)
        {
            Game.changeHoveredCellType(CellTypes.cell_none);
            Game.setSelectedRoadType(CellTypes.cell_road_air);
        }
    }
}
