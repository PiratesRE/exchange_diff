using System;
using System.ServiceModel;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Data.Storage.ResourceHealth;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class MicroDelayScope : DisposeTrackableBase
	{
		private MicroDelayScope(MailboxReplicationProxyService mrsProxy, params ResourceKey[] resources)
		{
			this.startedCallProcessingAt = ExDateTime.UtcNow;
			this.resources = resources;
			this.budget = StandardBudget.AcquireUnthrottledBudget("MrsProxyBudget", BudgetType.ResourceTracking);
			this.workLoadSettings = (mrsProxy.IsHighPriority ? CommonUtils.WorkloadSettingsHighPriority : CommonUtils.WorkloadSettings);
			this.skipWLMThrottling = (!ResourceHealthMonitorManager.Active || !TestIntegration.Instance.MicroDelayEnabled || mrsProxy.SkipWLMThrottling || mrsProxy.IsInFinalization);
			bool flag = false;
			try
			{
				if (!this.skipWLMThrottling)
				{
					ResourceLoadDelayInfo.CheckResourceHealth(this.budget, this.workLoadSettings, this.resources);
				}
				this.budget.StartConnection("MailboxReplicationService.MicroDelayScope.MicroDelayScope");
				this.budget.StartLocal("MailboxReplicationService.MicroDelayScope.MicroDelayScope", default(TimeSpan));
				if (!ActivityContext.IsStarted)
				{
					this.scope = ActivityContext.Start(null);
					this.scope.Action = "MailboxReplicationProxyService";
					if (OperationContext.Current != null)
					{
						this.scope.UpdateFromMessage(OperationContext.Current);
					}
					this.scope.UserId = mrsProxy.ExchangeGuid.ToString();
					if (mrsProxy.ClientVersion != null)
					{
						this.scope.ClientInfo = mrsProxy.ClientVersion.ComputerName;
					}
				}
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					this.SuppressDisposeTracker();
					if (this.budget != null)
					{
						this.budget.Dispose();
						this.budget = null;
					}
					if (this.scope != null)
					{
						this.scope.End();
						this.scope = null;
					}
				}
			}
		}

		public static MicroDelayScope Create(MailboxReplicationProxyService mrsProxy, DelayScopeKind kind)
		{
			Guid guid = ConfigBase<MRSConfigSchema>.CurrentContext.DatabaseGuid ?? Guid.Empty;
			if (guid == Guid.Empty && (kind == DelayScopeKind.DbRead || kind == DelayScopeKind.DbWrite))
			{
				kind = DelayScopeKind.CPUOnly;
			}
			switch (kind)
			{
			case DelayScopeKind.CPUOnly:
				return new MicroDelayScope(mrsProxy, MicroDelayScope.LocalCpuResourcesOnly);
			case DelayScopeKind.DbRead:
				if (mrsProxy.IsE15OrHigher)
				{
					return new MicroDelayScope(mrsProxy, new ResourceKey[]
					{
						ProcessorResourceKey.Local,
						new MdbResourceHealthMonitorKey(guid),
						new DiskLatencyResourceKey(guid)
					});
				}
				return new MicroDelayScope(mrsProxy, new ResourceKey[]
				{
					ProcessorResourceKey.Local,
					new MdbResourceHealthMonitorKey(guid)
				});
			case DelayScopeKind.DbWrite:
				if (mrsProxy.IsE15OrHigher)
				{
					return new MicroDelayScope(mrsProxy, new ResourceKey[]
					{
						ProcessorResourceKey.Local,
						new MdbResourceHealthMonitorKey(guid),
						new MdbReplicationResourceHealthMonitorKey(guid),
						new MdbAvailabilityResourceHealthMonitorKey(guid),
						new CiAgeOfLastNotificationResourceKey(guid),
						new DiskLatencyResourceKey(guid)
					});
				}
				return new MicroDelayScope(mrsProxy, new ResourceKey[]
				{
					ProcessorResourceKey.Local,
					new MdbResourceHealthMonitorKey(guid),
					new LegacyResourceHealthMonitorKey(guid)
				});
			}
			return null;
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.budget != null)
				{
					if (!this.skipWLMThrottling)
					{
						TimeSpan timeSpan = MicroDelayScope.maxCallProcessingTime - (ExDateTime.UtcNow - this.startedCallProcessingAt);
						if (timeSpan < TimeSpan.Zero)
						{
							timeSpan = TimeSpan.Zero;
						}
						DelayEnforcementResults delayEnforcement = ResourceLoadDelayInfo.EnforceDelay(this.budget, this.workLoadSettings, this.resources, timeSpan, null);
						this.TraceDelay(delayEnforcement);
					}
					this.budget.Dispose();
					this.budget = null;
				}
				if (this.scope != null)
				{
					this.scope.End();
					this.scope = null;
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MicroDelayScope>(this);
		}

		private void TraceDelay(DelayEnforcementResults delayEnforcement)
		{
			if (delayEnforcement.DelayInfo != DelayInfo.NoDelay)
			{
				MrsTracer.ResourceHealth.Debug("Micro Delay: {0} msec due to resource: '{1}'", new object[]
				{
					delayEnforcement.DelayedAmount.TotalMilliseconds,
					(delayEnforcement.DelayInfo as ResourceLoadDelayInfo).ResourceKey
				});
			}
		}

		private const string CallerDescription = "MailboxReplicationService.MicroDelayScope.MicroDelayScope";

		private static readonly TimeSpan maxCallProcessingTime = TimeSpan.FromSeconds(40.0);

		private static readonly ResourceKey[] LocalCpuResourcesOnly = new ResourceKey[]
		{
			ProcessorResourceKey.Local
		};

		private readonly ExDateTime startedCallProcessingAt;

		private readonly bool skipWLMThrottling;

		private ResourceKey[] resources;

		private IStandardBudget budget;

		private IActivityScope scope;

		private WorkloadSettings workLoadSettings;
	}
}
