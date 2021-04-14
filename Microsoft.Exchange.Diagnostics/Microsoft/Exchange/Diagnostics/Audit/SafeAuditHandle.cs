using System;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Diagnostics.Audit
{
	internal sealed class SafeAuditHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		private SafeAuditHandle() : base(true)
		{
		}

		protected override bool ReleaseHandle()
		{
			return NativeMethods.AuthzUnregisterSecurityEventSource(0U, ref this.handle);
		}
	}
}
