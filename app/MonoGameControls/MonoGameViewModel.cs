using System;
using System.Windows;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using app.logics;
using CellTypes = app.logics.CellTypes;

namespace app.MonoGameControls
{
    public interface IMonoGameViewModel : IDisposable
    {
        IGraphicsDeviceService GraphicsDeviceService { get; set; }

        void Initialize();
        void LoadContent();
        void UnloadContent();
        void Update(GameTime gameTime);
        void Draw(GameTime gameTime);
        void OnActivated(object sender, EventArgs args);
        void OnDeactivated(object sender, EventArgs args);
        void OnExiting(object sender, EventArgs args);

        void SizeChanged(object sender, SizeChangedEventArgs args);

        void setMapSize(Vector2 size);
        void clearAll();
        void setSize(Vector2 size);
        void setMousePos(Vector2 pos);
        void changeMousePressState(bool state);
        void changeHoveredCellType(CellTypes type);
        void setSelectedRoadType(CellTypes type);
        void setOperationState(bool state);

        Tuple<int, List<Vector2>> getPath(Vector2 start, Vector2 end);

        event SelectEventHandler SelectReached;
        void OnSelectReached(MapSelectedEventArgs e);
    }

    public class MonoGameViewModel : ViewModel, IMonoGameViewModel
    {
        public event SelectEventHandler SelectReached;

        public MonoGameViewModel()
        {
        }

        public void Dispose()
        {
            Content?.Dispose();
        }

        public IGraphicsDeviceService GraphicsDeviceService { get; set; }
        protected GraphicsDevice GraphicsDevice => GraphicsDeviceService?.GraphicsDevice;
        protected MonoGameServiceProvider Services { get; private set; }
        protected ContentManager Content { get; set; }

        public virtual void Initialize()
        {
            Services = new MonoGameServiceProvider();
            Services.AddService(GraphicsDeviceService);
            Content = new ContentManager(Services) { RootDirectory = "Content" };
        }

        public virtual void LoadContent() { }
        public virtual void UnloadContent() { }
        public virtual void Update(GameTime gameTime) { }
        public virtual void Draw(GameTime gameTime) { }
        public virtual void OnActivated(object sender, EventArgs args) { }
        public virtual void OnDeactivated(object sender, EventArgs args) { }
        public virtual void OnExiting(object sender, EventArgs args) { }
        public virtual void SizeChanged(object sender, SizeChangedEventArgs args) { }

        public virtual void setMapSize(Vector2 size) { }
        public virtual void clearAll() { }
        public virtual void setSize(Vector2 size) { }
        public virtual void setMousePos(Vector2 pos) { }
        public virtual void changeMousePressState(bool state) { }
        public virtual void changeHoveredCellType(CellTypes type) { }
        public virtual void setSelectedRoadType(CellTypes type) { }
        public virtual void setOperationState(bool state) { }


        public virtual Tuple<int, List<Vector2>> getPath(Vector2 start, Vector2 end) { return null; }

        public virtual void OnSelectReached(MapSelectedEventArgs e)
        {
            SelectEventHandler handler = SelectReached;
            handler?.Invoke(this, e);
        }
    }
}
