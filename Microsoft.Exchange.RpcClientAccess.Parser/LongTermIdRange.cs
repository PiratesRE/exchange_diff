using System;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal struct LongTermIdRange
	{
		public StoreLongTermId MinLongTermId
		{
			get
			{
				return this.minLongTermId;
			}
		}

		public StoreLongTermId MaxLongTermId
		{
			get
			{
				return this.maxLongTermId;
			}
		}

		public static implicit operator GuidGlobCountSet(LongTermIdRange longTermIdRange)
		{
			GuidGlobCount guidGlobCount = longTermIdRange.MinLongTermId;
			GuidGlobCount guidGlobCount2 = longTermIdRange.MaxLongTermId;
			GlobCountRange range = new GlobCountRange(guidGlobCount.GlobCount, guidGlobCount2.GlobCount);
			GlobCountSet globCountSet = new GlobCountSet();
			globCountSet.Insert(range);
			return new GuidGlobCountSet(guidGlobCount.Guid, globCountSet);
		}

		public override string ToString()
		{
			return string.Format("{0}: [{1}]-[{2}]", this.IsValid() ? "Valid" : "Invalid", this.minLongTermId, this.maxLongTermId);
		}

		public bool IsValid()
		{
			return this.MinLongTermId.Guid == this.MaxLongTermId.Guid && this.MinLongTermId <= this.MaxLongTermId && !ArrayComparer<byte>.Comparer.Equals(this.MinLongTermId.GlobCount, StoreLongTermId.Null.GlobCount);
		}

		public LongTermIdRange(StoreLongTermId minLongTermId, StoreLongTermId maxLongTermId)
		{
			this.minLongTermId = minLongTermId;
			this.maxLongTermId = maxLongTermId;
		}

		internal static LongTermIdRange Parse(Reader reader)
		{
			return new LongTermIdRange(StoreLongTermId.Parse(reader), StoreLongTermId.Parse(reader));
		}

		internal void Serialize(Writer writer)
		{
			this.minLongTermId.Serialize(writer);
			this.maxLongTermId.Serialize(writer);
		}

		internal const int Size = 48;

		private readonly StoreLongTermId minLongTermId;

		private readonly StoreLongTermId maxLongTermId;
	}
}
