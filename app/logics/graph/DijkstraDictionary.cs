using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app.logics.graph
{
    public class DijkstraDictionary
    {
        private List<Vertex> keys;
        private List<DijkstraVertexInfo> values;

        public DijkstraDictionary()
        {
            keys = new List<Vertex> { };
            values = new List<DijkstraVertexInfo> { };
        }

        public DijkstraDictionary(List<Vertex> _keys)
        {
            keys = new List<Vertex>(_keys);
            values = new List<DijkstraVertexInfo>(keys.Count);

            for (int i = 0; i < values.Count; i++)
            {
                values[i] = new DijkstraVertexInfo();
            }
        }

        public DijkstraVertexInfo this[int i]
        {
            get
            {
                return values[i];
            }
        }

        public DijkstraVertexInfo this[Vertex key]
        {
            get
            {
                int k = -1;

                for (int i = 0; i <keys.Count; i++)
                {
                    if (keys[i].isPosEqual(key))
                    {
                        k = i;
                        break;
                    }
                }

                if (k != -1)
                    return values[k];

                throw new ArgumentException("Key doesn't exist!");
            }
        }
    
        public void AddKey(Vertex key)
        {
            for (int i = 0; i < keys.Count; i++)
            {
                if (keys[i].isPosEqual(key))
                    return;
            }
            keys.Add(key);
            values.Add(new DijkstraVertexInfo());
        }
    }

    public class DijkstraVertexInfo
    {
        public int label { get; set; }
        public bool used { get; set; }

        public DijkstraVertexInfo()
        {
            label = Int32.MaxValue;
            used = false;
        }
    }
}
