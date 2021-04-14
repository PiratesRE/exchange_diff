using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Security
{
	[StructLayout(LayoutKind.Sequential)]
	internal class ConnectionInfo
	{
		internal unsafe ConnectionInfo(byte[] memory)
		{
			fixed (IntPtr* ptr = memory)
			{
				IntPtr ptr2 = new IntPtr((void*)ptr);
				checked
				{
					this.Protocol = (int)((uint)Marshal.ReadInt32(ptr2));
					this.Cipher = (int)((uint)Marshal.ReadInt32(ptr2, 4));
					this.CipherStrength = (int)((uint)Marshal.ReadInt32(ptr2, 8));
					this.MACHashAlgorithm = (int)((uint)Marshal.ReadInt32(ptr2, 12));
					this.MACHashStrength = (int)((uint)Marshal.ReadInt32(ptr2, 16));
					this.KeyExchangeAlgorithm = (int)((uint)Marshal.ReadInt32(ptr2, 20));
					this.KeyExchangeStrength = (int)((uint)Marshal.ReadInt32(ptr2, 24));
				}
			}
		}

		private ConnectionInfo()
		{
		}

		public readonly int Protocol;

		public readonly int Cipher;

		public readonly int CipherStrength;

		public readonly int MACHashAlgorithm;

		public readonly int MACHashStrength;

		public readonly int KeyExchangeAlgorithm;

		public readonly int KeyExchangeStrength;

		public static readonly ConnectionInfo Empty = new ConnectionInfo();

		public static readonly int Size = Marshal.SizeOf(typeof(ConnectionInfo));
	}
}
