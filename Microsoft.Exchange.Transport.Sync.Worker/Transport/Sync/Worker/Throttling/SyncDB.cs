using System;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.Storage.ResourceHealth;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ContentAggregation;
using Microsoft.Exchange.MailboxTransport.ContentAggregation;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Rpc.Submission;

namespace Microsoft.Exchange.Transport.Sync.Worker.Throttling
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SyncDB : SyncResource
	{
		protected SyncDB(Guid databaseGuid, SyncLogSession syncLogSession) : base(syncLogSession, databaseGuid.ToString())
		{
			this.databaseGuid = databaseGuid;
			this.averageRPCLatency = new RunningAverageFloat(SyncDB.NumberOfRPCLatencySamples);
			this.averageDelay = new RunningAverageFloat(SyncDB.NumberOfDelaySamples);
			base.Initialize();
		}

		protected override int MaxConcurrentWorkInUnknownState
		{
			get
			{
				return AggregationConfiguration.Instance.MaxItemsForDBInUnknownHealthState;
			}
		}

		protected override SubscriptionSubmissionResult ResourceHealthUnknownResult
		{
			get
			{
				return SubscriptionSubmissionResult.DatabaseHealthUnknown;
			}
		}

		protected override SubscriptionSubmissionResult MaxConcurrentWorkAgainstResourceLimitReachedResult
		{
			get
			{
				return SubscriptionSubmissionResult.MaxConcurrentMailboxSubmissions;
			}
		}

		private float RawRpcLatency
		{
			get
			{
				return this.rpcLatencyMonitor.GetRawRpcLatency();
			}
		}

		private float RawRpcLatencyAverage
		{
			get
			{
				return this.rpcLatencyMonitor.GetRawRpcLatencyAverage();
			}
		}

		protected override SyncResourceMonitor[] InitializeHealthMonitoring()
		{
			ResourceKey databaseResourceKey = this.CreateDatabaseRPCResourceKey();
			ResourceKey resourceKey = this.CreateDatabaseReplicationResourceKey();
			this.rpcLatencyMonitor = this.CreateSyncRPCMonitor(databaseResourceKey);
			return new SyncResourceMonitor[]
			{
				this.rpcLatencyMonitor,
				this.CreateSyncResourceMonitor(resourceKey, SyncResourceMonitorType.DatabaseReplicationLog)
			};
		}

		internal static SyncDB CreateSyncDB(Guid databaseGuid, SyncLogSession syncLogSession)
		{
			SyncUtilities.ThrowIfGuidEmpty("databaseGuid", databaseGuid);
			SyncUtilities.ThrowIfArgumentNull("syncLogSession", syncLogSession);
			return new SyncDB(databaseGuid, syncLogSession);
		}

		internal override void UpdateDelay(int delay)
		{
			base.UpdateDelay(delay);
			this.averageDelay.Update((float)delay);
		}

		internal void NotifyStoreRoundtripComplete(string callMethodName, EventHandler<RoundtripCompleteEventArgs> roundtripComplete, RoundtripCompleteEventArgs eventArgs)
		{
			base.SyncLogSession.LogDebugging((TSLID)1339UL, ExTraceGlobals.SchedulerTracer, "StoreRoundtrip on database {0}. Backoff:{1} Latency:{2} RHMLatency:{3} RHMAverageLatency: {4} MethodName:{5}", new object[]
			{
				base.ResourceId,
				eventArgs.ThrottlingInfo.BackOffTime,
				eventArgs.RoundtripTime,
				this.RawRpcLatency,
				this.RawRpcLatencyAverage,
				callMethodName
			});
			this.averageRPCLatency.Update((float)eventArgs.RoundtripTime.TotalMilliseconds);
			roundtripComplete(null, eventArgs);
		}

		protected override SubscriptionSubmissionResult GetResultForResourceUnhealthy(SyncResourceMonitorType syncResourceMonitorType)
		{
			if (syncResourceMonitorType == SyncResourceMonitorType.DatabaseReplicationLog)
			{
				return SubscriptionSubmissionResult.MailboxServerHAUnhealthy;
			}
			return SubscriptionSubmissionResult.DatabaseRpcLatencyUnhealthy;
		}

		protected override bool CanAcceptWorkBasedOnResourceSpecificChecks(out SubscriptionSubmissionResult result)
		{
			result = SubscriptionSubmissionResult.Success;
			if (AggregationConfiguration.Instance.MaxItemsForDBInManualConcurrencyMode > 0)
			{
				if (base.WorkItemsCount >= AggregationConfiguration.Instance.MaxItemsForDBInManualConcurrencyMode)
				{
					base.SyncLogSession.LogError((TSLID)1343UL, ExTraceGlobals.SchedulerTracer, "AcceptCheck: Cannot accept WI. Reached the cap on work items per DB for database {0}. WI Count: {1}.", new object[]
					{
						base.ResourceId,
						base.WorkItemsCount
					});
					result = SubscriptionSubmissionResult.DatabaseOverloaded;
					return false;
				}
				base.SyncLogSession.LogVerbose((TSLID)1344UL, ExTraceGlobals.SchedulerTracer, "AcceptCheck: WI can be accepted on database {0} since we have not reached the WI cap per DB. WI Count: {1}.", new object[]
				{
					base.ResourceId,
					base.WorkItemsCount
				});
				return true;
			}
			else
			{
				float effectiveRPCConcurrency = this.GetEffectiveRPCConcurrency();
				float value = this.averageDelay.Value;
				float rawRpcLatency = this.RawRpcLatency;
				float rawRpcLatencyAverage = this.RawRpcLatencyAverage;
				if (base.WorkItemsCount == 0)
				{
					base.SyncLogSession.LogVerbose((TSLID)1345UL, ExTraceGlobals.SchedulerTracer, "AcceptCheck: WI can be accepted on database {0} since it is the first item. WI Count: {1}.", new object[]
					{
						base.ResourceId,
						base.WorkItemsCount
					});
					return true;
				}
				if (rawRpcLatency > (float)SyncDB.MaxAcceptedRawRPCLatency || rawRpcLatencyAverage > (float)SyncDB.MaxAcceptedRawRPCLatency)
				{
					base.SyncLogSession.LogError((TSLID)1347UL, ExTraceGlobals.SchedulerTracer, "AcceptCheck: Cannot accept WI. Database {0} overloaded. RPC Latency exceeds maximum allowed. Raw Latency: {1} Average Latency: {2} WI Count: {3}.", new object[]
					{
						base.ResourceId,
						rawRpcLatency,
						rawRpcLatencyAverage,
						base.WorkItemsCount
					});
					result = SubscriptionSubmissionResult.DatabaseOverloaded;
					return false;
				}
				if (value <= (float)AggregationConfiguration.Instance.DelayTresholdForAcceptingNewWorkItems + SyncDB.MarginOfError)
				{
					base.SyncLogSession.LogVerbose((TSLID)1348UL, ExTraceGlobals.SchedulerTracer, "AcceptCheck: WI can be accepted on database {0} since RHM indicates that the DB health is good (average delay {1}). WI Count: {2}.", new object[]
					{
						base.ResourceId,
						value,
						base.WorkItemsCount
					});
					return true;
				}
				float storeLatencyAverage = SyncStoreLoadManager.StoreLatencyAverage;
				float cloudLatencyAverage = SyncStoreLoadManager.CloudLatencyAverage;
				float storeCloudRatioAverage = SyncStoreLoadManager.StoreCloudRatioAverage;
				float num = (value + storeLatencyAverage) / (value + storeLatencyAverage + cloudLatencyAverage / storeCloudRatioAverage);
				float num2 = (float)base.WorkItemsCount * num;
				base.SyncLogSession.LogVerbose((TSLID)1349UL, ExTraceGlobals.SchedulerTracer, "AcceptCheck: Database {0} Snapshot. Actual threads:{1} Effective threads:{2} WI Count:{3} Percent WI in store:{4} RPCLatency:{5} RHMLatency:{6} AverageDelay:{7}", new object[]
				{
					base.ResourceId,
					num2,
					effectiveRPCConcurrency,
					base.WorkItemsCount,
					num,
					this.averageRPCLatency,
					rawRpcLatency,
					value
				});
				if (!base.CanAddOneMoreConcurrentRequestToResource())
				{
					result = this.MaxConcurrentWorkAgainstResourceLimitReachedResult;
					return false;
				}
				return true;
			}
		}

		protected virtual float GetEffectiveRPCConcurrency()
		{
			float storeLatencyAverage = SyncStoreLoadManager.StoreLatencyAverage;
			float cloudLatencyAverage = SyncStoreLoadManager.CloudLatencyAverage;
			float storeCloudRatioAverage = SyncStoreLoadManager.StoreCloudRatioAverage;
			float value = this.averageDelay.Value;
			float num = (value + storeLatencyAverage) / (value + storeLatencyAverage + cloudLatencyAverage / storeCloudRatioAverage);
			return (float)base.WorkItemsCount * num;
		}

		protected virtual SyncRPCResourceMonitor CreateSyncRPCMonitor(ResourceKey databaseResourceKey)
		{
			return new SyncRPCResourceMonitor(base.SyncLogSession, databaseResourceKey, SyncResourceMonitorType.DatabaseRPCLatency);
		}

		protected virtual SyncResourceMonitor CreateSyncResourceMonitor(ResourceKey resourceKey, SyncResourceMonitorType syncResourceMonitorType)
		{
			return new SyncResourceMonitor(base.SyncLogSession, resourceKey, syncResourceMonitorType);
		}

		private ResourceKey CreateDatabaseRPCResourceKey()
		{
			return new MdbResourceHealthMonitorKey(this.databaseGuid);
		}

		private ResourceKey CreateDatabaseReplicationResourceKey()
		{
			return new MdbReplicationResourceHealthMonitorKey(this.databaseGuid);
		}

		private static readonly int MaxAcceptedRawRPCLatency = 50;

		private static readonly ushort NumberOfDelaySamples = 100;

		private static readonly ushort NumberOfRPCLatencySamples = 5;

		private static readonly float MarginOfError = 1E-09f;

		private readonly Guid databaseGuid;

		private RunningAverageFloat averageDelay;

		private RunningAverageFloat averageRPCLatency;

		private SyncRPCResourceMonitor rpcLatencyMonitor;
	}
}
