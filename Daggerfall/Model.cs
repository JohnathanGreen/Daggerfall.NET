using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daggerfall {
	public class ModelArchive : BsaArchive<Model> {
		public ModelArchive(State state, string path) : base(state, path) { }

		protected override Model Load(System.IO.BinaryReader reader, BsaArchiveRecordInfo record) {
			return new Model(State, reader, record.Size);
		}
	}

	public struct UsedRange {
		public UsedRange(string name, int from, int to) { Name = name;  From = from; To = to; }

		public readonly string Name;
		public readonly int From;
		public readonly int To;

		public override string ToString() { return string.Format("{{{0} - {1} to {2}}}", Name, From, To); }
	}

	public struct UsedArea {
		public UsedArea(BinaryReader reader, int total) {
			Reader = reader;
			Start = reader.BaseStream.Position;
			Total = total;
			Ranges = new List<UsedRange>();
		}

		public readonly BinaryReader Reader;
		public readonly long Start;
		public readonly int Total;
		public readonly List<UsedRange> Ranges;

		public void AddRangeFromEnd(string name, int start) {
			Ranges.Add(new UsedRange(name, start, (int)(Reader.BaseStream.Position - Start)));
			Ranges.Sort((a, b) => a.From.CompareTo(b.From));
		}
	}

	public class Model : StateObject {
		#region Constructors

		public Model(State state, BinaryReader reader, int total) : base(state) {
			Used = new UsedArea(reader, total);
			var offset = reader.BaseStream.Position;
			var version = reader.ReadNulTerminatedAsciiString(4);
			int pointCount = reader.ReadInt32();
			int planeCount = reader.ReadInt32();
			Radius = reader.ReadInt32();
			reader.ReadZeroes(8);
			var planeDataOffset = reader.ReadInt32();
			var objectOffset = reader.ReadInt32();
			int objectCount = reader.ReadInt32();
			U2 = reader.ReadInt32();
			reader.ReadZeroes(8);
			var pointOffset = reader.ReadInt32();
			var normalOffset = reader.ReadInt32(); // One normal per plane
			U3 = reader.ReadInt32();
			var planeListOffset = reader.ReadInt32();
			Used.AddRangeFromEnd("Header", 0);

			#region Point list
			Points = new Vector3[pointCount];
			reader.BaseStream.Position = offset + pointOffset;
			for(int index = 0; index < pointCount; index++)
				Points[index] = reader.ReadVector3i() * new Vector3(1, -1, 1);
			Used.AddRangeFromEnd("Points", pointOffset);
			#endregion Point list

			#region Plane list
			Planes = new ModelPlane[planeCount];
			reader.BaseStream.Position = offset + planeListOffset;
			for(int index = 0; index < planeCount; index++)
				Planes[index] = new ModelPlane(reader, version == "v2.5");
			Used.AddRangeFromEnd("Planes", planeListOffset);

			reader.BaseStream.Position = offset + planeDataOffset;
			for(int index = 0; index < planeCount; index++)
				Planes[index].LoadData(reader);
			Used.AddRangeFromEnd("Plane data", planeDataOffset);

			reader.BaseStream.Position = offset + normalOffset;
			for(int index = 0; index < planeCount; index++)
				Planes[index].LoadNormal(reader);
			Used.AddRangeFromEnd("Plane normals", normalOffset);
			#endregion Plane list

			#region Objects
			Objects = new ModelObject[objectCount];
			reader.BaseStream.Position = offset + objectOffset;
			for(int index = 0; index < objectCount; index++)
				Objects[index] = new ModelObject(reader);
			Used.AddRangeFromEnd("Objects", objectOffset);
			#endregion Objects
		}

		#endregion Constructors

		#region Internals and fields

		VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;

		#endregion Internals and fields

		#region Properties

		public UsedArea Used = new UsedArea();
		public readonly int Radius, U2, U3;
		public readonly Vector3[] Points;
		public readonly ModelPlane[] Planes;
		public readonly ModelObject[] Objects;

		/// <summary>Get the total number of points in all of the <see cref="Planes"/>.</summary>
		public int PlanePointCount { get { int value = 0; foreach(ModelPlane plane in Planes) value += plane.Points.Length; return value; } }

        /// <summary>Get the total number of triangles in all of the <see cref="Planes"/>.</summary>
        public int PlaneTriangleCount { get { int value = 0; foreach (ModelPlane plane in Planes) value += Math.Max(0, plane.Points.Length - 2); return value; } }

        /// <summary>
        /// Get an <see cref="Microsoft.Xna.Framework.Graphics.IndexBuffer"/> that has indices for all the triangles in the model, appropriate for <see cref="PrimitiveType.TriangleList"/>.
        /// </summary>
        public IndexBuffer IndexBuffer
        {
            get
            {
                if (indexBuffer == null)
                {
                    ushort[] indices = new ushort[PlaneTriangleCount * 3];
                    int offset = 0, vertexOffset = 0;

                    foreach (ModelPlane plane in Planes)
                    {
                        for (int triangleIndex = 1; triangleIndex < plane.Points.Length - 1; triangleIndex++)
                        {
                            indices[offset++] = (ushort)vertexOffset;
                            indices[offset++] = (ushort)(vertexOffset + triangleIndex);
                            indices[offset++] = (ushort)(vertexOffset + triangleIndex + 1);
                        }

                        vertexOffset += plane.Points.Length;
                    }

                    if (vertexOffset >= ushort.MaxValue)
                        throw new InvalidOperationException("Overflow on indices.");
                    indexBuffer = new IndexBuffer(Graphics, IndexElementSize.SixteenBits, indices.Length, BufferUsage.WriteOnly);
                    indexBuffer.SetData(indices);
                }

                return indexBuffer;
            }
        }

        /// <summary>Get a <see cref="Microsoft.Xna.Framework.Graphics.VertexBuffer"/> that contains all the points in the <see cref="Planes"/> in <see cref="VertexPositionNormalTexture"/> vertices.</summary>
		public VertexBuffer VertexBuffer {
			get {
				if(vertexBuffer == null) {
					VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[PlanePointCount];
					int offset = 0;

					foreach(ModelPlane plane in Planes) {
						foreach(ModelPlanePoint point in plane.Points) {
							vertices[offset++] = new VertexPositionNormalTexture(Points[point.Index], plane.Normal, new Vector2(point.U, point.V));
						}
					}

					vertexBuffer = new VertexBuffer(Graphics, ((IVertexType)vertices[0]).VertexDeclaration, vertices.Length, BufferUsage.WriteOnly);
					vertexBuffer.SetData(vertices);
				}
				return vertexBuffer;
			}
		}

		#endregion Properties

		#region Methods

		public override string ToString() {
			return string.Format("{0}(Radius {1}, {2}, {3}, {4} point[s], {5} plane[s], {6} object[s])", GetType().Name, Radius, U2, U3, Points.Length, Planes.Length, Objects.Length);
		}

		#endregion Methods
	}

	public class ModelPlane {
		internal ModelPlane(BinaryReader reader, bool earlyVersion) {
			int pointCount = reader.ReadByte();
			U1 = reader.ReadByte();
			TextureInfo = reader.ReadUInt16();
			U2 = reader.ReadUInt16();
			U3 = reader.ReadUInt16();

			Points = new ModelPlanePoint[pointCount];
			ModelPlanePoint point;
			for(int pointIndex = 0; pointIndex < pointCount; pointIndex++) {
				int offset = reader.ReadInt32();
				if(earlyVersion) offset *= 3;
				if(offset % 12 != 0) throw new Exception();
				point.Index = offset / 12;
				point.U = reader.ReadUInt16();
				point.V = reader.ReadUInt16();
				Points[pointIndex] = point;
			}
		}

		internal void LoadData(BinaryReader reader) {
			reader.Read(Data, 0, 24);
		}

		internal void LoadNormal(BinaryReader reader) {
			normal = ((Vector3)reader.ReadVector3i() / 256.0f).Normalized() * new Vector3(1, -1, 1);
		}

		Vector3 normal;
		ushort TextureInfo;

		public readonly byte U1;
		public readonly ushort U2, U3;
		public readonly ModelPlanePoint[] Points;
		public readonly byte[] Data = new byte[24];
		public Vector3 Normal { get { return normal; } }

		public int FileIndex { get { return TextureInfo >> 7; } }
		public int ImageIndex { get { return TextureInfo & 0x7F; } }

		public override string ToString() {
			return string.Format("{{{0}, {1}, {2}, {3} point[s], {4}, texture {5}.{6}}}", U1, U2, U3, Points.Length, Normal, FileIndex, ImageIndex);
		}
	}

	public struct ModelPlanePoint {
		public int Index;
		public ushort U, V;

		public override string ToString() { return "{" + ToShortString() + "}"; }
		public string ToShortString() { return string.Format("{0}, {1}, {2})", Index, U, V); }
	}

	public class ModelObject {
		public ModelObject(BinaryReader reader) {
			Position = reader.ReadVector3i();
			U4 = reader.ReadInt32();
			int recordCount = reader.ReadUInt16();
			Records = new ModelObjectRecord[recordCount];
			for(int index = 0; index < recordCount; index++)
				Records[index] = new ModelObjectRecord(reader);
		}

		public Vector3 Position;
		public readonly int U4;
		public readonly ModelObjectRecord[] Records;

		public override string ToString() { return "{" + ToShortString() + "}"; }
		public string ToShortString() { return string.Format("{0}, {1}, {2} record[s]", Position, U4, Records.Length); }
	}

	public class ModelObjectRecord {
		public ModelObjectRecord(BinaryReader reader) {
			U1 = reader.ReadInt32();
			U2 = reader.ReadInt16();
		}

		public int U1;
		public short U2;

		public override string ToString() { return "{" + ToShortString() + "}"; }
		public string ToShortString() { return string.Format("{0}, {1}", U1, U2); }
	}
}
