using System;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	internal interface ILogger
	{
		void TaskStarted(ITaskDescriptor task);

		void TaskCompleted(ITaskDescriptor task, TaskResult result);
	}
}
