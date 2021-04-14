using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum ServerStatus
	{
		[LocDescription(DirectoryStrings.IDs.Enabled)]
		Enabled,
		[LocDescription(DirectoryStrings.IDs.Disabled)]
		Disabled,
		[LocDescription(DirectoryStrings.IDs.NoNewCalls)]
		NoNewCalls
	}
}
