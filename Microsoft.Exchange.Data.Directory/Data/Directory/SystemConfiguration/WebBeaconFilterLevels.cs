using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum WebBeaconFilterLevels
	{
		[LocDescription(DirectoryStrings.IDs.UserFilterChoice)]
		UserFilterChoice,
		[LocDescription(DirectoryStrings.IDs.ForceFilter)]
		ForceFilter,
		[LocDescription(DirectoryStrings.IDs.DisableFilter)]
		DisableFilter
	}
}
