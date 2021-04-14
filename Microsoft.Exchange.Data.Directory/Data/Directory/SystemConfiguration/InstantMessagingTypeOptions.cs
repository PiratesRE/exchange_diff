using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum InstantMessagingTypeOptions
	{
		[LocDescription(DirectoryStrings.IDs.None)]
		None,
		[LocDescription(DirectoryStrings.IDs.Ocs)]
		Ocs,
		[LocDescription(DirectoryStrings.IDs.Msn)]
		Msn
	}
}
