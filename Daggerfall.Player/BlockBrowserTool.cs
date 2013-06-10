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

        public override int Count { get { return State.Blocks.Records.Count; } }
        public override string ToolName { get { return "Block browser"; } }

        public override void Draw(GameTime gameTime)
        {
            var batch = Game.Batch;
            var effect = Game.Effect;
            var font = Game.Font;

            base.Draw(gameTime);

            var block = State.Blocks.RecordList[Index].Contents;
            Matrix world = Matrix.Identity;
            if(block != null)
                block.Draw(effect, ref world, true, false);
            DrawText();
        }
    }
}
