using System;

namespace Microsoft.Exchange.Management.Deployment
{
	[Flags]
	public enum ServicePlanSkus : byte
	{
		Datacenter = 1,
		Hosted = 2,
		All = 3
	}
}
