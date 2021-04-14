using System;
using Microsoft.Exchange.Collections;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal struct StoreLongTermId : IEquatable<StoreLongTermId>, IComparable<StoreLongTermId>
	{
		public StoreLongTermId(Guid guid, byte[] globCount)
		{
			Util.ThrowOnNullArgument(globCount, "globCount");
			if (globCount.Length != 6)
			{
				throw new ArgumentException(string.Format("GlobCount needs to be {0} bytes", 6));
			}
			this.guid = guid;
			this.globCount = globCount;
		}

		public Guid Guid
		{
			get
			{
				return this.guid;
			}
		}

		public byte[] GlobCount
		{
			get
			{
				return this.globCount;
			}
		}

		internal static int ArraySize
		{
			get
			{
				return 22;
			}
		}

		internal static int Size
		{
			get
			{
				return 24;
			}
		}

		public static bool operator <=(StoreLongTermId storeLongTermId1, StoreLongTermId storeLongTermId2)
		{
			return storeLongTermId1.CompareTo(storeLongTermId2) <= 0;
		}

		public static bool operator >=(StoreLongTermId storeLongTermId1, StoreLongTermId storeLongTermId2)
		{
			return storeLongTermId1.CompareTo(storeLongTermId2) >= 0;
		}

		public static implicit operator GuidGlobCount(StoreLongTermId storeLongTermId)
		{
			ulong num = 0UL;
			for (int i = 0; i < 6; i++)
			{
				num += (ulong)storeLongTermId.GlobCount[i] << 8 * (5 - i);
			}
			return new GuidGlobCount(storeLongTermId.Guid, num);
		}

		public override bool Equals(object obj)
		{
			return obj is StoreLongTermId && this.Equals((StoreLongTermId)obj);
		}

		public override int GetHashCode()
		{
			return this.guid.GetHashCode() ^ ArrayComparer<byte>.Comparer.GetHashCode(this.globCount);
		}

		internal static StoreLongTermId Parse(Reader reader)
		{
			return StoreLongTermId.Parse(reader, true);
		}

		internal static StoreLongTermId Parse(Reader reader, bool includePadding)
		{
			Guid guid = reader.ReadGuid();
			byte[] array = reader.ReadBytes(6U);
			if (includePadding)
			{
				reader.ReadArraySegment(2U);
			}
			return new StoreLongTermId(guid, array);
		}

		internal static StoreLongTermId Parse(byte[] rawId)
		{
			return StoreLongTermId.Parse(rawId, false);
		}

		internal static StoreLongTermId Parse(byte[] rawId, bool includePadding)
		{
			Util.ThrowOnNullArgument(rawId, "rawId");
			int num = includePadding ? StoreLongTermId.Size : StoreLongTermId.ArraySize;
			if (rawId.Length != num)
			{
				throw new BufferParseException("The buffer representing StoreLongTermId is invalid.");
			}
			StoreLongTermId result;
			using (Reader reader = Reader.CreateBufferReader(rawId))
			{
				result = StoreLongTermId.Parse(reader, includePadding);
			}
			return result;
		}

		internal void Serialize(Writer writer)
		{
			this.Serialize(writer, true);
		}

		internal void Serialize(Writer writer, bool includePadding)
		{
			writer.WriteGuid(this.guid);
			writer.WriteBytes(this.globCount);
			if (includePadding)
			{
				writer.WriteUInt16(0);
			}
		}

		internal byte[] ToBytes()
		{
			return this.ToBytes(false);
		}

		internal byte[] ToBytes(bool includePadding)
		{
			int num = includePadding ? StoreLongTermId.Size : StoreLongTermId.ArraySize;
			byte[] array = new byte[num];
			using (Writer writer = new BufferWriter(array))
			{
				writer.WriteGuid(this.guid);
				writer.WriteBytes(this.globCount);
			}
			return array;
		}

		public bool Equals(StoreLongTermId other)
		{
			return this.guid == other.guid && ArrayComparer<byte>.Comparer.Equals(this.globCount, other.globCount);
		}

		public int CompareTo(StoreLongTermId other)
		{
			int num = this.Guid.CompareTo(other.Guid);
			if (num == 0)
			{
				num = ArrayComparer<byte>.Comparer.Compare(this.GlobCount, other.GlobCount);
			}
			return num;
		}

		public override string ToString()
		{
			if (this.GlobCount == null)
			{
				return string.Format("{0} - null", this.Guid);
			}
			return string.Format("{0} - {1:X2}{2:X2}{3:X2}{4:X2}{5:X2}{6:X2}", new object[]
			{
				this.Guid,
				this.GlobCount[0],
				this.GlobCount[1],
				this.GlobCount[2],
				this.GlobCount[3],
				this.GlobCount[4],
				this.GlobCount[5]
			});
		}

		private const int GuidSize = 16;

		private const int GlobCountLength = 6;

		private const int PaddingSize = 2;

		public const int SizeWithoutPadding = 22;

		public const int SizeWithPadding = 24;

		private readonly Guid guid;

		private readonly byte[] globCount;

		public static readonly StoreLongTermId Null = new StoreLongTermId(Guid.Empty, new byte[6]);
	}
}
