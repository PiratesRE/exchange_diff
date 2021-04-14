using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery
{
	public class RecoveryActionRunner
	{
		public RecoveryActionRunner(RecoveryActionId actionId, string resourceName, ResponderWorkItem responderWorkItem, bool isThrowOnException, CancellationToken cancellationToken, string localServerName = null)
		{
			GlobalTunables tunables = Dependencies.ThrottleHelper.Tunables;
			if (!tunables.IsRunningMock || string.IsNullOrEmpty(localServerName))
			{
				localServerName = Dependencies.ThrottleHelper.Tunables.LocalMachineName;
			}
			this.localServerName = localServerName;
			this.TraceContext = TracingContext.Default;
			this.ActionId = actionId;
			this.ResourceName = resourceName;
			this.responderWorkItem = responderWorkItem;
			this.RequesterName = responderWorkItem.Definition.Name;
			this.isThrowOnException = isThrowOnException;
			this.timeout = TimeSpan.FromSeconds((double)responderWorkItem.Definition.TimeoutSeconds);
			this.InstanceId = Interlocked.Increment(ref RecoveryActionRunner.globalInstanceCounter);
			this.inProgressTracker = new ThrottlingProgressTracker(this.InstanceId, actionId, resourceName, this.RequesterName, this.timeout);
			FixedThrottleEntry throttleDefinition = Dependencies.ThrottleHelper.Settings.GetThrottleDefinition(actionId, resourceName, this.responderWorkItem.Definition);
			this.throttleGroupName = this.responderWorkItem.Definition.ThrottleGroupName;
			this.throttleParameters = throttleDefinition.ThrottleParameters;
			this.throttleIdentity = CrimsonHelper.NullCode(throttleDefinition.Identity);
			this.throttlePropertiesXml = CrimsonHelper.NullCode(throttleDefinition.GetThrottlePropertiesAsXml().ToString());
		}

		public bool IgnoreGroupThrottlingWhenMajorityNotSucceeded { get; set; }

		public bool IsIgnoreResourceName { get; set; }

		internal RecoveryActionId ActionId { get; private set; }

		internal string ResourceName { get; private set; }

		internal string RequesterName { get; private set; }

		internal long InstanceId { get; private set; }

		internal int TotalServersInGroup { get; private set; }

		internal LocalThrottlingResult LocalThrottleResult { get; private set; }

		internal GroupThrottlingResult GroupThrottleResult { get; private set; }

		internal bool IsSynchronousDistributedCheck { get; set; }

		private protected TracingContext TraceContext { protected get; private set; }

		public static void SetThrottleProperties(ResponderDefinition responderDefinition, string throttleGroupName, RecoveryActionId actionId, string resourceName, string[] serversInGroup = null)
		{
			if (throttleGroupName == null)
			{
				throttleGroupName = string.Empty;
			}
			responderDefinition.ThrottleGroupName = throttleGroupName;
			responderDefinition.ThrottlePolicyXml = Dependencies.ThrottleHelper.Settings.GetThrottleDefinitionsAsCompactXml(actionId, resourceName, responderDefinition);
			ThrottleGroupCache.Instance.AddGroup(throttleGroupName);
			ThrottleGroupCache.Instance.AddServers(serversInGroup);
		}

		public void Execute(Action action)
		{
			this.Execute(delegate(RecoveryActionEntry startEntry)
			{
				action();
			});
		}

		public void Execute(Action<RecoveryActionEntry> action)
		{
			bool flag = false;
			Exception exception = null;
			if (this.IsIgnoreResourceName)
			{
				Thread.Sleep(new Random().Next() % 1000);
			}
			try
			{
				this.inProgressTracker.MarkBegin();
				this.CheckLocalThrottleLimits();
				this.CheckGroupThrottleLimits();
				flag = true;
			}
			catch (Exception ex)
			{
				exception = ex;
				if (ex is ThrottlingRejectedOperationException)
				{
					this.responderWorkItem.Result.IsThrottled = true;
				}
				if (this.isThrowOnException)
				{
					throw;
				}
			}
			finally
			{
				this.inProgressTracker.MarkEnd();
				this.LogThrottleResults(flag, exception);
			}
			if (flag && action != null)
			{
				this.PerformAction(action);
			}
		}

		public string[] GetServersInGroup()
		{
			string[] array = null;
			if (!string.IsNullOrEmpty(this.throttleGroupName))
			{
				array = ThrottleGroupCache.Instance.GetServersInGroup(this.throttleGroupName, true);
			}
			if (Utilities.IsSequenceNullOrEmpty<string>(array))
			{
				array = this.throttleServersInGroup;
			}
			this.TotalServersInGroup = ((array != null) ? array.Length : 0);
			return array;
		}

		public void SetServersInGroup(string[] serversInGroup)
		{
			this.throttleServersInGroup = serversInGroup;
		}

		public void VerifyThrottleLimitsNotExceeded()
		{
			this.Execute(null);
		}

		internal void PerformAction(Action<RecoveryActionEntry> action)
		{
			Exception ex = null;
			RecoveryActionEntry recoveryActionEntry = null;
			try
			{
				ManagedAvailabilityCrimsonEvents.StartingRecoveryAction.LogGeneric(new object[]
				{
					this.ActionId,
					this.ResourceName,
					this.RequesterName,
					RecoveryActionThrottlingMode.None
				});
				recoveryActionEntry = this.PublishStartEntry();
				this.inProgressTracker.MarkEnd();
				action(recoveryActionEntry);
			}
			catch (Exception ex2)
			{
				ex = ex2;
				if (this.isThrowOnException)
				{
					throw;
				}
			}
			finally
			{
				if (recoveryActionEntry != null)
				{
					this.PublishFinishEntry(recoveryActionEntry, ex);
					if (ex == null)
					{
						ManagedAvailabilityCrimsonEvents.SuccessfulyFinishedRecoveryAction.Log<RecoveryActionId, string, string, string, string, string>(this.ActionId, this.ResourceName, this.RequesterName, "<none>", this.throttleIdentity, this.throttlePropertiesXml);
					}
					else
					{
						ManagedAvailabilityCrimsonEvents.FailedToFinishedRecoveryAction.Log<RecoveryActionId, string, string, string, string, string>(this.ActionId, this.ResourceName, this.RequesterName, ex.ToString(), this.throttleIdentity, this.throttlePropertiesXml);
					}
				}
			}
		}

		internal void LogThrottleResults(bool isThrottlingPassed, Exception exception = null)
		{
			ManagedAvailabilityCrimsonEvent managedAvailabilityCrimsonEvent = ManagedAvailabilityCrimsonEvents.ThrottlingAllowedOperationV2;
			if (!isThrottlingPassed)
			{
				managedAvailabilityCrimsonEvent = ManagedAvailabilityCrimsonEvents.ThrottlingRejectedOperationV2;
			}
			managedAvailabilityCrimsonEvent.LogGeneric(new object[]
			{
				this.InstanceId,
				this.ActionId,
				this.ResourceName,
				this.RequesterName,
				(exception == null) ? "<none>" : exception.Message,
				(this.LocalThrottleResult != null) ? this.LocalThrottleResult.ToXml(false) : "<not attempted>",
				(this.GroupThrottleResult != null) ? this.GroupThrottleResult.ToXml(false) : "<not attempted>",
				this.TotalServersInGroup,
				this.TotalServersInGroup
			});
		}

		internal RecoveryActionEntry PublishStartEntry()
		{
			RecoveryActionEntry recoveryActionEntry = RecoveryActionHelper.ConstructStartActionEntry(this.ActionId, this.ResourceName, this.RequesterName, ExDateTime.Now.LocalTime + this.timeout, this.throttleIdentity, this.throttlePropertiesXml, null, null);
			this.FillQuotaInfo(recoveryActionEntry);
			this.PublishRecoveryActionEntry(recoveryActionEntry);
			return recoveryActionEntry;
		}

		internal RecoveryActionEntry PublishFinishEntry(RecoveryActionEntry startEntry, Exception exception)
		{
			RecoveryActionEntry recoveryActionEntry = RecoveryActionHelper.ConstructFinishActionEntry(startEntry, exception, null, null);
			this.FillQuotaInfo(recoveryActionEntry);
			this.PublishRecoveryActionEntry(recoveryActionEntry);
			return recoveryActionEntry;
		}

		internal void PublishRecoveryActionEntry(RecoveryActionEntry entry)
		{
			try
			{
				Dependencies.LamRpc.UpdateRecoveryActionEntry(this.localServerName, entry, 30000);
			}
			catch (Exception ex)
			{
				ManagedAvailabilityCrimsonEvents.FailedToPublishRecoveryActionEntry.Log<RecoveryActionId, string, string, string, string>(entry.Id, entry.InstanceId, entry.ResourceName, entry.RequestorName, ex.Message);
				throw;
			}
		}

		internal void FillQuotaInfo(RecoveryActionEntry entry)
		{
			string str = string.Empty;
			if (this.LocalThrottleResult != null)
			{
				entry.TotalLocalActionsInOneHour = this.LocalThrottleResult.TotalInOneHour;
				entry.TotalLocalActionsInOneDay = this.LocalThrottleResult.TotalInOneDay;
				str = this.LocalThrottleResult.ToXml(false);
			}
			string str2 = string.Empty;
			if (this.GroupThrottleResult != null)
			{
				entry.TotalGroupActionsInOneDay = this.GroupThrottleResult.TotalInOneDay;
				str2 = this.GroupThrottleResult.ToXml(false);
			}
			entry.Context = str + Environment.NewLine + str2;
		}

		internal void CheckLocalThrottleLimits()
		{
			string resourceName = this.IsIgnoreResourceName ? null : this.ResourceName;
			RpcGetThrottlingStatisticsImpl.ThrottlingStatistics throttlingStatistics = Dependencies.LamRpc.GetThrottlingStatistics(this.localServerName, this.ActionId, resourceName, this.throttleParameters.LocalMaximumAllowedAttemptsInOneHour, this.throttleParameters.LocalMaximumAllowedAttemptsInADay, false, true, 35000);
			DateTime localTime = ExDateTime.Now.LocalTime;
			DateTime dateTime = DateTime.MinValue;
			RecoveryActionRunner.ThrottleFailedChecks throttleFailedChecks = RecoveryActionRunner.ThrottleFailedChecks.None;
			if (this.throttleParameters.LocalMinimumMinutesBetweenAttempts != -1 && throttlingStatistics.MostRecentEntry != null)
			{
				TimeSpan t = localTime - throttlingStatistics.MostRecentEntry.EndTime;
				TimeSpan timeSpan = TimeSpan.FromMinutes((double)this.throttleParameters.LocalMinimumMinutesBetweenAttempts);
				if (t < timeSpan)
				{
					throttleFailedChecks |= RecoveryActionRunner.ThrottleFailedChecks.LocalMinimumMinutes;
					dateTime = throttlingStatistics.MostRecentEntry.EndTime + timeSpan;
				}
			}
			if (this.throttleParameters.LocalMaximumAllowedAttemptsInOneHour != -1 && throttlingStatistics.NumberOfActionsInOneHour >= this.throttleParameters.LocalMaximumAllowedAttemptsInOneHour)
			{
				throttleFailedChecks |= RecoveryActionRunner.ThrottleFailedChecks.LocalMaxInHour;
				if (throttlingStatistics.EntryExceedingOneHourLimit != null)
				{
					DateTime dateTime2 = throttlingStatistics.EntryExceedingOneHourLimit.EndTime + TimeSpan.FromHours(1.0);
					if (dateTime2 > dateTime)
					{
						dateTime = dateTime2;
					}
				}
			}
			if (this.throttleParameters.LocalMaximumAllowedAttemptsInADay != -1 && throttlingStatistics.NumberOfActionsInOneDay >= this.throttleParameters.LocalMaximumAllowedAttemptsInADay)
			{
				throttleFailedChecks |= RecoveryActionRunner.ThrottleFailedChecks.LocalMaxInDay;
				if (throttlingStatistics.EntryExceedingOneDayLimit != null)
				{
					DateTime dateTime3 = throttlingStatistics.EntryExceedingOneDayLimit.EndTime + TimeSpan.FromDays(1.0);
					if (dateTime3 > dateTime)
					{
						dateTime = dateTime3;
					}
				}
			}
			if (throttlingStatistics.IsThrottlingInProgress && throttlingStatistics.ThrottleProgressInfo.InstanceId != this.InstanceId)
			{
				throttleFailedChecks |= RecoveryActionRunner.ThrottleFailedChecks.InProgressMismatch;
			}
			if (throttlingStatistics.IsRecoveryInProgress)
			{
				throttleFailedChecks |= RecoveryActionRunner.ThrottleFailedChecks.RecoveryInProgress;
			}
			string text = (throttleFailedChecks != RecoveryActionRunner.ThrottleFailedChecks.None) ? throttleFailedChecks.ToString() : string.Empty;
			LocalThrottlingResult localThrottleResult = new LocalThrottlingResult
			{
				IsPassed = (throttleFailedChecks == RecoveryActionRunner.ThrottleFailedChecks.None),
				MostRecentEntry = throttlingStatistics.MostRecentEntry,
				MinimumMinutes = this.throttleParameters.LocalMinimumMinutesBetweenAttempts,
				TotalInOneHour = throttlingStatistics.NumberOfActionsInOneHour,
				MaxAllowedInOneHour = this.throttleParameters.LocalMaximumAllowedAttemptsInOneHour,
				TotalInOneDay = throttlingStatistics.NumberOfActionsInOneDay,
				MaxAllowedInOneDay = this.throttleParameters.LocalMaximumAllowedAttemptsInADay,
				IsThrottlingInProgress = throttlingStatistics.IsThrottlingInProgress,
				IsRecoveryInProgress = throttlingStatistics.IsRecoveryInProgress,
				ChecksFailed = text,
				TimeToRetryAfter = dateTime
			};
			this.LocalThrottleResult = localThrottleResult;
			if (throttleFailedChecks != RecoveryActionRunner.ThrottleFailedChecks.None)
			{
				throw new LocalThrottlingRejectedOperationException(this.ActionId.ToString(), this.ResourceName, this.RequesterName, text);
			}
		}

		internal void CheckGroupThrottleLimits()
		{
			DateTime timeToRetryAfter = DateTime.MinValue;
			if (string.IsNullOrEmpty(this.throttleGroupName) && Utilities.IsSequenceNullOrEmpty<string>(this.throttleServersInGroup))
			{
				this.GroupThrottleResult = new GroupThrottlingResult
				{
					IsPassed = true,
					Comment = "Neither ThrottleGroupName or ServersInGroup are specified. Allowing the operation for backward compatibility"
				};
				return;
			}
			string[] serversInGroup = this.GetServersInGroup();
			if (Utilities.IsSequenceNullOrEmpty<string>(serversInGroup))
			{
				if (this.IgnoreGroupThrottlingWhenMajorityNotSucceeded)
				{
					this.GroupThrottleResult = new GroupThrottlingResult
					{
						IsPassed = true,
						Comment = "No servers were detected in the group. Allowing the operation to continue since IgnoreGroupThrottlingWhenMajorityNotSucceeded is specified."
					};
					return;
				}
				string text = "ServersEmpty";
				this.GroupThrottleResult = new GroupThrottlingResult
				{
					IsPassed = false,
					ChecksFailed = text,
					Comment = "Not a single server is found in the group, but group throttling is requested"
				};
				throw new GroupThrottlingRejectedOperationException(this.ActionId.ToString(), this.ResourceName, this.RequesterName, text);
			}
			else
			{
				ConcurrentDictionary<string, Exception> exceptionsByServer;
				Dictionary<string, RpcGetThrottlingStatisticsImpl.ThrottlingStatistics> throttleStatisticsAcrossGroup = this.GetThrottleStatisticsAcrossGroup(serversInGroup, out exceptionsByServer);
				int count = throttleStatisticsAcrossGroup.Count;
				int num = throttleStatisticsAcrossGroup.Count((KeyValuePair<string, RpcGetThrottlingStatisticsImpl.ThrottlingStatistics> kv) => kv.Value != null);
				RecoveryActionHelper.RecoveryActionEntrySerializable recoveryActionEntrySerializable = null;
				int num2 = 0;
				RpcGetThrottlingStatisticsImpl.ThrottlingStatistics[] array = throttleStatisticsAcrossGroup.Values.ToArray<RpcGetThrottlingStatisticsImpl.ThrottlingStatistics>();
				foreach (RpcGetThrottlingStatisticsImpl.ThrottlingStatistics throttlingStatistics in array)
				{
					if (throttlingStatistics != null)
					{
						if (recoveryActionEntrySerializable == null || (throttlingStatistics.MostRecentEntry != null && throttlingStatistics.MostRecentEntry.EndTime > recoveryActionEntrySerializable.EndTime))
						{
							recoveryActionEntrySerializable = throttlingStatistics.MostRecentEntry;
						}
						num2 += throttlingStatistics.NumberOfActionsInOneDay;
					}
					else if (this.throttleParameters.LocalMaximumAllowedAttemptsInADay != -1)
					{
						num2 += this.throttleParameters.LocalMaximumAllowedAttemptsInADay;
					}
				}
				RecoveryActionRunner.ThrottleFailedChecks throttleFailedChecks = RecoveryActionRunner.ThrottleFailedChecks.None;
				if (this.throttleParameters.GroupMinimumMinutesBetweenAttempts != -1 && recoveryActionEntrySerializable != null)
				{
					DateTime localTime = ExDateTime.Now.LocalTime;
					TimeSpan t = localTime - recoveryActionEntrySerializable.EndTime;
					TimeSpan t2 = TimeSpan.FromMinutes((double)this.throttleParameters.GroupMinimumMinutesBetweenAttempts);
					if (t < TimeSpan.FromMinutes((double)this.throttleParameters.GroupMinimumMinutesBetweenAttempts))
					{
						throttleFailedChecks |= RecoveryActionRunner.ThrottleFailedChecks.GroupMinimumMinutes;
						timeToRetryAfter = recoveryActionEntrySerializable.EndTime + t2;
					}
				}
				if (this.throttleParameters.GroupMaximumAllowedAttemptsInADay != -1 && num2 >= this.throttleParameters.GroupMaximumAllowedAttemptsInADay)
				{
					throttleFailedChecks |= RecoveryActionRunner.ThrottleFailedChecks.GroupMaxInDay;
				}
				string[] array3 = (from s in array
				where s != null && s.IsThrottlingInProgress
				orderby s.ThrottleProgressInfo.OperationStartTime, s.ServerName
				select s.ServerName).ToArray<string>();
				string text2 = array3.FirstOrDefault<string>();
				if (!string.IsNullOrEmpty(text2) && !string.Equals(text2, this.localServerName))
				{
					throttleFailedChecks |= RecoveryActionRunner.ThrottleFailedChecks.GroupThrottlingInProgress;
				}
				string[] array4 = (from s in array
				where s != null && s.IsRecoveryInProgress
				orderby s.ServerName
				select s.ServerName).ToArray<string>();
				if (array4.Length > 0)
				{
					throttleFailedChecks |= RecoveryActionRunner.ThrottleFailedChecks.GroupRecoveryInProgress;
				}
				string text3 = (throttleFailedChecks != RecoveryActionRunner.ThrottleFailedChecks.None) ? throttleFailedChecks.ToString() : string.Empty;
				GroupThrottlingResult groupThrottlingResult = new GroupThrottlingResult
				{
					IsPassed = (throttleFailedChecks == RecoveryActionRunner.ThrottleFailedChecks.None),
					TotalRequestsSent = count,
					TotalRequestsSucceeded = num,
					MostRecentEntry = recoveryActionEntrySerializable,
					MinimumMinutes = this.throttleParameters.GroupMinimumMinutesBetweenAttempts,
					TotalInOneDay = num2,
					MaxAllowedInOneDay = this.throttleParameters.GroupMaximumAllowedAttemptsInADay,
					ThrottlingInProgressServers = array3,
					RecoveryInProgressServers = array4,
					ChecksFailed = text3,
					TimeToRetryAfter = timeToRetryAfter,
					GroupStats = throttleStatisticsAcrossGroup,
					ExceptionsByServer = exceptionsByServer
				};
				this.GroupThrottleResult = groupThrottlingResult;
				if (throttleFailedChecks == RecoveryActionRunner.ThrottleFailedChecks.None)
				{
					return;
				}
				if (throttleFailedChecks.HasFlag(RecoveryActionRunner.ThrottleFailedChecks.GroupMaxInDay) && this.IgnoreGroupThrottlingWhenMajorityNotSucceeded && num <= throttleStatisticsAcrossGroup.Count / 2)
				{
					groupThrottlingResult.IsPassed = true;
					groupThrottlingResult.Comment = "Allowing the operation to continue since majority requests did not succeed, and  IgnoreGroupThrottlingWhenMajorityNotSucceeded is requested";
					return;
				}
				throw new GroupThrottlingRejectedOperationException(this.ActionId.ToString(), this.ResourceName, this.RequesterName, text3);
			}
		}

		private Dictionary<string, RpcGetThrottlingStatisticsImpl.ThrottlingStatistics> GetThrottleStatisticsAcrossGroup(string[] servers, out ConcurrentDictionary<string, Exception> exceptionsByServer)
		{
			Dictionary<string, RpcGetThrottlingStatisticsImpl.ThrottlingStatistics> groupStats = new Dictionary<string, RpcGetThrottlingStatisticsImpl.ThrottlingStatistics>();
			exceptionsByServer = null;
			if (servers == null || servers.Length == 0)
			{
				return groupStats;
			}
			foreach (string key in servers)
			{
				groupStats[key] = null;
			}
			using (DistributedAction distributedAction = new DistributedAction(servers, servers.Length, this.IsSynchronousDistributedCheck))
			{
				string resourceName = this.IsIgnoreResourceName ? null : this.ResourceName;
				distributedAction.Run(delegate(string serverName)
				{
					RpcGetThrottlingStatisticsImpl.ThrottlingStatistics throttlingStatistics = Dependencies.LamRpc.GetThrottlingStatistics(serverName, this.ActionId, resourceName, -1, -1, false, true, 35000);
					Dictionary<string, RpcGetThrottlingStatisticsImpl.ThrottlingStatistics> groupStats;
					lock (groupStats)
					{
						groupStats[serverName] = throttlingStatistics;
					}
				}, TimeSpan.FromMilliseconds(40000.0));
				exceptionsByServer = distributedAction.ExceptionsByServer;
			}
			return groupStats;
		}

		public const int RpcTimeoutInMs = 35000;

		private static long globalInstanceCounter = DateTime.UtcNow.Ticks;

		private readonly ThrottleParameters throttleParameters;

		private readonly ResponderWorkItem responderWorkItem;

		private readonly bool isThrowOnException;

		private readonly ThrottlingProgressTracker inProgressTracker;

		private readonly string localServerName;

		private readonly TimeSpan timeout;

		private readonly string throttleIdentity;

		private readonly string throttlePropertiesXml;

		private readonly string throttleGroupName;

		private string[] throttleServersInGroup;

		[Flags]
		public enum ThrottleFailedChecks
		{
			None = 0,
			LocalMinimumMinutes = 1,
			LocalMaxInHour = 2,
			LocalMaxInDay = 4,
			InProgressMismatch = 8,
			RecoveryInProgress = 16,
			GroupMinimumMinutes = 32,
			GroupMaxInDay = 64,
			GroupThrottlingInProgress = 128,
			GroupRecoveryInProgress = 256
		}
	}
}
