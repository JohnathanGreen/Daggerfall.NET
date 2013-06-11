using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daggerfall.Player
{
    public class BlockBrowserTool : BrowserTool
    {
        public BlockBrowserTool(DaggerfallGame game)
            : base(game)
        {
        }

        public override int Count { get { return State.Blocks.RecordMap.Count; } }
        public override string ElementName { get { return "Block"; } }

        public override void Draw(GameTime gameTime)
        {
            var batch = Game.Batch;
            var effect = Game.Effect;
            var font = Game.Font;

            base.Draw(gameTime);

            var block = State.Blocks.Records[ElementIndex].Value;
            Matrix world = Matrix.Identity;
            if(block != null)
                block.Draw(effect, ref world, true, false);
            DrawText();
        }
    }
}
