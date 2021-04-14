using System;
using System.Collections;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public class IdSet : IEnumerable, IEquatable<IdSet>
	{
		public IdSet()
		{
			this.wrappee = new IdSet();
		}

		internal IdSet(IdSet idSet)
		{
			this.wrappee = idSet;
		}

		public ulong CountIds
		{
			get
			{
				return this.wrappee.CountIds;
			}
		}

		public int CountGuids
		{
			get
			{
				return this.wrappee.CountGuids;
			}
		}

		public int CountRanges
		{
			get
			{
				return this.wrappee.CountRanges;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.wrappee.IsEmpty;
			}
		}

		public bool IsDirty
		{
			get
			{
				return this.wrappee.IsDirty;
			}
			set
			{
				this.wrappee.IsDirty = value;
			}
		}

		public static IdSet Parse(Context context, byte[] idsetBytes)
		{
			return new IdSet(IdSetUtilities.IdSetFromBytes(context, idsetBytes));
		}

		public static IdSet ThrowableParse(Context context, byte[] idsetBytes)
		{
			return new IdSet(IdSetUtilities.ThrowableIdSetFromBytes(context, idsetBytes));
		}

		public static IdSet ThrowableParse(Context context, Stream readStream)
		{
			return new IdSet(IdSetUtilities.ThrowableIdSetFromStream(context, readStream));
		}

		internal static IdSet ThrowableParse(Reader reader)
		{
			return new IdSet(IdSet.ParseWithReplGuids(reader));
		}

		public static IdSet Union(IdSet first, IdSet second)
		{
			return new IdSet(IdSet.Union(first.wrappee, second.wrappee));
		}

		public static IdSet Subtract(IdSet first, IdSet second)
		{
			return new IdSet(IdSet.Subtract(first.wrappee, second.wrappee));
		}

		public static IdSet Intersect(IdSet first, IdSet second)
		{
			return new IdSet(IdSet.Intersect(first.wrappee, second.wrappee));
		}

		public bool Insert(ExchangeId id)
		{
			return this.Insert(id.Guid, id.Counter);
		}

		public bool Insert(byte[] twentySixByteArray)
		{
			Guid guid;
			ushort num;
			ulong counter;
			ExchangeIdHelpers.From26ByteArray(twentySixByteArray, out guid, out num, out counter);
			return this.Insert(guid, counter);
		}

		public bool Insert(Guid guid, ulong counter)
		{
			return counter != 0UL && this.wrappee.Insert(guid, counter);
		}

		internal bool Insert(LongTermIdRange idRange)
		{
			if (!idRange.IsValid())
			{
				throw new StoreException((LID)43129U, ErrorCodeValue.InvalidParameter);
			}
			return this.wrappee.Insert(idRange);
		}

		public bool Insert(IdSet other)
		{
			return this.wrappee.Insert(other.wrappee);
		}

		public bool Remove(ExchangeId id)
		{
			return this.Remove(id.Guid, id.Counter);
		}

		public bool Remove(byte[] twentySixByteArray)
		{
			Guid guid;
			ushort num;
			ulong counter;
			ExchangeIdHelpers.From26ByteArray(twentySixByteArray, out guid, out num, out counter);
			return this.Remove(guid, counter);
		}

		public bool Remove(Guid guid, ulong counter)
		{
			return counter != 0UL && this.wrappee.Remove(new GuidGlobCount(guid, counter));
		}

		public bool Remove(IdSet other)
		{
			return this.wrappee.Remove(other.wrappee);
		}

		public bool Contains(ExchangeId id)
		{
			return this.Contains(id.Guid, id.Counter);
		}

		public bool Contains(byte[] twentySixByteArray)
		{
			Guid guid;
			ushort num;
			ulong counter;
			ExchangeIdHelpers.From26ByteArray(twentySixByteArray, out guid, out num, out counter);
			return this.Contains(guid, counter);
		}

		public bool Contains(Guid guid, ulong counter)
		{
			return counter != 0UL && this.wrappee.Contains(new GuidGlobCount(guid, counter));
		}

		public bool Equals(IdSet other)
		{
			return other != null && this.wrappee.Equals(other.wrappee);
		}

		public override bool Equals(object other)
		{
			return this.Equals(other as IdSet);
		}

		public override int GetHashCode()
		{
			return this.wrappee.GetHashCode();
		}

		public void IdealPack()
		{
			this.wrappee.IdealPack();
		}

		public byte[] Serialize()
		{
			return IdSetUtilities.BytesFromIdSet(this.wrappee);
		}

		internal byte[] Serialize(Func<Guid, ReplId> replIdFromGuidMapper)
		{
			return this.wrappee.SerializeWithReplIds(replIdFromGuidMapper);
		}

		internal void Serialize(Writer writer)
		{
			this.wrappee.SerializeWithReplGuids(writer);
		}

		public override string ToString()
		{
			return this.wrappee.ToString();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.wrappee.GetEnumerator();
		}

		public IdSet Clone()
		{
			return new IdSet(this.wrappee.Clone());
		}

		private IdSet wrappee;
	}
}
