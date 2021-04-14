using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AllContactsCursor : IDisposeTrackable, IDisposable
	{
		public AllContactsCursor(MailboxSession session, PropertyDefinition[] properties, SortBy[] sortByProperties)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				Util.ThrowOnNullArgument(session, "session");
				Util.ThrowOnNullArgument(properties, "properties");
				StorageGlobals.TraceConstructIDisposable(this);
				this.disposeTracker = this.GetDisposeTracker();
				this.session = session;
				this.properties = PropertyDefinitionCollection.Merge<PropertyDefinition>(AllContactsCursor.requiredProperties, properties);
				this.sortByProperties = sortByProperties;
				this.PrepareQuery();
				disposeGuard.Success();
			}
		}

		public IStorePropertyBag Current
		{
			get
			{
				this.CheckDisposed("get_Current");
				if (this.rows == null || this.currentRow < 0)
				{
					return null;
				}
				return this.rows[this.currentRow];
			}
		}

		public int EstimatedRowCount
		{
			get
			{
				this.CheckDisposed("get_EstimatedRowCount");
				return this.query.EstimatedRowCount;
			}
		}

		public void MoveNext()
		{
			this.CheckDisposed("MoveNext");
			for (;;)
			{
				this.currentRow++;
				if (this.rows == null || this.currentRow >= this.rows.Length)
				{
					this.currentRow = 0;
					this.rows = this.query.GetPropertyBags(this.chunkSize);
					if (this.rows.Length == 0)
					{
						break;
					}
					this.chunkSize = Math.Min(this.chunkSize * 2, 1000);
				}
				IStorePropertyBag storePropertyBag = this.Current;
				object obj = storePropertyBag.TryGetProperty(StoreObjectSchema.ItemClass);
				if (storePropertyBag != null && !(storePropertyBag.TryGetProperty(ItemSchema.Id) is PropertyError) && !(obj is PropertyError) && (ObjectClass.IsContact((string)obj) || ObjectClass.IsDistributionList((string)obj)))
				{
					return;
				}
				AllContactsCursor.Tracer.TraceDebug(0L, "AllContactsCursor.MoveNext: Skipping bogus contact");
			}
			this.rows = null;
		}

		private void ResetRows()
		{
			this.rows = null;
			this.currentRow = 0;
			this.chunkSize = 12;
		}

		private void PrepareQuery()
		{
			this.ResetRows();
			StoreObjectId defaultFolderId = this.session.GetDefaultFolderId(DefaultFolderType.AllContacts);
			if (defaultFolderId == null)
			{
				AllContactsCursor.Tracer.TraceDebug(0L, "AllContactsCursor.PrepareQuery: AllContacts search folder doesn't exist. Creating it.");
				this.session.CreateDefaultFolder(DefaultFolderType.AllContacts);
				defaultFolderId = this.session.GetDefaultFolderId(DefaultFolderType.AllContacts);
			}
			this.folder = Folder.Bind(this.session, defaultFolderId);
			this.query = this.folder.ItemQuery(ItemQueryType.None, null, this.sortByProperties, this.properties);
			this.MoveNext();
		}

		public bool SeekToOffset(int offset)
		{
			this.CheckDisposed("SeekToOffset");
			this.ResetRows();
			if (this.query.SeekToOffset(SeekReference.OriginBeginning, offset) == offset)
			{
				this.MoveNext();
				return true;
			}
			return false;
		}

		public bool SeekToCondition(SeekReference reference, QueryFilter seekFilter)
		{
			this.CheckDisposed("SeekToCondition");
			this.ResetRows();
			if (this.query.SeekToCondition(reference, seekFilter))
			{
				this.MoveNext();
				return true;
			}
			return false;
		}

		protected void CheckDisposed(string methodName)
		{
			if (this.isDisposed)
			{
				StorageGlobals.TraceFailedCheckDisposed(this, methodName);
				throw new ObjectDisposedException(base.GetType().ToString());
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			StorageGlobals.TraceDispose(this, this.isDisposed, disposing);
			if (!this.isDisposed)
			{
				this.isDisposed = true;
				this.InternalDispose(disposing);
			}
		}

		protected virtual void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.query != null)
				{
					this.query.Dispose();
					this.query = null;
				}
				if (this.folder != null)
				{
					this.folder.Dispose();
					this.folder = null;
				}
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
				}
			}
		}

		public virtual DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<AllContactsCursor>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		private const int MaxChunkSize = 1000;

		private static readonly Trace Tracer = ExTraceGlobals.PersonTracer;

		private static readonly PropertyDefinition[] requiredProperties = new PropertyDefinition[]
		{
			ItemSchema.Id,
			StoreObjectSchema.ItemClass
		};

		private readonly MailboxSession session;

		private readonly PropertyDefinition[] properties;

		private readonly SortBy[] sortByProperties;

		private readonly DisposeTracker disposeTracker;

		private bool isDisposed;

		private Folder folder;

		private QueryResult query;

		private int chunkSize = 12;

		private IStorePropertyBag[] rows;

		private int currentRow;
	}
}
