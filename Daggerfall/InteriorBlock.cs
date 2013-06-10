using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daggerfall {
	public class InteriorBlock : Block {
		#region Constructors

		internal InteriorBlock(State state, BinaryReader reader) : base(state, reader) {
			//throw new NotImplementedException();
		}

		#endregion Constructors

		#region Internals and fields

		#endregion Internals and fields

		#region Properties

		#region Dependency properties

		#endregion Dependency properties

		#endregion Properties

		#region Methods

        public override void Draw(BasicEffect effect, ref Matrix world, bool exterior, bool interior)
        {
            throw new NotImplementedException();
        }

		#endregion Methods
	}
}
