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
    /// Texture archives are in files like "TEXTURE.###"
    /// </summary>
    public class TextureArchive : PlainArchive<TextureSet, TextureArchive, TextureArchive.Record>
    {
        /// <summary>A record in the <see cref="TextureArchive"/>.</summary>
        public class Record : ArchiveRecord<TextureSet, TextureArchive, Record>
        {
            /// <summary>Unknown fields.</summary>
            public readonly Unknowns Unknowns = new Unknowns();

            internal Record(TextureArchive archive, BinaryReader reader, uint offsetBase)
                : base(archive, 0, null)
            {
                Unknowns = new Unknowns();
                Unknowns.AddUInt16("a", reader);
                Offset = reader.ReadUInt32() + offsetBase;
                Unknowns.AddUInt16("b", reader);
                Unknowns.AddInt32("c", reader);
                reader.ReadZeroes(8);
            }

            public override string ToString()
            {
                return string.Format("{0} {1}", Offset, Unknowns);
            }
        }

        /// <summary>Get a name or description of the <see cref="TextureArchive"/>.</summary>
        public string Name { get; protected set; }

        public TextureArchive(State state, string path)
            : base(state, path)
        {
        }

        protected override TextureSet Load(Record record)
        {
            return new TextureSet(this, Reader);
        }

        protected override void ReadHeaders()
        {
            int recordCount = Reader.ReadUInt16();
            this.Name = Reader.ReadNulTerminatedAsciiString(24).Trim();

            for (int index = 0; index < recordCount; index++)
                AddRecord(new Record(this, Reader, (uint)(0)));
        }
    }

    public class TextureArchiveList : StateObject
    {
        public WeakReference<TextureArchive>[] Archives = new WeakReference<TextureArchive>[512];

        public TextureArchiveList(State state) : base(state) {
        }

        public int Count { get { return Archives.Length; } }

        public TextureArchive this[int index]
        {
            get
            {
                var reference = Archives[index];
                TextureArchive archive;

                if (reference == null)
                    reference = Archives[index] = new WeakReference<TextureArchive>(null);

                if (!reference.TryGetTarget(out archive))
                {
                    string path = State.GetDataFileName(string.Format("TEXTURE.{0:D3}", index));
                    if (!File.Exists(path)) return null;
                    archive = new TextureArchive(State, path);
                    reference.SetTarget(archive);
                }

                return archive;
            }
        }
    }
}
