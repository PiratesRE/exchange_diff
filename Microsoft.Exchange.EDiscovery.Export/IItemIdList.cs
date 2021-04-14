using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal interface IItemIdList
	{
		string SourceId { get; }

		IList<ItemId> MemoryCache { get; }

		bool Exists { get; }

		bool IsUnsearchable { get; }

		void WriteItemId(ItemId itemId);

		void Flush();

		IEnumerable<ItemId> ReadItemIds();
	}
}
