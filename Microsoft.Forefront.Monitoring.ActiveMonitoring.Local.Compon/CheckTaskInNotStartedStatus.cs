using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Hygiene.Data.BackgroundJobBackend;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public class CheckTaskInNotStartedStatus : FfoBGDTaskProbeBase
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			try
			{
				this.Initialize();
				this.ReadProbeParameter();
				List<Exception> list = new List<Exception>();
				if (!cancellationToken.IsCancellationRequested)
				{
					foreach (RoleDefinition roleDefinition in base.RoleDefinitions)
					{
						base.RoleId = roleDefinition.RoleId;
						BackgroundJobMgrInstance[] source = base.BackgroundJobSession.FindBackgroundJobMgrInstances(base.RoleId, Environment.MachineName);
						base.ManagerInstance = source.FirstOrDefault<BackgroundJobMgrInstance>();
						if (base.ManagerInstance != null)
						{
							TaskItem[] array = base.BackgroundJobSession.FindTasks(base.RoleId, null, new TaskExecutionStateType?(TaskExecutionStateType.NotStarted), null, new int?((int)base.ManagerInstance.Region), null, null, null, null, null, null);
							TaskItem[] array2 = array;
							for (int j = 0; j < array2.Length; j++)
							{
								TaskItem taskItem = array2[j];
								if (DateTime.UtcNow - taskItem.InsertTimeStamp > base.TimeOfTaskInNotStartedStatus)
								{
									if (taskItem.HasBJMOwnerId)
									{
										BackgroundJobMgrInstance backgroundJobMgrInstance = base.BackgroundJobSession.FindBackgroundJobMgrInstances(base.RoleId, null).FirstOrDefault((BackgroundJobMgrInstance m) => m.MachineId == taskItem.BJMOwnerId);
										if (backgroundJobMgrInstance.Active && backgroundJobMgrInstance.HeartBeat - taskItem.InsertTimeStamp > base.TimeOfTaskInNotStartedStatus)
										{
											list.Add(new Exception(string.Format("The BGD task (task id : {0}) of the schedule (schedule id : {1} ) has been in Not Started state more than {2} minutes. The job was assigned to BGD machine '{3}'. The task's InsertTimeStamp is {4}. The threshold is {5} minutes.", new object[]
											{
												taskItem.TaskId,
												taskItem.ScheduleId,
												(DateTime.UtcNow - taskItem.InsertTimeStamp).TotalMinutes,
												backgroundJobMgrInstance.MachineName,
												taskItem.InsertTimeStamp,
												base.TimeOfTaskInNotStartedStatus.TotalMinutes
											})));
										}
										else if (DateTime.UtcNow - backgroundJobMgrInstance.HeartBeat > this.timeOfTaskStuckOnInactiveBGDMachine)
										{
											list.Add(new Exception(string.Format("The BGD task (task id : {0}) of the schedule (schedule id : {1} ) has been in Not Started state more than {2} minutes. The job was assigned to BGD machine '{3}'. The task's InsertTimeStamp is {4}. But the BGD service on this machine is inactive for now. The last heart beat of the BGD machine is {5}. The BJM has been inactive on the machien more than {6} minutes. The threshold of the task stuck on inactive BGD is {7} minutes.", new object[]
											{
												taskItem.TaskId,
												taskItem.ScheduleId,
												(DateTime.UtcNow - taskItem.InsertTimeStamp).TotalMinutes,
												backgroundJobMgrInstance.MachineName,
												taskItem.InsertTimeStamp,
												backgroundJobMgrInstance.HeartBeat,
												(DateTime.UtcNow - backgroundJobMgrInstance.HeartBeat).TotalMinutes,
												this.timeOfTaskStuckOnInactiveBGDMachine.TotalMinutes
											})));
										}
									}
									else
									{
										list.Add(new Exception(string.Format("The BGD task (task id : {0}) of the schedule (schedule id : {1} ) has been in Not Started state more than {2} minutes and no BGD machine is assigned for this job. The task's InsertTimeStamp is {3}. The threshold is {4} minutes.", new object[]
										{
											taskItem.TaskId,
											taskItem.ScheduleId,
											(DateTime.UtcNow - taskItem.InsertTimeStamp).TotalMinutes,
											taskItem.InsertTimeStamp,
											base.TimeOfTaskInNotStartedStatus.TotalMinutes
										})));
									}
									base.Log("The task information:\n   Schedule id: {0}\n   Task id: {1}\n   Task current executable state: {2}", new object[]
									{
										taskItem.ScheduleId,
										taskItem.TaskId,
										taskItem.TaskExecutionState
									});
									base.DisplayBGDScheduleAndTaskCommands(taskItem);
								}
							}
						}
					}
					if (list.Count > 0)
					{
						throw new AggregateException(list);
					}
				}
			}
			catch (ArgumentException ex)
			{
				base.Log(string.Format("Probe parameter is invalid: {0}", ex.Message), new object[0]);
				throw;
			}
		}

		protected override void ReadProbeParameter()
		{
			base.TimeOfTaskInNotStartedStatus = TimeSpan.FromMinutes((double)FfoBGDTaskProbeBase.ReadProbeParameters<int>(base.Definition.Attributes, "NumberOfMinutesOfTaskInNotStartedStatus"));
			this.timeOfTaskStuckOnInactiveBGDMachine = TimeSpan.FromMinutes((double)FfoBGDTaskProbeBase.ReadProbeParameters<int>(base.Definition.Attributes, "NumberOfMinutesOfTaskStuckOnInactiveBGDMachine"));
			if (base.TimeOfTaskInNotStartedStatus < TimeSpan.Zero)
			{
				throw new ArgumentException(string.Format("The value of the parameter NumberOfMinutesOfTaskInNotStartedStatus ({0}) should be larger than 0.", base.TimeOfTaskInNotStartedStatus));
			}
			if (this.timeOfTaskStuckOnInactiveBGDMachine <= TimeSpan.Zero)
			{
				throw new ArgumentException(string.Format("The value of the parameter NumberOfMinutesOfTaskStuckOnInactiveBGDMachine ({0}) should be larger than 0.", this.timeOfTaskStuckOnInactiveBGDMachine));
			}
			base.Log("The parameter value NumberOfMinutesOfTaskInNotStartedStatus is {0} minutes. ", new object[]
			{
				base.TimeOfTaskInNotStartedStatus,
				Environment.NewLine
			});
			base.Log("The parameter value NumberOfMinutesOfTaskStuckOnInactiveBGDMachine is {0} minutes. ", new object[]
			{
				this.timeOfTaskStuckOnInactiveBGDMachine,
				Environment.NewLine
			});
		}

		private void Initialize()
		{
			if (base.BackgroundJobSession == null)
			{
				BackgroundJobBackendSession backgroundJobBackendSession = new BackgroundJobBackendSession();
				base.RoleDefinitions = backgroundJobBackendSession.FindRoleByNameVersion(null, null);
				base.BackgroundJobSession = backgroundJobBackendSession;
			}
		}

		private TimeSpan timeOfTaskStuckOnInactiveBGDMachine;
	}
}
