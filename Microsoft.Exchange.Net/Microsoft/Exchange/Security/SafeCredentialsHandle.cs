using System;

namespace Microsoft.Exchange.Security
{
	internal class SafeCredentialsHandle : SafeSspiHandle
	{
		protected override bool ReleaseHandle()
		{
			return SspiNativeMethods.FreeCredentialsHandle(ref this.SspiHandle) == SecurityStatus.OK;
		}
	}
}
