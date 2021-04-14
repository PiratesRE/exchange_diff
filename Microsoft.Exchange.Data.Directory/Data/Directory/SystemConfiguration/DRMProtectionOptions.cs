using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum DRMProtectionOptions
	{
		[LocDescription(DirectoryStrings.IDs.None)]
		None,
		[LocDescription(DirectoryStrings.IDs.Private)]
		Private,
		[LocDescription(DirectoryStrings.IDs.AllUsers)]
		All = -1
	}
}
