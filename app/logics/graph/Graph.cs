using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace app.logics.graph
{
    public class Graph
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
                {
                    roads[i].setDistance(dist);
                }
            }

            for (int i = 0; i < vertexes.Count; i++)
            {
                if (vertexes[i].isPosEqual(_first))
                    vertexes[i].setNeighbourDistance(_second, dist, type);
                if (vertexes[i].isPosEqual(_second))
                    vertexes[i].setNeighbourDistance(_first, dist, type);
            }
        }

        public void addRoad(Vertex _first, Vertex _second, CellTypes type, int dist)
        {
            Road road = new Road(_first, _second, type);
            road.setDistance(dist);
            roads.Add(road);

            for (int i = 0; i < vertexes.Count; i++)
            {
                if (vertexes[i].isPosEqual(_first))
                    vertexes[i].addNeighbour(_second, road.distance, type);
                if (vertexes[i].isPosEqual(_second))
                    vertexes[i].addNeighbour(_first, road.distance, type);
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

        public Vertex getVertex(Vertex _vertex)
        {
            for (int i = 0; i < vertexes.Count; i++)
            {
                if (vertexes[i] == _vertex)
                    return vertexes[i];
            }

            throw new Exception("There isn't vertex in the graph!");
        }

        public Vertex getVertexByPos(Vertex _vertex)
        {
            for (int i = 0; i < vertexes.Count; i++)
            {
                if (vertexes[i].isPosEqual(_vertex))
                    return vertexes[i];
            }

            throw new Exception("There isn't vertex in the graph!");
        }

        public PathInfo DijkstraAlgorim(Vertex start, Vertex end)
        {
            DijkstraDictionary d = new DijkstraDictionary { };

            int endPos = -1;

            for (int i = 0; i < vertexes.Count; i++)
            {
                d.AddKey(vertexes[i]);
                if (vertexes[i].isPosEqual(end))
                {
                    endPos = i;
                }
            }

            if (endPos == -1)
                throw new Exception("There isn't end vertex in graph!");

            d[start].label = 0;
            int remembered_pos;

            for (int i = 0; i < vertexes.Count && d[end].label == Int32.MaxValue; i++)
            {
                remembered_pos = endPos;
                for (int j = 0; j < vertexes.Count; j++)
                {
                    if (!d[vertexes[j]].used && d[vertexes[j]].label < d[vertexes[remembered_pos]].label)
                    {
                        remembered_pos = j;
                    }
                }

                if (d[vertexes[remembered_pos]].label == Int32.MaxValue)
                    break;

                d[vertexes[remembered_pos]].used = true;

                foreach (Neighbour _neighbour in vertexes[remembered_pos].neighbours)
                {
                    if (d[vertexes[remembered_pos]].label + _neighbour.dist < d[_neighbour.neighbour].label)
                        d[_neighbour.neighbour].label = d[vertexes[remembered_pos]].label + _neighbour.dist;
                }
            }

            if (d[end].label == Int32.MaxValue)
                throw new Exception("End vertex is in another connected component!");

            PathInfo res = new PathInfo();

            res.addPoint(ref end);
            int cost, distance, time;

            Vertex current = getVertexByPos(end);
            while (current != start)
            {
                cost = 0;
                distance = 0;
                time = 0;

                foreach (Neighbour _neighbour in current.neighbours)
                {
                    if (d[_neighbour.neighbour].label != Int32.MaxValue && d[current].label - d[_neighbour.neighbour].label == _neighbour.dist)
                    {
                        current = getVertexByPos(_neighbour.neighbour);

                        distance = _neighbour.dist;
                        cost = _neighbour.cost;
                        time = _neighbour.time;

                        break;
                    }
                }

                res.addPoint(ref current);
                res.totalDistance += distance;
                res.totalCost += cost;
                res.totalTime += time;
            }

            res.totalDistance -= res.path.Count - 2;

            // Optional
            res.Reverse();

            return res;
        }
    }

}
