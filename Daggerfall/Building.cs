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

		internal Building(Exterior exterior, ExteriorBlock block, int index, BinaryReader reader) 
			: base(((StateObject)block ?? exterior).State) {
			Exterior = exterior;
			Block = block;
			Index = index;

			NameSeed = reader.ReadUInt16();
			reader.ReadZeroes(16);
			FactionId = new FactionIndex(reader.ReadUInt16());
			Sector = reader.ReadInt16();
			var locationId = reader.ReadUInt16();
			if(exterior != null && locationId != exterior.Id) throw new Exception();
			if (exterior == null && locationId != 0) throw new Exception();
			Type = (BuildingType)reader.ReadByte();
			Quality = reader.ReadByte();
		}

		#endregion Constructors

		#region Internals and fields

		#endregion Internals and fields

		#region Properties

		/// <summary>Get the <see cref="Daggerfall.Block"/> that this <see cref="Building"/> is in, or <c>null</c> if this <see cref="Building"/> is in an <see cref="Daggerfall.Exterior"/> (accessed by <see cref="Exterior"/>).</summary>
		public readonly Block Block;

		/// <summary>Get the <see cref="Daggerfall.Exterior"/> that this <see cref="Building"/> is in, or <c>null</c> if this <see cref="Building"/> is in a <see cref="Daggerfall.Block"/> (accessed by <see cref="Block"/>).</summary>
		public readonly Exterior Exterior;

		public readonly FactionIndex FactionId;

		public readonly int Index;

		public readonly int NameSeed;

		/// <summary>Get the quality of the <see cref="Building"/>, from 1 to 20. Dividing by 4 gets a rating on the "rusty relics" to "incense burning" scale.</summary>
		public readonly int Quality;

		public readonly int Sector;

		public readonly BuildingType Type;

		#endregion Properties

		#region Methods

		#endregion Methods
	}
}
