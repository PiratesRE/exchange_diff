using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal static class Xpress
	{
		[DllImport("huffman_xpress.dll")]
		private static extern int EcCompressEx(IntPtr pb, int cb, IntPtr pbOut, ref int cbOut);

		[DllImport("huffman_xpress.dll")]
		private static extern int EcDecompressEx(IntPtr pb, int cb, IntPtr pbOut, ref int cbOut);

		public unsafe static bool Compress(byte[] inBuf, int inOffset, int inLength, byte[] outBuf, int outOffset, int maxOutLength, out int compressedSize)
		{
			DiagCore.RetailAssert(outBuf.Length >= outOffset + maxOutLength, "BufferOverflow: Offset({0}) + Len({1}) exceeds buffer length({2})", new object[]
			{
				outOffset,
				maxOutLength,
				outBuf.Length
			});
			compressedSize = maxOutLength;
			fixed (byte* ptr = inBuf)
			{
				fixed (byte* ptr2 = outBuf)
				{
					if (Xpress.EcCompressEx(new IntPtr((void*)((byte*)ptr + inOffset)), inLength, new IntPtr((void*)((byte*)ptr2 + outOffset)), ref compressedSize) == 0 && compressedSize < inLength)
					{
						return true;
					}
				}
			}
			compressedSize = inLength;
			Array.Copy(inBuf, inOffset, outBuf, outOffset, inLength);
			return false;
		}

		public unsafe static bool Decompress(byte[] inBuf, int inOffset, int inLength, byte[] outBuf, int outOffset, int expectedDecompressedLength)
		{
			if (expectedDecompressedLength == inLength)
			{
				Array.Copy(inBuf, inOffset, outBuf, outOffset, inLength);
				return true;
			}
			int num = expectedDecompressedLength;
			fixed (byte* ptr = inBuf)
			{
				fixed (byte* ptr2 = outBuf)
				{
					if (Xpress.EcDecompressEx(new IntPtr((void*)((byte*)ptr + inOffset)), inLength, new IntPtr((void*)((byte*)ptr2 + outOffset)), ref num) == 0)
					{
						bool result;
						if (num != expectedDecompressedLength)
						{
							result = false;
						}
						else
						{
							result = true;
						}
						return result;
					}
				}
			}
			return false;
		}

		public const int MaxXpressBlockSize = 65536;

		public const int MinXpressBlockSize = 265;
	}
}
