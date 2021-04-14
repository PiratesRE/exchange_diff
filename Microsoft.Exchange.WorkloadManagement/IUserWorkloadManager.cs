using System;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal interface IUserWorkloadManager
	{
		bool TrySubmitNewTask(ITask task);
	}
}
