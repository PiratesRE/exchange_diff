using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Flags]
	internal enum ThrottlingPolicyFlags
	{
		None = 0,
		IsServiceAccount = 1,
		OrganizationScope = 2,
		GlobalScope = 4
	}
}
