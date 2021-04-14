using System;

namespace Microsoft.Exchange.Data.Directory
{
	[Flags]
	public enum MobileDeviceProvisioningMasks
	{
		None = 0,
		IsDisabledMask = 1,
		IsManagedMask = 2,
		IsCompliantMask = 4
	}
}
