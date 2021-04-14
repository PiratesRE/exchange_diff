using System;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Diagnostics
{
	internal sealed class ThreadSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		public ThreadSafeHandle() : this(IntPtr.Zero, true)
		{
		}

		public ThreadSafeHandle(IntPtr handle) : this(handle, true)
		{
		}

		public ThreadSafeHandle(IntPtr handle, bool ownHandle) : base(ownHandle)
		{
			base.SetHandle(handle);
		}

		protected override bool ReleaseHandle()
		{
			return DiagnosticsNativeMethods.CloseHandle(this.handle);
		}
	}
}
