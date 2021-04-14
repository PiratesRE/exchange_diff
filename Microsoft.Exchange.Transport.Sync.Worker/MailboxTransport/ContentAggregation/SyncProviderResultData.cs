using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class SyncProviderResultData
	{
		internal static SyncProviderResultData CreateAcknowledgeChangesResult(IList<SyncChangeEntry> changeList, bool hasPermanentSyncErrors, bool hasTransientSyncErrors, int cloudItemsSynced, bool moreItemsAvailable)
		{
			return new SyncProviderResultData(changeList, hasPermanentSyncErrors, hasTransientSyncErrors, cloudItemsSynced, moreItemsAvailable, false);
		}

		internal SyncProviderResultData(IList<SyncChangeEntry> changeList, bool hasPermanentSyncErrors, bool hasTransientSyncErrors) : this(changeList, hasPermanentSyncErrors, hasTransientSyncErrors, 0, false, false)
		{
		}

		internal SyncProviderResultData(IList<SyncChangeEntry> changeList, bool hasPermanentSyncErrors, bool hasTransientSyncErrors, bool hasNoChangesOnCloud) : this(changeList, hasPermanentSyncErrors, hasTransientSyncErrors, 0, false, hasNoChangesOnCloud)
		{
		}

		internal SyncProviderResultData(IList<SyncChangeEntry> changeList, bool hasPermanentSyncErrors, bool hasTransientSyncErrors, int cloudItemsSynced, bool moreItemsAvailable) : this(changeList, hasPermanentSyncErrors, hasTransientSyncErrors, cloudItemsSynced, moreItemsAvailable, false)
		{
		}

		internal SyncProviderResultData(IList<SyncChangeEntry> changeList, bool hasPermanentSyncErrors, bool hasTransientSyncErrors, bool moreItemsAvailable, bool hasNoChangesOnCloud) : this(changeList, hasPermanentSyncErrors, hasTransientSyncErrors, 0, moreItemsAvailable, hasNoChangesOnCloud)
		{
		}

		internal SyncProviderResultData(IList<SyncChangeEntry> changeList, bool hasPermanentSyncErrors, bool hasTransientSyncErrors, int cloudItemsSynced, bool moreItemsAvailable, bool hasNoChangesOnCloud)
		{
			this.changeList = changeList;
			this.hasPermanentSyncErrors = hasPermanentSyncErrors;
			this.hasTransientSyncErrors = hasTransientSyncErrors;
			this.cloudItemsSynced = cloudItemsSynced;
			this.moreItemsAvailable = moreItemsAvailable;
			this.hasNoChangesOnCloud = hasNoChangesOnCloud;
		}

		internal IList<SyncChangeEntry> ChangeList
		{
			get
			{
				return this.changeList;
			}
		}

		internal bool HasPermanentSyncErrors
		{
			get
			{
				return this.hasPermanentSyncErrors;
			}
			set
			{
				this.hasPermanentSyncErrors = value;
			}
		}

		internal bool HasTransientSyncErrors
		{
			get
			{
				return this.hasTransientSyncErrors;
			}
			set
			{
				this.hasTransientSyncErrors = value;
			}
		}

		internal int CloudItemsSynced
		{
			get
			{
				return this.cloudItemsSynced;
			}
		}

		internal bool MoreItemsAvailable
		{
			get
			{
				return this.moreItemsAvailable;
			}
		}

		internal bool HasNoChangesOnCloud
		{
			get
			{
				return this.hasNoChangesOnCloud;
			}
		}

		private readonly IList<SyncChangeEntry> changeList;

		private readonly int cloudItemsSynced;

		private readonly bool moreItemsAvailable;

		private readonly bool hasNoChangesOnCloud;

		private bool hasTransientSyncErrors;

		private bool hasPermanentSyncErrors;
	}
}
