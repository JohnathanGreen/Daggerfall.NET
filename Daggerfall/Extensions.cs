using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daggerfall
{
    public static class Extensions
    {
        #region BinaryReader

        static byte[] asciiData = new byte[256];

        public static int[] ReadInt32Array(this BinaryReader reader, int count)
        {
            int[] array = new int[count];
            unsafe
            {
                fixed (int* pointer = array)
                    for (int* current = pointer, end = pointer + count; current < end; current++)
                        *current = reader.ReadInt32();
            }
            return array;
        }

        public static uint[] ReadUInt32Array(this BinaryReader reader, int count)
        {
            uint[] array = new uint[count];
            unsafe {
                fixed(uint* pointer = array)
                    for(uint* current = pointer, end = pointer + count; current < end; current++)
                        *current = reader.ReadUInt32();
            }
            return array;
        }

        public static string ReadNulTerminatedAsciiString(this BinaryReader reader, int maximumLength)
        {
            lock (asciiData)
            {
                byte[] data = asciiData;

                if (maximumLength > asciiData.Length)
                    data = new byte[maximumLength];

                reader.Read(data, 0, maximumLength);
                for (int count = 0; ; count++)
                    if (count >= maximumLength || data[count] == 0)
                    {
                        string result = Encoding.ASCII.GetString(data, 0, count);
                        asciiData = data;
                        return result;
                    }
            }
        }

        public static Vector3 ReadVector3i(this BinaryReader reader)
        {
            return new Vector3(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
        }

        public static void ReadZeroes(this BinaryReader reader, int count)
        {
            for (; count >= 4; count -= 4)
                if (reader.ReadInt32() != 0)
                    throw new Exception();
            for (; count > 0; count--)
                if (reader.ReadByte() != 0)
                    throw new Exception();
        }

        #endregion BinaryReader

        public static int Hash(int hash, int value)
        {
            long newHash = (uint)hash * 149L + value * 11;
            return (int)(newHash ^ (newHash >> 32));
        }

        public static int Hash(int hash, string value)
        {
            foreach (char ch in value)
                hash = Hash(hash, ch);
            return hash;
        }

        public static string ToHexValue(this int value)
        {
            if (value < 10 && value > -10)
                return value.ToString();
            return string.Format("{0}/{0:X2}h", value);
        }

        public static string ToHexString(this byte[] data)
        {
            string text = "";

            for (int index = 0; index < data.Length; index++)
            {
                var value = data[index];

                if (index > 0)
                    text += " ";
                text += ToHexValue(value);

                int count = 1;
                for (; index < data.Length - 1; index++, count++)
                    if (data[index + 1] != value)
                        break;
                if (count > 1)
                    text += "×" + count;
            }

            return "[" + text + "]";
        }

        public static int GetDaggerfallHash(int hash, char value)
        {
            return (hash << 1) + value;
        }

        public static int GetDaggerfallHash(this string value)
        {
            if (value == null) return 0;
            int hash = 0;
            foreach (char ch in value) hash = GetDaggerfallHash(hash, ch);
            return hash;
        }

        public static bool CheckRepeat(this GameTime gameTime, ref TimeSpan lastTime, TimeSpan repeatTime)
        {
            if (gameTime.TotalGameTime - lastTime > repeatTime)
            {
                lastTime = gameTime.TotalGameTime;
                return true;
            }
            return false;
        }

        public static bool CheckRepeat(this KeyboardState keyboard, Keys key, GameTime gameTime, ref TimeSpan lastTime, TimeSpan repeatTime)
        {
            return keyboard.IsKeyDown(key) && gameTime.CheckRepeat(ref lastTime, repeatTime);
        }

        public static int CheckNextPreviousKeys(this KeyboardState keyboard, Keys next, Keys previous, TimeSpan repeatTime, GameTime gameTime, ref TimeSpan lastNext, ref TimeSpan lastPrevious) {
            int value = 0;
            if(keyboard.CheckRepeat(next, gameTime, ref lastNext, repeatTime))
                value++;
            if(keyboard.CheckRepeat(previous, gameTime, ref lastPrevious, repeatTime))
                value--;
            return value;
        }

        /// <summary>
        /// Constrain the value from 0 to count-1, wrapping values as necessary. For example, a value of -2 will return count-2.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static int Wrap(this int value, int count) {
            value %= count;
            if (value < 0) value += count;
            return value;
        }
    }
}
