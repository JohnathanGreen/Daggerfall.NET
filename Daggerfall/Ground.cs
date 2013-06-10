using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daggerfall
{
    /// <summary>
    /// Ground texture identification for an <see cref="ExteriorBlock"/>.
    /// </summary>
    public struct Ground
    {
        internal Ground(byte value)
        {
            this.value = value;
        }

        readonly byte value;

        /// <summary>Get whether to flip the texture on the X and Z axes.</summary>
        public bool IsFlipped { get { return (value & 128) != 0; } }

        /// <summary>Get whether to rotate the texture 90 degrees clockwise.</summary>
        public bool RotateClockwise { get { return (value & 64) != 0; } }

        /// <summary>Get the texture index to display. This will depend upon the biome, weather, and season.</summary>
        public int TextureIndex { get { return value & 63; } }

        public override string ToString()
        {
            string text = "{" + TextureIndex;
            if (IsFlipped) text += ", Flipped";
            if (RotateClockwise) text += ", Rotated";
            return text + "}";
        }
    }
}
