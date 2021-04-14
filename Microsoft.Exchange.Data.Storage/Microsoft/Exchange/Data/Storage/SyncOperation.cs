using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SyncOperation : IReadOnlyPropertyBag
	{
		internal SyncOperation()
		{
		}

		public int?[] ChangeTrackingInformation
		{
			get
			{
				if (this.manifestEntry.IsDelayedServerOperation)
				{
					return this.WholeItemChangedChangeTrackingInformation;
				}
				return this.manifestEntry.ChangeTrackingInformation;
			}
			set
			{
				this.manifestEntry.ChangeTrackingInformation = value;
			}
		}

		public ChangeType ChangeType
		{
			get
			{
				return this.manifestEntry.ChangeType;
			}
		}

		public bool IsRead
		{
			get
			{
				return this.manifestEntry.IsRead;
			}
		}

		public ISyncItemId Id
		{
			get
			{
				return this.manifestEntry.Id;
			}
		}

		public string MessageClass
		{
			get
			{
				return this.manifestEntry.MessageClass;
			}
		}

		public ConversationId ConversationId
		{
			get
			{
				return this.manifestEntry.ConversationId;
			}
		}

		public bool FirstMessageInConversation
		{
			get
			{
				return this.manifestEntry.FirstMessageInConversation;
			}
		}

		public object this[PropertyDefinition propertyDefinition]
		{
			get
			{
				return this.manifestEntry[propertyDefinition];
			}
		}

		private int?[] WholeItemChangedChangeTrackingInformation
		{
			get
			{
				if (this.wholeItemChangedChangeTrackingInformation == null && this.manifestEntry.ChangeTrackingInformation != null)
				{
					this.wholeItemChangedChangeTrackingInformation = new int?[this.manifestEntry.ChangeTrackingInformation.Length];
					for (int i = 0; i < this.manifestEntry.ChangeTrackingInformation.Length; i++)
					{
						this.wholeItemChangedChangeTrackingInformation[i] = (this.manifestEntry.ChangeTrackingInformation[i] ^ 1);
					}
				}
				return this.wholeItemChangedChangeTrackingInformation;
			}
		}

		public void EnsureServerManifestWatermark()
		{
			if (this.manifestEntry.Watermark == null && this.manifestEntry.ChangeType != ChangeType.Delete)
			{
				ISyncItem syncItem = this.item;
				if (syncItem == null)
				{
					syncItem = this.folderSync.GetItem(this.manifestEntry.Id, new PropertyDefinition[0]);
				}
				this.manifestEntry.Watermark = syncItem.Watermark;
				if (this.cacheItem)
				{
					this.item = syncItem;
				}
			}
		}

		public ISyncItem GetItem(params PropertyDefinition[] prefetchProperties)
		{
			if (this.rejected)
			{
				throw new InvalidOperationException(ServerStrings.ExCannotOpenRejectedItem);
			}
			if (this.item != null)
			{
				return this.item;
			}
			ISyncItem result = this.folderSync.GetItem(this.manifestEntry, prefetchProperties);
			if (this.cacheItem)
			{
				this.item = result;
			}
			return result;
		}

		public void Reject()
		{
			if (this.rejected)
			{
				throw new InvalidOperationException(ServerStrings.ExCannotRejectSameOperationTwice);
			}
			this.rejected = true;
			this.folderSync.RejectServerOperation(this.manifestEntry);
		}

		public object[] GetProperties(ICollection<PropertyDefinition> propertyDefinitions)
		{
			return this.manifestEntry.GetProperties(propertyDefinitions);
		}

		internal void Bind(ISyncItem item, ServerManifestEntry manifestEntry)
		{
			this.cacheItem = true;
			this.manifestEntry = manifestEntry;
			this.item = item;
			this.folderSync = null;
		}

		internal void Bind(FolderSync folderSync, ServerManifestEntry manifestEntry, bool cacheItem)
		{
			this.cacheItem = cacheItem;
			this.manifestEntry = manifestEntry;
			this.item = null;
			this.folderSync = folderSync;
		}

		internal void DisposeCachedItem()
		{
			if (this.item != null)
			{
				this.item.Dispose();
				this.item = null;
			}
		}

		private bool cacheItem;

		private FolderSync folderSync;

		private ISyncItem item;

		private ServerManifestEntry manifestEntry;

		private bool rejected;

		private int?[] wholeItemChangedChangeTrackingInformation;
	}
}
