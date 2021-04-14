using System;
using System.Collections.Generic;
using Microsoft.Exchange.Hygiene.Data.BackgroundJobBackend;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public abstract class FfoBGDTaskProbeBase : ProbeWorkItem
	{
		internal BackgroundJobBackendSession BackgroundJobSession { get; set; }

		internal Guid RoleId { get; set; }

		internal BackgroundJobMgrInstance ManagerInstance { get; set; }

		internal RoleDefinition[] RoleDefinitions { get; set; }

		internal TimeSpan TimeOfTaskInNotStartedStatus { get; set; }

		internal void DisplayBGDScheduleAndTaskCommands(TaskItem taskItem)
		{
			if (taskItem != null)
			{
				this.Log("For more the last task information, please execute 'Get-BackgroundJobTask -TaskId {0}'. ", new object[]
				{
					taskItem.TaskId
				});
				this.Log("For more BGD job schedule information, please execute 'Get-BackgroundJobSchedule -ScheduleId {0}'. ", new object[]
				{
					taskItem.ScheduleId
				});
				this.Log("To get the tasks list, please execute 'Get-BackgroundJobTask -ScheduleId {0} | Sort StartTime '.", new object[]
				{
					taskItem.ScheduleId
				});
			}
		}

		protected static T ReadProbeParameters<T>(Dictionary<string, string> parameters, string name)
		{
			string value;
			if (parameters.TryGetValue(name, out value))
			{
				return (T)((object)Convert.ChangeType(value, typeof(T)));
			}
			return default(T);
		}

		protected void Log(string message, params object[] args)
		{
			ProbeResult result = base.Result;
			result.ExecutionContext += string.Format("{0}: {1}{2}", DateTime.UtcNow, string.Format(message, args), Environment.NewLine);
		}

		protected abstract void ReadProbeParameter();
	}
}
