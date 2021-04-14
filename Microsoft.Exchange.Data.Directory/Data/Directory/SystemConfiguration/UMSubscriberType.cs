using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum UMSubscriberType
	{
		[LocDescription(DirectoryStrings.IDs.Enterprise)]
		Enterprise,
		[LocDescription(DirectoryStrings.IDs.Consumer)]
		Consumer
	}
}
