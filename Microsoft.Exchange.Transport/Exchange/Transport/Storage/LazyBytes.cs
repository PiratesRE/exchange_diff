using System;
using System.IO;

namespace Microsoft.Exchange.Transport.Storage
{
	internal class LazyBytes : IDataWithinRowComponent, IDataObjectComponent
	{
		public LazyBytes(DataRow row, DataColumn column)
		{
			if (row == null)
			{
				throw new ArgumentNullException("row");
			}
			this.row = row;
			if (column == null)
			{
				throw new ArgumentNullException("column");
			}
			this.column = column;
		}

		public LazyBytes(DataRow row, BlobCollection blobCollection, byte blobCollectionKey)
		{
			if (row == null)
			{
				throw new ArgumentNullException("row");
			}
			this.row = row;
			if (blobCollection == null)
			{
				throw new ArgumentNullException("blobCollection");
			}
			this.blobCollection = blobCollection;
			this.blobCollectionKey = blobCollectionKey;
		}

		public LazyBytes()
		{
		}

		public byte[] Value
		{
			get
			{
				if (this.IsDeferred)
				{
					lock (this)
					{
						if (this.IsDeferred)
						{
							this.DeferredLoad();
						}
					}
				}
				return this.bytes;
			}
			set
			{
				this.ThrowIfReadOnly();
				this.state &= ~LazyBytes.State.Deferred;
				this.state |= LazyBytes.State.Dirty;
				this.bytes = value;
			}
		}

		public bool PendingDatabaseUpdates
		{
			get
			{
				return this.IsDirty;
			}
		}

		public int PendingDatabaseUpdateCount
		{
			get
			{
				if (!this.IsDirty)
				{
					return 0;
				}
				return 1;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return (byte)(this.state & LazyBytes.State.ReadOnly) == 16;
			}
			set
			{
				if (value)
				{
					this.state |= LazyBytes.State.ReadOnly;
					return;
				}
				this.state &= ~LazyBytes.State.ReadOnly;
			}
		}

		private bool IsDirty
		{
			get
			{
				return (byte)(this.state & LazyBytes.State.Dirty) == 1;
			}
		}

		private bool IsDeleted
		{
			get
			{
				return (byte)(this.state & LazyBytes.State.Deleted) == 8;
			}
		}

		private bool IsDeferred
		{
			get
			{
				return (byte)(this.state & LazyBytes.State.Deferred) == 4;
			}
		}

		private bool IsSaved
		{
			get
			{
				return (byte)(this.state & LazyBytes.State.PreviouslySaved) == 2;
			}
		}

		public void CloneFrom(IDataObjectComponent other)
		{
			this.ThrowIfReadOnly();
			this.state = (LazyBytes.State.PreviouslySaved | LazyBytes.State.Deferred);
		}

		public void MarkDeleted()
		{
			this.state |= LazyBytes.State.Deleted;
		}

		public void MinimizeMemory()
		{
			if (this.IsDeleted)
			{
				this.bytes = null;
				return;
			}
			if (this.IsSaved && !this.IsDirty)
			{
				this.bytes = null;
				this.state |= LazyBytes.State.Deferred;
			}
		}

		public void LoadFromParentRow(DataTableCursor cursor)
		{
			this.ThrowIfReadOnly();
			if (cursor == null)
			{
				throw new ArgumentNullException("cursor");
			}
			this.state = (LazyBytes.State.PreviouslySaved | LazyBytes.State.Deferred);
			this.row.PerfCounters.LazyBytesLoadRequested.Increment();
		}

		public void SaveToParentRow(DataTableCursor cursor, Func<bool> checkpointCallback)
		{
			if (cursor == null)
			{
				throw new ArgumentNullException("cursor");
			}
			if (!cursor.IsWithinTransaction)
			{
				throw new InvalidOperationException(Strings.NotInTransaction);
			}
			if (this.IsDeleted || !this.IsDirty)
			{
				return;
			}
			using (Stream stream = (this.blobCollection == null) ? this.column.OpenImmediateWriter(cursor, this.row, this.IsSaved, 1) : this.blobCollection.OpenWriter(this.blobCollectionKey, cursor, this.IsSaved, false, null))
			{
				if (this.bytes != null)
				{
					stream.Write(this.bytes, 0, this.bytes.Length);
				}
				else
				{
					stream.SetLength(0L);
				}
			}
			this.state = (LazyBytes.State)(2 | (byte)(this.state & LazyBytes.State.ReadOnly));
		}

		public void Cleanup()
		{
		}

		private void DeferredLoad()
		{
			using (DataTableCursor dataTableCursor = this.OpenCursor())
			{
				using (dataTableCursor.BeginTransaction())
				{
					this.MoveToItem(dataTableCursor);
					this.LoadFromCursor(dataTableCursor);
				}
			}
			this.row.PerfCounters.LazyBytesLoadPerformed.Increment();
		}

		private void LoadFromCursor(DataTableCursor cursor)
		{
			using (Stream stream = (this.blobCollection == null) ? this.column.OpenImmediateReader(cursor, this.row, 1) : this.blobCollection.OpenReader(this.blobCollectionKey, cursor, false))
			{
				if (stream.Length > 0L)
				{
					this.bytes = new byte[stream.Length];
					stream.Read(this.bytes, 0, (int)stream.Length);
				}
			}
			this.state = (LazyBytes.State)(2 | (byte)(this.state & LazyBytes.State.ReadOnly));
		}

		private DataTableCursor OpenCursor()
		{
			return this.row.Table.GetCursor();
		}

		private void MoveToItem(DataTableCursor cursor)
		{
			this.row.SeekCurrent(cursor);
		}

		private void ThrowIfReadOnly()
		{
			if (this.IsReadOnly)
			{
				throw new InvalidOperationException("This LazyBytes operation cannot be performed in read-only mode.");
			}
		}

		private readonly DataRow row;

		private readonly DataColumn column;

		private readonly byte blobCollectionKey;

		private readonly BlobCollection blobCollection;

		private volatile LazyBytes.State state;

		private byte[] bytes;

		[Flags]
		private enum State : byte
		{
			New = 0,
			Dirty = 1,
			PreviouslySaved = 2,
			Deferred = 4,
			Deleted = 8,
			ReadOnly = 16
		}
	}
}
