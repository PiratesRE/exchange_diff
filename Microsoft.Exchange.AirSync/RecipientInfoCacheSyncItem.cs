using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync
{
	internal class RecipientInfoCacheSyncItem : ISyncItem, IDisposeTrackable, IDisposable
	{
		private RecipientInfoCacheSyncItem(RecipientInfoCacheEntry item)
		{
			this.disposeTracker = this.GetDisposeTracker();
			this.item = item;
		}

		public ISyncItemId Id
		{
			get
			{
				this.CheckDisposed("get_Id");
				if (this.id == null)
				{
					this.id = new RecipientInfoCacheSyncItemId(this.item.CacheEntryId);
				}
				return this.id;
			}
		}

		public object NativeItem
		{
			get
			{
				this.CheckDisposed("get_NativeItem");
				return this.item;
			}
		}

		public ISyncWatermark Watermark
		{
			get
			{
				this.CheckDisposed("get_Watermark");
				if (this.watermark == null)
				{
					Dictionary<RecipientInfoCacheSyncItemId, long> dictionary = new Dictionary<RecipientInfoCacheSyncItemId, long>(1);
					using (RecipientInfoCacheSyncItem recipientInfoCacheSyncItem = RecipientInfoCacheSyncItem.Bind(this.item))
					{
						dictionary[(RecipientInfoCacheSyncItemId)recipientInfoCacheSyncItem.Id] = this.item.DateTimeTicks;
						this.watermark = RecipientInfoCacheSyncWatermark.Create(dictionary, (ExDateTime)new DateTime(this.item.DateTimeTicks));
					}
				}
				return this.watermark;
			}
		}

		public static RecipientInfoCacheSyncItem Bind(RecipientInfoCacheEntry item)
		{
			return new RecipientInfoCacheSyncItem(item);
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<RecipientInfoCacheSyncItem>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				this.disposed = true;
				this.InternalDispose(disposing);
			}
		}

		public bool IsItemInFilter(QueryFilter filter)
		{
			this.CheckDisposed("IsItemInFilter");
			return true;
		}

		public void Load()
		{
			this.CheckDisposed("Load");
		}

		public void Save()
		{
			this.CheckDisposed("Save");
		}

		protected void InternalDispose(bool disposing)
		{
			if (disposing && this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
			}
			this.item = null;
		}

		private void CheckDisposed(string methodName)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
		}

		private readonly DisposeTracker disposeTracker;

		private RecipientInfoCacheSyncItemId id;

		private RecipientInfoCacheEntry item;

		private bool disposed;

		private RecipientInfoCacheSyncWatermark watermark;
	}
}
