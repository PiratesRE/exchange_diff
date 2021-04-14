using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum HygieneSuiteEnum
	{
		[LocDescription(DirectoryStrings.IDs.HygieneSuiteStandard)]
		Standard,
		[LocDescription(DirectoryStrings.IDs.HygieneSuitePremium)]
		Premium
	}
}
