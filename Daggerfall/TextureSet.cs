using Microsoft.Xna.Framework;
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
    /// A collection of <see cref="Image"/>s.
    /// </summary>
    public class TextureSet : StateObject
    {
        ConcealableList<Image> frames = new ConcealableList<Image>();

        /// <summary>Get the dimensions of the texture in pixels.</summary>
        public readonly Point Dimensions;

        public ReadOnlyCollection<Image> Frames { get { return frames; } }

        /// <summary>Get the display offset for the image.</summary>
        public readonly Point Offset;

        /// <summary>Get the value to multiply against the <see cref="Dimensions"/> of the texture, then divided by 256, to get the size of the texture when it's used as a flat.</summary>
        public readonly Point Scale;

        public readonly Unknowns Unknowns = new Unknowns();

        public TextureSet(TextureArchive archive, BinaryReader reader)
            : base(archive.State)
        {
            long start = reader.BaseStream.Position;
            Offset = new Point(reader.ReadInt16(), reader.ReadInt16());
            Dimensions = new Point(reader.ReadUInt16(), reader.ReadUInt16());
            var compression = (ImageCompression)reader.ReadUInt16();

            var totalRecordSize = reader.ReadUInt32();
            var dataOffset = start + reader.ReadUInt32();
            Unknowns.AddUInt16("normal", reader);
            var frameCount = reader.ReadUInt16();
            Unknowns.AddUInt16("a", reader);
            var palette = State.ArtPalette;

            reader.BaseStream.Position = dataOffset;
            switch (compression)
            {
                case (ImageCompression)2304:
                case (ImageCompression)256:
                case (ImageCompression)257:
                case ImageCompression.Uncompressed:
                    if (frameCount == 1)
                    {
                        byte[] data = new byte[Dimensions.X * Dimensions.Y];

                        for (int y = 0; y < Dimensions.Y; y++)
                        {
                            reader.Read(data, y * Dimensions.X, Dimensions.X);
                            reader.BaseStream.Seek(256 - Dimensions.X, SeekOrigin.Current);
                        }

                        frames.Add(new Image(this, Dimensions, data, palette));
                    }
                    else
                    {
                        uint[] offsets = reader.ReadUInt32Array(frameCount);
                        for (int frameIndex = 0; frameIndex < frameCount; frameIndex++)
                        {
                            reader.BaseStream.Position = dataOffset + offsets[frameIndex];
                            var dimensions = new Point(reader.ReadUInt16(), reader.ReadUInt16());
                            var data = new byte[dimensions.X * dimensions.Y];
                            int runCount;// = reader.ReadByte();
                            bool isZero = true;

                            for (int y = 0; y < dimensions.Y; y++)
                            {
                                runCount = reader.ReadByte();
                                isZero = true;
                                for (int x = 0; x < dimensions.X; isZero = !isZero)
                                {
                                    if (!isZero)
                                    {
                                        if (dimensions.X - runCount < x)
                                            runCount = dimensions.X - x;
                                        if (reader.BaseStream.Position > reader.BaseStream.Length - runCount) { y = dimensions.Y; break; }
                                        reader.Read(data, x + y * dimensions.X, runCount);
                                    }

                                    x += runCount;
                                    if (x < dimensions.X || (x == dimensions.X && isZero))
                                    {
                                        if (reader.BaseStream.Position >= reader.BaseStream.Length) { y = dimensions.Y; break; }
                                        runCount = reader.ReadByte();
                                    }

                                    /*if (x == dimensions.X && isZero)
                                    {
                                    }*/
                                }
                            }

                            frames.Add(new Image(this, dimensions, data, palette));
                        }
                    }
                    break;

                case ImageCompression.RecordRle:
                case ImageCompression.ImageRle:
                    {
                        if (frameCount != 1)
                            throw new Exception();
                        uint[] rowData = reader.ReadUInt32Array(Dimensions.Y);
                        byte[] data = new byte[Dimensions.X * Dimensions.Y];

                        for (int y = 0; y < Dimensions.Y; y++)
                        {
                            reader.BaseStream.Position = (rowData[y] & 0xFFFF) + dataOffset;
                            switch (rowData[y] >> 16)
                            {
                                case 0x0000:
                                    reader.Read(data, y * Dimensions.X, Dimensions.X);
                                    break;

                                case 0x8000:
                                    for (int offset = y * Dimensions.X, end = offset + reader.ReadUInt16(); offset < end; )
                                    {
                                        int runCount = reader.ReadInt16();
                                        if (runCount < 0)
                                        {
                                            runCount = -runCount;
                                            byte value = reader.ReadByte();
                                            for (int spanEnd = offset + runCount; offset < spanEnd; )
                                                data[offset++] = value;
                                        }
                                        else
                                        {
                                            reader.Read(data, offset, runCount);
                                            offset += runCount;
                                        }
                                    }
                                    break;

                                default:
                                    throw new Exception();
                            }
                        }
                    }
                    break;

                default:
                    throw new Exception();
            }
        }
    }

    enum ImageCompression
    {
        Uncompressed = 0,
        RleCompressed = 2,
        ImageRle = 0x108,
        RecordRle = 0x1108,
    }
}
