using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum GatewayStatus
	{
		[LocDescription(DirectoryStrings.IDs.Enabled)]
		Enabled,
		[LocDescription(DirectoryStrings.IDs.Disabled)]
		Disabled,
		[LocDescription(DirectoryStrings.IDs.NoNewCalls)]
		NoNewCalls
	}
}
