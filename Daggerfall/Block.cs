using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daggerfall {
	public abstract class Block : StateObject {
		#region Constructors

		public Block(State state) : base(state) {
		}

		#endregion Constructors

		#region Internals and fields

		#endregion Internals and fields

		#region Properties

		#endregion Properties

		#region Methods

		#endregion Methods
	}

	public class BlockArchive : BsaStringIdArchive<Block> {
		public BlockArchive(State state, string path) : base(state, path) { }

		protected override Block Load(Record record) {
			string extension = Path.GetExtension(record.Id);
			throw new NotImplementedException();
		}
	}
}
