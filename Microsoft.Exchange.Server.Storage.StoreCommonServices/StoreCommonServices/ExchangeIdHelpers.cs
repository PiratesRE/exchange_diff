using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public static class ExchangeIdHelpers
	{
		public static void From26ByteArray(byte[] buff, out Guid guid, out ushort replid, out ulong counter)
		{
			if (buff == null)
			{
				guid = Guid.Empty;
				replid = 0;
				counter = 0UL;
				return;
			}
			int a = ParseSerialize.ParseInt32(buff, 0);
			short b = ParseSerialize.ParseInt16(buff, 4);
			short c = ParseSerialize.ParseInt16(buff, 6);
			guid = new Guid(a, b, c, buff[8], buff[9], buff[10], buff[11], buff[12], buff[13], buff[14], buff[15]);
			counter = ExchangeIdHelpers.GlobcntFromByteArray(buff, 16U);
			replid = (ushort)ParseSerialize.ParseInt16(buff, 24);
		}

		public static void From8ByteArray(byte[] buff, out ushort replid, out ulong counter)
		{
			if (buff == null)
			{
				replid = 0;
				counter = 0UL;
				return;
			}
			ExchangeIdHelpers.FromLong(ParseSerialize.ParseInt64(buff, 0), out replid, out counter);
		}

		public static void From9ByteArray(byte[] buff, out ushort replid, out ulong counter)
		{
			if (buff == null)
			{
				replid = 0;
				counter = 0UL;
				return;
			}
			ExchangeIdHelpers.FromLong(ParseSerialize.ParseInt64(buff, 1), out replid, out counter);
		}

		public static void FromLong(long shortTermId, out ushort replid, out ulong counter)
		{
			replid = (ushort)shortTermId;
			counter = (ulong)((shortTermId & 16711680L) << 24 | (shortTermId & (long)((ulong)-16777216)) << 8 | (long)((ulong)(shortTermId & 1095216660480L) >> 8) | (long)((ulong)(shortTermId & 280375465082880L) >> 24) | (long)((ulong)(shortTermId & 71776119061217280L) >> 40) | (long)((ulong)(shortTermId & -72057594037927936L) >> 56));
		}

		public static ulong CounterFromLong(long shortTermId)
		{
			return (ulong)((shortTermId & 16711680L) << 24 | (shortTermId & (long)((ulong)-16777216)) << 8 | (long)((ulong)(shortTermId & 1095216660480L) >> 8) | (long)((ulong)(shortTermId & 280375465082880L) >> 24) | (long)((ulong)(shortTermId & 71776119061217280L) >> 40) | (long)((ulong)(shortTermId & -72057594037927936L) >> 56));
		}

		public static byte[] Convert26ByteTo22Byte(byte[] bytes)
		{
			byte[] array = new byte[22];
			Buffer.BlockCopy(bytes, 0, array, 0, 22);
			return array;
		}

		public static byte[] Convert26ByteTo24Byte(byte[] bytes)
		{
			byte[] array = new byte[24];
			Buffer.BlockCopy(bytes, 0, array, 0, 24);
			return array;
		}

		public static long Convert26ByteToLong(byte[] bytes)
		{
			ulong counter = ExchangeIdHelpers.GlobcntFromByteArray(bytes, 16U);
			ushort replid = (ushort)ParseSerialize.ParseInt16(bytes, 24);
			return ExchangeIdHelpers.ToLong(replid, counter);
		}

		public static long Convert9ByteToLong(byte[] bytes)
		{
			ushort replid = (ushort)ParseSerialize.ParseInt16(bytes, 1);
			ulong counter = ExchangeIdHelpers.GlobcntFromByteArray(bytes, 3U);
			return ExchangeIdHelpers.ToLong(replid, counter);
		}

		public static byte[] Convert26ByteTo9Byte(byte[] bytes)
		{
			ulong counter = ExchangeIdHelpers.GlobcntFromByteArray(bytes, 16U);
			ushort replid = (ushort)ParseSerialize.ParseInt16(bytes, 24);
			return ExchangeIdHelpers.To9ByteArray(replid, counter);
		}

		public static byte[] ConvertLongTo9Byte(long shortTermId)
		{
			ushort replid;
			ulong counter;
			ExchangeIdHelpers.FromLong(shortTermId, out replid, out counter);
			return ExchangeIdHelpers.To9ByteArray(replid, counter);
		}

		public static byte[] Convert26ByteToFolderSvrEid(byte[] fid)
		{
			Guid guid;
			ushort replid;
			ulong counter;
			ExchangeIdHelpers.From26ByteArray(fid, out guid, out replid, out counter);
			return ExchangeIdHelpers.BuildOursServerEntryId(ExchangeIdHelpers.ToLong(replid, counter), 0L, 0);
		}

		public static byte[] BuildOursServerEntryId(byte[] fid, byte[] mid, int instanceNum)
		{
			Guid guid;
			ushort replid;
			ulong counter;
			ExchangeIdHelpers.From26ByteArray(fid, out guid, out replid, out counter);
			ushort replid2;
			ulong counter2;
			ExchangeIdHelpers.From26ByteArray(mid, out guid, out replid2, out counter2);
			return ExchangeIdHelpers.BuildOursServerEntryId(ExchangeIdHelpers.ToLong(replid, counter), ExchangeIdHelpers.ToLong(replid2, counter2), instanceNum);
		}

		public static byte[] BuildOursServerEntryId(long fid, long mid, int instanceNum)
		{
			byte[] array = new byte[21];
			int num = 0;
			array[num++] = 1;
			num += ParseSerialize.SerializeInt64(fid, array, num);
			num += ParseSerialize.SerializeInt64(mid, array, num);
			ParseSerialize.SerializeInt32(instanceNum, array, num);
			return array;
		}

		public static bool ParseOursServerEntryId(byte[] entryId, out long fid, out long mid, out int instanceNum)
		{
			if (entryId == null || entryId.Length != 21 || entryId[0] != 1)
			{
				fid = 0L;
				mid = 0L;
				instanceNum = 0;
				DiagnosticContext.TraceLocation((LID)57320U);
				return false;
			}
			fid = ParseSerialize.ParseInt64(entryId, 1);
			mid = ParseSerialize.ParseInt64(entryId, 9);
			instanceNum = ParseSerialize.ParseInt32(entryId, 17);
			return true;
		}

		public static ulong GlobcntFromByteArray(byte[] src, uint offset)
		{
			return (ulong)src[(int)((UIntPtr)offset)] << 40 | (ulong)src[(int)((UIntPtr)(offset + 1U))] << 32 | (ulong)src[(int)((UIntPtr)(offset + 2U))] << 24 | (ulong)src[(int)((UIntPtr)(offset + 3U))] << 16 | (ulong)src[(int)((UIntPtr)(offset + 4U))] << 8 | (ulong)src[(int)((UIntPtr)(offset + 5U))];
		}

		public static int GlobcntIntoByteArray(ulong globCnt, byte[] dst, int offset)
		{
			dst[offset] = (byte)(globCnt >> 40);
			dst[offset + 1] = (byte)(globCnt >> 32);
			dst[offset + 2] = (byte)(globCnt >> 24);
			dst[offset + 3] = (byte)(globCnt >> 16);
			dst[offset + 4] = (byte)(globCnt >> 8);
			dst[offset + 5] = (byte)globCnt;
			return 6;
		}

		public static long ToLong(ushort replid, ulong counter)
		{
			return (long)((counter & 255UL) << 56 | (counter & 65280UL) << 40 | (counter & 16711680UL) << 24 | (counter & (ulong)-16777216) << 8 | (counter & 1095216660480UL) >> 8 | (counter & 280375465082880UL) >> 24 | (ulong)replid);
		}

		public static byte[] To8ByteArray(ushort replid, ulong counter)
		{
			byte[] array = new byte[8];
			ExchangeIdHelpers.To8ByteArray(replid, counter, array, 0);
			return array;
		}

		public static int To8ByteArray(ushort replid, ulong counter, byte[] buffer, int offset)
		{
			int num = offset;
			offset += ParseSerialize.SerializeInt16((short)replid, buffer, offset);
			offset += ExchangeIdHelpers.GlobcntIntoByteArray(counter, buffer, offset);
			return offset - num;
		}

		public static byte[] To9ByteArray(ushort replid, ulong counter)
		{
			byte[] array = new byte[9];
			ExchangeIdHelpers.To9ByteArray(replid, counter, array, 0);
			return array;
		}

		public static int To9ByteArray(ushort replid, ulong counter, byte[] buffer, int offset)
		{
			int num = offset;
			buffer[offset++] = ((replid == 1) ? 1 : 0);
			offset += ParseSerialize.SerializeInt16((short)replid, buffer, offset);
			offset += ExchangeIdHelpers.GlobcntIntoByteArray(counter, buffer, offset);
			return offset - num;
		}

		public static byte[] To22ByteArray(Guid guid, ulong counter)
		{
			byte[] array = new byte[22];
			ExchangeIdHelpers.To22ByteArray(guid, counter, array, 0);
			return array;
		}

		public static int To22ByteArray(Guid guid, ulong counter, byte[] buffer, int offset)
		{
			int num = offset;
			offset += ParseSerialize.SerializeGuid(guid, buffer, offset);
			offset += ExchangeIdHelpers.GlobcntIntoByteArray(counter, buffer, offset);
			return offset - num;
		}

		public static byte[] To24ByteArray(Guid guid, ulong counter)
		{
			byte[] array = new byte[24];
			ExchangeIdHelpers.To22ByteArray(guid, counter, array, 0);
			return array;
		}

		public static byte[] To26ByteArray(ushort replid, Guid guid, ulong counter)
		{
			byte[] array = new byte[26];
			ExchangeIdHelpers.To26ByteArray(replid, guid, counter, array, 0);
			return array;
		}

		public static int To26ByteArray(ushort replid, Guid guid, ulong counter, byte[] buffer, int offset)
		{
			int num = offset;
			offset += ParseSerialize.SerializeGuid(guid, buffer, offset);
			offset += ExchangeIdHelpers.GlobcntIntoByteArray(counter, buffer, offset);
			offset += 2;
			offset += ParseSerialize.SerializeInt16((short)replid, buffer, offset);
			return offset - num;
		}
	}
}
