using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daggerfall {
	public abstract class Block : StateObject {
		#region Constructors

		public Block(State state, BinaryReader reader) : base(state) {
		}

		#endregion Constructors

		#region Internals and fields

		#endregion Internals and fields

		#region Properties

		#endregion Properties

		#region Methods

        public abstract void Draw(BasicEffect effect, ref Matrix world, bool exterior, bool interior);

		#endregion Methods
	}

	public class BlockArchive : BsaStringIdArchive<Block> {
		public BlockArchive(State state, string path) : base(state, path) { }

		protected override Block Load(Record record) {
			if (record.Id.EndsWith(".RMB")) return new ExteriorBlock(State, Reader, record);
			if (record.Id.EndsWith(".RDB")) return new InteriorBlock(State, Reader);
			if (record.Id == "FOO" || record.Id.EndsWith(".RDI")) return null;
			throw new NotImplementedException();
		}
	}
}
