using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Win32
{
	internal sealed class SafeDsNameResultHandle : SafeHandleZeroIsInvalid
	{
		public NativeMethods.DSNameResult GetNameResult()
		{
			if (this.IsInvalid)
			{
				throw new InvalidOperationException("GetNameResult() called on an invalid handle.");
			}
			return (NativeMethods.DSNameResult)Marshal.PtrToStructure(this.handle, typeof(NativeMethods.DSNameResult));
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		protected override bool ReleaseHandle()
		{
			SafeDsNameResultHandle.DsFreeNameResult(this.handle);
			return true;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[DllImport("NTDSAPI.DLL", CharSet = CharSet.Unicode)]
		private static extern void DsFreeNameResult(IntPtr dsNameResultHandle);
	}
}
