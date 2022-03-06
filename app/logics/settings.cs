using System;
using System.Windows;
using Microsoft.Xna.Framework;

namespace app.logics
{
    public delegate void SelectEventHandler(object sender, MapSelectedEventArgs e);
    public delegate void RoutedSelectEventHandler(object sender, RoutedMapSelectedEventArgs e);

    public static class CellSize
    {
        public static int X = 32;
        public static int Y = 32;

        public static Vector2 toVector()
        {
            return new Vector2(X, Y);
        }
    }

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

    public static class CellIndexes
    {
        public static int ground_type = 0;
        public static int building_type = 1;
        public static int isCarRoad = 2;
        public static int isRailway = 3;
        public static int isAirRoad = 4;
        public static int isFocus = 5;
    }

    public enum costEnum
    {
        car = 1,
        train = 3,
        airplane = 7
    }

    public enum timeEnum
    {
        car = 5,
        train = 3,
        airplane = 1
    }

    public enum selectPointTypeEnum
    {
        None,
        Start,
        End
    }

    public class MapSelectedEventArgs : EventArgs
    {
        private readonly Vector2 postOfficePos;

        public MapSelectedEventArgs(Vector2 postOfficePos)
        {
            this.postOfficePos = postOfficePos;
        }

        public Vector2 Pos
        {
            get { return postOfficePos; }
        }
    }

    public class RoutedMapSelectedEventArgs : RoutedEventArgs
    {
        private readonly Vector2 postOfficePos;

        public RoutedMapSelectedEventArgs(RoutedEvent e, Vector2 postOfficePos) : base(e)
        {
            this.postOfficePos = postOfficePos;
        }

        public Vector2 Pos
        {
            get { return postOfficePos; }
        }
    }
}
