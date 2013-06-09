using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daggerfall {
	public class Region : StateObject {
		#region Constructors

		public Region(State state, int index)
			: base(state) {
			this.index = index;
		}

		#endregion Constructors

		#region Internals and fields

		readonly int index;
		ReadOnlyCollection<Location> locations;

		#endregion Internals and fields

		#region Properties

		/// <summary>Get the index of the region.</summary>
		public int Index { get { return index; } }

		/// <summary>Get the read-only collection of <see cref="Location"/>s in the <see cref="Region"/>.</summary>
		public ReadOnlyCollection<Location> Locations { get { return locations; } }

		#endregion Properties

		#region Methods

		internal void LoadMapNames(BinaryReader reader) {
			var locations = new Location[reader.ReadUInt32()];

			for(int index = 0; index < locations.Length; index++)
				locations[index] = new Location(this, index, reader.ReadNulTerminatedAsciiString(32));
			this.locations = new ReadOnlyCollection<Location>(locations);
		}

		internal void LoadMapPItem(BinaryReader reader) {
			int[] offsets = new int[locations.Count];
			for(int index = 0; index < offsets.Length; index++)
				offsets[index] = reader.ReadInt32();
			long baseOffset = reader.BaseStream.Position;

			for(int index = 0; index < offsets.Length; index++) {
				reader.BaseStream.Position = baseOffset + offsets[index];
				locations[index].LoadMapPItem(reader);
			}
		}

		internal void LoadMapDItem(BinaryReader reader) {
			int interiorCount = reader.ReadInt32();
			long start = reader.BaseStream.Position;

			for(int index = 0; index < interiorCount; index++) {
				reader.BaseStream.Position = start + index * 8;
				int offset = reader.ReadInt32();
				int one = reader.ReadUInt16();
				if(one != 1) throw new Exception();
				int exteriorId = reader.ReadUInt16();
				Location location = null;

				foreach(var compare in locations)
					if(compare.Exterior.Id == exteriorId) {
						location = compare;
						break;
					}
				if(location == null) throw new Exception();

				reader.BaseStream.Position = start + interiorCount * 8 + offset;
				location.LoadMapDItem(reader);
			}
		}

		#endregion Methods
	}

	public class RegionArchive : StateObject, IList<Region> {
		public RegionArchive(State state, string path)
			: base(state) {
			archive = new BaseArchive(state, path);
			regions = new Region[archive.Count / 4];
		}

		public int Count { get { return regions.Length; } }

		public Region this[int index] {
			get {
				if(regions[index] == null) {
					var region = regions[index] = new Region(State, index);
					string extension = string.Format(".{0:D3}", index);
					archive.region = region;
					var boop = archive[MapNames + extension];
					boop = archive[MapTable + extension];
					boop = archive[MapPItem + extension];
					boop = archive[MapDItem + extension];
				}
				return regions[index];
			}
		}

		Region[] regions;
		readonly BaseArchive archive;

		const string MapNames = "MAPNAMES", MapTable = "MAPTABLE", MapPItem = "MAPPITEM", MapDItem = "MAPDITEM";

		class BaseArchive : BsaArchive<object> {
			internal Region region;

			public BaseArchive(State state, string path) : base(state, path) { }

			protected override object Load(BinaryReader reader, BsaArchiveRecordInfo record) {
				if(record.Name.StartsWith(MapNames))
					region.LoadMapNames(reader);
				else if(record.Name.StartsWith(MapTable)) {
					foreach(var location in region.Locations)
						location.LoadMapTable(reader);
				} else if(record.Name.StartsWith(MapPItem)) {
					region.LoadMapPItem(reader);
				} else if(record.Name.StartsWith(MapDItem)) {
					region.LoadMapDItem(reader);
				} else
					throw new NotImplementedException();
				return null;
			}
		}

		public int IndexOf(Region item) { return ((IList<Region>)regions).IndexOf(item); }
		public bool Contains(Region item) { return IndexOf(item) >= 0; }

		public void CopyTo(Region[] array, int arrayIndex) {
			for(int index = 0; index < regions.Length; index++)
				array[arrayIndex + index] = this[index];
		}

		public bool IsReadOnly { get { return true; } }

		public IEnumerator<Region> GetEnumerator() {
			for(int index = 0; index < regions.Length; index++)
				yield return this[index];
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return GetEnumerator(); }

		Exception ReadOnlyException() { return new NotSupportedException("The region archive is read-only."); }
		void IList<Region>.Insert(int index, Region item) { throw ReadOnlyException(); }
		void IList<Region>.RemoveAt(int index) { throw ReadOnlyException(); }
		Region IList<Region>.this[int index] { get { return this[index]; } set { throw ReadOnlyException(); } }
		void ICollection<Region>.Add(Region item) { throw ReadOnlyException(); }
		void ICollection<Region>.Clear() { throw ReadOnlyException(); }
		bool ICollection<Region>.Remove(Region item) { throw ReadOnlyException(); }
	}
}
