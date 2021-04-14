using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum BluetoothType
	{
		[LocDescription(DirectoryStrings.IDs.BluetoothDisable)]
		Disable,
		[LocDescription(DirectoryStrings.IDs.BluetoothHandsfreeOnly)]
		HandsfreeOnly,
		[LocDescription(DirectoryStrings.IDs.BluetoothAllow)]
		Allow
	}
}
