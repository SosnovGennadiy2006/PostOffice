using System;
using System.Collections.Generic;
using app.logics.graph;
using Microsoft.Xna.Framework;

namespace app.logics
{
    /// <summary>
    /// Class for path info
    /// </summary>
    public class PathInfo
    {
        /// <summary>
        /// Path nodes - cells through which the path passes 
        /// </summary>
        public List<Vector2> path { get; private set; } = new List<Vector2>();
        
        /// <summary>
        /// Total distance
        /// </summary>
        public int totalDistance { get; set; } = 0;
        
        /// <summary>
        /// Total cost
        /// </summary>
        public int totalCost { get; set; } = 0;
        
        /// <summary>
        /// Total time
        /// </summary>
        public int totalTime { get; set; } = 0;

        /// <summary>
        /// The method that add cell to path
        /// </summary>
        /// <param name="point">New node (cell)</param>
        public void addPoint(ref Vertex point)
        {
            path.Add(new Vector2(point.X, point.Y));
        }

        /// <summary>
        /// Reverse
        /// </summary>
        public void Reverse()
        {
            path.Reverse();
        }

        /// <summary>
        /// First path node (cell)
        /// </summary>
        /// <returns>First node (cell)</returns>
        public Vector2 first()
        {
            return path[0];
        }

        /// <summary>
        /// Last path node (cell)
        /// </summary>
        /// <returns>Last node (cell)</returns>
        public Vector2 last()
        {
            return path[path.Count - 1];
        }
    
        /// <summary>
        /// Convert path node (cell) to string at pos i
        /// </summary>
        /// <param name="i">Index</param>
        /// <returns>Cell string</returns>
        public string toStringByIndex(int i)
        {
            return "(" + Convert.ToString(path[i].X + 1) + ", " + Convert.ToString(path[i].Y + 1) + ")";
        }
    }
}
