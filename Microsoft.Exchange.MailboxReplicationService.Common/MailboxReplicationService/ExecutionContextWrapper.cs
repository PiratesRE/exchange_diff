using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class ExecutionContextWrapper
	{
		public ExecutionContextWrapper(CommonUtils.UpdateDuration updateDuration, string callName, params DataContext[] additionalContexts)
		{
			this.updateDuration = updateDuration;
			this.contexts.Add(new OperationDataContext(callName, OperationType.None));
			if (additionalContexts != null)
			{
				this.contexts.AddRange(additionalContexts);
			}
			this.callName = callName;
		}

		public void Execute(Action operation, bool measure = true)
		{
			this.wrappedContext = ExecutionContext.Create(this.contexts.ToArray());
			if (measure)
			{
				Stopwatch stopwatch = Stopwatch.StartNew();
				try
				{
					this.wrappedContext.Execute(operation);
					return;
				}
				finally
				{
					this.updateDuration(this.callName, stopwatch.Elapsed);
					stopwatch.Stop();
				}
			}
			this.wrappedContext.Execute(operation);
		}

		public ExecutionContext Create()
		{
			this.wrappedContext = ExecutionContext.Create(this.contexts.ToArray());
			return this.wrappedContext;
		}

		private ExecutionContext wrappedContext;

		private List<DataContext> contexts = new List<DataContext>();

		private readonly string callName;

		private CommonUtils.UpdateDuration updateDuration;
	}
}
