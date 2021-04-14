using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Core;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Common
{
	internal class CoconetDecompressor : DisposeTrackableBase
	{
		public CoconetDecompressor(long dictionarySize, CoconetCompressor.LzOption lzOpt)
		{
			this.hDecompressor = CoconetDecompressor.GetDecompressor((ulong)dictionarySize, (uint)lzOpt);
			if (this.hDecompressor == IntPtr.Zero)
			{
				throw new CompressionOutOfMemoryException();
			}
		}

		public CoconetDecompressor() : this(16777216L, CoconetCompressor.LzOption.LzHuf)
		{
		}

		public unsafe void Decompress(byte[] inBuf, int inOffset, int inLength, byte[] outBuf, int outOffset, int expectedDecompressedLength)
		{
			uint maxOutputSize = (uint)expectedDecompressedLength;
			fixed (byte* ptr = inBuf)
			{
				fixed (byte* ptr2 = outBuf)
				{
					int num = CoconetDecompressor.DecompressBufferCoconet(new IntPtr((void*)((byte*)ptr + inOffset)), (uint)inLength, new IntPtr((void*)((byte*)ptr2 + outOffset)), maxOutputSize, ref maxOutputSize, this.hDecompressor);
					if (num != 0)
					{
						throw new DecompressionException(num);
					}
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<CoconetDecompressor>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				IntPtr value = this.hDecompressor;
				this.hDecompressor = IntPtr.Zero;
				if (value != IntPtr.Zero)
				{
					CoconetDecompressor.FreeDecompressor(value);
				}
			}
		}

		[DllImport("coconet.dll")]
		private static extern IntPtr GetDecompressor(ulong dictionarySize, uint lzOpt);

		[DllImport("coconet.dll")]
		private static extern void FreeDecompressor(IntPtr hDecompressor);

		[DllImport("coconet.dll")]
		private static extern int DecompressBufferCoconet(IntPtr pInput, uint inputLen, IntPtr pOutput, uint maxOutputSize, ref uint decompressedLen, IntPtr workSpace);

		private IntPtr hDecompressor = IntPtr.Zero;
	}
}
