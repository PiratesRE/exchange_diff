using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.Win32.SafeHandles
{
	[SecurityCritical]
	internal sealed class SafeLsaReturnBufferHandle : SafeBuffer
	{
		private SafeLsaReturnBufferHandle() : base(true)
		{
		}

		internal SafeLsaReturnBufferHandle(IntPtr handle) : base(true)
		{
			base.SetHandle(handle);
		}

		internal static SafeLsaReturnBufferHandle InvalidHandle
		{
			get
			{
				return new SafeLsaReturnBufferHandle(IntPtr.Zero);
			}
		}

		[SecurityCritical]
		protected override bool ReleaseHandle()
		{
			return Win32Native.LsaFreeReturnBuffer(this.handle) >= 0;
		}
	}
}
