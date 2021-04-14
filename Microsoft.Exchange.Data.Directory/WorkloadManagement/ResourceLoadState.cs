using System;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal enum ResourceLoadState : uint
	{
		Unknown,
		Underloaded,
		Full,
		Overloaded,
		Critical
	}
}
