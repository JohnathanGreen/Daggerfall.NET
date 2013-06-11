using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daggerfall.Player
{
    public class TextureBrowserTool : BrowserTool
    {
        public TextureBrowserTool(DaggerfallGame game) : base(game) {
            elementIndex = 100;
        }

        public override int Count { get { return State.Textures.Count; } }

        public override string ElementName { get { return "Texture archive"; } }

        public TextureArchive CurrentArchive { get { return Game.State.Textures[elementIndex]; } }

        protected override string ControlsText { get { return base.ControlsText + "; " + (CurrentArchive != null ? CurrentArchive.Name : "(missing archive)"); } }

        TimeSpan FrameRate = TimeSpan.FromSeconds(1.0 / 6.0);

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Draw(gameTime);

            if (CurrentArchive != null)
            {
                var batch = Game.Batch;
                batch.Begin();

                int w = GraphicsDevice.Viewport.Width, h = GraphicsDevice.Viewport.Height;
                int border = 5, x = border, y = border + Game.Font.LineSpacing, maxHeight = 0;
                long currentFrame = gameTime.TotalGameTime.Ticks / FrameRate.Ticks;

                foreach (TextureSet set in CurrentArchive.Values)
                {
                    if (set.Frames.Count == 0)
                        continue;
                    Image frame = set.Frames[(int)(currentFrame % set.Frames.Count)];
                    if (w - frame.Dimensions.X - border < x)
                    {
                        x = border;
                        y += maxHeight + border;
                        maxHeight = 0;
                    }

                    maxHeight = Math.Max(maxHeight, frame.Dimensions.Y);
                    var texture = frame.Texture;
                    batch.Draw(texture, new Vector2(x, y), Color.White);
                    x += border + frame.Dimensions.X;
                }

                batch.End();
            }

            DrawText();
        }
    }
}
