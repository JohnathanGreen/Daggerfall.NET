using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Daggerfall
{
    /// <summary>
    /// An individual frame in a <see cref="TextureSet"/>, or a standalone image file.
    /// </summary>
    public class Image : StateObject
    {
        readonly WeakReference<Texture2D> texture = new WeakReference<Texture2D>(null);

        public readonly byte[] Data;

        public readonly Point Dimensions;

        /// <summary>Get the <see cref="Daggerfall.Palette"/> that this <see cref="Image"/> uses.</summary>
        public readonly Palette Palette;

        public int PixelCount { get { return Dimensions.X * Dimensions.Y; } }

        /// <summary>The <see cref="TextureSet"/> this <see cref="Image"/> is in, or <c>null</c> if this is not a texture frame.</summary>
        public readonly TextureSet Set;

        /// <summary>Get the RGBA <see cref="Texture2D"/> for this <see cref="Image"/>, creating it if necessary.</summary>
        public Texture2D Texture
        {
            get
            {
                Texture2D texture;

                this.texture.TryGetTarget(out texture);
                if (texture == null)
                {
                    texture = new Texture2D(Graphics, Dimensions.X, Dimensions.Y);

                    Color[] palette = Palette.Colors;
                    Color[] colors = new Color[Dimensions.X * Dimensions.Y];

#if NoUnsafe
                    for (int index = 0, count = Dimensions.X * Dimensions.Y; index < count; index++)
                        colors[index] = palette[Data[index]];
#else
                    unsafe
                    {
                        fixed(byte* dataPointer = Data)
                        fixed(Color* palettePointer = palette)
                        fixed(Color* colorPointer = colors) {
                            byte* data = dataPointer;
                            for (Color* pointer = colorPointer, end = colorPointer + PixelCount; pointer < end; pointer++, data++)
                                *pointer = palettePointer[*data];
                        }
                    }
#endif

                    texture.SetData(colors);
                    this.texture.SetTarget(texture);
                }
                return texture;
            }
        }

        public Image(TextureSet set, Point dimensions, byte[] data, Palette palette) : base(set.State)
        {
            if (set == null) throw new ArgumentNullException("set");
            if (data == null) throw new ArgumentNullException("data");
            if (dimensions.X * dimensions.Y != data.Length) throw new ArgumentException("data");
            if (palette == null) throw new ArgumentNullException("palette");

            this.Set = set;
            this.Dimensions = dimensions;
            this.Data = data;
            this.Palette = palette;
        }

        public override void Dispose() {
            Texture2D texture;
            this.texture.TryGetTarget(out texture);
            if (texture != null && !texture.IsDisposed) texture.Dispose();
        }
    }
}
