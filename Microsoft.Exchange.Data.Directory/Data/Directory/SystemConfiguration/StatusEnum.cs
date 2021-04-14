using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum StatusEnum
	{
		[LocDescription(DirectoryStrings.IDs.Enabled)]
		Enabled,
		[LocDescription(DirectoryStrings.IDs.Disabled)]
		Disabled
	}
}
