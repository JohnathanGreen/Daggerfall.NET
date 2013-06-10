using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daggerfall
{
    public class ExteriorChunkFlat : ExteriorChunkObject
    {
        #region Properties

        public readonly TextureIndex TextureIndex;

        #endregion Properties

        #region Constructors

        public ExteriorChunkFlat(ExteriorChunkArea area, int index, BinaryReader reader) : base(area, index, reader) {
            TextureIndex = new TextureIndex(reader.ReadUInt16());
            Unknowns.Add(reader.ReadUInt16());
            Unknowns.Add(reader.ReadByte());
        }

        #endregion Constructors
    }
}
