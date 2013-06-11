using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daggerfall
{
    public abstract class BsaArchive<TKey, TValue, TArchive>
        : KeyedArchive<TKey, TValue, TArchive, BsaArchive<TKey, TValue, TArchive>.Record>
        where TValue : class
        where TArchive : BsaArchive<TKey, TValue, TArchive>
    {
        #region Constructors

        public class Record : KeyedArchiveRecord<TKey, TValue, TArchive, Record>
        {
            internal Record(TArchive archive, TKey key, uint offset, uint size) : base(archive, key, offset, size) { }
        }

        public BsaArchive(State state, string path, BinaryReader reader) : base(state, path, reader) { }
        public BsaArchive(State state, string path, Stream stream) : base(state, path, stream) { }
        public BsaArchive(State state, string path) : base(state, path) { }

        protected override void ReadHeaders()
        {
            var reader = Reader;
            var count = reader.ReadUInt16();
            var type = reader.ReadUInt16();
            bool names = UsesNames;

            if (type != TypeCode) throw new Exception();

            reader.BaseStream.Seek(-count * (names ? 18 : 8), SeekOrigin.End);
            uint offset = 4;
            for (int recordIndex = 0; recordIndex < count; recordIndex++)
            {
                TKey id = ReadId();
                uint size;

                size = reader.ReadUInt32();
                AddRecord(new Record((TArchive)this, id, offset, size));
                offset += size;
            }
        }

        #endregion Constructors

        #region Internals and fields

        #endregion Internals and fields

        #region Properties

        protected abstract ushort TypeCode { get; }
        public bool UsesNames { get { return typeof(TKey) == typeof(string); } }
        public bool UsesIndexes { get { return typeof(TKey) == typeof(int); } }

        #endregion Properties

        #region Methods

        protected abstract TKey ReadId();

        #endregion Methods
    }

    public abstract class BsaInt32IdArchive<TValue> : BsaArchive<int, TValue, BsaInt32IdArchive<TValue>> where TValue : class
    {
        public BsaInt32IdArchive(State state, string path) : base(state, path) { }

        protected override ushort TypeCode { get { return 0x200; } }

        protected override int ReadId() { return Reader.ReadInt32(); }
    }

    public abstract class BsaStringIdArchive<TValue> : BsaArchive<string, TValue, BsaStringIdArchive<TValue>> where TValue : class
    {
        public BsaStringIdArchive(State state, string path) : base(state, path) { }

        protected override ushort TypeCode { get { return 0x100; } }

        protected override string ReadId() { return Reader.ReadNulTerminatedAsciiString(14); }
    }
}
