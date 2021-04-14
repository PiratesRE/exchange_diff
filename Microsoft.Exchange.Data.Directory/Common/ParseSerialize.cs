using System;
using System.Text;
using Microsoft.Exchange.Conversion;

namespace Microsoft.Exchange.Common
{
	public static class ParseSerialize
	{
		public static bool CheckOffsetLength(int maxOffset, int offset, int length)
		{
			return offset >= 0 && length >= 0 && offset <= maxOffset && length <= maxOffset - offset;
		}

		public static bool CheckOffsetLength(byte[] buffer, int offset, int length)
		{
			return ParseSerialize.CheckOffsetLength(buffer.Length, offset, length);
		}

		public static short ParseInt16(byte[] buffer, int offset)
		{
			return BitConverter.ToInt16(buffer, offset);
		}

		public static int ParseInt32(byte[] buffer, int offset)
		{
			return BitConverter.ToInt32(buffer, offset);
		}

		public static long ParseInt64(byte[] buffer, int offset)
		{
			return BitConverter.ToInt64(buffer, offset);
		}

		public static float ParseSingle(byte[] buffer, int offset)
		{
			return BitConverter.ToSingle(buffer, offset);
		}

		public static double ParseDouble(byte[] buffer, int offset)
		{
			return BitConverter.ToDouble(buffer, offset);
		}

		public static Guid ParseGuid(byte[] buffer, int offset)
		{
			return ExBitConverter.ReadGuid(buffer, offset);
		}

		public static byte[] ParseBinary(byte[] buffer, int offset, int length)
		{
			if (length == 0)
			{
				return ParseSerialize.emptyByteArray;
			}
			byte[] array = new byte[length];
			Buffer.BlockCopy(buffer, offset, array, 0, length);
			return array;
		}

		public static string ParseUcs16String(byte[] buffer, int offset, int length)
		{
			if (length != 0)
			{
				return Encoding.Unicode.GetString(buffer, offset, length);
			}
			return string.Empty;
		}

		public static string ParseUtf8String(byte[] buffer, int offset, int length)
		{
			if (length != 0)
			{
				return Encoding.UTF8.GetString(buffer, offset, length);
			}
			return string.Empty;
		}

		public static int GetLengthOfUtf8String(byte[] buffer, int offset, int length)
		{
			if (length != 0)
			{
				return Encoding.UTF8.GetCharCount(buffer, offset, length);
			}
			return 0;
		}

		public static string ParseAsciiString(byte[] buffer, int offset, int length)
		{
			if (length != 0)
			{
				return CTSGlobals.AsciiEncoding.GetString(buffer, offset, length);
			}
			return string.Empty;
		}

		public static DateTime ParseFileTime(byte[] buffer, int offset)
		{
			DateTime result;
			ParseSerialize.TryParseFileTime(buffer, offset, out result);
			return result;
		}

		public static bool TryParseFileTime(byte[] buffer, int offset, out DateTime dateTime)
		{
			long fileTime = ParseSerialize.ParseInt64(buffer, offset);
			return ParseSerialize.TryConvertFileTime(fileTime, out dateTime);
		}

		public static bool TryConvertFileTime(long fileTime, out DateTime dateTime)
		{
			bool result;
			if (fileTime < ParseSerialize.MinFileTime || fileTime >= ParseSerialize.MaxFileTime)
			{
				dateTime = DateTime.MaxValue;
				result = (fileTime == long.MaxValue);
			}
			else if (fileTime == 0L)
			{
				dateTime = DateTime.MinValue;
				result = true;
			}
			else
			{
				dateTime = DateTime.FromFileTimeUtc(fileTime);
				result = true;
			}
			return result;
		}

		public static int SerializeInt16(short value, byte[] buffer, int offset)
		{
			buffer[offset] = (byte)value;
			buffer[offset + 1] = (byte)(value >> 8);
			return 2;
		}

		public static int SerializeInt32(int value, byte[] buffer, int offset)
		{
			ExBitConverter.Write(value, buffer, offset);
			return 4;
		}

		public static int SerializeInt64(long value, byte[] buffer, int offset)
		{
			ExBitConverter.Write(value, buffer, offset);
			return 8;
		}

		public static int SerializeSingle(float value, byte[] buffer, int offset)
		{
			ExBitConverter.Write(value, buffer, offset);
			return 4;
		}

		public static int SerializeDouble(double value, byte[] buffer, int offset)
		{
			ExBitConverter.Write(value, buffer, offset);
			return 8;
		}

		public static int SerializeGuid(Guid value, byte[] buffer, int offset)
		{
			ExBitConverter.Write(value, buffer, offset);
			return 16;
		}

		public static int SerializeFileTime(DateTime dateTime, byte[] buffer, int offset)
		{
			long value;
			if (dateTime < ParseSerialize.MinFileTimeDateTime)
			{
				value = 0L;
			}
			else if (dateTime == DateTime.MaxValue)
			{
				value = long.MaxValue;
			}
			else
			{
				value = dateTime.ToFileTimeUtc();
			}
			ParseSerialize.SerializeInt64(value, buffer, offset);
			return 8;
		}

		public static int SerializeAsciiString(string value, byte[] buffer, int offset)
		{
			CTSGlobals.AsciiEncoding.GetBytes(value, 0, value.Length, buffer, offset);
			return value.Length;
		}

		public static void SetWord(byte[] buff, ref int pos, ushort w)
		{
			ParseSerialize.SetWord(buff, ref pos, (short)w);
		}

		public static void SetWord(byte[] buff, ref int pos, short w)
		{
			ParseSerialize.CheckBounds(pos, buff, 2);
			if (buff != null)
			{
				ParseSerialize.SerializeInt16(w, buff, pos);
			}
			pos += 2;
		}

		public static void SetDword(byte[] buff, ref int pos, uint dw)
		{
			ParseSerialize.SetDword(buff, ref pos, (int)dw);
		}

		public static void SetDword(byte[] buff, ref int pos, int dw)
		{
			ParseSerialize.CheckBounds(pos, buff, 4);
			if (buff != null)
			{
				ParseSerialize.SerializeInt32(dw, buff, pos);
			}
			pos += 4;
		}

		public static void SetQword(byte[] buff, ref int pos, ulong qw)
		{
			ParseSerialize.SetQword(buff, ref pos, (long)qw);
		}

		public static void SetQword(byte[] buff, ref int pos, long qw)
		{
			ParseSerialize.CheckBounds(pos, buff, 8);
			if (buff != null)
			{
				ParseSerialize.SerializeInt64(qw, buff, pos);
			}
			pos += 8;
		}

		public static void SetSysTime(byte[] buff, ref int pos, DateTime value)
		{
			ParseSerialize.CheckBounds(pos, buff, 8);
			if (buff != null)
			{
				ParseSerialize.SerializeFileTime(value, buff, pos);
			}
			pos += 8;
		}

		public static void SetBoolean(byte[] buff, ref int pos, bool value)
		{
			ParseSerialize.SetByte(buff, ref pos, value ? 1 : 0);
		}

		public static void SetByte(byte[] buff, ref int pos, byte b)
		{
			ParseSerialize.CheckBounds(pos, buff, 1);
			if (buff != null)
			{
				buff[pos] = b;
			}
			pos++;
		}

		public static void SetUnicodeString(byte[] buff, ref int pos, string str)
		{
			ParseSerialize.CheckBounds(pos, buff, (str.Length + 1) * 2);
			if (buff != null)
			{
				Encoding.Unicode.GetBytes(str, 0, str.Length, buff, pos);
				buff[pos + str.Length * 2] = 0;
				buff[pos + str.Length * 2 + 1] = 0;
			}
			pos += (str.Length + 1) * 2;
		}

		public static void SetASCIIString(byte[] buff, ref int pos, string str)
		{
			ParseSerialize.CheckBounds(pos, buff, str.Length + 1);
			if (buff != null)
			{
				CTSGlobals.AsciiEncoding.GetBytes(str, 0, str.Length, buff, pos);
				buff[pos + str.Length] = 0;
			}
			pos += str.Length + 1;
		}

		public static void SetByteArray(byte[] buff, ref int pos, byte[] byteArray)
		{
			ParseSerialize.CheckBounds(pos, buff, 2 + byteArray.Length);
			if (buff != null)
			{
				ParseSerialize.SerializeInt16((short)byteArray.Length, buff, pos);
				Buffer.BlockCopy(byteArray, 0, buff, pos + 2, byteArray.Length);
			}
			pos += 2 + byteArray.Length;
		}

		public static void SetFloat(byte[] buff, ref int pos, float fl)
		{
			ParseSerialize.CheckBounds(pos, buff, 4);
			if (buff != null)
			{
				ParseSerialize.SerializeSingle(fl, buff, pos);
			}
			pos += 4;
		}

		public static void SetDouble(byte[] buff, ref int pos, double dbl)
		{
			ParseSerialize.CheckBounds(pos, buff, 8);
			if (buff != null)
			{
				ParseSerialize.SerializeDouble(dbl, buff, pos);
			}
			pos += 8;
		}

		public static void SetGuid(byte[] buff, ref int pos, Guid guid)
		{
			ParseSerialize.CheckBounds(pos, buff, 16);
			if (buff != null)
			{
				ParseSerialize.SerializeGuid(guid, buff, pos);
			}
			pos += 16;
		}

		public static void SetMVInt16(byte[] buff, ref int pos, short[] values)
		{
			ParseSerialize.CheckBounds(pos, buff, 4 + values.Length * 2);
			if (buff != null)
			{
				ParseSerialize.SerializeInt32(values.Length, buff, pos);
				for (int i = 0; i < values.Length; i++)
				{
					ParseSerialize.SerializeInt16(values[i], buff, pos + 4 + i * 2);
				}
			}
			pos += 4 + values.Length * 2;
		}

		public static void SetMVInt32(byte[] buff, ref int pos, int[] values)
		{
			ParseSerialize.CheckBounds(pos, buff, 4 + values.Length * 4);
			if (buff != null)
			{
				ParseSerialize.SerializeInt32(values.Length, buff, pos);
				for (int i = 0; i < values.Length; i++)
				{
					ParseSerialize.SerializeInt32(values[i], buff, pos + 4 + i * 4);
				}
			}
			pos += 4 + values.Length * 4;
		}

		public static void SetMVInt64(byte[] buff, ref int pos, long[] values)
		{
			ParseSerialize.CheckBounds(pos, buff, 4 + values.Length * 8);
			if (buff != null)
			{
				ParseSerialize.SerializeInt32(values.Length, buff, pos);
				for (int i = 0; i < values.Length; i++)
				{
					ParseSerialize.SerializeInt64(values[i], buff, pos + 4 + i * 8);
				}
			}
			pos += 4 + values.Length * 8;
		}

		public static void SetMVReal32(byte[] buff, ref int pos, float[] values)
		{
			ParseSerialize.CheckBounds(pos, buff, 4 + values.Length * 4);
			if (buff != null)
			{
				ParseSerialize.SerializeInt32(values.Length, buff, pos);
				for (int i = 0; i < values.Length; i++)
				{
					ParseSerialize.SerializeSingle(values[i], buff, pos + 4 + i * 4);
				}
			}
			pos += 4 + values.Length * 4;
		}

		public static void SetMVReal64(byte[] buff, ref int pos, double[] values)
		{
			ParseSerialize.CheckBounds(pos, buff, 4 + values.Length * 8);
			if (buff != null)
			{
				ParseSerialize.SerializeInt32(values.Length, buff, pos);
				for (int i = 0; i < values.Length; i++)
				{
					ParseSerialize.SerializeDouble(values[i], buff, pos + 4 + i * 8);
				}
			}
			pos += 4 + values.Length * 8;
		}

		public static void SetMVGuid(byte[] buff, ref int pos, Guid[] values)
		{
			ParseSerialize.CheckBounds(pos, buff, 4 + values.Length * 16);
			if (buff != null)
			{
				ParseSerialize.SerializeInt32(values.Length, buff, pos);
				for (int i = 0; i < values.Length; i++)
				{
					ParseSerialize.SerializeGuid(values[i], buff, pos + 4 + i * 16);
				}
			}
			pos += 4 + values.Length * 16;
		}

		public static void SetMVSystime(byte[] buff, ref int pos, DateTime[] values)
		{
			ParseSerialize.CheckBounds(pos, buff, 4 + values.Length * 8);
			if (buff != null)
			{
				ParseSerialize.SerializeInt32(values.Length, buff, pos);
				for (int i = 0; i < values.Length; i++)
				{
					ParseSerialize.SerializeFileTime(values[i], buff, pos + 4 + i * 8);
				}
			}
			pos += 4 + values.Length * 8;
		}

		public static void SetMVUnicode(byte[] buff, ref int pos, string[] values)
		{
			ParseSerialize.SetDword(buff, ref pos, values.Length);
			for (int i = 0; i < values.Length; i++)
			{
				ParseSerialize.SetUnicodeString(buff, ref pos, values[i]);
			}
		}

		public static void SetMVBinary(byte[] buff, ref int pos, byte[][] values)
		{
			ParseSerialize.SetDword(buff, ref pos, values.Length);
			for (int i = 0; i < values.Length; i++)
			{
				ParseSerialize.SetByteArray(buff, ref pos, values[i]);
			}
		}

		public static void CheckBounds(int pos, int posMax, int sizeNeeded)
		{
			if (!ParseSerialize.CheckOffsetLength(posMax, pos, sizeNeeded))
			{
				throw new BufferTooSmallException("Request would overflow buffer");
			}
		}

		public static void CheckBounds(int pos, byte[] buffer, int sizeNeeded)
		{
			if (buffer != null)
			{
				ParseSerialize.CheckBounds(pos, buffer.Length, sizeNeeded);
			}
		}

		internal static void CheckCount(uint count, int elementSize, int availableSize)
		{
			if (count < 0U)
			{
				throw new BufferTooSmallException("value count is negative");
			}
			if ((ulong)count * (ulong)((long)elementSize) > (ulong)((long)availableSize))
			{
				throw new BufferTooSmallException("overflow available size");
			}
			if ((ulong)count * (ulong)((long)elementSize) > 536870911UL)
			{
				throw new BufferTooSmallException("value count is too large");
			}
		}

		public static byte GetByte(byte[] buff, ref int pos, int posMax)
		{
			ParseSerialize.CheckBounds(pos, posMax, 1);
			byte result = buff[pos];
			pos++;
			return result;
		}

		public static uint GetDword(byte[] buff, ref int pos, int posMax)
		{
			ParseSerialize.CheckBounds(pos, posMax, 4);
			uint result = (uint)ParseSerialize.ParseInt32(buff, pos);
			pos += 4;
			return result;
		}

		public static ushort GetWord(byte[] buff, ref int pos, int posMax)
		{
			ParseSerialize.CheckBounds(pos, posMax, 2);
			ushort result = (ushort)ParseSerialize.ParseInt16(buff, pos);
			pos += 2;
			return result;
		}

		public static float GetFloat(byte[] buff, ref int pos, int posMax)
		{
			ParseSerialize.CheckBounds(pos, posMax, 4);
			float result = ParseSerialize.ParseSingle(buff, pos);
			pos += 4;
			return result;
		}

		public static ulong GetQword(byte[] buff, ref int pos, int posMax)
		{
			ParseSerialize.CheckBounds(pos, posMax, 8);
			ulong result = (ulong)ParseSerialize.ParseInt64(buff, pos);
			pos += 8;
			return result;
		}

		public static double GetDouble(byte[] buff, ref int pos, int posMax)
		{
			ParseSerialize.CheckBounds(pos, posMax, 8);
			double result = ParseSerialize.ParseDouble(buff, pos);
			pos += 8;
			return result;
		}

		public static DateTime GetSysTime(byte[] buff, ref int pos, int posMax)
		{
			ParseSerialize.CheckBounds(pos, posMax, 8);
			DateTime result = ParseSerialize.ParseFileTime(buff, pos);
			pos += 8;
			return result;
		}

		public static bool GetBoolean(byte[] buff, ref int pos, int posMax)
		{
			return 0 != ParseSerialize.GetByte(buff, ref pos, posMax);
		}

		public static Guid GetGuid(byte[] buff, ref int pos, int posMax)
		{
			ParseSerialize.CheckBounds(pos, posMax, 16);
			Guid result = ParseSerialize.ParseGuid(buff, pos);
			pos += 16;
			return result;
		}

		public static byte[][] GetMVBinary(byte[] buff, ref int pos, int posMax)
		{
			uint dword = ParseSerialize.GetDword(buff, ref pos, posMax);
			ParseSerialize.CheckCount(dword, 2, posMax - pos);
			byte[][] array = new byte[dword][];
			int num = 0;
			while ((long)num < (long)((ulong)dword))
			{
				array[num] = ParseSerialize.GetByteArray(buff, ref pos, posMax);
				num++;
			}
			return array;
		}

		public static short[] GetMVInt16(byte[] buff, ref int pos, int posMax)
		{
			uint dword = ParseSerialize.GetDword(buff, ref pos, posMax);
			ParseSerialize.CheckCount(dword, 2, posMax - pos);
			short[] array = new short[dword];
			int num = 0;
			while ((long)num < (long)((ulong)dword))
			{
				array[num] = (short)ParseSerialize.GetWord(buff, ref pos, posMax);
				num++;
			}
			return array;
		}

		public static int[] GetMVInt32(byte[] buff, ref int pos, int posMax)
		{
			uint dword = ParseSerialize.GetDword(buff, ref pos, posMax);
			ParseSerialize.CheckCount(dword, 4, posMax - pos);
			int[] array = new int[dword];
			int num = 0;
			while ((long)num < (long)((ulong)dword))
			{
				array[num] = (int)ParseSerialize.GetDword(buff, ref pos, posMax);
				num++;
			}
			return array;
		}

		public static float[] GetMVReal32(byte[] buff, ref int pos, int posMax)
		{
			uint dword = ParseSerialize.GetDword(buff, ref pos, posMax);
			ParseSerialize.CheckCount(dword, 4, posMax - pos);
			float[] array = new float[dword];
			int num = 0;
			while ((long)num < (long)((ulong)dword))
			{
				array[num] = ParseSerialize.GetFloat(buff, ref pos, posMax);
				num++;
			}
			return array;
		}

		public static double[] GetMVR8(byte[] buff, ref int pos, int posMax)
		{
			uint dword = ParseSerialize.GetDword(buff, ref pos, posMax);
			ParseSerialize.CheckCount(dword, 8, posMax - pos);
			double[] array = new double[dword];
			int num = 0;
			while ((long)num < (long)((ulong)dword))
			{
				array[num] = ParseSerialize.GetDouble(buff, ref pos, posMax);
				num++;
			}
			return array;
		}

		public static long[] GetMVInt64(byte[] buff, ref int pos, int posMax)
		{
			uint dword = ParseSerialize.GetDword(buff, ref pos, posMax);
			ParseSerialize.CheckCount(dword, 8, posMax - pos);
			long[] array = new long[dword];
			int num = 0;
			while ((long)num < (long)((ulong)dword))
			{
				array[num] = (long)ParseSerialize.GetQword(buff, ref pos, posMax);
				num++;
			}
			return array;
		}

		public static DateTime[] GetMVSysTime(byte[] buff, ref int pos, int posMax)
		{
			uint dword = ParseSerialize.GetDword(buff, ref pos, posMax);
			ParseSerialize.CheckCount(dword, 8, posMax - pos);
			DateTime[] array = new DateTime[dword];
			int num = 0;
			while ((long)num < (long)((ulong)dword))
			{
				array[num] = ParseSerialize.GetSysTime(buff, ref pos, posMax);
				num++;
			}
			return array;
		}

		public static Guid[] GetMVGuid(byte[] buff, ref int pos, int posMax)
		{
			uint dword = ParseSerialize.GetDword(buff, ref pos, posMax);
			ParseSerialize.CheckCount(dword, 16, posMax - pos);
			Guid[] array = new Guid[dword];
			int num = 0;
			while ((long)num < (long)((ulong)dword))
			{
				array[num] = ParseSerialize.GetGuid(buff, ref pos, posMax);
				num++;
			}
			return array;
		}

		public static string GetStringFromUnicode(byte[] buff, ref int pos, int posMax)
		{
			int num = 0;
			ParseSerialize.CheckBounds(pos, posMax, 2);
			while (buff[pos + num] != 0 || buff[pos + num + 1] != 0)
			{
				num += 2;
				ParseSerialize.CheckBounds(pos + num, posMax, 2);
			}
			return ParseSerialize.GetStringFromUnicode(buff, ref pos, posMax, num + 2);
		}

		public static string GetStringFromUnicode(byte[] buff, ref int pos, int posMax, int byteCount)
		{
			ParseSerialize.CheckBounds(pos, posMax, byteCount);
			string @string = Encoding.Unicode.GetString(buff, pos, byteCount - 2);
			pos += byteCount;
			return @string;
		}

		public static byte PeekByte(byte[] buff, int pos, int posMax)
		{
			ParseSerialize.CheckBounds(pos, posMax, 1);
			return buff[pos];
		}

		public static string GetStringFromASCII(byte[] buff, ref int pos, int posMax)
		{
			int num = 0;
			while (pos + num < posMax && buff[pos + num] != 0)
			{
				num++;
			}
			if (pos + num >= posMax)
			{
				throw new BufferTooSmallException("Request would overflow buffer");
			}
			return ParseSerialize.GetStringFromASCII(buff, ref pos, posMax, num + 1);
		}

		public static string GetStringFromASCII(byte[] buff, ref int pos, int posMax, int charCount)
		{
			ParseSerialize.CheckBounds(pos, posMax, charCount);
			string @string = CTSGlobals.AsciiEncoding.GetString(buff, pos, charCount - 1);
			pos += charCount;
			return @string;
		}

		public static string GetStringFromASCIINoNull(byte[] buff, ref int pos, int posMax, int charCount)
		{
			ParseSerialize.CheckBounds(pos, posMax, charCount);
			string @string = CTSGlobals.AsciiEncoding.GetString(buff, pos, charCount);
			pos += charCount;
			return @string;
		}

		public static string[] GetMVUnicode(byte[] buff, ref int pos, int posMax)
		{
			uint dword = ParseSerialize.GetDword(buff, ref pos, posMax);
			ParseSerialize.CheckCount(dword, 2, posMax - pos);
			string[] array = new string[dword];
			int num = 0;
			while ((long)num < (long)((ulong)dword))
			{
				array[num] = ParseSerialize.GetStringFromUnicode(buff, ref pos, posMax);
				num++;
			}
			return array;
		}

		public static byte[] GetByteArray(byte[] buff, ref int pos, int posMax)
		{
			int word = (int)ParseSerialize.GetWord(buff, ref pos, posMax);
			ParseSerialize.CheckBounds(pos, posMax, word);
			byte[] result = ParseSerialize.ParseBinary(buff, pos, word);
			pos += word;
			return result;
		}

		public static string[] GetMVString8(byte[] buff, ref int pos, int posMax)
		{
			uint dword = ParseSerialize.GetDword(buff, ref pos, posMax);
			ParseSerialize.CheckCount(dword, 1, posMax - pos);
			string[] array = new string[dword];
			int num = 0;
			while ((long)num < (long)((ulong)dword))
			{
				array[num] = ParseSerialize.GetStringFromASCII(buff, ref pos, posMax);
				num++;
			}
			return array;
		}

		public const int SizeOfByte = 1;

		public const int SizeOfInt16 = 2;

		public const int SizeOfInt32 = 4;

		public const int SizeOfInt64 = 8;

		public const int SizeOfSingle = 4;

		public const int SizeOfDouble = 8;

		public const int SizeOfGuid = 16;

		public const int SizeOfFileTime = 8;

		public const int SizeOfUnicodeChar = 2;

		public static readonly long MinFileTime = 0L;

		public static readonly long MaxFileTime = DateTime.MaxValue.ToFileTimeUtc();

		public static readonly DateTime MinFileTimeDateTime = DateTime.FromFileTimeUtc(ParseSerialize.MinFileTime);

		private static readonly byte[] emptyByteArray = new byte[0];
	}
}
