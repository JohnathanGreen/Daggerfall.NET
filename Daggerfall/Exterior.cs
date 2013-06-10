using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daggerfall {
	public class Exterior : Area {
		#region Constructors

		public Exterior(Location location, BinaryReader reader)
			: base(location, reader) {
			Building[] buildings = new Building[reader.ReadUInt16()];
			U4 = reader.ReadBytes(5);

			for(int index = 0; index < buildings.Length; index++)
				buildings[index] = new Building(this, null, index, reader);
			this.Buildings = new ReadOnlyCollection<Building>(buildings);

			UnusedName = reader.ReadNulTerminatedAsciiString(32);

			int mapId = reader.ReadInt32();
			if((mapId & 0xFFFF) != (Location.MapId & 0xFFFF)) throw new Exception();

			U5 = reader.ReadInt32();
			int width = reader.ReadByte();
			int height = reader.ReadByte();
			U6 = reader.ReadBytes(7);

			Blocks = new ExteriorBlockIndex[width, height];
			for(int x = 0; x < width; x++)
				for(int y = 0; y < height; y++)
					Blocks[x, y].prefix = reader.ReadByte();
			reader.BaseStream.Seek(64 - width * height, SeekOrigin.Current);
			for(int x = 0; x < width; x++)
				for(int y = 0; y < height; y++)
					Blocks[x, y].number = reader.ReadByte();
			reader.BaseStream.Seek(64 - width * height, SeekOrigin.Current);
			for(int x = 0; x < width; x++)
				for(int y = 0; y < height; y++)
					Blocks[x, y].character = reader.ReadByte();
			reader.BaseStream.Seek(64 - width * height, SeekOrigin.Current);

			U7 = reader.ReadBytes(34);
			reader.ReadZeroes(9);

			U8 = new int[22];
			for(int index = 0; index < U8.Length; index++) U8[index] = reader.ReadInt32();
			reader.ReadZeroes(40);

			U9 = reader.ReadInt32();
		}

		#endregion Constructors

		#region Internals and fields

		#endregion Internals and fields

		#region Properties

		public readonly byte[] U4, U6, U7;
		public readonly int[] U8;
		public readonly int U5, U9;

		public readonly ExteriorBlockIndex[,] Blocks;

		/// <summary>The list of buildings in the exterior area.</summary>
		public readonly ReadOnlyCollection<Building> Buildings;

		public readonly string UnusedName;

		#endregion Properties

		#region Methods

		#endregion Methods
	}
}
