using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daggerfall.Player
{
    /// <summary>
    /// Base of classes that uses the viewer.
    /// </summary>
    public abstract class ViewerTool : Tool
    {
        public ViewerTool(DaggerfallGame game) : base(game) { }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Draw(gameTime);
            var effect = Game.Effect;
            effect.World = Matrix.Identity;
            effect.View = Viewer.View;
            effect.Projection = Viewer.Projection;
        }
    }
}
