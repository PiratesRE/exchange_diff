using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.OAuth
{
	public sealed class OAuthDiscovery : MaintenanceWorkItem
	{
		public int ProbeRecurrenceIntervalSeconds { get; private set; }

		public int ProbeTimeoutSeconds { get; private set; }

		public int FailedProbeThreshold { get; private set; }

		public bool IsAlertResponderEnabled { get; private set; }

		public bool ExchangeProbeEnabled { get; private set; }

		public bool LyncProbeEnabled { get; private set; }

		public bool SharePointProbeEnabled { get; private set; }

		public bool OnPremProbeEnabled { get; private set; }

		public List<Uri> ExchangeServerEndpoints { get; private set; }

		public List<Uri> LyncServerEndpoints { get; private set; }

		public List<Uri> SharePointServerEndpoints { get; private set; }

		public string MonitoringUserIdentity { get; private set; }

		public bool Verbose { get; private set; }

		public bool StartRightAway { get; private set; }

		public bool TrustAnySslCertificate { get; private set; }

		protected override void DoWork(CancellationToken cancellationToken)
		{
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			if (instance.ExchangeServerRoleEndpoint.IsCafeRoleInstalled)
			{
				this.serverToRunExchangeProbe = true;
				WTFDiagnostics.TraceInformation(ExTraceGlobals.EWSTracer, base.TraceContext, "OAuthDiscovery.DoWork: Cafe role is present on this server.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OAuth\\OAuthDiscovery.cs", 157);
			}
			if (instance.ExchangeServerRoleEndpoint.IsCafeRoleInstalled)
			{
				this.serverToRunLyncProbe = true;
				WTFDiagnostics.TraceInformation(ExTraceGlobals.EWSTracer, base.TraceContext, "OAuthDiscovery.DoWork: Cafe role is present on this server.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OAuth\\OAuthDiscovery.cs", 166);
			}
			if (instance.ExchangeServerRoleEndpoint.IsCafeRoleInstalled)
			{
				this.serverToRunSharepointProbe = true;
				WTFDiagnostics.TraceInformation(ExTraceGlobals.EWSTracer, base.TraceContext, "OAuthDiscovery.DoWork: Cafe role is present on this server.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OAuth\\OAuthDiscovery.cs", 175);
			}
			if (!this.serverToRunExchangeProbe && !this.serverToRunLyncProbe && !this.serverToRunSharepointProbe)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.EWSTracer, base.TraceContext, "OAuthDiscovery.DoWork: Server roles are not present on this server.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OAuth\\OAuthDiscovery.cs", 183);
				return;
			}
			this.Configure();
			List<string> oauthMonitoringUsers = this.GetOAuthMonitoringUsers(instance);
			if (oauthMonitoringUsers.Count == 0)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.EWSTracer, base.TraceContext, "OAuthDiscovery.DoWork: Unable to locate valid monitoring user", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OAuth\\OAuthDiscovery.cs", 198);
				return;
			}
			this.CreateProbeChains(oauthMonitoringUsers);
		}

		private void Configure()
		{
			this.Verbose = this.ReadAttribute("Verbose", false);
			this.ProbeRecurrenceIntervalSeconds = (int)this.ReadAttribute("ProbeRecurrenceSpan", TimeSpan.FromSeconds(900.0)).TotalSeconds;
			this.ProbeTimeoutSeconds = (int)this.ReadAttribute("ProbeTimeoutSpan", TimeSpan.FromSeconds(480.0)).TotalSeconds;
			this.FailedProbeThreshold = this.ReadAttribute("FailedProbeThreshold", 4);
			this.TrustAnySslCertificate = this.ReadAttribute("TrustAnySslCertificate", false);
			this.ExchangeProbeEnabled = this.ReadAttribute("ExchangeProbeEnabled", true);
			this.LyncProbeEnabled = this.ReadAttribute("LyncProbeEnabled", true);
			this.SharePointProbeEnabled = this.ReadAttribute("SharePointProbeEnabled", true);
			this.ExchangeServerEndpoints = this.ParseEndpoints(this.ReadAttribute("ExchangeServerEndpoints", string.Empty));
			this.LyncServerEndpoints = this.ParseEndpoints(this.ReadAttribute("LyncServerEndpoints", string.Empty));
			this.SharePointServerEndpoints = this.ParseEndpoints(this.ReadAttribute("SharePointServerEndpoints", string.Empty));
			this.MonitoringUserIdentity = this.ReadAttribute("MonitoringUserIdentity", string.Empty);
			this.IsAlertResponderEnabled = this.ReadAttribute("AlertResponderEnabled", !ExEnvironment.IsTest);
			this.StartRightAway = this.ReadAttribute("StartRightAway", false);
		}

		private List<Uri> ParseEndpoints(string rawEndpoints)
		{
			List<Uri> list = new List<Uri>();
			if (!string.IsNullOrEmpty(rawEndpoints))
			{
				string[] array = rawEndpoints.Split(new char[]
				{
					','
				}, StringSplitOptions.RemoveEmptyEntries);
				foreach (string text in array)
				{
					try
					{
						list.Add(new Uri(text));
					}
					catch (Exception ex)
					{
						WTFDiagnostics.TraceInformation<string, string>(ExTraceGlobals.EWSTracer, base.TraceContext, "OAuthDiscovery.ParseEndpoints: Invalid endpoint URI supplied {0}:{1}", text, ex.Message, null, "ParseEndpoints", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OAuth\\OAuthDiscovery.cs", 271);
						throw;
					}
				}
			}
			return list;
		}

		private List<string> GetOAuthMonitoringUsers(LocalEndpointManager endpointManager)
		{
			List<string> list = new List<string>();
			if (string.IsNullOrEmpty(this.MonitoringUserIdentity))
			{
				if (endpointManager.MailboxDatabaseEndpoint == null || endpointManager.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForCafe.Count == 0)
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.EWSTracer, base.TraceContext, "OAuthDiscovery.GetOAuthMonitoringUsers: mailbox database collection is empty on this server", null, "GetOAuthMonitoringUsers", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OAuth\\OAuthDiscovery.cs", 301);
					return list;
				}
				using (IEnumerator<MailboxDatabaseInfo> enumerator = endpointManager.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForCafe.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MailboxDatabaseInfo mailboxDatabaseInfo = enumerator.Current;
						if (string.IsNullOrWhiteSpace(mailboxDatabaseInfo.MonitoringAccountPassword))
						{
							WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.EWSTracer, base.TraceContext, "OAuthDiscovery.GetOAuthMonitoringUsers: Ignore mailbox database {0} because it does not have monitoring mailbox", mailboxDatabaseInfo.MailboxDatabaseName, null, "GetOAuthMonitoringUsers", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OAuth\\OAuthDiscovery.cs", 313);
						}
						else
						{
							list.Add(mailboxDatabaseInfo.MonitoringAccount + "@" + mailboxDatabaseInfo.MonitoringAccountDomain);
						}
					}
					return list;
				}
			}
			list.Add(this.MonitoringUserIdentity);
			return list;
		}

		private void CreateProbeChains(List<string> monitoringUsers)
		{
			WTFDiagnostics.TraceInformation(ExTraceGlobals.EWSTracer, base.TraceContext, "OAuthDiscovery.CreateProbeChains: Creating OAuth Partner probe chains", null, "CreateProbeChains", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OAuth\\OAuthDiscovery.cs", 341);
			if (LocalEndpointManager.IsDataCenter || this.OnPremProbeEnabled)
			{
				if (this.ExchangeProbeEnabled && this.serverToRunExchangeProbe)
				{
					this.CreateByEndPointPartnerProbeChains(OAuthDiscovery.probeDefinitions["Exchange"], monitoringUsers, this.ExchangeServerEndpoints, new Func<string, string, string, Uri, ProbeDefinition>(OAuthExchangeProbe.CreateDefinition));
				}
				if (this.LyncProbeEnabled && this.serverToRunLyncProbe)
				{
					this.CreateByEndPointPartnerProbeChains(OAuthDiscovery.probeDefinitions["Lync"], monitoringUsers, this.LyncServerEndpoints, new Func<string, string, string, Uri, ProbeDefinition>(OAuthLyncProbe.CreateDefinition));
				}
				if (this.SharePointProbeEnabled && this.serverToRunSharepointProbe)
				{
					this.CreateByEndPointPartnerProbeChains(OAuthDiscovery.probeDefinitions["SharePoint"], monitoringUsers, this.SharePointServerEndpoints, new Func<string, string, string, Uri, ProbeDefinition>(OAuthSharePointProbe.CreateDefinition));
				}
				WTFDiagnostics.TraceInformation(ExTraceGlobals.EWSTracer, base.TraceContext, "OAuthDiscovery.CreateProbeChains: Created OAuth Partner probes chains", null, "CreateProbeChains", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OAuth\\OAuthDiscovery.cs", 375);
			}
		}

		private void CreateByEndPointPartnerProbeChains(OAuthDiscovery.ProbeStuff probeStuff, List<string> monitoringUsers, List<Uri> partnerServerEndpoints, Func<string, string, string, Uri, ProbeDefinition> probeCreateDefinition)
		{
			string monitoringUser = monitoringUsers[0];
			foreach (Uri partnerServerEndpoint in partnerServerEndpoints)
			{
				this.CreateOneProbeChain(probeStuff, monitoringUser, partnerServerEndpoint, probeCreateDefinition);
			}
		}

		private void CreateByMonitoringUserPartnerProbeChains(OAuthDiscovery.ProbeStuff probeStuff, List<string> monitoringUsers, List<Uri> partnerServerEndpoints, Func<string, string, string, Uri, ProbeDefinition> probeCreateDefinition)
		{
			Uri partnerServerEndpoint = null;
			if (partnerServerEndpoints.Count > 0)
			{
				partnerServerEndpoint = partnerServerEndpoints[0];
			}
			using (List<string>.Enumerator enumerator = monitoringUsers.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					string monitoringUser = enumerator.Current;
					this.CreateOneProbeChain(probeStuff, monitoringUser, partnerServerEndpoint, probeCreateDefinition);
				}
			}
		}

		private void CreateOneProbeChain(OAuthDiscovery.ProbeStuff probeStuff, string monitoringUser, Uri partnerServerEndpoint, Func<string, string, string, Uri, ProbeDefinition> probeCreateDefinition)
		{
			string name = probeStuff.Name;
			string arg = (partnerServerEndpoint != null) ? partnerServerEndpoint.Authority : monitoringUser;
			WTFDiagnostics.TraceInformation<string, string, string>(ExTraceGlobals.EWSTracer, base.TraceContext, "OAuthDiscovery.CreateByEndPointPartnerProbeChains: Configuring probe {0} at endpoint {1} with user {2}", name, (partnerServerEndpoint != null) ? partnerServerEndpoint.AbsolutePath : "(none)", monitoringUser, null, "CreateOneProbeChain", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OAuth\\OAuthDiscovery.cs", 456);
			ProbeDefinition probeDefinition = probeCreateDefinition(monitoringUser, name, arg, partnerServerEndpoint);
			probeDefinition.RecurrenceIntervalSeconds = this.ProbeRecurrenceIntervalSeconds;
			probeDefinition.TimeoutSeconds = this.ProbeTimeoutSeconds;
			this.CopyAttributes(probeStuff.Attributes, probeDefinition);
			if (this.StartRightAway)
			{
				probeDefinition.StartTime = DateTime.UtcNow;
			}
			WTFDiagnostics.TraceInformation(ExTraceGlobals.EWSTracer, base.TraceContext, "configuring probe " + probeDefinition.Name, null, "CreateOneProbeChain", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OAuth\\OAuthDiscovery.cs", 484);
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			this.CreateMonitors(probeStuff, probeDefinition);
			WTFDiagnostics.TraceInformation<string, string, string>(ExTraceGlobals.EWSTracer, base.TraceContext, "OAuthDiscovery.CreateByEndPointPartnerProbeChains: Created probes/monitors/responders for probe {0} at endpoint {1} with user {2}", name, (partnerServerEndpoint != null) ? partnerServerEndpoint.AbsolutePath : "(none)", monitoringUser, null, "CreateOneProbeChain", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OAuth\\OAuthDiscovery.cs", 494);
		}

		private void CreateMonitors(OAuthDiscovery.ProbeStuff probeStuff, ProbeDefinition p)
		{
			string monitorName = probeStuff.MonitorName;
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(monitorName, p.ConstructWorkItemResultName(), ExchangeComponent.Ews.Name, ExchangeComponent.Ews, this.FailedProbeThreshold, true, 300);
			monitorDefinition.RecurrenceIntervalSeconds = 0;
			monitorDefinition.MonitoringIntervalSeconds = (this.FailedProbeThreshold + 1) * this.ProbeRecurrenceIntervalSeconds;
			monitorDefinition.TimeoutSeconds = this.ProbeTimeoutSeconds;
			monitorDefinition.MaxRetryAttempts = 0;
			monitorDefinition.TargetResource = p.TargetResource;
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate OAuth health is not impacted by any issues";
			this.CreateResponders(probeStuff, monitorDefinition);
			if (this.StartRightAway)
			{
				monitorDefinition.StartTime = DateTime.UtcNow;
			}
			WTFDiagnostics.TraceInformation(ExTraceGlobals.EWSTracer, base.TraceContext, "configuring monitor " + monitorDefinition.Name, null, "CreateMonitors", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OAuth\\OAuthDiscovery.cs", 544);
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
		}

		private void CreateResponders(OAuthDiscovery.ProbeStuff probeStuff, MonitorDefinition m)
		{
			if (this.IsAlertResponderEnabled)
			{
				string alertResponderName = probeStuff.AlertResponderName;
				string escalationMessageUnhealthy = Strings.EwsAutodEscalationMessageUnhealthy(string.Empty);
				ResponderDefinition responderDefinition = EscalateResponder.CreateDefinition(alertResponderName, ExchangeComponent.Ews.Name, m.Name, m.ConstructWorkItemResultName(), m.TargetResource, ServiceHealthStatus.None, ExchangeComponent.Ews.EscalationTeam, Strings.EscalationSubjectUnhealthy, escalationMessageUnhealthy, true, NotificationServiceClass.Urgent, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
				WTFDiagnostics.TraceInformation(ExTraceGlobals.CafeTracer, base.TraceContext, "configuring escalate responder " + responderDefinition.Name, null, "CreateResponders", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OAuth\\OAuthDiscovery.cs", 583);
				if (this.StartRightAway)
				{
					responderDefinition.StartTime = DateTime.UtcNow;
				}
				base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
			}
		}

		public const int DefaultProbeTimeoutSeconds = 480;

		public const int DefaultProbeRecurrenceIntervalSeconds = 900;

		public const int DefaultFailedProbeThreshold = 4;

		internal static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string[] ProbeAttributes = new string[]
		{
			"TrustAnySslCertificate",
			"Verbose"
		};

		private static readonly Dictionary<string, OAuthDiscovery.ProbeStuff> probeDefinitions = new Dictionary<string, OAuthDiscovery.ProbeStuff>
		{
			{
				"Exchange",
				new OAuthDiscovery.ProbeStuff
				{
					ParterProbeType = "Exchange",
					Name = "OAuthExchangeProbe",
					Attributes = OAuthDiscovery.ProbeAttributes,
					MonitorName = "OAuthExchangeMonitor",
					AlertResponderName = "OAuthExchangeAlert"
				}
			},
			{
				"Lync",
				new OAuthDiscovery.ProbeStuff
				{
					ParterProbeType = "Lync",
					Name = "OAuthLyncProbe",
					Attributes = OAuthDiscovery.ProbeAttributes,
					MonitorName = "OAuthLyncMonitor",
					AlertResponderName = "OAuthLyncAlert"
				}
			},
			{
				"SharePoint",
				new OAuthDiscovery.ProbeStuff
				{
					ParterProbeType = "SharePoint",
					Name = "OAuthSharePointProbe",
					Attributes = OAuthDiscovery.ProbeAttributes,
					MonitorName = "OAuthSharePointMonitor",
					AlertResponderName = "OAuthSharePointAlert"
				}
			}
		};

		private bool serverToRunExchangeProbe;

		private bool serverToRunLyncProbe;

		private bool serverToRunSharepointProbe;

		private class ProbeStuff
		{
			public string ParterProbeType { get; set; }

			public string Name { get; set; }

			public string[] Attributes { get; set; }

			public string MonitorName { get; set; }

			public string AlertResponderName { get; set; }
		}
	}
}
