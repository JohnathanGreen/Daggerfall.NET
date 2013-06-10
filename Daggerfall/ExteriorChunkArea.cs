using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daggerfall
{
    public class ExteriorChunkArea : StateObject
    {
        public readonly ExteriorChunk Chunk;
        public readonly Unknowns Unknowns = new Unknowns();
        public ReadOnlyCollection<ExteriorChunkModel> Models { get; protected set; }
        public ReadOnlyCollection<ExteriorChunkFlat> Flats { get; protected set; }
        public ReadOnlyCollection<ExteriorChunkSection> Sections { get; protected set; }
        public ReadOnlyCollection<ExteriorChunkPerson> Persons { get; protected set; }
        public ReadOnlyCollection<ExteriorChunkDoor> Doors { get; protected set; }

        internal ExteriorChunkArea(ExteriorChunk chunk, bool isExterior, BinaryReader reader)
            : base(chunk.State)
        {
            Chunk = chunk;

            byte modelCount = reader.ReadByte();
            byte flatCount = reader.ReadByte();
            byte sectionCount = reader.ReadByte();
            byte personCount = reader.ReadByte();
            byte doorCount = reader.ReadByte();
            Unknowns.Add("a", reader.ReadUInt16());
            Unknowns.Add("b", reader.ReadUInt16()); // always non-zero
            Unknowns.Add("c", reader.ReadUInt16());
            Unknowns.Add("d", reader.ReadUInt16()); // always non-zero
            Unknowns.Add("e", reader.ReadUInt16()); // always non-zero
            Unknowns.Add("f", reader.ReadUInt16()); // always non-zero

            ExteriorChunkModel[] models = new ExteriorChunkModel[modelCount];
            for (int index = 0; index < modelCount; index++)
                models[index] = new ExteriorChunkModel(this, index, reader);
            Models = new ReadOnlyCollection<ExteriorChunkModel>(models);

            ExteriorChunkFlat[] flats = new ExteriorChunkFlat[flatCount];
            for (int index = 0; index < flatCount; index++)
                flats[index] = new ExteriorChunkFlat(this, index, reader);
            Flats = new ReadOnlyCollection<ExteriorChunkFlat>(flats);

            ExteriorChunkSection[] sections = new ExteriorChunkSection[sectionCount];
            for (int index = 0; index < sectionCount; index++)
                sections[index] = new ExteriorChunkSection(this, index, reader);
            Sections = new ReadOnlyCollection<ExteriorChunkSection>(sections);

            ExteriorChunkPerson[] persons = new ExteriorChunkPerson[personCount];
            for (int index = 0; index < personCount; index++)
                persons[index] = new ExteriorChunkPerson(this, index, reader);
            Persons = new ReadOnlyCollection<ExteriorChunkPerson>(persons);

            ExteriorChunkDoor[] doors = new ExteriorChunkDoor[doorCount];
            for (int index = 0; index < doorCount; index++)
                doors[index] = new ExteriorChunkDoor(this, index, reader);
            Doors = new ReadOnlyCollection<ExteriorChunkDoor>(doors);
            if (doorCount > 0 || sectionCount > 0 || personCount > 0)
            {
            }

        }

        public void Draw(BasicEffect effect, ref Matrix world)
        {
            foreach (var model in Models)
                model.Draw(effect, ref world);
        }
    }
}
