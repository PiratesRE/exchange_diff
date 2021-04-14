using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Security
{
	internal class ClientSpecifiedTarget
	{
		internal unsafe ClientSpecifiedTarget(byte[] memory)
		{
			fixed (IntPtr* ptr = memory)
			{
				IntPtr ptr2 = new IntPtr((void*)ptr);
				using (SafeContextBuffer safeContextBuffer = new SafeContextBuffer(Marshal.ReadIntPtr(ptr2, 0)))
				{
					this.target = Marshal.PtrToStringUni(safeContextBuffer.DangerousGetHandle());
				}
			}
		}

		public override string ToString()
		{
			return this.target;
		}

		private readonly string target;
	}
}
