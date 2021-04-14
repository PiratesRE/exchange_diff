using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Security
{
	[StructLayout(LayoutKind.Sequential)]
	internal class EapKeyBlock
	{
		internal EapKeyBlock(byte[] buffer)
		{
			this.rgbKeys = new byte[128];
			Buffer.BlockCopy(buffer, 0, this.rgbKeys, 0, this.rgbKeys.Length);
			this.rgbIVs = new byte[64];
			Buffer.BlockCopy(buffer, this.rgbKeys.Length, this.rgbIVs, 0, this.rgbIVs.Length);
		}

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		public readonly byte[] rgbKeys;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		public readonly byte[] rgbIVs;

		public static readonly int Size = Marshal.SizeOf(typeof(EapKeyBlock));
	}
}
