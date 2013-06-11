using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daggerfall
{
    /// <summary>
    /// A record in an <see cref="Archive"/> type.
    /// </summary>
    public abstract class ArchiveRecord : StateObject
    {
        /// <summary>Get the archive this is in.</summary>
        public readonly Archive Archive;

        /// <summary>Get the zero-based offset of the record in the archive.</summary>
        public uint Offset { get; protected set; }
        public uint? Size { get; protected set; }

        public ArchiveRecord(Archive archive) : base(archive.State) {
            Archive = archive;
        }
    }

    /// <summary>
    /// A record in an <see cref="Archive"/> type.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public abstract class ArchiveRecord<TValue, TArchive, TRecord> : ArchiveRecord
        where TValue : class
        where TArchive : Archive<TValue, TArchive, TRecord>
        where TRecord : ArchiveRecord<TValue, TArchive, TRecord>
    {
        readonly WeakReference<TValue> value = new WeakReference<TValue>(null);

        /// <summary>Get the <see cref="Archive"/> that this is in.</summary>
        public new TArchive Archive { get { return (TArchive)base.Archive; } }

        protected ArchiveRecord(TArchive archive, uint offset, uint? size)
            : base(archive)
        {
            Offset = offset;
            Size = size;
        }

        /// <summary>
        /// Get the contents of this record, loading them if necessary.
        /// </summary>
        public TValue Value
        {
            get
            {
                TValue result;
                if (!value.TryGetTarget(out result))
                {
                    Archive.Reader.BaseStream.Position = Offset;
                    result = Archive.Load(this);
                    value.SetTarget(result);
                }
                return result;
            }
        }

        public override string ToString() { return string.Format("{1}+{2}", Offset, Size); }

        public static implicit operator TValue(ArchiveRecord<TValue, TArchive, TRecord> record) { return record.Value; }
    }

    public abstract class KeyedArchiveRecord<TKey, TValue, TArchive, TRecord> : ArchiveRecord<TValue, TArchive, TRecord>
        where TValue : class
        where TArchive : KeyedArchive<TKey, TValue, TArchive, TRecord>
        where TRecord : KeyedArchiveRecord<TKey, TValue, TArchive, TRecord>
    {
        /// <summary>Get the identifying key of this record.</summary>
        public TKey Key { get; protected set; }

        public KeyedArchiveRecord(TArchive archive, TKey key, uint offset, uint? size) : base(archive, offset, size) {
            Key = key;
        }

        public override string ToString() { return string.Format("{{{0}, {1}+{2}}}", Key, Offset, Size); }
    }

    /// <summary>Base class of the archive files.</summary>
    public abstract class Archive : StateObject, IDisposable
    {
        /// <summary>The binary reader for accessing the archive.</summary>
        public readonly BinaryReader Reader;

        /// <summary>Get the full file path of the <see cref="Archive"/>.</summary>
        public readonly string Path;

        public Archive(State state, string path, BinaryReader reader) : base(state) {
            if (path == null) throw new ArgumentNullException("path");
            this.Path = path;
            this.Reader = reader;
            ReadHeaders();
        }

        public Archive(State state, string path, Stream stream) : this(state, path, new BinaryReader(stream)) { }
        public Archive(State state, string path) : this(state, path, File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read)) { }

        public override void Dispose() { base.Dispose(); Reader.Dispose(); }

        protected abstract void ReadHeaders();
    }

    public abstract class Archive<TValue, TArchive, TRecord> : Archive
        where TValue : class
        where TArchive : Archive<TValue, TArchive, TRecord>
        where TRecord : ArchiveRecord<TValue, TArchive, TRecord>
    {
        /// <summary>Get the collection of records in the archive.</summary>
        readonly ConcealableList<TRecord> records = new ConcealableList<TRecord>();

        /// <summary>Get the list of records in the archive.</summary>
        public ReadOnlyCollection<TRecord> Records { get { return records; }}

        public IEnumerable<TValue> Values
        {
            get
            {
                foreach (TRecord record in records)
                    yield return record.Value;
            }
        }

        public Archive(State state, string path, BinaryReader reader) : base(state, path, reader) { }
        public Archive(State state, string path, Stream stream) : base(state, path, stream) { }
        public Archive(State state, string path) : base(state, path) { }

        internal TValue Load(object record) { return Load((TRecord)record); }
        protected abstract TValue Load(TRecord record);

        protected virtual void AddRecord(TRecord record) {
            records.Add(record);
        }
    }

    /// <summary>An archive that is purely a collection of records in a row.</summary>
    /// <typeparam name="TValue"></typeparam>
    public abstract class PlainArchive<TValue, TArchive, TRecord> : Archive<TValue, TArchive, TRecord>
        where TValue : class
        where TArchive : PlainArchive<TValue, TArchive, TRecord>
        where TRecord : ArchiveRecord<TValue, TArchive, TRecord>
    {
        public PlainArchive(State state, string path, BinaryReader reader) : base(state, path, reader) { }
        public PlainArchive(State state, string path, Stream stream) : base(state, path, stream) { }
        public PlainArchive(State state, string path) : base(state, path) { }
    }

    public abstract class KeyedArchive<TKey, TValue, TArchive, TRecord> : Archive<TValue, TArchive, TRecord>
        where TValue : class
        where TArchive : KeyedArchive<TKey, TValue, TArchive, TRecord>
        where TRecord : KeyedArchiveRecord<TKey, TValue, TArchive, TRecord> {
        readonly ConcealableDictionary<TKey, TRecord> recordMap = new ConcealableDictionary<TKey,TRecord>();

        /// <summary>Get the list of records indexed by their key.</summary>
        public ReadOnlyDictionary<TKey, TRecord> RecordMap { get { return recordMap; } }

        public KeyedArchive(State state, string path, BinaryReader reader) : base(state, path, reader) { }
        public KeyedArchive(State state, string path, Stream stream) : base(state, path, stream) { }
        public KeyedArchive(State state, string path) : base(state, path) { }

        protected override void AddRecord(TRecord record)
        {
            base.AddRecord(record);
            recordMap[record.Key] = record;
        }
    }
}
