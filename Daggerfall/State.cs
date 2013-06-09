using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daggerfall {
	public class State {
		#region Constructors

		public State(GraphicsDevice graphics, string path) {
			//if(graphics == null) throw new ArgumentNullException("graphics");
			if(path == null) throw new ArgumentNullException("path");

			this.path = path;
			this.graphics = graphics;
		}

		#endregion Constructors

		#region Internals and fields

		readonly GraphicsDevice graphics;
		ModelArchive models;
		readonly string path;
		RegionArchive regions;

		#endregion Internals and fields

		#region Properties

		public GraphicsDevice Graphics { get { return graphics; } }

		public ModelArchive Models { get { return models ?? (models = new ModelArchive(this, path + "ARCH3D.BSA")); } }

		public string Path { get { return path; } }

		public RegionArchive Regions { get { return regions ?? (regions = new RegionArchive(this, path + "MAPS.BSA")); } }

		#endregion Properties

		#region Methods

		#endregion Methods
	}

	public abstract class StateObject {
		public StateObject(State state) {
			if(state == null) throw new ArgumentNullException("state");
			this.state = state;
		}

		readonly State state;

		public State State { get { return state; } }
		public GraphicsDevice Graphics { get { return state.Graphics; } }
	}
}
