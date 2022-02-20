using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Routing.logics
{
    class Map
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Vector2 firstSelectedPos { get; set; }
        public Vector2 secondSelectedPos { get; set; }

        public List<Vector4> CarRoads { get; private set; }
        public List<Vector4> TrainRoads { get; private set; }
        public List<Vector4> AirRoads { get; private set; }

        private CellTypes[,,] _map;

        public CellTypes this[int i, int j, int w]
        {
            get { return _map[i, j, w]; }
            set { _map[i, j, w] = value; }
        }

        public Map(int w = 10, int h = 10)
        {
            resize_map(w, h);

            CarRoads = new List<Vector4> { };
            TrainRoads = new List<Vector4> { };
            AirRoads = new List<Vector4> { };
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
                    _map[i, j, (int)CellIndexes.ground_type] = CellTypes.cell_ground_basic;
                    _map[i, j, (int)CellIndexes.building_type] = CellTypes.cell_none;
                    _map[i, j, (int)CellIndexes.isCarRoad] = CellTypes.cell_none;
                    _map[i, j, (int)CellIndexes.isRailway] = CellTypes.cell_none;
                    _map[i, j, (int)CellIndexes.isAirRoad] = CellTypes.cell_none;
                    _map[i, j, (int)CellIndexes.isFocus] = CellTypes.cell_none;
                }
            }
        }

        public void setCellType(int x, int y, CellTypes type)
        {
            switch (type)
            {
                case CellTypes.cell_base:
                    _map[x, y, (int)CellIndexes.ground_type] = CellTypes.cell_ground_basic;
                    _map[x, y, (int)CellIndexes.building_type] = CellTypes.cell_base;
                    break;
                case CellTypes.cell_train_station:
                    _map[x, y, (int)CellIndexes.ground_type] = CellTypes.cell_ground_basic;
                    _map[x, y, (int)CellIndexes.building_type] = CellTypes.cell_train_station;
                    break;
                case CellTypes.cell_airport:
                    _map[x, y, (int)CellIndexes.ground_type] = CellTypes.cell_ground_basic;
                    _map[x, y, (int)CellIndexes.building_type] = CellTypes.cell_airport;
                    break;
                case CellTypes.cell_ground_basic:
                    _map[x, y, (int)CellIndexes.ground_type] = CellTypes.cell_ground_basic;
                    _map[x, y, (int)CellIndexes.building_type] = CellTypes.cell_none;
                    break;
                case CellTypes.cell_ground_water:
                    _map[x, y, (int)CellIndexes.ground_type] = CellTypes.cell_ground_water;
                    _map[x, y, (int)CellIndexes.building_type] = CellTypes.cell_none;
                    break;
                case CellTypes.cell_focus:
                    _map[x, y, (int)CellIndexes.isFocus] = CellTypes.cell_focus;
                    break;
            }
        }

        public void makeRoad(Vector2 pos1, Vector2 pos2, CellTypes type)
        {
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
                    _map[i, (int)pos1.Y, (int)CellIndexes.isAirRoad] = type;
                }
                if (pos1.Y < pos2.Y)
                {
                    for (int i = (int)pos1.Y; i <= pos2.Y; i++)
                    {
                        _map[(int)pos2.X, i, (int)CellIndexes.isAirRoad] = type;
                    }
                }
                else
                {
                    for (int i = (int)pos2.Y; i <= pos1.Y; i++)
                    {
                        _map[(int)pos2.X, i, (int)CellIndexes.isAirRoad] = type;
                    }
                }
            }
            else
            {
                int index = (int)CellIndexes.isRailway;
                if (type == CellTypes.cell_road_car)
                {
                    index = (int)CellIndexes.isCarRoad;
                }
                List<Vector2> cells = findPath(pos1, pos2);
                if (cells.Count != 0)
                {
                    for (int i = 0; i < cells.Count; i++)
                    {
                        _map[(int)cells[i].X, (int)cells[i].Y, index] = type;
                    }
                }
            }
        }

        public void addRoad(Vector2 pos1, Vector2 pos2, CellTypes type)
        {
            switch (type)
            {
                case CellTypes.cell_road_car:
                    {
                        CarRoads.Add(new Vector4(pos1.X, pos1.Y, pos2.X, pos2.Y));
                        break;
                    }
                case CellTypes.cell_road_train:
                    {
                        TrainRoads.Add(new Vector4(pos1.X, pos1.Y, pos2.X, pos2.Y));
                        break;
                    }
                case CellTypes.cell_road_air:
                    {
                        AirRoads.Add(new Vector4(pos1.X, pos1.Y, pos2.X, pos2.Y));
                        break;
                    }
            }
        }

        public bool checkRoad(Vector2 pos1, Vector2 pos2, CellTypes type)
        {
            if (type == CellTypes.cell_road_air)
            {
                if (AirRoads.Contains(new Vector4(pos1.X, pos1.Y, pos2.X, pos2.Y)) || AirRoads.Contains(new Vector4(pos2.X, pos2.Y, pos1.X, pos1.Y)))
                {
                    return false;
                }
            }
            else if (type == CellTypes.cell_road_car)
            {
                if (CarRoads.Contains(new Vector4(pos1.X, pos1.Y, pos2.X, pos2.Y)) || CarRoads.Contains(new Vector4(pos2.X, pos2.Y, pos1.X, pos1.Y)))
                {
                    return false;
                }
            }
            else if (type == CellTypes.cell_road_train)
            {
                if (TrainRoads.Contains(new Vector4(pos1.X, pos1.Y, pos2.X, pos2.Y)) || TrainRoads.Contains(new Vector4(pos2.X, pos2.Y, pos1.X, pos1.Y)))
                {
                    return false;
                }
            }

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
                    if (_map[i, j, (int)CellIndexes.ground_type] == CellTypes.cell_ground_water)
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
            List<int> CarRoadsToDelete = new List<int> { };
            List<int> TrainRoadsToDelete = new List<int> { };
            List<int> AirRoadsToDelete = new List<int> { };

            for (int i = 0; i < CarRoads.Count; i++)
            {
                if ((CarRoads[i].X == pos.X && CarRoads[i].Y == pos.Y) || (CarRoads[i].Z == pos.X && CarRoads[i].W == pos.Y))
                {
                    CarRoadsToDelete.Add(i);
                }
            }

            for (int i = 0; i < TrainRoads.Count; i++)
            {
                if ((TrainRoads[i].X == pos.X && TrainRoads[i].Y == pos.Y) || (TrainRoads[i].Z == pos.X && TrainRoads[i].W == pos.Y))
                {
                    TrainRoadsToDelete.Add(i);
                }
            }

            for (int i = 0; i < AirRoads.Count; i++)
            {
                if ((AirRoads[i].X == pos.X && AirRoads[i].Y == pos.Y) || (AirRoads[i].Z == pos.X && AirRoads[i].W == pos.Y))
                {
                    AirRoadsToDelete.Add(i);
                }
            }

            for (int index = (int)CarRoadsToDelete.Count - 1; index >= 0; index--)
            {
                CarRoads.RemoveAt(CarRoadsToDelete[index]);
            }
            for (int index = (int)TrainRoadsToDelete.Count - 1; index >= 0; index--)
            {
                TrainRoads.RemoveAt(TrainRoadsToDelete[index]);
            }
            for (int index = (int)AirRoadsToDelete.Count - 1; index >= 0; index--)
            {
                AirRoads.RemoveAt(AirRoadsToDelete[index]);
            }

            clearRoads();

            foreach (Vector4 road in CarRoads.ToArray())
            {
                makeRoad(new Vector2(road.X, road.Y), new Vector2(road.Z, road.W), CellTypes.cell_road_car);
            }
            foreach (Vector4 road in TrainRoads.ToArray())
            {
                makeRoad(new Vector2(road.X, road.Y), new Vector2(road.Z, road.W), CellTypes.cell_road_train);
            }
            foreach (Vector4 road in AirRoads.ToArray())
            {
                makeRoad(new Vector2(road.X, road.Y), new Vector2(road.Z, road.W), CellTypes.cell_road_air);
            }
        }

        public void clearRoads()
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    _map[i, j, (int)CellIndexes.isCarRoad] = CellTypes.cell_none;
                    _map[i, j, (int)CellIndexes.isRailway] = CellTypes.cell_none;
                    _map[i, j, (int)CellIndexes.isAirRoad] = CellTypes.cell_none;
                }
            }
        }
    }
}
