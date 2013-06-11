using Daggerfall.Editor;
using Gtk;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Daggerfall.Player
{
    public class DaggerfallGame : Game
    {
        #region Fields

        const bool StartFullScreen = false;

        int currentTool;
        string path;

        #endregion Fields

        #region Properties

        public readonly List<Tool> Tools = new List<Tool>();

        public int CurrentTool
        {
            get { return currentTool; }
            set
            {
                value = value.Wrap(Tools.Count);
                if (currentTool != value)
                {
                    Tools[currentTool].Enabled = Tools[currentTool].Visible = false;
                    Tools[value].Enabled = Tools[value].Visible = true;
                    currentTool = value;
                }
            }
        }

        public SpriteBatch Batch { get; private set; }
        public SpriteFont Font { get; private set; }
        public State State { get; private set; }
        public BasicEffect Effect { get; private set; }
        public ViewerComponent Viewer { get; private set; }

        public ModelArchive Models { get { return State.Models; } }
        public RegionArchive Regions { get { return State.Regions; } }

        #endregion Properties

        GraphicsDeviceManager manager;

        public DaggerfallGame(string path)
        {
            this.path = path;

            manager = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = StartFullScreen ? 1 : GraphicsDeviceManager.DefaultBackBufferWidth,
                PreferredBackBufferHeight = StartFullScreen ? 1 : GraphicsDeviceManager.DefaultBackBufferHeight,
                PreferMultiSampling = true,
                SynchronizeWithVerticalRetrace = true,
                IsFullScreen = StartFullScreen,
            };

            Components.Add(Viewer = new FirstPersonPlayerComponent(this)
            {
                MouseEnabled = true,
                FarPlaneDistance = 1 << 20,
                NearPlaneDistance = 1 << 8,
                Enabled = true,
                WalkSpeed = 1 << 10,
                FlyUpDirection = new Vector3(0, 1 << 10, 0),
            });

            new ModelBrowserTool(this);
            new BlockBrowserTool(this);
            new TextureBrowserTool(this);
            Tools[0].Enabled = Tools[0].Visible = true;
        }

        protected override void LoadContent()
        {
            Content = new ResourceContentManager(Services, Properties.Resources.ResourceManager);
            base.LoadContent();
            State = new State(GraphicsDevice, path + "\\");
            Effect = new BasicEffect(GraphicsDevice);
            var block = State.Blocks.Records[0].Archive;
            var models = State.Models;

            Font = Content.Load<SpriteFont>("UIFont");
            Batch = new SpriteBatch(GraphicsDevice);

            manager.IsFullScreen = StartFullScreen;
        }

        TimeSpan lastPrevious, lastNext;

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            var keyboard = Keyboard.GetState();

            CurrentTool += keyboard.CheckNextPreviousKeys(Keys.End, Keys.Home, Tool.KeyRepeatTime, gameTime, ref lastNext, ref lastPrevious) * 1;
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
