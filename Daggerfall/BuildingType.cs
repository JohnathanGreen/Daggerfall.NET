using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daggerfall {
	public enum BuildingType : byte {
		Alchemist = 0x00,
		HouseForSale = 0x01,
		Armorer = 0x02,
		Bank = 0x03,
		Town1 = 0x04,
		Bookseller = 0x05,
		ClothingStore = 0x06,
		FurnitureStore = 0x07,
		GemStore = 0x08,
		GeneralStore = 0x09,
		Library = 0x0A,
		Guildhall = 0x0B,
		PawnShop = 0x0C,
		WeaponSmith = 0x0D,
		Temple = 0x0E,
		Tavern = 0x0F,
		Palace = 0x10,

		/// <summary>Entry doors are always locked.</summary>
		House1 = 0x11,

		/// <summary>Has unlocked entry doors from 0600 to 1800.</summary>
		House2 = 0x12,

		House3 = 0x13,
		House4 = 0x14,
		House5 = 0x15,
		House6 = 0x16,
		Town2 = 0x17,

		/// <summary>Never displayed on the automap.</summary>
		Ship = 0x18,

		/// <summary>Never displayed on the automap.</summary>
		Special1 = 0x74,

		/// <summary>Never displayed on the automap.</summary>
		Special2 = 0xDF,

		/// <summary>Never displayed on the automap.</summary>
		Special3 = 0xF9,

		/// <summary>Never displayed on the automap.</summary>
		Special4 = 0xFA,
	}
}
