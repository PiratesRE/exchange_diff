using System;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal interface ITaskProvider : IDisposable
	{
		SystemTaskBase GetNextTask();
	}
}
