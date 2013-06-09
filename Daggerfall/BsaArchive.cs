using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daggerfall {
    public class BsaArchiveRecordInfo
    {
		public BsaArchiveRecordInfo(string name, int id, int offset, int size) {
			Name = name;
			Id = id;
			Offset = offset;
			Size = size;
		}

		public readonly string Name;
		public readonly int Id;
		public readonly int Offset;
		public readonly int Size;
    }

	public abstract class BsaArchive<TRecord> : StateObject where TRecord : class {
		class Record : BsaArchiveRecordInfo {
            public Record(string name, int id, int offset, int size) : base(name, id, offset, size)
            {
            }

			public WeakReference<TRecord> Value = null;
		}

		public BsaArchive(State state, string path) : this(state, File.OpenRead(path)) { }

		public BsaArchive(State state, Stream stream) : this(state, new BinaryReader(stream)) { }

		public BsaArchive(State state, BinaryReader reader) : base(state) {
			if(reader == null) throw new ArgumentNullException("reader");
			Reader = reader;

			var count = reader.ReadUInt16();
			var type = reader.ReadUInt16();
			bool names;

			if(type == 0x100) names = true;
			else if(type == 0x200) names = false;
			else throw new Exception();
			UsesNames = names;

			reader.BaseStream.Seek(-count * (names ? 18 : 8), SeekOrigin.End);
			int offset = 4;
			Record[] records = Records = new Record[count];
			for(int recordIndex = 0; recordIndex < count; recordIndex++) {
				string name = null;
				int id = -1, size;

				if(names)
					name = reader.ReadNulTerminatedAsciiString(14);
				else 
					id = reader.ReadInt32();

				size = reader.ReadInt32();
				Record record = records[recordIndex] = new Record(name, id, offset, size);
				offset += size;

				if(names)
					RecordsByName[name] = record;
				else
					RecordsById[id] = record;
			}
		}

		public readonly BinaryReader Reader;
		readonly Record[] Records;
		readonly Dictionary<string, Record> RecordsByName = new Dictionary<string, Record>();
		readonly Dictionary<int, Record> RecordsById = new Dictionary<int, Record>();

		public readonly bool UsesNames;
		public bool UsesIndexes { get { return !UsesNames; } }

		TRecord Load(Record record) {
			TRecord value = null;
			bool result = record.Value != null && record.Value.TryGetTarget(out value);

			if(!result) {
				Reader.BaseStream.Position = record.Offset;
				record.Value = new WeakReference<TRecord>(value = Load(Reader, record));
			}
			return value;
		}

		protected abstract TRecord Load(BinaryReader reader, BsaArchiveRecordInfo record);

		public TRecord this[int id] { get { return Load(RecordsById[id]); } }
		public TRecord this[string name] { get { return Load(RecordsByName[name]); } }

		public TRecord GetRecordAtIndex(int index) {
			return Load(Records[index]);
		}

        /// <summary>Get the number of records in the archive.</summary>
        public int Count { get { return Records.Length; } }
	}
}
