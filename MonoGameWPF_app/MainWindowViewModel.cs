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
        private CellTypes hoveredCellType;
        private static Texture2D _texture;

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
            cell_road_air
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

            hoveredCellType = CellTypes.cell_base;
        }

        public override void Update(GameTime gameTime)
        {
            isKeyDown();

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

        public override void setMapSize(Vector2 size)
        {
            mapSize.X = size.X;
            mapSize.Y = size.Y;

            Map = new CellTypes[(int)mapSize.Y, (int)mapSize.X, 4];
            for (int i = 0; i < mapSize.Y; i++)
            {
                for (int j = 0; j < mapSize.X; j++)
                {
                    Map[i, j, 0] = CellTypes.cell_ground_basic;
                    Map[i, j, 1] = CellTypes.cell_none;
                    Map[i, j, 2] = CellTypes.cell_none;
                    Map[i, j, 3] = CellTypes.cell_none;
                }
            }

            Map[0, 0, 1] = CellTypes.cell_airport;
        }

        public override void setSize(Vector2 S)
        {
            thisSize.X = S.X;
            thisSize.Y = S.Y;

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
    }
}