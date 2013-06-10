using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Xna.Framework
{
    /// <summary>
    /// A <see cref="GameComponent"/> that handles input for a first person game.
    /// </summary>
    public class FirstPersonPlayerComponent : ViewerComponent
    {
        #region Constructors

        public FirstPersonPlayerComponent(Game game, PlayerIndex playerIndex = PlayerIndex.One)
            : base(game)
        {
            this.playerIndex = playerIndex;
            KeyForward = Keys.W;
            KeyBackward = Keys.S;
            KeyStrafeLeft = Keys.A;
            KeyStrafeRight = Keys.D;
            KeyFlyUp = Keys.E;
            KeyFlyDown = Keys.Q;
            KeyRun = Keys.LeftShift;
            WalkSpeed = 10;
            RunMultiplier = 2;
            FlyUpDirection = new Vector3(0, 10, 0);

            Game.Activated += OnGameActivatedChanged;
            Game.Deactivated += OnGameActivatedChanged;
        }

        #endregion Constructors

        #region Fields

        bool mouseEnabled;
        MouseMoveComponent mouseMoveComponent;
        readonly PlayerIndex playerIndex;

        #endregion Fields

        #region Properties

        public bool MouseEnabled
        {
            get { return mouseEnabled; }

            set
            {
                if (mouseEnabled == value)
                    return;

                if (value)
                {
                    if (mouseMoveComponent == null)
                    {
                        mouseMoveComponent = new MouseMoveComponent(Game)
                        {
                            CentreMouse = true,
                        };

                        if (Game.Components.Contains(this))
                            Game.Components.Add(mouseMoveComponent);
                    }

                    if (Enabled)
                        mouseMoveComponent.Enabled = true;
                }
                else
                    mouseMoveComponent.Enabled = false;

                mouseEnabled = value;
            }
        }

        public MouseMoveComponent MouseMoveComponent { get { return mouseMoveComponent; } }

        public PlayerIndex PlayerIndex { get { return playerIndex; } }

        /// <summary>Get or set the world position of the player.</summary>
        public Vector3 Position { get; set; }

        /// <summary>Get or set the facing of the player. Higher values turn to the right.</summary>
        public Angle Yaw { get; set; }

        /// <summary>Get or set the nodding angle of the player in degrees. Higher values look down.</summary>
        public Angle Pitch { get; set; }

        public Angle Roll { get; set; }

        public Keys KeyForward { get; set; }

        public Keys KeyBackward { get; set; }

        public Keys KeyStrafeLeft { get; set; }

        public Keys KeyStrafeRight { get; set; }

        public Keys KeyFlyUp { get; set; }

        public Keys KeyFlyDown { get; set; }

        public Keys KeyRun { get; set; }

        /// <summary>Get or set the direction and scale of flying up. The default is (0, 10, 0).</summary>
        public Vector3 FlyUpDirection { get; set; }

        /// <summary>Get or set how quickly the player walks. The default is 10.</summary>
        public float WalkSpeed { get; set; }

        /// <summary>Get or set how much faster <see cref="WalkSpeed"/> is than <see cref="RunSpeed"/>. The default is 2.</summary>
        public float RunMultiplier { get; set; }

        public float RunSpeed { get { return WalkSpeed * RunMultiplier; } set { RunMultiplier = value / WalkSpeed; } }

        #endregion Properties

        #region Methods

        public override void Initialize()
        {
            base.Initialize();
            if (mouseMoveComponent != null && !Game.Components.Contains(mouseMoveComponent))
                Game.Components.Add(mouseMoveComponent);
        }

        protected override void OnEnabledChanged(object sender, EventArgs args)
        {
            if (mouseMoveComponent != null)
                mouseMoveComponent.Enabled = Enabled;
            base.OnEnabledChanged(sender, args);
        }

        void OnGameActivatedChanged(object sender, EventArgs e)
        {
        }

        public override void Update(GameTime gameTime)
        {
            if (!Game.IsActive)
                return;

            var mouse = Mouse.GetState();
            var keyboard = Keyboard.GetState(playerIndex);
            var gamepad = GamePad.GetState(playerIndex);

            if (MouseEnabled)
            {
                Point mouseDelta = mouseMoveComponent.MouseDelta;
                Yaw += Angle.Degrees(mouseDelta.X);
                Pitch += Angle.Degrees(mouseDelta.Y);
            }
            /*if(LastMousePosition != null) {
                Vector2l last = LastMousePosition.Value, delta = new Vector2l(mousePosition.X - last.X, mousePosition.Y - last.Y);
                Yaw += Angle.Degrees(delta.X);
                Pitch += Angle.Degrees(delta.Y);
            }

            mouse.Position = new Vector2l(Game.Context.Viewport.Width / 2, Game.Context.Viewport.Height / 2);
            LastMousePosition = new Vector2l(Game.Context.Viewport.Width / 2, Game.Context.Viewport.Height / 2);*/

            bool run = keyboard.IsKeyDown(KeyRun);
            float walkSpeed = run ? RunSpeed : WalkSpeed;
            Vector3 flyDirection = run ? FlyUpDirection * RunMultiplier : FlyUpDirection;

            if (keyboard.IsKeyDown(KeyFlyUp))
                Position += flyDirection;
            if (keyboard.IsKeyDown(KeyFlyDown))
                Position -= flyDirection;

            Vector2 facing = new Vector2(0, -1).Rotate(Yaw); // Positive is forward, negative is backwards.
            Vector2 strafing = new Vector2(1, 0).Rotate(Yaw); // Positive is right, negative is left.
            Vector2 movement = Vector2.Zero;

            if (keyboard.IsKeyDown(KeyForward))
                movement += facing;
            if (keyboard.IsKeyDown(KeyBackward))
                movement -= facing;
            if (keyboard.IsKeyDown(KeyStrafeRight))
                movement += strafing;
            if (keyboard.IsKeyDown(KeyStrafeLeft))
                movement -= strafing;

            if (movement != Vector2.Zero)
                Position += new Vector3(movement.X, 0, movement.Y).Normalized() * walkSpeed;

            Pitch = Pitch.MaxDegrees(-90);
            Pitch = Pitch.MinDegrees(90);

            View = Matrix.CreateTranslation(-Position) *
                Matrix.CreateFromYawPitchRoll(Yaw.InRadians, 0, 0) *
                Matrix.CreateFromYawPitchRoll(0, Pitch.InRadians, 0) *
                Matrix.CreateFromYawPitchRoll(0, 0, Roll.InRadians);

            base.Update(gameTime);
        }

        #endregion Methods
    }
}
