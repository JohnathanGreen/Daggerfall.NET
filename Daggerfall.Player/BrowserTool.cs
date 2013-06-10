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

        int index;

        public abstract int Count { get; }

        public int Index
        {
            get { return index; }
            set { index = Math.Max(0, Math.Min(Count - 1, value)); }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        protected override string ControlsText
        {
            get
            {
                return base.ControlsText + string.Format("; Change current item with PageUp/PageDown ({0} of {1})", Index + 1, Count);
            }
        }

        TimeSpan lastPrevious, lastNext;

        public override void Update(GameTime gameTime)
        {
            var keyboard = Keyboard.GetState();
            TimeSpan repeatTime = TimeSpan.FromSeconds(0.1);

            bool fast = keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift);
            bool zoom = keyboard.IsKeyDown(Keys.LeftControl) || keyboard.IsKeyDown(Keys.RightControl);
            int speed = zoom ? 100 : fast ? 10 : 1;

            if (keyboard.CheckRepeat(Keys.PageUp, gameTime, ref lastPrevious, repeatTime))
                Index -= speed;
            if (keyboard.CheckRepeat(Keys.PageDown, gameTime, ref lastNext, repeatTime))
                Index += speed;

            base.Update(gameTime);
        }
    }
}
