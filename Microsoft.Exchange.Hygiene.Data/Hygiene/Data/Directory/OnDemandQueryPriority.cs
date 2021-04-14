using System;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	internal enum OnDemandQueryPriority
	{
		Urgent = 100,
		High = 500,
		Normal = 1000,
		Low = 2000
	}
}
