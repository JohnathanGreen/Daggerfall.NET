using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daggerfall
{

    public class ExteriorChunkPerson : ExteriorChunkObject
    {
        #region Properties

        public readonly TextureIndex TextureIndex;
        public readonly FactionIndex FactionIndex;

        #endregion Properties

        #region Constructors

        public ExteriorChunkPerson(ExteriorChunkArea area, int index, BinaryReader reader)
            : base(area, index, reader)
        {
            TextureIndex = new TextureIndex(reader.ReadUInt16());
            FactionIndex = new FactionIndex(reader.ReadUInt16());
            Unknowns.Add(reader.ReadByte());
        }

        #endregion Constructors
    }
}
