using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Xna.Framework
{
    public struct Angle
    {
        #region Constructors

        Angle(float value) { this.value = value; }

        public static readonly Angle Zero = new Angle(0);

        public static Angle Degrees(float value) { return new Angle(value / ToDegrees); }
        public static Angle Radians(float value) { return new Angle(value / ToRadians); }

        #endregion Constructors

        #region Internals and fields

        readonly float value;

        const float ToDegrees = (float)(ToRadians * 180 / Math.PI);
        const float ToRadians = 1;

        #endregion Internals and fields

        #region Properties

        public float InDegrees { get { return value * ToDegrees; } }
        public float InRadians { get { return value * ToRadians; } }

        #endregion Properties

        #region Methods

        /// <summary>Get the maximum of this angle and the other.</summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Angle Max(Angle other) { return new Angle(Math.Max(value, other.value)); }

        public Angle MaxDegrees(float otherDegrees) { return Max(Degrees(otherDegrees)); }
        public Angle MaxRadians(float otherRadians) { return Max(Radians(otherRadians)); }

        /// <summary>Get the minimum of this and the other angle.</summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Angle Min(Angle other) { return new Angle(Math.Min(value, other.value)); }

        public Angle MinDegrees(float otherDegrees) { return Min(Degrees(otherDegrees)); }
        public Angle MinRadians(float otherRadians) { return Min(Radians(otherRadians)); }

        #endregion Methods

        #region Operators

        public static Angle operator +(Angle a, Angle b) { return new Angle(a.value + b.value); }
        public static Angle operator -(Angle a, Angle b) { return new Angle(a.value - b.value); }
        public static Angle operator *(Angle a, float b) { return new Angle(a.value * b); }
        public static Angle operator /(Angle a, float b) { return new Angle(a.value / b); }
        public static Angle operator %(Angle a, Angle b) { return new Angle(a.value % b.value); }

        #endregion Operators
    }
}
