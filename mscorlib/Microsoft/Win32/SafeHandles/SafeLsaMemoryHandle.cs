using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.Win32.SafeHandles
{
	[SecurityCritical]
	internal sealed class SafeLsaMemoryHandle : SafeBuffer
	{
		private SafeLsaMemoryHandle() : base(true)
		{
		}

		internal SafeLsaMemoryHandle(IntPtr handle) : base(true)
		{
			base.SetHandle(handle);
		}

		internal static SafeLsaMemoryHandle InvalidHandle
		{
			get
			{
				return new SafeLsaMemoryHandle(IntPtr.Zero);
			}
		}

		[SecurityCritical]
		protected override bool ReleaseHandle()
		{
			return Win32Native.LsaFreeMemory(this.handle) == 0;
		}
	}
}
