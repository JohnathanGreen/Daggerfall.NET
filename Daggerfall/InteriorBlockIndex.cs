using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daggerfall {
	public struct InteriorBlockIndex {
		internal InteriorBlockIndex(byte x, byte z, ushort blockIndex) {
			this.X = x;
			this.Z = z;
			this.BlockIndex = blockIndex;
		}

		static readonly string[] IndexStrings = new string[] { "N", "W", "L", "S", "B", "M" };

		readonly ushort BlockIndex;

		public readonly byte X;
		public readonly byte Z;

		/// <summary>Get whether this is where the player starts in the dungeon. There may only be one <see cref="InteriorBlockIndex"/> with this flag set.</summary>
		public bool IsStartingBlock { get { return (BlockIndex & (1 << 10)) != 0; } }

		public string BlockName {
			get {
				return string.Format("{0}{1}.RDB", IndexStrings[BlockIndex >> 11], BlockIndex & 1023);
			}
		}
	}
}
