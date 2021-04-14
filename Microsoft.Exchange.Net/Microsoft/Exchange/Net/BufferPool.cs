using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Net
{
	public class BufferPool
	{
		public BufferPool(int bufferSize) : this(bufferSize, 20, true, false)
		{
		}

		public BufferPool(int bufferSize, int maxBufferCountPerProcessor) : this(bufferSize, maxBufferCountPerProcessor, true, false)
		{
		}

		public BufferPool(int bufferSize, bool cleanBufferOnRelease) : this(bufferSize, 20, cleanBufferOnRelease, false)
		{
		}

		public BufferPool(int bufferSize, int maxBufferCountPerProcessor, bool cleanBufferOnRelease) : this(bufferSize, maxBufferCountPerProcessor, cleanBufferOnRelease, false)
		{
		}

		public BufferPool(int bufferSize, int maxBufferCountPerProcessor, bool cleanBufferOnRelease, bool enablePoolSharing)
		{
			if (bufferSize > 1048576)
			{
				throw new ArgumentOutOfRangeException("bufferSize", string.Format(CultureInfo.InvariantCulture, NetException.LargeBuffer, new object[]
				{
					bufferSize,
					1048576
				}));
			}
			if (bufferSize < 256)
			{
				throw new ArgumentOutOfRangeException("bufferSize", string.Format(CultureInfo.InvariantCulture, NetException.SmallBuffer, new object[]
				{
					bufferSize,
					256
				}));
			}
			if (maxBufferCountPerProcessor < 1)
			{
				throw new ArgumentOutOfRangeException("maxBufferCountPerProcessor", string.Format(CultureInfo.InvariantCulture, NetException.InvalidMaxBufferCount, new object[0]));
			}
			this.countLimit = Math.Min(maxBufferCountPerProcessor, 4194304 / bufferSize);
			this.bufferSize = bufferSize;
			this.cleanBufferOnRelease = cleanBufferOnRelease;
			this.enablePoolSharing = (bufferSize >= 92160 || enablePoolSharing);
		}

		public static bool EnableReleaseTracking
		{
			get
			{
				return BufferPool.enableReleaseTracking;
			}
			set
			{
				BufferPool.enableReleaseTracking = value;
			}
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
			int currentProcessor = BufferPool.NativeMethods.CurrentProcessor;
			Stack<byte[]> stack = this.GetPool(currentProcessor);
			lock (stack)
			{
				if (stack.Count > 0)
				{
					return stack.Pop();
				}
			}
			if (this.enablePoolSharing)
			{
				for (int i = 1; i < this.pool.Length; i++)
				{
					stack = this.GetPool(currentProcessor + i);
					lock (stack)
					{
						if (stack.Count > 0)
						{
							return stack.Pop();
						}
					}
				}
			}
			return new byte[this.bufferSize];
		}

		public void Release(byte[] buffer)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (buffer.Length != this.bufferSize)
			{
				throw new ArgumentException(NetException.BufferMismatch, "buffer");
			}
			if (this.cleanBufferOnRelease)
			{
				Array.Clear(buffer, 0, this.bufferSize);
			}
			if (BufferPool.EnableReleaseTracking)
			{
				lock (this)
				{
					if (this.recentRelease == null)
					{
						this.recentRelease = new Queue<byte[]>(1024);
					}
					if (this.recentReleaseStacks == null)
					{
						this.recentReleaseStacks = new Dictionary<byte[], StackTrace>();
					}
					if (this.recentRelease.Contains(buffer))
					{
						throw new InvalidOperationException("Buffer is released twice! Originally released\n" + this.recentReleaseStacks[buffer].ToString() + "\nSecond release is\n" + new StackTrace(true).ToString());
					}
					this.recentRelease.Enqueue(buffer);
					this.recentReleaseStacks.Add(buffer, new StackTrace(true));
					if (this.recentRelease.Count <= 1000)
					{
						return;
					}
					buffer = this.recentRelease.Dequeue();
					this.recentReleaseStacks.Remove(buffer);
				}
			}
			int currentProcessor = BufferPool.NativeMethods.CurrentProcessor;
			Stack<byte[]> stack = this.GetPool(currentProcessor);
			if (stack.Count < this.countLimit)
			{
				lock (stack)
				{
					if (stack.Count < this.countLimit)
					{
						stack.Push(buffer);
						buffer = null;
					}
				}
			}
			if (buffer != null && this.enablePoolSharing)
			{
				for (int i = 1; i < this.pool.Length; i++)
				{
					stack = this.GetPool(currentProcessor + i);
					lock (stack)
					{
						if (stack.Count < this.countLimit)
						{
							stack.Push(buffer);
							buffer = null;
							break;
						}
					}
				}
			}
		}

		private Stack<byte[]> GetPool(int index)
		{
			index %= this.pool.Length;
			if (this.pool[index] == null)
			{
				lock (this.initializationLock)
				{
					if (this.pool[index] == null)
					{
						this.pool[index] = new Stack<byte[]>(this.countLimit);
					}
				}
			}
			return this.pool[index];
		}

		public const int MinBufferSize = 256;

		public const int MaxBufferSize = 1048576;

		public const int DefaultMaxStackDepth = 20;

		private const int MinBufferSizeToUseSharing = 92160;

		private static bool enableReleaseTracking;

		private readonly int countLimit;

		private readonly int bufferSize;

		private readonly bool cleanBufferOnRelease;

		private readonly bool enablePoolSharing;

		private readonly object initializationLock = new object();

		private Stack<byte[]>[] pool = new Stack<byte[]>[4];

		private Queue<byte[]> recentRelease;

		private Dictionary<byte[], StackTrace> recentReleaseStacks;

		internal static class NativeMethods
		{
			internal static int CurrentProcessor
			{
				get
				{
					return (int)BufferPool.NativeMethods.GetCurrentProcessorNumber();
				}
			}

			[DllImport("kernel32.dll")]
			private static extern uint GetCurrentProcessorNumber();
		}
	}
}
