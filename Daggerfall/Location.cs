using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daggerfall {
	/// <summary>
	/// A location within a <see cref="Region"/>.
	/// </summary>
	public class Location : StateObject {
		#region Constructors

		internal Location(Region region, int index, string name)
			: base(region.State) {
			this.region = region;
			this.index = index;
			this.name = name;
		}

		#endregion Constructors

		#region Internals and fields

		readonly int index;
		readonly Region region;
		readonly string name;
		int mapId;

		#endregion Internals and fields

		#region Properties

		/// <summary>Get the index of this <see cref="Location"/> within the <see cref="Region"/>.</summary>
		public int Index { get { return index; } }

		public int MapId { get { return mapId; } }

		/// <summary>Get the name of the <see cref="Location"/>.</summary>
		public string Name { get { return name; } }

		/// <summary>Get the <see cref="Daggerfall.Region"/> that this <see cref="Location"/> is within.</summary>
		public Region Region { get { return region; } }

		public byte U1 { get; private set; }
		public ushort U2 { get; private set; }
		public uint U3 { get; private set; }

		public int X { get; private set; }
		public int Y { get; private set; }
		public LocationType Type { get; private set; }
		public bool Known { get; private set; }

		public Area Exterior { get; private set; }
		public Area Interior { get; private set; }

		#endregion Properties

		#region Methods

		internal void LoadMapTable(BinaryReader reader) {
			mapId = reader.ReadInt32();
			U1 = reader.ReadByte();
			int yAndType = reader.ReadInt32();
			Y = (yAndType & 0x1FFFF) * 256;
			LocationType typeAndKnown = (LocationType)(yAndType >> 17);
			Type = typeAndKnown & ~LocationType.KnownMask;
			Known = (typeAndKnown & LocationType.KnownMask) != 0;
			X = reader.ReadUInt16() * 256;
			U2 = reader.ReadUInt16();
			U3 = reader.ReadUInt32();
		}

		internal void LoadMapPItem(BinaryReader reader) {
			Exterior = new Exterior(this, reader);
		}

		internal void LoadMapDItem(BinaryReader reader) {
			Interior = new Interior(this, reader);
		}

		public override string ToString() {
			string text = "";

			text += string.Format("\"{0}\", {2}, at {3}x{4}", Name, Type, X, Y);
			return text;
		}

		#endregion Methods
	}

	public abstract class LocationObject : StateObject {
		internal LocationObject(Location location, int index) : base(location.State) { this.location = location; this.index = index; }

		readonly Location location;
		readonly int index;

		public int Index { get { return index; } }
		public Location Location { get { return location; } }
		public Region Region { get { return location.Region; } }
	}
}
