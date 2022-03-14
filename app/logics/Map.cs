using System.Collections.Generic;
using Microsoft.Xna.Framework;
using app.logics.graph;
using System;

namespace app.logics
{
    /// <summary>
    /// Class for map
    /// </summary>
    class Map
    {
        /// <summary>
        /// Map width
        /// </summary>
        public int Width { get; private set; }
        
        /// <summary>
        /// Map Height
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// First selected post office
        ///
        /// This variable for select event
        /// </summary>
        public Vector2 firstSelectedPos { get; set; }
        
        /// <summary>
        /// Second selected post office
        ///
        /// This variable for select event
        /// </summary>
        public Vector2 secondSelectedPos { get; set; }

        /// <summary>
        /// Graph variable for Dijkstra algorithm
        /// </summary>
        public Graph _graph;

        public Graph graph
        {
            get
            {
                return _graph;
            }
            set
            {
                _graph = value;
            }
        }

        /// <summary>
        /// Variable that presents a map
        /// </summary>
        private CellTypes[,,] _map;

        public CellTypes[,,] map
        {
            get
            {
                return _map;
            }
            private set
            {
                _map = value;
            }
        }

        /// <summary>
        /// Indexer for map
        /// </summary>
        /// <param name="i">Cell position X</param>
        /// <param name="j">Cell position Y</param>
        /// <param name="w">Cell parameter</param>
        public CellTypes this[int i, int j, int w]
        {
            get { return _map[i, j, w]; }
            set { _map[i, j, w] = value; }
        }

        /// <summary>
        /// Constructor for map
        /// </summary>
        /// <param name="w">Map width</param>
        /// <param name="h">Map height</param>
        public Map(int w = 10, int h = 10)
        {
            _graph = new Graph();
            resize_map(w, h);
        }

        /// <summary>
        /// Method that resize map
        /// </summary>
        /// <param name="w">New width</param>
        /// <param name="h">New height</param>
        public void resize_map(int w, int h)
        {
            Width = w;
            Height = h;

            _map = new CellTypes[Width, Height, 6];
            // Reset all cell`s parameters
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    _map[i, j, CellIndexes.ground_type] = CellTypes.cell_ground_basic;
                    _map[i, j, CellIndexes.building_type] = CellTypes.cell_none;
                    _map[i, j, CellIndexes.isCarRoad] = CellTypes.cell_none;
                    _map[i, j, CellIndexes.isRailway] = CellTypes.cell_none;
                    _map[i, j, CellIndexes.isAirRoad] = CellTypes.cell_none;
                    _map[i, j, CellIndexes.isFocus] = CellTypes.cell_none;
                }
            }
        }

        /// <summary>
        /// Method for setting map
        /// </summary>
        /// <param name="w">New map width</param>
        /// <param name="h">New map height</param>
        /// <param name="map">New map array</param>
        public void setMap(int w, int h, ref CellTypes[,,] map)
        {
            resize_map(w, h);

            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    _map[i, j, CellIndexes.ground_type] = map[i, j, CellIndexes.ground_type];
                    _map[i, j, CellIndexes.building_type] = map[i, j, CellIndexes.building_type];
                    _map[i, j, CellIndexes.isCarRoad] = map[i, j, CellIndexes.isCarRoad];
                    _map[i, j, CellIndexes.isRailway] = map[i, j, CellIndexes.isRailway];
                    _map[i, j, CellIndexes.isAirRoad] = map[i, j, CellIndexes.isAirRoad];
                    _map[i, j, CellIndexes.isFocus] = map[i, j, CellIndexes.isFocus];
                }
            }
        }

        /// <summary>
        /// Method to set cell type
        /// </summary>
        /// <param name="x">Cell position X</param>
        /// <param name="y">Cell position Y</param>
        /// <param name="type">New cell type</param>
        public void setCellType(int x, int y, CellTypes type)
        {
            switch (type)
            {
                case CellTypes.cell_base:
                    _map[x, y, CellIndexes.ground_type] = CellTypes.cell_ground_basic;
                    _map[x, y, CellIndexes.building_type] = CellTypes.cell_base;
                    _graph.addVertex(new Vertex(x, y, CellTypes.cell_road_car));
                    break;
                case CellTypes.cell_train_station:
                    _map[x, y, CellIndexes.ground_type] = CellTypes.cell_ground_basic;
                    _map[x, y, CellIndexes.building_type] = CellTypes.cell_train_station;
                    _graph.addVertex(new Vertex(x, y, CellTypes.cell_road_train));
                    break;
                case CellTypes.cell_airport:
                    _map[x, y, CellIndexes.ground_type] = CellTypes.cell_ground_basic;
                    _map[x, y, CellIndexes.building_type] = CellTypes.cell_airport;
                    _graph.addVertex(new Vertex(x, y, CellTypes.cell_road_air));
                    break;
                case CellTypes.cell_ground_basic:
                    _map[x, y, CellIndexes.ground_type] = CellTypes.cell_ground_basic;
                    _map[x, y, CellIndexes.building_type] = CellTypes.cell_none;
                    break;
                case CellTypes.cell_ground_water:
                    _map[x, y, CellIndexes.ground_type] = CellTypes.cell_ground_water;
                    _map[x, y, CellIndexes.building_type] = CellTypes.cell_none;
                    break;
                case CellTypes.cell_focus:
                    _map[x, y, CellIndexes.isFocus] = CellTypes.cell_focus;
                    break;
            }
        }

        /// <summary>
        /// Method to make road between two objects
        /// </summary>
        /// <param name="pos1">First object position (start)</param>
        /// <param name="pos2">Second object position (end)</param>
        /// <param name="type">New road type</param>
        /// <returns>New road distance</returns>
        public int makeRoad(Vector2 pos1, Vector2 pos2, CellTypes type)
        {
            int dist = 0;

            // if the road is aerial, then ignore the water on the map
            if (type == CellTypes.cell_road_air)
            {
                Vector2 t;
                if (pos1.X > pos2.X)
                {
                    t = pos1;
                    pos1 = pos2;
                    pos2 = t;
                }
                for (int i = (int)pos1.X; i <= pos2.X; i++)
                {
                    _map[i, (int)pos1.Y, CellIndexes.isAirRoad] = type;
                    dist++;
                }
                if (pos1.Y < pos2.Y)
                {
                    for (int i = (int)pos1.Y; i <= pos2.Y; i++)
                    {
                        _map[(int)pos2.X, i, CellIndexes.isAirRoad] = type;
                        dist++;
                    }
                }
                else
                {
                    for (int i = (int)pos2.Y; i <= pos1.Y; i++)
                    {
                        _map[(int)pos2.X, i, CellIndexes.isAirRoad] = type;
                        dist++;
                    }
                }
            }
            else
            {
                int index = CellIndexes.isRailway;
                if (type == CellTypes.cell_road_car)
                {
                    index = CellIndexes.isCarRoad;
                }
                List<Vector2> cells = findPath(pos1, pos2);
                dist = cells.Count;
                for (int i = 0; i < cells.Count; i++)
                {
                    _map[(int)cells[i].X, (int)cells[i].Y, index] = type;
                }
            }

            return dist;
        }

        /// <summary>
        /// Get empty vertex for Dijkstra algorithm
        /// </summary>
        /// <param name="pos">Pos for vertex</param>
        /// <returns>New vertex</returns>
        public Vertex getVertex(Vector2 pos)
        {
            return new Vertex((int)pos.X, (int)pos.Y, getCellRoadType(pos));
        }
        
        /// <summary>
        /// Get empty vertex with current type for Dijkstra algorithm
        /// </summary>
        /// <param name="pos">Pos for vertex</param>
        /// <param name="type">New vertex type</param>
        /// <returns>New vertex</returns>
        public Vertex getVertex(Vector2 pos, CellTypes type)
        {
            return new Vertex((int)pos.X, (int)pos.Y, type);
        }

        /// <summary>
        /// Add road to graph
        /// </summary>
        /// <param name="pos1">First vertex position (start)</param>
        /// <param name="pos2">Second vertex position (end)</param>
        /// <param name="type">Road type</param>
        public void addRoad(Vector2 pos1, Vector2 pos2, CellTypes type)
        {
            _graph.addRoad(getVertex(pos1), getVertex(pos2), type, makeRoad(pos1, pos2, type));
        }

        /// <summary>
        /// Check if road exists
        /// </summary>
        /// <param name="pos1">First vertex position (start)</param>
        /// <param name="pos2">Second vertex position (end)</param>
        /// <param name="type">Road type</param>
        /// <returns>If road exists, then return true, else return false</returns>
        public bool checkRoad(Vector2 pos1, Vector2 pos2, CellTypes type)
        {
            if (_graph.containsRoad(getVertex(pos1), getVertex(pos2), type))
                return false;

            return true;
        }

        /// <summary>
        /// Method that find the path between start and end
        ///
        /// This method works for car road or railway
        /// </summary>
        /// <param name="pos_start">Position for start</param>
        /// <param name="pos_end">Position for end</param>
        /// <returns>The path</returns>
        public List<Vector2> findPath(Vector2 pos_start, Vector2 pos_end)
        {
            List<Vector2> path = new List<Vector2> { };

            int[,] matrix = new int[Width, Height];

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    if (_map[i, j, CellIndexes.ground_type] == CellTypes.cell_ground_water)
                    {
                        matrix[i, j] = -1;
                    }
                    else
                    {
                        matrix[i, j] = 0;
                    }
                }
            }

            matrix[(int)pos_start.X, (int)pos_start.Y] = 1;

            int k = 1;

            bool flag = true;

            while (matrix[(int)pos_end.X, (int)pos_end.Y] == 0 && flag)
            {
                flag = make_step(ref matrix, k);
                k++;
            }

            Vector2 last_pos = pos_end;

            if (flag)
            {
                while (k != 1)
                {
                    path.Add(last_pos);
                    if (last_pos.X > 0 && matrix[(int)last_pos.X - 1, (int)last_pos.Y] == k - 1)
                    {
                        last_pos = new Vector2((int)last_pos.X - 1, (int)last_pos.Y);
                        k--;
                    }
                    else if (last_pos.Y > 0 && matrix[(int)last_pos.X, (int)last_pos.Y - 1] == k - 1)
                    {
                        last_pos = new Vector2((int)last_pos.X, (int)last_pos.Y - 1);
                        k--;
                    }
                    else if (last_pos.X < Width - 1 && matrix[(int)last_pos.X + 1, (int)last_pos.Y] == k - 1)
                    {
                        last_pos = new Vector2((int)last_pos.X + 1, (int)last_pos.Y);
                        k--;
                    }
                    else if (last_pos.Y < Height - 1 && matrix[(int)last_pos.X, (int)last_pos.Y + 1] == k - 1)
                    {
                        last_pos = new Vector2((int)last_pos.X, (int)last_pos.Y + 1);
                        k--;
                    }
                }
                path.Add(pos_start);
            }

            return path;
        }

        /// <summary>
        /// Function for finding path
        ///
        /// for more information check https://habr.com/ru/post/444828/
        /// </summary>
        public bool make_step(ref int[,] matrix, int k)
        {
            bool flag = false;

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    if (matrix[i, j] == k)
                    {
                        if (i > 0 && matrix[i - 1, j] == 0)
                        {
                            matrix[i - 1, j] = k + 1;
                            flag = true;
                        }
                        if (j > 0 && matrix[i, j - 1] == 0)
                        {
                            matrix[i, j - 1] = k + 1;
                            flag = true;
                        }
                        if (i < Width - 1 && matrix[i + 1, j] == 0)
                        {
                            matrix[i + 1, j] = k + 1;
                            flag = true;
                        }
                        if (j < Height - 1 && matrix[i, j + 1] == 0)
                        {
                            matrix[i, j + 1] = k + 1;
                            flag = true;
                        }
                    }
                }
            }

            return flag;
        }
        
        /// <summary>
        /// Delete road that contains current cell
        /// </summary>
        /// <param name="pos">Current cell</param>
        public void DeleteRoads(Vector2 pos)
        {
            _graph.deleteRoads(pos);

            clearRoads();

            List<Vector4> rememberToDelete = new List<Vector4> { };

            foreach (Road _road in _graph.roads)
            {
                int dist = makeRoad(new Vector2(_road.first.X, _road.first.Y),
                        new Vector2(_road.second.X, _road.second.Y), _road.type);

                if (dist != 0)
                    _graph.setRoadDistance(_road.first, _road.second, _road.type, dist);
                else
                    rememberToDelete.Add(new Vector4(_road.first.X, _road.first.Y, _road.second.X, _road.second.Y));
            }

            for (int i = 0; i < rememberToDelete.Count; i++)
            {
                _graph.deleteCurrentNeighbours(new Vector2(rememberToDelete[i].X, rememberToDelete[i].Y),
                    new Vector2(rememberToDelete[i].Z, rememberToDelete[i].W));
            }
        }

        /// <summary>
        /// Clear all roads for map
        ///
        /// That method doesn't delete any roads
        /// </summary>
        public void clearRoads()
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    _map[i, j, CellIndexes.isCarRoad] = CellTypes.cell_none;
                    _map[i, j, CellIndexes.isRailway] = CellTypes.cell_none;
                    _map[i, j, CellIndexes.isAirRoad] = CellTypes.cell_none;
                }
            }
        }

        /// <summary>
        /// Clear all cell in map, include buildings and water
        /// </summary>
        public void clearAll()
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    _map[i, j, CellIndexes.ground_type] = CellTypes.cell_none;
                    _map[i, j, CellIndexes.building_type] = CellTypes.cell_none;
                    _map[i, j, CellIndexes.isCarRoad] = CellTypes.cell_none;
                    _map[i, j, CellIndexes.isRailway] = CellTypes.cell_none;
                    _map[i, j, CellIndexes.isAirRoad] = CellTypes.cell_none;
                    _map[i, j, CellIndexes.isFocus] = CellTypes.cell_none;
                }
            }

            _graph = new Graph();
        }

        /// <summary>
        /// Returns road type for cell
        /// </summary>
        /// <param name="pos">Cell position</param>
        /// <returns>Road cell type</returns>
        public CellTypes getCellRoadType(Vector2 pos)
        {
            if (_map[(int)pos.X, (int)pos.Y, CellIndexes.isCarRoad] == CellTypes.cell_road_car)
            {
                return CellTypes.cell_road_car;
            }
            if (_map[(int)pos.X, (int)pos.Y, CellIndexes.isRailway] == CellTypes.cell_road_train)
            {
                return CellTypes.cell_road_train;
            }
            if (_map[(int)pos.X, (int)pos.Y, CellIndexes.isRailway] == CellTypes.cell_road_air)
            {
                return CellTypes.cell_road_air;
            }
            
            return CellTypes.cell_none;
        }

        /// <summary>
        /// Method that returns path with error code
        /// </summary>
        /// <param name="start">Start cell position</param>
        /// <param name="end">End cell position</param>
        /// <returns>Returns path info with error code</returns>
        public Tuple<errorCodes, PathInfo> getPath(Vector2 start, Vector2 end)
        {
            if (start.X > Width - 1 || start.Y > Height - 1)
                return new Tuple<errorCodes, PathInfo>(errorCodes.startVertexDoesntExistError, new PathInfo());
            if (_map[(int)start.X, (int)start.Y, CellIndexes.building_type] != CellTypes.cell_base)
                return new Tuple<errorCodes, PathInfo>(errorCodes.startVertexDoesntExistError, new PathInfo());
            if (end.X > Width - 1 || end.Y > Height - 1)
                return new Tuple<errorCodes, PathInfo>(errorCodes.endVertexDoesntExistError, new PathInfo());
            if (_map[(int)end.X, (int)end.Y, CellIndexes.building_type] != CellTypes.cell_base)
                return new Tuple<errorCodes, PathInfo>(errorCodes.endVertexDoesntExistError, new PathInfo());

            Vertex _start = new Vertex((int)start.X, (int)start.Y, getCellRoadType(new Vector2((int)start.X, (int)start.Y)));
            Vertex _end = new Vertex((int)end.X, (int)end.Y, getCellRoadType(new Vector2((int)end.X, (int)end.Y)));

            return _graph.DijkstraAlgorim(_start, _end);
        }
    }
}
