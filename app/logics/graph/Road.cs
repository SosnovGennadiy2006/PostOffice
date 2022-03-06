namespace app.logics.graph
{
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
