using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daggerfall
{
    public class ExteriorChunkSection : ExteriorChunkObject
    {
        #region Properties

        #endregion Properties

        #region Constructors

        public ExteriorChunkSection(ExteriorChunkArea area, int index, BinaryReader reader)
            : base(area, index, reader)
        {
            Unknowns.Add(reader.ReadUInt16());
            Unknowns.Add(reader.ReadUInt16());
        }

        #endregion Constructors
    }
}
