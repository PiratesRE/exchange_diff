using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Security
{
	internal sealed class SafeLsaMemoryHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		internal SafeLsaMemoryHandle() : base(true)
		{
		}

		internal SafeLsaMemoryHandle(IntPtr handle) : base(true)
		{
			base.SetHandle(handle);
		}

		protected override bool ReleaseHandle()
		{
			return SafeLsaMemoryHandle.LsaFreeMemory(this.handle) == 0;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[DllImport("advapi32.dll", ExactSpelling = true)]
		private static extern int LsaFreeMemory(IntPtr handle);
	}
}
