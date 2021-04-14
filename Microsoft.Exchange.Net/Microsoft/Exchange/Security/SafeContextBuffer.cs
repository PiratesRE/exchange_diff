using System;

namespace Microsoft.Exchange.Security
{
	internal sealed class SafeContextBuffer : DebugSafeHandle
	{
		internal SafeContextBuffer()
		{
		}

		internal SafeContextBuffer(IntPtr handle)
		{
			base.SetHandle(handle);
		}

		protected override bool ReleaseHandle()
		{
			return SspiNativeMethods.FreeContextBuffer(this.handle) == SecurityStatus.OK;
		}
	}
}
