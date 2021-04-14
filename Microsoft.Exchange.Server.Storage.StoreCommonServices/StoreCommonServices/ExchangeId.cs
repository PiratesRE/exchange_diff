using System;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public struct ExchangeId : IComparable<ExchangeId>, IEquatable<ExchangeId>
	{
		private ExchangeId(byte[] binaryValue)
		{
			this.binaryValue = binaryValue;
		}

		public static ExchangeId Create(Guid guid, ulong itemNbr, ushort replid)
		{
			if (itemNbr != 0UL || !(guid == Guid.Empty))
			{
				return new ExchangeId(ExchangeIdHelpers.To26ByteArray(replid, guid, itemNbr));
			}
			return ExchangeId.zeroId;
		}

		public static ExchangeId Create(Context context, IReplidGuidMap replidGuidMap, Guid guid, ulong itemNbr)
		{
			return ExchangeId.Create(guid, itemNbr, replidGuidMap.GetReplidFromGuid(context, guid));
		}

		public static ExchangeId Create(Context context, IReplidGuidMap replidGuidMap, ushort replid, byte[] globCnt)
		{
			Guid guidFromReplid = replidGuidMap.GetGuidFromReplid(context, replid);
			ulong itemNbr = ExchangeIdHelpers.GlobcntFromByteArray(globCnt, 0U);
			return ExchangeId.Create(guidFromReplid, itemNbr, replid);
		}

		public static ExchangeId CreateFromInt64(Context context, IReplidGuidMap replidGuidMap, long legacyId)
		{
			return ExchangeId.CreateFromInt64(legacyId, replidGuidMap.GetGuidFromReplid(context, (ushort)(legacyId & 65535L)), (ushort)(legacyId & 65535L));
		}

		public static ExchangeId CreateFromInternalShortId(Context context, IReplidGuidMap replidGuidMap, ExchangeShortId shortId)
		{
			if (shortId.IsZero)
			{
				return ExchangeId.Zero;
			}
			ushort replid = shortId.Replid;
			return ExchangeId.Create(replidGuidMap.InternalGetGuidFromReplid(context, replid), shortId.Counter, replid);
		}

		public static ExchangeId CreateFromInt64(long legacyId, Guid guid, ushort replid)
		{
			ulong itemNbr = (ulong)((legacyId & 16711680L) << 24 | (legacyId & (long)((ulong)-16777216)) << 8 | (long)((ulong)(legacyId & 1095216660480L) >> 8) | (long)((ulong)(legacyId & 280375465082880L) >> 24) | (long)((ulong)(legacyId & 71776119061217280L) >> 40) | (long)((ulong)(legacyId & -72057594037927936L) >> 56));
			return ExchangeId.Create(guid, itemNbr, replid);
		}

		public static ExchangeId CreateFrom26ByteArray(Context context, IReplidGuidMap replidGuidMap, byte[] bytes)
		{
			if (bytes == null)
			{
				return ExchangeId.nullId;
			}
			if (bytes[0] != 0 || !ExchangeId.EntryIdBytesEquals(bytes, ExchangeId.zeroIdBinaryValue))
			{
				return new ExchangeId(bytes);
			}
			return ExchangeId.zeroId;
		}

		public static ExchangeId CreateFrom26ByteArray(Context context, IReplidGuidMap replidGuidMap, byte[] bytes, int offset)
		{
			Guid guid = ParseSerialize.ParseGuid(bytes, offset);
			ulong itemNbr = ExchangeIdHelpers.GlobcntFromByteArray(bytes, (uint)(offset + 16));
			ushort replid = (ushort)ParseSerialize.ParseInt16(bytes, offset + 24);
			return ExchangeId.Create(guid, itemNbr, replid);
		}

		public static ExchangeId CreateFrom9ByteArray(Context context, IReplidGuidMap replidGuidMap, byte[] bytes)
		{
			return ExchangeId.CreateFrom8ByteArray(context, replidGuidMap, bytes, 1);
		}

		public static ExchangeId CreateFrom8ByteArray(Context context, IReplidGuidMap replidGuidMap, byte[] bytes)
		{
			return ExchangeId.CreateFrom8ByteArray(context, replidGuidMap, bytes, 0);
		}

		public static ExchangeId CreateFrom8ByteArray(Context context, IReplidGuidMap replidGuidMap, byte[] bytes, int offset)
		{
			ushort replid = (ushort)ParseSerialize.ParseInt16(bytes, offset);
			ulong itemNbr = ExchangeIdHelpers.GlobcntFromByteArray(bytes, (uint)(offset + 2));
			Guid guidFromReplid = replidGuidMap.GetGuidFromReplid(context, replid);
			return ExchangeId.Create(guidFromReplid, itemNbr, replid);
		}

		public static ExchangeId CreateFrom22ByteArray(Context context, IReplidGuidMap replidGuidMap, byte[] bytes)
		{
			return ExchangeId.CreateFrom22Or24ByteArray(context, replidGuidMap, bytes, 0);
		}

		public static ExchangeId CreateFrom22ByteArray(Context context, IReplidGuidMap replidGuidMap, byte[] bytes, int offset)
		{
			return ExchangeId.CreateFrom22Or24ByteArray(context, replidGuidMap, bytes, offset);
		}

		public static ExchangeId CreateFrom24ByteArray(Context context, IReplidGuidMap replidGuidMap, byte[] bytes)
		{
			return ExchangeId.CreateFrom22Or24ByteArray(context, replidGuidMap, bytes, 0);
		}

		public static ExchangeId CreateFrom24ByteArray(Context context, IReplidGuidMap replidGuidMap, byte[] bytes, int offset)
		{
			return ExchangeId.CreateFrom22Or24ByteArray(context, replidGuidMap, bytes, offset);
		}

		public static bool IsGlobCntValid(ulong globCnt)
		{
			return globCnt > 0UL && globCnt <= 281474976645120UL;
		}

		public static bool EntryIdBytesEquals(byte[] x, byte[] y)
		{
			if (object.ReferenceEquals(x, y))
			{
				return true;
			}
			if (x == null || y == null)
			{
				return false;
			}
			if (x.Length != y.Length)
			{
				return false;
			}
			if (x.Length != 0)
			{
				int num = x.Length - 1;
				while (0 <= num)
				{
					if (x[num] != y[num])
					{
						return false;
					}
					num--;
				}
			}
			return true;
		}

		private static ExchangeId CreateFrom22Or24ByteArray(Context context, IReplidGuidMap replidGuidMap, byte[] bytes, int offset)
		{
			Guid guid = ParseSerialize.ParseGuid(bytes, offset);
			ulong itemNbr = ExchangeIdHelpers.GlobcntFromByteArray(bytes, (uint)(offset + 16));
			ushort replidFromGuid = replidGuidMap.GetReplidFromGuid(context, guid);
			return ExchangeId.Create(guid, itemNbr, replidFromGuid);
		}

		public static ExchangeId Zero
		{
			get
			{
				return ExchangeId.zeroId;
			}
		}

		public static ExchangeId Null
		{
			get
			{
				return ExchangeId.nullId;
			}
		}

		public Guid Guid
		{
			get
			{
				if (!this.IsNullOrZero)
				{
					return ParseSerialize.ParseGuid(this.binaryValue, 0);
				}
				return Guid.Empty;
			}
		}

		public ushort Replid
		{
			get
			{
				return this.GetReplid();
			}
		}

		public bool IsReplidKnown
		{
			get
			{
				return this.GetReplid() != ushort.MaxValue;
			}
		}

		public byte[] Globcnt
		{
			get
			{
				byte[] array = new byte[6];
				ExchangeIdHelpers.GlobcntIntoByteArray(this.Counter, array, 0);
				return array;
			}
		}

		public ulong Counter
		{
			get
			{
				if (!this.IsNullOrZero)
				{
					return ExchangeIdHelpers.GlobcntFromByteArray(this.binaryValue, 16U);
				}
				return 0UL;
			}
		}

		public bool IsZero
		{
			get
			{
				return object.ReferenceEquals(this.binaryValue, ExchangeId.zeroIdBinaryValue);
			}
		}

		public bool IsNull
		{
			get
			{
				return this.binaryValue == null;
			}
		}

		public bool IsNullOrZero
		{
			get
			{
				return this.IsNull || this.IsZero;
			}
		}

		public bool IsValid
		{
			get
			{
				return !this.IsNullOrZero;
			}
		}

		public static bool operator ==(ExchangeId id1, ExchangeId id2)
		{
			return ExchangeId.EntryIdBytesEquals(id1.binaryValue, id2.binaryValue);
		}

		public static bool operator !=(ExchangeId id1, ExchangeId id2)
		{
			return !(id1 == id2);
		}

		[Obsolete]
		public static bool operator ==(ExchangeId id1, object id2)
		{
			throw new Exception();
		}

		[Obsolete]
		public static bool operator !=(ExchangeId id1, object id2)
		{
			throw new Exception();
		}

		[Obsolete]
		public static bool operator ==(object id1, ExchangeId id2)
		{
			throw new Exception();
		}

		[Obsolete]
		public static bool operator !=(object id1, ExchangeId id2)
		{
			throw new Exception();
		}

		public long ToLong()
		{
			return ExchangeIdHelpers.ToLong(this.Replid, this.Counter);
		}

		public ExchangeShortId ToExchangeShortId()
		{
			return ExchangeShortId.Create(this.Counter, this.Replid);
		}

		public byte[] To8ByteArray()
		{
			return ExchangeIdHelpers.To8ByteArray(this.Replid, this.Counter);
		}

		public byte[] To9ByteArray()
		{
			return ExchangeIdHelpers.To9ByteArray(this.Replid, this.Counter);
		}

		public byte[] To22ByteArray()
		{
			return ExchangeIdHelpers.To22ByteArray(this.Guid, this.Counter);
		}

		public byte[] To24ByteArray()
		{
			return ExchangeIdHelpers.To24ByteArray(this.Guid, this.Counter);
		}

		public byte[] To26ByteArray()
		{
			return this.binaryValue;
		}

		public ExchangeId ConvertNullToZero()
		{
			if (!this.IsNull)
			{
				return this;
			}
			return ExchangeId.zeroId;
		}

		public override int GetHashCode()
		{
			return (int)this.Counter;
		}

		public override bool Equals(object other)
		{
			return other is ExchangeId && this.Equals((ExchangeId)other);
		}

		public bool Equals(ExchangeId other)
		{
			return ExchangeId.EntryIdBytesEquals(this.binaryValue, other.binaryValue);
		}

		public int CompareTo(ExchangeId other)
		{
			ushort replid = this.GetReplid();
			if (replid != 0 && replid == other.GetReplid())
			{
				return this.Counter.CompareTo(other.Counter);
			}
			return ValueHelper.ArraysCompare<byte>(this.binaryValue, other.binaryValue);
		}

		public override string ToString()
		{
			ushort replid = this.GetReplid();
			return string.Format("{0}[{1}]-{2:X}", this.Guid, (replid == ushort.MaxValue) ? "?" : replid.ToString(), this.Counter);
		}

		private ushort GetReplid()
		{
			if (!this.IsNullOrZero)
			{
				return (ushort)ParseSerialize.ParseInt16(this.binaryValue, 24);
			}
			return 0;
		}

		public const ulong MaxGlobCntValue = 281474976645120UL;

		public const ushort UnknownReplid = 65535;

		private static readonly byte[] zeroIdBinaryValue = new byte[26];

		private static readonly ExchangeId zeroId = new ExchangeId(ExchangeId.zeroIdBinaryValue);

		private static readonly ExchangeId nullId = default(ExchangeId);

		private byte[] binaryValue;
	}
}
