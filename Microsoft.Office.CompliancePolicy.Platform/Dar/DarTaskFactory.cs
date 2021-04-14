using System;
using System.Collections.Generic;
using Microsoft.Office.CompliancePolicy.ComplianceTask;

namespace Microsoft.Office.CompliancePolicy.Dar
{
	public abstract class DarTaskFactory
	{
		public abstract DarTaskAggregate CreateTaskAggregate(string taskType);

		public virtual DarTask CreateTask(string taskType)
		{
			if (taskType != null)
			{
				if (taskType == "Common.NoOp")
				{
					return new NoOpTask();
				}
				if (taskType == "Common.Iteration")
				{
					return new DarIterationTask();
				}
				if (taskType == "Common.TaskGenerator")
				{
					return new TaskGenerator();
				}
				if (taskType == "Common.Retention")
				{
					return new RetentionTask();
				}
			}
			throw new InvalidOperationException("Unsupported TaskType");
		}

		public IEnumerable<string> GetAllTaskTypes()
		{
			yield return "Common.BindingApplication";
			yield return "Common.NoOp";
			yield return "Common.Iteration";
			yield return "Common.Retention";
			yield return "Common.TaskGenerator";
			foreach (string taskType in this.GetWorkloadSpecificTaskTypes())
			{
				yield return taskType;
			}
			yield break;
		}

		protected virtual IEnumerable<string> GetWorkloadSpecificTaskTypes()
		{
			yield break;
		}
	}
}
