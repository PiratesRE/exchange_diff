using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class NoSyncFilter : QueryBasedSyncFilter
	{
		public NoSyncFilter() : base(null, null)
		{
		}

		public override string Id
		{
			get
			{
				return "NoSyncFilter";
			}
		}

		public override bool IsItemInFilter(ISyncItemId id)
		{
			return true;
		}

		public override void UpdateFilterState(SyncOperation syncOperation)
		{
			ServerManifestEntry value = new ServerManifestEntry(syncOperation.Id);
			base.EntriesInFilter[syncOperation.Id] = value;
		}
	}
}
