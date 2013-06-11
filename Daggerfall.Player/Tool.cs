using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daggerfall.Player
{
    public abstract class Tool : DrawableGameComponent
    {
        public Tool(DaggerfallGame game)
            : base(game)
        {
            game.Tools.Add(this);
            game.Components.Add(this);
            Visible = false;
        }

        public new DaggerfallGame Game { get { return (DaggerfallGame)base.Game; } }
        public abstract string ToolName { get; }
        public State State { get { return Game.State; } }
        public ViewerComponent Viewer { get { return Game.Viewer; } }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            GraphicsDevice.Clear(ClearOptions.DepthBuffer | ClearOptions.Stencil | ClearOptions.Target, Color.Black, 1, 0);
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            var effect = Game.Effect;
            var technique = effect.CurrentTechnique;
            effect.DiffuseColor = new Vector3(1, 1, 1);
            effect.SpecularColor = new Vector3(1, 1, 1);
            effect.SpecularPower = 120;
            effect.PreferPerPixelLighting = true;
            effect.EnableDefaultLighting();
        }

        public void DrawText()
        {
            Game.Batch.Begin();
            Vector2 point = new Vector2(5);
            DrawText(ref point);
            Game.Batch.End();
        }

        public virtual void DrawText(ref Vector2 point)
        {
            DrawTextLine(ref point, ControlsText);
        }

        protected virtual string ControlsText
        {
            get
            {
                return string.Format("Tool: Home/End ({0}/{1}; {2})", Game.CurrentTool + 1, Game.Tools.Count, Game.Tools[Game.CurrentTool].ToolName);
            }
        }

        public void DrawTextLine(ref Vector2 point, string format, params object[] args)
        {
            var font = Game.Font;
            string text = string.Format(format, args);
            Game.Batch.DrawAlignedString(font, text, point, Color.NavajoWhite, Alignment.Left);
            point += new Vector2(0, font.LineSpacing);
        }

        public static readonly TimeSpan KeyRepeatTime = TimeSpan.FromSeconds(1 / 10.0);
    }
}
