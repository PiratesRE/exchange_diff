using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Security
{
	[StructLayout(LayoutKind.Sequential)]
	internal class StreamSizes
	{
		internal unsafe StreamSizes(byte[] memory)
		{
			fixed (IntPtr* ptr = memory)
			{
				IntPtr ptr2 = new IntPtr((void*)ptr);
				checked
				{
					this.Header = (int)((uint)Marshal.ReadInt32(ptr2));
					this.Trailer = (int)((uint)Marshal.ReadInt32(ptr2, 4));
					this.MaxMessage = (int)((uint)Marshal.ReadInt32(ptr2, 8));
					this.BufferCount = (int)((uint)Marshal.ReadInt32(ptr2, 12));
					this.BlockSize = (int)((uint)Marshal.ReadInt32(ptr2, 16));
				}
			}
		}

		public readonly int Header;

		public readonly int Trailer;

		public readonly int MaxMessage;

		public readonly int BufferCount;

		public readonly int BlockSize;

		public static readonly int Size = Marshal.SizeOf(typeof(StreamSizes));
	}
}
