using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum DeviceAccessLevel
	{
		[LocDescription(DirectoryStrings.IDs.AccessGranted)]
		Allow,
		[LocDescription(DirectoryStrings.IDs.AccessDenied)]
		Block,
		[LocDescription(DirectoryStrings.IDs.AccessQuarantined)]
		Quarantine
	}
}
