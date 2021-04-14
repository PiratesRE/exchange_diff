using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Win32
{
	internal sealed class SafeDomainControllerInfoHandle : SafeHandleZeroIsInvalid
	{
		public NativeMethods.DomainControllerInformation GetDomainControllerInfo()
		{
			if (this.IsInvalid)
			{
				throw new InvalidOperationException("GetDomainControllerInfo() called on an invalid handle.");
			}
			return (NativeMethods.DomainControllerInformation)Marshal.PtrToStructure(this.handle, typeof(NativeMethods.DomainControllerInformation));
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		protected override bool ReleaseHandle()
		{
			return SafeDomainControllerInfoHandle.NetApiBufferFree(this.handle) == 0;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[DllImport("NETAPI32.DLL")]
		private static extern int NetApiBufferFree(IntPtr ptr);
	}
}
