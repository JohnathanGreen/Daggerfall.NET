using Daggerfall.Editor;
using Gtk;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Daggerfall.Player
{
    public class DaggerfallGame : Game
    {
        #region Fields

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
                value = Math.Max(0, Math.Min(Tools.Count - 1, value));
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

        static void Main(string[] args)
        {
            Application.Init("Daggerfall", ref args);
#if false
			var viewer = new Viewer() {
			};

			viewer.DeleteEvent += (o, a) => Application.Quit();
			viewer.Show();
			Application.Run();
#else
            string path = Properties.Settings.Default.DaggerfallPath;
            int check;

            while ((check = CheckPath(ref path)) != 1)
            {
                if (check < 0)
                    return;
                var chooser = new Gtk.FileChooserDialog("Select the path Daggerfall is in.", null, FileChooserAction.SelectFolder,
                    "Select", ResponseType.Accept, "Cancel", ResponseType.Cancel);

                chooser.Show();
                if (chooser.Run() == (int)ResponseType.Accept)
                {
                    path = chooser.Filename;
                } else
                    return;
                chooser.Destroy();
            }

            Properties.Settings.Default.DaggerfallPath = path;
            Properties.Settings.Default.Save();

            new DaggerfallGame(path).Run();
            //new GtkApp().Run();

#endif
        }

        static int CheckPath(ref string path)
        {
            if (path == null || path == "")
            {
                if (new MessageDialog(null, DialogFlags.Modal, MessageType.Info, ButtonsType.OkCancel, "Please select the path you've installed Daggerfall to. You can select either the main folder or the ARENA2 folder.").Run() == (int)ResponseType.Cancel)
                    return -1;
                return 0;
            }
            else
            {
                if (!File.Exists(path + Path.DirectorySeparatorChar + "SKY00.DAT"))
                {
                    string subpath = path + Path.DirectorySeparatorChar + "ARENA2";
                    if (Directory.Exists(subpath))
                    {
                        path = subpath;
                        return CheckPath(ref path);
                    }
                }

                if (!File.Exists(path + Path.DirectorySeparatorChar + "ARCH3D.BSA"))
                {
                    if (new MessageDialog(null, DialogFlags.Modal, MessageType.Error, ButtonsType.OkCancel, "Daggerfall seems to be copied to this path, but it has not been installed in DOS. Please do this or hit cancel and wait for me to figure out the compression.").Run() == (int)ResponseType.Cancel)
                        return -1;
                    return 0;
                }
            }
                return 1;
        }

        GraphicsDeviceManager manager;

        public DaggerfallGame(string path)
        {
            this.path = path;

            manager = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = GraphicsDeviceManager.DefaultBackBufferWidth,
                PreferredBackBufferHeight = GraphicsDeviceManager.DefaultBackBufferHeight,
                PreferMultiSampling = true,
                SynchronizeWithVerticalRetrace = true,
                IsFullScreen = true,
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
            Tools[0].Enabled = Tools[0].Visible = true;
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            State = new State(GraphicsDevice, path + "\\");
            Effect = new BasicEffect(GraphicsDevice);
            var block = State.Blocks.RecordList[0].Contents;
            var models = State.Models;

            Font = Content.Load<SpriteFont>("DebugFont");
            Batch = new SpriteBatch(GraphicsDevice);

            manager.IsFullScreen = true;
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
