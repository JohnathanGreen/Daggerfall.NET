using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daggerfall
{
    /// <summary>
    /// A section in an <see cref="ExteriorBlock"/> that draws part of a building.
    /// </summary>
    public class ExteriorChunk : StateObject
    {
        internal ExteriorChunk(ExteriorBlock block, BinaryReader reader, int index)
            : base(block.State)
        {
            this.Index = index;
            Unknowns.Add("g", u1 = reader.ReadInt32());
            Unknowns.Add("h", u2 = reader.ReadInt32());
            this.X = reader.ReadInt32();
            this.Z = reader.ReadInt32();
            this.Angle = Angle.Daggerfall(reader.ReadInt32());
            this.Block = block;

            var b1 = State.Models.RecordMap.ContainsKey(u1);
            var b2 = State.Models.RecordMap.ContainsKey(u2);
        }

        int u1, u2;

        internal Building building;

        public readonly Unknowns Unknowns = new Unknowns();

        public readonly Angle Angle;
        public readonly Block Block;
        public Building Building { get { return building; } }
        public readonly int Index;
        public string ModelFilename { get; protected set; }
        public readonly int X, Z;
        public ExteriorChunkArea Exterior { get; protected set; }
        public ExteriorChunkArea Interior { get; protected set; }

        public void Draw(BasicEffect effect, ref Matrix world, bool exterior, bool interior)
        {
            if(exterior)
                Exterior.Draw(effect, ref world);
            if(interior)
                Interior.Draw(effect, ref world);
        }

        internal void LoadFilename(BinaryReader reader)
        {
            ModelFilename = reader.ReadNulTerminatedAsciiString(13);
        }

        internal void LoadContents(BinaryReader reader)
        {
            Exterior = new ExteriorChunkArea(this, true, reader);
            Interior = new ExteriorChunkArea(this, false, reader);
        }
    }
}
