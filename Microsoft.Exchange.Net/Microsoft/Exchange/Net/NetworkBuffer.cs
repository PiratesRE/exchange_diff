using System;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Net
{
	internal sealed class NetworkBuffer : SafeHandleMinusOneIsInvalid, IDisposeTrackable, IDisposable
	{
		public NetworkBuffer(int bufferSize) : base(true)
		{
			this.disposeTracker = ((IDisposeTrackable)this).GetDisposeTracker();
			this.AllocateBuffer(bufferSize);
		}

		public byte[] Buffer
		{
			get
			{
				return this.buffer;
			}
		}

		public int Consumed
		{
			get
			{
				return this.consumed;
			}
		}

		public int Filled
		{
			get
			{
				return this.filled;
			}
			set
			{
				this.filled = value;
			}
		}

		public int Length
		{
			get
			{
				return this.length;
			}
		}

		public int Remaining
		{
			get
			{
				return this.filled - this.consumed;
			}
		}

		public int EncryptedDataLength
		{
			get
			{
				return this.encryptedBytes;
			}
			set
			{
				this.encryptedBytes = value;
			}
		}

		public int EncryptedDataOffset
		{
			get
			{
				return this.encryptedDataOffset;
			}
			set
			{
				this.encryptedDataOffset = value;
			}
		}

		public int Unused
		{
			get
			{
				if (this.encryptedBytes == 0)
				{
					return this.length - this.filled;
				}
				return this.length - (this.encryptedDataOffset + this.encryptedBytes);
			}
		}

		public int BufferStartOffset
		{
			get
			{
				return this.bufferStartOffset;
			}
		}

		public int DataStartOffset
		{
			get
			{
				return this.bufferStartOffset + this.consumed;
			}
		}

		public int UnusedStartOffset
		{
			get
			{
				if (this.encryptedBytes == 0)
				{
					return this.bufferStartOffset + this.filled;
				}
				return this.bufferStartOffset + (this.encryptedDataOffset + this.encryptedBytes);
			}
		}

		public void ResetFindLineCache()
		{
			this.lastFindLineFind = -1;
			this.lastFindLineStart = -1;
			this.lastFindLineMaxLineLength = -1;
			this.lastFindLineOverflow = false;
		}

		public void ReportBytesFilled(int bytes)
		{
			if (bytes < 0 || bytes > this.length - this.filled)
			{
				throw new ArgumentException(NetException.InvalidNumberOfBytes, "bytes");
			}
			if (this.encryptedBytes != 0)
			{
				throw new InvalidOperationException(NetException.UseReportEncryptedBytesFilled);
			}
			this.filled += bytes;
		}

		public void ReportEncryptedBytesFilled(int bytes)
		{
			if (bytes < 0 || bytes > this.Unused)
			{
				throw new ArgumentException(NetException.InvalidNumberOfBytes, "bytes");
			}
			this.encryptedBytes += bytes;
		}

		public void ExpandBuffer(int newSize)
		{
			int maxBufferSize = NetworkBuffer.BufferFactory.GetMaxBufferSize(this.handle);
			if (newSize > maxBufferSize)
			{
				this.AdjustBuffer(newSize);
				return;
			}
			int optimalBufferSize = NetworkBuffer.BufferFactory.GetOptimalBufferSize(newSize);
			if (optimalBufferSize < maxBufferSize)
			{
				this.AdjustBuffer(optimalBufferSize);
				return;
			}
			this.length = newSize;
		}

		public int FindLine(int maxLineLength, out bool overflow)
		{
			overflow = false;
			int i = this.consumed;
			int num = -1;
			if (this.lastFindLineStart > this.consumed || this.consumed >= this.lastFindLineFind || this.lastFindLineMaxLineLength != maxLineLength)
			{
				while (i < this.filled)
				{
					i = this.IndexOf(10, i);
					if (i == -1)
					{
						if (this.Remaining >= maxLineLength)
						{
							num = maxLineLength;
							overflow = true;
							break;
						}
						break;
					}
					else if (i > this.consumed && this.buffer[this.bufferStartOffset + i - 1] == 13)
					{
						num = i - this.consumed + 1 - 2;
						if (num > maxLineLength)
						{
							num = maxLineLength;
							overflow = true;
							break;
						}
						break;
					}
					else
					{
						if (i - this.consumed + 1 > maxLineLength)
						{
							num = maxLineLength;
							overflow = true;
							break;
						}
						i++;
					}
				}
				this.lastFindLineStart = this.consumed;
				this.lastFindLineFind = ((-1 == num) ? -1 : (this.lastFindLineStart + num));
				this.lastFindLineMaxLineLength = maxLineLength;
				this.lastFindLineOverflow = overflow;
				return num;
			}
			overflow = this.lastFindLineOverflow;
			if (-1 != this.lastFindLineFind)
			{
				return this.lastFindLineFind - this.lastFindLineStart;
			}
			return -1;
		}

		public void ConsumeData(int bytesConsumed)
		{
			if (bytesConsumed < 0 || bytesConsumed > this.Remaining)
			{
				throw new ArgumentException(NetException.InvalidNumberOfBytes, "bytesConsumed");
			}
			this.consumed += bytesConsumed;
		}

		public void PutBackUnconsumedData(int bytesToPutBack)
		{
			if (bytesToPutBack < 0 || bytesToPutBack > this.consumed)
			{
				throw new ArgumentException(NetException.InvalidNumberOfBytes, "bytesToPutBack");
			}
			this.consumed -= bytesToPutBack;
		}

		public void EmptyBuffer()
		{
			this.ResetFindLineCache();
			this.consumed = 0;
			this.filled = 0;
			this.encryptedBytes = 0;
		}

		public void EmptyBufferReservingBytes(int headerBytes)
		{
			this.ResetFindLineCache();
			this.consumed = headerBytes;
			this.filled = headerBytes;
			this.encryptedBytes = 0;
		}

		public void ShuffleBuffer()
		{
			this.ResetFindLineCache();
			if (this.Remaining != 0)
			{
				System.Buffer.BlockCopy(this.buffer, this.DataStartOffset, this.buffer, this.BufferStartOffset, this.Remaining);
			}
			this.filled = this.Remaining;
			this.consumed = 0;
			if (this.encryptedBytes != 0)
			{
				System.Buffer.BlockCopy(this.buffer, this.bufferStartOffset + this.encryptedDataOffset, this.buffer, this.bufferStartOffset + this.filled, this.encryptedBytes);
			}
			this.encryptedDataOffset = this.filled;
		}

		DisposeTracker IDisposeTrackable.GetDisposeTracker()
		{
			return DisposeTracker.Get<NetworkBuffer>(this);
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
			base.Dispose(disposing);
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
			}
		}

		protected override bool ReleaseHandle()
		{
			NetworkBuffer.BufferFactory.Free(this.handle);
			return true;
		}

		private void AdjustBuffer(int newSize)
		{
			IntPtr handle = this.handle;
			byte[] src = this.buffer;
			int srcOffset = this.bufferStartOffset;
			this.AllocateBuffer(newSize);
			System.Buffer.BlockCopy(src, srcOffset, this.buffer, this.bufferStartOffset, this.filled);
			NetworkBuffer.BufferFactory.Free(handle);
		}

		private void AllocateBuffer(int bufferSize)
		{
			this.ResetFindLineCache();
			this.handle = NetworkBuffer.BufferFactory.Allocate(bufferSize, out this.buffer, out this.bufferStartOffset);
			this.length = bufferSize;
		}

		private int IndexOf(byte val, int offset)
		{
			int num = ExBuffer.IndexOf(this.buffer, val, this.bufferStartOffset + offset, this.filled - offset);
			if (num == -1)
			{
				return num;
			}
			return num - this.bufferStartOffset;
		}

		private DisposeTracker disposeTracker;

		private byte[] buffer;

		private int bufferStartOffset;

		private int filled;

		private int consumed;

		private int encryptedBytes;

		private int encryptedDataOffset;

		private int length;

		private int lastFindLineStart;

		private int lastFindLineFind;

		private int lastFindLineMaxLineLength;

		private bool lastFindLineOverflow;

		internal static class BufferFactory
		{
			internal static IntPtr Allocate(int size, out byte[] buffer, out int offset)
			{
				if (size < 0 || size > 262144)
				{
					throw new ArgumentException(NetException.InvalidNumberOfBytes, "size");
				}
				NetworkBuffer.BufferFactory.PoolId poolId;
				int num;
				if (size <= 4096)
				{
					poolId = NetworkBuffer.BufferFactory.PoolId.Small;
					num = NetworkBuffer.BufferFactory.bufferPoolSmall.Alloc(out buffer, out offset);
				}
				else if (size <= 20528)
				{
					poolId = NetworkBuffer.BufferFactory.PoolId.Medium;
					num = NetworkBuffer.BufferFactory.bufferPoolMedium.Alloc(out buffer, out offset);
				}
				else if (size <= 24576)
				{
					poolId = NetworkBuffer.BufferFactory.PoolId.MediumLarge;
					num = NetworkBuffer.BufferFactory.bufferPoolMediumLarge.Alloc(out buffer, out offset);
				}
				else
				{
					poolId = NetworkBuffer.BufferFactory.PoolId.Large;
					num = size;
					offset = 0;
					buffer = new byte[size];
				}
				return (IntPtr)(num << 8 | (int)poolId);
			}

			internal static void Free(IntPtr bufferIndexAndPool)
			{
				NetworkBuffer.BufferFactory.PoolId poolId = (NetworkBuffer.BufferFactory.PoolId)((int)bufferIndexAndPool & 255);
				int bufferToFree = (int)bufferIndexAndPool >> 8;
				switch (poolId)
				{
				case NetworkBuffer.BufferFactory.PoolId.Small:
					NetworkBuffer.BufferFactory.bufferPoolSmall.Free(bufferToFree);
					return;
				case NetworkBuffer.BufferFactory.PoolId.Medium:
					NetworkBuffer.BufferFactory.bufferPoolMedium.Free(bufferToFree);
					return;
				case NetworkBuffer.BufferFactory.PoolId.MediumLarge:
					NetworkBuffer.BufferFactory.bufferPoolMediumLarge.Free(bufferToFree);
					return;
				case NetworkBuffer.BufferFactory.PoolId.Large:
					return;
				default:
					throw new ArgumentException(NetException.InvalidSize, "poolId");
				}
			}

			internal static int GetMaxBufferSize(IntPtr bufferIndexAndPool)
			{
				switch ((int)bufferIndexAndPool & 255)
				{
				case 1:
					return NetworkBuffer.BufferFactory.bufferPoolSmall.MaxBufferSize;
				case 2:
					return NetworkBuffer.BufferFactory.bufferPoolMedium.MaxBufferSize;
				case 3:
					return NetworkBuffer.BufferFactory.bufferPoolMediumLarge.MaxBufferSize;
				case 4:
					return (int)bufferIndexAndPool >> 8;
				default:
					throw new ArgumentException(NetException.InvalidSize, "poolId");
				}
			}

			internal static int GetOptimalBufferSize(int size)
			{
				if (size <= NetworkBuffer.BufferFactory.bufferPoolSmall.MaxBufferSize)
				{
					return NetworkBuffer.BufferFactory.bufferPoolSmall.MaxBufferSize;
				}
				if (size <= NetworkBuffer.BufferFactory.bufferPoolMedium.MaxBufferSize)
				{
					return NetworkBuffer.BufferFactory.bufferPoolMedium.MaxBufferSize;
				}
				if (size <= NetworkBuffer.BufferFactory.bufferPoolMediumLarge.MaxBufferSize)
				{
					return NetworkBuffer.BufferFactory.bufferPoolMediumLarge.MaxBufferSize;
				}
				return size;
			}

			private const int BufferSizeSmall = 4096;

			private const int BufferSizeMedium = 20528;

			private const int BufferSizeMediumLarge = 24576;

			private const int MaxBufferSize = 262144;

			private static BufferManager bufferPoolSmall = new BufferManager(4096, 1048576);

			private static BufferManager bufferPoolMedium = new BufferManager(20528, 1048576);

			private static BufferManager bufferPoolMediumLarge = new BufferManager(24576, 1048576);

			internal enum PoolId
			{
				Small = 1,
				Medium,
				MediumLarge,
				Large
			}
		}
	}
}
