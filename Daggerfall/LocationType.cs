using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daggerfall {
	public enum LocationType : byte {
		/// <summary>City, colored light rose.</summary>
		City = 0x80,

		/// <summary>Town, colored medium rose.</summary>
		Town = 0x81,

		/// <summary>Village, colored medium rose.</summary>
		Village = 0x82,

		/// <summary>Farmstead, colored medium rose.</summary>
		Farmstead = 0x83,

		/// <summary>Major dungeons such as labyrinths. Colored orange.</summary>
		MajorDungeon = 0x84,

		/// <summary>Temple, colored light blue.</summary>
		Temple = 0x85,

		/// <summary>Tavern, colored dark rose.</summary>
		Tavern = 0x86,

		/// <summary>Large dungeons such as fortresses and castles. Colored dark orange.</summary>
		LargeDungeon = 0x87,

		/// <summary>Manor, colored light pink.</summary>
		Manor = 0x88,

		/// <summary>Shrine, colored dark blue.</summary>
		Shrine = 0x89,

		/// <summary>Small dungeons such as ruins and hovels. Colored medium red.</summary>
		SmallDungeon = 0x8A,

		Shack = 0x8B,

		/// <summary>An ancient graveyard. Colored dark red. When already known on the map, it's a common graveyard colored dark pink.</summary>
		AncientGraveyard = 0x8C,

		/// <summary>Coven, colored black.</summary>
		Coven = 0x8D,

		/// <summary>Player's ship; invisible.</summary>
		Ship = 0x8E,

		KnownMask = 0x40,
	}
}
