using System;
using Microsoft.Exchange.WorkloadManagement;
using Microsoft.Office.CompliancePolicy.Dar;
using Microsoft.Office.CompliancePolicy.Exchange.Dar.Diagnostics;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Office.CompliancePolicy.Exchange.Dar.Execution
{
	internal class TaskWrapper : SystemTaskBase
	{
		public TaskWrapper(DarTask task, DarTaskManager taskManager, SystemWorkloadBase workload, ResourceReservation resourceReservation) : base(workload, resourceReservation)
		{
			if (task == null)
			{
				throw new ArgumentNullException("task");
			}
			if (taskManager == null)
			{
				throw new ArgumentNullException("taskManager");
			}
			this.Task = task;
			this.TaskManager = taskManager;
		}

		public DarTask Task { get; private set; }

		public DarTaskManager TaskManager { get; private set; }

		protected override TaskStepResult Execute()
		{
			try
			{
				GrayException.MapAndReportGrayExceptions(delegate()
				{
					try
					{
						this.ExecuteInternal();
					}
					catch (AggregateException ex2)
					{
						LogItem.Publish("TaskLifeCycle", "TaskFatalErrorRunningAndUpdatingTask", ex2.ToString(), this.Task.CorrelationId, ResultSeverityLevel.Error);
					}
				});
			}
			catch (GrayException ex)
			{
				LogItem.Publish("TaskLifeCycle", "TaskFatalGrayErrorRunningAndUpdatingTask", ex.ToString(), this.Task.CorrelationId, ResultSeverityLevel.Error);
			}
			return TaskStepResult.Complete;
		}

		private void ExecuteInternal()
		{
			try
			{
				ExceptionHandler.Handle(delegate
				{
					this.Task.InvokeTask(this.TaskManager);
				}, new ExceptionGroupHandler(ExceptionGroupHandlers.Unhandled), new ExceptionHandlingOptions
				{
					ClientId = "TaskLifeCycle",
					Operation = "TaskRunning",
					CorrelationId = this.Task.ToString(),
					IsTimeoutEnabled = false
				});
			}
			catch (AggregateException ex)
			{
				string component = "TaskLifeCycle";
				string tag = "TaskFailedWithUnhandledException";
				string correlationId = this.Task.CorrelationId;
				LogItem.Publish(component, tag, ex.ToString(), correlationId, ResultSeverityLevel.Error);
				this.Task.TaskState = DarTaskState.Failed;
				this.TaskManager.UpdateTaskState(this.Task);
			}
		}
	}
}
