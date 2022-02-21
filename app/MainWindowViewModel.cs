using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using app.MonoGameControls;
using app.logics;
using Keyboard = System.Windows.Input.Keyboard;
using CellTypes = app.logics.CellTypes;
using CellIndexes = app.logics.CellIndexes;

namespace app
{
    public class MainWindowViewModel : MonoGameViewModel
    {
        private SpriteBatch _spriteBatch;

        private Vector2 offsetPos;
        private Vector2 cellSize;
        private Vector2 thisSize;
        private Vector2 currentMousePos;
        private Vector2 hoveredCell = new Vector2(-1, -1);
        private bool isMousePressed;
        private CellTypes hoveredCellType;
        private CellTypes selectedRoadType;
        private static Texture2D _texture;

        public static Color waterColor = new Color(28, 163, 236);

        private Vector2 rememberedCell = new Vector2(-1, -1);

        Texture2D baseTexture;
        Texture2D trainStationTexture;
        Texture2D airportTexture;

        private Map _map = new Map(10, 10);

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
            baseTexture = Content.Load<Texture2D>("Images/office");
            trainStationTexture = Content.Load<Texture2D>("Images/trainStation");
            airportTexture = Content.Load<Texture2D>("Images/airport");

            offsetPos.X = 0;
            offsetPos.Y = 0;
            cellSize.X = 32;
            cellSize.Y = 32;

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
                if (this.offsetPos.Y > Math.Min(0, _map.Height * cellSize.Y + thisSize.Y))
                {
                    this.offsetPos.Y -= 2;
                    setHoveredCell();
                }
            }
            if (Keyboard.IsKeyDown(Key.D) || Keyboard.IsKeyDown(Key.Right))
            {
                if (this.offsetPos.X < Math.Max(0, -_map.Width * cellSize.X + thisSize.X))
                {
                    this.offsetPos.X += 2;
                    setHoveredCell();
                }
            }
            if (Keyboard.IsKeyDown(Key.S) || Keyboard.IsKeyDown(Key.Down))
            {
                if (this.offsetPos.Y < Math.Max(0, -_map.Height * cellSize.Y + thisSize.Y))
                {
                    this.offsetPos.Y += 2;
                    setHoveredCell();
                }
            }
            if (Keyboard.IsKeyDown(Key.A) || Keyboard.IsKeyDown(Key.Left))
            {
                if (this.offsetPos.X > Math.Min(0, -_map.Width * cellSize.X + thisSize.X))
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
                    bool flag = (_map[(int)hoveredCell.X, (int)hoveredCell.Y, (int)CellIndexes.building_type] == CellTypes.cell_airport &&
                                selectedRoadType == CellTypes.cell_road_air);
                    
                    if (selectedRoadType == CellTypes.cell_road_car && _map[(int)hoveredCell.X, (int)hoveredCell.Y, (int)CellIndexes.building_type] != CellTypes.cell_none)
                        flag = true;
                    if (_map[(int)hoveredCell.X, (int)hoveredCell.Y, (int)CellIndexes.building_type] == CellTypes.cell_train_station &&
                        selectedRoadType == CellTypes.cell_road_train)
                        flag = true;
                    
                    if (flag && _map[(int)hoveredCell.X, (int)hoveredCell.Y, (int)CellIndexes.isFocus] != CellTypes.cell_focus && rememberedCell != hoveredCell)
                    {
                        setCellType(hoveredCell, CellTypes.cell_focus);
                        if (_map.firstSelectedPos == new Vector2(-1, -1))
                        {
                            _map.firstSelectedPos = hoveredCell;
                        }else if (_map.secondSelectedPos == new Vector2(-1, -1))
                        {
                            _map.secondSelectedPos = hoveredCell;
                            makeRoad();
                            rememberedCell = hoveredCell;
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
            for (int i = 0; i < _map.Width + 1; i++)
            {
                DrawLine(_spriteBatch, new Vector2(offsetPos.X + i * cellSize.X, offsetPos.Y), new Vector2(offsetPos.X + i * cellSize.X, _map.Height * cellSize.Y + offsetPos.Y), Color.LightGray);
            }
            for (int i = 0; i < _map.Height + 1; i++)
            {
                DrawLine(_spriteBatch, new Vector2(offsetPos.X, offsetPos.Y + i * cellSize.Y), new Vector2(_map.Width * cellSize.X + offsetPos.X, offsetPos.Y + i * cellSize.Y), Color.LightGray);
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
            for (int i = 0; i < _map.Width && i * cellSize.X + offsetPos.X < thisSize.X; i++)
            {
                for (int j = 0; j < _map.Height && j * cellSize.Y + offsetPos.Y < thisSize.Y; j++)
                {
                    DrawCell(new Vector2(i * cellSize.X + offsetPos.X, j * cellSize.Y + offsetPos.Y), _map[i, j, (int)CellIndexes.ground_type]);
                    DrawCell(new Vector2(i * cellSize.X + offsetPos.X, j * cellSize.Y + offsetPos.Y), _map[i, j, (int)CellIndexes.isCarRoad]);
                    DrawCell(new Vector2(i * cellSize.X + offsetPos.X, j * cellSize.Y + offsetPos.Y), _map[i, j, (int)CellIndexes.isRailway]);
                    DrawCell(new Vector2(i * cellSize.X + offsetPos.X, j * cellSize.Y + offsetPos.Y), _map[i, j, (int)CellIndexes.isAirRoad]);
                    DrawCell(new Vector2(i * cellSize.X + offsetPos.X, j * cellSize.Y + offsetPos.Y), _map[i, j, (int)CellIndexes.building_type]);
                    DrawCell(new Vector2(i * cellSize.X + offsetPos.X, j * cellSize.Y + offsetPos.Y), _map[i, j, (int)CellIndexes.isFocus]);
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
                    _spriteBatch.Draw(GetTexture(_spriteBatch), pos, null, waterColor * opacity, 0f, new Vector2(0f, 0f), cellSize, SpriteEffects.None, 0);
                    break;
                case CellTypes.cell_road_air:
                case CellTypes.cell_road_train:
                case CellTypes.cell_road_car:
                    {
                        DrawRoadCell(pos, type, opacity);
                        break;
                    }
                case CellTypes.cell_focus:
                    {
                        DrawFocusCell(pos, opacity);
                        break;
                    }
                case CellTypes.cell_ground_basic:
                case CellTypes.cell_none:
                default:
                    break;
            }
        }

        public void DrawFocusCell(Vector2 pos, float opacity = 1)
        {
            DrawLine(_spriteBatch, new Vector2(pos.X, pos.Y + 1), cellSize.X * 5 / 12, 0, Color.Red * opacity, 2);
            DrawLine(_spriteBatch, new Vector2(pos.X, pos.Y + cellSize.Y - 2), cellSize.X * 5 / 12, 0, Color.Red * opacity, 2);

            DrawLine(_spriteBatch, new Vector2(pos.X + 1, pos.Y), (cellSize.Y - 2) * 5 / 12, (float)(Math.PI / 2), Color.Red * opacity, 2);
            DrawLine(_spriteBatch, new Vector2(pos.X + cellSize.X - 2, pos.Y), cellSize.Y * 5 / 12, (float)(Math.PI / 2), Color.Red * opacity, 2);

            DrawLine(_spriteBatch, new Vector2(pos.X + 1, pos.Y + cellSize.Y * 7 / 12), (cellSize.Y - 2) * 5 / 12, (float)(Math.PI / 2), Color.Red * opacity, 2);
            DrawLine(_spriteBatch, new Vector2(pos.X + cellSize.X - 2, pos.Y + cellSize.Y * 7 / 12), (cellSize.Y - 2) * 5 / 12, (float)(Math.PI / 2), Color.Red * opacity, 2);

            DrawLine(_spriteBatch, new Vector2(pos.X + cellSize.X * 7 / 12, pos.Y + 1), cellSize.X * 5 / 12, 0, Color.Red * opacity, 2);
            DrawLine(_spriteBatch, new Vector2(pos.X + cellSize.X * 7 / 12, pos.Y + cellSize.Y - 2), cellSize.X * 5 / 12, 0, Color.Red * opacity, 2);
        }

        public void DrawRoadCell(Vector2 pos, CellTypes roadType, float opacity = 1)
        {
            Vector2 cellPos = new Vector2((pos.X - offsetPos.X) / cellSize.X, (pos.Y - offsetPos.Y) / cellSize.Y);
            Vector2 point = new Vector2(cellSize.X / 2, cellSize.Y / 2);
            Color color = Color.Black;
            int index = (int)CellIndexes.ground_type;

            if (roadType == CellTypes.cell_road_car)
            {
                index = (int)CellIndexes.isCarRoad;
                color = Color.Black;
            }else if (roadType == CellTypes.cell_road_train)
            {
                index = (int)CellIndexes.isRailway;
                point.X -= 2;
                point.Y -= 2;
                color = Color.Orange;
            }else if (roadType == CellTypes.cell_road_air)
            {
                index = (int)CellIndexes.isAirRoad;
                point.X += 2;
                point.Y += 2;
                color = Color.Aqua;
            }

            if (cellPos.X > 0)
            {
                if (_map[(int)cellPos.X - 1, (int)cellPos.Y, index] != CellTypes.cell_none)
                {
                    DrawLine(_spriteBatch, new Vector2(pos.X, pos.Y + point.Y), new Vector2(pos.X + point.X, pos.Y + point.Y), color * opacity, 2);
                }
            }
            if (cellPos.X < _map.Width - 1)
            {
                if (_map[(int)cellPos.X + 1, (int)cellPos.Y, index] != CellTypes.cell_none)
                {
                    DrawLine(_spriteBatch, new Vector2(pos.X + cellSize.X, pos.Y + point.Y), new Vector2(pos.X + point.X, pos.Y + point.Y), color * opacity, 2);
                }
            }
            if (cellPos.Y < _map.Height - 1)
            {
                if (_map[(int)cellPos.X, (int)cellPos.Y + 1, index] != CellTypes.cell_none)
                {
                    DrawLine(_spriteBatch, new Vector2(pos.X + point.X, pos.Y + cellSize.Y), new Vector2(pos.X + point.X, pos.Y + point.Y), color * opacity, 2);
                }
            }
            if (cellPos.Y > 0)
            {
                if (_map[(int)cellPos.X, (int)cellPos.Y - 1, index] != CellTypes.cell_none)
                {
                    DrawLine(_spriteBatch, new Vector2(pos.X + point.X, pos.Y), new Vector2(pos.X + point.X, pos.Y + point.Y), color * opacity, 2);
                }
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
            _map.setCellType((int)pos.X, (int)pos.Y, type);
            if (type == CellTypes.cell_ground_basic || type == CellTypes.cell_ground_water)
            {
                _map.DeleteRoads(pos);
            }
        }

        public override void setSize(Vector2 size)
        {
            thisSize.X = size.X;
            thisSize.Y = size.Y;

            if (offsetPos.X + _map.Width * cellSize.X > thisSize.X)
            {
                offsetPos.X = thisSize.X - _map.Width * cellSize.X;
            }
            if (offsetPos.X < -_map.Width * cellSize.X + thisSize.X)
            {
                offsetPos.X = 0;
            }
            if (offsetPos.Y + _map.Height * cellSize.Y > thisSize.Y)
            {
                offsetPos.Y = thisSize.Y - _map.Height * cellSize.Y;
            }
            if (offsetPos.Y < -_map.Height * cellSize.Y + thisSize.Y)
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
                if (offsetMousePos.X >= 0 && offsetMousePos.X <= _map.Width * cellSize.X && offsetMousePos.Y >= 0 && offsetMousePos.Y <= _map.Height * cellSize.Y)
                {
                    hoveredCell = new Vector2(Math.Min((int)offsetMousePos.X / (int)cellSize.X, _map.Width - 1), Math.Min((int)offsetMousePos.Y / (int)cellSize.Y, _map.Height - 1));
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
            if (!state)
            {
                rememberedCell = new Vector2(-1, -1);
            }
        }

        public override void changeHoveredCellType(CellTypes type)
        {
            hoveredCellType = type;
        }
        
        public override void setSelectedRoadType(CellTypes type)
        {
            selectedRoadType = type;
            if ((int)_map.firstSelectedPos.X != -1 && (int)_map.firstSelectedPos.Y != -1)
                _map[(int)_map.firstSelectedPos.X, (int)_map.firstSelectedPos.Y, (int)CellIndexes.isFocus] = CellTypes.cell_none;
            if ((int)_map.secondSelectedPos.X != -1 && (int)_map.secondSelectedPos.Y != -1)
                _map[(int)_map.secondSelectedPos.X, (int)_map.secondSelectedPos.Y, (int)CellIndexes.isFocus] = CellTypes.cell_none;
            _map.firstSelectedPos = new Vector2(-1, -1);
            _map.secondSelectedPos = new Vector2(-1, -1);
        }
    
        public void makeRoad()
        {
            if (_map.checkRoad(_map.firstSelectedPos, _map.secondSelectedPos, selectedRoadType))
            {
                _map.addRoad(_map.firstSelectedPos, _map.secondSelectedPos, selectedRoadType);
            }


            _map[(int)_map.firstSelectedPos.X, (int)_map.firstSelectedPos.Y, (int)CellIndexes.isFocus] = CellTypes.cell_none;
            _map[(int)_map.secondSelectedPos.X, (int)_map.secondSelectedPos.Y, (int)CellIndexes.isFocus] = CellTypes.cell_none;

            _map.firstSelectedPos = new Vector2(-1, -1);
            _map.secondSelectedPos = new Vector2(-1, -1);
        }
    }
}