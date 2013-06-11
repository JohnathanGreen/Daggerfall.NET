using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daggerfall
{
    public class Palette : StateObject
    {
        public readonly Color[] Colors = new Color[256];

        public Palette(State state, string dataFile) : base(state)
        {
            using (BinaryReader reader = State.GetDataReader(dataFile))
            {
                if (reader.BaseStream.Length == 776)
                {
                    var size = reader.ReadInt32();
                    var major = reader.ReadUInt16();
                    var minor = reader.ReadUInt16();
                    if (size != 776 || major != 0xb123 || minor != 0) throw new Exception();
                }
                else if (reader.BaseStream.Length != 768)
                    throw new Exception(string.Format("'{0}' is not a palette file.", dataFile));

                for (int index = 0; index < 256; index++)
                    Colors[index] = new Color(reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
                Colors[0].A = 0;
            }
        }
    }
}
