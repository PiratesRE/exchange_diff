using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net
{
	internal sealed class PooledBufferedStream : Stream, IDisposeTrackable, IDisposable
	{
		public PooledBufferedStream(Stream stream, BufferPool bufferPool, bool closeStream) : this(stream, bufferPool, bufferPool.BufferSize, closeStream)
		{
		}

		public PooledBufferedStream(Stream stream, BufferPool bufferPool, int bufferSizeToUse, bool closeStream) : this(stream, bufferPool, bufferSizeToUse)
		{
			this.closeStream = closeStream;
		}

		public PooledBufferedStream(Stream stream, BufferPoolCollection.BufferSize bufferSize) : this(stream, BufferPoolCollection.AutoCleanupCollection.Acquire(bufferSize))
		{
		}

		public PooledBufferedStream(Stream stream, BufferPool bufferPool) : this(stream, bufferPool, bufferPool.BufferSize)
		{
		}

		public PooledBufferedStream(Stream stream, BufferPool bufferPool, int bufferSizeToUse)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (bufferPool == null)
			{
				throw new ArgumentNullException("bufferPool");
			}
			if (bufferPool.BufferSize < bufferSizeToUse)
			{
				throw new ArgumentException("Buffer size mismatch");
			}
			if (!stream.CanRead)
			{
				if (!stream.CanWrite)
				{
					throw new ArgumentException(NetException.ImmutableStream, "stream");
				}
				this.isWriteMode = true;
			}
			this.pool = bufferPool;
			this.bufferSize = bufferSizeToUse;
			this.internalStream = stream;
			if (this.internalStream.CanSeek)
			{
				this.position = this.internalStream.Position;
			}
			this.bufferOffset = this.position;
			this.disposeTracker = this.GetDisposeTracker();
		}

		public override bool CanRead
		{
			get
			{
				return this.internalStream.CanRead;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return this.internalStream.CanWrite;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return this.internalStream.CanSeek;
			}
		}

		public override long Length
		{
			get
			{
				long length = this.internalStream.Length;
				long val = (this.bufferTopBorder != 0) ? (this.bufferOffset + (long)this.bufferTopBorder) : 0L;
				return Math.Max(length, val);
			}
		}

		public override long Position
		{
			get
			{
				if (!this.internalStream.CanSeek)
				{
					throw new NotSupportedException();
				}
				return this.position;
			}
			set
			{
				if (!this.internalStream.CanSeek)
				{
					throw new NotSupportedException();
				}
				this.position = value;
			}
		}

		private bool HasDataBufferedForReading
		{
			get
			{
				return this.position < this.bufferOffset + (long)this.bufferTopBorder && this.position >= this.bufferOffset;
			}
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<PooledBufferedStream>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public Stream FlushAndTakeWrappedStream()
		{
			this.Flush();
			Stream result = this.internalStream;
			this.internalStream = null;
			return result;
		}

		public override void Write(byte[] source, int offset, int count)
		{
			this.EnsureWriting();
			int num = this.FlushIfPositionOutOfBuffer();
			if (num == 0 && count > this.bufferSize)
			{
				this.UpdateInternalStreamPositionForWriting();
				this.internalStream.Write(source, offset, count);
				this.position += (long)count;
				this.bufferOffset = this.position;
				return;
			}
			int num2 = Math.Min(this.bufferSize - num, count);
			this.EnsureBufferAcquired();
			Array.Copy(source, offset, this.buffer, num, num2);
			this.bufferTopBorder = Math.Max(num + num2, this.bufferTopBorder);
			this.position += (long)num2;
			this.FlushIfBufferFull();
			if (num2 < count)
			{
				this.Write(source, offset + num2, count - num2);
			}
		}

		public override int Read(byte[] destination, int offset, int count)
		{
			this.EnsureReading();
			if (!this.HasDataBufferedForReading)
			{
				this.UpdateInternalStreamPositionForReading();
				if (count >= this.bufferSize)
				{
					int num = this.internalStream.Read(destination, offset, count);
					this.position += (long)num;
					return num;
				}
				if (!this.FillBuffer())
				{
					return 0;
				}
			}
			int num2 = (int)(this.position - this.bufferOffset);
			int num3 = Math.Min(count, this.bufferTopBorder - num2);
			Array.Copy(this.buffer, num2, destination, offset, num3);
			this.position += (long)num3;
			return num3;
		}

		public override void SetLength(long value)
		{
			if (!this.internalStream.CanSeek)
			{
				throw new NotSupportedException();
			}
			this.EnsureWriting();
			this.FlushBuffer();
			if (this.internalStream.Position != this.position)
			{
				this.internalStream.Position = this.position;
			}
			this.internalStream.SetLength(value);
			this.position = this.internalStream.Position;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			if (!this.internalStream.CanSeek)
			{
				throw new NotSupportedException();
			}
			switch (origin)
			{
			case SeekOrigin.Begin:
				this.position = offset;
				break;
			case SeekOrigin.Current:
				this.position += offset;
				break;
			case SeekOrigin.End:
				if (this.isWriteMode)
				{
					this.FlushBuffer();
				}
				this.internalStream.Seek(offset, SeekOrigin.End);
				this.position = this.internalStream.Position;
				break;
			}
			return this.position;
		}

		public override void Flush()
		{
			if (this.internalStream.CanWrite)
			{
				if (this.isWriteMode)
				{
					this.FlushBuffer();
				}
				this.internalStream.Flush();
				return;
			}
			throw new NotSupportedException();
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					this.isClosed = true;
					if (this.internalStream != null)
					{
						try
						{
							if (this.isWriteMode)
							{
								this.FlushBuffer();
							}
						}
						finally
						{
							if (this.closeStream)
							{
								this.internalStream.Dispose();
								this.internalStream = null;
							}
						}
					}
					if (this.disposeTracker != null)
					{
						this.disposeTracker.Dispose();
					}
				}
			}
			finally
			{
				base.Dispose(disposing);
				if (disposing && this.buffer != null && this.pool != null)
				{
					this.pool.Release(this.buffer);
					this.buffer = null;
					this.pool = null;
				}
			}
		}

		[Conditional("DEBUG")]
		private void CheckDisposed(string methodName)
		{
			if (this.isClosed)
			{
				throw new ObjectDisposedException(base.GetType().ToString(), methodName);
			}
		}

		private void EnsureBufferAcquired()
		{
			if (this.buffer == null)
			{
				this.buffer = this.pool.Acquire();
			}
		}

		private void EnsureReading()
		{
			if (!this.isWriteMode)
			{
				return;
			}
			if (!this.internalStream.CanRead)
			{
				throw new NotSupportedException();
			}
			if (this.bufferTopBorder != 0)
			{
				this.UpdateInternalStreamPositionForWriting();
				this.internalStream.Write(this.buffer, 0, this.bufferTopBorder);
			}
			this.isWriteMode = false;
		}

		private void EnsureWriting()
		{
			if (this.isWriteMode)
			{
				return;
			}
			if (!this.internalStream.CanWrite || !this.internalStream.CanSeek)
			{
				throw new NotSupportedException();
			}
			this.bufferTopBorder = 0;
			this.bufferOffset = this.position;
			this.isWriteMode = true;
		}

		private void UpdateInternalStreamPositionForReading()
		{
			if (this.internalStream.CanSeek && this.internalStream.Position != this.position)
			{
				this.internalStream.Position = this.position;
			}
		}

		private bool FillBuffer()
		{
			this.bufferOffset = this.position;
			this.EnsureBufferAcquired();
			this.bufferTopBorder = this.internalStream.Read(this.buffer, 0, this.bufferSize);
			return this.bufferTopBorder != 0;
		}

		private void UpdateInternalStreamPositionForWriting()
		{
			if (this.internalStream.CanSeek && this.internalStream.Position != this.bufferOffset)
			{
				this.internalStream.Position = this.bufferOffset;
			}
		}

		private void FlushBuffer()
		{
			if (this.bufferTopBorder != 0)
			{
				this.UpdateInternalStreamPositionForWriting();
				this.internalStream.Write(this.buffer, 0, this.bufferTopBorder);
				this.bufferTopBorder = 0;
			}
			this.bufferOffset = this.position;
		}

		private int FlushIfPositionOutOfBuffer()
		{
			if (this.position < this.bufferOffset || this.position > this.bufferOffset + (long)this.bufferTopBorder)
			{
				this.FlushBuffer();
			}
			return (int)(this.position - this.bufferOffset);
		}

		private void FlushIfBufferFull()
		{
			if (this.bufferTopBorder == this.bufferSize)
			{
				this.FlushBuffer();
			}
		}

		private readonly int bufferSize;

		private readonly DisposeTracker disposeTracker;

		private readonly bool closeStream = true;

		private BufferPool pool;

		private byte[] buffer;

		private Stream internalStream;

		private bool isClosed;

		private long position;

		private long bufferOffset;

		private int bufferTopBorder;

		private bool isWriteMode;
	}
}
