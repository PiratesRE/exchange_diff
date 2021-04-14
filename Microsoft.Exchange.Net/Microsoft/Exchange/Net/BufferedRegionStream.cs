using System;
using System.IO;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net
{
	internal class BufferedRegionStream : Stream, IDisposeTrackable, IDisposable
	{
		public BufferedRegionStream(Stream stream) : this(stream, false)
		{
		}

		public BufferedRegionStream(Stream stream, bool takeStreamOwnership)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (!stream.CanRead)
			{
				throw new ArgumentException("Stream is not readable", "stream");
			}
			this.stream = stream;
			this.ownWrappedStream = takeStreamOwnership;
			this.disposeTracker = ((IDisposeTrackable)this).GetDisposeTracker();
		}

		public override bool CanRead
		{
			get
			{
				return !this.isDisposed;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return !this.isDisposed;
			}
		}

		public override bool CanTimeout
		{
			get
			{
				return this.stream.CanTimeout;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}

		public override long Length
		{
			get
			{
				this.CheckDisposed();
				return this.stream.Length;
			}
		}

		public static BufferedRegionStream CreateWithBufferPoolCollection(Stream stream, int maxBufferSize, bool takeStreamOwnership)
		{
			BufferedRegionStream bufferedRegionStream = null;
			bool flag = false;
			BufferedRegionStream result;
			try
			{
				BufferPool bufferPool = null;
				bufferedRegionStream = new BufferedRegionStream(stream, takeStreamOwnership);
				bufferedRegionStream.SetBufferedRegion(maxBufferSize, delegate(int size)
				{
					BufferPoolCollection.BufferSize bufferSize;
					if (!BufferPoolCollection.AutoCleanupCollection.TryMatchBufferSize(maxBufferSize, out bufferSize))
					{
						throw new InvalidOperationException(string.Format("Could not get buffer size of {0} for BufferedRegionStream buffer", maxBufferSize));
					}
					bufferPool = BufferPoolCollection.AutoCleanupCollection.Acquire(bufferSize);
					return bufferPool.Acquire();
				}, delegate(byte[] memory)
				{
					bufferPool.Release(memory);
				});
				flag = true;
				result = bufferedRegionStream;
			}
			finally
			{
				if (!flag && bufferedRegionStream != null)
				{
					bufferedRegionStream.Dispose();
				}
			}
			return result;
		}

		public override long Position
		{
			get
			{
				this.CheckDisposed();
				return this.stream.Position;
			}
			set
			{
				this.CheckDisposed();
				if (this.regionBuffer == null)
				{
					throw new InvalidOperationException("Not currently buffering");
				}
				if (value < 0L)
				{
					throw new ArgumentOutOfRangeException("Position cannot be set to a negative value");
				}
				if (value > (long)this.regionBuffer.Length)
				{
					throw new ArgumentOutOfRangeException("Position set beyond the end of the buffer");
				}
				if (value > this.regionBufferPosition)
				{
					throw new ArgumentOutOfRangeException("Position set beyond furthest read");
				}
				this.regionBufferPosition = value;
			}
		}

		public override int ReadTimeout
		{
			get
			{
				return this.stream.ReadTimeout;
			}
			set
			{
				this.stream.ReadTimeout = value;
			}
		}

		public override int WriteTimeout
		{
			get
			{
				return this.stream.WriteTimeout;
			}
			set
			{
				this.stream.WriteTimeout = value;
			}
		}

		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			this.CheckDisposed();
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (buffer.Length - offset < count)
			{
				throw new ArgumentException("offset and count run past end of buffer");
			}
			if (this.regionBuffer == null)
			{
				return this.stream.BeginRead(buffer, offset, count, callback, state);
			}
			if (count == 0)
			{
				IAsyncResult asyncResult3 = new BufferedRegionStream.BufferedAsyncResult(state, 0);
				if (callback != null)
				{
					callback(asyncResult3);
				}
				return asyncResult3;
			}
			if (this.regionBufferPosition < this.stream.Position)
			{
				long num = this.stream.Position - this.regionBufferPosition;
				if ((long)count < num)
				{
					num = (long)count;
				}
				Array.Copy(this.regionBuffer, this.regionBufferPosition, buffer, (long)offset, num);
				this.regionBufferPosition += num;
				IAsyncResult asyncResult2 = new BufferedRegionStream.BufferedAsyncResult(state, (int)num);
				if (callback != null)
				{
					callback(asyncResult2);
				}
				return asyncResult2;
			}
			AsyncCallback callback2 = null;
			if (callback != null)
			{
				callback2 = delegate(IAsyncResult asyncResult)
				{
					BufferedRegionStream.WrappedAsyncResult ar = new BufferedRegionStream.WrappedAsyncResult(asyncResult, buffer, offset);
					callback(ar);
				};
			}
			IAsyncResult wrappedResult = this.stream.BeginRead(buffer, offset, count, callback2, state);
			return new BufferedRegionStream.WrappedAsyncResult(wrappedResult, buffer, offset);
		}

		public override int EndRead(IAsyncResult asyncResult)
		{
			this.CheckDisposed();
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			BufferedRegionStream.BufferedAsyncResult bufferedAsyncResult = asyncResult as BufferedRegionStream.BufferedAsyncResult;
			if (bufferedAsyncResult != null)
			{
				return bufferedAsyncResult.BytesRead;
			}
			BufferedRegionStream.WrappedAsyncResult wrappedAsyncResult = asyncResult as BufferedRegionStream.WrappedAsyncResult;
			if (wrappedAsyncResult != null)
			{
				int num = this.stream.EndRead(wrappedAsyncResult.WrappedResult);
				if ((long)(this.regionBuffer.Length - num) < this.regionBufferPosition)
				{
					this.releaseAction(this.regionBuffer);
					this.regionBuffer = null;
				}
				else
				{
					Array.Copy(wrappedAsyncResult.Buffer, (long)wrappedAsyncResult.Offset, this.regionBuffer, this.regionBufferPosition, (long)num);
					this.regionBufferPosition += (long)num;
				}
				return num;
			}
			return this.stream.EndRead(asyncResult);
		}

		public override void Flush()
		{
			this.CheckDisposed();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			this.CheckDisposed();
			if (count == 0)
			{
				return 0;
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (buffer.Length - offset < count)
			{
				throw new ArgumentException("offset and count run past end of buffer");
			}
			if (this.regionBuffer == null)
			{
				return this.stream.Read(buffer, offset, count);
			}
			if (this.regionBufferPosition < this.stream.Position)
			{
				long num = this.stream.Position - this.regionBufferPosition;
				if ((long)count < num)
				{
					num = (long)count;
				}
				Array.Copy(this.regionBuffer, this.regionBufferPosition, buffer, (long)offset, num);
				this.regionBufferPosition += num;
				return (int)num;
			}
			int num2 = this.stream.Read(buffer, offset, count);
			if ((long)(this.regionBuffer.Length - num2) < this.regionBufferPosition)
			{
				this.releaseAction(this.regionBuffer);
				this.regionBuffer = null;
			}
			else
			{
				Array.Copy(buffer, (long)offset, this.regionBuffer, this.regionBufferPosition, (long)num2);
				this.regionBufferPosition += (long)num2;
			}
			return num2;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			this.CheckDisposed();
			throw new NotImplementedException();
		}

		public void SetBufferedRegion(int regionSize, Func<int, byte[]> acquireFunc, Action<byte[]> releaseAction)
		{
			this.CheckDisposed();
			if (regionSize <= 0)
			{
				throw new ArgumentOutOfRangeException("regionSize");
			}
			if (acquireFunc == null)
			{
				throw new ArgumentNullException("acquireFunc");
			}
			if (releaseAction == null)
			{
				throw new ArgumentNullException("releaseAction");
			}
			if (this.stream.Position != 0L)
			{
				throw new InvalidOperationException("Can only set buffered region at the start of a stream");
			}
			this.regionBuffer = acquireFunc(regionSize);
			this.releaseAction = releaseAction;
		}

		public override void SetLength(long value)
		{
			this.CheckDisposed();
			throw new NotSupportedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			this.CheckDisposed();
			throw new NotSupportedException();
		}

		DisposeTracker IDisposeTrackable.GetDisposeTracker()
		{
			return DisposeTracker.Get<BufferedRegionStream>(this);
		}

		void IDisposeTrackable.SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (this.regionBuffer != null)
			{
				this.releaseAction(this.regionBuffer);
				this.regionBuffer = null;
			}
			if (this.ownWrappedStream && this.stream != null)
			{
				this.stream.Dispose();
				this.stream = null;
			}
			base.Dispose(disposing);
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
			}
			this.isDisposed = true;
		}

		protected void CheckDisposed()
		{
			if (this.isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
		}

		private readonly bool ownWrappedStream;

		private Stream stream;

		private DisposeTracker disposeTracker;

		private bool isDisposed;

		private byte[] regionBuffer;

		private Action<byte[]> releaseAction;

		private long regionBufferPosition;

		private class BufferedAsyncResult : IAsyncResult
		{
			public BufferedAsyncResult(object asyncState, int bytesRead)
			{
				this.asyncState = asyncState;
				this.bytesRead = bytesRead;
			}

			public object AsyncState
			{
				get
				{
					return this.asyncState;
				}
			}

			public WaitHandle AsyncWaitHandle
			{
				get
				{
					if (this.asyncWaitHandle == null)
					{
						this.asyncWaitHandle = new ManualResetEvent(true);
					}
					return this.asyncWaitHandle;
				}
			}

			public bool CompletedSynchronously
			{
				get
				{
					return true;
				}
			}

			public bool IsCompleted
			{
				get
				{
					return true;
				}
			}

			public int BytesRead
			{
				get
				{
					return this.bytesRead;
				}
			}

			private readonly object asyncState;

			private readonly int bytesRead;

			private WaitHandle asyncWaitHandle;
		}

		private class WrappedAsyncResult : IAsyncResult
		{
			public WrappedAsyncResult(IAsyncResult wrappedResult, byte[] buffer, int offset)
			{
				this.wrappedResult = wrappedResult;
				this.buffer = buffer;
				this.offset = offset;
			}

			public object AsyncState
			{
				get
				{
					return this.wrappedResult.AsyncState;
				}
			}

			public WaitHandle AsyncWaitHandle
			{
				get
				{
					return this.wrappedResult.AsyncWaitHandle;
				}
			}

			public bool CompletedSynchronously
			{
				get
				{
					return this.wrappedResult.CompletedSynchronously;
				}
			}

			public bool IsCompleted
			{
				get
				{
					return this.wrappedResult.IsCompleted;
				}
			}

			public byte[] Buffer
			{
				get
				{
					return this.buffer;
				}
			}

			public int Offset
			{
				get
				{
					return this.offset;
				}
			}

			public IAsyncResult WrappedResult
			{
				get
				{
					return this.wrappedResult;
				}
			}

			private readonly IAsyncResult wrappedResult;

			private readonly byte[] buffer;

			private readonly int offset;
		}
	}
}
