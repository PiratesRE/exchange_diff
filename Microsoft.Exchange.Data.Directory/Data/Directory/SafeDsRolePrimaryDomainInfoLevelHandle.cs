using System;
using System.Runtime.ConstrainedExecution;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Data.Directory
{
	internal class SafeDsRolePrimaryDomainInfoLevelHandle : SafeHandleZeroIsInvalid
	{
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		protected override bool ReleaseHandle()
		{
			NativeMethods.DsRoleFreeMemory(this.handle);
			return true;
		}
	}
}
