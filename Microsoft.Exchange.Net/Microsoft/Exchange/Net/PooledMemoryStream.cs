using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Diagnostics;

namespace Microsoft.Exchange.Net
{
	public sealed class PooledMemoryStream : Stream, IDisposeTrackable, IDisposable
	{
		public PooledMemoryStream(int size)
		{
			if (size < 0)
			{
				throw new ArgumentOutOfRangeException("size", size, NetException.NegativeParameter);
			}
			this.isOpen = true;
			this.startingSize = PooledMemoryStream.GetCapacityToUse(size);
			this.currentBuffer = PooledMemoryStream.empty;
			this.disposeTracker = this.GetDisposeTracker();
		}

		public override bool CanRead
		{
			get
			{
				return this.isOpen;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return this.isOpen;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return this.isOpen;
			}
		}

		public int Capacity
		{
			get
			{
				return this.capacity;
			}
			set
			{
				this.TryIncreaseCapacity(value);
			}
		}

		public override long Length
		{
			get
			{
				return (long)this.length;
			}
		}

		public override long Position
		{
			get
			{
				return (long)this.position;
			}
			set
			{
				if (value < 0L)
				{
					throw new ArgumentOutOfRangeException("value", value, NetException.NegativeParameter);
				}
				if (value > 2147483647L)
				{
					throw new ArgumentOutOfRangeException("value", value, NetException.LargeParameter);
				}
				this.position = (int)value;
			}
		}

		internal BufferPool CurrentBufferPool
		{
			get
			{
				return this.currentPool;
			}
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<PooledMemoryStream>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public override void Flush()
		{
		}

		public byte[] GetBuffer()
		{
			return this.currentBuffer;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			PooledMemoryStream.CheckBufferArguments(buffer, offset, count);
			int num = this.length - this.position;
			if (num > count)
			{
				num = count;
			}
			if (num > 0)
			{
				Buffer.BlockCopy(this.currentBuffer, this.position, buffer, offset, num);
				this.position += num;
			}
			else
			{
				num = 0;
			}
			return num;
		}

		public override int ReadByte()
		{
			if (this.position >= this.length)
			{
				return -1;
			}
			return (int)this.currentBuffer[this.position++];
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			PooledMemoryStream.CheckForLargeBufferIndex(offset);
			switch (origin)
			{
			case SeekOrigin.Begin:
				PooledMemoryStream.CheckForNegativeBufferIndex(offset);
				this.position = (int)offset;
				break;
			case SeekOrigin.Current:
			{
				long num = offset + (long)this.position;
				PooledMemoryStream.CheckForLargeBufferIndex(num);
				PooledMemoryStream.CheckForNegativeBufferIndex(num);
				this.position = (int)num;
				break;
			}
			case SeekOrigin.End:
			{
				long num = (long)this.length + offset;
				PooledMemoryStream.CheckForLargeBufferIndex(num);
				PooledMemoryStream.CheckForNegativeBufferIndex(num);
				this.position = (int)num;
				break;
			}
			default:
				throw new ArgumentException(NetException.SeekOrigin, "origin");
			}
			return (long)this.position;
		}

		public override void SetLength(long value)
		{
			if (value > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("value", value, NetException.LargeParameter);
			}
			if (value < 0L)
			{
				throw new ArgumentOutOfRangeException("value", value, NetException.NegativeParameter);
			}
			int num = (int)value;
			if (!this.TryIncreaseCapacity(num) && num > this.length)
			{
				Array.Clear(this.currentBuffer, this.length, num - this.length);
			}
			this.length = num;
			if (this.position > num)
			{
				this.position = num;
			}
		}

		public byte[] ToArray()
		{
			byte[] array = new byte[this.length];
			Buffer.BlockCopy(this.currentBuffer, 0, array, 0, this.length);
			return array;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			PooledMemoryStream.CheckBufferArguments(buffer, offset, count);
			long num = (long)(this.position + count);
			PooledMemoryStream.CheckForLargeBufferIndex(num);
			PooledMemoryStream.CheckForNegativeBufferIndex(num);
			int num2 = (int)num;
			if (num > (long)this.length)
			{
				bool flag = (num <= (long)this.capacity || !this.TryIncreaseCapacity(num2)) && this.position > this.length;
				if (flag)
				{
					Array.Clear(this.currentBuffer, this.length, num2 - this.length);
				}
				this.length = num2;
			}
			Buffer.BlockCopy(buffer, offset, this.currentBuffer, this.position, count);
			this.position = num2;
		}

		public override void WriteByte(byte value)
		{
			if (this.position >= this.length)
			{
				int num = this.position + 1;
				bool flag = this.position > this.length;
				if (num >= this.capacity && this.TryIncreaseCapacity(num))
				{
					flag = false;
				}
				if (flag)
				{
					Array.Clear(this.currentBuffer, this.length, this.position - this.length);
				}
				this.length = num;
			}
			this.currentBuffer[this.position++] = value;
		}

		public void WriteTo(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			stream.Write(this.currentBuffer, 0, this.length);
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					this.isOpen = false;
					if (this.currentBuffer != null && this.currentPool != null)
					{
						this.currentPool.Release(this.currentBuffer);
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
			}
		}

		private static void CheckForNegativeBufferIndex(long index)
		{
			if (index < 0L)
			{
				throw new IOException(NetException.NegativeIndex);
			}
		}

		private static void CheckForLargeBufferIndex(long index)
		{
			if (index > 2147483647L)
			{
				throw new IOException(NetException.LargeIndex);
			}
		}

		private static void CheckBufferArguments(byte[] buffer, int offset, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", offset, NetException.NegativeParameter);
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", count, NetException.NegativeParameter);
			}
			if (buffer.Length - offset < count)
			{
				throw new ArgumentException(NetException.BufferOverflow);
			}
		}

		private static int GetCapacityToUse(int minimumCapacity)
		{
			if (minimumCapacity < 0)
			{
				throw new ArgumentOutOfRangeException("minimumCapacity", minimumCapacity, NetException.NegativeCapacity);
			}
			int num = 256;
			while (num < 2147483647 && num < minimumCapacity)
			{
				if (num < 0)
				{
					num = int.MaxValue;
					break;
				}
				num *= 2;
			}
			return num;
		}

		private bool TryIncreaseCapacity(int value)
		{
			if (value < 0)
			{
				throw new IOException(NetException.NegativeCapacity);
			}
			int num = Math.Max(value, this.startingSize);
			bool flag = num > this.capacity;
			if (flag)
			{
				this.SetCapacity(PooledMemoryStream.GetCapacityToUse(num));
			}
			return flag;
		}

		private void SetCapacity(int value)
		{
			if (value != this.capacity)
			{
				if (value < this.length)
				{
					throw new ArgumentOutOfRangeException("value", value, NetException.SmallCapacity);
				}
				ExTraceGlobals.CommonTracer.TraceWarning<int, int>((long)this.GetHashCode(), "PooledMemoryStream.SetCapacity({0}) called for a stream that had a buffer with capacity = {1}.", value, this.capacity);
				if (value > 0)
				{
					BufferPoolCollection.BufferSize bufferSize;
					BufferPool bufferPool;
					byte[] array;
					if (PooledMemoryStream.pool.TryMatchBufferSize(value, out bufferSize))
					{
						bufferPool = PooledMemoryStream.pool.Acquire(bufferSize);
						array = bufferPool.Acquire();
					}
					else
					{
						bufferPool = null;
						array = new byte[value];
					}
					if (this.length > 0)
					{
						Buffer.BlockCopy(this.currentBuffer, 0, array, 0, this.length);
					}
					this.ReplaceCurrentBuffer(array);
					this.currentPool = bufferPool;
				}
				else
				{
					this.ReplaceCurrentBuffer(null);
				}
				this.capacity = value;
			}
		}

		private void ReplaceCurrentBuffer(byte[] newBuffer)
		{
			if (this.currentBuffer != null && this.currentPool != null)
			{
				this.currentPool.Release(this.currentBuffer);
			}
			this.currentBuffer = newBuffer;
		}

		[Conditional("DEBUG")]
		private void CheckDisposed()
		{
			if (!this.isOpen)
			{
				throw new ObjectDisposedException(NetException.ClosedStream);
			}
		}

		public const int MaximumBufferSize = 2147483647;

		private static readonly byte[] empty = new byte[0];

		private static readonly BufferPoolCollection pool = BufferPoolCollection.AutoCleanupCollection;

		private readonly int startingSize;

		private readonly DisposeTracker disposeTracker;

		private byte[] currentBuffer;

		private BufferPool currentPool;

		private bool isOpen;

		private int capacity;

		private int length;

		private int position;
	}
}
