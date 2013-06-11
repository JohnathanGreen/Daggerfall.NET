using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daggerfall {
	public class ExteriorBlock : Block {
		#region Constructors

		internal ExteriorBlock(State state, BinaryReader reader, BlockArchive.Record record) : base(state, reader) {
			this.ChunkCount = reader.ReadByte();
			int modelCount = reader.ReadByte();
			int flatCount = reader.ReadByte();

			for (int index = 0; index < Chunks.Length; index++)
				Chunks[index] = new ExteriorChunk(this, reader, index);
			for (int index = 0; index < Chunks.Length; index++)
				Chunks[index].building = new Building(null, this, index, reader);
            for (int index = 0; index < Chunks.Length; index++)
            {
                Chunks[index].Unknowns.Add(reader.ReadUInt16());
                Chunks[index].Unknowns.Add(reader.ReadUInt16());
            }

			int[] chunkSizeList = new int[32];
			for (int index = 0; index < chunkSizeList.Length; index++)
				chunkSizeList[index] = reader.ReadInt32();

			U1 = reader.ReadBytes(8);
			for (int y = 0, yCount = Ground.GetLength(1); y < yCount; y++)
				for (int x = 0, xCount = Ground.GetLength(0); x < xCount; x++)
					Ground[x, y] = new Ground(reader.ReadByte());
			U2 = reader.ReadBytes(256);

			for (int y = 0, yCount = Automap.GetLength(1); y < yCount; y++)
				for (int x = 0, xCount = Automap.GetLength(0); x < xCount; x++)
					Automap[x, y] = reader.ReadByte();

			string filename = reader.ReadNulTerminatedAsciiString(13);
			if (filename != record.Key) throw new Exception();
            for (int index = 0; index < Chunks.Length; index++)
                Chunks[index].LoadFilename(reader);

            for (int index = 0; index < ChunkCount; index++)
            {
                long start = reader.BaseStream.Position;
                Chunks[index].LoadContents(reader);
                int required = chunkSizeList[index];
                long size = reader.BaseStream.Position - start;

                if (size == required - 1)
                {
                    reader.ReadByte();
                    size++;
                }

                if (required != size)
                    throw new Exception();
            }
		}

        #endregion Constructors

        #region Internals and fields

        #endregion Internals and fields

        #region Properties

        public readonly int ChunkCount;
		public readonly ExteriorChunk[] Chunks = new ExteriorChunk[32];
		public readonly Ground[,] Ground = new Ground[16, 16];
		public readonly byte[,] Automap = new byte[64, 64]; // If nonzero, subtract one to get a BuildingType.
		public readonly byte[] U1, U2;

		#endregion Properties

		#region Methods

        public override void Draw(BasicEffect effect, ref Matrix world, bool exterior, bool interior)
        {
            for (int index = 0; index < ChunkCount; index++)
                Chunks[index].Draw(effect, ref world, exterior, interior);
        }

		#endregion Methods
	}
}
