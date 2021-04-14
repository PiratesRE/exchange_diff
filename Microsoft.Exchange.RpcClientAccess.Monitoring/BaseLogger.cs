using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	internal abstract class BaseLogger : ILogger
	{
		public BaseLogger()
		{
		}

		public event Action<LocalizedString> LogOutput;

		public abstract void TaskStarted(ITaskDescriptor task);

		public abstract void TaskCompleted(ITaskDescriptor task, TaskResult result);

		protected virtual void OnLogOutput(LocalizedString output)
		{
			if (this.LogOutput != null)
			{
				this.LogOutput(output);
			}
		}
	}
}
