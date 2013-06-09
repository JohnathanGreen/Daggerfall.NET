using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daggerfall {
	/// <summary>
	/// The inside area of a <see cref="Location"/>, such as the homes or dungeon.
	/// </summary>
	public class Interior : Area {
		public Interior(Location location, BinaryReader reader)
			: base(location, reader) {
			reader.ReadZeroes(2);
			U4 = reader.ReadInt32();
			U5 = reader.ReadInt32();

			Blocks = new InteriorBlock[reader.ReadUInt16()];
			U6 = reader.ReadBytes(5);

			for(int index = 0, count = Blocks.Length; index < count; index++)
				Blocks[index] = new InteriorBlock(reader.ReadByte(), reader.ReadByte(), reader.ReadUInt16());
		}

		public readonly int U4, U5;
		public readonly byte[] U6;
		public readonly InteriorBlock[] Blocks;
	}
}
