using System;
using System.Windows;
using Microsoft.Xna.Framework;


namespace app.logics
{
    /// <summary>
    /// Handler for select event
    /// It appears, when user select post office
    ///
    /// This is view model handler
    /// </summary>
    public delegate void SelectEventHandler(object sender, MapSelectedEventArgs e);
    
    /// <summary>
    /// Handler for select event
    /// It appears, when user select post office
    ///
    /// This is monogame control handler
    /// </summary>
    public delegate void RoutedSelectEventHandler(object sender, RoutedMapSelectedEventArgs e);

    /// <summary>
    /// Class for Cell Size
    /// </summary>
    public static class CellSize
    {
        // Cell Width
        public static int X = 32;
        // Cell Height
        public static int Y = 32;

        /// <summary>
        /// Function to convert 'CellSize' to 'Vector2'
        /// </summary>
        /// <returns></returns>
        public static Vector2 toVector()
        {
            return new Vector2(X, Y);
        }
    }

    /// <summary>
    /// All cell types
    /// </summary>
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

    /// <summary>
    /// Indexes for cell parameter in map cell
    /// </summary>
    public static class CellIndexes
    {
        public static int ground_type = 0;
        public static int building_type = 1;
        public static int isCarRoad = 2;
        public static int isRailway = 3;
        public static int isAirRoad = 4;
        public static int isFocus = 5;
    }

    /// <summary>
    /// Enum for cost by cell
    /// </summary>
    public enum costEnum
    {
        car = 1,
        train = 3,
        airplane = 7
    }

    /// <summary>
    /// Enum for time by cell
    /// </summary>
    public enum timeEnum
    {
        car = 5,
        train = 3,
        airplane = 1
    }

    /// <summary>
    /// Possible types for selected node (cell)
    /// </summary>
    public enum selectPointTypeEnum
    {
        None,
        Start,
        End
    }

    /// <summary>
    /// Possible errors
    /// </summary>
    public enum errorCodes
    {
        success,
        startDestinationIsNotSelectedError,
        endDestinationIsNotSelectedError,
        startVertexDoesntExistError,
        endVertexDoesntExistError,
        differentConnectivityComponentError
    }

    /// <summary>
    /// Class, that return error text for each error code
    /// </summary>
    public static class ErrorText
    {
        public static string getError(errorCodes code)
        {
            switch (code)
            {
                case errorCodes.startDestinationIsNotSelectedError:
                    return "The start destination isn't selected!";
                case errorCodes.endDestinationIsNotSelectedError:
                    return "The end destination isn't selected!";
                case errorCodes.startVertexDoesntExistError:
                    return "The start destination isn't post office!";
                case errorCodes.endVertexDoesntExistError:
                    return "The end destination isn't post office!";
                case errorCodes.differentConnectivityComponentError:
                    return "the start and end destination are in different \nconnectivity components!";
                default:
                    return "Undefined error!";
            }
        }
    }

    /// <summary>
    /// Event arguments for select event in view model
    /// </summary>
    public class MapSelectedEventArgs : EventArgs
    {
        // Selected post office position
        private readonly Vector2 postOfficePos;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="postOfficePos">Post office position</param>
        public MapSelectedEventArgs(Vector2 postOfficePos)
        {
            this.postOfficePos = postOfficePos;
        }

        /// <summary>
        /// Method to return post office position
        /// </summary>
        public Vector2 Pos
        {
            get { return postOfficePos; }
        }
    }

    /// <summary>
    /// Event arguments for select event in user control
    /// </summary>
    public class RoutedMapSelectedEventArgs : RoutedEventArgs
    {
        // Selected post office position
        private readonly Vector2 postOfficePos;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="e"></param>
        /// <param name="postOfficePos">Post office position</param>
        public RoutedMapSelectedEventArgs(RoutedEvent e, Vector2 postOfficePos) : base(e)
        {
            this.postOfficePos = postOfficePos;
        }

        /// <summary>
        /// Method to return post office position
        /// </summary>
        public Vector2 Pos
        {
            get { return postOfficePos; }
        }
    }
}
