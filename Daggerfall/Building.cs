using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daggerfall {
	/// <summary>
	/// A building in a <see cref="Exterior"/>.
	/// </summary>
	public class Building : StateObject {
		#region Constructors

		internal Building(Exterior exterior, int index, BinaryReader reader) : base(exterior.State) {
			Exterior = exterior;
			Index = index;

			NameSeed = reader.ReadUInt16();
			reader.ReadZeroes(16);
			FactionId = reader.ReadUInt16();
			Sector = reader.ReadInt16();
			var locationId = reader.ReadUInt16();
			if(locationId != exterior.Id) throw new Exception();
			Type = (BuildingType)reader.ReadByte();
			Quality = reader.ReadByte();
		}

		#endregion Constructors

		#region Internals and fields

		#endregion Internals and fields

		#region Properties

		public readonly Exterior Exterior;

		public readonly int FactionId;

		public readonly int Index;

		public readonly int NameSeed;

		/// <summary>
		/// Quality of the <see cref="Building"/>, from 1 to 20. Dividing by 4 gets a rating on the "rusty relics" to "incense burning" scale.
		/// </summary>
		public readonly int Quality;

		public readonly int Sector;

		public readonly BuildingType Type;

		#endregion Properties

		#region Methods

		#endregion Methods
	}
}
