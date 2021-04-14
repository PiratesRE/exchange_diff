using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.MailboxReplicationService
{
	public class EntryIdMap<TValue> : Dictionary<byte[], TValue>
	{
		public EntryIdMap() : base(ArrayComparer<byte>.EqualityComparer)
		{
		}

		public EntryIdMap(int capacity) : base(capacity, ArrayComparer<byte>.EqualityComparer)
		{
		}

		public byte[][] EntryIds
		{
			get
			{
				List<byte[]> list = new List<byte[]>(base.Count);
				list.AddRange(base.Keys);
				return list.ToArray();
			}
		}
	}
}
