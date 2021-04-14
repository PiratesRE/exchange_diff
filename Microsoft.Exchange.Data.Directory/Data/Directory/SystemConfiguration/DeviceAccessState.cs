using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum DeviceAccessState
	{
		[LocDescription(DirectoryStrings.IDs.Unknown)]
		Unknown,
		[LocDescription(DirectoryStrings.IDs.Allowed)]
		Allowed,
		[LocDescription(DirectoryStrings.IDs.Blocked)]
		Blocked,
		[LocDescription(DirectoryStrings.IDs.Quarantined)]
		Quarantined,
		[LocDescription(DirectoryStrings.IDs.DeviceDiscovery)]
		DeviceDiscovery
	}
}
