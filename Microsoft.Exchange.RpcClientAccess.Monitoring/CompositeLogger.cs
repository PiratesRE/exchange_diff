using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	internal sealed class CompositeLogger : ILogger, IEnumerable<ILogger>, IEnumerable
	{
		public void Add(ILogger logger)
		{
			ArgumentValidator.ThrowIfNull("logger", logger);
			this.innerLoggers.Add(logger);
		}

		public void TaskStarted(ITaskDescriptor task)
		{
			foreach (ILogger logger in this.innerLoggers)
			{
				logger.TaskStarted(task);
			}
		}

		public void TaskCompleted(ITaskDescriptor task, TaskResult result)
		{
			foreach (ILogger logger in this.innerLoggers)
			{
				logger.TaskCompleted(task, result);
			}
		}

		public IEnumerator<ILogger> GetEnumerator()
		{
			return this.innerLoggers.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private readonly List<ILogger> innerLoggers = new List<ILogger>();
	}
}
