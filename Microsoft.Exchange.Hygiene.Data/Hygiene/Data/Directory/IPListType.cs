using System;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	public enum IPListType
	{
		Outbound = 4,
		Gateway,
		TenantAllowList = 10,
		TenantBlockList = 14
	}
}
