using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daggerfall.Player
{
    public class ModelBrowserTool : BrowserTool
    {
        public ModelBrowserTool(DaggerfallGame game)
            : base(game)
        {
        }

        public override int Count { get { return State.Models.Records.Count; } }
        public override string ToolName { get { return "Model browser"; } }

        public override void Draw(GameTime gameTime)
        {
            var batch = Game.Batch;
            var effect = Game.Effect;
            var font = Game.Font;

            base.Draw(gameTime);

            var model = State.Models.RecordList[Index].Contents;
            Matrix world = Matrix.Identity;
            model.Draw(effect, ref world);
            DrawText();
        }
    }
}
