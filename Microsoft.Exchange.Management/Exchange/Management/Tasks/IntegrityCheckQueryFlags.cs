using System;

namespace Microsoft.Exchange.Management.Tasks
{
	[Flags]
	public enum IntegrityCheckQueryFlags : uint
	{
		None = 0U,
		QueryJob = 4U
	}
}
