using System;

namespace Microsoft.Exchange.Management.Tasks
{
	public enum DeviceRemoteWipeStatus
	{
		[LocDescription(Strings.IDs.DeviceOk)]
		DeviceOk,
		[LocDescription(Strings.IDs.DeviceBlocked)]
		DeviceBlocked,
		[LocDescription(Strings.IDs.DeviceWipePending)]
		DeviceWipePending,
		[LocDescription(Strings.IDs.DeviceWipeSucceeded)]
		DeviceWipeSucceeded
	}
}
