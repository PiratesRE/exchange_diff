using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Security
{
	internal class SafeLsaPolicyHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		internal SafeLsaPolicyHandle() : base(true)
		{
		}

		internal SafeLsaPolicyHandle(IntPtr handle) : base(true)
		{
			base.SetHandle(handle);
		}

		protected override bool ReleaseHandle()
		{
			return SafeLsaPolicyHandle.LsaClose(this.handle) == 0;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[DllImport("advapi32.dll", ExactSpelling = true)]
		private static extern int LsaClose(IntPtr handle);
	}
}
