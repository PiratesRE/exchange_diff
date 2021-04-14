using System;
using System.Collections.Generic;
using Microsoft.Office.CompliancePolicy.Dar;

namespace Microsoft.Office.CompliancePolicy.ComplianceTask
{
	public class TaskGenerator : ComplianceTask
	{
		public TaskGenerator()
		{
			this.Category = DarTaskCategory.High;
			base.TaskRetryTotalCount = 20;
			base.TaskRetryInterval = new TimeSpan(0, 0, 10);
		}

		public override string TaskType
		{
			get
			{
				return "Common.TaskGenerator";
			}
		}

		public static DateTime CalculateNextScheduledTime(DateTime currenScheduledTime, RecurrenceFrequency frequency, int recurrenceInterval)
		{
			switch (frequency)
			{
			case RecurrenceFrequency.Minute:
				return currenScheduledTime + TimeSpan.FromMinutes((double)recurrenceInterval);
			case RecurrenceFrequency.Hour:
				return currenScheduledTime + TimeSpan.FromHours((double)recurrenceInterval);
			case RecurrenceFrequency.Day:
				return currenScheduledTime + TimeSpan.FromDays((double)recurrenceInterval);
			default:
				return DateTime.MaxValue;
			}
		}

		public static DateTime CalculatePreviousScheduledTime(DateTime currenScheduledTime, RecurrenceFrequency frequency, int recurrenceInterval)
		{
			switch (frequency)
			{
			case RecurrenceFrequency.Minute:
				return currenScheduledTime - TimeSpan.FromMinutes((double)recurrenceInterval);
			case RecurrenceFrequency.Hour:
				return currenScheduledTime - TimeSpan.FromHours((double)recurrenceInterval);
			case RecurrenceFrequency.Day:
				return currenScheduledTime - TimeSpan.FromDays((double)recurrenceInterval);
			default:
				return DateTime.MinValue;
			}
		}

		public override DarTaskExecutionResult Execute(DarTaskManager darTaskManager)
		{
			DarTaskExecutionResult result;
			try
			{
				DarTaskAggregate darTaskAggregate = darTaskManager.ServiceProvider.DarTaskAggregateProvider.Find(base.TenantId, "Common.TaskGenerator");
				darTaskManager.ServiceProvider.ExecutionLog.LogInformation("TaskGenerator", null, this.CorrelationId, "TaskGenerator: Beginning Task Generation", new KeyValuePair<string, object>[]
				{
					new KeyValuePair<string, object>("Tag", DarExecutionLogClientIDs.TaskGenerator1.ToString())
				});
				if (darTaskAggregate == null)
				{
					darTaskManager.ServiceProvider.ExecutionLog.LogError("TaskGenerator", null, this.CorrelationId, null, "TaskGenerator: Cannot find DarTaskAggregate for Task Generator. Cannot generate tasks", new KeyValuePair<string, object>[]
					{
						new KeyValuePair<string, object>("Tag", DarExecutionLogClientIDs.TaskGenerator2.ToString())
					});
					result = DarTaskExecutionResult.Failed;
				}
				else
				{
					IEnumerable<DarTaskAggregate> enumerable = darTaskManager.ServiceProvider.DarTaskAggregateProvider.FindAll(base.TenantId);
					IEnumerable<DarTask> lastScheduledTasks = darTaskManager.ServiceProvider.DarTaskQueue.GetLastScheduledTasks(base.TenantId);
					Dictionary<string, DateTime> dictionary = new Dictionary<string, DateTime>();
					foreach (DarTask darTask in lastScheduledTasks)
					{
						dictionary.Add(darTask.TaskType, darTask.MinTaskScheduleTime);
					}
					DateTime dateTime = base.MinTaskScheduleTime;
					if (base.MinTaskScheduleTime == DateTime.MinValue)
					{
						dateTime = DateTime.UtcNow;
					}
					DateTime dateTime2 = dateTime;
					while (dateTime2 <= DateTime.UtcNow)
					{
						dateTime2 = TaskGenerator.CalculateNextScheduledTime(dateTime, darTaskAggregate.RecurrenceFrequency, darTaskAggregate.RecurrenceInterval);
						darTaskManager.ServiceProvider.ExecutionLog.LogInformation("TaskGenerator", null, this.CorrelationId, string.Format("TaskGenerator: Generating tasks between {0} and {1}", dateTime, dateTime2), new KeyValuePair<string, object>[]
						{
							new KeyValuePair<string, object>("Tag", DarExecutionLogClientIDs.TaskGenerator3.ToString())
						});
						foreach (DarTaskAggregate darTaskAggregate2 in enumerable)
						{
							if (darTaskAggregate2.RecurrenceType == RecurrenceType.Recurrent)
							{
								DateTime dateTime3 = DateTime.MinValue;
								DateTime dateTime4 = DateTime.MinValue;
								if (dictionary.ContainsKey(darTaskAggregate2.TaskType))
								{
									dateTime3 = dictionary[darTaskAggregate2.TaskType];
								}
								while (dateTime4 <= dateTime2)
								{
									if (dateTime3 == DateTime.MinValue)
									{
										dateTime4 = dateTime;
									}
									else
									{
										dateTime4 = TaskGenerator.CalculateNextScheduledTime(dateTime3, darTaskAggregate2.RecurrenceFrequency, darTaskAggregate2.RecurrenceInterval);
									}
									darTaskManager.ServiceProvider.ExecutionLog.LogInformation("TaskGenerator", null, this.CorrelationId, string.Format("TaskGenerator: Processing Task type: {0}. Last Scheduled time: {1} Next Scheduled time: {2}", darTaskAggregate2.TaskType, dateTime3, dateTime4), new KeyValuePair<string, object>[]
									{
										new KeyValuePair<string, object>("Tag", DarExecutionLogClientIDs.TaskGenerator4.ToString())
									});
									if (dateTime4 <= dateTime2)
									{
										darTaskManager.ServiceProvider.ExecutionLog.LogInformation("TaskGenerator", null, this.CorrelationId, string.Format("TaskGenerator: Generating task for Task type: {0} with Scheduled time: {1}", darTaskAggregate2.TaskType, dateTime4), new KeyValuePair<string, object>[]
										{
											new KeyValuePair<string, object>("Tag", DarExecutionLogClientIDs.TaskGenerator5.ToString())
										});
										DarTask darTask2 = darTaskManager.ServiceProvider.DarTaskFactory.CreateTask(darTaskAggregate2.TaskType);
										darTask2.MinTaskScheduleTime = dateTime4;
										darTask2.TenantId = base.TenantId;
										darTaskManager.Enqueue(darTask2);
									}
									else
									{
										darTaskManager.ServiceProvider.ExecutionLog.LogInformation("TaskGenerator", null, this.CorrelationId, string.Format("TaskGenerator: Skipping generating task for Task type: {0} as Next Scheduled time: {1} is greater than Generation Period End time: {2}", darTaskAggregate2.TaskType, dateTime4, dateTime2), new KeyValuePair<string, object>[]
										{
											new KeyValuePair<string, object>("Tag", DarExecutionLogClientIDs.TaskGenerator6.ToString())
										});
									}
									dateTime3 = dateTime4;
								}
							}
						}
						dateTime = dateTime2;
					}
					result = DarTaskExecutionResult.Completed;
				}
			}
			catch (Exception arg)
			{
				darTaskManager.ServiceProvider.ExecutionLog.LogError("TaskGenerator", null, this.CorrelationId, null, string.Format("TaskGenerator: Error when generating tasks: {0} ", arg), new KeyValuePair<string, object>[]
				{
					new KeyValuePair<string, object>("Tag", DarExecutionLogClientIDs.TaskGenerator7.ToString())
				});
				result = DarTaskExecutionResult.Failed;
			}
			return result;
		}

		public override void CompleteTask(DarTaskManager darTaskManager)
		{
		}

		private const string LoggingClientId = "TaskGenerator";

		private const int DefaultRetryCount = 20;

		private const int DefaultRetryIntervalInSeconds = 10;
	}
}
