using Gtk;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gtk
{
    public class GtkGame : Game
    {
        public GtkGame(GLWidget widget)
            : base((game) => new GtkGamePlatform(game, widget))
        {
            OpenTK.Toolkit.Init();
            OpenTK.Graphics.OpenGL.GL.InitNames();
            var manager = new GraphicsDeviceManager(this)
            {
            };

            DoInitialize();
        }
    }

    public class GtkGameWindow : GameWindow
    {
        public GtkGameWindow(GtkGamePlatform platform)
        {
            this.platform = platform;
            this.widget = platform.widget;
        }

        readonly GtkGamePlatform platform;
        readonly GLWidget widget;

        public override bool AllowUserResizing
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override Rectangle ClientBounds
        {
            get { throw new NotImplementedException(); }
        }

        public override DisplayOrientation CurrentOrientation
        {
            get { throw new NotImplementedException(); }
        }

        public override IntPtr Handle
        {
            get { return widget.Handle; }
        }

        public override string ScreenDeviceName
        {
            get { throw new NotImplementedException(); }
        }

        public override void BeginScreenDeviceChange(bool willBeFullScreen)
        {
            throw new NotImplementedException();
        }

        public override void EndScreenDeviceChange(string screenDeviceName, int clientWidth, int clientHeight)
        {
            throw new NotImplementedException();
        }

        protected override void SetSupportedOrientations(DisplayOrientation orientations)
        {
            //throw new NotImplementedException();
        }

        protected override void SetTitle(string title)
        {
            widget.Name = title;
            //throw new NotImplementedException();
        }
    }

    public class GtkGamePlatform : GamePlatform
    {
        internal GLWidget widget;
        //private OpenALSoundController soundControllerInstance = null; TODO
        // stored the current screen state, so we can check if it has changed.
        private bool isCurrentlyFullScreen = false;


        public override bool VSyncEnabled
        {
            get
            {
                return true;//return _view.Window.VSync == OpenTK.VSyncMode.On ? true : false;
            }

            set
            {
                //_view.Window.VSync = value ? OpenTK.VSyncMode.On : OpenTK.VSyncMode.Off;
            }
        }

        public GtkGamePlatform(Game game, GLWidget widget)
            : base(game)
        {
            this.widget = widget;
            this.Window = new GtkGameWindow(this);

            // Setup our OpenALSoundController to handle our SoundBuffer pools TODO
            /*try
            {
                soundControllerInstance = OpenALSoundController.GetInstance;
            } catch (DllNotFoundException ex)
            {
                throw (new NoAudioHardwareException("Failed to init OpenALSoundController", ex));
            }*/

#if LINUX
            // also set up SdlMixer to play background music. If one of these functions fails, we will not get any background music (but that should rarely happen)
            Tao.Sdl.Sdl.SDL_InitSubSystem(Tao.Sdl.Sdl.SDL_INIT_AUDIO);
            Tao.Sdl.SdlMixer.Mix_OpenAudio(44100, (short)Tao.Sdl.Sdl.AUDIO_S16SYS, 2, 1024);			

            //even though this method is called whenever IsMouseVisible is changed it needs to be called during startup
            //so that the cursor can be put in the correct inital state (hidden)
            OnIsMouseVisibleChanged();
#endif
        }

        public override GameRunBehavior DefaultRunBehavior
        {
            get { return GameRunBehavior.Synchronous; }
        }

#if WINDOWS
        protected override void OnIsMouseVisibleChanged()
        {
            //widget.MouseVisibleToggled(); TODO
        }
#endif

        public override void RunLoop()
        {
            throw new NotImplementedException();
            //ResetWindowBounds(false);
            //widget.Window.Run(0);
        }

        public override void StartRunLoop()
        {
            throw new NotImplementedException();
        }

        public override void Exit()
        {
            /*if (!widget.Window.IsExiting)
            {
                Net.NetworkSession.Exit();
                widget.Window.Exit();
            }*/
#if LINUX
            Tao.Sdl.SdlMixer.Mix_CloseAudio();
#endif
            OpenTK.DisplayDevice.Default.RestoreResolution();
        }

        public override bool BeforeUpdate(GameTime gameTime)
        {
            IsActive = widget.HasFocus;

            // Update our OpenAL sound buffer pools TODO
            //if (soundControllerInstance != null)
              //  soundControllerInstance.Update();
            return true;
        }

        public override bool BeforeDraw(GameTime gameTime)
        {
            return true;
        }

        public override void EnterFullScreen()
        {
            ResetWindowBounds(false);
        }

        public override void ExitFullScreen()
        {
            ResetWindowBounds(false);
        }

        internal void ResetWindowBounds(bool toggleFullScreen)
        {
            Rectangle bounds;

            bounds = Window.ClientBounds;

            //Changing window style forces a redraw. Some games
            //have fail-logic and toggle fullscreen in their draw function,
            //so temporarily become inactive so it won't execute.

            bool wasActive = IsActive;
            IsActive = false;

            var graphicsDeviceManager = (GraphicsDeviceManager)
                Game.Services.GetService(typeof(IGraphicsDeviceManager));

            if (graphicsDeviceManager.IsFullScreen)
            {
                throw new NotImplementedException();
                /*bounds = new Rectangle(0, 0, graphicsDeviceManager.PreferredBackBufferWidth, graphicsDeviceManager.PreferredBackBufferHeight);

                if (OpenTK.DisplayDevice.Default.Width != graphicsDeviceManager.PreferredBackBufferWidth ||
                    OpenTK.DisplayDevice.Default.Height != graphicsDeviceManager.PreferredBackBufferHeight)
                {
                    OpenTK.DisplayDevice.Default.ChangeResolution(graphicsDeviceManager.PreferredBackBufferWidth,
                            graphicsDeviceManager.PreferredBackBufferHeight,
                            OpenTK.DisplayDevice.Default.BitsPerPixel,
                            OpenTK.DisplayDevice.Default.RefreshRate);
                }*/
            } else
            {
                // switch back to the normal screen resolution
                //OpenTK.DisplayDevice.Default.RestoreResolution(); -BR
                // now update the bounds 
                bounds.Width = widget.WidthRequest;// graphicsDeviceManager.PreferredBackBufferWidth;
                bounds.Height = widget.HeightRequest;// graphicsDeviceManager.PreferredBackBufferHeight;
            }


            // Now we set our Presentation Parameters
            var device = (GraphicsDevice)graphicsDeviceManager.GraphicsDevice;
            // FIXME: Eliminate the need for null checks by only calling
            //        ResetWindowBounds after the device is ready.  Or,
            //        possibly break this method into smaller methods.
            if (device != null)
            {
                PresentationParameters parms = device.PresentationParameters;
                parms.BackBufferHeight = (int)bounds.Height;
                parms.BackBufferWidth = (int)bounds.Width;
            }

            if (graphicsDeviceManager.IsFullScreen != isCurrentlyFullScreen)
            {
                //widget.ToggleFullScreen();
            }

            // we only change window bounds if we are not fullscreen
            // or if fullscreen mode was just entered
            //if (!graphicsDeviceManager.IsFullScreen || (graphicsDeviceManager.IsFullScreen != isCurrentlyFullScreen))
              //  widget.ChangeClientBounds(bounds);

            // store the current fullscreen state
            isCurrentlyFullScreen = graphicsDeviceManager.IsFullScreen;

            IsActive = wasActive;
        }

        public override void EndScreenDeviceChange(string screenDeviceName, int clientWidth, int clientHeight)
        {

        }

        public override void BeginScreenDeviceChange(bool willBeFullScreen)
        {

        }
#if LINUX
        protected override void OnIsMouseVisibleChanged()
        {
            MouseState oldState = Mouse.GetState();
            _view.Window.CursorVisible = IsMouseVisible;
            // IsMouseVisible changes the location of the cursor on Linux and we have to manually set it back to the correct position
            System.Drawing.Point mousePos = _view.Window.PointToScreen(new System.Drawing.Point(oldState.X, oldState.Y));
            OpenTK.Input.Mouse.SetPosition(mousePos.X, mousePos.Y);
        }
#endif

        public override void Log(string Message)
        {
            Console.WriteLine(Message);
        }

        public override void Present()
        {
            throw new NotSupportedException();
            /*base.Present();

            var device = Game.GraphicsDevice;
            if (device != null)
                device.Present();*/

            //if (widget != null)
              //  widget.Window.SwapBuffers();
        }

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (widget != null)
                {
                    widget.Dispose();
                    widget = null;
                }
            }

            base.Dispose(disposing);
        }

    }
}
