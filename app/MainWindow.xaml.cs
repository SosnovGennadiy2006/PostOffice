using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq; // For Xml
using Microsoft.Xna.Framework;
using app.logics;
using app.widgets;
using app.logics.graph;
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

                if (path.Item1 == errorCodes.differentConnectivityComponentError)
                {
                    InfoWindow_notFound window = new InfoWindow_notFound(ErrorText.getError(path.Item1), StartPos, EndPos);
                    window.Owner = this;
                    window.ShowDialog();
                }else
                {
                    handleError(path);
                }
            }else
            {
                if (PointStart_X.Text == "" && PointEnd_X.Text == "")
                {
                    InfoWindowError window = new InfoWindowError(ErrorText.getError(errorCodes.startDestinationIsNotSelectedError));
                    window.Owner = this;
                    window.ShowDialog();
                }
                else if (PointEnd_X.Text == "" && PointEnd_Y.Text == "")
                {
                    InfoWindowError window = new InfoWindowError(ErrorText.getError(errorCodes.endDestinationIsNotSelectedError));
                    window.Owner = this;
                    window.ShowDialog();
                }
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

        private XDocument getSettings()
        {
            Vector2 mapSize = Game.getMapSize();
            int mapWidth = (int)mapSize.X;
            int mapHeight = (int)mapSize.Y;
            CellTypes[,,] map = Game.getMap();
            ref Graph graph = ref Game.getGraph();

            XDocument doc = new XDocument();

            // Size settings:
            XElement sizeX = new XElement("MapWidth", mapWidth);
            XElement sizeY = new XElement("MapHeight", mapHeight);

            // Map:
            XElement mapElem = new XElement("Map");

            string itemStr;

            for (int i = 0; i < mapHeight; i++)
            {
                XElement rowElem = new XElement("Row");

                for (int j = 0; j < mapWidth; j++)
                {
                    itemStr = "";

                    itemStr += Convert.ToString((int)map[j, i, CellIndexes.ground_type]);
                    itemStr += Convert.ToString((int)map[j, i, CellIndexes.building_type]);
                    itemStr += Convert.ToString((int)map[j, i, CellIndexes.isCarRoad]);
                    itemStr += Convert.ToString((int)map[j, i, CellIndexes.isRailway]);
                    itemStr += Convert.ToString((int)map[j, i, CellIndexes.isAirRoad]);
                    itemStr += 0;

                    XElement columnElem = new XElement("Item", itemStr);

                    rowElem.Add(columnElem);
                }

                mapElem.Add(rowElem);
            }

            // Graph:
            XElement graphElem = new XElement("Graph");

            XElement vertexesElem = new XElement("Vertexes");

            //Vertexes
            for (int i = 0; i < graph.vertexes.Count; i++)
            {
                XElement vertexElem = new XElement("Vertex");

                XElement xPosElem = new XElement("X", graph.vertexes[i].X);
                XElement yPosElem = new XElement("Y", graph.vertexes[i].Y);
                XElement typeElem = new XElement("Type", (int)graph.vertexes[i].type);

                // Neighbours:
                XElement neighboursElem = new XElement("Neighbours");

                for (int j = 0; j < graph.vertexes[i].neighbours.Count; j++)
                {
                    XElement neighbourElem = new XElement("Neighbour");

                    XElement neighbourPosXElem = new XElement("X", graph.vertexes[i].neighbours[j].neighbour.X);
                    XElement neighbourPosYElem = new XElement("Y", graph.vertexes[i].neighbours[j].neighbour.Y);
                    XElement neighbourDistanceElem = new XElement("Distance", graph.vertexes[i].neighbours[j].dist);
                    XElement neighbourCostElem = new XElement("Cost", graph.vertexes[i].neighbours[j].cost);
                    XElement neighbourTimeElem = new XElement("Time", graph.vertexes[i].neighbours[j].time);

                    neighbourElem.Add(neighbourPosXElem);
                    neighbourElem.Add(neighbourPosYElem);
                    neighbourElem.Add(neighbourDistanceElem);
                    neighbourElem.Add(neighbourCostElem);
                    neighbourElem.Add(neighbourTimeElem);

                    neighboursElem.Add(neighbourElem);
                }

                vertexElem.Add(xPosElem);
                vertexElem.Add(yPosElem);
                vertexElem.Add(typeElem);
                vertexElem.Add(neighboursElem);

                vertexesElem.Add(vertexElem);
            }

            XElement roadsElem = new XElement("Roads");

            //Roads
            for (int i = 0; i < graph.roads.Count; i++)
            {
                XElement roadElem = new XElement("Road");

                XElement typeElem = new XElement("Type", (int)graph.roads[i].type);

                XElement firstElem = new XElement("First");

                XElement firstPosXElem = new XElement("X", graph.roads[i].first.X);
                XElement firstPosYElem = new XElement("Y", graph.roads[i].first.Y);
                XElement firstPosTypeElem = new XElement("Type", graph.roads[i].first.type);

                firstElem.Add(firstPosXElem);
                firstElem.Add(firstPosYElem);
                firstElem.Add(firstPosTypeElem);

                XElement secondElem = new XElement("Second");

                XElement secondPosXElem = new XElement("X", graph.roads[i].second.X);
                XElement secondPosYElem = new XElement("Y", graph.roads[i].second.Y);
                XElement secondPosTypeElem = new XElement("Type", graph.roads[i].second.type);

                secondElem.Add(secondPosXElem);
                secondElem.Add(secondPosYElem);
                secondElem.Add(secondPosTypeElem);

                XElement distElem = new XElement("Distance", graph.roads[i].distance);
                XElement costElem = new XElement("Cost", graph.roads[i].cost);
                XElement timeElem = new XElement("Time", graph.roads[i].time);

                roadElem.Add(typeElem);
                roadElem.Add(firstElem);
                roadElem.Add(secondElem);
                roadElem.Add(distElem);
                roadElem.Add(costElem);
                roadElem.Add(timeElem);

                roadsElem.Add(roadElem);
            }

            graphElem.Add(vertexesElem);
            graphElem.Add(roadsElem);

            XElement settings_elem = new XElement("settings");

            settings_elem.Add(sizeX);
            settings_elem.Add(sizeY);
            settings_elem.Add(mapElem);
            settings_elem.Add(graphElem);

            doc.Add(settings_elem);

            return doc;
        }

        public static CellTypes convertToCellTypes(int _int)
        {
            switch ((char)_int)
            {
                case '0': return CellTypes.cell_none;
                case '1': return CellTypes.cell_base;
                case '2': return CellTypes.cell_train_station;
                case '3': return CellTypes.cell_airport;
                case '4': return CellTypes.cell_ground_basic;
                case '5': return CellTypes.cell_ground_water;
                case '6': return CellTypes.cell_road_car;
                case '7': return CellTypes.cell_road_train;
                case '8': return CellTypes.cell_road_air;
                case '9': return CellTypes.cell_focus;
            }
            return CellTypes.cell_none;
        }

        private void setSettings(string filePath)
        {
            XElement root = XElement.Load(filePath);
            int width = Convert.ToInt32(root.Element("MapWidth").Value);
            int height = Convert.ToInt32(root.Element("MapHeight").Value);
            Graph emptyGraph = new Graph();

            int i = 0, j;
            CellTypes[,,] map = new CellTypes[width, height, 6];

            // Map:
            foreach (XElement row in root.Element("Map").Elements())
            {
                j = 0;
                foreach (XElement elem in row.Elements())
                {
                    string mapItem = elem.Value;
                    map[j, i, CellIndexes.building_type] = convertToCellTypes(mapItem[CellIndexes.building_type]);
                    map[j, i, CellIndexes.ground_type] = convertToCellTypes(mapItem[CellIndexes.ground_type]);
                    map[j, i, CellIndexes.isCarRoad] = convertToCellTypes(mapItem[CellIndexes.isCarRoad]);
                    map[j, i, CellIndexes.isRailway] = convertToCellTypes(mapItem[CellIndexes.isRailway]);
                    map[j, i, CellIndexes.isAirRoad] = convertToCellTypes(mapItem[CellIndexes.isAirRoad]);
                    map[j, i, CellIndexes.isFocus] = convertToCellTypes(mapItem[CellIndexes.isFocus]);
                    j++;
                }
                i++;
            }

            // Vertexes:
            foreach (XElement vertex in root.Element("Graph").Element("Vertexes").Elements())
            {
                int posX = Convert.ToInt32(vertex.Element("X").Value);
                int posY = Convert.ToInt32(vertex.Element("Y").Value);
                CellTypes type = convertToCellTypes(vertex.Element("Type").Value[0]);

                Vertex currentVertex = new Vertex(posX, posY, type);

                foreach (XElement neighbour in vertex.Element("Neighbours").Elements())
                {
                    int neighborPosX = Convert.ToInt32(neighbour.Element("X").Value);
                    int neighborPosY = Convert.ToInt32(neighbour.Element("Y").Value);
                    int dist = Convert.ToInt32(neighbour.Element("Distance").Value);
                    int cost = Convert.ToInt32(neighbour.Element("Cost").Value);
                    int time = Convert.ToInt32(neighbour.Element("Time").Value);

                    currentVertex.addNeighbour(new Vertex(neighborPosX, neighborPosY, CellTypes.cell_none), dist, cost, time);
                }

                emptyGraph.addVertex(currentVertex);
            }

            // Roads:
            foreach (XElement road in root.Element("Graph").Element("Roads").Elements())
            {
                CellTypes type = convertToCellTypes(road.Element("Type").Value[0]);
                int dist = Convert.ToInt32(road.Element("Distance").Value);

                // first:
                int firstPosX = Convert.ToInt32(road.Element("First").Element("X").Value);
                int firstPosY = Convert.ToInt32(road.Element("First").Element("Y").Value);
                CellTypes firstType = convertToCellTypes(road.Element("First").Element("Type").Value[0]);

                // second:
                int secondPosX = Convert.ToInt32(road.Element("Second").Element("X").Value);
                int secondPosY = Convert.ToInt32(road.Element("Second").Element("Y").Value);
                CellTypes secondType = convertToCellTypes(road.Element("Second").Element("Type").Value[0]);

                emptyGraph.addRoad(new Vertex(firstPosX, firstPosY, firstType), new Vertex(secondPosX, secondPosY, secondType), type, dist);
            }

            MapWidthParam.Text = Convert.ToString(width);
            MapHeightParam.Text = Convert.ToString(height);

            Game.setMap(width, height, ref map);
            Game.setGraph(emptyGraph);

            Game.changeHoveredCellType(CellTypes.cell_none);
            Game.setSelectedRoadType(CellTypes.cell_none);
            Game.setOperationState(false);
            selectType = selectPointTypeEnum.None;

            PointStart_X.Text = "";
            PointStart_Y.Text = "";
            PointEnd_X.Text = "";
            PointEnd_Y.Text = "";
        }

        private void SaveCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Save();
        }

        private void OpenCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Open();
        }

        private void Save()
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "settings"; // Default file name
            dlg.DefaultExt = ".xml"; // Default file extension
            dlg.Filter = "XML documents (.xml)|*.xml"; // Filter files by extension
                                                        // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();
            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;

                getSettings().Save(filename);
            }
        }

        private void Open()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Multiselect = false;
            dlg.DefaultExt = ".xml"; // Default file extension
            dlg.Filter = "XML documents (.xml)|*.xml"; // Filter files by extension
                                                        // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();
            // Process open file dialog box results
            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;

                try
                {
                    setSettings(filename);
                }catch
                {
                    InfoWindowError window = new InfoWindowError("Error while loading settings!");
                    window.Owner = this;
                    window.ShowDialog();
                }
            }
        }
    }
}
