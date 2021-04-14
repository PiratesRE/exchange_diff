using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Responders;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Ews.Probes
{
	public sealed class EwsDiscovery : MaintenanceWorkItem
	{
		public int ProbeRecurrenceIntervalSeconds { get; private set; }

		public int MonitoringIntervalSeconds { get; private set; }

		public int MonitoringRecurrenceIntervalSeconds { get; private set; }

		public int ProbeTimeoutSeconds { get; private set; }

		public int DegradedTransitionSeconds { get; private set; }

		public int UnhealthyTransitionSeconds { get; private set; }

		public int UnrecoverableTransitionSeconds { get; private set; }

		public int IISRecycleRetryCount { get; private set; }

		public int IISRecycleRetryIntervalSeconds { get; private set; }

		public int FailedProbeThreshold { get; private set; }

		public bool IsIISRecycleResponderEnabled { get; private set; }

		public bool IsFailoverResponderEnabled { get; private set; }

		public bool IsAlertResponderEnabled { get; private set; }

		public string ServerRole { get; private set; }

		public string ProbeName { get; private set; }

		public bool Verbose { get; private set; }

		public bool EnablePagedAlerts { get; private set; }

		public bool CreateRespondersForTest { get; private set; }

		public ProbeDefinition ProbeDefinition { get; private set; }

		public ProbeIdentity ProbeIdentity { get; private set; }

		public MonitorIdentity MonitorIdentity { get; private set; }

		protected override void DoWork(CancellationToken cancellationToken)
		{
			this.breadcrumbs = new Breadcrumbs(1024, base.TraceContext);
			try
			{
				this.Configure(base.TraceContext);
				LocalEndpointManager instance = LocalEndpointManager.Instance;
				try
				{
					if (instance.ExchangeServerRoleEndpoint == null)
					{
						WTFDiagnostics.TraceInformation(ExTraceGlobals.EWSTracer, base.TraceContext, "EwsDiscovery:: DoWork(): Could not find ExchangeServerRoleEndpoint", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Ews\\EwsDiscovery.cs", 218);
						return;
					}
				}
				catch (Exception ex)
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.EWSTracer, base.TraceContext, string.Format("EwsDiscovery:: DoWork(): ExchangeServerRoleEndpoint object threw exception.  Exception:{0}", ex.ToString()), null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Ews\\EwsDiscovery.cs", 224);
					return;
				}
				try
				{
					if (instance.MailboxDatabaseEndpoint == null)
					{
						WTFDiagnostics.TraceInformation(ExTraceGlobals.EWSTracer, base.TraceContext, "EwsDiscovery:: DoWork(): Could not find MailboxDatabaseEndpoint", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Ews\\EwsDiscovery.cs", 234);
						return;
					}
				}
				catch (Exception ex2)
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.EWSTracer, base.TraceContext, string.Format("EwsDiscovery:: DoWork(): MailboxDatabaseEndpoint object threw exception.  Exception:{0}", ex2.ToString()), null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Ews\\EwsDiscovery.cs", 240);
					return;
				}
				foreach (EwsDiscovery.ProbeStuff probeStuff in EwsDiscovery.ProbeTable)
				{
					if (this.ProbeName.Equals(probeStuff.Name, StringComparison.OrdinalIgnoreCase))
					{
						this.probeStuff = probeStuff;
					}
				}
				if (this.probeStuff == null)
				{
					this.breadcrumbs.Drop("EwsDiscovery.DoWork: ProbeType {0} is not supported at this time.", new object[]
					{
						this.ProbeName
					});
					throw new NotSupportedException(string.Format("EwsDiscovery.DoWork: ProbeType {0} is not supported at this time.", this.ProbeName));
				}
				string serverRole;
				if ((serverRole = this.ServerRole) != null)
				{
					ICollection<MailboxDatabaseInfo> collection;
					if (!(serverRole == "Mailbox"))
					{
						if (!(serverRole == "ClientAccess"))
						{
							goto IL_2C6;
						}
						if (LocalEndpointManager.IsDataCenter)
						{
							this.breadcrumbs.Drop("EwsDiscovery.DoWork: Skipping ProbeType {0} as it is not needed for ClientAccess in Datacenter.", new object[]
							{
								this.ProbeName
							});
							return;
						}
						if (!instance.ExchangeServerRoleEndpoint.IsCafeRoleInstalled)
						{
							this.breadcrumbs.Drop("EwsDiscovery.DoWork: {0} role is required and is not present on this server.", new object[]
							{
								this.ServerRole
							});
							return;
						}
						if (instance.MailboxDatabaseEndpoint == null || instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForCafe.Count == 0)
						{
							this.breadcrumbs.Drop("EwsDiscovery.DoWork: mailbox database collection is empty on this server.");
							return;
						}
						collection = instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForCafe;
					}
					else
					{
						if (!instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled)
						{
							this.breadcrumbs.Drop("EwsDiscovery.DoWork: {0} role is required and is not present on this server.", new object[]
							{
								this.ServerRole
							});
							return;
						}
						if (instance.MailboxDatabaseEndpoint == null || instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend.Count == 0)
						{
							this.breadcrumbs.Drop("EwsDiscovery.DoWork: mailbox database collection is empty on this server.");
							return;
						}
						collection = instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend;
					}
					HashSet<string> hashSet = new HashSet<string>();
					this.breadcrumbs.Drop("CreateInstancePerServer={0},AllMailboxes={1}", new object[]
					{
						this.probeStuff.CreateInstancePerServer ? "True" : "False",
						collection.Count
					});
					foreach (MailboxDatabaseInfo mailboxDatabaseInfo in collection)
					{
						if (string.IsNullOrWhiteSpace(mailboxDatabaseInfo.MonitoringAccountPassword))
						{
							this.breadcrumbs.Drop("Ignore mbxdb {0} (password empty)", new object[]
							{
								mailboxDatabaseInfo.MailboxDatabaseName
							});
						}
						else
						{
							string s = string.Format("Creating probe", new object[0]);
							DatabaseLocationInfo databaseLocationInfo = null;
							if (this.probeStuff.CreateInstancePerServer)
							{
								databaseLocationInfo = EwsDiscovery.activeManager.Value.GetServerForDatabase(mailboxDatabaseInfo.MailboxDatabaseGuid);
								string item = databaseLocationInfo.ServerFqdn.ToUpper();
								if (hashSet.Contains(item))
								{
									continue;
								}
								s = string.Format("Creating probe for {0}", databaseLocationInfo.ServerFqdn);
								hashSet.Add(item);
							}
							this.breadcrumbs.Drop(s);
							this.CreateProbe(base.TraceContext, mailboxDatabaseInfo, databaseLocationInfo);
							this.CreateMonitors(base.TraceContext);
							if (!this.probeStuff.CreateInstancePerServer)
							{
								break;
							}
						}
					}
					this.breadcrumbs.Drop("All done!");
					return;
				}
				IL_2C6:
				throw new NotSupportedException(string.Format("EwsDiscovery.DoWork: server role {0} is not supported at this time", this.ServerRole));
			}
			finally
			{
				this.ReportResult(base.TraceContext);
			}
		}

		private static ProbeDefinition CreateAutodiscoverProbeDefinition(string monitoringAccount, string monitoringAccountDomain, string monitoringAccountPassword, string probeName, string serviceName, string targetResource = null)
		{
			return new ProbeDefinition
			{
				AssemblyPath = Assembly.GetExecutingAssembly().Location,
				TypeName = typeof(AutodiscoverE15Probe).FullName,
				Name = probeName,
				TargetResource = targetResource,
				ServiceName = serviceName,
				RecurrenceIntervalSeconds = 300,
				TimeoutSeconds = 20,
				MaxRetryAttempts = 0,
				Account = monitoringAccount + "@" + monitoringAccountDomain,
				AccountPassword = monitoringAccountPassword,
				AccountDisplayName = monitoringAccount,
				Endpoint = EwsConstants.AutodiscoverSvcEndpoint,
				SecondaryEndpoint = EwsConstants.AutodiscoverXmlEndpoint
			};
		}

		private static ProbeDefinition CreateEwsGenericProbeDefinition(string monitoringAccount, string monitoringAccountDomain, string monitoringAccountPassword, string probeName, string serviceName, string targetResource = null)
		{
			return new ProbeDefinition
			{
				AssemblyPath = Assembly.GetExecutingAssembly().Location,
				TypeName = typeof(EwsGenericProbe).FullName,
				Name = probeName,
				TargetResource = targetResource,
				ServiceName = serviceName,
				Account = monitoringAccount + "@" + monitoringAccountDomain,
				AccountPassword = monitoringAccountPassword,
				AccountDisplayName = monitoringAccount,
				RecurrenceIntervalSeconds = 300,
				TimeoutSeconds = 20,
				MaxRetryAttempts = 0,
				Endpoint = EwsConstants.EwsEndpoint
			};
		}

		private void Configure(TracingContext traceContext)
		{
			this.Verbose = this.ReadAttribute("Verbose", true);
			this.ServerRole = this.ReadAttribute("ServerRole", "Mailbox");
			this.ProbeName = this.ReadAttribute("ProbeType", "EwsProtocolSelfTest");
			this.ProbeRecurrenceIntervalSeconds = (int)this.ReadAttribute("ProbeRecurrenceSpan", TimeSpan.FromMinutes(300.0)).TotalSeconds;
			this.ProbeTimeoutSeconds = (int)this.ReadAttribute("ProbeTimeoutSpan", TimeSpan.FromSeconds(20.0)).TotalSeconds;
			this.MonitoringIntervalSeconds = (int)this.ReadAttribute("MonitoringIntervalSpan", TimeSpan.FromSeconds(1800.0)).TotalSeconds;
			this.MonitoringRecurrenceIntervalSeconds = (int)this.ReadAttribute("MonitoringRecurrenceIntervalSpan", TimeSpan.FromSeconds(0.0)).TotalSeconds;
			this.DegradedTransitionSeconds = (int)this.ReadAttribute("DegradedTransitionSpan", TimeSpan.FromMinutes(0.0)).TotalSeconds;
			this.UnhealthyTransitionSeconds = (int)this.ReadAttribute("UnhealthyTransitionSpan", TimeSpan.FromMinutes(20.0)).TotalSeconds;
			this.UnrecoverableTransitionSeconds = (int)this.ReadAttribute("UnrecoverableTransitionSpan", TimeSpan.FromMinutes(20.0)).TotalSeconds;
			this.IISRecycleRetryCount = this.ReadAttribute("IISRecycleRetryCount", 1);
			this.IISRecycleRetryIntervalSeconds = (int)this.ReadAttribute("IISRecycleRetrySpan", TimeSpan.FromSeconds(30.0)).TotalSeconds;
			this.FailedProbeThreshold = this.ReadAttribute("FailedProbeThreshold", 4);
			this.IsIISRecycleResponderEnabled = this.ReadAttribute("IISRecycleResponderEnabled", false);
			this.IsFailoverResponderEnabled = this.ReadAttribute("FailoverResponderEnabled", false);
			this.IsAlertResponderEnabled = this.ReadAttribute("AlertResponderEnabled", false);
			this.EnablePagedAlerts = this.ReadAttribute("EnablePagedAlerts", true);
			this.CreateRespondersForTest = this.ReadAttribute("CreateRespondersForTest", false);
		}

		private void ReportResult(TracingContext traceContext)
		{
			string text = this.breadcrumbs.ToString();
			base.Result.StateAttribute5 = text;
			WTFDiagnostics.TraceInformation(ExTraceGlobals.EWSTracer, base.TraceContext, text, null, "ReportResult", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Ews\\EwsDiscovery.cs", 498);
		}

		private void CreateProbe(TracingContext traceContext, MailboxDatabaseInfo dbInfo, DatabaseLocationInfo dbLocationInfo = null)
		{
			this.ProbeDefinition = this.GetBaseDefinition(dbInfo, dbLocationInfo);
			this.ProbeDefinition.RecurrenceIntervalSeconds = this.ProbeRecurrenceIntervalSeconds;
			this.ProbeDefinition.TimeoutSeconds = this.ProbeTimeoutSeconds;
			this.CopyAttributes(this.probeStuff.Attributes, this.ProbeDefinition);
			this.probeResultName = this.ProbeDefinition.ConstructWorkItemResultName();
			WTFDiagnostics.TraceInformation(ExTraceGlobals.EWSTracer, base.TraceContext, "configuring probe " + this.probeStuff.Name, null, "CreateProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Ews\\EwsDiscovery.cs", 523);
			base.Broker.AddWorkDefinition<ProbeDefinition>(this.ProbeDefinition, base.TraceContext);
		}

		private ProbeDefinition GetBaseDefinition(MailboxDatabaseInfo dbInfo, DatabaseLocationInfo dbLocationInfo = null)
		{
			bool flag = true;
			string targetResource = (dbLocationInfo != null) ? dbLocationInfo.ServerFqdn : string.Empty;
			string name;
			if ((name = this.probeStuff.Name) != null)
			{
				if (!(name == "EWSDeepTest"))
				{
					if (!(name == "EWSSelfTest"))
					{
						if (!(name == "EWSCtpTest"))
						{
							if (!(name == "AutodiscoverSelfTest"))
							{
								if (!(name == "AutodiscoverCtpTest"))
								{
									goto IL_E2;
								}
								this.ProbeIdentity = ProbeIdentity.Create(ExchangeComponent.Autodiscover, ProbeType.Ctp, null, targetResource);
								flag = false;
							}
							else
							{
								this.ProbeIdentity = ProbeIdentity.Create(ExchangeComponent.AutodiscoverProtocol, ProbeType.SelfTest, null, "MSExchangeAutoDiscoverAppPool");
								flag = false;
							}
						}
						else
						{
							this.ProbeIdentity = ProbeIdentity.Create(ExchangeComponent.Ews, ProbeType.Ctp, null, targetResource);
						}
					}
					else
					{
						this.ProbeIdentity = ProbeIdentity.Create(ExchangeComponent.EwsProtocol, ProbeType.SelfTest, null, "MSExchangeServicesAppPool");
					}
				}
				else
				{
					this.ProbeIdentity = ProbeIdentity.Create(ExchangeComponent.EwsProtocol, ProbeType.DeepTest, null, dbInfo.MailboxDatabaseName);
				}
				if (!flag)
				{
					return EwsDiscovery.CreateAutodiscoverProbeDefinition(dbInfo.MonitoringAccount, dbInfo.MonitoringAccountDomain, dbInfo.MonitoringAccountPassword, this.ProbeIdentity.Name, this.ProbeIdentity.ServiceName, this.ProbeIdentity.TargetResource);
				}
				return EwsDiscovery.CreateEwsGenericProbeDefinition(dbInfo.MonitoringAccount, dbInfo.MonitoringAccountDomain, dbInfo.MonitoringAccountPassword, this.ProbeIdentity.Name, this.ProbeIdentity.ServiceName, this.ProbeIdentity.TargetResource);
			}
			IL_E2:
			throw new NotSupportedException(string.Format("probe type '{0}' is not supported at this time", this.probeStuff.Name));
		}

		private void CreateMonitors(TracingContext traceContext)
		{
			this.MonitorIdentity = this.ProbeIdentity.CreateMonitorIdentity();
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(this.MonitorIdentity.Name, this.probeResultName, this.MonitorIdentity.ServiceName, this.MonitorIdentity.Component, this.FailedProbeThreshold, true, 300);
			monitorDefinition.RecurrenceIntervalSeconds = this.MonitoringRecurrenceIntervalSeconds;
			monitorDefinition.TimeoutSeconds = this.ProbeTimeoutSeconds;
			monitorDefinition.MaxRetryAttempts = 0;
			monitorDefinition.MonitoringIntervalSeconds = this.MonitoringIntervalSeconds;
			monitorDefinition.TargetResource = this.ProbeIdentity.TargetResource;
			monitorDefinition.IsHaImpacting = this.IsFailoverResponderEnabled;
			WTFDiagnostics.TraceInformation(ExTraceGlobals.EWSTracer, base.TraceContext, "configuring monitor " + monitorDefinition.Name, null, "CreateMonitors", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Ews\\EwsDiscovery.cs", 618);
			monitorDefinition.MonitorStateTransitions = this.CreateResponderChain(base.TraceContext, this.probeStuff.AppPool, monitorDefinition);
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate EWS health is not impacted by any issues";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
		}

		private MonitorStateTransition[] CreateResponderChain(TracingContext traceContext, string appPool, MonitorIdentity monitorIdentity)
		{
			MonitorStateTransition[] result = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded, this.DegradedTransitionSeconds),
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, this.UnhealthyTransitionSeconds),
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, this.UnrecoverableTransitionSeconds)
			};
			if (Utils.EnableResponderForCurrentEnvironment(this.IsIISRecycleResponderEnabled, this.CreateRespondersForTest))
			{
				ResponderIdentity responderIdentity = this.MonitorIdentity.CreateResponderIdentity("Restart", appPool);
				ResponderDefinition responderDefinition = ResetIISAppPoolResponder.CreateDefinition(responderIdentity.Name, monitorIdentity.Name, appPool, ServiceHealthStatus.Degraded, DumpMode.None, null, 15.0, 0, responderIdentity.ServiceName, true, this.probeStuff.ResponderThrottleGroup);
				responderDefinition.AlertMask = monitorIdentity.GetAlertMask();
				responderDefinition.AlertTypeId = monitorIdentity.Name;
				responderDefinition.TargetResource = responderIdentity.TargetResource;
				responderDefinition.RecurrenceIntervalSeconds = this.IISRecycleRetryIntervalSeconds;
				responderDefinition.WaitIntervalSeconds = this.IISRecycleRetryIntervalSeconds;
				responderDefinition.TimeoutSeconds = this.IISRecycleRetryIntervalSeconds;
				responderDefinition.MaxRetryAttempts = this.IISRecycleRetryCount;
				responderDefinition.Enabled = this.IsIISRecycleResponderEnabled;
				WTFDiagnostics.TraceInformation(ExTraceGlobals.EWSTracer, base.TraceContext, "configuring IISRecycle responder " + responderDefinition.Name, null, "CreateResponderChain", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Ews\\EwsDiscovery.cs", 681);
				base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
			}
			if (Utils.EnableResponderForCurrentEnvironment(this.IsFailoverResponderEnabled, this.CreateRespondersForTest))
			{
				ResponderIdentity responderIdentity2 = this.MonitorIdentity.CreateResponderIdentity("Failover", appPool);
				ResponderDefinition responderDefinition = SystemFailoverResponder.CreateDefinition(responderIdentity2.Name, monitorIdentity.Name, ServiceHealthStatus.Unhealthy, this.ProbeIdentity.Component.Name, responderIdentity2.ServiceName, true);
				WTFDiagnostics.TraceInformation(ExTraceGlobals.EWSTracer, base.TraceContext, "configuring Failover responder " + responderDefinition.Name, null, "CreateResponderChain", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Ews\\EwsDiscovery.cs", 701);
				base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
			}
			if (Utils.EnableResponderForCurrentEnvironment(this.IsAlertResponderEnabled, this.CreateRespondersForTest))
			{
				ResponderIdentity responderIdentity3 = this.MonitorIdentity.CreateResponderIdentity("Escalate", appPool);
				string escalationMessageUnhealthy = Strings.EwsAutodEscalationMessageUnhealthy((this.probeStuff.RecoveryStringDelegate != null) ? this.probeStuff.RecoveryStringDelegate(appPool) : string.Empty);
				ResponderDefinition responderDefinition = EscalateResponder.CreateDefinition(responderIdentity3.Name, responderIdentity3.ServiceName, monitorIdentity.Name, monitorIdentity.GetAlertMask(), this.MonitorIdentity.TargetResource, ServiceHealthStatus.Unrecoverable, this.ProbeIdentity.Component.EscalationTeam, Strings.EwsAutodEscalationSubjectUnhealthy, escalationMessageUnhealthy, true, NotificationServiceClass.Urgent, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
				responderDefinition.Enabled = this.IsAlertResponderEnabled;
				responderDefinition.NotificationServiceClass = (this.EnablePagedAlerts ? NotificationServiceClass.Urgent : NotificationServiceClass.UrgentInTraining);
				WTFDiagnostics.TraceInformation(ExTraceGlobals.EWSTracer, base.TraceContext, "configuring escalate responder " + responderDefinition.Name, null, "CreateResponderChain", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Ews\\EwsDiscovery.cs", 733);
				base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
			}
			return result;
		}

		internal static readonly string RecycleResponderTypeName = typeof(ResetIISAppPoolResponder).FullName;

		public static string[] StandardAttributes = new string[]
		{
			"ApiRetryCount",
			"ApiRetrySleepInMilliseconds",
			"UseXropEndPoint",
			"TargetPort",
			"Domain",
			"IsOutsideInMonitoring",
			"ExchangeSku",
			"PrimaryAuthN",
			"TrustAnySslCertificate",
			"UserAgentPart",
			"Verbose",
			"OperationName",
			"IncludeExchangeRpcUrl"
		};

		private static readonly EwsDiscovery.ProbeStuff[] ProbeTable = new EwsDiscovery.ProbeStuff[]
		{
			new EwsDiscovery.ProbeStuff
			{
				Name = "EWSDeepTest",
				Attributes = EwsDiscovery.StandardAttributes,
				AppPool = "MSExchangeServicesAppPool",
				CreateInstancePerServer = false,
				ResponderThrottleGroup = "Dag"
			},
			new EwsDiscovery.ProbeStuff
			{
				Name = "EWSSelfTest",
				Attributes = EwsDiscovery.StandardAttributes,
				AppPool = "MSExchangeServicesAppPool",
				CreateInstancePerServer = false,
				RecoveryStringDelegate = new Func<string, LocalizedString>(Strings.EwsAutodSelfTestEscalationRecoveryDetails),
				ResponderThrottleGroup = "Dag"
			},
			new EwsDiscovery.ProbeStuff
			{
				Name = "EWSCtpTest",
				Attributes = EwsDiscovery.StandardAttributes,
				AppPool = "MSExchangeServicesAppPool",
				CreateInstancePerServer = true,
				ResponderThrottleGroup = "Cafe"
			},
			new EwsDiscovery.ProbeStuff
			{
				Name = "AutodiscoverSelfTest",
				Attributes = EwsDiscovery.StandardAttributes,
				AppPool = "MSExchangeAutoDiscoverAppPool",
				CreateInstancePerServer = false,
				RecoveryStringDelegate = new Func<string, LocalizedString>(Strings.EwsAutodSelfTestEscalationRecoveryDetails),
				ResponderThrottleGroup = "Dag"
			},
			new EwsDiscovery.ProbeStuff
			{
				Name = "AutodiscoverCtpTest",
				Attributes = EwsDiscovery.StandardAttributes,
				AppPool = "MSExchangeAutoDiscoverAppPool",
				CreateInstancePerServer = true,
				ResponderThrottleGroup = "Cafe"
			}
		};

		private string probeResultName;

		private EwsDiscovery.ProbeStuff probeStuff;

		private Breadcrumbs breadcrumbs;

		private static Lazy<ActiveManager> activeManager = new Lazy<ActiveManager>(() => ActiveManager.GetNoncachingActiveManagerInstance());

		internal class ProbeStuff
		{
			public string Name { get; set; }

			public string[] Attributes { get; set; }

			public string AppPool { get; set; }

			public bool CreateInstancePerServer { get; set; }

			public Func<string, LocalizedString> RecoveryStringDelegate { get; set; }

			public string ResponderThrottleGroup;
		}
	}
}
