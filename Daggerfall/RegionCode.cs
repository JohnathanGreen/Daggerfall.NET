using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daggerfall {
	public enum RegionCode : byte {
		AlikrDesert,
		DragontailMountains,

		/// <summary>Unused</summary>
		GlenpointFoothills,

		/// <summary>Unused</summary>
		DaggerfallBluffs,

		YeorthBurrowland,
		Dwynnen,

		/// <summary>Unused</summary>
		RavennianForest,

		/// <summary>Unused</summary>
		Devilrock,

		/// <summary>Unused</summary>
		MaleknaForest,

		BalfieraIsle,

		/// <summary>Unused</summary>
		Bantha,

		Dakfron,

		/// <summary>Unused</summary>
		IslandsInTheWesternIliacBay,

		/// <summary>Unused</summary>
		TamarilynPoint,

		/// <summary>Unused</summary>
		LainlynCliffs,

		/// <summary>Unused</summary>
		BjoulsaeRiver,

		WrothgarianMountains,
		Daggerfall,
		Glenpoint,
		Betony,
		Sentinel,
		Anticlere,
		Lainlyn,
		Wayrest,

		/// <summary>Unused</summary>
		GenTemHighRockVillage,

		/// <summary>Unused</summary>
		GenRaiHammerfellVillage,
		OrsiniumArea,
		SkeffingtonWood,

		/// <summary>Unused</summary>
		HammerfellBayCoast,

		/// <summary>Unused</summary>
		HammerfellSeaCoast,

		/// <summary>Unused</summary>
		HighRockBayCoast,

		HighRockSeaCoast,
		Northmoor,
		Menevia,
		Alcaire,
		Koegria,
		Bhoriane,
		Kambria,
		Phrygias,
		Urvaius,
		Ykalon,
		Daenia,
		Shalgora,
		AbibonGora,
		Kairou,
		Pothago,
		Myrkwasa,
		Ayasofya,
		Tigonus,
		Kozanset,
		Satakalaam,
		Totambu,
		Mournoth,
		Ephesus,
		Santaki,
		Antiphyllos,
		Bergama,
		Gavaudon,
		Tulune,
		GlenumbraMoors,
		IlessanHills,
		Cybiades,

		/// <summary>This region doesn't exist but is a group of other regions.</summary>
		Vraseth,

		/// <summary>This region doesn't exist but is a group of other regions.</summary>
		Haarvenu,

		/// <summary>This region doesn't exist but is a group of other regions.</summary>
		Thrafey,

		/// <summary>This region doesn't exist but is a group of other regions.</summary>
		Lyrezi,

		/// <summary>This region doesn't exist but is a group of other regions.</summary>
		Montalion,

		/// <summary>This region doesn't exist but is a group of other regions.</summary>
		Khulari,

		/// <summary>This region doesn't exist but is a group of other regions.</summary>
		Garlythi,

		/// <summary>This region doesn't exist but is a group of other regions.</summary>
		Anthotis,

		/// <summary>This region doesn't exist but is a group of other regions.</summary>
		Selenu,

		/// <summary>Mistakenly used for an enclave in the <see cref="WrothgarianMountains"/>.</summary>
		Unknownregion = 105,
	}
}
