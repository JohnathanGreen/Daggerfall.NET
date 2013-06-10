using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daggerfall
{
    public class ExteriorChunkDoor : ExteriorChunkObject
    {
        #region Properties

        public readonly Angle Rotation;

        #endregion Properties

        #region Constructors

        public ExteriorChunkDoor(ExteriorChunkArea area, int index, BinaryReader reader)
            : base(area, index, reader)
        {
            Unknowns.Add(reader.ReadUInt16());
            Rotation = Angle.Daggerfall(reader.ReadInt16());
            Unknowns.Add(reader.ReadUInt16());
            reader.ReadZeroes(1);
        }

        #endregion Constructors
    }
}
