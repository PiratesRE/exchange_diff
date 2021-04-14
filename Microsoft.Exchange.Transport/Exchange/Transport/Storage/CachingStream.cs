using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Transport.Storage
{
	internal class CachingStream : Stream, IDisposeTrackable, IDisposable
	{
		public CachingStream(Stream parentStream, int maximum)
		{
			this.wrappedStream = parentStream;
			this.maximum = maximum;
			this.SetMemoryStreamModeIfPossible(parentStream.Length);
			this.disposeTracker = this.GetDisposeTracker();
		}

		public override bool CanRead
		{
			get
			{
				this.ThrowIfDisposed();
				return true;
			}
		}

		public override bool CanWrite
		{
			get
			{
				this.ThrowIfDisposed();
				return this.wrappedStream != null && this.wrappedStream.CanWrite;
			}
		}

		public override bool CanSeek
		{
			get
			{
				this.ThrowIfDisposed();
				return true;
			}
		}

		public override long Length
		{
			get
			{
				this.ThrowIfDisposed();
				if (this.memoryStream != null)
				{
					return this.memoryStream.Length;
				}
				return this.wrappedStream.Length;
			}
		}

		public bool InMemory
		{
			get
			{
				this.ThrowIfDisposed();
				return this.memoryStream != null;
			}
		}

		public override long Position
		{
			get
			{
				this.ThrowIfDisposed();
				if (this.memoryStream != null)
				{
					return this.memoryStream.Position;
				}
				return this.wrappedStream.Position;
			}
			set
			{
				this.ThrowIfDisposed();
				if (this.wrappedStream != null && value == this.wrappedStream.Position)
				{
					return;
				}
				this.Seek(value, SeekOrigin.Begin);
			}
		}

		public DisposeTracker GetDisposeTracker()
		{
			this.ThrowIfDisposed();
			return DisposeTracker.Get<CachingStream>(this);
		}

		public void SuppressDisposeTracker()
		{
			this.ThrowIfDisposed();
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public override void Flush()
		{
			this.ThrowIfDisposed();
			this.wrappedStream.Flush();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			this.ThrowIfDisposed();
			if (this.memoryStream != null)
			{
				return this.memoryStream.Read(buffer, offset, count);
			}
			return this.wrappedStream.Read(buffer, offset, count);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			this.ThrowIfDisposed();
			long result = -1L;
			if (this.wrappedStream != null)
			{
				result = this.wrappedStream.Seek(offset, origin);
			}
			if (this.memoryStream != null)
			{
				result = this.memoryStream.Seek(offset, origin);
			}
			return result;
		}

		public override void SetLength(long value)
		{
			this.ThrowIfDisposed();
			if (this.Length != value)
			{
				this.SetMemoryStreamModeIfPossible(value);
				if (this.memoryStream != null)
				{
					if (value > (long)this.maximum)
					{
						this.memoryStream.Close();
						this.memoryStream = null;
					}
					else
					{
						this.memoryStream.SetLength(value);
					}
				}
				this.wrappedStream.SetLength(value);
			}
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			this.ThrowIfDisposed();
			if (this.memoryStream != null)
			{
				if (this.memoryStream.Position + (long)count > (long)this.maximum)
				{
					this.memoryStream.Close();
					this.memoryStream = null;
				}
				else
				{
					this.memoryStream.Write(buffer, offset, count);
				}
			}
			if (this.wrappedStream != null)
			{
				this.wrappedStream.Write(buffer, offset, count);
				return;
			}
			throw new NotSupportedException("The stream does not support writing. ");
		}

		internal void ReleaseDatabase()
		{
			this.ThrowIfDisposed();
			if (this.wrappedStream != null)
			{
				this.wrappedStream.Close();
				this.wrappedStream = null;
			}
		}

		internal void ReOpenForRead()
		{
			this.ThrowIfDisposed();
			using (PooledBufferedStream pooledBufferedStream = this.wrappedStream as PooledBufferedStream)
			{
				if (pooledBufferedStream == null)
				{
					throw new InvalidOperationException(Strings.NotBufferedStream);
				}
				using (DataStreamImmediateWriter dataStreamImmediateWriter = pooledBufferedStream.FlushAndTakeWrappedStream() as DataStreamImmediateWriter)
				{
					if (dataStreamImmediateWriter == null)
					{
						throw new InvalidOperationException(Strings.NotOpenForWrite);
					}
					this.wrappedStream = new PooledBufferedStream(new DataStreamLazyReader(dataStreamImmediateWriter), CachingStream.pool);
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					this.disposed = true;
					if (this.disposeTracker != null)
					{
						this.disposeTracker.Dispose();
						this.disposeTracker = null;
					}
					if (this.wrappedStream != null)
					{
						this.wrappedStream.Close();
						this.wrappedStream = null;
					}
					if (this.memoryStream != null)
					{
						this.memoryStream.Close();
						this.memoryStream = null;
					}
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		private void SetMemoryStreamModeIfPossible(long length)
		{
			if (length == 0L && this.maximum > 0 && this.memoryStream == null)
			{
				this.memoryStream = new PooledMemoryStream(8192);
			}
		}

		private void ThrowIfDisposed()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException("CachingStream");
			}
		}

		private static BufferPool pool = new BufferPool(DataStream.BufferedStreamSize, true);

		private Stream wrappedStream;

		private Stream memoryStream;

		private int maximum;

		private DisposeTracker disposeTracker;

		private bool disposed;
	}
}
