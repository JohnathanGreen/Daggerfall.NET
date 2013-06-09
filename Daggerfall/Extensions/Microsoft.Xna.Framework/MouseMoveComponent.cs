using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Xna.Framework
{
    /// <summary>
    /// A <see cref="GameComponent"/> that, on <see cref="Update"/>, calls <see cref="MouseMoved"/> when the mouse is moved. It can also recentre the mouse.
    /// </summary>
    public class MouseMoveComponent : GameComponent
    {
        public MouseMoveComponent(Game game) : base(game) { }

        bool centreMouse, wasActive, reportStationaryMouse;
        Point mousePosition, oldMousePosition;

        public event MouseMovedEventHandler MouseMoved;

        /// <summary>Get or set whether to centre the mouse in the game window after every update, or to let the mouse move freely. If the game window is not active, then no messages will be created and the mouse won't be centred. The default is <c>false</c>.</summary>
        public bool CentreMouse { get { return centreMouse; } set { centreMouse = value; } }

        /// <summary>Get the difference between the <see cref="MousePosition"/> and the <see cref="MousePosition"/> of the previous <see cref="Update"/>.</summary>
        public Point MouseDelta { get { return new Point(mousePosition.X - oldMousePosition.X, mousePosition.Y - oldMousePosition.Y); } }

        public Point MouseOldPosition { get { return oldMousePosition; } }

        /// <summary>Get the position of the mouse in the last <see cref="Update"/>.</summary>
        public Point MousePosition { get { return mousePosition; } }

        /// <summary>Get or set whether to raise a <see cref="MouseMoved"/> event even if the mouse has not moved. The default is <c>false</c>.</summary>
        public bool ReportStationaryMouse { get { return reportStationaryMouse; } set { reportStationaryMouse = value; } }

        protected virtual void OnMouseMoved(Point position, Point oldPosition, MouseState state)
        {
            if (MouseMoved != null)
                MouseMoved(this, new MouseMovedEventArgs(position, oldPosition, state));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            MouseState state = Mouse.GetState();
            Point position = new Point(state.X, state.Y);
            Point centre = new Point(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            Point oldPosition = mousePosition;

            oldMousePosition = mousePosition;
            mousePosition = position;

            // Handle the CentreMouse condition.
            if (centreMouse)
            {
                if (Game.IsActive)
                {
                    // Only report the change if the game window was active on the last update to get a proper delta.
                    if (wasActive && (ReportStationaryMouse || position != centre))
                        OnMouseMoved(position, centre, state);
                    Mouse.SetPosition(centre.X, centre.Y);
                    oldMousePosition = centre;
                }

                wasActive = Game.IsActive;
            }
            else
            {
                if(ReportStationaryMouse || position != oldPosition)
                    OnMouseMoved(position, oldPosition, state);
                wasActive = false;
            }
        }
    }

    public class MouseMovedEventArgs : EventArgs
    {
        public MouseMovedEventArgs(Point position, Point oldPosition, MouseState state)
        {
            this.position = position;
            this.oldPosition = oldPosition;
            this.state = state;
        }

        readonly Point position, oldPosition;
        readonly MouseState state;

        /// <summary>Get the difference between the position of the mouse in the last update and the current one.</summary>
        public Point Delta { get { return new Point(position.X - oldPosition.X, position.Y - oldPosition.Y); } }

        /// <summary>Get the position the mouse was at in the last update, relative to the top-left corner of the window's client area.</summary>
        public Point OldPosition { get { return oldPosition; } }

        /// <summary>Get the current position of the mouse relative to the top-left corner of the window's client area.</summary>
        public Point Position { get { return position; } }

        /// <summary>Get the current <see cref="MouseState"/>.</summary>
        public MouseState State { get { return state; } }
    }

    public delegate void MouseMovedEventHandler(object sender, MouseMovedEventArgs a);
}
