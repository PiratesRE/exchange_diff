using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Threading;
using Microsoft.Exchange.Transport.Configuration;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class RoutingTablesLoader
	{
		public RoutingTablesLoader(IMailRouter parentRouter)
		{
			RoutingUtils.ThrowIfNull(parentRouter, "parentRouter");
			this.parentRouter = parentRouter;
			this.syncObject = new object();
		}

		public event RoutingTablesChangedHandler RoutingTablesChanged;

		public RoutingTables RoutingTables
		{
			get
			{
				return this.routingTables;
			}
		}

		public bool LoadAndSubscribe(RoutingContextCore context)
		{
			RoutingUtils.ThrowIfNull(context, "context");
			this.context = context;
			RoutingDiag.Tracer.TraceDebug((long)this.GetHashCode(), "Loading configuration information for routing and subscribing");
			this.context.Dependencies.RegisterForLocalServerChanges(new ConfigurationUpdateHandler<TransportServerConfiguration>(this.HandleLocalServerConfigChange));
			if (this.context.IsEdgeMode)
			{
				this.context.EdgeDependencies.RegisterForAcceptedDomainChanges(new ConfigurationUpdateHandler<AcceptedDomainTable>(this.HandleAcceptedDomainChange));
			}
			this.routingTables = null;
			this.databaseLoader = new DatabaseLoader(this.context);
			if (this.context.Settings.DagSelectorEnabled)
			{
				this.tenantDagQuota = new TenantDagQuota(this.context.Settings.TenantDagQuotaDagsPerTenant, this.context.Settings.TenantDagQuotaMessagesPerDag, this.context.Settings.TenantDagQuotaPastWeight, this.context.Settings.DagSelectorLogDiagnosticInfo);
			}
			IList<ADNotificationRequestCookie> list = null;
			try
			{
				int num = 0;
				while (this.routingTables == null)
				{
					ADOperationResult adoperationResult = ADOperationResult.Success;
					RoutingTopologyBase routingTopologyBase = this.CreateTopologyConfig();
					if (list == null)
					{
						adoperationResult = routingTopologyBase.TryRegisterForADNotifications(new ADNotificationCallback(this.HandleADChangeNotification), out list);
					}
					if (adoperationResult.Succeeded)
					{
						adoperationResult = this.TryLoadRoutingTablesAndNotify(routingTopologyBase, false);
					}
					if (!adoperationResult.Succeeded && !this.HandleFailedInitialLoadAttempt(adoperationResult, num))
					{
						RoutingDiag.Tracer.TraceDebug((long)this.GetHashCode(), "Leaving LoadAndSubscribe() because the process is shutting down");
						return false;
					}
					num++;
				}
				lock (this.syncObject)
				{
					if (this.context.Dependencies.IsProcessShuttingDown)
					{
						RoutingDiag.Tracer.TraceDebug((long)this.GetHashCode(), "Leaving LoadAndSubscribe() because the process is shutting down (II)");
						return false;
					}
					Interlocked.Exchange(ref this.lastUpdateTimeTicks, DateTime.UtcNow.Ticks);
					RoutingDiag.Tracer.TraceDebug(0L, "Creating reload timer");
					this.reloadTimer = new GuardedTimer(new TimerCallback(this.ReloadRoutingTables), null, this.context.Settings.ConfigReloadInterval, this.context.Settings.ConfigReloadInterval);
					this.context.Dependencies.RegisterForServiceControlConfigUpdates(new EventHandler(this.ForceReloadRoutingTables));
					this.notificationCookies = list;
					list = null;
				}
			}
			finally
			{
				if (list != null)
				{
					RoutingTopologyBase.UnregisterFromADNotifications(list);
				}
			}
			if (this.routingTableReloadRequired)
			{
				ThreadPool.QueueUserWorkItem(new WaitCallback(this.ReloadRoutingTables));
			}
			return true;
		}

		public void Unsubscribe()
		{
			RoutingDiag.Tracer.TraceDebug((long)this.GetHashCode(), "Unsubscribing from all configuration change notifications");
			GuardedTimer guardedTimer = null;
			IList<ADNotificationRequestCookie> list = null;
			lock (this.syncObject)
			{
				guardedTimer = this.reloadTimer;
				this.reloadTimer = null;
				list = this.notificationCookies;
				this.notificationCookies = null;
			}
			if (list != null)
			{
				RoutingTopologyBase.UnregisterFromADNotifications(list);
			}
			if (guardedTimer != null)
			{
				guardedTimer.Dispose(true);
				this.context.Dependencies.UnregisterFromServiceControlConfigUpdates(new EventHandler(this.ForceReloadRoutingTables));
			}
			if (this.context.IsEdgeMode)
			{
				this.context.EdgeDependencies.UnregisterFromAcceptedDomainChanges(new ConfigurationUpdateHandler<AcceptedDomainTable>(this.HandleAcceptedDomainChange));
			}
			this.context.Dependencies.UnregisterFromLocalServerChanges(new ConfigurationUpdateHandler<TransportServerConfiguration>(this.HandleLocalServerConfigChange));
		}

		public bool TryGetDiagnosticInfo(bool verbose, DiagnosableParameters parameters, out XElement diagnosticInfo)
		{
			if (this.tenantDagQuota == null)
			{
				diagnosticInfo = null;
				return false;
			}
			return this.tenantDagQuota.TryGetDiagnosticInfo(verbose, parameters, out diagnosticInfo);
		}

		private bool HandleFailedInitialLoadAttempt(ADOperationResult result, int attemptNumber)
		{
			if (attemptNumber >= 5)
			{
				RoutingDiag.Tracer.TraceError<int>((long)this.GetHashCode(), "Max number of retries ({0}) to load routing config data reached. Stopping the service.", 5);
				RoutingDiag.EventLogger.LogEvent(TransportEventLogConstants.Tuple_RoutingMaxConfigLoadRetriesReached, null, new object[0]);
				throw new TransportComponentLoadFailedException(Strings.RoutingMaxConfigLoadRetriesReached, result.Exception);
			}
			RoutingDiag.Tracer.TraceError<int>((long)this.GetHashCode(), "Failed to load configuration information for routing; will block the process and retry in {0} seconds", 10);
			RoutingDiag.EventLogger.LogEvent(TransportEventLogConstants.Tuple_RoutingWillRetryLoad, null, new object[]
			{
				10
			});
			for (int i = 0; i < 10; i++)
			{
				if (this.context.Dependencies.IsProcessShuttingDown)
				{
					return false;
				}
				Thread.Sleep(1000);
			}
			return true;
		}

		private ADOperationResult TryLoadRoutingTablesAndNotify(RoutingTopologyBase topologyConfig, bool forcedReload)
		{
			RoutingDiag.Tracer.TraceDebug((long)this.GetHashCode(), "Loading routing tables");
			RoutingTables newTables = null;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				topologyConfig.PreLoad();
				newTables = new RoutingTables(topologyConfig, this.context, this.tenantDagQuota, forcedReload);
			}, 0);
			if (!adoperationResult.Succeeded)
			{
				TransientRoutingException ex = adoperationResult.Exception as TransientRoutingException;
				if (ex != null)
				{
					RoutingDiag.Tracer.TraceError<DateTime, TransientRoutingException>((long)this.GetHashCode(), "[{0}] RoutingTables load failed with transient error. Exception details: {1}", topologyConfig.WhenCreated, ex);
					RoutingDiag.EventLogger.LogEvent(TransportEventLogConstants.Tuple_RoutingTransientConfigError, null, new object[]
					{
						ex.LocalizedString,
						ex
					});
				}
				else
				{
					RoutingDiag.Tracer.TraceError<Exception>((long)this.GetHashCode(), "Failed to load Routing tables; AD Exception occurred: {0}", adoperationResult.Exception);
					RoutingDiag.EventLogger.LogEvent(TransportEventLogConstants.Tuple_RoutingAdUnavailable, null, new object[0]);
				}
				return adoperationResult;
			}
			this.context.PerfCounters.IncrementRoutingTablesCalculated();
			bool flag;
			bool flag2;
			this.MatchAndReplaceRoutingTables(newTables, out flag, out flag2);
			if (flag2)
			{
				ThreadPool.QueueUserWorkItem(new WaitCallback(this.LogRoutingTables), Tuple.Create<RoutingTables, RoutingTopologyBase>(newTables, topologyConfig));
				if (!flag)
				{
					this.context.PerfCounters.IncrementRoutingTablesChanged();
				}
				RoutingTablesChangedHandler routingTablesChanged = this.RoutingTablesChanged;
				if (routingTablesChanged != null)
				{
					routingTablesChanged(this.parentRouter, newTables.WhenCreated, !flag);
				}
			}
			return adoperationResult;
		}

		private void MatchAndReplaceRoutingTables(RoutingTables newTables, out bool matched, out bool replaced)
		{
			replaced = false;
			matched = false;
			RoutingTables routingTables;
			lock (this.syncObject)
			{
				routingTables = this.routingTables;
				if (this.context.Dependencies.IsProcessShuttingDown)
				{
					if (routingTables == null)
					{
						this.routingTables = newTables;
					}
					return;
				}
				if (routingTables == null || newTables.WhenCreated > routingTables.WhenCreated)
				{
					if (routingTables != null)
					{
						matched = routingTables.Match(newTables);
					}
					this.routingTables = newTables;
					replaced = true;
				}
			}
			if (replaced)
			{
				RoutingDiag.Tracer.TraceDebug<object, DateTime>((long)this.GetHashCode(), "Replaced routing tables [{0}] with [{1}]", (routingTables == null) ? "null" : routingTables.WhenCreated, newTables.WhenCreated);
				if (this.context.ServerRoutingSupported && routingTables != null)
				{
					routingTables.DatabaseMap.LogDagSelectorInfo();
					return;
				}
			}
			else
			{
				RoutingDiag.Tracer.TraceDebug<object, DateTime>((long)this.GetHashCode(), "Routing tables [{0}] are newer than [{1}] and were not replaced", (routingTables == null) ? "null" : routingTables.WhenCreated, newTables.WhenCreated);
			}
		}

		private RoutingTopologyBase CreateTopologyConfig()
		{
			if (!this.context.IsEdgeMode)
			{
				return new RoutingTopology(this.databaseLoader, this.context);
			}
			return new EdgeRoutingTopology();
		}

		private void HandleLocalServerConfigChange(TransportServerConfiguration transportServerConfiguration)
		{
			RoutingDiag.Tracer.TraceDebug((long)this.GetHashCode(), "HandleLocalServerConfigChange() callback invoked");
			RoutingTableLogFileManager.HandleTransportServerConfigChange(transportServerConfiguration);
		}

		private void HandleAcceptedDomainChange(AcceptedDomainTable acceptedDomainTable)
		{
			RoutingDiag.Tracer.TraceDebug((long)this.GetHashCode(), "HandleLocalAcceptedDomainChange() callback invoked");
			this.HandleConfigurationChange();
		}

		private void HandleADChangeNotification(ADNotificationEventArgs args)
		{
			RoutingDiag.Tracer.TraceDebug<ADObjectId>((long)this.GetHashCode(), "HandleAdChangeNotification() callback invoked due to {0}", args.Id);
			if (this.context.IsEdgeMode && this.context.EdgeDependencies.IsLocalServerId(args.Id))
			{
				return;
			}
			this.HandleConfigurationChange();
		}

		private void ForceReloadRoutingTables(object source, EventArgs args)
		{
			RoutingDiag.Tracer.TraceDebug((long)this.GetHashCode(), "ForceReloadRoutingTables() callback invoked");
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.ReloadRoutingTables), true);
		}

		private void ReloadRoutingTables(object state)
		{
			RoutingDiag.Tracer.TraceDebug((long)this.GetHashCode(), "ReloadRoutingTables() callback invoked");
			if (this.context.Dependencies.IsProcessShuttingDown)
			{
				RoutingDiag.Tracer.TraceDebug((long)this.GetHashCode(), "Bailing out of ReloadRoutingTables() because the process is shutting down");
				return;
			}
			Interlocked.Exchange(ref this.deferredNotificationCount, 0);
			Interlocked.Exchange(ref this.lastUpdateTimeTicks, DateTime.UtcNow.Ticks);
			this.TryLoadRoutingTablesAndNotify(this.CreateTopologyConfig(), state != null && (bool)state);
		}

		private void HandleConfigurationChange()
		{
			try
			{
				if (Interlocked.Increment(ref this.notificationHandlerCount) == 1 && Interlocked.Increment(ref this.deferredNotificationCount) <= this.context.Settings.MaxDeferredNotifications)
				{
					GuardedTimer guardedTimer = this.reloadTimer;
					if (guardedTimer != null)
					{
						TimeSpan timeSpan = this.CalculateDelayForNextUpdate();
						RoutingDiag.Tracer.TraceDebug<TimeSpan>((long)this.GetHashCode(), "Scheduling config reload to happen after {0}", timeSpan);
						guardedTimer.Change(timeSpan, this.context.Settings.ConfigReloadInterval);
					}
					else
					{
						this.routingTableReloadRequired = true;
					}
				}
			}
			finally
			{
				Interlocked.Decrement(ref this.notificationHandlerCount);
			}
		}

		private TimeSpan CalculateDelayForNextUpdate()
		{
			TimeSpan deferredReloadInterval = this.context.Settings.DeferredReloadInterval;
			TimeSpan minConfigReloadInterval = this.context.Settings.MinConfigReloadInterval;
			long num = Interlocked.Read(ref this.lastUpdateTimeTicks);
			TimeSpan timeSpan = (num != 0L) ? new TimeSpan(num + minConfigReloadInterval.Ticks - DateTime.UtcNow.Ticks) : TimeSpan.Zero;
			if (!(deferredReloadInterval > timeSpan))
			{
				return timeSpan;
			}
			return deferredReloadInterval;
		}

		private void LogRoutingTables(object routingInfoObj)
		{
			Tuple<RoutingTables, RoutingTopologyBase> tuple = (Tuple<RoutingTables, RoutingTopologyBase>)routingInfoObj;
			RoutingTableLogger.LogRoutingTables(tuple.Item1, tuple.Item2, this.context);
		}

		private const int MaxLoadRetryCount = 5;

		private const int LoadRetryIntervalSeconds = 10;

		private IMailRouter parentRouter;

		private RoutingContextCore context;

		private RoutingTables routingTables;

		private DatabaseLoader databaseLoader;

		private TenantDagQuota tenantDagQuota;

		private IList<ADNotificationRequestCookie> notificationCookies;

		private GuardedTimer reloadTimer;

		private int deferredNotificationCount;

		private int notificationHandlerCount;

		private long lastUpdateTimeTicks;

		private bool routingTableReloadRequired;

		private object syncObject;
	}
}
