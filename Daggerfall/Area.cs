using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daggerfall {
	/// <summary>
	/// Describes exterior and interior information for a <see cref="Location"/>.
	/// </summary>
	public abstract class Area : StateObject {
		public Area(Location location, BinaryReader reader) : base(location.State) {
			this.Location = location;
			bool interior = this is Interior;

			AreaDoor[] doors = new AreaDoor[reader.ReadInt32()];
			for(int index = 0; index < doors.Length; index++)
				doors[index] = new AreaDoor(this, index, reader);
			Doors = new ReadOnlyCollection<AreaDoor>(doors);

			if(reader.ReadInt32() != 1) throw new Exception();
			reader.ReadZeroes(3);
			Y = reader.ReadInt32();
			reader.ReadZeroes(4);
			X = reader.ReadInt32();
			int isExterior = reader.ReadUInt16();
			if((isExterior == 0 && !interior) || (isExterior == 0x8000 && interior) || (isExterior != 0 && isExterior != 0x8000)) throw new Exception();
			reader.ReadZeroes(2);
			U1 = reader.ReadInt32();
			U2 = reader.ReadInt32();
			int f1 = reader.ReadInt16();
			if(f1 != 1) throw new Exception();
			Id = reader.ReadInt32();
			int isInterior = reader.ReadUInt16();
			if(isInterior != 0) throw new Exception();
			//if((isInterior == 0 && interior) || (isInterior == 1 && !interior) || (isInterior != 0 && isInterior != 1)) throw new Exception();

			// U3a as exteriorId is supposed to pass the test, but it seems to be an unrelated number.
			U3a = reader.ReadInt32();
			//if(interior && exteriorId != Location.Exterior.Id) throw new Exception();

			reader.ReadZeroes(28);
			EnteringName = reader.ReadNulTerminatedAsciiString(32);
			U3 = reader.ReadBytes(9);
		}

		/// <summary>The name to use when entering the location.</summary>
		public readonly string EnteringName;

		public readonly Location Location;

		/// <summary>Unique identification of this interior/exterior.</summary>
		public readonly int Id;

		public Region Region { get { return Location.Region; } }
		public readonly int X;
		public readonly int Y;

		public readonly int U1, U2, U3a;
		public readonly byte[] U3;

		public readonly ReadOnlyCollection<AreaDoor> Doors;
	}
}
