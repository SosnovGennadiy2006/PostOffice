using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Routing.MonoGameControls;
using Keyboard = System.Windows.Input.Keyboard;

namespace Routing
{
    public class MainWindowViewModel : MonoGameViewModel
    {
        private SpriteBatch _spriteBatch;

        private Vector2 offsetPos;
        private Vector2 cellSize;
        private Vector2 mapSize;
        private static Texture2D _texture;

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

            offsetPos.X = 0;
            offsetPos.Y = 0;
            cellSize.X = 30;
            cellSize.Y = 30;
            mapSize.X = 30;
            mapSize.Y = 30;
        }

        public override void Update(GameTime gameTime)
        {
            if (Keyboard.IsKeyDown(Key.W) || Keyboard.IsKeyDown(Key.Up))
            {
                if (this.offsetPos.Y > Math.Min(0, -mapSize.Y * cellSize.Y + SystemParameters.FullPrimaryScreenHeight))
                {
                    this.offsetPos.Y -= 2;
                }
            }
            if (Keyboard.IsKeyDown(Key.D) || Keyboard.IsKeyDown(Key.Right))
            {
                if (this.offsetPos.X < Math.Max(0, -mapSize.X * cellSize.X + SystemParameters.FullPrimaryScreenWidth / 2))
                {
                    this.offsetPos.X += 2;
                }
            }
            if (Keyboard.IsKeyDown(Key.S) || Keyboard.IsKeyDown(Key.Down))
            {
                if (this.offsetPos.Y < Math.Max(0, -mapSize.Y * cellSize.Y + SystemParameters.FullPrimaryScreenHeight))
                {
                    this.offsetPos.Y += 2;
                }
            }
            if (Keyboard.IsKeyDown(Key.A) || Keyboard.IsKeyDown(Key.Left))
            {
                if (this.offsetPos.X > Math.Min(0, -mapSize.X * cellSize.X + SystemParameters.FullPrimaryScreenWidth / 2))
                {
                    this.offsetPos.X -= 2;
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            
            _spriteBatch.Begin();
            DrawGrid();
            _spriteBatch.End();
        }

        public void DrawGrid()
        {
            double WindowHeight = SystemParameters.FullPrimaryScreenWidth;
            double WindowWidth = SystemParameters.FullPrimaryScreenWidth;
            
            for (int i = 0; i < mapSize.X + 1; i++)
            {
                DrawLine(_spriteBatch, new Vector2(offsetPos.X + i * cellSize.X, offsetPos.Y), new Vector2(offsetPos.X + i * cellSize.X, mapSize.Y * cellSize.Y + offsetPos.Y), Color.LightGray);
            }
            for (int i = 0; i < mapSize.Y + 1; i++)
            {
                DrawLine(_spriteBatch, new Vector2(offsetPos.X, offsetPos.Y + i * cellSize.Y), new Vector2(mapSize.X * cellSize.X + offsetPos.X, offsetPos.Y + i * cellSize.Y), Color.LightGray);
            }
            
            DrawLine(_spriteBatch, new Vector2(0, 1), new Vector2((float)WindowWidth, 1), Color.Gray);
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

        public override void setMapSize(Vector2 size)
        {
            mapSize.X = size.X;
            mapSize.Y = size.Y;
        }
    }
}