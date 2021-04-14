using System;
using System.Security;

namespace Microsoft.Win32.SafeHandles
{
	[SecurityCritical]
	internal sealed class SafeLsaLogonProcessHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		private SafeLsaLogonProcessHandle() : base(true)
		{
		}

		internal SafeLsaLogonProcessHandle(IntPtr handle) : base(true)
		{
			base.SetHandle(handle);
		}

		internal static SafeLsaLogonProcessHandle InvalidHandle
		{
			get
			{
				return new SafeLsaLogonProcessHandle(IntPtr.Zero);
			}
		}

		[SecurityCritical]
		protected override bool ReleaseHandle()
		{
			return Win32Native.LsaDeregisterLogonProcess(this.handle) >= 0;
		}
	}
}
