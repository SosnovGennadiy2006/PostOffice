using System.Collections.Generic;
using Microsoft.Xna.Framework;
using app.logics.graph;
using System;

namespace app.logics
{
    class Map
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Vector2 firstSelectedPos { get; set; }
        public Vector2 secondSelectedPos { get; set; }

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

        public CellTypes this[int i, int j, int w]
        {
            get { return _map[i, j, w]; }
            set { _map[i, j, w] = value; }
        }

        public Map(int w = 10, int h = 10)
        {
            _graph = new Graph();
            resize_map(w, h);
        }

        public void resize_map(int w, int h)
        {
            Width = w;
            Height = h;

            _map = new CellTypes[Width, Height, 6];
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

        public int makeRoad(Vector2 pos1, Vector2 pos2, CellTypes type)
        {
            int dist = 0;

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
                for (int i = 0; i < cells.Count; i++)
                {
                    _map[(int)cells[i].X, (int)cells[i].Y, index] = type;
                    dist++;
                }
            }

            return dist;
        }

        public Vertex getVertex(Vector2 pos)
        {
            return new Vertex((int)pos.X, (int)pos.Y, getCellRoadType(pos));
        }
        
        public Vertex getVertex(Vector2 pos, CellTypes type)
        {
            return new Vertex((int)pos.X, (int)pos.Y, type);
        }

        public void addRoad(Vector2 pos1, Vector2 pos2, CellTypes type)
        {
            _graph.addRoad(getVertex(pos1), getVertex(pos2), type, makeRoad(pos1, pos2, type));
        }

        public bool checkRoad(Vector2 pos1, Vector2 pos2, CellTypes type)
        {
            if (_graph.containsRoad(getVertex(pos1), getVertex(pos2), type))
                return false;

            return true;
        }

        public List<Vector2> findPath(Vector2 pos_start, Vector2 pos_end)
        {
            List<Vector2> path = new List<Vector2> { };

            int[,] matrix = new int[Width, (int)Height];

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

        public void DeleteRoads(Vector2 pos)
        {
            _graph.deleteRoads(pos);

            clearRoads();

            foreach (Road _road in _graph.roads)
            {
                _graph.setRoadDistance(_road.first, _road.second, _road.type,
                    makeRoad(new Vector2(_road.first.X, _road.first.Y),
                    new Vector2(_road.second.X, _road.second.Y), _road.type));
            }
        }
        
        public void DeleteRoads(Vector2 pos, CellTypes type)
        {
            
            _graph.deleteRoads(pos);

            clearRoads();

            foreach (Road _road in _graph.roads)
            {
                _graph.setRoadDistance(_road.first, _road.second, _road.type,
                    makeRoad(new Vector2(_road.first.X, _road.first.Y),
                        new Vector2(_road.second.X, _road.second.Y), _road.type));
            }
        }

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

        public Tuple<errorCodes, PathInfo> DijkstraAlgorim(Vertex start, Vertex end)
        {
            return _graph.DijkstraAlgorim(start, end);
        }

        public CellTypes getCellRoadType(Vector2 pos)
        {
            if (_map[(int)pos.X, (int)pos.Y, CellIndexes.isCarRoad] == CellTypes.cell_road_car)
            {
                return CellTypes.cell_road_car;
            }else if (_map[(int)pos.X, (int)pos.Y, CellIndexes.isRailway] == CellTypes.cell_road_train)
            {
                return CellTypes.cell_road_train;
            }else if (_map[(int)pos.X, (int)pos.Y, CellIndexes.isRailway] == CellTypes.cell_road_air)
            {
                return CellTypes.cell_road_air;
            }else
            {
                return CellTypes.cell_none;
            }
        }

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
