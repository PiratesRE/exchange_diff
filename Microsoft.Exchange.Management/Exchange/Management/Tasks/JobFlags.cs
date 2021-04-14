using System;

namespace Microsoft.Exchange.Management.Tasks
{
	[Flags]
	public enum JobFlags : uint
	{
		None = 0U,
		DetectOnly = 1U,
		Background = 2U,
		OnDemand = 4U,
		System = 8U,
		Force = 16U,
		Verbose = 2147483648U
	}
}
