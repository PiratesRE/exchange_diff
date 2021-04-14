using System;

namespace Microsoft.Exchange.InfoWorker.Common.Sharing
{
	internal sealed class Changes
	{
		public Changes(string syncState, bool moreChangesAvailable, ItemChange[] items)
		{
			this.SyncState = syncState;
			this.MoreChangesAvailable = moreChangesAvailable;
			this.Items = items;
		}

		public string SyncState { get; private set; }

		public bool MoreChangesAvailable { get; private set; }

		public ItemChange[] Items { get; private set; }
	}
}
