using System;
using System.Collections;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;

namespace Microsoft.Exchange.Net
{
	internal sealed class BufferManager
	{
		public BufferManager(int bufferSize, int fragmentSize)
		{
			if (bufferSize % 8 != 0)
			{
				throw new ArgumentException(NetException.MultipleOfAlignmentFactor, "bufferSize");
			}
			this.bufferSize = bufferSize;
			fragmentSize = Math.Max(fragmentSize, 131072);
			this.buffersPerFragment = (fragmentSize - 4) / (8 + bufferSize) + 1;
			this.fragmentSize = 4 + this.buffersPerFragment * (8 + bufferSize);
			this.freePool = new Queue(this.buffersPerFragment);
		}

		public int MaxBufferSize
		{
			get
			{
				return this.bufferSize;
			}
		}

		public int Alloc(out byte[] fragment, out int offset)
		{
			int num;
			lock (this.freePool)
			{
				if (this.freePool.Count == 0)
				{
					this.AllocateNewFragment();
				}
				num = (int)this.freePool.Dequeue();
			}
			this.GetBufferAndOffset(num, out fragment, out offset);
			this.CheckBuffer(fragment, offset, 252);
			this.ApplyBufferSignature(fragment, offset, 172);
			return num;
		}

		public void Free(int bufferToFree)
		{
			if (bufferToFree == -1)
			{
				throw new ArgumentOutOfRangeException("bufferToFree");
			}
			byte[] array;
			int num;
			this.GetBufferAndOffset(bufferToFree, out array, out num);
			this.CheckBuffer(array, num, 172);
			this.ApplyBufferSignature(array, num, 252);
			lock (this.freePool)
			{
				this.freePool.Enqueue(bufferToFree);
			}
		}

		public void GetBufferAndOffset(int bufferIndex, out byte[] buffer, out int offset)
		{
			buffer = this.fragmentList[bufferIndex / this.buffersPerFragment];
			int num = bufferIndex % this.buffersPerFragment;
			offset = 8 + num * (8 + this.bufferSize);
		}

		public void CheckBuffer(byte[] fragment, int offsetToBuffer, byte signatureByte)
		{
			if (offsetToBuffer < 8 || offsetToBuffer > this.fragmentSize - 8 - 4 || offsetToBuffer % 8 != 0)
			{
				throw new ArgumentException(NetException.BadOffset, "offsetToBuffer");
			}
		}

		private void AllocateNewFragment()
		{
			int num = this.FindNextAvailableFragment();
			BufferManager.ApplyFragmentSignature(this.fragmentList[num]);
			int num2 = 8;
			for (int i = 0; i < this.buffersPerFragment; i++)
			{
				this.freePool.Enqueue(num * this.buffersPerFragment + i);
				this.ApplyBufferSignature(this.fragmentList[num], num2, 252);
				num2 += this.bufferSize + 8;
			}
			if (this.freePool.Count == 0)
			{
				throw new InvalidOperationException(NetException.CouldNotAllocateFragment);
			}
		}

		private int FindNextAvailableFragment()
		{
			int i;
			for (i = 0; i < this.fragmentList.Length; i++)
			{
				if (this.fragmentList[i] == null)
				{
					this.fragmentList[i] = new byte[this.fragmentSize];
					return i;
				}
			}
			byte[][] destinationArray = new byte[this.fragmentList.Length + 16][];
			Array.Copy(this.fragmentList, destinationArray, this.fragmentList.Length);
			i = this.fragmentList.Length;
			this.fragmentList = destinationArray;
			this.fragmentList[i] = new byte[this.fragmentSize];
			return i;
		}

		private static void ApplyFragmentSignature(byte[] fragment)
		{
			for (int i = 0; i < 4; i++)
			{
				fragment[i] = BufferManager.FragmentSignature[i];
			}
		}

		private void ApplyBufferSignature(byte[] buffer, int offset, byte signature)
		{
			for (int i = 0; i < 4; i++)
			{
				buffer[offset - 4 + i] = signature;
				buffer[offset + this.bufferSize + i] = signature;
			}
		}

		[Conditional("DEBUG")]
		private static void VerifyFragmentSignature(byte[] buffer)
		{
			for (int i = 0; i < 4; i++)
			{
				if (buffer[i] != BufferManager.FragmentSignature[i])
				{
					ExTraceGlobals.NetworkTracer.Information<byte, byte, int>(0L, "Fragment signature does not match. Found {0:x}, expected {1:x} at location {2}", buffer[i], BufferManager.FragmentSignature[i], i);
					throw new InvalidOperationException(NetException.SignatureDoesNotMatch);
				}
			}
		}

		[Conditional("DEBUG")]
		private static void VerifySignature(byte[] buffer, int offset, int length, byte signature)
		{
			for (int i = 0; i < length; i++)
			{
				if (buffer[offset + i] != signature)
				{
					ExTraceGlobals.NetworkTracer.Information(0L, "Signature does not match. Found 0x{0:x}, expected 0x{1:x} at location {2}+{3}", new object[]
					{
						buffer[offset + i],
						signature,
						offset,
						i
					});
					throw new InvalidOperationException(NetException.SignatureDoesNotMatch);
				}
			}
		}

		public const int InvalidBufferIndex = -1;

		private const int MinFragmentSize = 131072;

		private const int FragmentListGrowFactor = 16;

		private const int AlignmentFactor = 8;

		private const int SignatureLength = 4;

		private const byte GuardByteInUse = 172;

		private const byte GuardByteFree = 252;

		private readonly int bufferSize;

		private readonly int fragmentSize;

		private readonly int buffersPerFragment;

		private static readonly byte[] FragmentSignature = new byte[]
		{
			67,
			67,
			13,
			10
		};

		private byte[][] fragmentList = new byte[0][];

		private Queue freePool;
	}
}
