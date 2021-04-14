using System;
using System.Security;

namespace Microsoft.Win32.SafeHandles
{
	[SecurityCritical]
	internal sealed class SafeViewOfFileHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		[SecurityCritical]
		internal SafeViewOfFileHandle() : base(true)
		{
		}

		[SecurityCritical]
		internal SafeViewOfFileHandle(IntPtr handle, bool ownsHandle) : base(ownsHandle)
		{
			base.SetHandle(handle);
		}

		[SecurityCritical]
		protected override bool ReleaseHandle()
		{
			if (Win32Native.UnmapViewOfFile(this.handle))
			{
				this.handle = IntPtr.Zero;
				return true;
			}
			return false;
		}
	}
}
