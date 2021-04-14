using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal interface ITaskTimeout
	{
		TimeSpan GetActionTimeout(CostType costType);
	}
}
