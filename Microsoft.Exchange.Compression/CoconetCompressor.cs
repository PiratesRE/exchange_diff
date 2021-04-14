using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Core;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Common
{
	internal class CoconetCompressor : DisposeTrackableBase
	{
		internal CoconetCompressor(long dictionarySize, int sampleRate, CoconetCompressor.LzOption lzOpt)
		{
			this.hCompressor = CoconetCompressor.GetCompressor((ulong)dictionarySize, (uint)sampleRate, (uint)lzOpt);
			if (this.hCompressor == IntPtr.Zero)
			{
				throw new CompressionOutOfMemoryException();
			}
		}

		internal CoconetCompressor() : this(16777216L, 8, CoconetCompressor.LzOption.LzHuf)
		{
		}

		internal unsafe void Compress(byte[] inBuf, int inOffset, int inLength, byte[] outBuf, int outOffset, int maxOutLength, out int compressedLength)
		{
			compressedLength = 0;
			uint num = 0U;
			fixed (byte* ptr = inBuf)
			{
				fixed (byte* ptr2 = outBuf)
				{
					int num2 = CoconetCompressor.CompressBufferCoconet(new IntPtr((void*)((byte*)ptr + inOffset)), (uint)inLength, new IntPtr((void*)((byte*)ptr2 + outOffset)), (uint)maxOutLength, ref num, this.hCompressor);
					if (num2 != 0)
					{
						throw new CompressionException(num2);
					}
				}
			}
			compressedLength = (int)num;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<CoconetCompressor>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				IntPtr value = this.hCompressor;
				this.hCompressor = IntPtr.Zero;
				if (value != IntPtr.Zero)
				{
					CoconetCompressor.FreeCompressor(value);
				}
			}
		}

		[DllImport("coconet.dll")]
		private static extern IntPtr GetCompressor(ulong dictionarySize, uint sampleRate, uint lzOpt);

		[DllImport("coconet.dll")]
		private static extern void FreeCompressor(IntPtr hCompressor);

		[DllImport("coconet.dll")]
		private static extern int CompressBufferCoconet(IntPtr pInput, uint inputLen, IntPtr pOutput, uint maxOutputSize, ref uint compressedLen, IntPtr workSpace);

		public const long DefaultDictionarySize = 16777216L;

		public const int DefaultSampleRate = 8;

		private IntPtr hCompressor = IntPtr.Zero;

		public enum LzOption
		{
			LzNone = 1,
			LzLz,
			LzHuf,
			LzMax,
			LzHufMax,
			LzDefault = 3
		}

		public enum StatusCode
		{
			OK,
			STATUS_BUFFER_TOO_SMALL = 35,
			STATUS_BAD_COMPRESSION_BUFFER = 578
		}
	}
}
