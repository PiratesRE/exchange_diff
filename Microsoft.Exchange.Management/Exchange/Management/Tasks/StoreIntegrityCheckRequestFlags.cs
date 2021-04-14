using System;

namespace Microsoft.Exchange.Management.Tasks
{
	[Flags]
	public enum StoreIntegrityCheckRequestFlags : uint
	{
		None = 0U,
		DetectOnly = 1U,
		Force = 2U,
		SystemJob = 4U,
		Verbose = 2147483648U
	}
}
