using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MailboxSyncItem : ISyncItem, IDisposeTrackable, IDisposable
	{
		protected MailboxSyncItem(Item item)
		{
			StorageGlobals.TraceConstructIDisposable(this);
			this.disposeTracker = this.GetDisposeTracker();
			this.item = item;
		}

		public virtual ISyncItemId Id
		{
			get
			{
				this.CheckDisposed("get_Id");
				if (this.id == null)
				{
					this.id = MailboxSyncItemId.CreateForNewItem(this.item.Id.ObjectId);
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
			protected set
			{
				this.CheckDisposed("set_NativeItem");
				if (!object.ReferenceEquals(this.item, value))
				{
					if (this.item != null)
					{
						this.item.Dispose();
					}
					this.item = (Item)value;
				}
			}
		}

		public ISyncWatermark Watermark
		{
			get
			{
				this.CheckDisposed("get_Watermark");
				if (this.watermark == null)
				{
					this.watermark = MailboxSyncWatermark.CreateForSingleItem();
				}
				this.watermark.UpdateWithChangeNumber((int)this.item.TryGetProperty(InternalSchema.ArticleId), (bool)this.item.TryGetProperty(InternalSchema.IsRead));
				return this.watermark;
			}
		}

		public static MailboxSyncItem Bind(Item item)
		{
			return new MailboxSyncItem(item);
		}

		public virtual DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<MailboxSyncItem>(this);
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
			StorageGlobals.TraceDispose(this, this.disposed, disposing);
			if (!this.disposed)
			{
				this.disposed = true;
				this.InternalDispose(disposing);
			}
		}

		public bool IsItemInFilter(QueryFilter filter)
		{
			this.CheckDisposed("IsItemInFilter");
			return EvaluatableFilter.Evaluate(filter, this.item);
		}

		public void Load()
		{
			this.CheckDisposed("Load");
			this.item.Load(new PropertyDefinition[]
			{
				InternalSchema.ArticleId
			});
		}

		public void Save()
		{
			this.CheckDisposed("Save");
			this.item.Save(SaveMode.NoConflictResolution);
		}

		protected virtual void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.item != null)
				{
					this.item.Dispose();
				}
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
				}
			}
			this.item = null;
		}

		protected void CheckDisposed(string methodName)
		{
			if (this.disposed)
			{
				StorageGlobals.TraceFailedCheckDisposed(this, methodName);
				throw new ObjectDisposedException(base.GetType().ToString());
			}
		}

		private readonly DisposeTracker disposeTracker;

		private MailboxSyncItemId id;

		private bool disposed;

		private Item item;

		private MailboxSyncWatermark watermark;
	}
}
