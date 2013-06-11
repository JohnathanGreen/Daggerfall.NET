using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Daggerfall {
	public class State : IDisposable {
		#region Constructors

		public State(GraphicsDevice graphics, string path) {
			//if(graphics == null) throw new ArgumentNullException("graphics");
			if(path == null) throw new ArgumentNullException("path");

			this.path = path;
			this.graphics = graphics;
            this.textures = new TextureArchiveList(this);

            mapPalette = new Palette(this, "MAP.PAL");
            oldMapPalette = new Palette(this, "OLDMAP.PAL");
            oldPalette = new Palette(this, "OLDPAL.PAL");
            rawPalette = new Palette(this, "PAL.RAW");
            artPalette = new Palette(this, "ART_PAL.COL");
            dankBMapPalette = new Palette(this, "DANKBMAP.COL");
            fMapPalette = new Palette(this, "FMAP_PAL.COL");
            nightSkyPalette = new Palette(this, "NIGHTSKY.COL");
            palette = new Palette(this, "PAL.PAL");
		}

		#endregion Constructors

		#region Internals and fields

		BlockArchive blocks;
		readonly GraphicsDevice graphics;
		ModelArchive models;
		readonly string path;
		RegionArchive regions;
        readonly TextureArchiveList textures;
        readonly Palette mapPalette, oldMapPalette, oldPalette, rawPalette, artPalette, dankBMapPalette, fMapPalette, nightSkyPalette, palette;

		#endregion Internals and fields

		#region Properties

        #region Palettes
        public Palette ArtPalette { get { return artPalette; } }
        public Palette DankBMapPalette { get { return dankBMapPalette; } }
        public Palette FMapPalette { get { return fMapPalette; } }
        public Palette MapPalette { get { return mapPalette; } }
        public Palette NightSkyPalette { get { return nightSkyPalette; } }
        public Palette OldMapPalette { get { return oldMapPalette; } }
        public Palette OldPalette { get { return oldPalette; } }
        public Palette Palette { get { return palette; } }
        public Palette RawPalette { get { return rawPalette; } }
        #endregion Palettes

        public BlockArchive Blocks { get { return blocks ?? (blocks = new BlockArchive(this, path + "BLOCKS.BSA")); } }

		public GraphicsDevice Graphics { get { return graphics; } }

		public ModelArchive Models { get { return models ?? (models = new ModelArchive(this, path + "ARCH3D.BSA")); } }

		public string Path { get { return path; } }

		public RegionArchive Regions { get { return regions ?? (regions = new RegionArchive(this, path + "MAPS.BSA")); } }

        public TextureArchiveList Textures { get { return textures; } }

		#endregion Properties

		#region Methods

        public virtual void Dispose() { }

        public string GetDataFileName(string fileName) { return Path + System.IO.Path.DirectorySeparatorChar + fileName; }

        public FileStream GetDataStream(string fileName) { return File.Open(GetDataFileName(fileName), FileMode.Open, FileAccess.Read, FileShare.Read); }

        public BinaryReader GetDataReader(string fileName) { return new BinaryReader(GetDataStream(fileName)); }

		#endregion Methods
	}

	public abstract class StateObject : IDisposable {
		public StateObject(State state) {
			if(state == null) throw new ArgumentNullException("state");
			this.state = state;
		}

		readonly State state;

		public State State { get { return state; } }
		public GraphicsDevice Graphics { get { return state.Graphics; } }

        public virtual void Dispose() { }
	}
}
