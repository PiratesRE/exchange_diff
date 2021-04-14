using System;
using System.Text;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public static class ParseSerialize
	{
		public static void CheckBounds(int pos, int posMax, int sizeNeeded)
		{
			if (!ParseSerialize.TryCheckBounds(pos, posMax, sizeNeeded))
			{
				throw new BufferTooSmall((LID)42104U, "Request would overflow buffer");
			}
		}

		public static bool TryCheckBounds(int pos, int posMax, int sizeNeeded)
		{
			return ParseSerialize.CheckOffsetLength(posMax, pos, sizeNeeded);
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
			if (!ParseSerialize.TryCheckCount(count, elementSize, availableSize))
			{
				throw new BufferTooSmall((LID)58488U, "TryCheckCount failed");
			}
		}

		internal static bool TryCheckCount(uint count, int elementSize, int availableSize)
		{
			if (count < 0U)
			{
				DiagnosticContext.TraceLocation((LID)39900U);
				return false;
			}
			if ((ulong)count * (ulong)((long)elementSize) > (ulong)((long)availableSize))
			{
				DiagnosticContext.TraceLocation((LID)56284U);
				return false;
			}
			if ((ulong)count * (ulong)((long)elementSize) > 536870911UL)
			{
				DiagnosticContext.TraceLocation((LID)43996U);
				return false;
			}
			return true;
		}

		public static byte GetByte(byte[] buff, ref int pos, int posMax)
		{
			byte result;
			if (!ParseSerialize.TryGetByte(buff, ref pos, posMax, out result))
			{
				throw new BufferTooSmall((LID)44604U, "Request would overflow buffer");
			}
			return result;
		}

		public static bool TryGetByte(byte[] buff, ref int pos, int posMax, out byte result)
		{
			if (!ParseSerialize.TryCheckBounds(pos, posMax, 1))
			{
				DiagnosticContext.TraceLocation((LID)60380U);
				result = 0;
				return false;
			}
			result = buff[pos];
			pos++;
			return true;
		}

		public static uint GetDword(byte[] buff, ref int pos, int posMax)
		{
			uint result;
			if (!ParseSerialize.TryGetDword(buff, ref pos, posMax, out result))
			{
				throw new BufferTooSmall((LID)49628U, "Request would overflow buffer");
			}
			return result;
		}

		public static bool TryGetDword(byte[] buff, ref int pos, int posMax, out uint result)
		{
			if (!ParseSerialize.TryCheckBounds(pos, posMax, 4))
			{
				DiagnosticContext.TraceLocation((LID)35804U);
				result = 0U;
				return false;
			}
			result = (uint)ParseSerialize.ParseInt32(buff, pos);
			pos += 4;
			return true;
		}

		public static ushort GetWord(byte[] buff, ref int pos, int posMax)
		{
			ushort result;
			if (!ParseSerialize.TryGetWord(buff, ref pos, posMax, out result))
			{
				throw new BufferTooSmall((LID)55772U, "Request would overflow buffer");
			}
			return result;
		}

		public static bool TryGetWord(byte[] buff, ref int pos, int posMax, out ushort result)
		{
			if (!ParseSerialize.TryCheckBounds(pos, posMax, 2))
			{
				DiagnosticContext.TraceLocation((LID)52188U);
				result = 0;
				return false;
			}
			result = (ushort)ParseSerialize.ParseInt16(buff, pos);
			pos += 2;
			return true;
		}

		public static float GetFloat(byte[] buff, ref int pos, int posMax)
		{
			float result;
			if (!ParseSerialize.TryGetFloat(buff, ref pos, posMax, out result))
			{
				throw new BufferTooSmall((LID)53724U, "Request would overflow buffer");
			}
			return result;
		}

		public static bool TryGetFloat(byte[] buff, ref int pos, int posMax, out float result)
		{
			if (!ParseSerialize.TryCheckBounds(pos, posMax, 4))
			{
				DiagnosticContext.TraceLocation((LID)46044U);
				result = 0f;
				return false;
			}
			result = ParseSerialize.ParseSingle(buff, pos);
			pos += 4;
			return true;
		}

		public static ulong GetQword(byte[] buff, ref int pos, int posMax)
		{
			ulong result;
			if (!ParseSerialize.TryGetQword(buff, ref pos, posMax, out result))
			{
				throw new BufferTooSmall((LID)34268U, "Request would overflow buffer");
			}
			return result;
		}

		public static bool TryGetQword(byte[] buff, ref int pos, int posMax, out ulong result)
		{
			if (!ParseSerialize.TryCheckBounds(pos, posMax, 8))
			{
				DiagnosticContext.TraceLocation((LID)62428U);
				result = 0UL;
				return false;
			}
			result = (ulong)ParseSerialize.ParseInt64(buff, pos);
			pos += 8;
			return true;
		}

		public static double GetDouble(byte[] buff, ref int pos, int posMax)
		{
			double result;
			if (!ParseSerialize.TryGetDouble(buff, ref pos, posMax, out result))
			{
				throw new BufferTooSmall((LID)50652U, "Request would overflow buffer");
			}
			return result;
		}

		public static bool TryGetDouble(byte[] buff, ref int pos, int posMax, out double result)
		{
			if (!ParseSerialize.TryCheckBounds(pos, posMax, 8))
			{
				DiagnosticContext.TraceLocation((LID)37852U);
				result = 0.0;
				return false;
			}
			result = ParseSerialize.ParseDouble(buff, pos);
			pos += 8;
			return true;
		}

		public static DateTime GetSysTime(byte[] buff, ref int pos, int posMax)
		{
			DateTime result;
			if (!ParseSerialize.TryGetSysTime(buff, ref pos, posMax, out result))
			{
				throw new BufferTooSmall((LID)47580U, "Request would overflow buffer");
			}
			return result;
		}

		public static bool TryGetSysTime(byte[] buff, ref int pos, int posMax, out DateTime result)
		{
			if (!ParseSerialize.TryCheckBounds(pos, posMax, 8))
			{
				DiagnosticContext.TraceLocation((LID)54236U);
				result = default(DateTime);
				return false;
			}
			result = ParseSerialize.ParseFileTime(buff, pos);
			pos += 8;
			return true;
		}

		public static bool GetBoolean(byte[] buff, ref int pos, int posMax)
		{
			bool result;
			if (!ParseSerialize.TryGetBoolean(buff, ref pos, posMax, out result))
			{
				throw new BufferTooSmall((LID)59868U, "Request would overflow buffer");
			}
			return result;
		}

		public static bool TryGetBoolean(byte[] buff, ref int pos, int posMax, out bool result)
		{
			byte b;
			if (!ParseSerialize.TryGetByte(buff, ref pos, posMax, out b))
			{
				DiagnosticContext.TraceLocation((LID)41948U);
				result = false;
				return false;
			}
			result = (b != 0);
			return true;
		}

		public static Guid GetGuid(byte[] buff, ref int pos, int posMax)
		{
			Guid result;
			if (!ParseSerialize.TryGetGuid(buff, ref pos, posMax, out result))
			{
				throw new BufferTooSmall((LID)33244U, "Request would overflow buffer");
			}
			return result;
		}

		public static bool TryGetGuid(byte[] buff, ref int pos, int posMax, out Guid result)
		{
			if (!ParseSerialize.TryCheckBounds(pos, posMax, 16))
			{
				DiagnosticContext.TraceLocation((LID)58332U);
				result = default(Guid);
				return false;
			}
			result = ParseSerialize.ParseGuid(buff, pos);
			pos += 16;
			return true;
		}

		public static byte[][] GetMVBinary(byte[] buff, ref int pos, int posMax)
		{
			byte[][] result;
			if (!ParseSerialize.TryGetMVBinary(buff, ref pos, posMax, out result))
			{
				throw new BufferTooSmall((LID)57820U, "Request would overflow buffer");
			}
			return result;
		}

		public static bool TryGetMVBinary(byte[] buff, ref int pos, int posMax, out byte[][] result)
		{
			uint num;
			if (!ParseSerialize.TryGetDword(buff, ref pos, posMax, out num))
			{
				DiagnosticContext.TraceLocation((LID)33756U);
				result = null;
				return false;
			}
			if (!ParseSerialize.TryCheckCount(num, 2, posMax - pos))
			{
				DiagnosticContext.TraceLocation((LID)50140U);
				result = null;
				return false;
			}
			byte[][] array = new byte[num][];
			int num2 = 0;
			while ((long)num2 < (long)((ulong)num))
			{
				if (!ParseSerialize.TryGetByteArray(buff, ref pos, posMax, out array[num2]))
				{
					DiagnosticContext.TraceLocation((LID)48348U);
					result = null;
					return false;
				}
				num2++;
			}
			result = array;
			return true;
		}

		public static short[] GetMVInt16(byte[] buff, ref int pos, int posMax)
		{
			short[] result;
			if (!ParseSerialize.TryGetMVInt16(buff, ref pos, posMax, out result))
			{
				throw new BufferTooSmall((LID)37340U, "Request would overflow buffer");
			}
			return result;
		}

		public static bool TryGetMVInt16(byte[] buff, ref int pos, int posMax, out short[] result)
		{
			uint num;
			if (!ParseSerialize.TryGetDword(buff, ref pos, posMax, out num))
			{
				DiagnosticContext.TraceLocation((LID)64732U);
				result = null;
				return false;
			}
			if (!ParseSerialize.TryCheckCount(num, 2, posMax - pos))
			{
				DiagnosticContext.TraceLocation((LID)40156U);
				result = null;
				return false;
			}
			short[] array = new short[num];
			int num2 = 0;
			while ((long)num2 < (long)((ulong)num))
			{
				ushort num3;
				if (!ParseSerialize.TryGetWord(buff, ref pos, posMax, out num3))
				{
					DiagnosticContext.TraceLocation((LID)56540U);
					result = null;
					return false;
				}
				array[num2] = (short)num3;
				num2++;
			}
			result = array;
			return true;
		}

		public static int[] GetMVInt32(byte[] buff, ref int pos, int posMax)
		{
			int[] result;
			if (!ParseSerialize.TryGetMVInt32(buff, ref pos, posMax, out result))
			{
				throw new BufferTooSmall((LID)39388U, "Request would overflow buffer");
			}
			return result;
		}

		public static bool TryGetMVInt32(byte[] buff, ref int pos, int posMax, out int[] result)
		{
			uint num;
			if (!ParseSerialize.TryGetDword(buff, ref pos, posMax, out num))
			{
				DiagnosticContext.TraceLocation((LID)44252U);
				result = null;
				return false;
			}
			if (!ParseSerialize.TryCheckCount(num, 4, posMax - pos))
			{
				DiagnosticContext.TraceLocation((LID)60636U);
				result = null;
				return false;
			}
			int[] array = new int[num];
			int num2 = 0;
			while ((long)num2 < (long)((ulong)num))
			{
				uint num3;
				if (!ParseSerialize.TryGetDword(buff, ref pos, posMax, out num3))
				{
					DiagnosticContext.TraceLocation((LID)36060U);
					result = null;
					return false;
				}
				array[num2] = (int)num3;
				num2++;
			}
			result = array;
			return true;
		}

		public static float[] GetMVReal32(byte[] buff, ref int pos, int posMax)
		{
			float[] result;
			if (!ParseSerialize.TryGetMVReal32(buff, ref pos, posMax, out result))
			{
				throw new BufferTooSmall((LID)63964U, "Request would overflow buffer");
			}
			return result;
		}

		public static bool TryGetMVReal32(byte[] buff, ref int pos, int posMax, out float[] result)
		{
			uint num;
			if (!ParseSerialize.TryGetDword(buff, ref pos, posMax, out num))
			{
				DiagnosticContext.TraceLocation((LID)52444U);
				result = null;
				return false;
			}
			if (!ParseSerialize.TryCheckCount(num, 4, posMax - pos))
			{
				DiagnosticContext.TraceLocation((LID)46300U);
				result = null;
				return false;
			}
			float[] array = new float[num];
			int num2 = 0;
			while ((long)num2 < (long)((ulong)num))
			{
				if (!ParseSerialize.TryGetFloat(buff, ref pos, posMax, out array[num2]))
				{
					DiagnosticContext.TraceLocation((LID)62684U);
					result = null;
					return false;
				}
				num2++;
			}
			result = array;
			return true;
		}

		public static double[] GetMVR8(byte[] buff, ref int pos, int posMax)
		{
			double[] result;
			if (!ParseSerialize.TryGetMVR8(buff, ref pos, posMax, out result))
			{
				throw new BufferTooSmall((LID)43484U, "Request would overflow buffer");
			}
			return result;
		}

		public static bool TryGetMVR8(byte[] buff, ref int pos, int posMax, out double[] result)
		{
			uint num;
			if (!ParseSerialize.TryGetDword(buff, ref pos, posMax, out num))
			{
				DiagnosticContext.TraceLocation((LID)38108U);
				result = null;
				return false;
			}
			if (!ParseSerialize.TryCheckCount(num, 8, posMax - pos))
			{
				DiagnosticContext.TraceLocation((LID)54492U);
				result = null;
				return false;
			}
			double[] array = new double[num];
			int num2 = 0;
			while ((long)num2 < (long)((ulong)num))
			{
				if (!ParseSerialize.TryGetDouble(buff, ref pos, posMax, out array[num2]))
				{
					DiagnosticContext.TraceLocation((LID)42204U);
					result = null;
					return false;
				}
				num2++;
			}
			result = array;
			return true;
		}

		public static long[] GetMVInt64(byte[] buff, ref int pos, int posMax)
		{
			long[] result;
			if (!ParseSerialize.TryGetMVInt64(buff, ref pos, posMax, out result))
			{
				throw new BufferTooSmall((LID)35292U, "Request would overflow buffer");
			}
			return result;
		}

		public static bool TryGetMVInt64(byte[] buff, ref int pos, int posMax, out long[] result)
		{
			uint num;
			if (!ParseSerialize.TryGetDword(buff, ref pos, posMax, out num))
			{
				DiagnosticContext.TraceLocation((LID)58588U);
				result = null;
				return false;
			}
			if (!ParseSerialize.TryCheckCount(num, 8, posMax - pos))
			{
				DiagnosticContext.TraceLocation((LID)34012U);
				result = null;
				return false;
			}
			long[] array = new long[num];
			int num2 = 0;
			while ((long)num2 < (long)((ulong)num))
			{
				ulong num3;
				if (!ParseSerialize.TryGetQword(buff, ref pos, posMax, out num3))
				{
					DiagnosticContext.TraceLocation((LID)50396U);
					result = null;
					return false;
				}
				array[num2] = (long)num3;
				num2++;
			}
			result = array;
			return true;
		}

		public static DateTime[] GetMVSysTime(byte[] buff, ref int pos, int posMax)
		{
			DateTime[] result;
			if (!ParseSerialize.TryGetMVSysTime(buff, ref pos, posMax, out result))
			{
				throw new BufferTooSmall((LID)51676U, "Request would overflow buffer");
			}
			return result;
		}

		public static bool TryGetMVSysTime(byte[] buff, ref int pos, int posMax, out DateTime[] result)
		{
			uint num;
			if (!ParseSerialize.TryGetDword(buff, ref pos, posMax, out num))
			{
				DiagnosticContext.TraceLocation((LID)47324U);
				result = null;
				return false;
			}
			if (!ParseSerialize.TryCheckCount(num, 8, posMax - pos))
			{
				DiagnosticContext.TraceLocation((LID)63708U);
				result = null;
				return false;
			}
			DateTime[] array = new DateTime[num];
			int num2 = 0;
			while ((long)num2 < (long)((ulong)num))
			{
				if (!ParseSerialize.TryGetSysTime(buff, ref pos, posMax, out array[num2]))
				{
					DiagnosticContext.TraceLocation((LID)39132U);
					result = null;
					return false;
				}
				num2++;
			}
			result = array;
			return true;
		}

		public static Guid[] GetMVGuid(byte[] buff, ref int pos, int posMax)
		{
			Guid[] result;
			if (!ParseSerialize.TryGetMVGuid(buff, ref pos, posMax, out result))
			{
				throw new BufferTooSmall((LID)45532U, "Request would overflow buffer");
			}
			return result;
		}

		public static bool TryGetMVGuid(byte[] buff, ref int pos, int posMax, out Guid[] result)
		{
			uint num;
			if (!ParseSerialize.TryGetDword(buff, ref pos, posMax, out num))
			{
				DiagnosticContext.TraceLocation((LID)55516U);
				result = null;
				return false;
			}
			if (!ParseSerialize.TryCheckCount(num, 16, posMax - pos))
			{
				DiagnosticContext.TraceLocation((LID)43228U);
				result = null;
				return false;
			}
			Guid[] array = new Guid[num];
			int num2 = 0;
			while ((long)num2 < (long)((ulong)num))
			{
				if (!ParseSerialize.TryGetGuid(buff, ref pos, posMax, out array[num2]))
				{
					DiagnosticContext.TraceLocation((LID)59612U);
					result = null;
					return false;
				}
				num2++;
			}
			result = array;
			return true;
		}

		public static string GetStringFromUnicode(byte[] buff, ref int pos, int posMax)
		{
			string result;
			if (!ParseSerialize.TryGetStringFromUnicode(buff, ref pos, posMax, out result))
			{
				throw new BufferTooSmall((LID)52700U, "Request would overflow buffer");
			}
			return result;
		}

		public static bool TryGetStringFromUnicode(byte[] buff, ref int pos, int posMax, out string result)
		{
			int num = 0;
			if (!ParseSerialize.TryCheckBounds(pos, posMax, 2))
			{
				DiagnosticContext.TraceLocation((LID)35036U);
				result = null;
				return false;
			}
			while (buff[pos + num] != 0 || buff[pos + num + 1] != 0)
			{
				num += 2;
				if (!ParseSerialize.TryCheckBounds(pos + num, posMax, 2))
				{
					DiagnosticContext.TraceLocation((LID)51420U);
					result = null;
					return false;
				}
			}
			return ParseSerialize.TryGetStringFromUnicode(buff, ref pos, posMax, num + 2, out result);
		}

		public static string GetStringFromUnicode(byte[] buff, ref int pos, int posMax, int byteCount)
		{
			string result;
			if (!ParseSerialize.TryGetStringFromUnicode(buff, ref pos, posMax, byteCount, out result))
			{
				throw new BufferTooSmall((LID)46556U, "Request would overflow buffer");
			}
			return result;
		}

		public static bool TryGetStringFromUnicode(byte[] buff, ref int pos, int posMax, int byteCount, out string result)
		{
			if (!ParseSerialize.TryCheckBounds(pos, posMax, byteCount))
			{
				DiagnosticContext.TraceLocation((LID)45276U);
				result = null;
				return false;
			}
			result = Encoding.Unicode.GetString(buff, pos, byteCount - 2);
			pos += byteCount;
			return true;
		}

		public static byte PeekByte(byte[] buff, int pos, int posMax)
		{
			byte result;
			if (!ParseSerialize.TryPeekByte(buff, pos, posMax, out result))
			{
				throw new BufferTooSmall((LID)61916U, "Request would overflow buffer");
			}
			return result;
		}

		public static bool TryPeekByte(byte[] buff, int pos, int posMax, out byte result)
		{
			if (!ParseSerialize.TryCheckBounds(pos, posMax, 1))
			{
				DiagnosticContext.TraceLocation((LID)65500U);
				result = 0;
				return false;
			}
			result = buff[pos];
			return true;
		}

		public static string GetStringFromASCII(byte[] buff, ref int pos, int posMax)
		{
			string result;
			if (!ParseSerialize.TryGetStringFromASCII(buff, ref pos, posMax, out result))
			{
				throw new BufferTooSmall((LID)41436U, "Request would overflow buffer");
			}
			return result;
		}

		public static bool TryGetStringFromASCII(byte[] buff, ref int pos, int posMax, out string result)
		{
			int num = 0;
			while (pos + num < posMax && buff[pos + num] != 0)
			{
				num++;
			}
			if (pos + num >= posMax)
			{
				DiagnosticContext.TraceLocation((LID)61404U);
				result = null;
				return false;
			}
			return ParseSerialize.TryGetStringFromASCII(buff, ref pos, posMax, num + 1, out result);
		}

		public static string GetStringFromASCII(byte[] buff, ref int pos, int posMax, int charCount)
		{
			string result;
			if (!ParseSerialize.TryGetStringFromASCII(buff, ref pos, posMax, charCount, out result))
			{
				throw new BufferTooSmall((LID)36316U, "Request would overflow buffer");
			}
			return result;
		}

		public static bool TryGetStringFromASCII(byte[] buff, ref int pos, int posMax, int charCount, out string result)
		{
			if (!ParseSerialize.TryCheckBounds(pos, posMax, charCount))
			{
				DiagnosticContext.TraceLocation((LID)36828U);
				result = null;
				return false;
			}
			result = CTSGlobals.AsciiEncoding.GetString(buff, pos, charCount - 1);
			pos += charCount;
			return true;
		}

		public static string GetStringFromASCIINoNull(byte[] buff, ref int pos, int posMax, int charCount)
		{
			string result;
			if (!ParseSerialize.TryGetStringFromASCIINoNull(buff, ref pos, posMax, charCount, out result))
			{
				throw new BufferTooSmall((LID)49116U, "Request would overflow buffer");
			}
			return result;
		}

		public static bool TryGetStringFromASCIINoNull(byte[] buff, ref int pos, int posMax, int charCount, out string result)
		{
			if (!ParseSerialize.TryCheckBounds(pos, posMax, charCount))
			{
				DiagnosticContext.TraceLocation((LID)32988U);
				result = null;
				return false;
			}
			result = CTSGlobals.AsciiEncoding.GetString(buff, pos, charCount);
			pos += charCount;
			return true;
		}

		public static string[] GetMVUnicode(byte[] buff, ref int pos, int posMax)
		{
			string[] result;
			if (!ParseSerialize.TryGetMVUnicode(buff, ref pos, posMax, out result))
			{
				throw new BufferTooSmall((LID)45020U, "Request would overflow buffer");
			}
			return result;
		}

		public static bool TryGetMVUnicode(byte[] buff, ref int pos, int posMax, out string[] result)
		{
			uint num;
			if (!ParseSerialize.TryGetDword(buff, ref pos, posMax, out num))
			{
				DiagnosticContext.TraceLocation((LID)65244U);
				result = null;
				return false;
			}
			if (!ParseSerialize.TryCheckCount(num, 2, posMax - pos))
			{
				DiagnosticContext.TraceLocation((LID)57564U);
				result = null;
				return false;
			}
			string[] array = new string[num];
			int num2 = 0;
			while ((long)num2 < (long)((ulong)num))
			{
				if (!ParseSerialize.TryGetStringFromUnicode(buff, ref pos, posMax, out array[num2]))
				{
					DiagnosticContext.TraceLocation((LID)48860U);
					result = null;
					return false;
				}
				num2++;
			}
			result = array;
			return true;
		}

		public static byte[] GetByteArray(byte[] buff, ref int pos, int posMax)
		{
			byte[] result;
			if (!ParseSerialize.TryGetByteArray(buff, ref pos, posMax, out result))
			{
				throw new BufferTooSmall((LID)57308U, "Request would overflow buffer");
			}
			return result;
		}

		public static bool TryGetByteArray(byte[] buff, ref int pos, int posMax, out byte[] result)
		{
			ushort num;
			if (!ParseSerialize.TryGetWord(buff, ref pos, posMax, out num))
			{
				DiagnosticContext.TraceLocation((LID)49372U);
				result = null;
				return false;
			}
			if (!ParseSerialize.TryCheckBounds(pos, posMax, (int)num))
			{
				DiagnosticContext.TraceLocation((LID)57052U);
				result = null;
				return false;
			}
			result = ParseSerialize.ParseBinary(buff, pos, (int)num);
			pos += (int)num;
			return true;
		}

		public static string[] GetMVString8(byte[] buff, ref int pos, int posMax)
		{
			string[] result;
			if (!ParseSerialize.TryGetMVString8(buff, ref pos, posMax, out result))
			{
				throw new BufferTooSmall((LID)40924U, "Request would overflow buffer");
			}
			return result;
		}

		public static bool TryGetMVString8(byte[] buff, ref int pos, int posMax, out string[] result)
		{
			uint num;
			if (!ParseSerialize.TryGetDword(buff, ref pos, posMax, out num))
			{
				DiagnosticContext.TraceLocation((LID)40668U);
				result = null;
				return false;
			}
			if (!ParseSerialize.TryCheckCount(num, 1, posMax - pos))
			{
				DiagnosticContext.TraceLocation((LID)36572U);
				result = null;
				return false;
			}
			string[] array = new string[num];
			int num2 = 0;
			while ((long)num2 < (long)((ulong)num))
			{
				if (!ParseSerialize.TryGetStringFromASCII(buff, ref pos, posMax, out array[num2]))
				{
					DiagnosticContext.TraceLocation((LID)34524U);
					result = null;
					return false;
				}
				num2++;
			}
			result = array;
			return true;
		}

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
			return (short)((int)buffer[offset] | (int)buffer[offset + 1] << 8);
		}

		public static int ParseInt32(byte[] buffer, int offset)
		{
			return (int)buffer[offset] | (int)buffer[offset + 1] << 8 | (int)buffer[offset + 2] << 16 | (int)buffer[offset + 3] << 24;
		}

		public static long ParseInt64(byte[] buffer, int offset)
		{
			uint num = (uint)((int)buffer[offset] | (int)buffer[offset + 1] << 8 | (int)buffer[offset + 2] << 16 | (int)buffer[offset + 3] << 24);
			uint num2 = (uint)((int)buffer[offset + 4] | (int)buffer[offset + 5] << 8 | (int)buffer[offset + 6] << 16 | (int)buffer[offset + 7] << 24);
			return (long)((ulong)num | (ulong)num2 << 32);
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

		public static void SetRestrictionByteArray(byte[] buff, ref int pos, byte[] serializedRestriction)
		{
			ParseSerialize.CheckBounds(pos, buff, serializedRestriction.Length);
			if (buff != null)
			{
				Buffer.BlockCopy(serializedRestriction, 0, buff, pos, serializedRestriction.Length);
			}
			pos += serializedRestriction.Length;
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
