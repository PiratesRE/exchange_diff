using System;

namespace Microsoft.Exchange.Data.Directory
{
	internal enum TenantConsistencyMode : byte
	{
		ExpectOnlyLiveTenants,
		IncludeRetiredTenants,
		IgnoreRetiredTenants
	}
}
