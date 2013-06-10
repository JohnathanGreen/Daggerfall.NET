using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daggerfall
{
    public struct FactionIndex
    {
        public FactionIndex(ushort index)
        {
            this.index = index;
        }

        ushort index;

        public ushort Index { get { return index; } }
    }
}
