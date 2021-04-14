using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum DeviceAccessCharacteristic
	{
		[LocDescription(DirectoryStrings.IDs.DeviceType)]
		DeviceType,
		[LocDescription(DirectoryStrings.IDs.DeviceModel)]
		DeviceModel,
		[LocDescription(DirectoryStrings.IDs.DeviceOS)]
		DeviceOS,
		[LocDescription(DirectoryStrings.IDs.UserAgent)]
		UserAgent,
		[LocDescription(DirectoryStrings.IDs.XMSWLHeader)]
		XMSWLHeader
	}
}
