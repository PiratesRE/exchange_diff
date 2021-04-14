using System;

namespace Microsoft.Exchange.Data.Directory
{
	internal enum ConfigScopes
	{
		None,
		TenantLocal,
		TenantSubTree,
		Global,
		Server,
		Database,
		AllTenants,
		RootOrg
	}
}
