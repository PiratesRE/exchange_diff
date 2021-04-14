using System;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal struct StoreId : IEquatable<StoreId>
	{
		public StoreId(long nativeId)
		{
			this = new StoreId((ulong)nativeId);
		}

		public StoreId(ulong nativeId)
		{
			this.nativeId = nativeId;
		}

		internal StoreId(ReplId replId, byte[] globCnt)
		{
			if (globCnt == null || globCnt.Length != 6)
			{
				throw new ArgumentException("Should be exactly 6 bytes", "globCnt");
			}
			byte[] array = new byte[8];
			using (BufferWriter bufferWriter = new BufferWriter(array))
			{
				bufferWriter.WriteUInt16(replId.Value);
				bufferWriter.WriteBytes(globCnt);
			}
			this.nativeId = BitConverter.ToUInt64(array, 0);
		}

		public ReplId ReplId
		{
			get
			{
				return new ReplId((ushort)(this.nativeId & 65535UL));
			}
		}

		public ulong GlobCount
		{
			get
			{
				return ((this.nativeId & 16711680UL) << 24) + ((this.nativeId & (ulong)-16777216) << 8) + ((this.nativeId & 1095216660480UL) >> 8) + ((this.nativeId & 280375465082880UL) >> 24) + ((this.nativeId & 71776119061217280UL) >> 40) + ((this.nativeId & 18374686479671623680UL) >> 56);
			}
		}

		public static implicit operator long(StoreId storeId)
		{
			return (long)storeId.nativeId;
		}

		public static implicit operator ulong(StoreId storeId)
		{
			return storeId.nativeId;
		}

		public override bool Equals(object obj)
		{
			return obj is StoreId && this.Equals((StoreId)obj);
		}

		public override int GetHashCode()
		{
			return this.nativeId.GetHashCode();
		}

		public bool Equals(StoreId other)
		{
			return this.nativeId == other.nativeId;
		}

		public override string ToString()
		{
			return string.Format("{0:B}-{1:X}", this.ReplId, this.GlobCount);
		}

		internal static StoreId Parse(Reader reader)
		{
			ulong num = reader.ReadUInt64();
			return new StoreId(num);
		}

		internal void Serialize(Writer writer)
		{
			writer.WriteUInt64(this.nativeId);
		}

		private readonly ulong nativeId;

		public static readonly StoreId Empty = new StoreId(0L);
	}
}
