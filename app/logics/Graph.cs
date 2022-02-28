using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace app.logics
{
    public class Graph
    {
        public List<Vertex> vertexes { get; private set; }
        public List<Road> roads { get; private set; }

        public Dictionary<Vertex, int> d;
        public int remembered_pos;

        public Graph()
        {
            vertexes = new List<Vertex> { };
            roads = new List<Road> { };
        }

        public bool containsRoad(Vertex _first, Vertex _second, CellTypes type)
        {
            for (int i = 0; i < roads.Count; i++)
            {
                if (roads[i].Equal(_first, _second, type))
                    return true;
            }
            return false;
        }

        public void setRoadDistance(Vertex _first, Vertex _second, CellTypes type, int dist)
        {
            for (int i = 0; i < roads.Count; i++)
            {
                if (roads[i].Equal(_first, _second, type))
                    roads[i].setDistance(dist);
            }
        }

        public void addRoad(Vertex _first, Vertex _second, CellTypes type)
        {
            Road road = new Road(_first, _second, type);
            roads.Add(road);

            for (int i = 0; i < vertexes.Count; i++)
            {
                if (vertexes[i] == _first)
                    vertexes[i].addNeighbour(_second, road.distance);
                if (vertexes[i] == _second)
                    vertexes[i].addNeighbour(_first, road.distance);
            }
        }

        public void addVertex(Vertex _vertex)
        {
            if (!vertexes.Contains(_vertex))
            {
                vertexes.Add(_vertex);
            }
        }

        public void deleteRoads(Vertex _vertex)
        {
            List<int> roadsToDelete = new List<int> { };

            for (int i = 0; i < roads.Count; i++)
            {
                if ((roads[i].first == _vertex) || (roads[i].second == _vertex))
                {
                    roadsToDelete.Add(i);
                }
            }

            for (int i = roadsToDelete.Count - 1; i >= 0; i--)
            {
                roads.RemoveAt(roadsToDelete[i]);
            }

            for (int i = 0; i < vertexes.Count(); i++)
            {
                vertexes[i].deleteNeighbour(_vertex);
            }

            vertexes.Remove(_vertex);
        }

        public void deleteRoads(Vector2 pos)
        {
            List<int> roadsToDelete = new List<int> { };

            for (int i = 0; i < roads.Count; i++)
            {
                if ((roads[i].first.X == pos.X && roads[i].first.Y == pos.Y) || (roads[i].second.X == pos.X && roads[i].second.Y == pos.Y))
                {
                    roadsToDelete.Add(i);
                }
            }

            for (int i = roadsToDelete.Count - 1; i >= 0; i--)
            {
                roads.RemoveAt(roadsToDelete[i]);
            }

            int k = -1;

            for (int i = 0; i < vertexes.Count(); i++)
            {
                vertexes[i].deleteNeighbour(pos);
                if (vertexes[i].X == pos.X && vertexes[i].Y == pos.Y)
                    k = i;
            }

            if (k != -1)
                vertexes.RemoveAt(k);
        }

        public Tuple<int, List<Vertex>> DijkstraAlgorim(Vertex start, Vertex end)
        {
            d = new Dictionary<Vertex, int> { };
            var used = new Dictionary<Vertex, bool> { };
            int end_pos = -1;

            for (int i = 0; i < vertexes.Count; i++)
            {
                d[vertexes[i]] = Int32.MaxValue;
                used[vertexes[i]] = false;

                if (vertexes[i] == end)
                {
                    end_pos = i;
                    end = vertexes[i];
                }else if (vertexes[i] == start)
                    start = vertexes[i];
            }

            if (end_pos == -1)
                return new Tuple<int, List<Vertex>>(-1, new List<Vertex> { });

            d[start] = 0;

            for (int i = 0; i < vertexes.Count && d[end] == Int32.MaxValue; i++)
            {
                remembered_pos = end_pos;
                for (int j = 0; j < vertexes.Count; j++)
                {
                    if (!used[vertexes[j]] && d[vertexes[j]] < d[vertexes[remembered_pos]])
                    {
                        remembered_pos = j;
                    }
                }

                if (d[vertexes[remembered_pos]] == Int32.MaxValue)
                    break;

                used[vertexes[remembered_pos]] = true;

                foreach (Tuple<Vertex, int> _neighbour in vertexes[remembered_pos].neighbours)
                {
                    if (d[vertexes[remembered_pos]] + _neighbour.Item2 < d[_neighbour.Item1])
                        d[_neighbour.Item1] = d[vertexes[remembered_pos]] + _neighbour.Item2;
                }
            }

            if (d[end] == Int32.MaxValue)
                return new Tuple<int, List<Vertex>>(-1, new List<Vertex> { });

            Tuple<int, List<Vertex>> res = new Tuple<int, List<Vertex>>(d[end], new List<Vertex> { });
            
            res.Item2.Add(end);

            Vertex current = end;
            while (current != start)
            {
                foreach(Tuple<Vertex, int> _neighbour in current.neighbours)
                {
                    if (d[_neighbour.Item1] != Int32.MaxValue && d[current] - d[_neighbour.Item1] == _neighbour.Item2)
                    {
                        current = _neighbour.Item1;
                        break;
                    }
                }
                res.Item2.Add(current);
            }

            return res;
        }
    }

    public class Vertex
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public CellTypes type { get; private set; }
        public List<Tuple<Vertex, int>> neighbours { get; private set; }

        public Vertex(int X, int Y, CellTypes type)
        {
            this.X = X;
            this.Y = Y;
            this.type = type;

            this.neighbours = new List<Tuple<Vertex, int>> { };
        }

        public static bool operator==(Vertex first, Vertex second)
        {
            if (first.X == second.X && first.Y == second.Y && first.type == second.type)
                return true;
            return false;
        }

        public static bool operator !=(Vertex first, Vertex second)
        {
            return !(first == second);
        }
    
        public void addNeighbour(Vertex _vertex, int dist)
        {
            neighbours.Add(new Tuple<Vertex, int>(_vertex, dist));
        }

        public void deleteNeighbour(Vertex neighbour)
        {
            int pos = -1;

            for (int i = 0; i < neighbours.Count(); i++)
            {
                if (neighbours[i].Item1 == neighbour)
                {
                    pos = i;
                    break;
                }
            }
            if (pos != -1)
                neighbours.RemoveAt(pos);
        }

        public void deleteNeighbour(Vector2 pos)
        {
            int index = -1;

            for (int i = 0; i < neighbours.Count(); i++)
            {
                if (neighbours[i].Item1.X == pos.X && neighbours[i].Item1.Y == pos.Y)
                {
                    index = i;
                    break;
                }
            }
            if (index != -1)
                neighbours.RemoveAt(index);
        }
    }

    public class Road
    {
        public int cost { get; private set; }
        public int distance { get; private set; }
        public int time { get; private set; }
        public CellTypes type { get; private set; }

        public Vertex first { get; private set; }
        public Vertex second { get; private set; }

        public Road(Vertex _first, Vertex _second, CellTypes _type)
        {
            first = _first;
            second = _second;
            type = _type;
        }

        public void setDistance(int dist)
        {
            distance = dist;

            switch (type)
            {
                case CellTypes.cell_road_car:
                    cost = distance * (int)costEnum.car;
                    time = distance * (int)timeEnum.car;
                    break;
                case CellTypes.cell_road_train:
                    cost = distance * (int)costEnum.train;
                    time = distance * (int)timeEnum.train;
                    break;
                case CellTypes.cell_road_air:
                    cost = distance * (int)costEnum.airplane;
                    time = distance * (int)timeEnum.airplane;
                    break;
            }
        }

        public bool Equal(Vertex _first, Vertex _second, CellTypes _type)
        {
            if ((first == _first && second == _second && type == _type) || (first == _second && second == _first && type == _type))
            {
                return true;
            }
            return false;
        }
    }
}
