using System;

namespace Microsoft.Exchange.Security
{
	internal sealed class SafeContextHandle : SafeSspiHandle
	{
		protected override bool ReleaseHandle()
		{
			return SspiNativeMethods.DeleteSecurityContext(ref this.SspiHandle) == SecurityStatus.OK;
		}
	}
}
