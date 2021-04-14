using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.Win32.SafeHandles
{
	[SecurityCritical]
	public sealed class SafeRegistryHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		[SecurityCritical]
		internal SafeRegistryHandle() : base(true)
		{
		}

		[SecurityCritical]
		public SafeRegistryHandle(IntPtr preexistingHandle, bool ownsHandle) : base(ownsHandle)
		{
			base.SetHandle(preexistingHandle);
		}

		[SecurityCritical]
		protected override bool ReleaseHandle()
		{
			return SafeRegistryHandle.RegCloseKey(this.handle) == 0;
		}

		[SuppressUnmanagedCodeSecurity]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[DllImport("advapi32.dll")]
		internal static extern int RegCloseKey(IntPtr hKey);
	}
}
