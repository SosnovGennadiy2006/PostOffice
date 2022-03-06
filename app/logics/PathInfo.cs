using System;
using System.Collections.Generic;
using app.logics.graph;
using Microsoft.Xna.Framework;

namespace app.logics
{
    public class PathInfo
    {
        public List<Vector2> path { get; private set; } = new List<Vector2>();
        public int totalDistance { get; set; } = 0;
        public int totalCost { get; set; } = 0;
        public int totalTime { get; set; } = 0;

        public void addPoint(ref Vertex point)
        {
            path.Add(new Vector2(point.X, point.Y));
        }

        public void Reverse()
        {
            path.Reverse();
        }

        public Vector2 first()
        {
            return path[0];
        }

        public Vector2 last()
        {
            return path[path.Count - 1];
        }
    
        public string toStringByIndex(int i)
        {
            return "(" + Convert.ToString(path[i].X + 1) + ", " + Convert.ToString(path[i].Y + 1) + ")";
        }
    }
}
