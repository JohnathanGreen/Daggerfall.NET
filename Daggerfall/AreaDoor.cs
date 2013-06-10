using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daggerfall {
	public class AreaDoor : StateObject {
		public AreaDoor(Area area, int index, BinaryReader reader)
			: base(area.State) {
			Area = area;
			Index = index;
			BuildingIndex = reader.ReadUInt16();
			reader.ReadZeroes(1);
			UnknownMask = reader.ReadByte();
			U1 = reader.ReadByte();
			U2 = reader.ReadByte();
		}

		public readonly Area Area;
		public readonly ushort BuildingIndex;
		public readonly byte UnknownMask;
		public readonly byte U1, U2;
		public readonly int Index;
	}
}
