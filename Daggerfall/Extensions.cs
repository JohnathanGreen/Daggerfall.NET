using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daggerfall {
	public static class Extensions {
		#region BinaryReader

		static byte[] asciiData = new byte[256];

		public static string ReadNulTerminatedAsciiString(this BinaryReader reader, int maximumLength) {
			lock(asciiData) {
				byte[] data = asciiData;

				if(maximumLength > asciiData.Length)
					data = new byte[maximumLength];

				reader.Read(data, 0, maximumLength);
				for(int count = 0; ; count++)
					if(count >= maximumLength || data[count] == 0) {
						string result = Encoding.ASCII.GetString(data, 0, count);
						asciiData = data;
						return result;
					}
			}
		}

		public static Vector3 ReadVector3i(this BinaryReader reader) {
			return new Vector3(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
		}

		public static void ReadZeroes(this BinaryReader reader, int count) {
			for(; count >= 4; count -= 4)
				if(reader.ReadInt32() != 0)
					throw new Exception();
			for(; count > 0; count--)
				if(reader.ReadByte() != 0)
					throw new Exception();
		}

		#endregion BinaryReader

#if false
		public static int GetSizeInBytes(this VertexAttribPointerType type) {
			switch(type) {
				case VertexAttribPointerType.Byte: return 1;
				case VertexAttribPointerType.Double: return 8;
				case VertexAttribPointerType.Fixed: return 4;
				case VertexAttribPointerType.Float: return 4;
				case VertexAttribPointerType.HalfFloat: return 2;
				case VertexAttribPointerType.Int: return 4;
				case VertexAttribPointerType.Int2101010Rev: return 4;
				case VertexAttribPointerType.Short: return 2;
				case VertexAttribPointerType.UnsignedByte: return 1;
				case VertexAttribPointerType.UnsignedInt: return 4;
				case VertexAttribPointerType.UnsignedInt2101010Rev: return 4;
				case VertexAttribPointerType.UnsignedShort: return 2;
				default: throw new ArgumentException("type");
			}
		}

		static readonly Dictionary<ActiveUniformType, ActiveUniformTypeInfo> ActiveUniformTypeInfo = new Dictionary<ActiveUniformType, ActiveUniformTypeInfo>() {
			{ ActiveUniformType.Bool, new ActiveUniformTypeInfo(typeof(bool), typeof(bool), 1) },
			{ ActiveUniformType.BoolVec2, new ActiveUniformTypeInfo(typeof(Vector2b), typeof(bool), 2) },
			{ ActiveUniformType.BoolVec3, new ActiveUniformTypeInfo(typeof(Vector3b), typeof(bool), 3) },
			{ ActiveUniformType.BoolVec4, new ActiveUniformTypeInfo(typeof(Vector3b), typeof(bool), 4) },
			{ ActiveUniformType.Double, new ActiveUniformTypeInfo(typeof(double), typeof(double), 1) },
			{ ActiveUniformType.DoubleMat2, new ActiveUniformTypeInfo(typeof(Matrix2d), typeof(double), 4) },
			{ ActiveUniformType.DoubleMat2x3, new ActiveUniformTypeInfo(typeof(Matrix2x3d), typeof(double), 6) },
			{ ActiveUniformType.DoubleMat2x4, new ActiveUniformTypeInfo(typeof(Matrix2x4d), typeof(double), 8) },
			{ ActiveUniformType.DoubleMat3, new ActiveUniformTypeInfo(typeof(Matrix3d), typeof(double), 9) },
			{ ActiveUniformType.DoubleMat3x2, new ActiveUniformTypeInfo(typeof(Matrix3x2d), typeof(double), 6) },
			{ ActiveUniformType.DoubleMat3x4, new ActiveUniformTypeInfo(typeof(Matrix3x4d), typeof(double), 12) },
			{ ActiveUniformType.DoubleMat4, new ActiveUniformTypeInfo(typeof(Matrix4d), typeof(double), 16) },
			{ ActiveUniformType.DoubleMat4x2, new ActiveUniformTypeInfo(typeof(Matrix4x2d), typeof(double), 8) },
			{ ActiveUniformType.DoubleMat4x3, new ActiveUniformTypeInfo(typeof(Matrix4x3d), typeof(double), 12) },
			{ ActiveUniformType.DoubleVec2, new ActiveUniformTypeInfo(typeof(Vector2d), typeof(double), 2) },
			{ ActiveUniformType.DoubleVec3, new ActiveUniformTypeInfo(typeof(Vector3d), typeof(double), 3) },
			{ ActiveUniformType.DoubleVec4, new ActiveUniformTypeInfo(typeof(Vector4d), typeof(double), 4) },
			{ ActiveUniformType.Float, new ActiveUniformTypeInfo(typeof(float), typeof(float), 1) },
			{ ActiveUniformType.FloatMat2, new ActiveUniformTypeInfo(typeof(Matrix2), typeof(float), 4) },
			{ ActiveUniformType.FloatMat2x3, new ActiveUniformTypeInfo(typeof(Matrix2x3), typeof(float), 6) },
			{ ActiveUniformType.FloatMat2x4, new ActiveUniformTypeInfo(typeof(Matrix2x4), typeof(float), 8) },
			{ ActiveUniformType.FloatMat3, new ActiveUniformTypeInfo(typeof(Matrix3), typeof(float), 9) },
			{ ActiveUniformType.FloatMat3x2, new ActiveUniformTypeInfo(typeof(Matrix3x2), typeof(float), 6) },
			{ ActiveUniformType.FloatMat3x4, new ActiveUniformTypeInfo(typeof(Matrix3x4), typeof(float), 12) },
			{ ActiveUniformType.FloatMat4, new ActiveUniformTypeInfo(typeof(Matrix4), typeof(float), 16) },
			{ ActiveUniformType.FloatMat4x2, new ActiveUniformTypeInfo(typeof(Matrix4x2), typeof(float), 8) },
			{ ActiveUniformType.FloatMat4x3, new ActiveUniformTypeInfo(typeof(Matrix4x3), typeof(float), 12) },
			{ ActiveUniformType.FloatVec2, new ActiveUniformTypeInfo(typeof(Vector2), typeof(float), 2) },
			{ ActiveUniformType.FloatVec3, new ActiveUniformTypeInfo(typeof(Vector3), typeof(float), 3) },
			{ ActiveUniformType.FloatVec4, new ActiveUniformTypeInfo(typeof(Vector4), typeof(float), 4) },
			{ ActiveUniformType.Int, new ActiveUniformTypeInfo(typeof(int), typeof(int), 1) },
			{ ActiveUniformType.IntSampler1D, new ActiveUniformTypeInfo(typeof(Sampler), typeof(Sampler), 1) },
			{ ActiveUniformType.IntSampler1DArray, new ActiveUniformTypeInfo(typeof(Sampler), typeof(Sampler), 1) },
			{ ActiveUniformType.IntSampler2D, new ActiveUniformTypeInfo(typeof(Sampler), typeof(Sampler), 1) },
			{ ActiveUniformType.IntSampler2DArray, new ActiveUniformTypeInfo(typeof(Sampler), typeof(Sampler), 1) },
			{ ActiveUniformType.IntSampler2DMultisample, new ActiveUniformTypeInfo(typeof(Sampler), typeof(Sampler), 1) },
			{ ActiveUniformType.IntSampler2DMultisampleArray, new ActiveUniformTypeInfo(typeof(Sampler), typeof(Sampler), 1) },
			{ ActiveUniformType.IntSampler2DRect, new ActiveUniformTypeInfo(typeof(Sampler), typeof(Sampler), 1) },
			{ ActiveUniformType.IntSampler3D, new ActiveUniformTypeInfo(typeof(Sampler), typeof(Sampler), 1) },
			{ ActiveUniformType.IntSamplerBuffer, new ActiveUniformTypeInfo(typeof(Sampler), typeof(Sampler), 1) },
			{ ActiveUniformType.IntSamplerCube, new ActiveUniformTypeInfo(typeof(Sampler), typeof(Sampler), 1) },
			{ ActiveUniformType.IntSamplerCubeMapArray, new ActiveUniformTypeInfo(typeof(Sampler), typeof(Sampler), 1) },
			{ ActiveUniformType.IntVec2, new ActiveUniformTypeInfo(typeof(Vector2i), typeof(int), 2) },
			{ ActiveUniformType.IntVec3, new ActiveUniformTypeInfo(typeof(Vector3i), typeof(int), 3) },
			{ ActiveUniformType.IntVec4, new ActiveUniformTypeInfo(typeof(Vector4i), typeof(int), 4) },
			{ ActiveUniformType.Sampler1D, new ActiveUniformTypeInfo(typeof(Sampler), typeof(Sampler), 1) },
			{ ActiveUniformType.Sampler1DArray, new ActiveUniformTypeInfo(typeof(Sampler), typeof(Sampler), 1) },
			{ ActiveUniformType.Sampler1DArrayShadow, new ActiveUniformTypeInfo(typeof(Sampler), typeof(Sampler), 1) },
			{ ActiveUniformType.Sampler1DShadow, new ActiveUniformTypeInfo(typeof(Sampler), typeof(Sampler), 1) },
			{ ActiveUniformType.Sampler2D, new ActiveUniformTypeInfo(typeof(Sampler), typeof(Sampler), 1) },
			{ ActiveUniformType.Sampler2DArray, new ActiveUniformTypeInfo(typeof(Sampler), typeof(Sampler), 1) },
			{ ActiveUniformType.Sampler2DArrayShadow, new ActiveUniformTypeInfo(typeof(Sampler), typeof(Sampler), 1) },
			{ ActiveUniformType.Sampler2DMultisample, new ActiveUniformTypeInfo(typeof(Sampler), typeof(Sampler), 1) },
			{ ActiveUniformType.Sampler2DMultisampleArray, new ActiveUniformTypeInfo(typeof(Sampler), typeof(Sampler), 1) },
			{ ActiveUniformType.Sampler2DRect, new ActiveUniformTypeInfo(typeof(Sampler), typeof(Sampler), 1) },
			{ ActiveUniformType.Sampler2DRectShadow, new ActiveUniformTypeInfo(typeof(Sampler), typeof(Sampler), 1) },
			{ ActiveUniformType.Sampler2DShadow, new ActiveUniformTypeInfo(typeof(Sampler), typeof(Sampler), 1) },
			{ ActiveUniformType.Sampler3D, new ActiveUniformTypeInfo(typeof(Sampler), typeof(Sampler), 1) },
			{ ActiveUniformType.SamplerBuffer, new ActiveUniformTypeInfo(typeof(Sampler), typeof(Sampler), 1) },
			{ ActiveUniformType.SamplerCube, new ActiveUniformTypeInfo(typeof(Sampler), typeof(Sampler), 1) },
			{ ActiveUniformType.SamplerCubeMapArray, new ActiveUniformTypeInfo(typeof(Sampler), typeof(Sampler), 1) },
			{ ActiveUniformType.SamplerCubeMapArrayShadow, new ActiveUniformTypeInfo(typeof(Sampler), typeof(Sampler), 1) },
			{ ActiveUniformType.SamplerCubeShadow, new ActiveUniformTypeInfo(typeof(Sampler), typeof(Sampler), 1) },
			{ ActiveUniformType.UnsignedInt, new ActiveUniformTypeInfo(typeof(uint), typeof(uint), 1) },
			{ ActiveUniformType.UnsignedIntSampler1D, new ActiveUniformTypeInfo(typeof(Sampler), typeof(Sampler), 1) },
			{ ActiveUniformType.UnsignedIntSampler1DArray, new ActiveUniformTypeInfo(typeof(Sampler), typeof(Sampler), 1) },
			{ ActiveUniformType.UnsignedIntSampler2D, new ActiveUniformTypeInfo(typeof(Sampler), typeof(Sampler), 1) },
			{ ActiveUniformType.UnsignedIntSampler2DArray, new ActiveUniformTypeInfo(typeof(Sampler), typeof(Sampler), 1) },
			{ ActiveUniformType.UnsignedIntSampler2DMultisample, new ActiveUniformTypeInfo(typeof(Sampler), typeof(Sampler), 1) },
			{ ActiveUniformType.UnsignedIntSampler2DMultisampleArray, new ActiveUniformTypeInfo(typeof(Sampler), typeof(Sampler), 1) },
			{ ActiveUniformType.UnsignedIntSampler2DRect, new ActiveUniformTypeInfo(typeof(Sampler), typeof(Sampler), 1) },
			{ ActiveUniformType.UnsignedIntSampler3D, new ActiveUniformTypeInfo(typeof(Sampler), typeof(Sampler), 1) },
			{ ActiveUniformType.UnsignedIntSamplerBuffer, new ActiveUniformTypeInfo(typeof(Sampler), typeof(Sampler), 1) },
			{ ActiveUniformType.UnsignedIntSamplerCube, new ActiveUniformTypeInfo(typeof(Sampler), typeof(Sampler), 1) },
			{ ActiveUniformType.UnsignedIntSamplerCubeMapArray, new ActiveUniformTypeInfo(typeof(Sampler), typeof(Sampler), 1) },
			{ ActiveUniformType.UnsignedIntVec2, new ActiveUniformTypeInfo(typeof(Vector2ui), typeof(uint), 2) },
			{ ActiveUniformType.UnsignedIntVec3, new ActiveUniformTypeInfo(typeof(Vector3ui), typeof(uint), 3) },
			{ ActiveUniformType.UnsignedIntVec4, new ActiveUniformTypeInfo(typeof(Vector4ui), typeof(uint), 4) },
		};

		public static ActiveUniformTypeInfo GetTypeAndSizeInfo(this ActiveUniformType type) {
			return ActiveUniformTypeInfo[type];
		}
#endif
	}

#if false
	public struct ActiveUniformTypeInfo {
		public ActiveUniformTypeInfo(Type type, Type elementType, int elementCount) {
			Type = type;
			ElementType = elementType;
			ElementCount = elementCount;
		}

		public readonly Type Type;
		public readonly Type ElementType;
		public readonly int ElementCount;
	}
#endif
}
