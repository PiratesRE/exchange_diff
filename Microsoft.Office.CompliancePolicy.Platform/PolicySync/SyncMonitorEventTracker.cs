using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.CompliancePolicy.Monitor;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	internal class SyncMonitorEventTracker
	{
		public SyncMonitorEventTracker(SyncJob syncJob) : this(syncJob.CurrentWorkItem.ExternalIdentity, syncJob.CurrentWorkItem.TenantContext.TenantId.ToString(), syncJob.CurrentWorkItem.TryCount + 1, syncJob.SyncAgentContext.SyncAgentConfig.PolicySyncSLA, syncJob.SyncAgentContext.MonitorProvider, syncJob.CurrentWorkItem.FirstChangeArriveUTC, syncJob.CurrentWorkItem.LastChangeArriveUTC, syncJob.CurrentWorkItem.ExecuteTimeUTC, syncJob.SyncAgentContext.PerfCounterProvider, syncJob.Errors, null)
		{
		}

		internal SyncMonitorEventTracker(string notificationId, string tenantId, int tryCount, TimeSpan policySyncLSA, IMonitoringNotification monitorProvider, DateTime firstNotificationArriveUtc, DateTime lastNotificationArriveUtc, DateTime currentWorkItemScheduleUtc, PerfCounterProvider perfCounterProvider, IEnumerable<SyncAgentExceptionBase> errors, Func<TimeSpan, TimeSpan> getLatencyValueDelegate)
		{
			this.notificationId = notificationId;
			this.tenantId = tenantId;
			this.tryCount = tryCount;
			this.policySyncLSA = policySyncLSA;
			this.monitorProvider = monitorProvider;
			this.firstNotificationArriveUtc = firstNotificationArriveUtc;
			this.lastNotificationArriveUtc = lastNotificationArriveUtc;
			this.currentWorkItemScheduleUtc = currentWorkItemScheduleUtc;
			this.getLatencyValueDelegate = getLatencyValueDelegate;
			this.ObjectSyncLatencyTable = new Dictionary<ConfigurationObjectType, PolicySyncLatencyInformation>();
			this.ObjectSyncStartTimeTable = new Dictionary<ConfigurationObjectType, Dictionary<LatencyType, DateTime>>();
			this.NonObjectSyncLatencyTable = new Dictionary<LatencyType, KeyValuePair<DateTime, TimeSpan>>
			{
				{
					LatencyType.Initialization,
					default(KeyValuePair<DateTime, TimeSpan>)
				},
				{
					LatencyType.TenantInfo,
					default(KeyValuePair<DateTime, TimeSpan>)
				},
				{
					LatencyType.PersistentQueue,
					default(KeyValuePair<DateTime, TimeSpan>)
				}
			};
			this.FailureTable = new Dictionary<Guid, PolicySyncFailureInformation>();
			this.perfCounterProvider = perfCounterProvider;
			this.errors = errors;
		}

		internal Dictionary<ConfigurationObjectType, PolicySyncLatencyInformation> ObjectSyncLatencyTable { get; private set; }

		internal Dictionary<ConfigurationObjectType, Dictionary<LatencyType, DateTime>> ObjectSyncStartTimeTable { get; private set; }

		internal Dictionary<LatencyType, KeyValuePair<DateTime, TimeSpan>> NonObjectSyncLatencyTable { get; private set; }

		internal Dictionary<Guid, PolicySyncFailureInformation> FailureTable { get; private set; }

		internal TimeSpan NotificationPickUpDelay { get; private set; }

		internal TimeSpan TotalProcessTime
		{
			get
			{
				return DateTime.UtcNow - this.firstNotificationArriveUtc;
			}
		}

		internal TimeSpan TotalProcessTimeForCurrentSyncCycle
		{
			get
			{
				return DateTime.UtcNow - this.lastNotificationArriveUtc;
			}
		}

		internal TimeSpan ExecutionDelayTime
		{
			get
			{
				return this.currentWorkItemScheduleUtc - this.lastNotificationArriveUtc + this.NotificationPickUpDelay;
			}
		}

		internal long WsCallNumber { get; private set; }

		public void TriggerAlertIfNecessary()
		{
			foreach (PolicySyncFailureInformation policySyncFailureInformation in this.FailureTable.Values)
			{
				this.monitorProvider.PublishEvent("UnifiedPolicySync.PermanentError", this.tenantId, this.GetFailureContext(policySyncFailureInformation), policySyncFailureInformation.LastException);
			}
			if (this.TotalProcessTime > this.policySyncLSA)
			{
				this.monitorProvider.PublishEvent("UnifiedPolicySync.PolicySyncTimeExceededError", this.tenantId, this.GetLatencyContext(), null);
			}
		}

		public void MarkNotificationPickedUp()
		{
			this.NotificationPickUpDelay = DateTime.UtcNow - this.currentWorkItemScheduleUtc;
		}

		public void TrackLatencyWrapper(LatencyType latencyType, Action action)
		{
			this.TrackLatencyWrapper(latencyType, ConfigurationObjectType.Policy, action, true);
		}

		public void TrackLatencyWrapper(LatencyType latencyType, ConfigurationObjectType objectType, Action action, bool alwaysMarkEnd = true)
		{
			int num = 0;
			this.TrackLatencyWrapper(latencyType, objectType, ref num, action, alwaysMarkEnd, false);
		}

		public void TrackLatencyWrapper(LatencyType latencyType, ConfigurationObjectType objectType, ref int deltaObjectCount, Action action, bool alwaysMarkEnd = true, bool markEndOnly = false)
		{
			if (!markEndOnly)
			{
				this.MarkSyncOperationStart(latencyType, objectType);
			}
			if (alwaysMarkEnd)
			{
				try
				{
					action();
					return;
				}
				finally
				{
					this.MarkSyncOperationEnd(latencyType, objectType, deltaObjectCount);
				}
			}
			try
			{
				action();
			}
			catch
			{
				this.MarkSyncOperationEnd(latencyType, objectType, deltaObjectCount);
				throw;
			}
		}

		public void ReportTenantLevelFailure(Exception exception)
		{
			this.InternalReportFailure(exception, Guid.Empty, null);
		}

		public void ReportObjectLevelFailure(Exception exception, ConfigurationObjectType objectType, Guid? policyId)
		{
			this.InternalReportFailure(exception, (policyId != null) ? policyId.Value : Guid.Empty, new ConfigurationObjectType?(objectType));
		}

		public void PublishPerfData()
		{
			this.perfCounterProvider.IncrementBy("Total Processing Time Per Sync Request", this.TotalProcessTimeForCurrentSyncCycle.Ticks, "Total Processing Time Per Sync Request Base");
			this.perfCounterProvider.IncrementBy("Execution Delay Time Per Sync Request", this.ExecutionDelayTime.Ticks, "Execution Delay Time Per Sync Request Base");
			this.perfCounterProvider.IncrementBy("Initialization Time Per Sync Request", this.NonObjectSyncLatencyTable[LatencyType.Initialization].Value.Ticks, "Initialization Time Per Sync Request Base");
			foreach (ConfigurationObjectType key in new ConfigurationObjectType[]
			{
				ConfigurationObjectType.Policy,
				ConfigurationObjectType.Rule,
				ConfigurationObjectType.Binding,
				ConfigurationObjectType.Association
			})
			{
				long incrementValue = 0L;
				long incrementValue2 = 0L;
				if (this.ObjectSyncLatencyTable.ContainsKey(key))
				{
					incrementValue = this.ObjectSyncLatencyTable[key].Latencies[LatencyType.FfoWsCall].Ticks;
					incrementValue2 = this.ObjectSyncLatencyTable[key].Latencies[LatencyType.CrudMgr].Ticks;
				}
				this.perfCounterProvider.IncrementBy(PerfCounters.PolicyObjectWsPerfCounters[key].Key, incrementValue, PerfCounters.PolicyObjectWsPerfCounters[key].Value);
				this.perfCounterProvider.IncrementBy(PerfCounters.PolicyObjectCrudMgrPerfCounters[key].Key, incrementValue2, PerfCounters.PolicyObjectCrudMgrPerfCounters[key].Value);
			}
			this.perfCounterProvider.IncrementBy("TenantInfo Processing Time Per Sync Request", this.NonObjectSyncLatencyTable[LatencyType.TenantInfo].Value.Ticks, "TenantInfo Processing Time Per Sync Request Base");
			this.perfCounterProvider.IncrementBy("Persistent Queue Processing Time Per Sync Request", this.NonObjectSyncLatencyTable[LatencyType.PersistentQueue].Value.Ticks, "Persistent Queue Processing Time Per Sync Request Base");
			this.perfCounterProvider.Increment("Processed Sync Request Number Per Second");
			this.perfCounterProvider.Increment("Processed Sync Request Number");
			if (!this.errors.Any<SyncAgentExceptionBase>())
			{
				this.perfCounterProvider.Increment("Successful Sync Request Number Per Second");
				this.perfCounterProvider.Increment("Successful Sync Request Number");
			}
			else
			{
				int num = this.errors.Count((SyncAgentExceptionBase p) => p is SyncAgentTransientException && !(p is PolicyConfigProviderTransientException));
				if (num > 0)
				{
					this.perfCounterProvider.IncrementBy("Policy Sync Ws Call Transient Error Number Per Second", (long)num);
					this.perfCounterProvider.IncrementBy("Policy Sync Ws Call Transient Error Number", (long)num);
				}
				int num2 = this.errors.Count((SyncAgentExceptionBase p) => p is PolicyConfigProviderTransientException);
				if (num2 > 0)
				{
					this.perfCounterProvider.IncrementBy("Policy Sync CrudMgr Transient Error Number Per Second", (long)num2);
					this.perfCounterProvider.IncrementBy("Policy Sync CrudMgr Transient Error Number", (long)num2);
				}
				int num3 = this.errors.Count((SyncAgentExceptionBase p) => p is SyncAgentPermanentException && !(p.InnerException is GrayException) && !(p is PolicyConfigProviderPermanentException));
				if (num3 > 0)
				{
					this.perfCounterProvider.IncrementBy("Policy Sync Ws Call Permanent Error Number Per Second", (long)num3);
					this.perfCounterProvider.IncrementBy("Policy Sync Ws Call Permanent Error Number", (long)num3);
				}
				int num4 = this.errors.Count((SyncAgentExceptionBase p) => p is PolicyConfigProviderPermanentException);
				if (num4 > 0)
				{
					this.perfCounterProvider.IncrementBy("Policy Sync CrudMgr Permanent Error Number Per Second", (long)num4);
					this.perfCounterProvider.IncrementBy("Policy Sync CrudMgr Permanent Error Number", (long)num4);
				}
			}
			if (this.tryCount > 1)
			{
				this.perfCounterProvider.Increment("Sync Request Retry Number Per Second");
				this.perfCounterProvider.Increment("Sync Request Retry Number");
			}
			this.perfCounterProvider.IncrementBy("Ws Call Number Per Sync Request", this.WsCallNumber, "Ws Call Number Per Sync Request Base");
			this.perfCounterProvider.Increment("Tenant Number Per Sync Request", "Tenant Number Per Sync Request Base");
			long incrementValue3 = (long)this.ObjectSyncLatencyTable.Values.Sum((PolicySyncLatencyInformation p) => p.Count);
			this.perfCounterProvider.IncrementBy("Object Number Per Sync Request", incrementValue3, "Object Number Per Sync Request Base");
		}

		private void InitializeForType(ConfigurationObjectType objectType)
		{
			if (!this.ObjectSyncStartTimeTable.ContainsKey(objectType))
			{
				this.ObjectSyncStartTimeTable[objectType] = new Dictionary<LatencyType, DateTime>();
				this.ObjectSyncLatencyTable[objectType] = new PolicySyncLatencyInformation(objectType, 0, this.getLatencyValueDelegate);
				this.ObjectSyncLatencyTable[objectType].Latencies[LatencyType.FfoWsCall] = TimeSpan.Zero;
				this.ObjectSyncLatencyTable[objectType].Latencies[LatencyType.CrudMgr] = TimeSpan.Zero;
			}
		}

		private void MarkSyncOperationStart(LatencyType latencyType, ConfigurationObjectType objectType)
		{
			DateTime utcNow = DateTime.UtcNow;
			switch (latencyType)
			{
			case LatencyType.Initialization:
			case LatencyType.TenantInfo:
			case LatencyType.PersistentQueue:
				this.NonObjectSyncLatencyTable[latencyType] = new KeyValuePair<DateTime, TimeSpan>(utcNow, this.NonObjectSyncLatencyTable[latencyType].Value);
				return;
			case LatencyType.FfoWsCall:
			case LatencyType.CrudMgr:
				this.InitializeForType(objectType);
				this.ObjectSyncStartTimeTable[objectType][latencyType] = utcNow;
				if (latencyType == LatencyType.FfoWsCall)
				{
					this.WsCallNumber += 1L;
					return;
				}
				return;
			default:
				throw new NotSupportedException();
			}
		}

		private void MarkSyncOperationEnd(LatencyType latencyType, ConfigurationObjectType objectType, int deltaObjectCount = 0)
		{
			DateTime utcNow = DateTime.UtcNow;
			switch (latencyType)
			{
			case LatencyType.Initialization:
			case LatencyType.TenantInfo:
			case LatencyType.PersistentQueue:
			{
				TimeSpan value = this.NonObjectSyncLatencyTable[latencyType].Value + this.GetLatencyValue(utcNow - this.NonObjectSyncLatencyTable[latencyType].Key);
				this.NonObjectSyncLatencyTable[latencyType] = new KeyValuePair<DateTime, TimeSpan>(this.NonObjectSyncLatencyTable[latencyType].Key, value);
				return;
			}
			case LatencyType.FfoWsCall:
			case LatencyType.CrudMgr:
			{
				this.ObjectSyncLatencyTable[objectType].Count += deltaObjectCount;
				Dictionary<LatencyType, TimeSpan> latencies;
				(latencies = this.ObjectSyncLatencyTable[objectType].Latencies)[latencyType] = latencies[latencyType] + this.GetLatencyValue(utcNow - this.ObjectSyncStartTimeTable[objectType][latencyType]);
				return;
			}
			default:
				throw new NotSupportedException();
			}
		}

		private string GetLatencyContext()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (PolicySyncLatencyInformation policySyncLatencyInformation in this.ObjectSyncLatencyTable.Values)
			{
				stringBuilder.Append(policySyncLatencyInformation.ToString());
			}
			return string.Format("NotificationId={0};Timestamp={1}\r\nLatencies:\r\nTotalProcessTime={2},TryCount={3}\r\nCurrentCycle:NotifyPickUpDelay={4};Initialization={5};{6}", new object[]
			{
				this.notificationId,
				DateTime.UtcNow,
				(int)this.TotalProcessTime.TotalSeconds,
				this.tryCount,
				(int)this.NotificationPickUpDelay.TotalSeconds,
				(int)this.NonObjectSyncLatencyTable[LatencyType.Initialization].Value.TotalSeconds,
				stringBuilder.ToString()
			});
		}

		private string GetFailureContext(PolicySyncFailureInformation info)
		{
			return string.Format("NotificationId={0}\r\nTimestamp={1}\r\n{2}", this.notificationId, DateTime.UtcNow, info.ToString());
		}

		private void InternalReportFailure(Exception exception, Guid policyId, ConfigurationObjectType? objectType)
		{
			if (!this.FailureTable.ContainsKey(policyId))
			{
				this.FailureTable[policyId] = new PolicySyncFailureInformation(policyId);
			}
			if (objectType != null)
			{
				this.FailureTable[policyId].ObjectTypes.Add(objectType.Value);
			}
			this.FailureTable[policyId].LastException = exception;
		}

		private TimeSpan GetLatencyValue(TimeSpan latency)
		{
			if (this.getLatencyValueDelegate == null)
			{
				return latency;
			}
			return this.getLatencyValueDelegate(latency);
		}

		private readonly int tryCount;

		private readonly TimeSpan policySyncLSA;

		private readonly string tenantId;

		private readonly string notificationId;

		private readonly DateTime firstNotificationArriveUtc;

		private readonly DateTime lastNotificationArriveUtc;

		private readonly DateTime currentWorkItemScheduleUtc;

		private readonly IMonitoringNotification monitorProvider;

		private readonly PerfCounterProvider perfCounterProvider;

		private readonly IEnumerable<SyncAgentExceptionBase> errors;

		private readonly Func<TimeSpan, TimeSpan> getLatencyValueDelegate;
	}
}
