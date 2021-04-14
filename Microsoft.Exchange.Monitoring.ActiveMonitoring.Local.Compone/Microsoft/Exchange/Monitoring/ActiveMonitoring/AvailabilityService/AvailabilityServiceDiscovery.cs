using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.AvailabilityService.Monitors;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.AvailabilityService.Probes;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.AvailabilityService
{
	public sealed class AvailabilityServiceDiscovery : MaintenanceWorkItem
	{
		public int DegradedTransitionSpanSeconds { get; private set; }

		public int FailedProbeThreshold { get; private set; }

		public bool IsAlertResponderEnabled { get; private set; }

		public bool IsOnPremisesEnabled { get; private set; }

		public int MinimumSecondsBetweenEscalates { get; private set; }

		public int MonitoringIntervalSeconds { get; private set; }

		public int MonitoringRecurrenceIntervalSeconds { get; private set; }

		public int ProbeRecurrenceIntervalSeconds { get; private set; }

		public int ProbeTimeoutSeconds { get; private set; }

		public string ProbeType { get; private set; }

		public int UnhealthyTransitionSpanSeconds { get; private set; }

		public bool IsAlerting24Hours { get; private set; }

		protected override void DoWork(CancellationToken cancellationToken)
		{
			this.breadcrumbs = new Breadcrumbs(1024, base.TraceContext);
			try
			{
				this.Configure();
				ICollection<MailboxDatabaseInfo> mbxDatabaseInfo = this.GetMbxDatabaseInfo();
				if (mbxDatabaseInfo != null)
				{
					this.CreateWorkItemDefinition(mbxDatabaseInfo);
				}
			}
			finally
			{
				this.ReportResult();
			}
		}

		private void Configure()
		{
			this.DegradedTransitionSpanSeconds = (int)this.ReadAttribute("DegradedTransitionSpan", AvailabilityServiceDiscovery.DefaultDegradedTransitionSpan).TotalSeconds;
			this.FailedProbeThreshold = this.ReadAttribute("NumberOfConsecutiveFailures", 3);
			this.IsAlertResponderEnabled = this.ReadAttribute("IsAlertingEnabled", false);
			this.IsOnPremisesEnabled = this.ReadAttribute("EnableOnPrem", false);
			this.MinimumSecondsBetweenEscalates = (int)this.ReadAttribute("TimeToWaitUntilNextAlert", AvailabilityServiceDiscovery.DefaultEscalateInterval).TotalSeconds;
			this.MonitoringIntervalSeconds = (int)this.ReadAttribute("MonitorLookbackWindow", AvailabilityServiceDiscovery.DefaultMonitoringInterval).TotalSeconds;
			this.MonitoringRecurrenceIntervalSeconds = (int)this.ReadAttribute("MonitorExecutionInterval", AvailabilityServiceDiscovery.DefaultMonitoringRecurrenceInterval).TotalSeconds;
			this.ProbeRecurrenceIntervalSeconds = (int)this.ReadAttribute("ProbeExecutionInterval", AvailabilityServiceDiscovery.DefaultProbeRecurrenceInterval).TotalSeconds;
			this.ProbeTimeoutSeconds = (int)this.ReadAttribute("ProbeTimeoutSpan", AvailabilityServiceDiscovery.DefaultProbeTimeout).TotalSeconds;
			this.ProbeType = this.ReadAttribute("ProbeType", "AvailabilityServiceCrossServerTest");
			this.UnhealthyTransitionSpanSeconds = (int)this.ReadAttribute("TimeToEscalateAfterUnhealthyState", AvailabilityServiceDiscovery.DefaultUnhealthyTransitionSpan).TotalSeconds;
			this.IsAlerting24Hours = this.ReadAttribute("IsAlerting24Hours", false);
			base.Definition.Attributes["ApiRetryCount"] = this.ReadAttribute("NumberOfRetriesInProbe", 0).ToString();
		}

		private ICollection<MailboxDatabaseInfo> GetMbxDatabaseInfo()
		{
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			if (!LocalEndpointManager.IsDataCenter && !this.IsOnPremisesEnabled)
			{
				this.breadcrumbs.Drop("In case of on-premises, IsOnPremisesEnabled should be true in order to create probe/monitor/responder");
				return null;
			}
			foreach (AvailabilityServiceDiscovery.ProbeInfo probeInfo in AvailabilityServiceDiscovery.ProbeTable)
			{
				if (this.ProbeType.Equals(probeInfo.TypeName, StringComparison.OrdinalIgnoreCase))
				{
					this.probeInfo = probeInfo;
				}
			}
			if (this.probeInfo == null)
			{
				this.breadcrumbs.Drop("ProbeType {0} is not supported at this time.", new object[]
				{
					this.ProbeType
				});
				return null;
			}
			if (!instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled)
			{
				this.breadcrumbs.Drop("Mailbox role is required and is not present on this server.");
				return null;
			}
			if (instance.MailboxDatabaseEndpoint == null || instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend.Count == 0)
			{
				this.breadcrumbs.Drop("Mailbox database collection is empty on this server.");
				return null;
			}
			return instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend;
		}

		private void CreateWorkItemDefinition(ICollection<MailboxDatabaseInfo> mbxDBInfo)
		{
			HashSet<string> hashSet = new HashSet<string>();
			foreach (MailboxDatabaseInfo mailboxDatabaseInfo in mbxDBInfo)
			{
				if (string.IsNullOrWhiteSpace(mailboxDatabaseInfo.MonitoringAccountPassword))
				{
					this.breadcrumbs.Drop("Ignore mailbox database {0} because it does not have monitoring mailbox", new object[]
					{
						mailboxDatabaseInfo.MailboxDatabaseName
					});
				}
				else
				{
					string databaseActiveHost = DirectoryAccessor.Instance.GetDatabaseActiveHost(mailboxDatabaseInfo.MailboxDatabaseGuid);
					if (!hashSet.Contains(databaseActiveHost))
					{
						if (!ExEnvironment.IsTest && databaseActiveHost.Equals(Environment.MachineName, StringComparison.OrdinalIgnoreCase))
						{
							this.breadcrumbs.Drop("[{0}]: Skipping a mailbox because Offbox request requires a passive copy of database {1}", new object[]
							{
								this.ProbeType,
								mailboxDatabaseInfo.MailboxDatabaseName
							});
						}
						else
						{
							this.CreateProbe(mailboxDatabaseInfo);
							this.CreateMonitorAndResponder(mailboxDatabaseInfo);
							hashSet.Add(databaseActiveHost);
							this.breadcrumbs.Drop("[{0}]: From the machine {1}, created a probe [ID={3}] targeting on {2}", new object[]
							{
								this.ProbeType,
								Environment.MachineName,
								databaseActiveHost,
								hashSet.Count
							});
						}
					}
					else
					{
						this.breadcrumbs.Drop("[{0}]: Skipping a mailbox as we already have a probe for server {1}", new object[]
						{
							this.ProbeType,
							databaseActiveHost
						});
					}
				}
			}
		}

		private void ReportResult()
		{
			string text = this.breadcrumbs.ToString();
			base.Result.StateAttribute5 = text;
			WTFDiagnostics.TraceInformation(ExTraceGlobals.AvailabilityServiceTracer, base.TraceContext, text, null, "ReportResult", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\AvailabilityService\\AvailabilityServiceDiscovery.cs", 305);
		}

		private void CreateProbe(MailboxDatabaseInfo dbInfo)
		{
			ProbeDefinition baseDefinition = this.GetBaseDefinition(dbInfo);
			baseDefinition.RecurrenceIntervalSeconds = this.ProbeRecurrenceIntervalSeconds;
			baseDefinition.TimeoutSeconds = this.ProbeTimeoutSeconds;
			this.CopyAttributes(this.probeInfo.Attributes, baseDefinition);
			this.probeResultName = baseDefinition.ConstructWorkItemResultName();
			WTFDiagnostics.TraceInformation(ExTraceGlobals.AvailabilityServiceTracer, base.TraceContext, "configuring probe " + this.probeInfo.ProbeName, null, "CreateProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\AvailabilityService\\AvailabilityServiceDiscovery.cs", 328);
			base.Broker.AddWorkDefinition<ProbeDefinition>(baseDefinition, base.TraceContext);
		}

		private ProbeDefinition GetBaseDefinition(MailboxDatabaseInfo dbInfo)
		{
			ProbeDefinition probeDefinition = new ProbeDefinition();
			probeDefinition.AssemblyPath = AvailabilityServiceDiscovery.AssemblyPath;
			probeDefinition.TypeName = typeof(AvailabilityServiceLocalProbe).FullName;
			probeDefinition.Name = this.probeInfo.ProbeName;
			probeDefinition.Account = dbInfo.MonitoringAccount + "@" + dbInfo.MonitoringAccountDomain;
			probeDefinition.AccountPassword = dbInfo.MonitoringAccountPassword;
			probeDefinition.AccountDisplayName = dbInfo.MonitoringAccount;
			probeDefinition.SecondaryAccount = dbInfo.MonitoringAccount + "@" + dbInfo.MonitoringAccountDomain;
			probeDefinition.SecondaryAccountPassword = dbInfo.MonitoringAccountPassword;
			probeDefinition.SecondaryAccountDisplayName = dbInfo.MonitoringAccount;
			probeDefinition.ServiceName = ExchangeComponent.FreeBusy.Name;
			probeDefinition.TargetResource = dbInfo.MailboxDatabaseName;
			probeDefinition.MaxRetryAttempts = 3;
			probeDefinition.Endpoint = AvailabilityServiceDiscovery.ProbeEndPoint.TrimEnd(new char[]
			{
				'/'
			}) + "/ews/exchange.asmx";
			probeDefinition.Attributes["DatabaseGuid"] = dbInfo.MailboxDatabaseGuid.ToString();
			return probeDefinition;
		}

		private void CreateMonitorAndResponder(MailboxDatabaseInfo dbInfo)
		{
			MonitorDefinition monitorDefinition = new MonitorDefinition();
			monitorDefinition.AssemblyPath = AvailabilityServiceDiscovery.AssemblyPath;
			monitorDefinition.TypeName = typeof(AvailabilityServiceOffBoxRequestMonitor).FullName;
			monitorDefinition.Component = ExchangeComponent.FreeBusy;
			monitorDefinition.Name = this.probeInfo.MonitorName;
			monitorDefinition.InsufficientSamplesIntervalSeconds = 0;
			monitorDefinition.SampleMask = this.probeResultName;
			monitorDefinition.ServiceName = ExchangeComponent.FreeBusy.Name;
			monitorDefinition.MaxRetryAttempts = 0;
			monitorDefinition.MonitoringThreshold = (double)this.FailedProbeThreshold;
			monitorDefinition.MonitoringIntervalSeconds = this.MonitoringIntervalSeconds;
			monitorDefinition.RecurrenceIntervalSeconds = this.MonitoringRecurrenceIntervalSeconds;
			monitorDefinition.TargetResource = dbInfo.MailboxDatabaseName;
			monitorDefinition.TimeoutSeconds = this.ProbeTimeoutSeconds;
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded, this.DegradedTransitionSpanSeconds),
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, this.UnhealthyTransitionSpanSeconds)
			};
			WTFDiagnostics.TraceInformation(ExTraceGlobals.AvailabilityServiceTracer, base.TraceContext, "configuring monitor " + monitorDefinition.Name, null, "CreateMonitorAndResponder", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\AvailabilityService\\AvailabilityServiceDiscovery.cs", 402);
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			if (this.IsAlertResponderEnabled)
			{
				string escalationMessageUnhealthy = this.GetEscalationBody(new Func<string, LocalizedString>(Strings.AvailabilityServiceEscalationHtmlBody), new Func<string, LocalizedString>(Strings.AvailabilityServiceEscalationBody));
				NotificationServiceClass notificationServiceClass = this.IsAlerting24Hours ? NotificationServiceClass.Urgent : NotificationServiceClass.Scheduled;
				ResponderDefinition responderDefinition = EscalateResponder.CreateDefinition(this.probeInfo.AlertResponderName, ExchangeComponent.FreeBusy.Name, monitorDefinition.Name, string.Format("{0}/{1}", monitorDefinition.Name, dbInfo.MailboxDatabaseName), dbInfo.MailboxDatabaseName, ServiceHealthStatus.Unhealthy, ExchangeComponent.FreeBusy.EscalationTeam, Strings.AvailabilityServiceEscalationSubjectUnhealthy(this.probeInfo.TypeName), escalationMessageUnhealthy, this.IsAlertResponderEnabled, notificationServiceClass, this.MinimumSecondsBetweenEscalates, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
				WTFDiagnostics.TraceInformation(ExTraceGlobals.AvailabilityServiceTracer, base.TraceContext, "configuring escalate responder " + responderDefinition.Name, null, "CreateMonitorAndResponder", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\AvailabilityService\\AvailabilityServiceDiscovery.cs", 432);
				base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
			}
		}

		private LocalizedString GetEscalationBody(Func<string, LocalizedString> htmlBodyDelegate, Func<string, LocalizedString> regularBodyDelegate)
		{
			if (LocalEndpointManager.IsDataCenter)
			{
				return htmlBodyDelegate(this.probeInfo.MonitorName);
			}
			return regularBodyDelegate(this.probeInfo.MonitorName);
		}

		public const string DatabaseGuidAttributeName = "DatabaseGuid";

		private const int DefaultFailedProbeThreshold = 3;

		private const int DefaultMaxRetryAttempts = 3;

		private const int DefaultApiRetryCount = 0;

		private static readonly string[] StandardAttributes = new string[]
		{
			"ApiRetryCount",
			"IsOutsideInMonitoring",
			"KnownErrorCodes",
			"OffBoxRequest",
			"PrimaryAuthN",
			"TargetPort",
			"TrustAnySslCertificate",
			"UserAgentPart",
			"Verbose"
		};

		private static readonly AvailabilityServiceDiscovery.ProbeInfo[] ProbeTable = new AvailabilityServiceDiscovery.ProbeInfo[]
		{
			new AvailabilityServiceDiscovery.ProbeInfo
			{
				TypeName = "AvailabilityServiceCrossServerTest",
				Attributes = AvailabilityServiceDiscovery.StandardAttributes,
				ProbeName = "AvailabilityServiceCrossServerTestProbe",
				MonitorName = "AvailabilityServiceCrossServerTestMonitor",
				AlertResponderName = "AvailabilityServiceCrossServerTestEscalate"
			}
		};

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly TimeSpan DefaultEscalateInterval = TimeSpan.FromHours(8.0);

		private static readonly TimeSpan DefaultMonitoringInterval = TimeSpan.FromMinutes(20.0);

		private static readonly TimeSpan DefaultMonitoringRecurrenceInterval = TimeSpan.FromMinutes(15.0);

		private static readonly TimeSpan DefaultProbeRecurrenceInterval = TimeSpan.FromMinutes(5.0);

		private static readonly TimeSpan DefaultProbeTimeout = TimeSpan.FromMinutes(2.0);

		private static readonly TimeSpan DefaultDegradedTransitionSpan = TimeSpan.FromMinutes(0.0);

		private static readonly TimeSpan DefaultUnhealthyTransitionSpan = TimeSpan.FromMinutes(20.0);

		private static readonly string ProbeEndPoint = Uri.UriSchemeHttps + "://localhost/";

		private Breadcrumbs breadcrumbs;

		private string probeResultName;

		private AvailabilityServiceDiscovery.ProbeInfo probeInfo;

		private static Lazy<ActiveManager> activeManager = new Lazy<ActiveManager>(() => ActiveManager.GetCachingActiveManagerInstance());

		internal class ProbeInfo
		{
			public string AlertResponderName { get; set; }

			public string[] Attributes { get; set; }

			public string MonitorName { get; set; }

			public string ProbeName { get; set; }

			public string TypeName { get; set; }
		}
	}
}
