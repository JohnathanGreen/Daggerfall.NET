using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Xna.Framework
{
    public static class Extensions
    {
        public static Vector2 Normalized(this Vector2 value) { return Vector2.Normalize(value); }
        public static Vector3 Normalized(this Vector3 value) { return Vector3.Normalize(value); }
        public static Vector4 Normalized(this Vector4 value) { return Vector4.Normalize(value); }

        /// <summary>Swizzle the <see cref="Vector3"/> so that the Z and Y components are reversed. This is useful for converting between coordinate systems where Y is up and Z is up.</summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Vector3 XZY(this Vector3 value) { return new Vector3(value.X, value.Z, value.Y); }

        /// <summary>
        /// Rotate the <see cref="Vector2"/> by a certain amount around the Z axis.
        /// </summary>
        /// <param name="value">The vector to rotate.</param>
        /// <param name="amount">The amount to rotate by.</param>
        /// <returns>The rotated vector.</returns>
		public static Vector2 Rotate(this Vector2 value, Angle amount) {
			float c = (float)Math.Cos(amount.InRadians), s = (float)Math.Sin(amount.InRadians);
			return new Vector2(value.X * c - value.Y * s, value.X * s + value.Y * c);
		}
    }
}
