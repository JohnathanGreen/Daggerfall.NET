using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daggerfall.Player
{
    /// <summary>
    /// A tool that shows items amongst a selection of items.
    /// </summary>
    public abstract class BrowserTool : ViewerTool
    {
        public BrowserTool(DaggerfallGame game) : base(game) { }

        protected int elementIndex;

        public abstract int Count { get; }
        public override string ToolName { get { return ElementName + " browser"; } }
        public abstract string ElementName { get; }

        public int ElementIndex
        {
            get { return elementIndex; }
            set { elementIndex = value.Wrap(Count); }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        protected override string ControlsText
        {
            get
            {
                return base.ControlsText + string.Format("; Change current item with PageUp/PageDown ({0} of {1})", ElementIndex + 1, Count);
            }
        }

        TimeSpan lastPrevious, lastNext;

        public override void Update(GameTime gameTime)
        {
            if (Game.Tools[Game.CurrentTool] != this)
                return;
            var keyboard = Keyboard.GetState();

            bool fast = keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift);
            bool zoom = keyboard.IsKeyDown(Keys.LeftControl) || keyboard.IsKeyDown(Keys.RightControl);
            int speed = zoom ? 100 : fast ? 10 : 1;

            ElementIndex += keyboard.CheckNextPreviousKeys(Keys.PageDown, Keys.PageUp, KeyRepeatTime, gameTime, ref lastNext, ref lastPrevious) * speed;

            base.Update(gameTime);
        }
    }
}
