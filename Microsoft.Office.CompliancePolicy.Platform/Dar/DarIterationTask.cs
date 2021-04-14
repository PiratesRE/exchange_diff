using System;
using System.Collections.Generic;
using Microsoft.Office.CompliancePolicy.ComplianceData;
using Microsoft.Office.CompliancePolicy.ComplianceTask;

namespace Microsoft.Office.CompliancePolicy.Dar
{
	public class DarIterationTask : ComplianceTask
	{
		[SerializableTaskData]
		public int StartPage { get; set; }

		[SerializableTaskData]
		public int ExecutedPage { get; set; }

		[SerializableTaskData]
		public string Scope { get; set; }

		[SerializableTaskData]
		public int Count { get; set; }

		[SerializableTaskData]
		public string FailReason { get; set; }

		public override string TaskType
		{
			get
			{
				return "Common.Iteration";
			}
		}

		public override DarTaskExecutionResult Execute(DarTaskManager darTaskManager)
		{
			if (this.Scope == null)
			{
				this.FailReason = "Iteration scope is undefined";
				darTaskManager.ExecutionLog.LogError("DarIterator", null, this.CorrelationId, new InvalidOperationException("Iteration task is scheduled for running, but iterating scope has not been specified"), null, new KeyValuePair<string, object>[0]);
				return DarTaskExecutionResult.Failed;
			}
			DarIterator darIterator = DarIterator.Get(this.Scope, base.TenantId, this.ComplianceServiceProvider);
			return darIterator.RunOnNextPage(this, darTaskManager);
		}

		public override void CompleteTask(DarTaskManager darTaskManager)
		{
		}

		public virtual void ProcessCurrent(ComplianceItem complianceItem)
		{
			this.Count++;
		}
	}
}
