using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daggerfall
{
    /// <summary>
    /// A <see cref="Model"/> that is displayed with an <see cref="ExteriorChunk"/>.
    /// </summary>
    public class ExteriorChunkModel : StateObject
    {
        #region Fields

        Matrix transform;

        #endregion Fields

        #region Properties

        public readonly ExteriorChunkArea Area;
        public readonly int Index;
        public readonly int ModelId;

        public Model Model { get { return State.Models.Records[ModelId]; } }

        public Vector3 Position { get; protected set; }
        public readonly Unknowns Unknowns = new Unknowns();
        public readonly Vector3 UnknownPoint;
        public readonly Angle YRotation;
        public Matrix Transform { get { return transform; } }

        #endregion Properties

        #region Constructors

        internal ExteriorChunkModel(ExteriorChunkArea area, int index, BinaryReader reader) : base(area.State)
        {
            this.Area = area;
            this.Index = index;
            ModelId = reader.ReadUInt16() * 100 + reader.ReadByte();
            Unknowns.Add(reader.ReadByte());
            Unknowns.Add(reader.ReadInt32()); // Nonzero in 1297 of 236250 records; sames to repeast within the same file. Could be two or four separate fields.
            Unknowns.Add(reader.ReadInt32());
            Unknowns.Add(reader.ReadInt32()); // Non-zero only in 272 of 236250 records. Seems to repeat within the same file. Could be two or four seperate fields.
            reader.ReadZeroes(8);
            UnknownPoint = reader.ReadVector3i();
            var position = Position = reader.ReadVector3i();
            reader.ReadZeroes(4);
            YRotation = Angle.Daggerfall(reader.ReadInt16());
            Unknowns.Add(reader.ReadUInt16());
            Unknowns.Add(reader.ReadInt32());
            Unknowns.Add(reader.ReadInt32()); // Only non-zero in CUSTAA45.RMB where it is 0x200; this is referenced only by Wayrest.
            reader.ReadZeroes(2);

            transform = Matrix.CreateRotationY(YRotation.InRadians) * Matrix.CreateTranslation(position);
        }

        #endregion Constructors

        #region Methods

        public void Draw(BasicEffect effect, ref Matrix world)
        {
            Matrix matrix;

            Matrix.Multiply(ref transform, ref world, out matrix);
            var model = Model;
            model.Draw(effect, ref world);
        }
        
        #endregion Methods
    }
}
