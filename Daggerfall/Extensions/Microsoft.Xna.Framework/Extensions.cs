using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Xna.Framework
{
    public static class Extensions
    {
        #region SpriteBatch

#if false
        public static void DrawRectangle(this SpriteBatch batch, Vector2 start, Vector2 size, Color color)
        {
            DrawRectangle(batch, start, size, null, color, 0);
        }

        public static void DrawRectangle(this SpriteBatch batch, Rectangle area, Color color)
        {
            DrawRectangle(batch, area, null, color, 0);
        }

        public static void DrawRectangle(this SpriteBatch batch, Vector2 start, Vector2 size, Texture2D texture, Color color, float layerDepth)
        {
            if (texture == null)
                texture = batch.Platform.WhiteTexture;
            batch.Draw(texture, start, null, color, 0, Vector2.Zero, size / new Vector2(texture.Width, texture.Height), layerDepth);
        }

        public static void DrawRectangle(this SpriteBatch batch, Rectangle area, Texture2D texture, Color color, float layerDepth)
        {
            batch.DrawRectangle(area.TopLeft, area.Size, texture, color, layerDepth);
        }
#endif

        public static void DrawAlignedString(this SpriteBatch batch, SpriteFont font, string text, Vector2 start, Color color, Alignment horizontalAlignment)
        {
            batch.DrawAlignedString(font, text, start, batch.GraphicsDevice.Viewport.Width, color, horizontalAlignment);
        }

        public static void DrawAlignedString(this SpriteBatch batch, SpriteFont font, string text, Vector2 start, float endX, Color color, Alignment horizontalAlignment)
        {
            Alignment partialAlignment = horizontalAlignment == Alignment.Full ? Alignment.Near : horizontalAlignment;
            batch.DrawAlignedString(font, text, new Rectangle((int)start.X, (int)start.Y, (int)(endX - start.X), (int)start.Y + 100000), color, new BlockAlignment(horizontalAlignment, partialAlignment, Alignment.Near));
        }

        /// <summary>
        /// Draw a string with formatting.
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="font"></param>
        /// <param name="label"></param>
        /// <param name="window"></param>
        /// <param name="color"></param>
        /// <param name="format"></param>
        public static void DrawAlignedString(this SpriteBatch batch, SpriteFont font, string text, Rectangle area, Color color, BlockAlignment format)
        {
            if (batch == null)
                throw new ArgumentNullException("batch");
            if (font == null)
                throw new ArgumentNullException("font");
            if (text == null)
                throw new ArgumentNullException("text");

            float spaceWidth = font.MeasureString(" ").X;
            int lineCount = FormattedLineCount(font, text, area.Width);
            float y = area.Top, advance = font.LineSpacing;

            switch (format.VerticalAlignment)
            {
                case Alignment.Near:
                    y = area.Top;
                    break;

                case Alignment.Center:
                    y = (area.Height - lineCount * advance) / 2 + area.Top;
                    break;

                case Alignment.Far:
                    y = area.Bottom - lineCount * advance;
                    break;

                case Alignment.Full:
                    if (lineCount > 1)
                        advance = (area.Height - advance) / (lineCount - 1);
                    else
                        y = (area.Height - advance) / 2 + area.Top;
                    break;

                default:
                    throw new Exception("Unknown or invalid vertical alignment value " + format.VerticalAlignment);
            }

            for (int lineStart = 0; lineStart < text.Length; )
            {
                int spaceCount = 0;
                float totalLength = 0;
                int lineEnd = lineStart;
                Alignment alignment = format.HorizontalAlignment == Alignment.Full ? format.HorizontalPartialAlignment : format.HorizontalAlignment;

                while (lineEnd < text.Length && text[lineEnd] != '\n')
                {
                    int wordStart = lineEnd;
                    SkipWhite(text, ref lineEnd);
                    int spaceAdd = lineEnd - wordStart;
                    SkipNonWhite(text, ref lineEnd);

                    float add = font.MeasureString(text.Substring(wordStart, lineEnd - wordStart)).X;

                    if (totalLength > 0 && totalLength + add >= area.Width)
                    {
                        lineEnd = wordStart;
                        SkipWhite(text, ref lineEnd);
                        alignment = format.HorizontalAlignment;
                        break;
                    }
                    spaceCount += spaceAdd;
                    totalLength += add;
                }

                DrawTextLine(batch, font, text.Substring(lineStart, lineEnd - lineStart), area, color, format, y, spaceCount, spaceWidth, totalLength, alignment);
                y += advance;

                lineStart = lineEnd;
                if (lineStart < text.Length && text[lineStart] == '\n')
                    lineStart++;
                lineCount++;
            }
        }

        static void DrawTextLine(SpriteBatch batch, SpriteFont font, string text, Rectangle area, Color color, BlockAlignment format, float y, float spaceCount, float spaceWidth, float totalLength, Alignment alignment)
        {
            float x;

            switch (alignment)
            {
                case Alignment.Near:
                    batch.DrawString(font, text, new Vector2(area.Left, y).Floor(), color);
                    break;

                case Alignment.Far:
                    x = area.Right - totalLength;
                    batch.DrawString(font, text, new Vector2(x, y).Floor(), color);
                    break;

                case Alignment.Center:
                    x = (area.Width - totalLength) / 2 + area.Left;
                    batch.DrawString(font, text, new Vector2(x, y).Floor(), color);
                    break;

                default:
                    throw new Exception("Unknown or invalid horizontal alignment value " + alignment);
            }
        }

        public static float FormattedLineHeight(this SpriteFont font, string text, float areaWidth)
        {
            return font.FormattedLineCount(text, areaWidth) * font.LineSpacing;
        }

        /// <summary>
        /// Return the number of lines that this would take to render with <see cref="DrawAlignedString"/>.
        /// </summary>
        /// <param name="font"></param>
        /// <param name="label"></param>
        /// <param name="areaWidth"></param>
        /// <returns></returns>
        public static int FormattedLineCount(this SpriteFont font, string text, float areaWidth)
        {
            int lineCount = 0;

            for (int lineStart = 0; lineStart < text.Length; )
            {
                float totalLength = 0;

                int lineEnd = lineStart;
                while (lineEnd < text.Length && text[lineEnd] != '\n')
                {
                    int wordStart = lineEnd;
                    SkipWhite(text, ref lineEnd);
                    SkipNonWhite(text, ref lineEnd);

                    float add = font.MeasureString(text.Substring(wordStart, lineEnd - wordStart)).X;

                    if (totalLength > 0 && totalLength + add >= areaWidth)
                    {
                        lineEnd = wordStart;
                        SkipWhite(text, ref lineEnd);
                        break;
                    }
                    totalLength += add;
                }

                lineStart = lineEnd;
                if (lineStart < text.Length && text[lineStart] == '\n')
                    lineStart++;
                lineCount++;
            }

            return lineCount;
        }

        static void SkipWhite(string text, ref int offset)
        {
            while (offset < text.Length && text[offset] != '\n' && char.IsWhiteSpace(text[offset]))
                offset++;
        }

        static void SkipNonWhite(string text, ref int offset)
        {
            while (offset < text.Length && !char.IsWhiteSpace(text[offset]))
                offset++;
        }

        #endregion

        #region Vectors

        public static Vector2 Floor(this Vector2 value) { return new Vector2((float)Math.Floor(value.X), (float)Math.Floor(value.Y)); }
        public static Vector3 Floor(this Vector3 value) { return new Vector3((float)Math.Floor(value.X), (float)Math.Floor(value.Y), (float)Math.Floor(value.Z)); }
        public static Vector4 Floor(this Vector4 value) { return new Vector4((float)Math.Floor(value.X), (float)Math.Floor(value.Y), (float)Math.Floor(value.Z), (float)Math.Floor(value.W)); }



        public static Vector2 Normalized(this Vector2 value) { return Vector2.Normalize(value); }
        public static Vector3 Normalized(this Vector3 value) { return Vector3.Normalize(value); }
        public static Vector4 Normalized(this Vector4 value) { return Vector4.Normalize(value); }

        #endregion Vectors

        #region Vector2

        /// <summary>
        /// Rotate the <see cref="Vector2"/> by a certain amount around the Z axis.
        /// </summary>
        /// <param name="value">The vector to rotate.</param>
        /// <param name="amount">The amount to rotate by.</param>
        /// <returns>The rotated vector.</returns>
        public static Vector2 Rotate(this Vector2 value, Angle amount)
        {
            float c = (float)Math.Cos(amount.InRadians), s = (float)Math.Sin(amount.InRadians);
            return new Vector2(value.X * c - value.Y * s, value.X * s + value.Y * c);
        }

        #endregion Vector2

        #region Vector3

        /// <summary>Swizzle the <see cref="Vector3"/> so that the Z and Y components are reversed. This is useful for converting between coordinate systems where Y is up and Z is up.</summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Vector3 XZY(this Vector3 value) { return new Vector3(value.X, value.Z, value.Y); }

        #endregion Vector3
    }

    public enum Alignment
    {
        /// <summary>
        /// The near edge is aligned to; the left edge for left-to-right, the right edge for right-to-left, or the top edge for vertical.
        /// </summary>
        Near,

        Top = Near,
        Left = Near,

        /// <summary>
        /// Centered between the edges.
        /// </summary>
        Center,

        /// <summary>
        /// The far edge is aligned to; the right edge for left-to-right, the left edge for right-to-left, or the bottom edge for vertical.
        /// </summary>
        Far,

        Right = Far,
        Bottom = Far,

        /// <summary>
        /// Expands spacing in the string so that it fully encompasses its space.
        /// If this is a vertical alignment and there is only one line, or a horizontal alignment and there are no spaces, then this
        /// is the same as <see cref="Center"/>.
        /// </summary>
        Full,
    }

    public struct BlockAlignment
    {
        public BlockAlignment(Alignment horizontalAlignment, Alignment horizontalPartialAlignment, Alignment verticalAlignment)
        {
            this.HorizontalAlignment = horizontalAlignment;
            this.HorizontalPartialAlignment = horizontalPartialAlignment;
            this.VerticalAlignment = verticalAlignment;
        }

        public BlockAlignment(Alignment horizontalAlignment, Alignment verticalAlignment) : this(horizontalAlignment, horizontalAlignment == Alignment.Full ? Alignment.Left : horizontalAlignment, verticalAlignment) { }

        public BlockAlignment(Alignment horizontalAlignment) : this(horizontalAlignment, Alignment.Center) { }

        /// <summary>
        /// Horizontal alignment value.
        /// </summary>
        public Alignment HorizontalAlignment;

        /// <summary>
        /// Get the horizontal alignment for partial lines, if <see cref="HorizontalAlignment"/> is <see cref="Alignment.Full"/>.
        /// If it is not, then this has no effect.
        /// </summary>
        public Alignment HorizontalPartialAlignment;

        /// <summary>Vertical alignment value.</summary>
        public Alignment VerticalAlignment;

        /// <summary>Get a string format that centers the label horizontally and vertically.</summary>
        public static readonly BlockAlignment FullCentered = new BlockAlignment(Alignment.Center, Alignment.Center);

        /// <summary>Get a string format that is centered horizontally. It is vertically top-aligned.</summary>
        public static readonly BlockAlignment HorizontallyCentered = new BlockAlignment(Alignment.Center);

        /// <summary>Get a string format that is full-justified. Partial lines are left-aligned and it is vertically top-aligned.</summary>
        public static readonly BlockAlignment Justified = new BlockAlignment(Alignment.Full, Alignment.Near, Alignment.Near);

        /// <summary>Get a string format that is horizontally and vertically near-aligned.</summary>
        public static readonly BlockAlignment GenericDefault = new BlockAlignment();

        /// <summary>Get a string format that is horizontally right-aligned.</summary>
        public static readonly BlockAlignment RightAligned = new BlockAlignment(Alignment.Far, Alignment.Far, Alignment.Near);
    }
}
