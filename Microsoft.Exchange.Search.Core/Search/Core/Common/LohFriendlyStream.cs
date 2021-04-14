using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Diagnostics;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Search.Core.Common
{
	public class LohFriendlyStream : Stream, IDisposeTrackable, IDisposable
	{
		public LohFriendlyStream(int size)
		{
			if (size < 0)
			{
				throw new ArgumentOutOfRangeException("size", size, NetException.NegativeParameter);
			}
			this.isOpen = true;
			this.startingSize = LohFriendlyStream.GetCapacityToUse(size);
			this.buffers = new List<byte[]>(this.startingSize / 16384);
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

		public LohFriendlyStream GetReference()
		{
			Interlocked.Increment(ref this.referenceCount);
			this.position = 0;
			return this;
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<LohFriendlyStream>(this);
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

		public override int Read(byte[] buffer, int offset, int count)
		{
			LohFriendlyStream.CheckBufferArguments(buffer, offset, count);
			int num = this.length - this.position;
			if (num > count)
			{
				num = count;
			}
			if (num > 0)
			{
				int i = 0;
				int num2 = this.position / 16384;
				int num3 = this.position % 16384;
				while (i < num)
				{
					byte[] array = this.buffers[num2];
					int num4 = Math.Min(array.Length - num3, num - i);
					Buffer.BlockCopy(array, num3, buffer, offset + i, num4);
					this.position += num4;
					i += num4;
					num2++;
					num3 = 0;
				}
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
			int result = (int)this.buffers[this.position / 16384][this.position % 16384];
			this.position++;
			return result;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			LohFriendlyStream.CheckForLargeBufferIndex(offset);
			switch (origin)
			{
			case SeekOrigin.Begin:
				LohFriendlyStream.CheckForNegativeBufferIndex(offset);
				this.position = (int)offset;
				break;
			case SeekOrigin.Current:
			{
				long num = offset + (long)this.position;
				LohFriendlyStream.CheckForLargeBufferIndex(num);
				LohFriendlyStream.CheckForNegativeBufferIndex(num);
				this.position = (int)num;
				break;
			}
			case SeekOrigin.End:
			{
				long num = (long)this.length + offset;
				LohFriendlyStream.CheckForLargeBufferIndex(num);
				LohFriendlyStream.CheckForNegativeBufferIndex(num);
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
			this.TryIncreaseCapacity(num);
			this.length = num;
			if (this.position > num)
			{
				this.position = num;
			}
		}

		public byte[] ToArray()
		{
			byte[] array = new byte[this.length];
			int num = 0;
			for (int i = 0; i < this.buffers.Count; i++)
			{
				int num2 = Math.Min(this.buffers[i].Length, this.length - num);
				Buffer.BlockCopy(this.buffers[i], 0, array, num, num2);
				num += num2;
			}
			return array;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			LohFriendlyStream.CheckBufferArguments(buffer, offset, count);
			long num = (long)(this.position + count);
			LohFriendlyStream.CheckForLargeBufferIndex(num);
			LohFriendlyStream.CheckForNegativeBufferIndex(num);
			int value = (int)num;
			if (num > (long)this.length)
			{
				if (num > (long)this.capacity)
				{
					this.TryIncreaseCapacity(value);
				}
				this.length = value;
			}
			int i = 0;
			int num2 = this.position / 16384;
			int num3 = this.position % 16384;
			while (i < count)
			{
				byte[] array = this.buffers[num2];
				int num4 = Math.Min(array.Length - num3, count - i);
				Buffer.BlockCopy(buffer, offset + i, array, num3, num4);
				this.position += num4;
				i += num4;
				num2++;
				num3 = 0;
			}
			this.position = value;
		}

		public override void WriteByte(byte value)
		{
			if (this.position >= this.length)
			{
				int num = this.position + 1;
				if (num >= this.capacity)
				{
					this.TryIncreaseCapacity(num);
				}
				this.length = num;
			}
			this.buffers[this.position / 16384][this.position % 16384] = value;
			this.position++;
		}

		public void WriteTo(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			int num = 0;
			int num2 = 0;
			while (num2 < this.buffers.Count && this.Length - (long)num > 0L)
			{
				int num3 = Math.Min(this.buffers[num2].Length, this.length - num);
				stream.Write(this.buffers[num2], 0, num3);
				num += num3;
				num2++;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (Interlocked.Decrement(ref this.referenceCount) == 0)
			{
				this.InternalDispose(disposing);
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
			int num = minimumCapacity / 16384 + ((minimumCapacity % 16384 > 0) ? 1 : 0);
			return num * 16384;
		}

		private void InternalDispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					this.isOpen = false;
					this.ReleaseCurrentBuffers();
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
				this.SetCapacity(LohFriendlyStream.GetCapacityToUse(num));
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
				ExTraceGlobals.CommonTracer.TraceWarning<int, int>((long)this.GetHashCode(), "LohFriendlyStream.SetCapacity({0}) called for a stream that had a buffer with capacity = {1}.", value, this.capacity);
				if (value > 0)
				{
					while (this.buffers.Count * 16384 < value)
					{
						this.buffers.Add(this.currentPool.Acquire());
					}
				}
				this.capacity = value;
			}
		}

		private void ReleaseCurrentBuffers()
		{
			for (int i = 0; i < this.buffers.Count; i++)
			{
				this.currentPool.Release(this.buffers[i]);
			}
			this.buffers.Clear();
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

		private const int DefaultBufferSize = 16384;

		private readonly int startingSize;

		private readonly DisposeTracker disposeTracker;

		private List<byte[]> buffers;

		private BufferPool currentPool = BufferPoolCollection.AutoCleanupCollection.Acquire(BufferPoolCollection.BufferSize.Size16K);

		private int referenceCount = 1;

		private bool isOpen;

		private int capacity;

		private int length;

		private int position;
	}
}
