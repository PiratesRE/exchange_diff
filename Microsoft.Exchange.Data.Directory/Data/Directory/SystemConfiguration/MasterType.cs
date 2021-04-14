using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum MasterType
	{
		[LocDescription(DirectoryStrings.IDs.DatabaseMasterTypeServer)]
		Server,
		[LocDescription(DirectoryStrings.IDs.DatabaseMasterTypeDag)]
		DatabaseAvailabilityGroup,
		[LocDescription(DirectoryStrings.IDs.DatabaseMasterTypeUnknown)]
		Unknown
	}
}
