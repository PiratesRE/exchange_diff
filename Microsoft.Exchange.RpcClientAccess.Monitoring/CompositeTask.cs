using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class CompositeTask : BaseTask
	{
		public CompositeTask(IContext context, params ITask[] tasks) : base(context, Strings.CompositeTaskTitle(tasks.Length), Strings.CompositeTaskDescription(tasks.Length), TaskType.Infrastructure, new ContextProperty[0])
		{
			this.tasks = tasks;
			base.Result = TaskResult.Success;
		}

		protected override IEnumerator<ITask> Process()
		{
			foreach (ITask task in this.tasks)
			{
				Util.ThrowOnNullArgument(task, "task");
				yield return task;
				base.Result = task.Result;
				if (base.Result != TaskResult.Success)
				{
					break;
				}
			}
			yield break;
		}

		private readonly ITask[] tasks;
	}
}
