using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Routing.MonoGameControls;

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
            mapSize.X = 100;
            mapSize.Y = 100;
        }

        public override void Update(GameTime gameTime)
        {

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
            int WindowHeight = 1000;
            int WindowWidth = 1000;

            DrawLine(_spriteBatch, new Vector2(1, 1), new Vector2(1000, 1), Color.Gray);
            DrawLine(_spriteBatch, new Vector2(1, 1), new Vector2(1, 1000), Color.Gray);
            DrawLine(_spriteBatch, new Vector2(1000, 1), new Vector2(1000, 1000), Color.Gray);
            DrawLine(_spriteBatch, new Vector2(1, 1000), new Vector2(1000, 1000), Color.Gray);

            for (int i = 0; i < mapSize.X + 1 - offsetPos.X % cellSize.X || i < WindowWidth / cellSize.X; i++)
            {
                float a = offsetPos.X % cellSize.X + i * cellSize.X;
                DrawLine(_spriteBatch, new Vector2(offsetPos.X % cellSize.X + i * cellSize.X, 0), new Vector2(offsetPos.X % cellSize.X + i * cellSize.X, WindowHeight), Color.LightGray);
            }
            for (int i = 0; i < mapSize.Y + 1 - offsetPos.Y % cellSize.Y || i < WindowHeight / cellSize.Y; i++)
            {
                DrawLine(_spriteBatch, new Vector2(0, offsetPos.Y % cellSize.Y + i * cellSize.Y), new Vector2(WindowWidth, offsetPos.Y % cellSize.Y + i * cellSize.Y), Color.LightGray);
            }
        }

        public static void DrawLine(SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color, float thickness = 1f)
        {
            var distance = Vector2.Distance(point1, point2);
            var angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            DrawLine(spriteBatch, point1, distance, angle, color, thickness);
        }

        public static void DrawLine(SpriteBatch spriteBatch, Vector2 point, float length, float angle, Color color, float thickness = 1f)
        {
            var origin = new Vector2(0f, 0.5f);
            var scale = new Vector2(length, thickness);
            spriteBatch.Draw(GetTexture(spriteBatch), point, null, color, angle, origin, scale, SpriteEffects.None, 0);
        }
    }
}