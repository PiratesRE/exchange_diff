using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO
{
	[ComVisible(true)]
	public sealed class BufferedStream : Stream
	{
		private BufferedStream()
		{
		}

		public BufferedStream(Stream stream) : this(stream, 4096)
		{
		}

		public BufferedStream(Stream stream, int bufferSize)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (bufferSize <= 0)
			{
				throw new ArgumentOutOfRangeException("bufferSize", Environment.GetResourceString("ArgumentOutOfRange_MustBePositive", new object[]
				{
					"bufferSize"
				}));
			}
			this._stream = stream;
			this._bufferSize = bufferSize;
			if (!this._stream.CanRead && !this._stream.CanWrite)
			{
				__Error.StreamIsClosed();
			}
		}

		private void EnsureNotClosed()
		{
			if (this._stream == null)
			{
				__Error.StreamIsClosed();
			}
		}

		private void EnsureCanSeek()
		{
			if (!this._stream.CanSeek)
			{
				__Error.SeekNotSupported();
			}
		}

		private void EnsureCanRead()
		{
			if (!this._stream.CanRead)
			{
				__Error.ReadNotSupported();
			}
		}

		private void EnsureCanWrite()
		{
			if (!this._stream.CanWrite)
			{
				__Error.WriteNotSupported();
			}
		}

		private void EnsureBeginEndAwaitableAllocated()
		{
			if (this._beginEndAwaitable == null)
			{
				this._beginEndAwaitable = new BeginEndAwaitableAdapter();
			}
		}

		private void EnsureShadowBufferAllocated()
		{
			if (this._buffer.Length != this._bufferSize || this._bufferSize >= 81920)
			{
				return;
			}
			byte[] array = new byte[Math.Min(this._bufferSize + this._bufferSize, 81920)];
			Buffer.InternalBlockCopy(this._buffer, 0, array, 0, this._writePos);
			this._buffer = array;
		}

		private void EnsureBufferAllocated()
		{
			if (this._buffer == null)
			{
				this._buffer = new byte[this._bufferSize];
			}
		}

		internal Stream UnderlyingStream
		{
			[FriendAccessAllowed]
			get
			{
				return this._stream;
			}
		}

		internal int BufferSize
		{
			[FriendAccessAllowed]
			get
			{
				return this._bufferSize;
			}
		}

		public override bool CanRead
		{
			get
			{
				return this._stream != null && this._stream.CanRead;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return this._stream != null && this._stream.CanWrite;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return this._stream != null && this._stream.CanSeek;
			}
		}

		public override long Length
		{
			get
			{
				this.EnsureNotClosed();
				if (this._writePos > 0)
				{
					this.FlushWrite();
				}
				return this._stream.Length;
			}
		}

		public override long Position
		{
			get
			{
				this.EnsureNotClosed();
				this.EnsureCanSeek();
				return this._stream.Position + (long)(this._readPos - this._readLen + this._writePos);
			}
			set
			{
				if (value < 0L)
				{
					throw new ArgumentOutOfRangeException("value", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
				}
				this.EnsureNotClosed();
				this.EnsureCanSeek();
				if (this._writePos > 0)
				{
					this.FlushWrite();
				}
				this._readPos = 0;
				this._readLen = 0;
				this._stream.Seek(value, SeekOrigin.Begin);
			}
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing && this._stream != null)
				{
					try
					{
						this.Flush();
					}
					finally
					{
						this._stream.Close();
					}
				}
			}
			finally
			{
				this._stream = null;
				this._buffer = null;
				this._lastSyncCompletedReadTask = null;
				base.Dispose(disposing);
			}
		}

		public override void Flush()
		{
			this.EnsureNotClosed();
			if (this._writePos > 0)
			{
				this.FlushWrite();
				return;
			}
			if (this._readPos >= this._readLen)
			{
				if (this._stream.CanWrite || this._stream is BufferedStream)
				{
					this._stream.Flush();
				}
				this._writePos = (this._readPos = (this._readLen = 0));
				return;
			}
			if (!this._stream.CanSeek)
			{
				return;
			}
			this.FlushRead();
			if (this._stream.CanWrite || this._stream is BufferedStream)
			{
				this._stream.Flush();
			}
		}

		public override Task FlushAsync(CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCancellation<int>(cancellationToken);
			}
			this.EnsureNotClosed();
			return BufferedStream.FlushAsyncInternal(cancellationToken, this, this._stream, this._writePos, this._readPos, this._readLen);
		}

		private static async Task FlushAsyncInternal(CancellationToken cancellationToken, BufferedStream _this, Stream stream, int writePos, int readPos, int readLen)
		{
			SemaphoreSlim sem = _this.EnsureAsyncActiveSemaphoreInitialized();
			await sem.WaitAsync().ConfigureAwait(false);
			try
			{
				if (writePos > 0)
				{
					await _this.FlushWriteAsync(cancellationToken).ConfigureAwait(false);
				}
				else if (readPos < readLen)
				{
					if (stream.CanSeek)
					{
						_this.FlushRead();
						if (stream.CanRead || stream is BufferedStream)
						{
							await stream.FlushAsync(cancellationToken).ConfigureAwait(false);
						}
					}
				}
				else if (stream.CanWrite || stream is BufferedStream)
				{
					await stream.FlushAsync(cancellationToken).ConfigureAwait(false);
				}
			}
			finally
			{
				sem.Release();
			}
		}

		private void FlushRead()
		{
			if (this._readPos - this._readLen != 0)
			{
				this._stream.Seek((long)(this._readPos - this._readLen), SeekOrigin.Current);
			}
			this._readPos = 0;
			this._readLen = 0;
		}

		private void ClearReadBufferBeforeWrite()
		{
			if (this._readPos == this._readLen)
			{
				this._readPos = (this._readLen = 0);
				return;
			}
			if (!this._stream.CanSeek)
			{
				throw new NotSupportedException(Environment.GetResourceString("NotSupported_CannotWriteToBufferedStreamIfReadBufferCannotBeFlushed"));
			}
			this.FlushRead();
		}

		private void FlushWrite()
		{
			this._stream.Write(this._buffer, 0, this._writePos);
			this._writePos = 0;
			this._stream.Flush();
		}

		private async Task FlushWriteAsync(CancellationToken cancellationToken)
		{
			await this._stream.WriteAsync(this._buffer, 0, this._writePos, cancellationToken).ConfigureAwait(false);
			this._writePos = 0;
			await this._stream.FlushAsync(cancellationToken).ConfigureAwait(false);
		}

		private int ReadFromBuffer(byte[] array, int offset, int count)
		{
			int num = this._readLen - this._readPos;
			if (num == 0)
			{
				return 0;
			}
			if (num > count)
			{
				num = count;
			}
			Buffer.InternalBlockCopy(this._buffer, this._readPos, array, offset, num);
			this._readPos += num;
			return num;
		}

		private int ReadFromBuffer(byte[] array, int offset, int count, out Exception error)
		{
			int result;
			try
			{
				error = null;
				result = this.ReadFromBuffer(array, offset, count);
			}
			catch (Exception ex)
			{
				error = ex;
				result = 0;
			}
			return result;
		}

		public override int Read([In] [Out] byte[] array, int offset, int count)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array", Environment.GetResourceString("ArgumentNull_Buffer"));
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
			}
			if (array.Length - offset < count)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_InvalidOffLen"));
			}
			this.EnsureNotClosed();
			this.EnsureCanRead();
			int num = this.ReadFromBuffer(array, offset, count);
			if (num == count)
			{
				return num;
			}
			int num2 = num;
			if (num > 0)
			{
				count -= num;
				offset += num;
			}
			this._readPos = (this._readLen = 0);
			if (this._writePos > 0)
			{
				this.FlushWrite();
			}
			if (count >= this._bufferSize)
			{
				return this._stream.Read(array, offset, count) + num2;
			}
			this.EnsureBufferAllocated();
			this._readLen = this._stream.Read(this._buffer, 0, this._bufferSize);
			num = this.ReadFromBuffer(array, offset, count);
			return num + num2;
		}

		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer", Environment.GetResourceString("ArgumentNull_Buffer"));
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
			}
			if (buffer.Length - offset < count)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_InvalidOffLen"));
			}
			if (this._stream == null)
			{
				__Error.ReadNotSupported();
			}
			this.EnsureCanRead();
			int num = 0;
			SemaphoreSlim semaphoreSlim = base.EnsureAsyncActiveSemaphoreInitialized();
			Task task = semaphoreSlim.WaitAsync();
			if (task.Status == TaskStatus.RanToCompletion)
			{
				bool flag = true;
				try
				{
					Exception ex;
					num = this.ReadFromBuffer(buffer, offset, count, out ex);
					flag = (num == count || ex != null);
					if (flag)
					{
						Stream.SynchronousAsyncResult synchronousAsyncResult = (ex == null) ? new Stream.SynchronousAsyncResult(num, state) : new Stream.SynchronousAsyncResult(ex, state, false);
						if (callback != null)
						{
							callback(synchronousAsyncResult);
						}
						return synchronousAsyncResult;
					}
				}
				finally
				{
					if (flag)
					{
						semaphoreSlim.Release();
					}
				}
			}
			return this.BeginReadFromUnderlyingStream(buffer, offset + num, count - num, callback, state, num, task);
		}

		private IAsyncResult BeginReadFromUnderlyingStream(byte[] buffer, int offset, int count, AsyncCallback callback, object state, int bytesAlreadySatisfied, Task semaphoreLockTask)
		{
			Task<int> task = this.ReadFromUnderlyingStreamAsync(buffer, offset, count, CancellationToken.None, bytesAlreadySatisfied, semaphoreLockTask, true);
			return TaskToApm.Begin(task, callback, state);
		}

		public override int EndRead(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			Stream.SynchronousAsyncResult synchronousAsyncResult = asyncResult as Stream.SynchronousAsyncResult;
			if (synchronousAsyncResult != null)
			{
				return Stream.SynchronousAsyncResult.EndRead(asyncResult);
			}
			return TaskToApm.End<int>(asyncResult);
		}

		private Task<int> LastSyncCompletedReadTask(int val)
		{
			Task<int> task = this._lastSyncCompletedReadTask;
			if (task != null && task.Result == val)
			{
				return task;
			}
			task = Task.FromResult<int>(val);
			this._lastSyncCompletedReadTask = task;
			return task;
		}

		public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer", Environment.GetResourceString("ArgumentNull_Buffer"));
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
			}
			if (buffer.Length - offset < count)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_InvalidOffLen"));
			}
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCancellation<int>(cancellationToken);
			}
			this.EnsureNotClosed();
			this.EnsureCanRead();
			int num = 0;
			SemaphoreSlim semaphoreSlim = base.EnsureAsyncActiveSemaphoreInitialized();
			Task task = semaphoreSlim.WaitAsync();
			if (task.Status == TaskStatus.RanToCompletion)
			{
				bool flag = true;
				try
				{
					Exception ex;
					num = this.ReadFromBuffer(buffer, offset, count, out ex);
					flag = (num == count || ex != null);
					if (flag)
					{
						return (ex == null) ? this.LastSyncCompletedReadTask(num) : Task.FromException<int>(ex);
					}
				}
				finally
				{
					if (flag)
					{
						semaphoreSlim.Release();
					}
				}
			}
			return this.ReadFromUnderlyingStreamAsync(buffer, offset + num, count - num, cancellationToken, num, task, false);
		}

		private async Task<int> ReadFromUnderlyingStreamAsync(byte[] array, int offset, int count, CancellationToken cancellationToken, int bytesAlreadySatisfied, Task semaphoreLockTask, bool useApmPattern)
		{
			await semaphoreLockTask.ConfigureAwait(false);
			int result;
			try
			{
				int num = this.ReadFromBuffer(array, offset, count);
				if (num == count)
				{
					result = bytesAlreadySatisfied + num;
				}
				else
				{
					if (num > 0)
					{
						count -= num;
						offset += num;
						bytesAlreadySatisfied += num;
					}
					int num2 = 0;
					this._readLen = num2;
					this._readPos = num2;
					if (this._writePos > 0)
					{
						await this.FlushWriteAsync(cancellationToken).ConfigureAwait(false);
					}
					if (count >= this._bufferSize)
					{
						if (useApmPattern)
						{
							this.EnsureBeginEndAwaitableAllocated();
							this._stream.BeginRead(array, offset, count, BeginEndAwaitableAdapter.Callback, this._beginEndAwaitable);
							int num3 = bytesAlreadySatisfied;
							Stream stream = this._stream;
							result = num3 + stream.EndRead(await this._beginEndAwaitable);
						}
						else
						{
							int num3 = bytesAlreadySatisfied;
							result = num3 + await this._stream.ReadAsync(array, offset, count, cancellationToken).ConfigureAwait(false);
						}
					}
					else
					{
						this.EnsureBufferAllocated();
						if (useApmPattern)
						{
							this.EnsureBeginEndAwaitableAllocated();
							this._stream.BeginRead(this._buffer, 0, this._bufferSize, BeginEndAwaitableAdapter.Callback, this._beginEndAwaitable);
							BufferedStream bufferedStream = this;
							int readLen = bufferedStream._readLen;
							Stream stream = this._stream;
							bufferedStream._readLen = stream.EndRead(await this._beginEndAwaitable);
							bufferedStream = null;
							stream = null;
						}
						else
						{
							BufferedStream bufferedStream = this;
							int readLen2 = bufferedStream._readLen;
							bufferedStream._readLen = await this._stream.ReadAsync(this._buffer, 0, this._bufferSize, cancellationToken).ConfigureAwait(false);
							bufferedStream = null;
						}
						result = bytesAlreadySatisfied + this.ReadFromBuffer(array, offset, count);
					}
				}
			}
			finally
			{
				base.EnsureAsyncActiveSemaphoreInitialized().Release();
			}
			return result;
		}

		public override int ReadByte()
		{
			this.EnsureNotClosed();
			this.EnsureCanRead();
			if (this._readPos == this._readLen)
			{
				if (this._writePos > 0)
				{
					this.FlushWrite();
				}
				this.EnsureBufferAllocated();
				this._readLen = this._stream.Read(this._buffer, 0, this._bufferSize);
				this._readPos = 0;
			}
			if (this._readPos == this._readLen)
			{
				return -1;
			}
			byte[] buffer = this._buffer;
			int readPos = this._readPos;
			this._readPos = readPos + 1;
			return buffer[readPos];
		}

		private void WriteToBuffer(byte[] array, ref int offset, ref int count)
		{
			int num = Math.Min(this._bufferSize - this._writePos, count);
			if (num <= 0)
			{
				return;
			}
			this.EnsureBufferAllocated();
			Buffer.InternalBlockCopy(array, offset, this._buffer, this._writePos, num);
			this._writePos += num;
			count -= num;
			offset += num;
		}

		private void WriteToBuffer(byte[] array, ref int offset, ref int count, out Exception error)
		{
			try
			{
				error = null;
				this.WriteToBuffer(array, ref offset, ref count);
			}
			catch (Exception ex)
			{
				error = ex;
			}
		}

		public override void Write(byte[] array, int offset, int count)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array", Environment.GetResourceString("ArgumentNull_Buffer"));
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
			}
			if (array.Length - offset < count)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_InvalidOffLen"));
			}
			this.EnsureNotClosed();
			this.EnsureCanWrite();
			if (this._writePos == 0)
			{
				this.ClearReadBufferBeforeWrite();
			}
			int num;
			bool flag;
			checked
			{
				num = this._writePos + count;
				flag = (num + count < this._bufferSize + this._bufferSize);
			}
			if (!flag)
			{
				if (this._writePos > 0)
				{
					if (num <= this._bufferSize + this._bufferSize && num <= 81920)
					{
						this.EnsureShadowBufferAllocated();
						Buffer.InternalBlockCopy(array, offset, this._buffer, this._writePos, count);
						this._stream.Write(this._buffer, 0, num);
						this._writePos = 0;
						return;
					}
					this._stream.Write(this._buffer, 0, this._writePos);
					this._writePos = 0;
				}
				this._stream.Write(array, offset, count);
				return;
			}
			this.WriteToBuffer(array, ref offset, ref count);
			if (this._writePos < this._bufferSize)
			{
				return;
			}
			this._stream.Write(this._buffer, 0, this._writePos);
			this._writePos = 0;
			this.WriteToBuffer(array, ref offset, ref count);
		}

		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer", Environment.GetResourceString("ArgumentNull_Buffer"));
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
			}
			if (buffer.Length - offset < count)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_InvalidOffLen"));
			}
			if (this._stream == null)
			{
				__Error.ReadNotSupported();
			}
			this.EnsureCanWrite();
			SemaphoreSlim semaphoreSlim = base.EnsureAsyncActiveSemaphoreInitialized();
			Task task = semaphoreSlim.WaitAsync();
			if (task.Status == TaskStatus.RanToCompletion)
			{
				bool flag = true;
				try
				{
					if (this._writePos == 0)
					{
						this.ClearReadBufferBeforeWrite();
					}
					flag = (count < this._bufferSize - this._writePos);
					if (flag)
					{
						Exception ex;
						this.WriteToBuffer(buffer, ref offset, ref count, out ex);
						Stream.SynchronousAsyncResult synchronousAsyncResult = (ex == null) ? new Stream.SynchronousAsyncResult(state) : new Stream.SynchronousAsyncResult(ex, state, true);
						if (callback != null)
						{
							callback(synchronousAsyncResult);
						}
						return synchronousAsyncResult;
					}
				}
				finally
				{
					if (flag)
					{
						semaphoreSlim.Release();
					}
				}
			}
			return this.BeginWriteToUnderlyingStream(buffer, offset, count, callback, state, task);
		}

		private IAsyncResult BeginWriteToUnderlyingStream(byte[] buffer, int offset, int count, AsyncCallback callback, object state, Task semaphoreLockTask)
		{
			Task task = this.WriteToUnderlyingStreamAsync(buffer, offset, count, CancellationToken.None, semaphoreLockTask, true);
			return TaskToApm.Begin(task, callback, state);
		}

		public override void EndWrite(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			Stream.SynchronousAsyncResult synchronousAsyncResult = asyncResult as Stream.SynchronousAsyncResult;
			if (synchronousAsyncResult != null)
			{
				Stream.SynchronousAsyncResult.EndWrite(asyncResult);
				return;
			}
			TaskToApm.End(asyncResult);
		}

		public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer", Environment.GetResourceString("ArgumentNull_Buffer"));
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
			}
			if (buffer.Length - offset < count)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_InvalidOffLen"));
			}
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCancellation<int>(cancellationToken);
			}
			this.EnsureNotClosed();
			this.EnsureCanWrite();
			SemaphoreSlim semaphoreSlim = base.EnsureAsyncActiveSemaphoreInitialized();
			Task task = semaphoreSlim.WaitAsync();
			if (task.Status == TaskStatus.RanToCompletion)
			{
				bool flag = true;
				try
				{
					if (this._writePos == 0)
					{
						this.ClearReadBufferBeforeWrite();
					}
					flag = (count < this._bufferSize - this._writePos);
					if (flag)
					{
						Exception ex;
						this.WriteToBuffer(buffer, ref offset, ref count, out ex);
						return (ex == null) ? Task.CompletedTask : Task.FromException(ex);
					}
				}
				finally
				{
					if (flag)
					{
						semaphoreSlim.Release();
					}
				}
			}
			return this.WriteToUnderlyingStreamAsync(buffer, offset, count, cancellationToken, task, false);
		}

		private async Task WriteToUnderlyingStreamAsync(byte[] array, int offset, int count, CancellationToken cancellationToken, Task semaphoreLockTask, bool useApmPattern)
		{
			await semaphoreLockTask.ConfigureAwait(false);
			try
			{
				if (this._writePos == 0)
				{
					this.ClearReadBufferBeforeWrite();
				}
				int totalUserBytes = checked(this._writePos + count);
				if (checked(totalUserBytes + count < this._bufferSize + this._bufferSize))
				{
					this.WriteToBuffer(array, ref offset, ref count);
					if (this._writePos >= this._bufferSize)
					{
						if (useApmPattern)
						{
							this.EnsureBeginEndAwaitableAllocated();
							this._stream.BeginWrite(this._buffer, 0, this._writePos, BeginEndAwaitableAdapter.Callback, this._beginEndAwaitable);
							Stream stream = this._stream;
							stream.EndWrite(await this._beginEndAwaitable);
							stream = null;
						}
						else
						{
							await this._stream.WriteAsync(this._buffer, 0, this._writePos, cancellationToken).ConfigureAwait(false);
						}
						this._writePos = 0;
						this.WriteToBuffer(array, ref offset, ref count);
					}
				}
				else
				{
					if (this._writePos > 0)
					{
						if (totalUserBytes <= this._bufferSize + this._bufferSize && totalUserBytes <= 81920)
						{
							this.EnsureShadowBufferAllocated();
							Buffer.InternalBlockCopy(array, offset, this._buffer, this._writePos, count);
							if (useApmPattern)
							{
								this.EnsureBeginEndAwaitableAllocated();
								this._stream.BeginWrite(this._buffer, 0, totalUserBytes, BeginEndAwaitableAdapter.Callback, this._beginEndAwaitable);
								Stream stream = this._stream;
								stream.EndWrite(await this._beginEndAwaitable);
								stream = null;
							}
							else
							{
								await this._stream.WriteAsync(this._buffer, 0, totalUserBytes, cancellationToken).ConfigureAwait(false);
							}
							this._writePos = 0;
							return;
						}
						if (useApmPattern)
						{
							this.EnsureBeginEndAwaitableAllocated();
							this._stream.BeginWrite(this._buffer, 0, this._writePos, BeginEndAwaitableAdapter.Callback, this._beginEndAwaitable);
							Stream stream = this._stream;
							stream.EndWrite(await this._beginEndAwaitable);
							stream = null;
						}
						else
						{
							await this._stream.WriteAsync(this._buffer, 0, this._writePos, cancellationToken).ConfigureAwait(false);
						}
						this._writePos = 0;
					}
					if (useApmPattern)
					{
						this.EnsureBeginEndAwaitableAllocated();
						this._stream.BeginWrite(array, offset, count, BeginEndAwaitableAdapter.Callback, this._beginEndAwaitable);
						Stream stream = this._stream;
						stream.EndWrite(await this._beginEndAwaitable);
						stream = null;
					}
					else
					{
						await this._stream.WriteAsync(array, offset, count, cancellationToken).ConfigureAwait(false);
					}
				}
			}
			finally
			{
				base.EnsureAsyncActiveSemaphoreInitialized().Release();
			}
		}

		public override void WriteByte(byte value)
		{
			this.EnsureNotClosed();
			if (this._writePos == 0)
			{
				this.EnsureCanWrite();
				this.ClearReadBufferBeforeWrite();
				this.EnsureBufferAllocated();
			}
			if (this._writePos >= this._bufferSize - 1)
			{
				this.FlushWrite();
			}
			byte[] buffer = this._buffer;
			int writePos = this._writePos;
			this._writePos = writePos + 1;
			buffer[writePos] = value;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			this.EnsureNotClosed();
			this.EnsureCanSeek();
			if (this._writePos > 0)
			{
				this.FlushWrite();
				return this._stream.Seek(offset, origin);
			}
			if (this._readLen - this._readPos > 0 && origin == SeekOrigin.Current)
			{
				offset -= (long)(this._readLen - this._readPos);
			}
			long position = this.Position;
			long num = this._stream.Seek(offset, origin);
			this._readPos = (int)(num - (position - (long)this._readPos));
			if (0 <= this._readPos && this._readPos < this._readLen)
			{
				this._stream.Seek((long)(this._readLen - this._readPos), SeekOrigin.Current);
			}
			else
			{
				this._readPos = (this._readLen = 0);
			}
			return num;
		}

		public override void SetLength(long value)
		{
			if (value < 0L)
			{
				throw new ArgumentOutOfRangeException("value", Environment.GetResourceString("ArgumentOutOfRange_NegFileSize"));
			}
			this.EnsureNotClosed();
			this.EnsureCanSeek();
			this.EnsureCanWrite();
			this.Flush();
			this._stream.SetLength(value);
		}

		private const int _DefaultBufferSize = 4096;

		private Stream _stream;

		private byte[] _buffer;

		private readonly int _bufferSize;

		private int _readPos;

		private int _readLen;

		private int _writePos;

		private BeginEndAwaitableAdapter _beginEndAwaitable;

		private Task<int> _lastSyncCompletedReadTask;

		private const int MaxShadowBufferSize = 81920;
	}
}
