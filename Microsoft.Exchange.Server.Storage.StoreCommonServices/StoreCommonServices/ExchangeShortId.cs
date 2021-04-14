using System;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public struct ExchangeShortId : IComparable<ExchangeShortId>, IEquatable<ExchangeShortId>
	{
		private ExchangeShortId(long value)
		{
			this.value = (ulong)value;
		}

		public static ExchangeShortId Create(ulong itemNbr, ushort replid)
		{
			return new ExchangeShortId(ExchangeIdHelpers.ToLong(replid, itemNbr));
		}

		public static ExchangeShortId Zero
		{
			get
			{
				return new ExchangeShortId(0L);
			}
		}

		public ushort Replid
		{
			get
			{
				ushort result;
				ulong num;
				ExchangeIdHelpers.FromLong((long)this.value, out result, out num);
				return result;
			}
		}

		public ulong Counter
		{
			get
			{
				ushort num;
				ulong result;
				ExchangeIdHelpers.FromLong((long)this.value, out num, out result);
				return result;
			}
		}

		public bool IsZero
		{
			get
			{
				return this.value == 0UL;
			}
		}

		public static bool operator ==(ExchangeShortId id1, ExchangeShortId id2)
		{
			return id1.value == id2.value;
		}

		public static bool operator !=(ExchangeShortId id1, ExchangeShortId id2)
		{
			return !(id1 == id2);
		}

		[Obsolete]
		public static bool operator ==(ExchangeShortId id1, object id2)
		{
			throw new Exception();
		}

		[Obsolete]
		public static bool operator !=(ExchangeShortId id1, object id2)
		{
			throw new Exception();
		}

		[Obsolete]
		public static bool operator ==(object id1, ExchangeShortId id2)
		{
			throw new Exception();
		}

		[Obsolete]
		public static bool operator !=(object id1, ExchangeShortId id2)
		{
			throw new Exception();
		}

		public override int GetHashCode()
		{
			return (int)this.Counter;
		}

		public override bool Equals(object other)
		{
			return other is ExchangeShortId && this.Equals((ExchangeShortId)other);
		}

		public bool Equals(ExchangeShortId other)
		{
			return this.value == other.value;
		}

		public int CompareTo(ExchangeShortId other)
		{
			return this.value.CompareTo(other.value);
		}

		public override string ToString()
		{
			return string.Format("[{0}]-{1:X}", this.Replid, this.Counter);
		}

		private ulong value;
	}
}
