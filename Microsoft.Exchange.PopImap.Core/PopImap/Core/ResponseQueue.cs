using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PopImap.Core
{
	internal class ResponseQueue : IDisposeTrackable, IDisposable
	{
		public ResponseQueue() : this(100)
		{
		}

		public ResponseQueue(int capacity) : this(capacity, true)
		{
		}

		public ResponseQueue(int capacity, bool shouldBufferResponse)
		{
			this.items = new Queue<IResponseItem>((capacity > 0) ? capacity : 100);
			this.disposeTracker = this.GetDisposeTracker();
			this.disposed = false;
			this.shouldBufferResponse = shouldBufferResponse;
			this.isSending = false;
			this.inBufferMode = true;
			if (this.shouldBufferResponse)
			{
				this.pooledBufferResponseItem = new PooledBufferResponseItem(ResponseQueue.responseBufferCapacity);
			}
		}

		public object SyncRoot
		{
			get
			{
				return this.syncRoot;
			}
		}

		public int Count
		{
			get
			{
				return this.items.Count;
			}
		}

		public bool IsSending
		{
			get
			{
				this.CheckDisposed();
				return this.isSending;
			}
		}

		public void Enqueue(IResponseItem item)
		{
			this.CheckDisposed();
			this.items.Enqueue(item);
		}

		public void DequeueForSend()
		{
			this.CheckDisposed();
			this.itemBeingSent = this.items.Dequeue();
			this.isSending = true;
			this.inBufferMode = true;
		}

		public void Clear()
		{
			this.CheckDisposed();
			this.items.Clear();
		}

		public int GetNextChunk(BaseSession session, out byte[] buffer, out int offset)
		{
			this.CheckDisposed();
			buffer = null;
			offset = 0;
			int num = 0;
			bool flag = false;
			try
			{
				if (this.shouldBufferResponse)
				{
					num = this.InternalGetNextChunk(session, out buffer, out offset);
					while (num > 0 && this.pooledBufferResponseItem.Size - num < ResponseQueue.responseBufferCapacity)
					{
						this.pooledBufferResponseItem.Write(buffer, offset, num);
						num = this.InternalGetNextChunk(session, out buffer, out offset);
					}
					if (session.Disposed)
					{
						flag = true;
						return 0;
					}
					if (num > 0)
					{
						this.pooledBufferResponseItem.Write(buffer, offset, num);
					}
					if (this.pooledBufferResponseItem.Size > 0)
					{
						num = this.pooledBufferResponseItem.GetNextChunk(session, out buffer, out offset);
						flag = true;
						return num;
					}
				}
				num = this.InternalGetNextChunk(session, out buffer, out offset);
				if (num == 0)
				{
					if (!this.inBufferMode)
					{
						num = this.InternalGetNextChunk(session, out buffer, out offset);
						if (num != 0)
						{
							flag = true;
							return num;
						}
					}
					this.isSending = false;
				}
			}
			finally
			{
				if (!flag)
				{
					this.isSending = false;
				}
			}
			return num;
		}

		public virtual DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<ResponseQueue>(this);
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
			this.disposed = true;
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
				this.disposeTracker = null;
			}
			this.DisposeItemsQueued();
			this.DisposeItemBeingProcessed();
			if (this.pooledBufferResponseItem != null)
			{
				this.pooledBufferResponseItem.Dispose();
				this.pooledBufferResponseItem = null;
			}
		}

		private int InternalGetNextChunk(BaseSession session, out byte[] buffer, out int offset)
		{
			buffer = null;
			offset = 0;
			if (this.itemBeingSent == null)
			{
				if (this.items.Count <= 0)
				{
					return 0;
				}
				this.DequeueForSend();
			}
			if (this.inBufferMode && (this.itemBeingSent is EndResponseItem || this.itemBeingSent.SendCompleteDelegate != null))
			{
				this.inBufferMode = false;
				return 0;
			}
			int nextChunk = this.itemBeingSent.GetNextChunk(session, out buffer, out offset);
			if (session.Disposed)
			{
				return 0;
			}
			while (nextChunk == 0)
			{
				if (this.itemBeingSent.SendCompleteDelegate != null)
				{
					this.itemBeingSent.SendCompleteDelegate();
				}
				if (session.Disposed)
				{
					return 0;
				}
				this.DisposeItemBeingProcessed();
				if (this.items.Count == 0)
				{
					this.itemBeingSent = null;
					return 0;
				}
				this.DequeueForSend();
				if (this.itemBeingSent == null)
				{
					return 0;
				}
				nextChunk = this.itemBeingSent.GetNextChunk(session, out buffer, out offset);
			}
			return nextChunk;
		}

		private void CheckDisposed()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
		}

		private void DisposeItemsQueued()
		{
			foreach (object obj in this.items)
			{
				IDisposable disposable = obj as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			this.items.Clear();
		}

		private void DisposeItemBeingProcessed()
		{
			if (this.itemBeingSent != null)
			{
				IDisposable disposable = this.itemBeingSent as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
				this.itemBeingSent = null;
			}
		}

		private const int DefaultCapacity = 100;

		private static int responseBufferCapacity = 8192;

		private object syncRoot = new object();

		private Queue<IResponseItem> items;

		private IResponseItem itemBeingSent;

		private DisposeTracker disposeTracker;

		private bool disposed;

		private bool shouldBufferResponse;

		private PooledBufferResponseItem pooledBufferResponseItem;

		private bool isSending;

		private bool inBufferMode;
	}
}
