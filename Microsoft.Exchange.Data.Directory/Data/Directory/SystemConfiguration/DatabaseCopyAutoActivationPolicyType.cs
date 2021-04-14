using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum DatabaseCopyAutoActivationPolicyType
	{
		[LocDescription(DirectoryStrings.IDs.DatabaseCopyAutoActivationPolicyUnrestricted)]
		Unrestricted,
		[LocDescription(DirectoryStrings.IDs.DatabaseCopyAutoActivationPolicyIntrasiteOnly)]
		IntrasiteOnly,
		[LocDescription(DirectoryStrings.IDs.DatabaseCopyAutoActivationPolicyBlocked)]
		Blocked
	}
}
