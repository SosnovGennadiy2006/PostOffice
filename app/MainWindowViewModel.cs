using System;
using System.Windows.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using app.MonoGameControls;
using app.logics;
using app.logics.graph;
using app.view;
using Keyboard = System.Windows.Input.Keyboard;
using CellTypes = app.logics.CellTypes;
using CellIndexes = app.logics.CellIndexes;
using CellSize = app.logics.CellSize;

namespace app
{
    public class MainWindowViewModel : MonoGameViewModel
    {
        private PaintEngine painter;

        private Vector2 thisSize;
        private Vector2 currentMousePos;
        private bool isMousePressed;
        private CellTypes selectedRoadType = CellTypes.cell_none;
        private bool isSelectPostOfficeOperation = false;

        private Vector2 rememberedCell = new Vector2(-1, -1);

        public override void LoadContent()
        {
            base.LoadContent();

            painter = new PaintEngine(GraphicsDevice, Content);
            painter.setSize(thisSize);
        }

        public override void Update(GameTime gameTime)
        {
            isKeyDown();
            isMouseDown();
        }

        public void isKeyDown()
        {
            int dx = 0, dy = 0;

            if (Keyboard.IsKeyDown(Key.W) || Keyboard.IsKeyDown(Key.Up))
            {
                if (painter.offsetPos.Y > Math.Min(0, painter.map.Height * CellSize.Y + painter.Size.Y))
                {
                    dy = -2;
                }
            }
            if (Keyboard.IsKeyDown(Key.D) || Keyboard.IsKeyDown(Key.Right))
            {
                if (painter.offsetPos.X < Math.Max(0, -painter.map.Width * CellSize.X + painter.Size.X))
                {
                    dx = 2;
                }
            }
            if (Keyboard.IsKeyDown(Key.S) || Keyboard.IsKeyDown(Key.Down))
            {
                if (painter.offsetPos.Y < Math.Max(0, -painter.map.Height * CellSize.Y + painter.Size.Y))
                {
                    dy = 2;
                }
            }
            if (Keyboard.IsKeyDown(Key.A) || Keyboard.IsKeyDown(Key.Left))
            {
                if (painter.offsetPos.X > Math.Min(0, -painter.map.Width * CellSize.X + painter.Size.X))
                {
                    dx = -2;
                }
            }

            setHoveredCell();
            painter.setOffsetPos(dx, dy);
        }

        public void isMouseDown()
        {
            if (isSelectPostOfficeOperation)
            {
                if (isMousePressed && painter.hoveredCell.X != -1 && painter.hoveredCell.Y != -1)
                {
                    if (painter.map[(int)painter.hoveredCell.X, (int)painter.hoveredCell.Y, CellIndexes.building_type] == CellTypes.cell_base)
                    {
                        MapSelectedEventArgs args = new MapSelectedEventArgs(painter.hoveredCell);

                        OnSelectReached(args);
                    }
                }
            }

            if (painter.hoveredCellType != CellTypes.cell_none)
            {
                if (isMousePressed && painter.hoveredCell.X != -1 && painter.hoveredCell.Y != -1)
                {
                    if (painter.map[(int)painter.hoveredCell.X, (int)painter.hoveredCell.Y, CellIndexes.building_type] != painter.hoveredCellType)
                    {
                        painter.setCellType(painter.hoveredCell, painter.hoveredCellType);
                        if (painter.map[(int)painter.hoveredCell.X, (int)painter.hoveredCell.Y, CellIndexes.building_type] != painter.hoveredCellType)
                            painter.map.DeleteRoads(painter.hoveredCell, painter.hoveredCellType);
                    }
                }
            }

            if (selectedRoadType != CellTypes.cell_none)
            {
                if (isMousePressed && painter.hoveredCell.X != -1 && painter.hoveredCell.Y != -1)
                {
                    bool flag = painter.map[(int)painter.hoveredCell.X, (int)painter.hoveredCell.Y, CellIndexes.building_type] == CellTypes.cell_airport &&
                                selectedRoadType == CellTypes.cell_road_air;

                    if (selectedRoadType == CellTypes.cell_road_car && painter.map[(int)painter.hoveredCell.X, (int)painter.hoveredCell.Y, CellIndexes.building_type] != CellTypes.cell_none)
                        flag = true;
                    if (painter.map[(int)painter.hoveredCell.X, (int)painter.hoveredCell.Y, CellIndexes.building_type] == CellTypes.cell_train_station &&
                        selectedRoadType == CellTypes.cell_road_train)
                        flag = true;

                    if (flag && painter.map[(int)painter.hoveredCell.X, (int)painter.hoveredCell.Y, CellIndexes.isFocus] != CellTypes.cell_focus && rememberedCell != painter.hoveredCell)
                    {
                        painter.setCellType(painter.hoveredCell, CellTypes.cell_focus);
                        if (painter.map.firstSelectedPos == new Vector2(-1, -1))
                        {
                            painter.map.firstSelectedPos = painter.hoveredCell;
                        } else if (painter.map.secondSelectedPos == new Vector2(-1, -1))
                        {
                            painter.map.secondSelectedPos = painter.hoveredCell;
                            makeRoad();
                            rememberedCell = painter.hoveredCell;
                        }
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            painter.Draw();
        }

        public void setHoveredCell()
        {
            if (currentMousePos != new Vector2(-1, -1))
            {
                Vector2 offsetMousePos = currentMousePos - painter.offsetPos;
                if (offsetMousePos.X >= 0 && offsetMousePos.X <= painter.map.Width * CellSize.X &&
                    offsetMousePos.Y >= 0 && offsetMousePos.Y <= painter.map.Height * CellSize.Y)
                {
                    painter.setHoveredCell(new Vector2(Math.Min((int)offsetMousePos.X / CellSize.X, painter.map.Width - 1),
                        Math.Min((int)offsetMousePos.Y / CellSize.Y, painter.map.Height - 1)));
                }
                else
                {
                    painter.setHoveredCell(new Vector2(-1, -1));
                }
            }
            else
            {
                painter.setHoveredCell(new Vector2(-1, -1));
            }
        }

        public override void setSize(Vector2 size)
        {
            thisSize = size;
            if (painter != null)
                painter.setSize(size);
        }
        
        public override void setMapSize(Vector2 size)
        {
            painter.clearAll();
            painter.resize_map((int)size.X, (int)size.Y);
        }

        public override void clearAll()
        {
            painter.clearAll();

            painter.hoveredCellType = CellTypes.cell_none;
            selectedRoadType = CellTypes.cell_none;
            isSelectPostOfficeOperation = false;
        }

        public override void setMousePos(Vector2 pos)
        {
            currentMousePos = pos;
            setHoveredCell();
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
            painter.hoveredCellType = type;
        }
        
        public override void setSelectedRoadType(CellTypes type)
        {
            selectedRoadType = type;
            if ((int)painter.map.firstSelectedPos.X != -1 && (int)painter.map.firstSelectedPos.Y != -1)
                painter.map[(int)painter.map.firstSelectedPos.X, (int)painter.map.firstSelectedPos.Y, CellIndexes.isFocus] = CellTypes.cell_none;
            if ((int)painter.map.secondSelectedPos.X != -1 && (int)painter.map.secondSelectedPos.Y != -1)
                painter.map[(int)painter.map.secondSelectedPos.X, (int)painter.map.secondSelectedPos.Y, CellIndexes.isFocus] = CellTypes.cell_none;
            painter.map.firstSelectedPos = new Vector2(-1, -1);
            painter.map.secondSelectedPos = new Vector2(-1, -1);
        }

        public override Tuple<errorCodes, PathInfo> getPath(Vector2 start, Vector2 end)
        {
            return painter.map.getPath(start, end);
        }

        public override CellTypes[,,] getMap()
        {
            return painter.map.map;
        }

        public override Vector2 getMapSize()
        {
            return new Vector2(painter.map.Width, painter.map.Height);
        }

        public override ref Graph getGraph()
        {
            return ref painter.map._graph;
        }

        public override void setMap(int mapWidth, int mapHeight, ref CellTypes[,,] map)
        {
            painter.map.setMap(mapWidth, mapHeight, ref map);
        }

        public override void setGraph(Graph newGraph)
        {
            painter.map.graph = newGraph;
        }

        public void makeRoad()
        {
            if (painter.map.checkRoad(painter.map.firstSelectedPos, painter.map.secondSelectedPos, selectedRoadType))
            {
                painter.map.addRoad(painter.map.firstSelectedPos, painter.map.secondSelectedPos, selectedRoadType);
            }

            painter.map[(int)painter.map.firstSelectedPos.X, (int)painter.map.firstSelectedPos.Y, CellIndexes.isFocus] = CellTypes.cell_none;
            painter.map[(int)painter.map.secondSelectedPos.X, (int)painter.map.secondSelectedPos.Y, CellIndexes.isFocus] = CellTypes.cell_none;

            painter.map.firstSelectedPos = new Vector2(-1, -1);
            painter.map.secondSelectedPos = new Vector2(-1, -1);
        }

        public override void setOperationState(bool state)
        {
            isSelectPostOfficeOperation = state;
        }
    }
}