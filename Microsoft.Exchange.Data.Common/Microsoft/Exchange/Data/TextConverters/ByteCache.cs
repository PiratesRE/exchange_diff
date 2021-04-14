using System;

namespace Microsoft.Exchange.Data.TextConverters
{
	internal class ByteCache
	{
		public int Length
		{
			get
			{
				return this.cachedLength;
			}
		}

		public void Reset()
		{
			while (this.headEntry != null)
			{
				this.headEntry.Reset();
				ByteCache.CacheEntry cacheEntry = this.headEntry;
				this.headEntry = this.headEntry.Next;
				if (this.headEntry == null)
				{
					this.tailEntry = null;
				}
				cacheEntry.Next = this.freeList;
				this.freeList = cacheEntry;
			}
			this.cachedLength = 0;
		}

		public void GetBuffer(int size, out byte[] buffer, out int offset)
		{
			if (this.tailEntry != null && this.tailEntry.GetBuffer(size, out buffer, out offset))
			{
				return;
			}
			this.AllocateTail(size);
			this.tailEntry.GetBuffer(size, out buffer, out offset);
		}

		public void Commit(int count)
		{
			this.tailEntry.Commit(count);
			this.cachedLength += count;
		}

		public void GetData(out byte[] outputBuffer, out int outputOffset, out int outputCount)
		{
			this.headEntry.GetData(out outputBuffer, out outputOffset, out outputCount);
		}

		public void ReportRead(int count)
		{
			this.headEntry.ReportRead(count);
			this.cachedLength -= count;
			if (this.headEntry.Length == 0)
			{
				ByteCache.CacheEntry cacheEntry = this.headEntry;
				this.headEntry = this.headEntry.Next;
				if (this.headEntry == null)
				{
					this.tailEntry = null;
				}
				cacheEntry.Next = this.freeList;
				this.freeList = cacheEntry;
			}
		}

		public int Read(byte[] buffer, int offset, int count)
		{
			int num = 0;
			while (count != 0)
			{
				int num2 = this.headEntry.Read(buffer, offset, count);
				offset += num2;
				count -= num2;
				num += num2;
				this.cachedLength -= num2;
				if (this.headEntry.Length == 0)
				{
					ByteCache.CacheEntry cacheEntry = this.headEntry;
					this.headEntry = this.headEntry.Next;
					if (this.headEntry == null)
					{
						this.tailEntry = null;
					}
					cacheEntry.Next = this.freeList;
					this.freeList = cacheEntry;
				}
				if (count == 0 || this.headEntry == null)
				{
					break;
				}
			}
			return num;
		}

		private void AllocateTail(int size)
		{
			ByteCache.CacheEntry cacheEntry = this.freeList;
			if (cacheEntry != null)
			{
				this.freeList = cacheEntry.Next;
				cacheEntry.Next = null;
			}
			else
			{
				cacheEntry = new ByteCache.CacheEntry(size);
			}
			if (this.tailEntry != null)
			{
				this.tailEntry.Next = cacheEntry;
			}
			else
			{
				this.headEntry = cacheEntry;
			}
			this.tailEntry = cacheEntry;
		}

		private int cachedLength;

		private ByteCache.CacheEntry headEntry;

		private ByteCache.CacheEntry tailEntry;

		private ByteCache.CacheEntry freeList;

		internal class CacheEntry
		{
			public CacheEntry(int size)
			{
				this.AllocateBuffer(size);
			}

			public int Length
			{
				get
				{
					return this.count;
				}
			}

			public ByteCache.CacheEntry Next
			{
				get
				{
					return this.next;
				}
				set
				{
					this.next = value;
				}
			}

			public void Reset()
			{
				this.count = 0;
			}

			public bool GetBuffer(int size, out byte[] buffer, out int offset)
			{
				if (this.count == 0)
				{
					this.offset = 0;
					if (this.buffer.Length < size)
					{
						this.AllocateBuffer(size);
					}
				}
				if (this.buffer.Length - (this.offset + this.count) >= size)
				{
					buffer = this.buffer;
					offset = this.offset + this.count;
					return true;
				}
				if (this.count < 64 && this.buffer.Length - this.count >= size)
				{
					Buffer.BlockCopy(this.buffer, this.offset, this.buffer, 0, this.count);
					this.offset = 0;
					buffer = this.buffer;
					offset = this.offset + this.count;
					return true;
				}
				buffer = null;
				offset = 0;
				return false;
			}

			public void Commit(int count)
			{
				this.count += count;
			}

			public void GetData(out byte[] outputBuffer, out int outputOffset, out int outputCount)
			{
				outputBuffer = this.buffer;
				outputOffset = this.offset;
				outputCount = this.count;
			}

			public void ReportRead(int count)
			{
				this.offset += count;
				this.count -= count;
			}

			public int Read(byte[] buffer, int offset, int count)
			{
				int num = Math.Min(count, this.count);
				Buffer.BlockCopy(this.buffer, this.offset, buffer, offset, num);
				this.count -= num;
				this.offset += num;
				count -= num;
				offset += num;
				return num;
			}

			private void AllocateBuffer(int size)
			{
				if (size < 2048)
				{
					size = 2048;
				}
				size = (size * 2 + 1023) / 1024 * 1024;
				this.buffer = new byte[size];
			}

			private const int DefaultMaxLength = 4096;

			private byte[] buffer;

			private int count;

			private int offset;

			private ByteCache.CacheEntry next;
		}
	}
}
