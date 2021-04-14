using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Security
{
	internal struct CredentialsNames
	{
		internal unsafe CredentialsNames(byte[] memory)
		{
			fixed (IntPtr* ptr = memory)
			{
				IntPtr ptr2 = new IntPtr((void*)ptr);
				using (SafeContextBuffer safeContextBuffer = new SafeContextBuffer(Marshal.ReadIntPtr(ptr2, 0)))
				{
					this.UserName = Marshal.PtrToStringUni(safeContextBuffer.DangerousGetHandle());
				}
			}
		}

		public readonly string UserName;

		public static readonly int Size = Marshal.SizeOf(typeof(IntPtr));
	}
}
