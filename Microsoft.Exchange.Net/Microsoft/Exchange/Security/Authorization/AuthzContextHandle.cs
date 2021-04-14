using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Security.Authorization
{
	[ComVisible(false)]
	[CLSCompliant(false)]
	[SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
	public sealed class AuthzContextHandle : DisposeTrackableSafeHandleZeroOrMinusOneIsInvalid
	{
		private AuthzContextHandle()
		{
		}

		internal AuthzContextHandle(IntPtr authenticatedUserHandle) : base(authenticatedUserHandle)
		{
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<AuthzContextHandle>(this);
		}

		protected override bool ReleaseHandle()
		{
			return this.IsInvalid || NativeMethods.AuthzFreeContext(this.handle);
		}
	}
}
