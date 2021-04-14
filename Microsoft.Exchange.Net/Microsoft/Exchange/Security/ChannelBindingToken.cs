using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Security
{
	[StructLayout(LayoutKind.Sequential)]
	internal class ChannelBindingToken
	{
		internal unsafe ChannelBindingToken(byte[] memory)
		{
			fixed (IntPtr* ptr = memory)
			{
				ChannelBindingToken.CBT cbt = (ChannelBindingToken.CBT)Marshal.PtrToStructure(new IntPtr((void*)ptr), typeof(ChannelBindingToken.CBT));
				using (SafeContextBuffer safeContextBuffer = new SafeContextBuffer(cbt.Token))
				{
					if (cbt.Length == 0)
					{
						this.Buffer = new byte[0];
					}
					else
					{
						this.Buffer = new byte[cbt.Length];
						Marshal.Copy(safeContextBuffer.DangerousGetHandle(), this.Buffer, 0, cbt.Length);
					}
				}
			}
		}

		public readonly byte[] Buffer;

		public static readonly int Size = Marshal.SizeOf(typeof(ChannelBindingToken.CBT));

		private struct CBT
		{
			public int Length;

			public IntPtr Token;
		}
	}
}
