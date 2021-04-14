using System;
using System.Threading;

namespace Microsoft.Isam.Esent.Interop
{
	internal sealed class MemoryCache
	{
		public MemoryCache(int bufferSize, int maxCachedBuffers)
		{
			this.bufferSize = bufferSize;
			this.cachedBuffers = new byte[maxCachedBuffers][];
		}

		public int BufferSize
		{
			get
			{
				return this.bufferSize;
			}
		}

		public static byte[] Duplicate(byte[] data, int length)
		{
			if (length == 0)
			{
				return MemoryCache.ZeroLengthArray;
			}
			byte[] array = new byte[length];
			Buffer.BlockCopy(data, 0, array, 0, length);
			return array;
		}

		public byte[] Allocate()
		{
			int startingOffset = this.GetStartingOffset();
			for (int i = 0; i < this.cachedBuffers.Length; i++)
			{
				int num = (i + startingOffset) % this.cachedBuffers.Length;
				byte[] array = Interlocked.Exchange<byte[]>(ref this.cachedBuffers[num], null);
				if (array != null)
				{
					return array;
				}
			}
			return new byte[this.bufferSize];
		}

		public void Free(ref byte[] data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (data.Length != this.bufferSize)
			{
				throw new ArgumentOutOfRangeException("data", data.Length, "buffer is not correct size for this MemoryCache");
			}
			int startingOffset = this.GetStartingOffset();
			for (int i = 0; i < this.cachedBuffers.Length; i++)
			{
				int num = (i + startingOffset) % this.cachedBuffers.Length;
				if (this.cachedBuffers[num] == null)
				{
					this.cachedBuffers[num] = data;
					break;
				}
			}
			data = null;
		}

		private int GetStartingOffset()
		{
			return LibraryHelpers.GetCurrentManagedThreadId() % this.cachedBuffers.Length;
		}

		private static readonly byte[] ZeroLengthArray = new byte[0];

		private readonly int bufferSize;

		private readonly byte[][] cachedBuffers;
	}
}
