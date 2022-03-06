using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using app.logics;
using CellSize = app.logics.CellSize;

namespace app.view
{
    class PaintEngine : SpriteBatch
    {
        private Vector2 _offsetPos;
        private Vector2 _thisSize;
        public Vector2 hoveredCell { get; private set; } = new Vector2(-1, -1);
        public CellTypes hoveredCellType { get; set; } = CellTypes.cell_none;
        public CellTypes selectedRoadType { get; set; } = CellTypes.cell_none;
        private static Texture2D _texture;

        private Map _map = new Map(10, 10);

        public Map map
        {
            get
            {
                return _map;
            }
            private set
            {
                _map = value;
            }
        }

        Texture2D baseTexture;
        Texture2D trainStationTexture;
        Texture2D airportTexture;

        public ContentManager Content;

        public static Color waterColor = new Color(28, 163, 236);

        public Vector2 offsetPos
        {
            get
            {
                return _offsetPos;
            }
            private set
            {
                _offsetPos = value;
            }
        }

        public Vector2 Size
        {
            get
            {
                return _thisSize;
            }
            private set
            {
                _thisSize = value;
            }
        }

        public PaintEngine(GraphicsDevice device, ContentManager content) : base(device)
        {
            offsetPos = new Vector2(0, 0);

            Content = content;

            Content.RootDirectory = "Content";
            baseTexture = Content.Load<Texture2D>("Images/office");
            trainStationTexture = Content.Load<Texture2D>("Images/trainStation");
            airportTexture = Content.Load<Texture2D>("Images/airport");
        }

        public void setCellType(Vector2 pos, CellTypes type)
        {
            _map.setCellType((int)pos.X, (int)pos.Y, type);
        }

        public void setOffsetPos(int dx, int dy)
        {
            _offsetPos.X += dx;
            _offsetPos.Y += dy;
        }

        public void setHoveredCell(Vector2 pos)
        {
            hoveredCell = pos;
        }

        public void setHoveredCellType(CellTypes type)
        {
            hoveredCellType = type;
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

        public void Draw()
        {
            Begin();

            DrawMap();
            DrawHoveredCell();
            DrawGrid();

            End();
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

        public void DrawGrid()
        {
            for (int i = 0; i < _map.Width + 1; i++)
            {
                DrawLine(this, new Vector2(offsetPos.X + i * CellSize.X, offsetPos.Y),
                    new Vector2(offsetPos.X + i * CellSize.X, _map.Height * CellSize.Y + offsetPos.Y),
                    Color.LightGray);
            }
            for (int i = 0; i < _map.Height + 1; i++)
            {
                DrawLine(this, new Vector2(offsetPos.X, offsetPos.Y + i * CellSize.Y),
                    new Vector2(_map.Width * CellSize.X + offsetPos.X, offsetPos.Y + i * CellSize.Y),
                    Color.LightGray);
            }

            DrawLine(this, new Vector2(0, 1), new Vector2(_thisSize.X, 1), Color.Gray);
        }

        public void setSize(Vector2 size)
        {
            _thisSize = size;

            if (_offsetPos.X + _map.Width * CellSize.X > _thisSize.X)
            {
                _offsetPos.X = _thisSize.X - _map.Width * CellSize.X;
            }
            if (offsetPos.X < -_map.Width * CellSize.X + _thisSize.X)
            {
                _offsetPos.X = 0;
            }
            if (offsetPos.Y + _map.Height * CellSize.Y > _thisSize.Y)
            {
                _offsetPos.Y = _thisSize.Y - _map.Height * CellSize.Y;
            }
            if (_offsetPos.Y < -_map.Height * CellSize.Y + _thisSize.Y)
            {
                _offsetPos.Y = 0;
            }
        }

        public void DrawMap()
        {
            for (int i = 0; i < _map.Width && i * CellSize.X + offsetPos.X < _thisSize.X; i++)
            {
                for (int j = 0; j < _map.Height && j * CellSize.Y + offsetPos.Y < _thisSize.Y; j++)
                {
                    DrawCell(new Vector2(i * CellSize.X + offsetPos.X, j * CellSize.Y + offsetPos.Y), _map[i, j, CellIndexes.ground_type]);
                    DrawCell(new Vector2(i * CellSize.X + offsetPos.X, j * CellSize.Y + offsetPos.Y), _map[i, j, CellIndexes.isCarRoad]);
                    DrawCell(new Vector2(i * CellSize.X + offsetPos.X, j * CellSize.Y + offsetPos.Y), _map[i, j, CellIndexes.isRailway]);
                    DrawCell(new Vector2(i * CellSize.X + offsetPos.X, j * CellSize.Y + offsetPos.Y), _map[i, j, CellIndexes.isAirRoad]);
                    DrawCell(new Vector2(i * CellSize.X + offsetPos.X, j * CellSize.Y + offsetPos.Y), _map[i, j, CellIndexes.building_type]);
                    DrawCell(new Vector2(i * CellSize.X + offsetPos.X, j * CellSize.Y + offsetPos.Y), _map[i, j, CellIndexes.isFocus]);
                }
            }
        }

        public void DrawHoveredCell()
        {
            if (hoveredCell.X != -1 && hoveredCell.Y != -1)
            {
                Vector2 pos = new Vector2(hoveredCell.X * CellSize.X + offsetPos.X, hoveredCell.Y * CellSize.Y + offsetPos.Y);
                if (hoveredCellType == CellTypes.cell_base || hoveredCellType == CellTypes.cell_train_station || hoveredCellType == CellTypes.cell_airport)
                    this.Draw(GetTexture(this), pos, null, Color.White, 0f, new Vector2(0f, 0f), 
                        new Vector2((float)CellSize.X, (float)CellSize.Y), SpriteEffects.None, 0);
                DrawCell(pos, hoveredCellType, 0.2f);
            }
        }

        public void DrawCell(Vector2 pos, CellTypes type, float opacity = 1)
        {
            switch (type)
            {
                case CellTypes.cell_base:
                    Draw(baseTexture, pos, null, Color.White * opacity);
                    break;
                case CellTypes.cell_train_station:
                    Draw(trainStationTexture, pos, null, Color.White * opacity);
                    break;
                case CellTypes.cell_airport:
                    Draw(airportTexture, pos, null, Color.White * opacity);
                    break;
                case CellTypes.cell_ground_water:
                    Draw(GetTexture(this), pos, null, waterColor * opacity, 0f, new Vector2(0f, 0f), CellSize.toVector(), SpriteEffects.None, 0);
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
            DrawLine(this, new Vector2(pos.X, pos.Y + 1), CellSize.X * 5 / 12, 0, Color.Red * opacity, 2);
            DrawLine(this, new Vector2(pos.X, pos.Y + CellSize.Y - 2), CellSize.X * 5 / 12, 0, Color.Red * opacity, 2);

            DrawLine(this, new Vector2(pos.X + 1, pos.Y), (CellSize.Y - 2) * 5 / 12, (float)(Math.PI / 2), Color.Red * opacity, 2);
            DrawLine(this, new Vector2(pos.X + CellSize.X - 2, pos.Y), CellSize.Y * 5 / 12, (float)(Math.PI / 2), Color.Red * opacity, 2);

            DrawLine(this, new Vector2(pos.X + 1, pos.Y + CellSize.Y * 7 / 12), (CellSize.Y - 2) * 5 / 12, (float)(Math.PI / 2), Color.Red * opacity, 2);
            DrawLine(this, new Vector2(pos.X + CellSize.X - 2, pos.Y + CellSize.Y * 7 / 12), (CellSize.Y - 2) * 5 / 12, (float)(Math.PI / 2), Color.Red * opacity, 2);

            DrawLine(this, new Vector2(pos.X + CellSize.X * 7 / 12, pos.Y + 1), CellSize.X * 5 / 12, 0, Color.Red * opacity, 2);
            DrawLine(this, new Vector2(pos.X + CellSize.X * 7 / 12, pos.Y + CellSize.Y - 2), CellSize.X * 5 / 12, 0, Color.Red * opacity, 2);
        }

        public void DrawRoadCell(Vector2 pos, CellTypes roadType, float opacity = 1)
        {
            Vector2 cellPos = new Vector2((pos.X - offsetPos.X) / CellSize.X, (pos.Y - offsetPos.Y) / CellSize.Y);
            Vector2 point = new Vector2(CellSize.X / 2, CellSize.Y / 2);
            Color color = Color.Black;
            int index = CellIndexes.ground_type;

            if (roadType == CellTypes.cell_road_car)
            {
                index = CellIndexes.isCarRoad;
                color = Color.Black;
            }
            else if (roadType == CellTypes.cell_road_train)
            {
                index = CellIndexes.isRailway;
                point.X -= 2;
                point.Y -= 2;
                color = Color.Orange;
            }
            else if (roadType == CellTypes.cell_road_air)
            {
                index = CellIndexes.isAirRoad;
                point.X += 2;
                point.Y += 2;
                color = Color.Aqua;
            }

            if (cellPos.X > 0)
            {
                if (_map[(int)cellPos.X - 1, (int)cellPos.Y, index] != CellTypes.cell_none)
                {
                    DrawLine(this, new Vector2(pos.X, pos.Y + point.Y), new Vector2(pos.X + point.X, pos.Y + point.Y), color * opacity, 2);
                }
            }
            if (cellPos.X < _map.Width - 1)
            {
                if (_map[(int)cellPos.X + 1, (int)cellPos.Y, index] != CellTypes.cell_none)
                {
                    DrawLine(this, new Vector2(pos.X + CellSize.X, pos.Y + point.Y), new Vector2(pos.X + point.X, pos.Y + point.Y), color * opacity, 2);
                }
            }
            if (cellPos.Y < _map.Height - 1)
            {
                if (_map[(int)cellPos.X, (int)cellPos.Y + 1, index] != CellTypes.cell_none)
                {
                    DrawLine(this, new Vector2(pos.X + point.X, pos.Y + CellSize.Y), new Vector2(pos.X + point.X, pos.Y + point.Y), color * opacity, 2);
                }
            }
            if (cellPos.Y > 0)
            {
                if (_map[(int)cellPos.X, (int)cellPos.Y - 1, index] != CellTypes.cell_none)
                {
                    DrawLine(this, new Vector2(pos.X + point.X, pos.Y), new Vector2(pos.X + point.X, pos.Y + point.Y), color * opacity, 2);
                }
            }
        }
    
        public void clearAll()
        {
            _map.clearAll();
        }

        public void resize_map(int w, int h)
        {
            _map.resize_map(w, h);
        }
    }
}
