using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daggerfall {
	public struct ExteriorBlock {
		static readonly String[] PrefixStrings = new string[] { "TVRN", "GENR", "RESI", "WEAP", "ARMR", "ALCH", "BANK", "BOOK", "CLOT", "FURN", "GEMS", "LIBR", "PAWN", "TEMP", "TEMP", "PALA", "FARM", "DUNG", "CAST", "MANR", "SHRI", "RUIN", "SHCK", "GRVE", "FILL", "KRAV", "KDRA", "KOWL", "KMOO", "KCAN", "KFLA", "KHOR", "KROS", "KWHE", "KSCA", "KHAW", "MAGE", "THIE", "DARK", "FIGH", "CUST", "WALL", "MARK", "SHIP", "WITC" };

		static readonly string[] TempleNumberStrings = new string[] { "A0", "B0", "C0", "D0", "E0", "F0", "G0", "H0" };

		static readonly string[] CustomIndexStrings = new string[] { "AA", "BA", "AL", "BL", "AM", "BM", "AS", "BS", "GA", "GL", "GM", "GS" };

		internal byte prefix, number, character;

		public BlockIndexPrefix Prefix { get { return (BlockIndexPrefix)prefix; } }

		public string BlockName(ExteriorBlockIndexCustom custom) {
			string text = PrefixStrings[prefix];

			if(Prefix == BlockIndexPrefix.Temple || Prefix == BlockIndexPrefix.Temple2) {
				if(character > 7)
					text += "GA";
				else
					text += "AA";
				text += TempleNumberStrings[character & 7];
			} else {
				int customIndex = character >> 4;

				if(Prefix == BlockIndexPrefix.Custom) {
					switch(custom) {
						case ExteriorBlockIndexCustom.Default: customIndex = 0; break;
						case ExteriorBlockIndexCustom.Sentinel: customIndex = 8; break;
						case ExteriorBlockIndexCustom.Wayrest: customIndex = Math.Max(0, customIndex - 1); break;
						default: throw new Exception();
					}
				}

				text += CustomIndexStrings[customIndex];
				text += (char)((number >> 4) + '0');
				text += (char)((number & 15) + '0');
				text += ".RMB";
				if((number >> 4) > 9 || (number & 15) > 9) throw new Exception();
			}

			return text;
		}
	}

	public enum ExteriorBlockIndexCustom {
		Default,
		Sentinel,
		Wayrest,
	}

	public enum BlockIndexPrefix {
		Tavern = 0,
		GeneralStore,
		Residence,
		Weaponsmith,
		Armorer,
		Alchemist,
		Bank,
		Bookseller,
		Clothier,
		FurnitureStore,
		Jeweler,
		Library,
		PawnBroker,

		/// <summary>Triggers special logic for producing the letters.</summary>
		Temple,

		/// <summary>Triggers special logic for producing the letters.</summary>
		Temple2,

		Palace,
		Farm,
		Dungeon,
		Castle,
		Manor,
		Shrine,
		Ruins,
		Shack,
		Cemetery,
		Filler,
		OrderOfTheRaven,
		KnightsOfTheRaven,
		KnightsOfTheDragon,
		KnightsOfTheOwl,

		/// <summary>Unused</summary>
		KnightsOfTheMoon,
		OrderOfTheCandle,
		KnightsOfTheFlame,
		HostOfTheHorn,
		KnightsOfTheRose,
		KnightsOfTheWheel,
		OrderOfTheScarab,
		KnightsOfTheHawk,
		MagesGuild,
		ThievesGuild,
		DarkBrotherhood,
		FightersGuild,
		Custom,
		Wall,
		Market,
		Ship,
		Coven,
	}
}
