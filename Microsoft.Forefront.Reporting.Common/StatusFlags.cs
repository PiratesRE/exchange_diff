using System;

namespace Microsoft.Forefront.Reporting.Common
{
	[Flags]
	public enum StatusFlags : uint
	{
		Unknown = 0U,
		Receive = 1U,
		Defer = 2U,
		Expand = 4U,
		Send = 8U,
		Deliver = 16U,
		Fail = 32U,
		Resolve = 64U
	}
}
