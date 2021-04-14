using System;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ContentAggregation;
using Microsoft.Exchange.MailboxTransport.ContentAggregation;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Rpc.Submission;

namespace Microsoft.Exchange.Transport.Sync.Worker.Throttling
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class SyncResource
	{
		public SyncResource(SyncLogSession syncLogSession, string resourceId)
		{
			SyncUtilities.ThrowIfArgumentNull("syncLogSession", syncLogSession);
			SyncUtilities.ThrowIfArgumentNullOrEmpty("resourceId", resourceId);
			this.SyncLogSession = syncLogSession;
			this.ResourceId = resourceId;
			this.CreateSlidingWindows();
		}

		internal string ResourceId { get; private set; }

		private protected SyncLogSession SyncLogSession { protected get; private set; }

		protected int WorkItemsCount
		{
			get
			{
				return this.workItemsCount;
			}
		}

		protected abstract int MaxConcurrentWorkInUnknownState { get; }

		protected abstract SubscriptionSubmissionResult ResourceHealthUnknownResult { get; }

		protected abstract SubscriptionSubmissionResult MaxConcurrentWorkAgainstResourceLimitReachedResult { get; }

		internal bool TryAcceptWorkItem(AggregationWorkItem workItem, out SubscriptionSubmissionResult result)
		{
			lock (this.syncObject)
			{
				if (this.CanAcceptWorkItem(workItem, out result))
				{
					this.AddWorkItem(workItem);
					return true;
				}
			}
			return false;
		}

		internal SyncResourceMonitor[] GetResourceMonitors()
		{
			return this.resourceHealthMonitors;
		}

		internal virtual void RemoveWorkItem(AggregationWorkItem workItem)
		{
			lock (this.syncObject)
			{
				this.workItemsCount--;
			}
		}

		internal virtual void UpdateDelay(int delay)
		{
			this.slidingDelayCount.Add((uint)delay);
		}

		internal int GetSuggestedDelay(int originalDelay)
		{
			int num = Math.Max(1, this.workItemsCount);
			return Math.Max(1, originalDelay / num);
		}

		protected abstract SyncResourceMonitor[] InitializeHealthMonitoring();

		protected abstract SubscriptionSubmissionResult GetResultForResourceUnhealthy(SyncResourceMonitorType syncResourceMonitorType);

		protected abstract bool CanAcceptWorkBasedOnResourceSpecificChecks(out SubscriptionSubmissionResult result);

		protected virtual void AddWorkItem(AggregationWorkItem workItem)
		{
			this.workItemsCount++;
			this.UpdateConcurrency(1);
		}

		protected void Initialize()
		{
			this.resourceHealthMonitors = this.InitializeHealthMonitoring();
		}

		protected bool CanAddOneMoreConcurrentRequestToResource()
		{
			int suggestedConcurrency = this.GetSuggestedConcurrency();
			int num = this.WorkItemsCount;
			uint delaySum = this.GetDelaySum();
			if (delaySum > 0U && num >= suggestedConcurrency && num > this.MaxConcurrentWorkInUnknownState)
			{
				this.SyncLogSession.LogError((TSLID)1529UL, ExTraceGlobals.SchedulerTracer, "CanAddOneMoreConcurrentRequestToResource: Cannot accept WI. {0} has concurrency {1} and suggested concurrency {2}.", new object[]
				{
					this.ResourceId,
					num,
					suggestedConcurrency
				});
				return false;
			}
			this.SyncLogSession.LogVerbose((TSLID)330UL, ExTraceGlobals.SchedulerTracer, "CanAddOneMoreConcurrentRequestToResource: For resource {0}, actual concurrency {1}, effective concurrency: {2}", new object[]
			{
				this.ResourceId,
				num,
				suggestedConcurrency
			});
			return true;
		}

		private bool CanAcceptWorkItem(AggregationWorkItem workItem, out SubscriptionSubmissionResult result)
		{
			bool flag;
			bool flag2;
			SyncResourceMonitorType syncResourceMonitorType = this.IsAnyResourceUnhealthyOrUnknown(workItem, out flag, out flag2);
			if (flag)
			{
				result = this.GetResultForResourceUnhealthy(syncResourceMonitorType);
				this.SyncLogSession.LogError((TSLID)1441UL, ExTraceGlobals.SchedulerTracer, "CanAcceptWorkItem: Cannot accept WI. {0} Unhealthy. WI Count: {1}. Result {2}", new object[]
				{
					this.ResourceId,
					this.WorkItemsCount,
					result
				});
				return false;
			}
			if (flag2)
			{
				if (this.WorkItemsCount >= this.MaxConcurrentWorkInUnknownState)
				{
					result = this.ResourceHealthUnknownResult;
					this.SyncLogSession.LogError((TSLID)1528UL, ExTraceGlobals.SchedulerTracer, "CanAcceptWorkItem: Cannot accept WI. {0} has unknown health. WI Count: {1}. Result {2}", new object[]
					{
						this.ResourceId,
						this.WorkItemsCount,
						result
					});
					return false;
				}
				this.SyncLogSession.LogVerbose((TSLID)1466UL, ExTraceGlobals.SchedulerTracer, "CanAcceptWorkItem: WI can be accepted on {0} since we have not reached MaxConcurrentWorkInUnknownState {1}. WI Count: {2}.", new object[]
				{
					this.ResourceId,
					this.MaxConcurrentWorkInUnknownState,
					this.WorkItemsCount
				});
				result = SubscriptionSubmissionResult.Success;
				return true;
			}
			else
			{
				if (!this.CanAcceptWorkBasedOnResourceSpecificChecks(out result))
				{
					return false;
				}
				this.SyncLogSession.LogVerbose((TSLID)962UL, ExTraceGlobals.SchedulerTracer, "CanAcceptWorkItem: WI can be accepted on {0}. All checks passed.", new object[]
				{
					this.ResourceId
				});
				result = SubscriptionSubmissionResult.Success;
				return true;
			}
		}

		private SyncResourceMonitorType IsAnyResourceUnhealthyOrUnknown(AggregationWorkItem workItem, out bool isAnyResourceUnhealthy, out bool isAnyResourceUnknown)
		{
			SyncResourceMonitor[] resourceMonitors = this.GetResourceMonitors();
			return SyncResourceMonitor.IsAnyResourceUnhealthyOrUnknown(workItem, resourceMonitors, out isAnyResourceUnhealthy, out isAnyResourceUnknown);
		}

		private void CreateSlidingWindows()
		{
			this.slidingConcurrencyAverage = new FixedTimeAverage(1000, 60, Environment.TickCount);
			this.slidingDelayCount = new FixedTimeSum(1000, 60);
		}

		private int GetSuggestedConcurrency()
		{
			if (AggregationConfiguration.Instance.SuggestedConcurrencyOverride != null)
			{
				return AggregationConfiguration.Instance.SuggestedConcurrencyOverride.Value;
			}
			double num = (double)this.slidingConcurrencyAverage.GetValue();
			uint value = this.slidingDelayCount.GetValue();
			if (num == 0.0)
			{
				num = (double)this.WorkItemsCount;
			}
			double num2 = 60000.0;
			double num3 = num2 * num - value;
			int num4 = (int)(num3 / num2 + 1.5);
			this.SyncLogSession.LogVerbose((TSLID)1530UL, ExTraceGlobals.SchedulerTracer, "AcceptCheck: For {0}. Current concurrency {1} and suggested concurrency {2} (greed). Calculated average {3}. DelaySum: {4}. Suggested concurrency before truncation: {5}.", new object[]
			{
				base.GetType().Name,
				this.WorkItemsCount,
				num4,
				num,
				value,
				num3 / num2 + 1.5
			});
			return num4;
		}

		private uint GetDelaySum()
		{
			return this.slidingDelayCount.GetValue();
		}

		private void UpdateConcurrency(int increment)
		{
			this.slidingConcurrencyAverage.Add(Environment.TickCount, (uint)(this.WorkItemsCount + increment));
		}

		private const ushort SlidingWindowSize = 60;

		private const ushort SlidingWindowBucketSize = 1000;

		private readonly object syncObject = new object();

		private SyncResourceMonitor[] resourceHealthMonitors;

		private FixedTimeAverage slidingConcurrencyAverage;

		private FixedTimeSum slidingDelayCount;

		private int workItemsCount;
	}
}
