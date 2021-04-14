using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.Isam.Esent.Interop
{
	internal static class Util
	{
		public static bool ArrayEqual(IList<byte> a, IList<byte> b, int offset, int count)
		{
			if (a == null || b == null)
			{
				return object.ReferenceEquals(a, b);
			}
			for (int i = 0; i < count; i++)
			{
				if (a[offset + i] != b[offset + i])
				{
					return false;
				}
			}
			return true;
		}

		public static string DumpBytes(byte[] data, int offset, int count)
		{
			if (data == null)
			{
				return "<null>";
			}
			if (count == 0)
			{
				return string.Empty;
			}
			if (offset < 0 || count < 0 || offset >= data.Length || offset + count > data.Length)
			{
				return "<invalid>";
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(BitConverter.ToString(data, offset, Math.Min(count, 8)));
			if (count > 8)
			{
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "... ({0} bytes)", new object[]
				{
					count
				});
			}
			return stringBuilder.ToString();
		}

		public static bool ObjectContentEquals<T>(T left, T right) where T : class, IContentEquatable<T>
		{
			if (left == null || right == null)
			{
				return object.ReferenceEquals(left, right);
			}
			return left.ContentEquals(right);
		}

		public static bool ArrayObjectContentEquals<T>(T[] left, T[] right, int length) where T : class, IContentEquatable<T>
		{
			if (left == null || right == null)
			{
				return object.ReferenceEquals(left, right);
			}
			for (int i = 0; i < length; i++)
			{
				if (!Util.ObjectContentEquals<T>(left[i], right[i]))
				{
					return false;
				}
			}
			return true;
		}

		public static bool ArrayStructEquals<T>(T[] left, T[] right, int length) where T : struct
		{
			if (left == null || right == null)
			{
				return object.ReferenceEquals(left, right);
			}
			for (int i = 0; i < length; i++)
			{
				if (!left[i].Equals(right[i]))
				{
					return false;
				}
			}
			return true;
		}

		public static T[] DeepCloneArray<T>(T[] value) where T : class, IDeepCloneable<T>
		{
			T[] array = null;
			if (value != null)
			{
				array = new T[value.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = ((value[i] == null) ? default(T) : value[i].DeepClone());
				}
			}
			return array;
		}

		public static int CalculateHashCode(IEnumerable<int> hashes)
		{
			int num = 0;
			foreach (int num2 in hashes)
			{
				num ^= num2;
				num *= 33;
			}
			return num;
		}

		public static string AddTrailingDirectorySeparator(string dir)
		{
			if (!string.IsNullOrEmpty(dir))
			{
				char[] trimChars = new char[]
				{
					LibraryHelpers.DirectorySeparatorChar,
					LibraryHelpers.AltDirectorySeparatorChar
				};
				return dir.TrimEnd(trimChars) + LibraryHelpers.DirectorySeparatorChar;
			}
			return dir;
		}

		public static byte[] ConvertToNullTerminatedAsciiByteArray(string value)
		{
			if (value == null)
			{
				return null;
			}
			byte[] array = new byte[value.Length + 1];
			LibraryHelpers.EncodingASCII.GetBytes(value, 0, value.Length, array, 0);
			array[array.Length - 1] = 0;
			return array;
		}

		public static byte[] ConvertToNullTerminatedUnicodeByteArray(string value)
		{
			if (value == null)
			{
				return null;
			}
			int byteCount = Encoding.Unicode.GetByteCount(value);
			byte[] array = new byte[byteCount + 2];
			Encoding.Unicode.GetBytes(value, 0, value.Length, array, 0);
			array[array.Length - 2] = 0;
			array[array.Length - 1] = 0;
			return array;
		}
	}
}
