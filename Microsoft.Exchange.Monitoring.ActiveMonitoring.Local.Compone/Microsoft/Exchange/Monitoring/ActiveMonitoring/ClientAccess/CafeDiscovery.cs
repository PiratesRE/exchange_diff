using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ClientAccess
{
	public sealed class CafeDiscovery : DiscoveryWorkItem
	{
		public int ProbeRecurrenceIntervalSeconds { get; private set; }

		public int MonitoringIntervalSeconds { get; private set; }

		public int MonitoringRecurrenceIntervalSeconds { get; private set; }

		public int ProbeTimeoutSeconds { get; private set; }

		public int Degraded1TransitionSeconds { get; private set; }

		public int DegradedTransitionSeconds { get; private set; }

		public int UnhealthyTransitionSeconds { get; private set; }

		public int Unhealthy1TransitionSeconds { get; private set; }

		public int Unhealthy2TransitionSeconds { get; private set; }

		public int UnrecoverableTransitionSeconds { get; private set; }

		public int IISRecycleRetryCount { get; private set; }

		public int IISRecycleRetryIntervalSeconds { get; private set; }

		public int ClearLsassCacheIntervalSeconds { get; private set; }

		public int FailedProbeThreshold { get; private set; }

		public bool IsIISRecycleResponderEnabled { get; private set; }

		public bool IsClearLsassCacheResponderEnabled { get; private set; }

		public bool IsAlertResponderEnabled { get; private set; }

		public bool IsOfflineResponderEnabled { get; private set; }

		public bool IsOfflineFailedAlertResponderEnabled { get; private set; }

		public bool AlertResponderCorrelationEnabled { get; private set; }

		public bool IsRebootResponderEnabled { get; private set; }

		public bool Verbose { get; private set; }

		public bool TrustAnySslCertificate { get; private set; }

		public bool CreateRespondersForTest { get; private set; }

		public bool EnablePagedAlerts { get; private set; }

		private Dictionary<HttpProtocol, bool> EnabledProbes { get; set; }

		public double OfflineResponderFractionOverride { get; private set; }

		protected override Trace Trace
		{
			get
			{
				return ExTraceGlobals.CafeTracer;
			}
		}

		private static string CafeArrayName
		{
			get
			{
				if (CafeDiscovery.cafeArrayName == null)
				{
					if (CafeDiscovery.AdSession == null)
					{
						throw new ApplicationException("Couldn't create ADSession.");
					}
					Server server = CafeDiscovery.AdSession.FindLocalServer();
					ADObjectId adobjectId = (ADObjectId)server[ServerSchema.ClientAccessArray];
					if (adobjectId != null)
					{
						QueryFilter filter = QueryFilter.AndTogether(new QueryFilter[]
						{
							new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Id, adobjectId),
							QueryFilter.NotFilter(ClientAccessArray.PriorTo15ExchangeObjectVersionFilter)
						});
						ClientAccessArray clientAccessArray = CafeDiscovery.AdSession.FindUnique<ClientAccessArray>(null, QueryScope.SubTree, filter);
						if (clientAccessArray != null)
						{
							CafeDiscovery.cafeArrayName = clientAccessArray.Name;
						}
					}
					if (CafeDiscovery.cafeArrayName == null)
					{
						CafeDiscovery.cafeArrayName = Strings.CafeArrayNameCouldNotBeRetrieved;
					}
				}
				return CafeDiscovery.cafeArrayName;
			}
		}

		private static ITopologyConfigurationSession AdSession
		{
			get
			{
				if (CafeDiscovery.adSession == null)
				{
					CafeDiscovery.adSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 252, "AdSession", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Cafe\\CafeDiscovery.cs");
				}
				return CafeDiscovery.adSession;
			}
		}

		protected override void CreateWorkTasks(CancellationToken cancellationToken)
		{
			this.cancellationToken = cancellationToken;
			TracingContext @default = TracingContext.Default;
			this.breadcrumbs = new Breadcrumbs(1024, @default);
			try
			{
				this.breadcrumbs.Drop("CafeDiscovery.CreateWorkTasks: start.");
				LocalEndpointManager instance = LocalEndpointManager.Instance;
				if (!instance.ExchangeServerRoleEndpoint.IsCafeRoleInstalled)
				{
					this.breadcrumbs.Drop("CafeDiscovery.CreateWorkTasks: Cafe role is not present on this server.");
				}
				else if (instance.MailboxDatabaseEndpoint == null || instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForCafe.Count == 0)
				{
					this.breadcrumbs.Drop("CafeDiscovery.CreateWorkTasks: mailbox database collection is empty on this server");
				}
				else
				{
					MailboxDatabaseInfo mailboxDatabaseInfo = instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForCafe.FirstOrDefault((MailboxDatabaseInfo db) => !string.IsNullOrWhiteSpace(db.MonitoringAccountPassword));
					if (mailboxDatabaseInfo != null)
					{
						this.breadcrumbs.Drop("CafeDiscovery.CreateWorkTasks: using mailbox database {0}, user {1}@{2}", new object[]
						{
							mailboxDatabaseInfo.MailboxDatabaseName,
							mailboxDatabaseInfo.MonitoringAccount,
							mailboxDatabaseInfo.MonitoringAccountDomain
						});
						this.Configure(@default);
						this.CreateProbesAndMonitors(@default, mailboxDatabaseInfo);
						this.breadcrumbs.Drop("CafeDiscovery.CreateWorkTasks: end.");
					}
					else
					{
						this.breadcrumbs.Drop("CafeDiscovery.CreateWorkTasks: No probes created! No suitable monitoring mailbox was found.");
					}
				}
			}
			finally
			{
				this.ReportResult(@default);
			}
		}

		private void Configure(TracingContext traceContext)
		{
			this.Verbose = this.ReadAttribute("Verbose", false);
			this.ProbeRecurrenceIntervalSeconds = (int)this.ReadAttribute("ProbeRecurrenceSpan", TimeSpan.FromSeconds(15.0)).TotalSeconds;
			this.ProbeTimeoutSeconds = (int)this.ReadAttribute("ProbeTimeoutSpan", TimeSpan.FromSeconds(15.0)).TotalSeconds;
			this.DegradedTransitionSeconds = (int)this.ReadAttribute("DegradedTransitionSpan", TimeSpan.Zero).TotalSeconds;
			this.Degraded1TransitionSeconds = (int)this.ReadAttribute("Degraded1TransitionSpan", TimeSpan.Zero).TotalSeconds;
			this.UnhealthyTransitionSeconds = (int)this.ReadAttribute("UnhealthyTransitionSpan", TimeSpan.FromMinutes(7.0)).TotalSeconds;
			this.Unhealthy1TransitionSeconds = (int)this.ReadAttribute("Unhealthy1TransitionSpan", TimeSpan.FromMinutes(8.0)).TotalSeconds;
			this.Unhealthy2TransitionSeconds = (int)this.ReadAttribute("Unhealthy2TransitionSpan", TimeSpan.FromMinutes(15.0)).TotalSeconds;
			this.UnrecoverableTransitionSeconds = (int)this.ReadAttribute("UnrecoverableTransitionSpan", TimeSpan.FromMinutes(60.0)).TotalSeconds;
			this.IISRecycleRetryCount = this.ReadAttribute("IISRecycleRetryCount", 1);
			this.IISRecycleRetryIntervalSeconds = (int)this.ReadAttribute("IISRecycleRetrySpan", TimeSpan.FromSeconds(30.0)).TotalSeconds;
			this.ClearLsassCacheIntervalSeconds = (int)this.ReadAttribute("ClearLsassCacheIntervalSpan", TimeSpan.FromSeconds(30.0)).TotalSeconds;
			this.FailedProbeThreshold = this.ReadAttribute("FailedProbeThreshold", 3);
			this.TrustAnySslCertificate = this.ReadAttribute("TrustAnySslCertificate", false);
			this.EnablePagedAlerts = this.ReadAttribute("EnablePagedAlerts", true);
			this.MonitoringIntervalSeconds = (int)this.ReadAttribute("MonitoringIntervalSpan", TimeSpan.FromSeconds(60.0)).TotalSeconds;
			this.MonitoringRecurrenceIntervalSeconds = (int)this.ReadAttribute("MonitoringRecurrenceIntervalSpan", TimeSpan.FromSeconds(0.0)).TotalSeconds;
			this.CreateRespondersForTest = this.ReadAttribute("CreateRespondersForTest", false);
			this.OfflineResponderFractionOverride = this.ReadAttribute("OfflineResponderFractionOverride", -1.0);
			this.AlertResponderCorrelationEnabled = (LocalEndpointManager.IsDataCenter && this.ReadAttribute("AlertResponderCorrelationEnabled", true));
			this.IsIISRecycleResponderEnabled = this.ReadAttribute("IISRecycleResponderEnabled", !ExEnvironment.IsTest);
			this.IsAlertResponderEnabled = this.ReadAttribute("AlertResponderEnabled", !ExEnvironment.IsTest);
			this.IsOfflineResponderEnabled = this.ReadAttribute("OfflineResponderEnabled", !ExEnvironment.IsTest);
			this.IsOfflineFailedAlertResponderEnabled = this.ReadAttribute("OfflineFailedAlertResponderEnabled", !ExEnvironment.IsTest);
			this.IsRebootResponderEnabled = this.ReadAttribute("RebootResponderEnabled", !ExEnvironment.IsTest);
			this.IsClearLsassCacheResponderEnabled = this.ReadAttribute("ClearLsassCacheResponderEnabled", !ExEnvironment.IsTest);
			if (LocalEndpointManager.IsDataCenter)
			{
				this.IsOfflineResponderEnabled = this.ReadAttribute("OfflineResponderEnabledInDC", false);
			}
			this.EnabledProbes = new Dictionary<HttpProtocol, bool>(10);
			for (int i = 0; i < 13; i++)
			{
				HttpProtocol httpProtocol = (HttpProtocol)i;
				string key = httpProtocol.ToString() + "ProbeEnabled";
				bool value = CafeDiscovery.IsProtocolAvailableInEnvironment(httpProtocol);
				bool flag;
				if (base.Definition.Attributes.ContainsKey(key) && bool.TryParse(base.Definition.Attributes[key], out flag))
				{
					value = flag;
				}
				this.EnabledProbes.Add(httpProtocol, value);
			}
			if (!LocalEndpointManager.IsDataCenter)
			{
				CafeProtocols.VirtualDirectories = from item in CafeDiscovery.AdSession.FindPaged<ADVirtualDirectory>(CafeDiscovery.AdSession.FindLocalServer().Id, QueryScope.SubTree, null, null, 0).ToList<ADVirtualDirectory>()
				where item.Identity.ToString().Contains("Default Web Site")
				select item;
			}
			this.breadcrumbs.Drop("CafeDiscovery.Configure: XML parameters read.");
		}

		private void ReportResult(TracingContext traceContext)
		{
			string text = this.breadcrumbs.ToString();
			base.Result.StateAttribute5 = text;
			WTFDiagnostics.TraceInformation(ExTraceGlobals.CafeTracer, traceContext, text, null, "ReportResult", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Cafe\\CafeDiscovery.cs", 413);
		}

		private void CreateProbesAndMonitors(TracingContext traceContext, MailboxDatabaseInfo dbInfo)
		{
			foreach (ProtocolDescriptor protocolDescriptor in CafeProtocols.Protocols)
			{
				if (this.cancellationToken.IsCancellationRequested)
				{
					this.breadcrumbs.Drop("CafeDiscovery.CreateProbesAndMonitors: cancelled by manager.");
					throw new OperationCanceledException(this.cancellationToken);
				}
				bool flag = this.EnabledProbes[protocolDescriptor.HttpProtocol];
				if (flag)
				{
					this.breadcrumbs.Drop("CafeDiscovery.AddProbesAndMonitors: adding {0}", new object[]
					{
						protocolDescriptor.HttpProtocol
					});
					this.AddWorkItemsForProtocol(protocolDescriptor, dbInfo, traceContext);
				}
				else
				{
					this.breadcrumbs.Drop("CafeDiscovery.AddProbesAndMonitors: skipped {0}", new object[]
					{
						protocolDescriptor.HttpProtocol
					});
				}
			}
		}

		private void AddWorkItemsForProtocol(ProtocolDescriptor protocol, MailboxDatabaseInfo dbInfo, TracingContext traceContext)
		{
			ProbeIdentity probeIdentity = ProbeIdentity.Create(protocol.HealthSet, ProbeType.ProxyTest, null, protocol.AppPool);
			probeIdentity = this.AddProbe(probeIdentity, dbInfo, traceContext);
			this.AddMonitor(protocol, probeIdentity, traceContext);
		}

		private ProbeIdentity AddProbe(ProbeIdentity probeIdentity, MailboxDatabaseInfo dbInfo, TracingContext traceContext)
		{
			ProbeDefinition probeDefinition = CafeLocalProbe.CreateDefinition(dbInfo, probeIdentity, CafeDiscovery.ProbeEndPoint);
			probeDefinition.RecurrenceIntervalSeconds = this.ProbeRecurrenceIntervalSeconds;
			probeDefinition.TimeoutSeconds = this.ProbeTimeoutSeconds;
			this.CopyAttributes(CafeDiscovery.ProbeAttributes, probeDefinition);
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, traceContext);
			return probeDefinition;
		}

		private void AddMonitor(ProtocolDescriptor protocol, ProbeIdentity probeIdentity, TracingContext traceContext)
		{
			MonitorIdentity monitorIdentity = probeIdentity.CreateMonitorIdentity();
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(monitorIdentity.Name, probeIdentity.GetAlertMask(), monitorIdentity.Component.Name, monitorIdentity.Component, this.FailedProbeThreshold, true, 300);
			monitorDefinition.RecurrenceIntervalSeconds = this.MonitoringRecurrenceIntervalSeconds;
			monitorDefinition.TimeoutSeconds = this.ProbeTimeoutSeconds;
			monitorDefinition.MaxRetryAttempts = 0;
			monitorDefinition.TargetResource = monitorIdentity.TargetResource;
			monitorDefinition.MonitoringIntervalSeconds = this.MonitoringIntervalSeconds;
			monitorDefinition.IsHaImpacting = this.ShouldCreateOfflineResponder(monitorIdentity.Component);
			monitorDefinition.MonitorStateTransitions = this.GetMonitorStateTransitions();
			monitorDefinition.ServicePriority = protocol.ProtocolPriority;
			monitorDefinition.ScenarioDescription = string.Format("Validate {0} protocal health on FE is not impacted by any issues", protocol.VirtualDirectory);
			this.CreateResponderChain(protocol, monitorDefinition, traceContext);
			if (this.AlertResponderCorrelationEnabled && protocol.HttpProtocol == HttpProtocol.EWS)
			{
				monitorDefinition.AllowCorrelationToMonitor = true;
			}
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, traceContext);
		}

		private MonitorStateTransition[] GetMonitorStateTransitions()
		{
			return new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded, this.DegradedTransitionSeconds),
				new MonitorStateTransition(ServiceHealthStatus.Degraded1, this.Degraded1TransitionSeconds),
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, this.UnhealthyTransitionSeconds),
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy1, this.Unhealthy1TransitionSeconds),
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy2, this.Unhealthy2TransitionSeconds),
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, this.UnrecoverableTransitionSeconds)
			};
		}

		private void CreateResponderChain(ProtocolDescriptor protocol, MonitorIdentity monitorIdentity, TracingContext traceContext)
		{
			if (Utils.EnableResponderForCurrentEnvironment(this.IsClearLsassCacheResponderEnabled, this.CreateRespondersForTest) && VariantConfiguration.InvariantNoFlightingSnapshot.ActiveMonitoring.ClearLsassCacheResponder.Enabled)
			{
				ResponderIdentity responderIdentity = monitorIdentity.CreateResponderIdentity("ClearLsassCacheResponder", null);
				ResponderDefinition responderDefinition = ClearLsassCacheResponder.CreateDefinition(responderIdentity.Name, monitorIdentity.GetAlertMask(), monitorIdentity.Component.ServerComponent, ServiceHealthStatus.Degraded);
				responderDefinition.AlertTypeId = monitorIdentity.Name;
				responderDefinition.ServiceName = monitorIdentity.Component.Name;
				responderDefinition.RecurrenceIntervalSeconds = this.ClearLsassCacheIntervalSeconds;
				responderDefinition.WaitIntervalSeconds = this.ClearLsassCacheIntervalSeconds;
				responderDefinition.TimeoutSeconds = this.ClearLsassCacheIntervalSeconds;
				responderDefinition.MaxRetryAttempts = 1;
				responderDefinition.Enabled = true;
				responderDefinition.TargetResource = responderIdentity.TargetResource;
				this.breadcrumbs.Drop("+" + responderDefinition.Name);
				base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, traceContext);
			}
			if (Utils.EnableResponderForCurrentEnvironment(this.IsIISRecycleResponderEnabled, this.CreateRespondersForTest))
			{
				ResponderIdentity responderIdentity2 = monitorIdentity.CreateResponderIdentity("RecycleAppPool", null);
				ResponderDefinition responderDefinition = ResetIISAppPoolResponder.CreateDefinition(responderIdentity2.Name, monitorIdentity.Name, monitorIdentity.TargetResource, ServiceHealthStatus.Degraded1, DumpMode.None, null, 15.0, 0, responderIdentity2.Component.Name, true, "Cafe");
				responderDefinition.AlertMask = monitorIdentity.GetAlertMask();
				responderDefinition.AlertTypeId = monitorIdentity.Name;
				responderDefinition.TargetResource = responderIdentity2.TargetResource;
				responderDefinition.RecurrenceIntervalSeconds = this.IISRecycleRetryIntervalSeconds;
				responderDefinition.WaitIntervalSeconds = this.IISRecycleRetryIntervalSeconds;
				responderDefinition.TimeoutSeconds = this.IISRecycleRetryIntervalSeconds;
				responderDefinition.MaxRetryAttempts = this.IISRecycleRetryCount;
				responderDefinition.Enabled = true;
				this.breadcrumbs.Drop("+" + responderDefinition.Name);
				base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, traceContext);
			}
			bool flag = this.ShouldCreateOfflineResponder(monitorIdentity.Component);
			if (flag)
			{
				ResponderIdentity responderIdentity3 = monitorIdentity.CreateResponderIdentity("Offline", null);
				ResponderDefinition responderDefinition = CafeOfflineResponder.CreateDefinition(responderIdentity3.Name, monitorIdentity.Name, monitorIdentity.Component.ServerComponent, ServiceHealthStatus.Unhealthy, responderIdentity3.Component.Name, this.OfflineResponderFractionOverride, "", "Datacenter", "F5AvailabilityData", "MachineOut");
				responderDefinition.TargetResource = responderIdentity3.TargetResource;
				this.breadcrumbs.Drop("+" + responderDefinition.Name);
				base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, traceContext);
			}
			bool flag2 = Utils.EnableResponderForCurrentEnvironment(this.IsOfflineFailedAlertResponderEnabled, this.CreateRespondersForTest) && (!LocalEndpointManager.IsDataCenter || CafeDiscovery.IsDatacenterP0Protocol(monitorIdentity.Component));
			if (flag2)
			{
				string alertMessageBody = CafeDiscovery.GetAlertMessageBody(Strings.CafeOfflineFailedEscalationRecoveryDetails(monitorIdentity.TargetResource), CafeDiscovery.CafeArrayName);
				ResponderIdentity responderIdentity4 = monitorIdentity.CreateResponderIdentity("OfflineFailedEscalate", null);
				ResponderDefinition responderDefinition = EscalateComponentStateResponder.CreateDefinition(responderIdentity4.Name, responderIdentity4.Component.Name, monitorIdentity.Name, monitorIdentity.GetAlertMask(), responderIdentity4.TargetResource, ServiceHealthStatus.Unhealthy1, monitorIdentity.Component.Service, monitorIdentity.Component.EscalationTeam, Strings.CafeEscalationSubjectUnhealthy, alertMessageBody, monitorIdentity.Component.ServerComponent, CafeUtils.TriggerConfig.ExecuteIfOnline, this.ProbeRecurrenceIntervalSeconds, "httpproxy\\" + protocol.LogFolderName.ToLower(), responderIdentity4.TargetResource, typeof(CafeExtraDetailsParser), true, this.EnablePagedAlerts ? NotificationServiceClass.Urgent : NotificationServiceClass.UrgentInTraining, 14400);
				if (this.AlertResponderCorrelationEnabled && protocol.DeferAlertOnCafeWideFailure)
				{
					CafeUtils.ConfigureResponderForCafeFailureCorrelation(responderDefinition);
				}
				this.breadcrumbs.Drop("+" + responderDefinition.Name);
				base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, traceContext);
			}
			bool flag3 = Utils.EnableResponderForCurrentEnvironment(this.IsRebootResponderEnabled, this.CreateRespondersForTest) && CafeDiscovery.IsDatacenterP0Protocol(monitorIdentity.Component);
			if (flag3)
			{
				ResponderIdentity responderIdentity5 = monitorIdentity.CreateResponderIdentity("RebootOfflineServer", null);
				ResponderDefinition responderDefinition = RebootServerComponentStateResponder.CreateDefinition(responderIdentity5.Name, responderIdentity5.Component.Name, monitorIdentity.Name, ServiceHealthStatus.Unhealthy2, monitorIdentity.Component.ServerComponent, CafeUtils.TriggerConfig.ExecuteIfOffline);
				responderDefinition.TargetResource = responderIdentity5.TargetResource;
				this.breadcrumbs.Drop("+" + responderDefinition.Name);
				base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, traceContext);
			}
			if (Utils.EnableResponderForCurrentEnvironment(this.IsAlertResponderEnabled, this.CreateRespondersForTest))
			{
				string alertMessageBody2 = CafeDiscovery.GetAlertMessageBody(Strings.CafeEscalationRecoveryDetails(monitorIdentity.TargetResource), CafeDiscovery.CafeArrayName);
				NotificationServiceClass notificationServiceClass = NotificationServiceClass.UrgentInTraining;
				if (this.EnablePagedAlerts)
				{
					notificationServiceClass = (flag2 ? NotificationServiceClass.Scheduled : NotificationServiceClass.Urgent);
				}
				CafeUtils.TriggerConfig triggerConfig = flag ? CafeUtils.TriggerConfig.ExecuteIfOffline : CafeUtils.TriggerConfig.ExecuteIfOnline;
				ResponderIdentity responderIdentity6 = monitorIdentity.CreateResponderIdentity("Escalate", null);
				ResponderDefinition responderDefinition = EscalateComponentStateResponder.CreateDefinition(responderIdentity6.Name, responderIdentity6.Component.Name, monitorIdentity.Name, monitorIdentity.GetAlertMask(), responderIdentity6.TargetResource, ServiceHealthStatus.Unrecoverable, monitorIdentity.Component.Service, monitorIdentity.Component.EscalationTeam, Strings.CafeEscalationSubjectUnhealthy, alertMessageBody2, monitorIdentity.Component.ServerComponent, triggerConfig, this.ProbeRecurrenceIntervalSeconds, "httpproxy\\" + protocol.LogFolderName.ToLower(), responderIdentity6.TargetResource, typeof(CafeExtraDetailsParser), true, notificationServiceClass, 14400);
				if (this.AlertResponderCorrelationEnabled && protocol.DeferAlertOnCafeWideFailure)
				{
					CafeUtils.ConfigureResponderForCafeFailureCorrelation(responderDefinition);
				}
				this.breadcrumbs.Drop("+" + responderDefinition.Name);
				base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, traceContext);
			}
		}

		private bool ShouldCreateOfflineResponder(Component component)
		{
			return Utils.EnableResponderForCurrentEnvironment(this.IsOfflineResponderEnabled, this.CreateRespondersForTest) && (!LocalEndpointManager.IsDataCenter || CafeDiscovery.IsDatacenterP0Protocol(component));
		}

		internal static bool IsProtocolAvailableInEnvironment(HttpProtocol protocol)
		{
			switch (protocol)
			{
			case HttpProtocol.PowerShellLiveID:
			case HttpProtocol.Reporting:
			case HttpProtocol.XRop:
				return LocalEndpointManager.IsDataCenter;
			}
			return true;
		}

		internal static bool IsDatacenterP0Protocol(Component component)
		{
			return component.ServerComponent == ServerComponentEnum.HttpProxyAvailabilityGroup;
		}

		private static string GetAlertMessageBody(string recoveryDetails, string cafeArrayName)
		{
			if (LocalEndpointManager.IsDataCenter)
			{
				return Strings.CafeEscalationMessageUnhealthyForDC(cafeArrayName);
			}
			return Strings.CafeEscalationMessageUnhealthy(recoveryDetails, cafeArrayName);
		}

		public const int DefaultProbeTimeoutSeconds = 15;

		public const int DefaultProbeRecurrenceIntervalSeconds = 15;

		public const int DefaultMonitoringIntervalSeconds = 60;

		public const int DefaultMonitoringRecurrenceIntervalSeconds = 0;

		public const int DefaultFailedProbeThreshold = 3;

		private const string ProbeName = "CafeLocalProbe";

		private const string TargetResource = "ClientAccess";

		public static readonly string ProbeEndPoint = Uri.UriSchemeHttps + "://localhost/";

		internal static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		internal static readonly string ProbeTypeName = typeof(CafeLocalProbe).FullName;

		internal static readonly string RecycleResponderTypeName = typeof(ResetIISAppPoolResponder).FullName;

		private static readonly string[] ProbeAttributes = new string[]
		{
			"TrustAnySslCertificate",
			"Verbose",
			"HttpRequestTimeoutSpan"
		};

		private Breadcrumbs breadcrumbs;

		private CancellationToken cancellationToken;

		private static string cafeArrayName;

		private static ITopologyConfigurationSession adSession;
	}
}
