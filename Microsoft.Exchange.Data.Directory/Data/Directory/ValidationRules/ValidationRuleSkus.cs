using System;

namespace Microsoft.Exchange.Data.Directory.ValidationRules
{
	[Flags]
	internal enum ValidationRuleSkus : byte
	{
		None = 0,
		Enterprise = 1,
		Datacenter = 2,
		DatacenterTenant = 4,
		Hosted = 8,
		HostedTenant = 16,
		All = 255
	}
}
