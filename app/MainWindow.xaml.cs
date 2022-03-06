using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Microsoft.Xna.Framework;
using app.logics;
using app.widgets;
using CellTypes = app.logics.CellTypes;
using selectPointTypeEnum = app.logics.selectPointTypeEnum;

namespace app
{
    public partial class MainWindow : Window
    {
        selectPointTypeEnum selectType;

        public MainWindow()
        {
            InitializeComponent();
        }

        public static bool onlyNumeric(string text)
        {
            Regex regex = new Regex("[^0-9]+"); //regex that allows numeric input only
            return !regex.IsMatch(text);
        }

        private void textBoxPreviewTextInput(object sender, TextCompositionEventArgs e)
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

        private void ChangeMapSizeBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (MapWidthParam.Text != "" && MapHeightParam.Text != "")
            {
                int n = Convert.ToInt32(MapWidthParam.Text);
                int m = Convert.ToInt32(MapHeightParam.Text);
                if (n > 30)
                {
                    MapWidthParam.Text = "30";
                }else if (n < 5)
                {
                    MapWidthParam.Text = "5";
                }
                if (m > 30)
                {
                    MapHeightParam.Text = "30";
                }
                else if (m < 5)
                {
                    MapHeightParam.Text = "5";
                }
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
            Game.setSelectedRoadType(CellTypes.cell_none);
            Game.setOperationState(false);
            selectType = selectPointTypeEnum.None;
        }

        private void WaterGroundBtn_OnClick(object sender, RoutedEventArgs e)
        {
            Game.changeHoveredCellType(CellTypes.cell_ground_water);
            Game.setSelectedRoadType(CellTypes.cell_none);
            Game.setOperationState(false);
            selectType = selectPointTypeEnum.None;
        }

        private void BasePlaceBtn_OnClick(object sender, RoutedEventArgs e)
        {
            Game.changeHoveredCellType(CellTypes.cell_base);
            Game.setSelectedRoadType(CellTypes.cell_none);
            Game.setOperationState(false);
            selectType = selectPointTypeEnum.None;
        }

        private void AirportPlaceBtn_OnClick(object sender, RoutedEventArgs e)
        {
            Game.changeHoveredCellType(CellTypes.cell_airport);
            Game.setSelectedRoadType(CellTypes.cell_none);
            Game.setOperationState(false);
            selectType = selectPointTypeEnum.None;
        }

        private void TrainStationBtn_OnClick(object sender, RoutedEventArgs e)
        {
            Game.changeHoveredCellType(CellTypes.cell_train_station);
            Game.setSelectedRoadType(CellTypes.cell_none);
            Game.setOperationState(false);
            selectType = selectPointTypeEnum.None;
        }

        private void CarRoadBtn_OnClick(object sender, RoutedEventArgs e)
        {
            Game.changeHoveredCellType(CellTypes.cell_none);
            Game.setSelectedRoadType(CellTypes.cell_road_car);
            Game.setOperationState(false);
            selectType = selectPointTypeEnum.None;
        }

        private void RailwayBtn_OnClick(object sender, RoutedEventArgs e)
        {
            Game.changeHoveredCellType(CellTypes.cell_none);
            Game.setSelectedRoadType(CellTypes.cell_road_train);
            Game.setOperationState(false);
            selectType = selectPointTypeEnum.None;
        }

        private void AirWayBtn_OnClick(object sender, RoutedEventArgs e)
        {
            Game.changeHoveredCellType(CellTypes.cell_none);
            Game.setSelectedRoadType(CellTypes.cell_road_air);
            Game.setOperationState(false);
            selectType = selectPointTypeEnum.None;
        }

        private void SelectEndPoint_Btn_Click(object sender, RoutedEventArgs e)
        {
            Game.changeHoveredCellType(CellTypes.cell_none);
            Game.setSelectedRoadType(CellTypes.cell_none);
            Game.setOperationState(true);
            selectType = selectPointTypeEnum.End;
        }

        private void SelectStartPoint_Btn_Click(object sender, RoutedEventArgs e)
        {
            Game.changeHoveredCellType(CellTypes.cell_none);
            Game.setSelectedRoadType(CellTypes.cell_none);
            Game.setOperationState(true);
            selectType = selectPointTypeEnum.Start;
        }

        private void Game_Select(object sender, RoutedMapSelectedEventArgs e)
        {
            if (selectType == selectPointTypeEnum.Start)
            {
                PointStart_X.Text = Convert.ToString(e.Pos.X + 1);
                PointStart_Y.Text = Convert.ToString(e.Pos.Y + 1);

                Game.changeHoveredCellType(CellTypes.cell_none);
                Game.setSelectedRoadType(CellTypes.cell_none);
                Game.setOperationState(false);
                selectType = selectPointTypeEnum.None;
            }
            else if (selectType == selectPointTypeEnum.End)
            {
                PointEnd_X.Text = Convert.ToString(e.Pos.X + 1);
                PointEnd_Y.Text = Convert.ToString(e.Pos.Y + 1);

                Game.changeHoveredCellType(CellTypes.cell_none);
                Game.setSelectedRoadType(CellTypes.cell_none);
                Game.setOperationState(false);
                selectType = selectPointTypeEnum.None;
            }
        }

        private void ClearMapBtn_Click(object sender, RoutedEventArgs e)
        {
            Game.clearWholeMap();
        }

        private void getPathBtn_Click(object sender, RoutedEventArgs e)
        {
            if (PointStart_X.Text != "" && PointStart_Y.Text != "" && PointEnd_X.Text != "" && PointEnd_Y.Text != "")
            {
                Vector2 StartPos = new Vector2(Convert.ToInt32(PointStart_X.Text) - 1, Convert.ToInt32(PointStart_Y.Text) - 1);
                Vector2 EndPos = new Vector2(Convert.ToInt32(PointEnd_X.Text) - 1, Convert.ToInt32(PointEnd_Y.Text) - 1);

                Tuple<errorCodes, PathInfo> path = Game.getPath(StartPos, EndPos);

                handleError(path);
            }else
            {
                InfoWindowError window = new InfoWindowError("");
                if (PointStart_X.Text == "" || PointEnd_X.Text == "")
                {
                    window = new InfoWindowError(ErrorText.getError(errorCodes.startDestinationIsNotSelectedError));
                }
                else if (PointEnd_X.Text == "" || PointEnd_Y.Text == "")
                {
                    window = new InfoWindowError(ErrorText.getError(errorCodes.endDestinationIsNotSelectedError));
                }
                window.Owner = this;
                window.ShowDialog();
            }
        }

        private void handleError(Tuple<errorCodes, PathInfo> path)
        {
            switch (path.Item1)
            {
                case errorCodes.success:
                    {
                        PathInfo info = path.Item2;

                        InfoWindow window = new InfoWindow(ref info);
                        window.Owner = this;
                        window.ShowDialog();

                        break;
                    }
                case errorCodes.endVertexDoesntExistError:
                case errorCodes.startVertexDoesntExistError:
                    {
                        InfoWindowError window = new InfoWindowError(ErrorText.getError(path.Item1));
                        window.Owner = this;
                        window.ShowDialog();

                        break;
                    }
            }
        }
    }
}
