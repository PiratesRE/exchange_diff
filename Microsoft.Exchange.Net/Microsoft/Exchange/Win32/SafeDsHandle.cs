using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Win32
{
	internal sealed class SafeDsHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		internal SafeDsHandle() : base(true)
		{
		}

		internal SafeDsHandle(IntPtr handle, bool ownsHandle) : base(ownsHandle)
		{
			base.SetHandle(handle);
		}

		internal SafeDsHandle(IntPtr handle) : this(handle, true)
		{
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		protected override bool ReleaseHandle()
		{
			return SafeDsHandle.DsUnBind(ref this.handle) == 0U;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[DllImport("NTDSAPI.DLL")]
		private static extern uint DsUnBind([In] ref IntPtr handle);
	}
}
