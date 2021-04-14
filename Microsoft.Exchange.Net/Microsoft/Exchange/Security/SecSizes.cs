using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Security
{
	[StructLayout(LayoutKind.Sequential)]
	internal class SecSizes
	{
		internal unsafe SecSizes(byte[] memory)
		{
			fixed (IntPtr* ptr = memory)
			{
				IntPtr ptr2 = new IntPtr((void*)ptr);
				checked
				{
					this.MaxToken = (int)((uint)Marshal.ReadInt32(ptr2));
					this.MaxSignature = (int)((uint)Marshal.ReadInt32(ptr2, 4));
					this.BlockSize = (int)((uint)Marshal.ReadInt32(ptr2, 8));
					this.SecurityTrailer = (int)((uint)Marshal.ReadInt32(ptr2, 12));
				}
			}
		}

		public readonly int MaxToken;

		public readonly int MaxSignature;

		public readonly int BlockSize;

		public readonly int SecurityTrailer;

		public static readonly int Size = Marshal.SizeOf(typeof(SecSizes));
	}
}
