using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daggerfall {
	public abstract class BsaRecord {
		public BsaRecord(int offset, int size) {
			this.Offset = offset;
			this.Size = size;
		}

		public readonly int Offset;
		public readonly int Size;
	}

    public class BsaRecord<TContents, TId, TArchive> : BsaRecord where TContents : class where TArchive : BsaArchive<TContents, TId, TArchive>
    {
		internal BsaRecord(TArchive archive, TId id, int offset, int size) : base(offset, size) {
			this.Archive = archive;
			this.Id = id;
		}

		public readonly TArchive Archive;
		public readonly TId Id;
        internal WeakReference<TContents> contents = new WeakReference<TContents>(null);

		/// <summary>
		/// Get the contents of this record.
		/// </summary>
		public TContents Contents {
			get {
				TContents result;
				if (!contents.TryGetTarget(out result)) {
					Archive.Reader.BaseStream.Position = Offset;
					result = Archive.Load(this);
					contents.SetTarget(result);
				}
				return result;
			}
		}

		public override string ToString() { return string.Format("{{{0}, {1}, {2}}}", Id, Offset, Size); }

		public static implicit operator TContents(BsaRecord<TContents, TId, TArchive> record) { return record.Contents; }
    }

	public abstract class BsaArchive<TValue, TId, TArchive> : StateObject where TValue : class where TArchive : BsaArchive<TValue, TId, TArchive> {
		#region Constructors

		public class Record : BsaRecord<TValue, TId, TArchive> {
			internal Record(TArchive archive, TId id, int offset, int size) : base(archive, id, offset, size) { }
		}

		public BsaArchive(State state, string path) : this(state, File.OpenRead(path)) { }

		public BsaArchive(State state, Stream stream) : this(state, new BinaryReader(stream)) { }

		public BsaArchive(State state, BinaryReader reader)
			: base(state) {
			if (reader == null) throw new ArgumentNullException("reader");
			Reader = reader;
			recordsReadOnly = new ReadOnlyDictionary<TId, Record>(records);

			var count = reader.ReadUInt16();
			var type = reader.ReadUInt16();
			bool names = UsesNames;

			if (type != TypeCode) throw new Exception();

			reader.BaseStream.Seek(-count * (names ? 18 : 8), SeekOrigin.End);
			int offset = 4;
			recordList = new Record[count];
			recordListReadOnly = new ReadOnlyCollection<Record>(recordList);
			for (int recordIndex = 0; recordIndex < count; recordIndex++) {
				TId id = ReadId();
				int size;

				size = reader.ReadInt32();
				Record record = recordList[recordIndex] = new Record((TArchive)this, id, offset, size);
				offset += size;

				records[id] = record;
			}
		}

		#endregion Constructors

		#region Internals and fields

		readonly Record[] recordList;
		readonly ReadOnlyCollection<Record> recordListReadOnly;
		readonly Dictionary<TId, Record> records = new Dictionary<TId, Record>();
		readonly ReadOnlyDictionary<TId, Record> recordsReadOnly;

		#endregion Internals and fields

		#region Properties

		public readonly BinaryReader Reader;

		/// <summary>Get the collection of records indexed by their Id.</summary>
		public ReadOnlyDictionary<TId, Record> Records { get { return recordsReadOnly; } }

		/// <summary>Get the flat collection of records in the order they appear in the file.</summary>
		public ReadOnlyCollection<Record> RecordList { get { return recordListReadOnly; } }

		protected abstract ushort TypeCode { get; }
		public bool UsesNames { get { return typeof(TId) == typeof(string); } }
		public bool UsesIndexes { get { return typeof(TId) == typeof(int); } }

		#endregion Properties

		#region Methods

		protected abstract TId ReadId();

		#endregion Methods

		protected abstract TValue Load(Record record);
		internal TValue Load(BsaRecord<TValue, TId, TArchive> record) { return Load((Record)record); }
	}

	public abstract class BsaInt32IdArchive<TRecord> : BsaArchive<TRecord, int, BsaInt32IdArchive<TRecord>> where TRecord : class {
		public BsaInt32IdArchive(State state, string path) : base(state, path) { }

		protected override ushort TypeCode { get { return 0x200; } }

		protected override int ReadId() { return Reader.ReadInt32(); }
	}

	public abstract class BsaStringIdArchive<TRecord> : BsaArchive<TRecord, string, BsaStringIdArchive<TRecord>> where TRecord : class {
		public BsaStringIdArchive(State state, string path) : base(state, path) { }

		protected override ushort TypeCode { get { return 0x100; } }

		protected override string ReadId() { return Reader.ReadNulTerminatedAsciiString(14); }
	}
}
