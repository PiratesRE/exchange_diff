using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Common.Cache;

namespace Microsoft.Exchange.Common.Net
{
	internal class MultiByteArrayMemoryStream : Stream
	{
		public override void Flush()
		{
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			long num;
			switch (origin)
			{
			case SeekOrigin.Begin:
				num = offset;
				break;
			case SeekOrigin.Current:
				num = this.position + offset;
				break;
			case SeekOrigin.End:
				num = this.length + offset;
				break;
			default:
				throw new InvalidOperationException("Unexpected SeekOrigin " + origin);
			}
			if (num < 0L)
			{
				throw new InvalidOperationException(string.Format("Unexpected offset {0} is specified with SeekOrigin {1}. Current position is {2}. Current length is {3}", new object[]
				{
					offset,
					origin,
					this.position,
					this.length
				}));
			}
			if (num > this.length)
			{
				if (num - this.length > 2000L)
				{
					throw new InvalidOperationException(string.Format("Cannot seek forward more than 2000 positions. Seek offset requested is {0}. Seek origin requested is {1}", offset, origin));
				}
				this.IncreaseCapacity(num);
			}
			this.position = num;
			return this.position;
		}

		private void IncreaseCapacity(long capacity)
		{
			while (capacity > (long)(this.inMemoryBackingStorage.Count * MultiByteArrayMemoryStream.BufferSize))
			{
				this.inMemoryBackingStorage.Add(MultiByteArrayMemoryStream.cache.GetBuffer(MultiByteArrayMemoryStream.BufferSize));
			}
			this.length = Math.Max(capacity, this.length);
		}

		public override void SetLength(long value)
		{
			throw new NotImplementedException();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			this.CheckArguments(buffer, offset, count);
			for (int i = offset; i < offset + count; i++)
			{
				if (this.position == this.length)
				{
					return i - offset;
				}
				buffer[i] = this.ReadByteAtCurrentPosition();
			}
			return count;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			this.CheckArguments(buffer, offset, count);
			this.IncreaseCapacity(this.position + (long)count);
			int num2;
			for (int i = offset; i < offset + count; i += num2)
			{
				byte[] byteArrayForCurrentPosition = this.GetByteArrayForCurrentPosition();
				long num = this.position % (long)MultiByteArrayMemoryStream.BufferSize;
				long val = (long)MultiByteArrayMemoryStream.BufferSize - num;
				num2 = (int)Math.Min(val, (long)(count - (i - offset)));
				Buffer.BlockCopy(buffer, i, byteArrayForCurrentPosition, (int)num, num2);
				this.position += (long)num2;
			}
		}

		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return true;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return true;
			}
		}

		public override long Length
		{
			get
			{
				return this.length;
			}
		}

		public override long Position
		{
			get
			{
				return this.position;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public override int ReadByte()
		{
			if (this.position == this.length)
			{
				return -1;
			}
			return (int)this.ReadByteAtCurrentPosition();
		}

		public override void WriteByte(byte value)
		{
			this.IncreaseCapacity(this.position + 1L);
			this.WriteByteAtCurrentPosition(value);
		}

		public override void Close()
		{
			lock (this)
			{
				if (this.inMemoryBackingStorage != null)
				{
					foreach (BufferCacheEntry bufferCacheEntry in this.inMemoryBackingStorage)
					{
						MultiByteArrayMemoryStream.cache.ReturnBuffer(bufferCacheEntry);
					}
					this.inMemoryBackingStorage.Clear();
					this.inMemoryBackingStorage = null;
				}
			}
			base.Close();
		}

		private void WriteByteAtCurrentPosition(byte value)
		{
			byte[] byteArrayForCurrentPosition = this.GetByteArrayForCurrentPosition();
			byteArrayForCurrentPosition[(int)(checked((IntPtr)(this.position % unchecked((long)MultiByteArrayMemoryStream.BufferSize))))] = value;
			this.position += 1L;
		}

		private void CheckArguments(byte[] buffer, int offset, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (buffer.Length < offset + count)
			{
				throw new InvalidOperationException(string.Format("Unexpected buffer size {0}. Expected a buffer with size {1} or more", buffer.Length, offset + count));
			}
			if (this.position > this.length)
			{
				throw new InvalidOperationException(string.Format("Position {0} cannot be greater than Length {1}", this.position, this.length));
			}
		}

		private byte[] GetByteArrayForCurrentPosition()
		{
			if (this.position >= this.length)
			{
				throw new InvalidOperationException(string.Format("Position {0} cannot be greater than or equal to length {1}", this.position, this.length));
			}
			return this.inMemoryBackingStorage[(int)this.position / MultiByteArrayMemoryStream.BufferSize].Buffer;
		}

		private byte ReadByteAtCurrentPosition()
		{
			byte[] byteArrayForCurrentPosition = this.GetByteArrayForCurrentPosition();
			byte result = byteArrayForCurrentPosition[(int)(checked((IntPtr)(this.position % unchecked((long)MultiByteArrayMemoryStream.BufferSize))))];
			this.position += 1L;
			return result;
		}

		private static readonly BufferCache cache = new BufferCache(500);

		private static readonly int BufferSize = BufferCache.OneKiloByteBufferSize;

		private List<BufferCacheEntry> inMemoryBackingStorage = new List<BufferCacheEntry>();

		private long length;

		private long position;
	}
}
