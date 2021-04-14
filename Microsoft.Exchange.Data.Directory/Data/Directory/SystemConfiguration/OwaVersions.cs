using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum OwaVersions
	{
		[LocDescription(DirectoryStrings.IDs.Exchange2003or2000)]
		Exchange2003or2000 = 1,
		[LocDescription(DirectoryStrings.IDs.Exchange2007)]
		Exchange2007,
		[LocDescription(DirectoryStrings.IDs.Exchange2009)]
		Exchange2010,
		[LocDescription(DirectoryStrings.IDs.Exchange2013)]
		Exchange2013
	}
}
