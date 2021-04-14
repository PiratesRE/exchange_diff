using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.SharedCache.Client;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.SharedCache
{
	public sealed class SharedCacheDiscovery : DiscoveryWorkItem
	{
		protected override Trace Trace
		{
			get
			{
				return ExTraceGlobals.CafeTracer;
			}
		}

		protected override void CreateWorkTasks(CancellationToken cancellationToken)
		{
			this.breadcrumbs = new Breadcrumbs(1024, this.traceContext);
			try
			{
				if (!LocalEndpointManager.IsDataCenter)
				{
					this.breadcrumbs.Drop("SharedCacheDiscovery.CreateWorkTasks: Only applicable in datacenter.");
				}
				else if (!LocalEndpointManager.Instance.ExchangeServerRoleEndpoint.IsCafeRoleInstalled)
				{
					this.breadcrumbs.Drop("SharedCacheDiscovery.CreateWorkTasks: Cafe role is not present on this server.");
				}
				else
				{
					this.Configure();
					this.CreateAllWorkItems();
				}
			}
			finally
			{
				this.ReportResult();
			}
		}

		private void CreateAllWorkItems()
		{
			this.CreateWorkItemsInternal("MailboxServerLocator", WellKnownSharedCache.MailboxServerLocator);
			this.CreateWorkItemsInternal("AnchorMailbox", WellKnownSharedCache.AnchorMailboxCache);
		}

		private void CreateWorkItemsInternal(string cacheName, Guid cacheGuid)
		{
			string targetResource = cacheGuid.ToString();
			this.breadcrumbs.Drop("Creating probe identity");
			ProbeIdentity probeIdentity = ProbeIdentity.Create(ExchangeComponent.SharedCache, ProbeType.Service, null, targetResource);
			this.breadcrumbs.Drop("Creating probe definition");
			ProbeDefinition probeDefinition = SharedCacheProbe.CreateDefinition(probeIdentity, this.ProbeRpcTimeoutMilliseconds);
			probeDefinition.RecurrenceIntervalSeconds = this.ProbeRecurrenceIntervalSeconds;
			probeDefinition.TimeoutSeconds = this.ProbeTimeoutSeconds;
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, this.traceContext);
			this.breadcrumbs.Drop("Creating monitor definition");
			MonitorIdentity monitorIdentity = probeIdentity.CreateMonitorIdentity();
			this.breadcrumbs.Drop("Creating monitor definition");
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(monitorIdentity.Name, probeIdentity.GetAlertMask(), monitorIdentity.Component.Name, monitorIdentity.Component, this.ProbeFailureCount, true, 300);
			monitorDefinition.TargetResource = targetResource;
			monitorDefinition.RecurrenceIntervalSeconds = 0;
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded, this.DegradedTransitionSeconds),
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, this.UnrecoverableTransitionSeconds)
			};
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, this.traceContext);
			if (this.RestartServiceResponderEnabled)
			{
				ResponderIdentity responderIdentity = monitorIdentity.CreateResponderIdentity(string.Format("{0}Restart", cacheName), null);
				this.breadcrumbs.Drop("Responder: {0}", new object[]
				{
					responderIdentity.Name
				});
				ResponderDefinition responderDefinition = RestartServiceResponder.CreateDefinition(responderIdentity.Name, monitorIdentity.Name, SharedCacheDiscovery.SharedCacheServiceName, ServiceHealthStatus.Degraded, 15, 120, 0, false, DumpMode.FullDump, null, 15.0, 0, "Exchange", null, true, true, null, false);
				responderDefinition.RecurrenceIntervalSeconds = 0;
				base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, this.traceContext);
			}
			if (this.EscalateResponderEnabled)
			{
				ResponderIdentity responderIdentity = monitorIdentity.CreateResponderIdentity(string.Format("{0}Escalate", cacheName), null);
				this.breadcrumbs.Drop("Responder: {0}", new object[]
				{
					responderIdentity.Name
				});
				ResponderDefinition responderDefinition2 = EscalateResponder.CreateDefinition(responderIdentity.Name, responderIdentity.Component.Name, monitorIdentity.Name, monitorIdentity.GetAlertMask(), responderIdentity.TargetResource, ServiceHealthStatus.Unrecoverable, monitorIdentity.Component.EscalationTeam, Strings.SharedCacheEscalationSubject, Strings.SharedCacheEscalationMessage, true, this.PagingAlertsEnabled ? NotificationServiceClass.Scheduled : NotificationServiceClass.UrgentInTraining, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
				responderDefinition2.RecurrenceIntervalSeconds = 0;
				base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition2, this.traceContext);
			}
		}

		private void ReportResult()
		{
			string text = this.breadcrumbs.ToString();
			base.Result.StateAttribute5 = text;
			WTFDiagnostics.TraceInformation(this.Trace, this.traceContext, text, null, "ReportResult", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\SharedCache\\SharedCacheDiscovery.cs", 194);
		}

		private void Configure()
		{
			this.ProbeFailureCount = this.ReadAttribute("ProbeFailureCount", 3);
			this.ProbeRecurrenceIntervalSeconds = (int)this.ReadAttribute("ProbeRecurrenceInterval", SharedCacheDiscovery.DefaultValues.ProbeRecurrenceInterval).TotalSeconds;
			this.ProbeTimeoutSeconds = (int)this.ReadAttribute("ProbeTimeoutInterval", SharedCacheDiscovery.DefaultValues.ProbeTimeoutInterval).TotalSeconds;
			this.ProbeRpcTimeoutMilliseconds = this.ReadAttribute("ProbeRpcTimeoutMilliseconds", SharedCacheDiscovery.DefaultValues.ProbeRpcTimeoutMilliseconds);
			this.DegradedTransitionSeconds = (int)this.ReadAttribute("DegradedTransitionSpan", SharedCacheDiscovery.DefaultValues.DegradedTransitionSpan).TotalSeconds;
			this.UnrecoverableTransitionSeconds = (int)this.ReadAttribute("UnrecoverableTransitionSpan", SharedCacheDiscovery.DefaultValues.UnrecoverableTransitionSpan).TotalSeconds;
			this.RestartServiceResponderEnabled = this.ReadAttribute("RestartServiceResponderEnabled", false);
			this.EscalateResponderEnabled = this.ReadAttribute("EscalateResponderEnabled", false);
			this.PagingAlertsEnabled = this.ReadAttribute("PagingAlertsEnabled", false);
		}

		private static readonly string SharedCacheServiceName = "MSExchangeSharedCache";

		private Breadcrumbs breadcrumbs;

		private TracingContext traceContext = TracingContext.Default;

		private int ProbeFailureCount;

		private int ProbeRecurrenceIntervalSeconds;

		private int ProbeTimeoutSeconds;

		private int ProbeRpcTimeoutMilliseconds;

		private int DegradedTransitionSeconds;

		private int UnrecoverableTransitionSeconds;

		private bool RestartServiceResponderEnabled;

		private bool EscalateResponderEnabled;

		private bool PagingAlertsEnabled;

		private class DefaultValues
		{
			public const int ProbeFailureCount = 3;

			public static readonly int ProbeRpcTimeoutMilliseconds = 100;

			public static readonly TimeSpan ProbeRecurrenceInterval = TimeSpan.FromSeconds(60.0);

			public static readonly TimeSpan ProbeTimeoutInterval = TimeSpan.FromSeconds(5.0);

			public static readonly TimeSpan DegradedTransitionSpan = TimeSpan.FromSeconds(0.0);

			public static readonly TimeSpan UnrecoverableTransitionSpan = TimeSpan.FromMinutes(15.0);
		}
	}
}
