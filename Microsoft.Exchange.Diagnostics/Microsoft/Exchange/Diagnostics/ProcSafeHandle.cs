using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Diagnostics
{
	internal sealed class ProcSafeHandle : SafeHandle
	{
		public ProcSafeHandle() : this(IntPtr.Zero, true)
		{
		}

		public ProcSafeHandle(IntPtr handle) : this(handle, true)
		{
		}

		public ProcSafeHandle(IntPtr handle, bool ownHandle) : base(IntPtr.Zero, ownHandle)
		{
			base.SetHandle(handle);
		}

		public override bool IsInvalid
		{
			get
			{
				return this.handle == IntPtr.Zero;
			}
		}

		protected override bool ReleaseHandle()
		{
			if (this.handle != (IntPtr)(-1))
			{
				DiagnosticsNativeMethods.CloseHandle(this.handle);
			}
			return true;
		}
	}
}
