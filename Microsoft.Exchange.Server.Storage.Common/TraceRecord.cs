using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods.Linq;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public static class TraceRecord
	{
		public static TraceBuffer Create(Guid recordGuid, bool useBufferPool, bool allowBufferSplit, string strValue)
		{
			int num = 1;
			int num2 = CTSGlobals.AsciiEncoding.GetByteCount(strValue.ValueOrEmpty()) + num;
			TraceBuffer traceBuffer = TraceBuffer.Create(recordGuid, num2, useBufferPool);
			if (num2 > 8064 && !allowBufferSplit)
			{
				string[] reducedStringSizes = TraceRecord.GetReducedStringSizes(new Tuple<Encoding, string>[]
				{
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, strValue)
				}, 0, 8064);
				traceBuffer.WriteAsciiString(reducedStringSizes[0]);
			}
			else
			{
				traceBuffer.WriteAsciiString(strValue);
			}
			return traceBuffer;
		}

		public static TraceBuffer Create(Guid recordGuid, bool useBufferPool, bool allowBufferSplit, string strValue0, string strValue1, string strValue2, string strValue3, string strValue4, string strValue5, string strValue6, long longValue0, long longValue1, int intValue0, int intValue1, int intValue2, int intValue3, int intValue4, int intValue5, long longValue2, int intValue6, long longValue3, int intValue7, int intValue8, long longValue4, bool boolValue0, bool boolValue1, bool boolValue2, long longValue5, string strValue7, string strValue8, string strValue9, string strValueA, string strValueB, long longValue6, long longValue7, int intValue9)
		{
			int num = 119;
			byte value = boolValue0 ? 1 : 0;
			byte value2 = boolValue1 ? 1 : 0;
			byte value3 = boolValue2 ? 1 : 0;
			int num2 = CTSGlobals.AsciiEncoding.GetByteCount(strValue0.ValueOrEmpty()) + CTSGlobals.AsciiEncoding.GetByteCount(strValue1.ValueOrEmpty()) + CTSGlobals.AsciiEncoding.GetByteCount(strValue2.ValueOrEmpty()) + CTSGlobals.AsciiEncoding.GetByteCount(strValue3.ValueOrEmpty()) + CTSGlobals.AsciiEncoding.GetByteCount(strValue4.ValueOrEmpty()) + CTSGlobals.AsciiEncoding.GetByteCount(strValue5.ValueOrEmpty()) + CTSGlobals.AsciiEncoding.GetByteCount(strValue6.ValueOrEmpty()) + CTSGlobals.AsciiEncoding.GetByteCount(strValue7.ValueOrEmpty()) + CTSGlobals.AsciiEncoding.GetByteCount(strValue8.ValueOrEmpty()) + CTSGlobals.AsciiEncoding.GetByteCount(strValue9.ValueOrEmpty()) + CTSGlobals.AsciiEncoding.GetByteCount(strValueA.ValueOrEmpty()) + CTSGlobals.AsciiEncoding.GetByteCount(strValueB.ValueOrEmpty()) + num;
			TraceBuffer traceBuffer = TraceBuffer.Create(recordGuid, num2, useBufferPool);
			if (num2 > 8064 && !allowBufferSplit)
			{
				string[] reducedStringSizes = TraceRecord.GetReducedStringSizes(new Tuple<Encoding, string>[]
				{
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, strValue0),
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, strValue1),
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, strValue2),
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, strValue3),
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, strValue4),
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, strValue5),
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, strValue6),
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, strValue7),
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, strValue8),
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, strValue9),
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, strValueA),
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, strValueB)
				}, num, 8064);
				traceBuffer.WriteAsciiString(reducedStringSizes[0]);
				traceBuffer.WriteAsciiString(reducedStringSizes[1]);
				traceBuffer.WriteAsciiString(reducedStringSizes[2]);
				traceBuffer.WriteAsciiString(reducedStringSizes[3]);
				traceBuffer.WriteAsciiString(reducedStringSizes[4]);
				traceBuffer.WriteAsciiString(reducedStringSizes[5]);
				traceBuffer.WriteAsciiString(reducedStringSizes[6]);
				traceBuffer.WriteLong(longValue0);
				traceBuffer.WriteLong(longValue1);
				traceBuffer.WriteInt(intValue0);
				traceBuffer.WriteInt(intValue1);
				traceBuffer.WriteInt(intValue2);
				traceBuffer.WriteInt(intValue3);
				traceBuffer.WriteInt(intValue4);
				traceBuffer.WriteInt(intValue5);
				traceBuffer.WriteLong(longValue2);
				traceBuffer.WriteInt(intValue6);
				traceBuffer.WriteLong(longValue3);
				traceBuffer.WriteInt(intValue7);
				traceBuffer.WriteInt(intValue8);
				traceBuffer.WriteLong(longValue4);
				traceBuffer.WriteByte(value);
				traceBuffer.WriteByte(value2);
				traceBuffer.WriteByte(value3);
				traceBuffer.WriteLong(longValue5);
				traceBuffer.WriteAsciiString(reducedStringSizes[7]);
				traceBuffer.WriteAsciiString(reducedStringSizes[8]);
				traceBuffer.WriteAsciiString(reducedStringSizes[9]);
				traceBuffer.WriteAsciiString(reducedStringSizes[10]);
				traceBuffer.WriteAsciiString(reducedStringSizes[11]);
				traceBuffer.WriteLong(longValue6);
				traceBuffer.WriteLong(longValue7);
				traceBuffer.WriteInt(intValue9);
			}
			else
			{
				traceBuffer.WriteAsciiString(strValue0);
				traceBuffer.WriteAsciiString(strValue1);
				traceBuffer.WriteAsciiString(strValue2);
				traceBuffer.WriteAsciiString(strValue3);
				traceBuffer.WriteAsciiString(strValue4);
				traceBuffer.WriteAsciiString(strValue5);
				traceBuffer.WriteAsciiString(strValue6);
				traceBuffer.WriteLong(longValue0);
				traceBuffer.WriteLong(longValue1);
				traceBuffer.WriteInt(intValue0);
				traceBuffer.WriteInt(intValue1);
				traceBuffer.WriteInt(intValue2);
				traceBuffer.WriteInt(intValue3);
				traceBuffer.WriteInt(intValue4);
				traceBuffer.WriteInt(intValue5);
				traceBuffer.WriteLong(longValue2);
				traceBuffer.WriteInt(intValue6);
				traceBuffer.WriteLong(longValue3);
				traceBuffer.WriteInt(intValue7);
				traceBuffer.WriteInt(intValue8);
				traceBuffer.WriteLong(longValue4);
				traceBuffer.WriteByte(value);
				traceBuffer.WriteByte(value2);
				traceBuffer.WriteByte(value3);
				traceBuffer.WriteLong(longValue5);
				traceBuffer.WriteAsciiString(strValue7);
				traceBuffer.WriteAsciiString(strValue8);
				traceBuffer.WriteAsciiString(strValue9);
				traceBuffer.WriteAsciiString(strValueA);
				traceBuffer.WriteAsciiString(strValueB);
				traceBuffer.WriteLong(longValue6);
				traceBuffer.WriteLong(longValue7);
				traceBuffer.WriteInt(intValue9);
			}
			return traceBuffer;
		}

		public static TraceBuffer Create(Guid recordGuid, bool useBufferPool, bool allowBufferSplit, short shortValue0, string asciiString0, string asciiString1, string asciiString2, string asciiString3, string asciiString4, int intValue0, int intValue1, string unicodeString0, byte byteValue0, string unicodeString1, byte byteValue1, string asciiString5, int intValue2, byte byteValue2, string asciiString6, int intValue3, int intValue4, int intValue5, int intValue6, int intValue7, byte byteValue3, int intValue8, int intValue9, int intValue10, int intValue11, int intValue12, int intValue13, int intValue14, int intValue15, string asciiString7, int intValue16, int intValue17, int intValue18, string asciiString8, uint intValue19, string asciiString9, string unicodeString2, string asciiString10)
		{
			int num = 103;
			int num2 = CTSGlobals.AsciiEncoding.GetByteCount(asciiString0.ValueOrEmpty()) + CTSGlobals.AsciiEncoding.GetByteCount(asciiString1.ValueOrEmpty()) + CTSGlobals.AsciiEncoding.GetByteCount(asciiString2.ValueOrEmpty()) + CTSGlobals.AsciiEncoding.GetByteCount(asciiString3.ValueOrEmpty()) + CTSGlobals.AsciiEncoding.GetByteCount(asciiString4.ValueOrEmpty()) + CTSGlobals.AsciiEncoding.GetByteCount(asciiString5.ValueOrEmpty()) + CTSGlobals.AsciiEncoding.GetByteCount(asciiString6.ValueOrEmpty()) + CTSGlobals.AsciiEncoding.GetByteCount(asciiString7.ValueOrEmpty()) + CTSGlobals.AsciiEncoding.GetByteCount(asciiString8.ValueOrEmpty()) + CTSGlobals.AsciiEncoding.GetByteCount(asciiString9.ValueOrEmpty()) + CTSGlobals.AsciiEncoding.GetByteCount(asciiString10.ValueOrEmpty()) + CTSGlobals.UnicodeEncoding.GetByteCount(unicodeString0.ValueOrEmpty()) + CTSGlobals.UnicodeEncoding.GetByteCount(unicodeString1.ValueOrEmpty()) + CTSGlobals.UnicodeEncoding.GetByteCount(unicodeString2.ValueOrEmpty()) + num;
			TraceBuffer traceBuffer = TraceBuffer.Create(recordGuid, num2, useBufferPool);
			if (num2 > 8064 && !allowBufferSplit)
			{
				string[] reducedStringSizes = TraceRecord.GetReducedStringSizes(new Tuple<Encoding, string>[]
				{
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, asciiString0),
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, asciiString1),
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, asciiString2),
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, asciiString3),
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, asciiString4),
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, asciiString5),
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, asciiString6),
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, asciiString7),
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, asciiString8),
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, asciiString9),
					new Tuple<Encoding, string>(CTSGlobals.UnicodeEncoding, unicodeString0),
					new Tuple<Encoding, string>(CTSGlobals.UnicodeEncoding, unicodeString1),
					new Tuple<Encoding, string>(CTSGlobals.UnicodeEncoding, unicodeString2),
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, asciiString10)
				}, num, 8064);
				traceBuffer.WriteShort(shortValue0);
				traceBuffer.WriteAsciiString(reducedStringSizes[0]);
				traceBuffer.WriteAsciiString(reducedStringSizes[1]);
				traceBuffer.WriteAsciiString(reducedStringSizes[2]);
				traceBuffer.WriteAsciiString(reducedStringSizes[3]);
				traceBuffer.WriteAsciiString(reducedStringSizes[4]);
				traceBuffer.WriteInt(intValue0);
				traceBuffer.WriteInt(intValue1);
				traceBuffer.WriteUnicodeString(reducedStringSizes[10]);
				traceBuffer.WriteByte(byteValue0);
				traceBuffer.WriteUnicodeString(reducedStringSizes[11]);
				traceBuffer.WriteByte(byteValue1);
				traceBuffer.WriteAsciiString(reducedStringSizes[5]);
				traceBuffer.WriteInt(intValue2);
				traceBuffer.WriteAsciiString(reducedStringSizes[6]);
				traceBuffer.WriteInt(intValue3);
				traceBuffer.WriteInt(intValue4);
				traceBuffer.WriteInt(intValue5);
				traceBuffer.WriteInt(intValue6);
				traceBuffer.WriteInt(intValue7);
				traceBuffer.WriteByte(byteValue3);
				traceBuffer.WriteInt(intValue8);
				traceBuffer.WriteInt(intValue9);
				traceBuffer.WriteInt(intValue10);
				traceBuffer.WriteInt(intValue11);
				traceBuffer.WriteInt(intValue12);
				traceBuffer.WriteInt(intValue13);
				traceBuffer.WriteInt(intValue14);
				traceBuffer.WriteInt(intValue15);
				traceBuffer.WriteAsciiString(reducedStringSizes[7]);
				traceBuffer.WriteInt(intValue16);
				traceBuffer.WriteInt(intValue17);
				traceBuffer.WriteInt(intValue18);
				traceBuffer.WriteAsciiString(reducedStringSizes[8]);
				traceBuffer.WriteInt(intValue19);
				traceBuffer.WriteAsciiString(reducedStringSizes[9]);
				traceBuffer.WriteUnicodeString(reducedStringSizes[12]);
				traceBuffer.WriteAsciiString(reducedStringSizes[13]);
			}
			else
			{
				traceBuffer.WriteShort(shortValue0);
				traceBuffer.WriteAsciiString(asciiString0);
				traceBuffer.WriteAsciiString(asciiString1);
				traceBuffer.WriteAsciiString(asciiString2);
				traceBuffer.WriteAsciiString(asciiString3);
				traceBuffer.WriteAsciiString(asciiString4);
				traceBuffer.WriteInt(intValue0);
				traceBuffer.WriteInt(intValue1);
				traceBuffer.WriteUnicodeString(unicodeString0);
				traceBuffer.WriteByte(byteValue0);
				traceBuffer.WriteUnicodeString(unicodeString1);
				traceBuffer.WriteByte(byteValue1);
				traceBuffer.WriteAsciiString(asciiString5);
				traceBuffer.WriteInt(intValue2);
				traceBuffer.WriteAsciiString(asciiString6);
				traceBuffer.WriteInt(intValue3);
				traceBuffer.WriteInt(intValue4);
				traceBuffer.WriteInt(intValue5);
				traceBuffer.WriteInt(intValue6);
				traceBuffer.WriteInt(intValue7);
				traceBuffer.WriteByte(byteValue3);
				traceBuffer.WriteInt(intValue8);
				traceBuffer.WriteInt(intValue9);
				traceBuffer.WriteInt(intValue10);
				traceBuffer.WriteInt(intValue11);
				traceBuffer.WriteInt(intValue12);
				traceBuffer.WriteInt(intValue13);
				traceBuffer.WriteInt(intValue14);
				traceBuffer.WriteInt(intValue15);
				traceBuffer.WriteAsciiString(asciiString7);
				traceBuffer.WriteInt(intValue16);
				traceBuffer.WriteInt(intValue17);
				traceBuffer.WriteInt(intValue18);
				traceBuffer.WriteAsciiString(asciiString8);
				traceBuffer.WriteInt(intValue19);
				traceBuffer.WriteAsciiString(asciiString9);
				traceBuffer.WriteUnicodeString(unicodeString2);
				traceBuffer.WriteAsciiString(asciiString10);
			}
			return traceBuffer;
		}

		public static TraceBuffer Create(Guid recordGuid, bool useBufferPool, bool allowBufferSplit, string strValue0, string strValue1, string strValue2, string strValue3, string strValue4, uint uintValue0, uint uintValue1, string strValue5)
		{
			int num = 14;
			int num2 = CTSGlobals.AsciiEncoding.GetByteCount(strValue0.ValueOrEmpty()) + CTSGlobals.AsciiEncoding.GetByteCount(strValue1.ValueOrEmpty()) + CTSGlobals.AsciiEncoding.GetByteCount(strValue2.ValueOrEmpty()) + CTSGlobals.AsciiEncoding.GetByteCount(strValue3.ValueOrEmpty()) + CTSGlobals.AsciiEncoding.GetByteCount(strValue4.ValueOrEmpty()) + CTSGlobals.AsciiEncoding.GetByteCount(strValue5.ValueOrEmpty()) + num;
			TraceBuffer traceBuffer = TraceBuffer.Create(recordGuid, num2, useBufferPool);
			if (num2 > 8064 && !allowBufferSplit)
			{
				string[] reducedStringSizes = TraceRecord.GetReducedStringSizes(new Tuple<Encoding, string>[]
				{
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, strValue0),
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, strValue1),
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, strValue2),
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, strValue3),
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, strValue4),
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, strValue5)
				}, num, 8064);
				traceBuffer.WriteAsciiString(reducedStringSizes[0]);
				traceBuffer.WriteAsciiString(reducedStringSizes[1]);
				traceBuffer.WriteAsciiString(reducedStringSizes[2]);
				traceBuffer.WriteAsciiString(reducedStringSizes[3]);
				traceBuffer.WriteAsciiString(reducedStringSizes[4]);
				traceBuffer.WriteInt(uintValue0);
				traceBuffer.WriteInt(uintValue1);
				traceBuffer.WriteAsciiString(reducedStringSizes[5]);
			}
			else
			{
				traceBuffer.WriteAsciiString(strValue0);
				traceBuffer.WriteAsciiString(strValue1);
				traceBuffer.WriteAsciiString(strValue2);
				traceBuffer.WriteAsciiString(strValue3);
				traceBuffer.WriteAsciiString(strValue4);
				traceBuffer.WriteInt(uintValue0);
				traceBuffer.WriteInt(uintValue1);
				traceBuffer.WriteAsciiString(strValue5);
			}
			return traceBuffer;
		}

		public static TraceBuffer Create(Guid recordGuid, bool useBufferPool, bool allowBufferSplit, int intValue0, uint uintValue0, byte byteValue0, int intValue1, int intValue2, byte byteValue1, uint uintValue1, byte byteValue2, uint uintValue2, bool boolValue0, uint uintValue3, uint uintValue4, uint uintValue5, uint uintValue6, uint uintValue7, uint uintValue8, uint uintValue9, uint uintValueA, uint uintValueB, uint uintValueC, uint uintValueD, uint uintValueE, uint uintValueF, uint uintValueG, uint uintValueH, uint uintValueI, uint uintValueJ, uint uintValueK, uint uintValueL, uint uintValueM, uint uintValueN, uint uintValueO, uint uintValueP, uint uintValueQ, int intValue3, int intValue4, int intValue5, int intValue6, int intValue7)
		{
			int length = 144;
			TraceBuffer traceBuffer = TraceBuffer.Create(recordGuid, length, useBufferPool);
			byte value = boolValue0 ? 1 : 0;
			traceBuffer.WriteInt(intValue0);
			traceBuffer.WriteInt(uintValue0);
			traceBuffer.WriteByte(byteValue0);
			traceBuffer.WriteInt(intValue1);
			traceBuffer.WriteInt(intValue2);
			traceBuffer.WriteByte(byteValue1);
			traceBuffer.WriteInt(uintValue1);
			traceBuffer.WriteByte(byteValue2);
			traceBuffer.WriteInt(uintValue2);
			traceBuffer.WriteByte(value);
			traceBuffer.WriteInt(uintValue3);
			traceBuffer.WriteInt(uintValue4);
			traceBuffer.WriteInt(uintValue5);
			traceBuffer.WriteInt(uintValue6);
			traceBuffer.WriteInt(uintValue7);
			traceBuffer.WriteInt(uintValue8);
			traceBuffer.WriteInt(uintValue9);
			traceBuffer.WriteInt(uintValueA);
			traceBuffer.WriteInt(uintValueB);
			traceBuffer.WriteInt(uintValueC);
			traceBuffer.WriteInt(uintValueD);
			traceBuffer.WriteInt(uintValueE);
			traceBuffer.WriteInt(uintValueF);
			traceBuffer.WriteInt(uintValueG);
			traceBuffer.WriteInt(uintValueH);
			traceBuffer.WriteInt(uintValueI);
			traceBuffer.WriteInt(uintValueJ);
			traceBuffer.WriteInt(uintValueK);
			traceBuffer.WriteInt(uintValueL);
			traceBuffer.WriteInt(uintValueM);
			traceBuffer.WriteInt(uintValueN);
			traceBuffer.WriteInt(uintValueO);
			traceBuffer.WriteInt(uintValueP);
			traceBuffer.WriteInt(uintValueQ);
			traceBuffer.WriteInt(intValue3);
			traceBuffer.WriteInt(intValue4);
			traceBuffer.WriteInt(intValue5);
			traceBuffer.WriteInt(intValue6);
			traceBuffer.WriteInt(intValue7);
			return traceBuffer;
		}

		public static TraceBuffer Create(Guid recordGuid, bool useBufferPool, bool allowBufferSplit, string strValue0, string strValue1, string strValue2, string strValue3, string strValue4, string strValue5, string strValue6, string strValue7, string strValue8, int intValue0, int intValue1, int intValue2)
		{
			int num = 30;
			int num2 = CTSGlobals.AsciiEncoding.GetByteCount(strValue0.ValueOrEmpty()) + CTSGlobals.AsciiEncoding.GetByteCount(strValue1.ValueOrEmpty()) + CTSGlobals.AsciiEncoding.GetByteCount(strValue2.ValueOrEmpty()) + CTSGlobals.AsciiEncoding.GetByteCount(strValue3.ValueOrEmpty()) + CTSGlobals.AsciiEncoding.GetByteCount(strValue4.ValueOrEmpty()) + CTSGlobals.AsciiEncoding.GetByteCount(strValue5.ValueOrEmpty()) + CTSGlobals.AsciiEncoding.GetByteCount(strValue6.ValueOrEmpty()) + CTSGlobals.AsciiEncoding.GetByteCount(strValue7.ValueOrEmpty()) + CTSGlobals.AsciiEncoding.GetByteCount(strValue8.ValueOrEmpty()) + num;
			TraceBuffer traceBuffer = TraceBuffer.Create(recordGuid, num2, useBufferPool);
			if (num2 > 8064 && !allowBufferSplit)
			{
				string[] reducedStringSizes = TraceRecord.GetReducedStringSizes(new Tuple<Encoding, string>[]
				{
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, strValue0),
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, strValue1),
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, strValue2),
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, strValue3),
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, strValue4),
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, strValue5),
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, strValue6),
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, strValue7),
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, strValue8)
				}, num, 8064);
				traceBuffer.WriteCountedAsciiString(reducedStringSizes[0]);
				traceBuffer.WriteCountedAsciiString(reducedStringSizes[1]);
				traceBuffer.WriteCountedAsciiString(reducedStringSizes[2]);
				traceBuffer.WriteCountedAsciiString(reducedStringSizes[3]);
				traceBuffer.WriteCountedAsciiString(reducedStringSizes[4]);
				traceBuffer.WriteCountedAsciiString(reducedStringSizes[5]);
				traceBuffer.WriteCountedAsciiString(reducedStringSizes[6]);
				traceBuffer.WriteCountedAsciiString(reducedStringSizes[7]);
				traceBuffer.WriteCountedAsciiString(reducedStringSizes[8]);
			}
			else
			{
				traceBuffer.WriteCountedAsciiString(strValue0);
				traceBuffer.WriteCountedAsciiString(strValue1);
				traceBuffer.WriteCountedAsciiString(strValue2);
				traceBuffer.WriteCountedAsciiString(strValue3);
				traceBuffer.WriteCountedAsciiString(strValue4);
				traceBuffer.WriteCountedAsciiString(strValue5);
				traceBuffer.WriteCountedAsciiString(strValue6);
				traceBuffer.WriteCountedAsciiString(strValue7);
				traceBuffer.WriteCountedAsciiString(strValue8);
			}
			traceBuffer.WriteInt(intValue0);
			traceBuffer.WriteInt(intValue1);
			traceBuffer.WriteInt(intValue2);
			return traceBuffer;
		}

		public static TraceBuffer Create(Guid recordGuid, bool useBufferPool, bool allowBufferSplit, short shortValue0, string strValue0, string strValue1, int intValue0, int intValue1, int intValue2, string strValue2)
		{
			int num = 17;
			int num2 = CTSGlobals.AsciiEncoding.GetByteCount(strValue0.ValueOrEmpty()) + CTSGlobals.AsciiEncoding.GetByteCount(strValue1.ValueOrEmpty()) + CTSGlobals.AsciiEncoding.GetByteCount(strValue2.ValueOrEmpty()) + num;
			TraceBuffer traceBuffer = TraceBuffer.Create(recordGuid, num2, useBufferPool);
			if (num2 > 8064 && !allowBufferSplit)
			{
				string[] reducedStringSizes = TraceRecord.GetReducedStringSizes(new Tuple<Encoding, string>[]
				{
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, strValue0),
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, strValue1),
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, strValue2)
				}, num, 8064);
				traceBuffer.WriteShort(shortValue0);
				traceBuffer.WriteAsciiString(reducedStringSizes[0]);
				traceBuffer.WriteAsciiString(reducedStringSizes[1]);
				traceBuffer.WriteInt(intValue0);
				traceBuffer.WriteInt(intValue1);
				traceBuffer.WriteInt(intValue2);
				traceBuffer.WriteAsciiString(reducedStringSizes[2]);
			}
			else
			{
				traceBuffer.WriteShort(shortValue0);
				traceBuffer.WriteAsciiString(strValue0);
				traceBuffer.WriteAsciiString(strValue1);
				traceBuffer.WriteInt(intValue0);
				traceBuffer.WriteInt(intValue1);
				traceBuffer.WriteInt(intValue2);
				traceBuffer.WriteAsciiString(strValue2);
			}
			return traceBuffer;
		}

		public static TraceBuffer Create(Guid recordGuid, bool useBufferPool, bool allowBufferSplit, int intValue0, int intValue1, byte byteValue0, uint uintValue0, byte byteValue1, uint uintValue1, byte byteValue2, byte byteValue3, uint uintValue2, byte byteValue4, byte byteValue5, uint uintValue3, byte byteValue6, byte byteValue7)
		{
			int length = 32;
			TraceBuffer traceBuffer = TraceBuffer.Create(recordGuid, length, useBufferPool);
			traceBuffer.WriteInt(intValue0);
			traceBuffer.WriteInt(intValue1);
			traceBuffer.WriteByte(byteValue0);
			traceBuffer.WriteInt(uintValue0);
			traceBuffer.WriteByte(byteValue1);
			traceBuffer.WriteInt(uintValue1);
			traceBuffer.WriteByte(byteValue2);
			traceBuffer.WriteByte(byteValue3);
			traceBuffer.WriteInt(uintValue2);
			traceBuffer.WriteByte(byteValue4);
			traceBuffer.WriteByte(byteValue5);
			traceBuffer.WriteInt(uintValue3);
			traceBuffer.WriteByte(byteValue6);
			traceBuffer.WriteByte(byteValue7);
			return traceBuffer;
		}

		public static TraceBuffer Create(Guid recordGuid, bool useBufferPool, bool allowBufferSplit, int intKey, string strValue)
		{
			int num = 4;
			int num2 = CTSGlobals.AsciiEncoding.GetByteCount(strValue.ValueOrEmpty()) + num;
			TraceBuffer traceBuffer = TraceBuffer.Create(recordGuid, num2, useBufferPool);
			if (num2 > 8064 && !allowBufferSplit)
			{
				string[] reducedStringSizes = TraceRecord.GetReducedStringSizes(new Tuple<Encoding, string>[]
				{
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, strValue)
				}, num, 8064);
				strValue = reducedStringSizes[0];
			}
			traceBuffer.WriteInt(intKey);
			traceBuffer.WriteAsciiString(strValue);
			return traceBuffer;
		}

		public static TraceBuffer Create(Guid recordGuid, bool useBufferPool, bool allowBufferSplit, int intKey, string strValue0, int intValue, bool boolValue0, bool boolValue1, bool boolValue2, bool boolValue3, bool boolValue4, string strValue1)
		{
			int num = 15;
			int num2 = CTSGlobals.AsciiEncoding.GetByteCount(strValue0.ValueOrEmpty()) + CTSGlobals.AsciiEncoding.GetByteCount(strValue1.ValueOrEmpty()) + num;
			TraceBuffer traceBuffer = TraceBuffer.Create(recordGuid, num2, useBufferPool);
			byte value = boolValue0 ? 1 : 0;
			byte value2 = boolValue1 ? 1 : 0;
			byte value3 = boolValue2 ? 1 : 0;
			byte value4 = boolValue3 ? 1 : 0;
			byte value5 = boolValue4 ? 1 : 0;
			if (num2 > 8064 && !allowBufferSplit)
			{
				string[] reducedStringSizes = TraceRecord.GetReducedStringSizes(new Tuple<Encoding, string>[]
				{
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, strValue0),
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, strValue1)
				}, num, 8064);
				traceBuffer.WriteInt(intKey);
				traceBuffer.WriteAsciiString(reducedStringSizes[0]);
				traceBuffer.WriteInt(intValue);
				traceBuffer.WriteByte(value);
				traceBuffer.WriteByte(value2);
				traceBuffer.WriteByte(value3);
				traceBuffer.WriteByte(value4);
				traceBuffer.WriteByte(value5);
				traceBuffer.WriteAsciiString(reducedStringSizes[1]);
			}
			else
			{
				traceBuffer.WriteInt(intKey);
				traceBuffer.WriteAsciiString(strValue0);
				traceBuffer.WriteInt(intValue);
				traceBuffer.WriteByte(value);
				traceBuffer.WriteByte(value2);
				traceBuffer.WriteByte(value3);
				traceBuffer.WriteByte(value4);
				traceBuffer.WriteByte(value5);
				traceBuffer.WriteAsciiString(strValue1);
			}
			return traceBuffer;
		}

		public static TraceBuffer Create(Guid recordGuid, bool useBufferPool, bool allowBufferSplit, string strValue0, string strValue1, int intValue0, int intValue1, byte byteValue0, byte byteValue1, byte byteValue2, bool boolValue0, bool boolValue1, bool boolValue2, long longValue0, long longValue1, long longValue2, long longValue3, long longValue4, long longValue5, long longValue6, long longValue7, long longValue8, int intValue2, int intValue3, int intValue4, int intValue5, int intValue6, int intValue7, int intValue8, string strValue2, int intValue9, int intValueA)
		{
			int num = 125;
			int num2 = CTSGlobals.AsciiEncoding.GetByteCount(strValue0.ValueOrEmpty()) + CTSGlobals.AsciiEncoding.GetByteCount(strValue1.ValueOrEmpty()) + CTSGlobals.AsciiEncoding.GetByteCount(strValue2.ValueOrEmpty()) + num;
			byte value = boolValue0 ? 1 : 0;
			byte value2 = boolValue1 ? 1 : 0;
			byte value3 = boolValue2 ? 1 : 0;
			TraceBuffer traceBuffer = TraceBuffer.Create(recordGuid, num2, useBufferPool);
			if (num2 > 8064 && !allowBufferSplit)
			{
				string[] reducedStringSizes = TraceRecord.GetReducedStringSizes(new Tuple<Encoding, string>[]
				{
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, strValue0),
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, strValue1),
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, strValue2)
				}, num, 8064);
				traceBuffer.WriteAsciiString(reducedStringSizes[0]);
				traceBuffer.WriteAsciiString(reducedStringSizes[1]);
				traceBuffer.WriteInt(intValue0);
				traceBuffer.WriteInt(intValue1);
				traceBuffer.WriteByte(byteValue0);
				traceBuffer.WriteByte(byteValue1);
				traceBuffer.WriteByte(byteValue2);
				traceBuffer.WriteByte(value);
				traceBuffer.WriteByte(value2);
				traceBuffer.WriteByte(value3);
				traceBuffer.WriteLong(longValue0);
				traceBuffer.WriteLong(longValue1);
				traceBuffer.WriteLong(longValue2);
				traceBuffer.WriteLong(longValue3);
				traceBuffer.WriteLong(longValue4);
				traceBuffer.WriteLong(longValue5);
				traceBuffer.WriteLong(longValue6);
				traceBuffer.WriteLong(longValue7);
				traceBuffer.WriteLong(longValue8);
				traceBuffer.WriteInt(intValue2);
				traceBuffer.WriteInt(intValue3);
				traceBuffer.WriteInt(intValue4);
				traceBuffer.WriteInt(intValue5);
				traceBuffer.WriteInt(intValue6);
				traceBuffer.WriteInt(intValue7);
				traceBuffer.WriteInt(intValue8);
				traceBuffer.WriteAsciiString(reducedStringSizes[2]);
				traceBuffer.WriteInt(intValue9);
				traceBuffer.WriteInt(intValueA);
			}
			else
			{
				traceBuffer.WriteAsciiString(strValue0);
				traceBuffer.WriteAsciiString(strValue1);
				traceBuffer.WriteInt(intValue0);
				traceBuffer.WriteInt(intValue1);
				traceBuffer.WriteByte(byteValue0);
				traceBuffer.WriteByte(byteValue1);
				traceBuffer.WriteByte(byteValue2);
				traceBuffer.WriteByte(value);
				traceBuffer.WriteByte(value2);
				traceBuffer.WriteByte(value3);
				traceBuffer.WriteLong(longValue0);
				traceBuffer.WriteLong(longValue1);
				traceBuffer.WriteLong(longValue2);
				traceBuffer.WriteLong(longValue3);
				traceBuffer.WriteLong(longValue4);
				traceBuffer.WriteLong(longValue5);
				traceBuffer.WriteLong(longValue6);
				traceBuffer.WriteLong(longValue7);
				traceBuffer.WriteLong(longValue8);
				traceBuffer.WriteInt(intValue2);
				traceBuffer.WriteInt(intValue3);
				traceBuffer.WriteInt(intValue4);
				traceBuffer.WriteInt(intValue5);
				traceBuffer.WriteInt(intValue6);
				traceBuffer.WriteInt(intValue7);
				traceBuffer.WriteInt(intValue8);
				traceBuffer.WriteAsciiString(strValue2);
				traceBuffer.WriteInt(intValue9);
				traceBuffer.WriteInt(intValueA);
			}
			return traceBuffer;
		}

		public static TraceBuffer Create(Guid recordGuid, bool useBufferPool, bool allowBufferSplit, int intValue0, int intValue1, byte byteValue0, uint uintValue0, byte byteValue1, int intValue2, int intValue3, int intValue4, int intValue5, int intValue6, int intValue7, int intValue8, int intValue9, int intValueA, int intValueB, int intValueC, int intValueD)
		{
			int length = 62;
			TraceBuffer traceBuffer = TraceBuffer.Create(recordGuid, length, useBufferPool);
			traceBuffer.WriteInt(intValue0);
			traceBuffer.WriteInt(intValue1);
			traceBuffer.WriteByte(byteValue0);
			traceBuffer.WriteInt(uintValue0);
			traceBuffer.WriteByte(byteValue1);
			traceBuffer.WriteInt(intValue2);
			traceBuffer.WriteInt(intValue3);
			traceBuffer.WriteInt(intValue4);
			traceBuffer.WriteInt(intValue5);
			traceBuffer.WriteInt(intValue6);
			traceBuffer.WriteInt(intValue7);
			traceBuffer.WriteInt(intValue8);
			traceBuffer.WriteInt(intValue9);
			traceBuffer.WriteInt(intValueA);
			traceBuffer.WriteInt(intValueB);
			traceBuffer.WriteInt(intValueC);
			traceBuffer.WriteInt(intValueD);
			return traceBuffer;
		}

		public static TraceBuffer Create(Guid recordGuid, bool useBufferPool, bool allowBufferSplit, byte byteValue0, byte byteValue1, byte byteValue2, byte byteValue3, int intValue0, int intValue1, int intValue2, string strValue0, string strValue1, string strValue2)
		{
			int num = 19;
			int num2 = CTSGlobals.AsciiEncoding.GetByteCount(strValue0.ValueOrEmpty()) + CTSGlobals.AsciiEncoding.GetByteCount(strValue1.ValueOrEmpty()) + CTSGlobals.AsciiEncoding.GetByteCount(strValue2.ValueOrEmpty()) + num;
			TraceBuffer traceBuffer = TraceBuffer.Create(recordGuid, num2, useBufferPool);
			traceBuffer.WriteByte(byteValue0);
			traceBuffer.WriteByte(byteValue1);
			traceBuffer.WriteByte(byteValue2);
			traceBuffer.WriteByte(byteValue3);
			traceBuffer.WriteInt(intValue0);
			traceBuffer.WriteInt(intValue1);
			traceBuffer.WriteInt(intValue2);
			if (num2 > 8064 && !allowBufferSplit)
			{
				string[] reducedStringSizes = TraceRecord.GetReducedStringSizes(new Tuple<Encoding, string>[]
				{
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, strValue0),
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, strValue1),
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, strValue2)
				}, num, 8064);
				traceBuffer.WriteAsciiString(reducedStringSizes[0]);
				traceBuffer.WriteAsciiString(reducedStringSizes[1]);
				traceBuffer.WriteAsciiString(reducedStringSizes[2]);
			}
			else
			{
				traceBuffer.WriteAsciiString(strValue0);
				traceBuffer.WriteAsciiString(strValue1);
				traceBuffer.WriteAsciiString(strValue2);
			}
			return traceBuffer;
		}

		public static TraceBuffer Create(Guid recordGuid, bool useBufferPool, bool allowBufferSplit, string strValue0, string strValue1, int intValue0, int intValue1, int intValue2, int intValue3)
		{
			int num = 18;
			int num2 = CTSGlobals.AsciiEncoding.GetByteCount(strValue0.ValueOrEmpty()) + CTSGlobals.AsciiEncoding.GetByteCount(strValue1.ValueOrEmpty()) + num;
			TraceBuffer traceBuffer = TraceBuffer.Create(recordGuid, num2, useBufferPool);
			if (num2 > 8064 && !allowBufferSplit)
			{
				string[] reducedStringSizes = TraceRecord.GetReducedStringSizes(new Tuple<Encoding, string>[]
				{
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, strValue0)
				}, num, 8064);
				traceBuffer.WriteAsciiString(reducedStringSizes[0]);
				traceBuffer.WriteAsciiString(reducedStringSizes[1]);
			}
			else
			{
				traceBuffer.WriteAsciiString(strValue0);
				traceBuffer.WriteAsciiString(strValue1);
			}
			traceBuffer.WriteInt(intValue0);
			traceBuffer.WriteInt(intValue1);
			traceBuffer.WriteInt(intValue2);
			traceBuffer.WriteInt(intValue3);
			return traceBuffer;
		}

		public static TraceBuffer Create(Guid recordGuid, bool useBufferPool, bool allowBufferSplit, string strValue0, string strValue1, string strValue2, double doubleValue)
		{
			int num = 11;
			int num2 = CTSGlobals.AsciiEncoding.GetByteCount(strValue0.ValueOrEmpty()) + CTSGlobals.AsciiEncoding.GetByteCount(strValue1.ValueOrEmpty()) + CTSGlobals.AsciiEncoding.GetByteCount(strValue2.ValueOrEmpty()) + num;
			TraceBuffer traceBuffer = TraceBuffer.Create(recordGuid, num2, useBufferPool);
			if (num2 > 8064 && !allowBufferSplit)
			{
				string[] reducedStringSizes = TraceRecord.GetReducedStringSizes(new Tuple<Encoding, string>[]
				{
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, strValue0),
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, strValue1),
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, strValue2)
				}, num, 8064);
				traceBuffer.WriteAsciiString(reducedStringSizes[0]);
				traceBuffer.WriteAsciiString(reducedStringSizes[1]);
				traceBuffer.WriteAsciiString(reducedStringSizes[2]);
			}
			else
			{
				traceBuffer.WriteAsciiString(strValue0);
				traceBuffer.WriteAsciiString(strValue1);
				traceBuffer.WriteAsciiString(strValue2);
			}
			traceBuffer.WriteDouble(doubleValue);
			return traceBuffer;
		}

		public static TraceBuffer Create(Guid recordGuid, bool useBufferPool, bool allowBufferSplit, string strValue0, int intValue0, int intValue1, string strValue1)
		{
			int num = 10;
			int num2 = CTSGlobals.AsciiEncoding.GetByteCount(strValue0.ValueOrEmpty()) + CTSGlobals.AsciiEncoding.GetByteCount(strValue1.ValueOrEmpty()) + num;
			TraceBuffer traceBuffer = TraceBuffer.Create(recordGuid, num2, useBufferPool);
			if (num2 > 8064 && !allowBufferSplit)
			{
				string[] reducedStringSizes = TraceRecord.GetReducedStringSizes(new Tuple<Encoding, string>[]
				{
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, strValue0),
					new Tuple<Encoding, string>(CTSGlobals.AsciiEncoding, strValue1)
				}, num, 8064);
				traceBuffer.WriteAsciiString(reducedStringSizes[0]);
				traceBuffer.WriteInt(intValue0);
				traceBuffer.WriteInt(intValue1);
				traceBuffer.WriteAsciiString(reducedStringSizes[1]);
			}
			else
			{
				traceBuffer.WriteAsciiString(strValue0);
				traceBuffer.WriteInt(intValue0);
				traceBuffer.WriteInt(intValue1);
				traceBuffer.WriteAsciiString(strValue1);
			}
			return traceBuffer;
		}

		private static string[] GetReducedStringSizes(Tuple<Encoding, string>[] values, int minimum, int maximum)
		{
			if (values == null || values.Length == 0)
			{
				return new string[0];
			}
			if (minimum > maximum)
			{
				return (from tuple in values
				select tuple.Item2).ToArray<string>();
			}
			int maximumStringSize = TraceRecord.GetMaximumStringSize(values, minimum, maximum);
			string[] array = new string[values.Length];
			for (int i = 0; i < values.Length; i++)
			{
				int byteCount = values[i].Item1.GetByteCount(values[i].Item2.ValueOrEmpty());
				if (byteCount > maximumStringSize)
				{
					char[] array2 = new char[1];
					int num = 0;
					array[i] = string.Empty;
					for (int j = 0; j < values[i].Item2.Length; j++)
					{
						array2[0] = values[i].Item2[j];
						int byteCount2 = values[i].Item1.GetByteCount(array2);
						if (num + byteCount2 > maximumStringSize)
						{
							break;
						}
						string[] array3;
						IntPtr intPtr;
						(array3 = array)[(int)(intPtr = (IntPtr)i)] = array3[(int)intPtr] + values[i].Item2[j];
						num += byteCount2;
					}
				}
				else
				{
					array[i] = values[i].Item2;
				}
			}
			return array;
		}

		private static int GetMaximumStringSize(Tuple<Encoding, string>[] values, int minimum, int maximum)
		{
			if (values == null || values.Length == 0 || maximum < minimum)
			{
				return maximum;
			}
			IList<int> list = new List<int>(values.Length);
			int num = (maximum - minimum) / values.Length;
			int num2 = (maximum - minimum) % values.Length;
			for (int i = 0; i < values.Length; i++)
			{
				list.Add(values[i].Item1.GetByteCount(values[i].Item2.ValueOrEmpty()));
			}
			for (;;)
			{
				for (int j = 0; j < list.Count; j++)
				{
					if (list[j] <= num)
					{
						num2 += num - list[j];
						list.RemoveAt(j);
						j--;
					}
				}
				if (list.Count == 0)
				{
					break;
				}
				int num3 = num2 / list.Count;
				num2 %= list.Count;
				num += num3;
				if (num3 <= 0)
				{
					return num;
				}
			}
			return maximum;
		}

		private const int MaximumBufferSize = 8064;

		internal static class TestAccess
		{
			public static string[] GetReducedStringSizes(Tuple<Encoding, string>[] values, int minimum, int maximum)
			{
				return TraceRecord.GetReducedStringSizes(values, minimum, maximum);
			}

			public static int GetMaximumStringSize(Tuple<Encoding, string>[] values, int minimum, int maximum)
			{
				return TraceRecord.GetMaximumStringSize(values, minimum, maximum);
			}
		}
	}
}
