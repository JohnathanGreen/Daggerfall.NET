using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daggerfall
{
    public class Unknowns : List<Unknowns.Value>
    {
        public class Value
        {
            Value(string label, int valueInt32, byte[] valueArray) { unknownInt32 = valueInt32; unknownArray = valueArray; this.label = label; }
            public Value(string label, int value) : this(label, value, null) { }
            public Value(string label, byte[] value) : this(label, 0, value) { }
            int unknownInt32;
            byte[] unknownArray;
            string label;

            public override string ToString()
            {
                string text = "";

                if (label != null)
                {
                    text += label;
                    if (unknownArray == null)
                        text += ":";
                }

                if (unknownArray != null)
                    return text + unknownArray.ToHexString();
                return text + unknownInt32.ToHexValue();
            }
        }

        public void Add(int value) { Add(null, value); }
        public void Add(byte[] value) { Add(null, value); }

        public void Add(string label, int value) { Add(new Value(label, value)); }
        public void Add(string label, byte[] value) { Add(new Value(label, value)); }

        public void AddByte(string label, BinaryReader reader) { Add(label, reader.ReadByte()); }
        public void AddBytes(string label, BinaryReader reader, int count) { Add(label, reader.ReadBytes(count)); }
        public void AddUInt16(string label, BinaryReader reader) { Add(label, reader.ReadUInt16()); }
        public void AddInt32(string label, BinaryReader reader) { Add(label, reader.ReadInt32()); }

        public override string ToString()
        {
            string text = "{";
            foreach (var value in this)
            {
                if (text.Length > 1) text += ", ";
                text += value.ToString();
            }
            return text + "}";
        }
    }
}
