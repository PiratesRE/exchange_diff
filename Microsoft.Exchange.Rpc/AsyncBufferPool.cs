using System;
using System.Collections.Generic;
using msclr;

namespace Microsoft.Exchange.Rpc
{
	internal class AsyncBufferPool
	{
		public AsyncBufferPool(int bufferSize)
		{
			this.acquireTotal = 0;
			this.releaseTotal = 0;
			this.acquireMiss = 0;
			this.releaseMiss = 0;
			this.poolLock = new object();
			this.pool = new Stack<byte[]>();
			if (bufferSize > 1048576)
			{
				throw new ArgumentOutOfRangeException("bufferSize too large");
			}
			if (bufferSize < 1024)
			{
				throw new ArgumentOutOfRangeException("bufferSize too small");
			}
			this.countLimit = Math.Min(256, 20971520 / bufferSize);
			this.bufferSize = bufferSize;
		}

		public int BufferSize
		{
			get
			{
				return this.bufferSize;
			}
		}

		public byte[] Acquire()
		{
			@lock @lock = null;
			@lock lock2 = new @lock(this.poolLock);
			byte[] result;
			try
			{
				@lock = lock2;
				this.acquireTotal++;
				if (this.pool.Count <= 0)
				{
					goto IL_4B;
				}
				result = this.pool.Pop();
			}
			catch
			{
				((IDisposable)@lock).Dispose();
				throw;
			}
			((IDisposable)@lock).Dispose();
			return result;
			IL_4B:
			try
			{
				this.acquireMiss++;
			}
			catch
			{
				((IDisposable)@lock).Dispose();
				throw;
			}
			((IDisposable)@lock).Dispose();
			return new byte[this.bufferSize];
		}

		public void Release(byte[] buffer)
		{
			@lock @lock = null;
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			int num = this.bufferSize;
			if (buffer.Length != num)
			{
				throw new ArgumentException("buffer wrong size");
			}
			Array.Clear(buffer, 0, num);
			@lock lock2 = new @lock(this.poolLock);
			try
			{
				@lock = lock2;
				this.releaseTotal++;
				if (this.pool.Count < this.countLimit)
				{
					this.pool.Push(buffer);
				}
				else
				{
					this.releaseMiss++;
				}
			}
			catch
			{
				((IDisposable)@lock).Dispose();
				throw;
			}
			((IDisposable)@lock).Dispose();
		}

		private int countLimit;

		private int bufferSize;

		private object poolLock;

		private Stack<byte[]> pool;

		private int acquireTotal;

		private int releaseTotal;

		private int acquireMiss;

		private int releaseMiss;
	}
}
