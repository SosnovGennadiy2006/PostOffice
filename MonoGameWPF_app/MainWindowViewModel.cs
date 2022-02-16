using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Routing.MonoGameControls;
using Keyboard = System.Windows.Input.Keyboard;
using Mouse = System.Windows.Input.Mouse;

namespace Routing
{
    public class MainWindowViewModel : MonoGameViewModel
    {
        private SpriteBatch _spriteBatch;

        private Vector2 offsetPos;
        private Vector2 cellSize;
        private Vector2 mapSize;
        private Vector2 thisSize;
        private Vector2 currentMousePos;
        private Vector2 hoveredCell = new Vector2(-1, -1);
        private bool isMousePressed;
        private CellTypes hoveredCellType;
        private CellTypes selectedRoadType;
        private static Texture2D _texture;

        private Vector2 firstSelectedItemPos = new Vector2(-1, -1);
        private Vector2 secondSelectedItemPos = new Vector2(-1, -1);

        Texture2D baseTexture;
        Texture2D trainStationTexture;
        Texture2D airportTexture;

        private CellTypes[,,] Map;

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

        private static Texture2D GetTexture(SpriteBatch spriteBatch)
        {
            if (_texture == null)
            {
                _texture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                _texture.SetData(new[] { Color.White });
            }

            return _texture;
        }

        public override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Content.RootDirectory = "Content";
            baseTexture = Content.Load<Texture2D>("Images/Circle");
            trainStationTexture = Content.Load<Texture2D>("Images/Square");
            airportTexture = Content.Load<Texture2D>("Images/Rhomb");

            offsetPos.X = 0;
            offsetPos.Y = 0;
            cellSize.X = 30;
            cellSize.Y = 30;
            setMapSize(new Vector2(10, 10));

            hoveredCellType = CellTypes.cell_none;
        }

        public override void Update(GameTime gameTime)
        {
            isKeyDown();
            isMouseDown();
        }

        public void isKeyDown()
        {
            if (Keyboard.IsKeyDown(Key.W) || Keyboard.IsKeyDown(Key.Up))
            {
                if (this.offsetPos.Y > Math.Min(0, -mapSize.Y * cellSize.Y + thisSize.Y))
                {
                    this.offsetPos.Y -= 2;
                    setHoveredCell();
                }
            }
            if (Keyboard.IsKeyDown(Key.D) || Keyboard.IsKeyDown(Key.Right))
            {
                if (this.offsetPos.X < Math.Max(0, -mapSize.X * cellSize.X + thisSize.X))
                {
                    this.offsetPos.X += 2;
                    setHoveredCell();
                }
            }
            if (Keyboard.IsKeyDown(Key.S) || Keyboard.IsKeyDown(Key.Down))
            {
                if (this.offsetPos.Y < Math.Max(0, -mapSize.Y * cellSize.Y + thisSize.Y))
                {
                    this.offsetPos.Y += 2;
                    setHoveredCell();
                }
            }
            if (Keyboard.IsKeyDown(Key.A) || Keyboard.IsKeyDown(Key.Left))
            {
                if (this.offsetPos.X > Math.Min(0, -mapSize.X * cellSize.X + thisSize.X))
                {
                    this.offsetPos.X -= 2;
                    setHoveredCell();
                }
            }
        }

        public void isMouseDown()
        {
            if (hoveredCellType != CellTypes.cell_none)
            {
                if (isMousePressed && hoveredCell.X != -1 && hoveredCell.Y != -1)
                {
                    setCellType(hoveredCell, hoveredCellType);
                }
            }

            if (selectedRoadType != CellTypes.cell_none)
            {
                if (isMousePressed && hoveredCell.X != -1 && hoveredCell.Y != -1)
                {
                    bool flag = Map[(int) hoveredCell.X, (int) hoveredCell.Y, 1] == CellTypes.cell_airport &&
                                selectedRoadType == CellTypes.cell_road_air;
                    
                    if (selectedRoadType == CellTypes.cell_road_car)
                        flag = true;
                    if (Map[(int) hoveredCell.X, (int) hoveredCell.Y, 1] == CellTypes.cell_train_station &&
                        selectedRoadType == CellTypes.cell_road_train)
                        flag = true;
                    
                    if (flag && Map[(int) hoveredCell.X, (int) hoveredCell.Y, 4] != CellTypes.cell_focus)
                    {
                        setCellType(hoveredCell, CellTypes.cell_focus);
                        if (firstSelectedItemPos == new Vector2(-1, -1))
                        {
                            firstSelectedItemPos.X = hoveredCell.X;
                            firstSelectedItemPos.Y = hoveredCell.Y;
                        }else if (secondSelectedItemPos == new Vector2(-1, -1))
                        {
                            secondSelectedItemPos.X = hoveredCell.X;
                            secondSelectedItemPos.Y = hoveredCell.Y;
                        }
                        else
                        {
                            Map[(int) firstSelectedItemPos.X, (int) firstSelectedItemPos.Y, 4] = CellTypes.cell_none;
                            Map[(int) secondSelectedItemPos.X, (int) secondSelectedItemPos.Y, 4] = CellTypes.cell_none;
                            firstSelectedItemPos.X = hoveredCell.X;
                            firstSelectedItemPos.Y = hoveredCell.Y;
                            secondSelectedItemPos.X = -1;
                            secondSelectedItemPos.Y = -1;
                        }
                    }
                }
            }
        }
        
        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            
            _spriteBatch.Begin();
            DrawMap();
            DrawHoveredCell();
            DrawGrid();
            _spriteBatch.End();
        }

        public void DrawGrid()
        {            
            for (int i = 0; i < mapSize.X + 1; i++)
            {
                DrawLine(_spriteBatch, new Vector2(offsetPos.X + i * cellSize.X, offsetPos.Y), new Vector2(offsetPos.X + i * cellSize.X, mapSize.Y * cellSize.Y + offsetPos.Y), Color.LightGray);
            }
            for (int i = 0; i < mapSize.Y + 1; i++)
            {
                DrawLine(_spriteBatch, new Vector2(offsetPos.X, offsetPos.Y + i * cellSize.Y), new Vector2(mapSize.X * cellSize.X + offsetPos.X, offsetPos.Y + i * cellSize.Y), Color.LightGray);
            }
            
            DrawLine(_spriteBatch, new Vector2(0, 1), new Vector2((float)thisSize.X, 1), Color.Gray);
        }

        public static void DrawLine(SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color, float thickness = 1f)
        {
            var distance = Vector2.Distance(point1, point2);
            var angle = (float)System.Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            DrawLine(spriteBatch, point1, distance, angle, color, thickness);
        }

        public static void DrawLine(SpriteBatch spriteBatch, Vector2 point, float length, float angle, Color color, float thickness = 1f)
        {
            var origin = new Vector2(0f, 0.5f);
            var scale = new Vector2(length, thickness);
            spriteBatch.Draw(GetTexture(spriteBatch), point, null, color, angle, origin, scale, SpriteEffects.None, 0);
        }

        public void DrawMap()
        {
            for (int i = 0; i < mapSize.Y && i * cellSize.Y + offsetPos.Y < thisSize.Y; i++)
            {
                for (int j = 0; j < mapSize.X && j * cellSize.X + offsetPos.X < thisSize.X; j++)
                {
                    DrawCell(new Vector2(i * cellSize.X + offsetPos.X, j * cellSize.Y + offsetPos.Y), Map[i, j, 0]);
                    DrawCell(new Vector2(i * cellSize.X + offsetPos.X, j * cellSize.Y + offsetPos.Y), Map[i, j, 1]);
                    DrawCell(new Vector2(i * cellSize.X + offsetPos.X, j * cellSize.Y + offsetPos.Y), Map[i, j, 2]);
                    DrawCell(new Vector2(i * cellSize.X + offsetPos.X, j * cellSize.Y + offsetPos.Y), Map[i, j, 3]);
                    DrawCell(new Vector2(i * cellSize.X + offsetPos.X, j * cellSize.Y + offsetPos.Y), Map[i, j, 4]);
                }
            }
        }

        public void DrawCell(Vector2 pos, CellTypes type, float opacity = 1)
        {
            switch (type)
            {
                case CellTypes.cell_base:
                    _spriteBatch.Draw(baseTexture, pos, null, Color.White * opacity);
                    break;
                case CellTypes.cell_train_station:
                    _spriteBatch.Draw(trainStationTexture, pos, null, Color.White * opacity);
                    break;
                case CellTypes.cell_airport:
                    _spriteBatch.Draw(airportTexture, pos, null, Color.White * opacity);
                    break;
                case CellTypes.cell_ground_water:
                    _spriteBatch.Draw(GetTexture(_spriteBatch), pos, null, Color.Blue * opacity, 0f, new Vector2(0f, 0f), cellSize, SpriteEffects.None, 0);
                    break;
                case CellTypes.cell_focus:
                    DrawLine(_spriteBatch, new Vector2(pos.X, pos.Y + 1), cellSize.X * 5 / 12, 0, Color.Red * opacity, 2);
                    DrawLine(_spriteBatch, new Vector2(pos.X, pos.Y + cellSize.Y - 2), cellSize.X * 5 / 12, 0, Color.Red * opacity, 2);
                    
                    DrawLine(_spriteBatch, new Vector2(pos.X + 1, pos.Y), (cellSize.Y - 2) * 5 / 12, (float)(Math.PI / 2), Color.Red * opacity, 2);
                    DrawLine(_spriteBatch, new Vector2(pos.X + cellSize.X - 2, pos.Y), cellSize.Y * 5 / 12, (float)(Math.PI / 2), Color.Red * opacity, 2);
                    
                    DrawLine(_spriteBatch, new Vector2(pos.X + 1, pos.Y + cellSize.Y * 7 / 12), (cellSize.Y - 2) * 5 / 12, (float)(Math.PI / 2), Color.Red * opacity, 2);
                    DrawLine(_spriteBatch, new Vector2(pos.X + cellSize.X - 2, pos.Y + cellSize.Y * 7 / 12), (cellSize.Y - 2) * 5 / 12, (float)(Math.PI / 2), Color.Red * opacity, 2);
                    
                    DrawLine(_spriteBatch, new Vector2(pos.X + cellSize.X * 7 / 12, pos.Y + 1), cellSize.X * 5 / 12, 0, Color.Red * opacity, 2);
                    DrawLine(_spriteBatch, new Vector2(pos.X + cellSize.X * 7 / 12, pos.Y + cellSize.Y - 2), cellSize.X * 5 / 12, 0, Color.Red * opacity, 2);
                    break;
                case CellTypes.cell_ground_basic:
                case CellTypes.cell_none:
                default:
                    break;
            }
        }

        public void DrawHoveredCell()
        {
            if (hoveredCell.X != -1 && hoveredCell.Y != -1)
            {
                Vector2 pos = new Vector2(hoveredCell.X * cellSize.X + offsetPos.X, hoveredCell.Y * cellSize.Y + offsetPos.Y);
                if (hoveredCellType == CellTypes.cell_base || hoveredCellType == CellTypes.cell_train_station || hoveredCellType == CellTypes.cell_airport)
                    _spriteBatch.Draw(GetTexture(_spriteBatch), pos, null, Color.White, 0f, new Vector2(0f, 0f), cellSize, SpriteEffects.None, 0);
                DrawCell(pos, hoveredCellType, 0.2f);
            }
        }

        public void setCellType(Vector2 pos, CellTypes type)
        {
            switch (type)
            {
                case CellTypes.cell_base:
                    Map[(int) pos.X, (int) pos.Y, 0] = CellTypes.cell_ground_basic;
                    Map[(int) pos.X, (int) pos.Y, 1] = CellTypes.cell_base;
                    break;
                case CellTypes.cell_train_station:
                    Map[(int) pos.X, (int) pos.Y, 0] = CellTypes.cell_ground_basic;
                    Map[(int) pos.X, (int) pos.Y, 1] = CellTypes.cell_train_station;
                    break;
                case CellTypes.cell_airport:
                    Map[(int) pos.X, (int) pos.Y, 0] = CellTypes.cell_ground_basic;
                    Map[(int) pos.X, (int) pos.Y, 1] = CellTypes.cell_airport;
                    break;
                case CellTypes.cell_ground_basic:
                    Map[(int) pos.X, (int) pos.Y, 0] = CellTypes.cell_ground_basic;
                    Map[(int) pos.X, (int) pos.Y, 1] = CellTypes.cell_none;
                    break;
                case CellTypes.cell_ground_water:
                    Map[(int) pos.X, (int) pos.Y, 0] = CellTypes.cell_ground_water;
                    Map[(int) pos.X, (int) pos.Y, 1] = CellTypes.cell_none;
                    break;
                case CellTypes.cell_focus:
                    Map[(int) pos.X, (int) pos.Y, 4] = CellTypes.cell_focus;
                    break;
            }
        }

        public override void setMapSize(Vector2 size)
        {
            mapSize.X = size.X;
            mapSize.Y = size.Y;

            Map = new CellTypes[(int)mapSize.Y, (int)mapSize.X, 5];
            for (int i = 0; i < mapSize.Y; i++)
            {
                for (int j = 0; j < mapSize.X; j++)
                {
                    Map[i, j, 0] = CellTypes.cell_ground_basic;
                    Map[i, j, 1] = CellTypes.cell_none;
                    Map[i, j, 2] = CellTypes.cell_none;
                    Map[i, j, 3] = CellTypes.cell_none;
                    Map[i, j, 4] = CellTypes.cell_none;
                }
            }
        }

        public override void setSize(Vector2 size)
        {
            thisSize.X = size.X;
            thisSize.Y = size.Y;

            if (offsetPos.X + mapSize.X * cellSize.X > thisSize.X)
            {
                offsetPos.X = thisSize.X - mapSize.X * cellSize.X;
            }
            if (offsetPos.X < -mapSize.X * cellSize.X + thisSize.X)
            {
                offsetPos.X = 0;
            }
            if (offsetPos.Y + mapSize.Y * cellSize.Y > thisSize.Y)
            {
                offsetPos.Y = thisSize.Y - mapSize.Y * cellSize.Y;
            }
            if (offsetPos.Y < -mapSize.Y * cellSize.Y + thisSize.Y)
            {
                offsetPos.Y = 0;
            }
        }

        public override void setMousePos(Vector2 pos)
        {
            currentMousePos = pos;
            setHoveredCell();
        }

        public void setHoveredCell()
        {
            if (currentMousePos != new Vector2(-1, -1))
            {
                Vector2 offsetMousePos = currentMousePos - offsetPos;
                if (offsetMousePos.X >= 0 && offsetMousePos.X <= mapSize.X * cellSize.X && offsetMousePos.Y >= 0 && offsetMousePos.Y <= mapSize.Y * cellSize.Y)
                {
                    hoveredCell = new Vector2(Math.Min((int)offsetMousePos.X / (int)cellSize.X, mapSize.X - 1), Math.Min((int)offsetMousePos.Y / (int)cellSize.Y, mapSize.Y - 1));
                }
                else
                {
                    hoveredCell = new Vector2(-1, -1);
                }
            }else
            {
                hoveredCell = currentMousePos;
            }
        }

        public override void changeMousePressState(bool state)
        {
            isMousePressed = state;
        }

        public override void changeHoveredCellType(CellTypes type)
        {
            hoveredCellType = type;
        }
        
        public override void setSelectedRoadType(CellTypes type)
        {
            selectedRoadType = type;
            if ((int)firstSelectedItemPos.X != -1 && (int)firstSelectedItemPos.Y != -1)
                Map[(int)firstSelectedItemPos.X, (int)firstSelectedItemPos.Y, 4] = CellTypes.cell_none;
            if ((int)secondSelectedItemPos.X != -1 && (int)secondSelectedItemPos.Y != -1)
                Map[(int)secondSelectedItemPos.X, (int)secondSelectedItemPos.Y, 4] = CellTypes.cell_none;
            firstSelectedItemPos.X = -1;
            firstSelectedItemPos.Y = -1;
            secondSelectedItemPos.X = -1;
            secondSelectedItemPos.Y = -1;
        }
    }
}