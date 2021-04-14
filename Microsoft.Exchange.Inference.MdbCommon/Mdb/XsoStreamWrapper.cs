using System;
using System.IO;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Inference.Mdb
{
	internal class XsoStreamWrapper : Stream, IDisposeTrackable, IDisposable
	{
		public XsoStreamWrapper(Stream wrappedStream) : this(null, wrappedStream)
		{
		}

		public XsoStreamWrapper(Item item, Stream wrappedStream)
		{
			this.item = item;
			this.internalStream = wrappedStream;
			this.canDisposeInternalStream = true;
			this.disposeTracker = this.GetDisposeTracker();
		}

		public override bool CanRead
		{
			get
			{
				this.CheckDisposed();
				return this.internalStream != null && this.internalStream.CanRead;
			}
		}

		public override bool CanWrite
		{
			get
			{
				this.CheckDisposed();
				return this.internalStream != null && this.internalStream.CanWrite;
			}
		}

		public override bool CanSeek
		{
			get
			{
				this.CheckDisposed();
				return this.internalStream != null && this.internalStream.CanSeek;
			}
		}

		public override long Length
		{
			get
			{
				this.CheckDisposed();
				if (this.internalStream == null)
				{
					throw new NotSupportedException();
				}
				return XsoStreamWrapper.MapXsoExceptions<long>(() => this.internalStream.Length);
			}
		}

		public override long Position
		{
			get
			{
				this.CheckDisposed();
				if (this.internalStream == null || !this.internalStream.CanSeek)
				{
					throw new NotSupportedException();
				}
				return XsoStreamWrapper.MapXsoExceptions<long>(() => this.internalStream.Position);
			}
			set
			{
				this.CheckDisposed();
				if (this.internalStream == null || !this.internalStream.CanSeek)
				{
					throw new NotSupportedException();
				}
				XsoStreamWrapper.MapXsoExceptions(delegate()
				{
					this.internalStream.Position = value;
				});
			}
		}

		internal Stream InternalStream
		{
			get
			{
				this.CheckDisposed();
				return this.internalStream;
			}
		}

		public override void Close()
		{
			try
			{
				if (!this.isClosed && this.canDisposeInternalStream && this.internalStream != null)
				{
					XsoStreamWrapper.MapXsoExceptions(new Action(this.internalStream.Close));
					this.internalStream = null;
				}
				this.isClosed = true;
			}
			finally
			{
				base.Close();
			}
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<XsoStreamWrapper>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			this.CheckDisposed();
			if (this.internalStream == null || !this.internalStream.CanWrite)
			{
				throw new NotSupportedException();
			}
			XsoStreamWrapper.MapXsoExceptions(delegate()
			{
				this.internalStream.Write(buffer, offset, count);
			});
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			this.CheckDisposed();
			if (this.internalStream == null || !this.internalStream.CanRead)
			{
				throw new NotSupportedException();
			}
			return XsoStreamWrapper.MapXsoExceptions<int>(() => this.internalStream.Read(buffer, offset, count));
		}

		public override void SetLength(long value)
		{
			this.CheckDisposed();
			if (this.internalStream == null || !this.internalStream.CanWrite || this.internalStream.CanSeek)
			{
				throw new NotSupportedException();
			}
			XsoStreamWrapper.MapXsoExceptions(delegate()
			{
				this.internalStream.SetLength(value);
			});
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			this.CheckDisposed();
			if (this.internalStream == null || !this.internalStream.CanSeek)
			{
				throw new NotSupportedException();
			}
			return XsoStreamWrapper.MapXsoExceptions<long>(() => this.internalStream.Seek(offset, origin));
		}

		public override void Flush()
		{
			this.CheckDisposed();
			if (this.internalStream == null)
			{
				throw new NotSupportedException();
			}
			XsoStreamWrapper.MapXsoExceptions(new Action(this.internalStream.Flush));
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.item != null)
				{
					if (this.item.Session != null)
					{
						this.item.Session.Dispose();
					}
					this.item.Dispose();
					this.item = null;
				}
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
					this.disposeTracker = null;
				}
			}
			base.Dispose(disposing);
		}

		private static TResult MapXsoExceptions<TResult>(Func<TResult> call)
		{
			Exception innerException = null;
			try
			{
				return call();
			}
			catch (StorageTransientException ex)
			{
				innerException = ex;
			}
			catch (StoragePermanentException ex2)
			{
				innerException = ex2;
			}
			throw new OperationFailedException(innerException);
		}

		private static void MapXsoExceptions(Action call)
		{
			Exception ex = null;
			try
			{
				call();
			}
			catch (StorageTransientException ex2)
			{
				ex = ex2;
			}
			catch (StoragePermanentException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				throw new OperationFailedException(ex);
			}
		}

		private void CheckDisposed()
		{
			if (this.isClosed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
		}

		private readonly bool canDisposeInternalStream;

		private Stream internalStream;

		private Item item;

		private bool isClosed;

		private DisposeTracker disposeTracker;
	}
}
