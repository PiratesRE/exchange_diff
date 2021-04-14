using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.HDPhoto.Probes;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.HDPhoto
{
	public sealed class HDPhotoDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			this.Configure(base.TraceContext);
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			if (!LocalEndpointManager.IsDataCenter && !this.isEnabledOnPrem)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.HDPhotoTracer, base.TraceContext, "HDPhotoDiscovery.DoWork: Skipping work since we are running onprem and 'EnableOnPrem' config setting is 'false'.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HDPhoto\\hdphotodiscovery.cs", 205);
				return;
			}
			if (instance.ExchangeServerRoleEndpoint.IsCafeRoleInstalled)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.HDPhotoTracer, base.TraceContext, "HDPhotoDiscovery.DoWork: {0} role is running on a Cafe server.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HDPhoto\\hdphotodiscovery.cs", 212);
				if (instance.MailboxDatabaseEndpoint != null && instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForCafe.Count != 0)
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.HDPhotoTracer, base.TraceContext, "HDPhotoDiscovery.DoWork: Found MailboxDatabases on the Cafe Server.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HDPhoto\\hdphotodiscovery.cs", 217);
					IEnumerable<MailboxDatabaseInfo> mbxDbs = instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForCafe;
					HDPhotoDiscovery.ProbeInfo probeInfo = new HDPhotoDiscovery.ProbeInfo
					{
						Attributes = HDPhotoDiscovery.StandardAttributes,
						TypeName = "HDPhotoCTPTest",
						ProbeName = "HDPhotoCafeProbe",
						MonitorName = "HDPhotoCafeMonitor",
						AlertResponderName = "HDPhotoCafeResponder"
					};
					if (this.isCafeProbeEnabled)
					{
						this.CreateDefinitions(base.TraceContext, mbxDbs, probeInfo);
					}
				}
			}
			if (instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.HDPhotoTracer, base.TraceContext, "HDPhotoDiscovery.DoWork: {0} role is running on a Mailbox server.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HDPhoto\\hdphotodiscovery.cs", 243);
				if (instance.MailboxDatabaseEndpoint != null && instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend.Count != 0)
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.HDPhotoTracer, base.TraceContext, "HDPhotoDiscovery.DoWork: Found MailboxDatabases on the Mailbox Server.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HDPhoto\\hdphotodiscovery.cs", 248);
					IEnumerable<MailboxDatabaseInfo> mbxDbs = instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend;
					HDPhotoDiscovery.ProbeInfo probeInfo2 = new HDPhotoDiscovery.ProbeInfo
					{
						Attributes = HDPhotoDiscovery.StandardAttributes,
						TypeName = "HDPhotoDeepTest",
						ProbeName = "HDPhotoMailboxProbe",
						MonitorName = "HDPhotoMailboxMonitor",
						AlertResponderName = "HDPhotoMailboxResponder"
					};
					if (this.isMailboxProbeEnabled)
					{
						this.CreateDefinitions(base.TraceContext, mbxDbs, probeInfo2);
					}
				}
			}
		}

		private void CreateDefinitions(TracingContext traceContext, IEnumerable<MailboxDatabaseInfo> mbxDbs, HDPhotoDiscovery.ProbeInfo probeInfo)
		{
			mbxDbs = from x in mbxDbs
			where !string.IsNullOrEmpty(x.MonitoringAccount) && !string.IsNullOrEmpty(x.MonitoringAccountPassword)
			select x;
			if (mbxDbs == null || mbxDbs.Count<MailboxDatabaseInfo>() == 0)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.HDPhotoTracer, base.TraceContext, "HDPhotoDiscovery.DoWork: No mailbox databases were found with a monitoring mailbox", null, "CreateDefinitions", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HDPhoto\\hdphotodiscovery.cs", 286);
				return;
			}
			this.CreateProbes(base.TraceContext, mbxDbs, probeInfo);
			MonitorDefinition monitor = this.CreateMonitor(base.TraceContext, probeInfo);
			this.CreateResponder(base.TraceContext, monitor, probeInfo);
		}

		private void Configure(TracingContext traceContext)
		{
			this.isAlertResponderEnabled = this.ReadAttribute("AlertResponderEnabled", false);
			this.isCafeProbeEnabled = this.ReadAttribute("IsCafeProbeEnabled", true);
			this.isMailboxProbeEnabled = this.ReadAttribute("IsMailboxProbeEnabled", true);
			this.isEnabledOnPrem = this.ReadAttribute("EnableOnPrem", false);
			this.failedProbeThreshold = this.ReadAttribute("FailedProbeThreshold", 75);
			this.probeRecurrenceInterval = this.ReadAttribute("ProbeRecurrenceInterval", HDPhotoDiscovery.DefaultProbeRecurrenceInterval);
			this.monitorRecurrenceInterval = this.ReadAttribute("MonitorRecurrenceInterval", HDPhotoDiscovery.DefaultMonitorRecurrenceInterval);
			this.monitoringInterval = this.ReadAttribute("MonitoringInterval", HDPhotoDiscovery.DefaultProbeRecurrenceInterval);
			this.probeTimeoutSeconds = (int)this.ReadAttribute("ProbeTimeoutSpan", HDPhotoDiscovery.DefaultProbeTimeout).TotalSeconds;
			this.degradedTransitionSpanSeconds = (int)this.ReadAttribute("DegradedTransitionSpan", HDPhotoDiscovery.DefaultDegradedTransitionSpan).TotalSeconds;
			this.unhealthyTransitionSpanSeconds = (int)this.ReadAttribute("UnhealthyTransitionSpan", HDPhotoDiscovery.DefaultUnhealthyTransitionSpan).TotalSeconds;
			this.maxRetryAttempts = this.ReadAttribute("MaxRetryAttempts", 3);
			this.minErrorCount = this.ReadAttribute("MinimumErrorCount", 5);
		}

		private void CreateProbes(TracingContext traceContext, IEnumerable<MailboxDatabaseInfo> dbInfos, HDPhotoDiscovery.ProbeInfo probeInfo)
		{
			foreach (MailboxDatabaseInfo mailboxDatabaseInfo in dbInfos)
			{
				if (DirectoryAccessor.Instance.IsDatabaseCopyActiveOnLocalServer(mailboxDatabaseInfo.MailboxDatabaseGuid))
				{
					ProbeDefinition probeDefinition = new ProbeDefinition();
					probeDefinition.AssemblyPath = Assembly.GetExecutingAssembly().Location;
					probeDefinition.TypeName = typeof(HDPhotoLocalProbe).FullName;
					probeDefinition.Name = probeInfo.ProbeName;
					probeDefinition.Account = mailboxDatabaseInfo.MonitoringAccount + "@" + mailboxDatabaseInfo.MonitoringAccountDomain;
					probeDefinition.AccountPassword = mailboxDatabaseInfo.MonitoringAccountPassword;
					probeDefinition.AccountDisplayName = mailboxDatabaseInfo.MonitoringAccount;
					probeDefinition.SecondaryAccount = mailboxDatabaseInfo.MonitoringAccount + "@" + mailboxDatabaseInfo.MonitoringAccountDomain;
					probeDefinition.SecondaryAccountPassword = mailboxDatabaseInfo.MonitoringAccountPassword;
					probeDefinition.SecondaryAccountDisplayName = mailboxDatabaseInfo.MonitoringAccount;
					probeDefinition.ServiceName = ExchangeComponent.HDPhoto.Name;
					probeDefinition.MaxRetryAttempts = this.maxRetryAttempts;
					probeDefinition.TargetResource = mailboxDatabaseInfo.MailboxDatabaseName;
					probeDefinition.Attributes["DatabaseGuid"] = mailboxDatabaseInfo.MailboxDatabaseGuid.ToString();
					if (probeInfo.TypeName == "HDPhotoCTPTest")
					{
						probeDefinition.Endpoint = (probeDefinition.SecondaryEndpoint = HDPhotoDiscovery.ProbeEndPoint.TrimEnd(new char[]
						{
							'/'
						}) + "/ews/exchange.asmx");
					}
					else
					{
						probeDefinition.Endpoint = (probeDefinition.SecondaryEndpoint = HDPhotoDiscovery.ProbeEndPoint.TrimEnd(new char[]
						{
							'/'
						}) + ":444/ews/exchange.asmx");
					}
					probeDefinition.RecurrenceIntervalSeconds = (int)this.probeRecurrenceInterval.TotalSeconds * dbInfos.Count<MailboxDatabaseInfo>();
					probeDefinition.TimeoutSeconds = this.probeTimeoutSeconds;
					this.CopyAttributes(probeInfo.Attributes, probeDefinition);
					WTFDiagnostics.TraceInformation(ExTraceGlobals.HDPhotoTracer, base.TraceContext, "Configured probe " + probeInfo.ProbeName, null, "CreateProbes", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HDPhoto\\hdphotodiscovery.cs", 372);
					base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
				}
			}
		}

		private MonitorDefinition CreateMonitor(TracingContext traceContext, HDPhotoDiscovery.ProbeInfo probeInfo)
		{
			MonitorDefinition monitorDefinition = OverallPercentSuccessMonitor.CreateDefinition(probeInfo.MonitorName, probeInfo.ProbeName, ExchangeComponent.HDPhoto.Name, ExchangeComponent.HDPhoto, (double)this.failedProbeThreshold, this.monitoringInterval, true);
			monitorDefinition.MaxRetryAttempts = 0;
			monitorDefinition.RecurrenceIntervalSeconds = (int)this.monitorRecurrenceInterval.TotalSeconds;
			monitorDefinition.TimeoutSeconds = this.probeTimeoutSeconds;
			monitorDefinition.MinimumErrorCount = this.minErrorCount;
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded, this.degradedTransitionSpanSeconds),
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, this.unhealthyTransitionSpanSeconds)
			};
			monitorDefinition.ServicePriority = 2;
			monitorDefinition.ScenarioDescription = "Validate HD Photo health is not impacted any issues";
			WTFDiagnostics.TraceInformation(ExTraceGlobals.HDPhotoTracer, base.TraceContext, "Configured monitor " + monitorDefinition.Name, null, "CreateMonitor", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HDPhoto\\hdphotodiscovery.cs", 412);
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			return monitorDefinition;
		}

		private void CreateResponder(TracingContext traceContext, MonitorDefinition monitor, HDPhotoDiscovery.ProbeInfo probeInfo)
		{
			ResponderDefinition responderDefinition = EscalateResponder.CreateDefinition(probeInfo.AlertResponderName, ExchangeComponent.HDPhoto.Name, monitor.Name, monitor.ConstructWorkItemResultName(), null, ServiceHealthStatus.Unhealthy, ExchangeComponent.HDPhoto.EscalationTeam, "The GetUserPhoto request failed on {Probe.MachineName}. Success rate is {Monitor.TotalValue}%", "The GetUserPhoto request failed with the error below.\n\n Error: {Probe.Error} \n Exception: {Probe.Exception} \n FailureContext: {Probe.FailureContext} \n Execution Context: {Probe.ExecutionContext} \n ResultName : {Probe.ResultName} \n Photo Url : {Probe.StateAttribute11} \n Users : {Probe.StateAttribute2}", true, NotificationServiceClass.UrgentInTraining, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			responderDefinition.Enabled = this.isAlertResponderEnabled;
			responderDefinition.RecurrenceIntervalSeconds = monitor.RecurrenceIntervalSeconds;
			WTFDiagnostics.TraceInformation(ExTraceGlobals.HDPhotoTracer, base.TraceContext, "Configured escalate responder " + responderDefinition.Name, null, "CreateResponder", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HDPhoto\\hdphotodiscovery.cs", 445);
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
		}

		public const string HDPhotoCafeProbeType = "HDPhotoCTPTest";

		public const string HDPhotoCafeProbe = "HDPhotoCafeProbe";

		public const string HDPhotoCafeMonitor = "HDPhotoCafeMonitor";

		public const string HDPhotoCafeResponder = "HDPhotoCafeResponder";

		public const string HDPhotoMailboxProbeType = "HDPhotoDeepTest";

		public const string HDPhotoMailboxProbe = "HDPhotoMailboxProbe";

		public const string HDPhotoMailboxMonitor = "HDPhotoMailboxMonitor";

		public const string HDPhotoMailboxResponder = "HDPhotoMailboxResponder";

		public const string MailboxDatabaseGuidParameterName = "DatabaseGuid";

		private const string HDPhotoFailedEscalationSubject = "The GetUserPhoto request failed on {Probe.MachineName}. Success rate is {Monitor.TotalValue}%";

		private const string HDPhotoFailedEscalationMessage = "The GetUserPhoto request failed with the error below.\n\n Error: {Probe.Error} \n Exception: {Probe.Exception} \n FailureContext: {Probe.FailureContext} \n Execution Context: {Probe.ExecutionContext} \n ResultName : {Probe.ResultName} \n Photo Url : {Probe.StateAttribute11} \n Users : {Probe.StateAttribute2}";

		private const int DefaultFailedProbeThreshold = 75;

		private const int DefaultMaxRetryAttempts = 3;

		private const int DefaultMinErrorCount = 5;

		private static readonly string[] StandardAttributes = new string[]
		{
			"IsOutsideInMonitoring",
			"PrimaryAuthN",
			"TargetPort",
			"TrustAnySslCertificate",
			"UserAgentPart",
			"Verbose"
		};

		private static readonly TimeSpan DefaultProbeTimeout = TimeSpan.FromMinutes(4.0);

		private static readonly TimeSpan DefaultProbeRecurrenceInterval = TimeSpan.FromMinutes(5.0);

		private static readonly TimeSpan DefaultMonitorRecurrenceInterval = TimeSpan.FromMinutes(15.0);

		private static readonly TimeSpan DefaultMonitoringInterval = TimeSpan.FromHours(1.0);

		private static readonly TimeSpan DefaultDegradedTransitionSpan = TimeSpan.FromMinutes(0.0);

		private static readonly TimeSpan DefaultUnhealthyTransitionSpan = TimeSpan.FromMinutes(20.0);

		private static readonly string ProbeEndPoint = Uri.UriSchemeHttps + "://localhost/";

		private bool isAlertResponderEnabled;

		private bool isCafeProbeEnabled;

		private bool isMailboxProbeEnabled;

		private bool isEnabledOnPrem;

		private int failedProbeThreshold;

		private TimeSpan monitoringInterval;

		private TimeSpan probeRecurrenceInterval;

		private TimeSpan monitorRecurrenceInterval;

		private int probeTimeoutSeconds;

		private int degradedTransitionSpanSeconds;

		private int unhealthyTransitionSpanSeconds;

		private int maxRetryAttempts;

		private int minErrorCount;

		private class ProbeInfo
		{
			public string TypeName { get; set; }

			public string[] Attributes { get; set; }

			public string ProbeName { get; set; }

			public string MonitorName { get; set; }

			public string AlertResponderName { get; set; }
		}
	}
}
