using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace app.logics.graph
{
    public class Vertex
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public CellTypes type { get; private set; }
        public List<Neighbour> neighbours { get; private set; }

        public Vertex(int X, int Y, CellTypes type)
        {
            this.X = X;
            this.Y = Y;
            this.type = type;

            this.neighbours = new List<Neighbour> { };
        }

        public static bool operator ==(Vertex first, Vertex second)
        {
            if (first.X == second.X && first.Y == second.Y && first.type == second.type)
                return true;
            return false;
        }

        public static bool operator !=(Vertex first, Vertex second)
        {
            return !(first == second);
        }

        public override bool Equals(object obj)
        {
            return (Vertex)obj == this;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool isPosEqual(Vector2 pos)
        {
            if (X == pos.X && Y == pos.Y)
            {
                return true;
            }

            return false;
        }

        public bool isPosEqual(Vertex _vertex)
        {
            if (X == _vertex.X && Y == _vertex.Y)
            {
                return true;
            }

            return false;
        }

        public void addNeighbour(Vertex _vertex, int dist, CellTypes type)
        {
            neighbours.Add(new Neighbour(_vertex, dist, type));
        }

        public void addNeighbour(Vertex _vertex, int dist, int cost, int time)
        {
            neighbours.Add(new Neighbour(_vertex, dist, cost, time));
        }

        public void deleteNeighbour(Vertex neighbour)
        {
            int pos = -1;

            for (int i = 0; i < neighbours.Count(); i++)
            {
                if (neighbours[i].neighbour == neighbour)
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
                if (neighbours[i].neighbour.isPosEqual(pos))
                {
                    index = i;
                    break;
                }
            }
            if (index != -1)
                neighbours.RemoveAt(index);
        }

        public void setNeighbourDistance(Vertex _vertex, int dist, CellTypes type)
        {
            for (int i = 0; i < neighbours.Count; i++)
            {
                if (neighbours[i].neighbour == _vertex)
                {
                    neighbours[i].setDistance(dist, type);
                }
            }
        }
    }

    public class Neighbour
    {
        public int dist { get; set; }
        public int cost { get; set; }
        public int time { get; set; }
        public Vertex neighbour { get; set; }

        public Neighbour(Vertex _vertex, int dist, CellTypes type)
        {
            neighbour = _vertex;
            setDistance(dist, type);
        }

        public Neighbour(Vertex _vertex, int dist, int cost, int time)
        {
            neighbour = _vertex;
            this.dist = dist;
            this.cost = cost;
            this.time = time;
        }

        public void setDistance(int dist, CellTypes type)
        {
            this.dist = dist;
            switch (type)
            {
                case CellTypes.cell_road_car:
                    cost = this.dist * (int)costEnum.car;
                    time = this.dist * (int)timeEnum.car;
                    break;
                case CellTypes.cell_road_train:
                    cost = this.dist * (int)costEnum.train;
                    time = this.dist * (int)timeEnum.train;
                    break;
                case CellTypes.cell_road_air:
                    cost = this.dist * (int)costEnum.airplane;
                    time = this.dist * (int)timeEnum.airplane;
                    break;
            }
        }
    }
}
