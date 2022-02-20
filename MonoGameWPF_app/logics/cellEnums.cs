namespace Routing.logics
{
    public enum CellTypes
    {
        cell_none,
        cell_base,
        cell_train_station,
        cell_airport,
        cell_ground_basic,
        cell_ground_water,
        cell_road_car,
        cell_road_train,
        cell_road_air,
        cell_focus
    }

    public enum CellIndexes
    {
        ground_type = 0,
        building_type,
        isCarRoad,
        isRailway,
        isAirRoad,
        isFocus
    }
}
