using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app.logics
{
    class Graph
    {
        public List<Vertex> vertexes { get; private set; }
        public List<Road> roads { get; private set; }

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
            roads.Add(new Road(_first, _second, type));
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

            for (int i = (int)roadsToDelete.Count - 1; i >= 0; i--)
            {
                roads.RemoveAt(roadsToDelete[i]);
            }
        }
    }

    class Vertex
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public CellTypes type { get; private set; }

        public Vertex(int X, int Y, CellTypes type)
        {
            this.X = X;
            this.Y = Y;
            this.type = type;
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
    }

    class Road
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
