using System;

namespace Microsoft.Exchange.Data
{
	[Flags]
	public enum ErrorPolicies
	{
		[LocDescription(DataStrings.IDs.ErrorPoliciesDefault)]
		Default = 0,
		[LocDescription(DataStrings.IDs.ErrorPoliciesDowngradeDnsFailures)]
		DowngradeDnsFailures = 4,
		[LocDescription(DataStrings.IDs.ErrorPoliciesDowngradeCustomFailures)]
		DowngradeCustomFailures = 8,
		[LocDescription(DataStrings.IDs.ErrorPoliciesUpgradeCustomFailures)]
		UpgradeCustomFailures = 16
	}
}
