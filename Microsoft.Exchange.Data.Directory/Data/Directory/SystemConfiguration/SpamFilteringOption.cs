using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum SpamFilteringOption
	{
		[LocDescription(DirectoryStrings.IDs.SpamFilteringOptionOff)]
		Off,
		[LocDescription(DirectoryStrings.IDs.SpamFilteringOptionOn)]
		On,
		[LocDescription(DirectoryStrings.IDs.SpamFilteringOptionTest)]
		Test
	}
}
