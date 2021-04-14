using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Office.CompliancePolicy.Dar
{
	public class NoOpTask : DarTask
	{
		public NoOpTask()
		{
			this.TaskData = new NoOpTaskData
			{
				States = new List<DarTaskExecutionResult>
				{
					DarTaskExecutionResult.Completed
				},
				StateHistory = new List<DarTaskState>()
			};
		}

		public override string TaskType
		{
			get
			{
				return "Common.NoOp";
			}
		}

		[SerializableTaskData]
		public NoOpTaskData TaskData { get; set; }

		public override DarTaskExecutionResult Execute(DarTaskManager darTaskManager)
		{
			darTaskManager.ExecutionLog.LogInformation("NoOpTask", null, this.CorrelationId, string.Format("NoOp Task Executed Id:{0}, Category:{1}, Priority:{2}, Time:{3}", new object[]
			{
				base.Id,
				this.Category,
				base.Priority,
				DateTime.UtcNow
			}), new KeyValuePair<string, object>[0]);
			DarTaskExecutionResult darTaskExecutionResult = this.TaskData.States.First<DarTaskExecutionResult>();
			this.TaskData.States = this.TaskData.States.Skip(1).ToList<DarTaskExecutionResult>();
			darTaskManager.ExecutionLog.LogInformation("NoOpTask", null, this.CorrelationId, string.Format("NoOp Task Completed Id:{0}, Category:{1}, Priority:{2}, Time:{3}, Result:{4}", new object[]
			{
				base.Id,
				this.Category,
				base.Priority,
				DateTime.UtcNow,
				darTaskExecutionResult
			}), new KeyValuePair<string, object>[0]);
			return darTaskExecutionResult;
		}

		public override void CompleteTask(DarTaskManager darTaskManager)
		{
		}

		protected override void OnExecuted(DarTaskState executionState)
		{
			this.TaskData.StateHistory.Add(executionState);
		}

		protected override IEnumerable<Type> GetKnownTypes()
		{
			return base.GetKnownTypes().Concat(new Type[]
			{
				typeof(NoOpTaskData)
			});
		}
	}
}
