using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SupportShellLogger : ScomAlertLogger
	{
		public override void TaskStarted(ITaskDescriptor task)
		{
			this.previousValues.Push(base.GetPropertyFeed(task, ContextProperty.AccessMode.Set).ToDictionary((KeyValuePair<ContextProperty, string> pair) => pair.Key, (KeyValuePair<ContextProperty, string> pair) => pair.Value));
			base.TaskStarted(task);
		}

		public override void TaskCompleted(ITaskDescriptor task, TaskResult result)
		{
			base.TaskCompleted(task, result);
			this.previousValues.Pop();
		}

		protected override IEnumerable<KeyValuePair<ContextProperty, string>> GetPropertyFeed(ITaskDescriptor task, ContextProperty.AccessMode forAccessMode)
		{
			IEnumerable<KeyValuePair<ContextProperty, string>> enumerable = base.GetPropertyFeed(task, forAccessMode);
			if (forAccessMode == ContextProperty.AccessMode.Set)
			{
				enumerable = from newValuePair in enumerable
				where SupportShellLogger.outputPropertiesToAlwaysOutput.Contains(newValuePair.Key) || !this.previousValues.Peek().Contains(newValuePair)
				select newValuePair;
			}
			return enumerable;
		}

		protected override bool ShouldLogTask(ITaskDescriptor task)
		{
			return task.TaskType != TaskType.Infrastructure;
		}

		public SupportShellLogger() : base(null)
		{
		}

		private static readonly ContextProperty[] outputPropertiesToAlwaysOutput = new ContextProperty[]
		{
			ContextPropertySchema.TaskStarted,
			ContextPropertySchema.TaskFinished,
			ContextPropertySchema.Latency
		};

		private readonly Stack<IDictionary<ContextProperty, string>> previousValues = new Stack<IDictionary<ContextProperty, string>>();
	}
}
