using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daggerfall
{
    public struct TextureIndex
    {
        public TextureIndex(ushort value) { this.value = value; }

        readonly ushort value;

        public int FileIndex { get { return value >> 7; } }
        public int ImageIndex { get { return value & 0x7F; } }

        public override string ToString() { return string.Format("{0}.{1}", FileIndex, ImageIndex); }
    }
}
