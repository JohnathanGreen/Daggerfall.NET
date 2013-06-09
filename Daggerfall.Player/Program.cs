using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Daggerfall.Player {
	public class PGame : Game {
		static void Main(string[] args) {
			var state = new State(null, @"D:\Games\Daggerfall\ARENA2\");
			var regions = state.Regions.ToArray();
			var region = regions[0];
			var location = region.Locations[0];
			Area exterior = location.Exterior, interior = location.Interior;
			var model = state.Models.RecordList[0].Contents;
			
			return;
			//new PGame().Run();
		}

		public PGame() {
			new GraphicsDeviceManager(this) {
				PreferredBackBufferWidth = 800,
				PreferredBackBufferHeight = 600,
				PreferMultiSampling = true,
				SynchronizeWithVerticalRetrace = true,
			};

            Components.Add(player = new FirstPersonPlayerComponent(this)
            {
                MouseEnabled = true,
                FarPlaneDistance = 1 << 20,
                NearPlaneDistance = 1 << 8,
                Enabled = true,
                WalkSpeed = 1 << 8,
                FlyUpDirection = new Vector3(0, 1 << 8, 0),
            });
		}

		protected override void LoadContent() {
			base.LoadContent();
			state = new State(GraphicsDevice, @"D:\Games\Daggerfall\ARENA2\");
			effect = new BasicEffect(GraphicsDevice);
		}

		State state;
        int modelIndex = 0;
		BasicEffect effect;
        ViewerComponent player;

		public ModelArchive Models { get { return state.Models; } }
		public RegionArchive Regions { get { return state.Regions; } }

		protected override void Draw(GameTime gameTime) {
			GraphicsDevice.Clear(ClearOptions.DepthBuffer | ClearOptions.Stencil | ClearOptions.Target, Color.Black, 1, 0);
			GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            var model = Models.RecordList[modelIndex].Contents;

            GraphicsDevice.SetVertexBuffer(model.VertexBuffer);
            GraphicsDevice.Indices = model.IndexBuffer;

            var technique = effect.CurrentTechnique;
            effect.World = Matrix.Identity;
            effect.View = player.View;
            effect.Projection = player.Projection;
			effect.DiffuseColor = new Vector3(1, 1, 1);
            effect.SpecularColor = new Vector3(1, 1, 1);
            effect.SpecularPower = 120;
			effect.PreferPerPixelLighting = true;
            effect.EnableDefaultLighting();

			foreach(var pass in effect.CurrentTechnique.Passes) {
				pass.Apply();
				GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, model.VertexBuffer.VertexCount, 0, model.IndexBuffer.IndexCount / 3);
			}
		}

        TimeSpan lastPrevious, lastNext;

        bool CheckRepeat(GameTime gameTime, ref TimeSpan lastTime, TimeSpan repeatTime)
        {
            if (gameTime.TotalGameTime - lastTime > repeatTime)
            {
                lastTime = gameTime.TotalGameTime;
                return true;
            }
            return false;
        }

        bool CheckRepeat(KeyboardState keyboard, Keys key, GameTime gameTime, ref TimeSpan lastTime, TimeSpan repeatTime)
        {
            return keyboard.IsKeyDown(key) && CheckRepeat(gameTime, ref lastTime, repeatTime);
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboard = Keyboard.GetState();
            TimeSpan repeatTime = TimeSpan.FromSeconds(0.1);

            if (CheckRepeat(keyboard, Keys.PageUp, gameTime, ref lastPrevious, repeatTime))
                modelIndex = Math.Max(0, modelIndex - 1);
            if (CheckRepeat(keyboard, Keys.PageDown, gameTime, ref lastNext, repeatTime))
                modelIndex = Math.Min(Models.Records.Count - 1, modelIndex + 1);

            base.Update(gameTime);
        }
	}
}
