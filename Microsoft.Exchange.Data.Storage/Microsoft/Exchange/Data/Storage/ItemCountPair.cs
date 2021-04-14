using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal struct ItemCountPair
	{
		public ItemCountPair(long itemCount, long unreadItemCount)
		{
			this.ItemCount = itemCount;
			this.UnreadItemCount = unreadItemCount;
		}

		public readonly long ItemCount;

		public readonly long UnreadItemCount;
	}
}
