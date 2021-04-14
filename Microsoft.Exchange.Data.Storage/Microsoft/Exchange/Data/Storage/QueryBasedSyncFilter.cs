using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class QueryBasedSyncFilter : ISyncFilter
	{
		public QueryBasedSyncFilter(QueryFilter filter, string stringId)
		{
			this.filter = filter;
			this.stringId = stringId;
			this.entriesInFilter = new Dictionary<ISyncItemId, ServerManifestEntry>();
		}

		public Dictionary<ISyncItemId, ServerManifestEntry> EntriesInFilter
		{
			get
			{
				return this.entriesInFilter;
			}
		}

		public QueryFilter FilterQuery
		{
			get
			{
				return this.filter;
			}
		}

		public virtual string Id
		{
			get
			{
				if (this.stringId == null)
				{
					this.stringId = "QueryBasedSyncFilter: " + this.filter.ToString();
				}
				return this.stringId;
			}
		}

		public void InitializeAllItemsInFilter(ISyncProvider syncProvider)
		{
			this.entriesInFilter.Clear();
			syncProvider.GetNewOperations(syncProvider.CreateNewWatermark(), null, false, -1, this.filter, this.entriesInFilter);
		}

		public virtual bool IsItemInFilter(ISyncItemId id)
		{
			return this.entriesInFilter.ContainsKey(id);
		}

		public virtual void UpdateFilterState(SyncOperation syncOperation)
		{
			if (syncOperation.ChangeType == ChangeType.Delete)
			{
				this.entriesInFilter.Remove(syncOperation.Id);
				return;
			}
			try
			{
				try
				{
					bool flag = EvaluatableFilter.Evaluate(this.filter, syncOperation, true);
					if (flag)
					{
						ServerManifestEntry serverManifestEntry = new ServerManifestEntry(syncOperation.Id);
						serverManifestEntry.UpdateManifestFromPropertyBag(syncOperation);
						serverManifestEntry.FirstMessageInConversation = syncOperation.FirstMessageInConversation;
						this.entriesInFilter[syncOperation.Id] = serverManifestEntry;
					}
				}
				catch (PropertyErrorException)
				{
					ISyncItem item = syncOperation.GetItem(MailboxSyncProvider.QueryColumns);
					bool flag = item.IsItemInFilter(this.filter);
					if (flag)
					{
						ServerManifestEntry serverManifestEntry2 = new ServerManifestEntry(syncOperation.Id);
						serverManifestEntry2.UpdateManifestFromItem(item);
						this.entriesInFilter[syncOperation.Id] = serverManifestEntry2;
					}
				}
			}
			catch (ObjectNotFoundException)
			{
			}
		}

		private Dictionary<ISyncItemId, ServerManifestEntry> entriesInFilter;

		private QueryFilter filter;

		private string stringId;
	}
}
