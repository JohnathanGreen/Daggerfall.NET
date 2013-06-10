using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daggerfall
{
    public class ExteriorChunkObject : StateObject
    {
        #region Properties

        public readonly ExteriorChunkArea Area;
        public readonly int Index;
        public readonly Vector3 Position;
        public readonly Unknowns Unknowns = new Unknowns();

        #endregion Properties

        #region Constructors

        internal ExteriorChunkObject(ExteriorChunkArea area, int index, BinaryReader reader) : base(area.State)
        {
            Area = area;
            Index = index;
            Position = reader.ReadVector3i();
        }

        #endregion Constructors
    }
}
