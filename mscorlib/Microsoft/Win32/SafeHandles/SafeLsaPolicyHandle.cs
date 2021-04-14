using System;
using System.Security;

namespace Microsoft.Win32.SafeHandles
{
	[SecurityCritical]
	internal sealed class SafeLsaPolicyHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		private SafeLsaPolicyHandle() : base(true)
		{
		}

		internal SafeLsaPolicyHandle(IntPtr handle) : base(true)
		{
			base.SetHandle(handle);
		}

		internal static SafeLsaPolicyHandle InvalidHandle
		{
			get
			{
				return new SafeLsaPolicyHandle(IntPtr.Zero);
			}
		}

		[SecurityCritical]
		protected override bool ReleaseHandle()
		{
			return Win32Native.LsaClose(this.handle) == 0;
		}
	}
}
