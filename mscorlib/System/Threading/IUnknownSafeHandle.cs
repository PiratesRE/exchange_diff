using System;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Threading
{
	[SecurityCritical]
	internal class IUnknownSafeHandle : SafeHandle
	{
		public IUnknownSafeHandle() : base(IntPtr.Zero, true)
		{
		}

		public override bool IsInvalid
		{
			[SecurityCritical]
			get
			{
				return this.handle == IntPtr.Zero;
			}
		}

		[SecurityCritical]
		protected override bool ReleaseHandle()
		{
			HostExecutionContextManager.ReleaseHostSecurityContext(this.handle);
			return true;
		}

		internal object Clone()
		{
			IUnknownSafeHandle unknownSafeHandle = new IUnknownSafeHandle();
			if (!this.IsInvalid)
			{
				HostExecutionContextManager.CloneHostSecurityContext(this, unknownSafeHandle);
			}
			return unknownSafeHandle;
		}
	}
}
