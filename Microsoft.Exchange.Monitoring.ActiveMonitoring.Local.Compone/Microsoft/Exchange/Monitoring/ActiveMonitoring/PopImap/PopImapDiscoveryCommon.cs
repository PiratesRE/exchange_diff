using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.PopImap
{
	public class PopImapDiscoveryCommon : MaintenanceWorkItem
	{
		public static void PopulateDefinition<Definition>(Definition definition, Dictionary<string, string> propertyBag, IPEndPoint mbxEndpoint, IPEndPoint cafeEndpoint, bool isMbxProbe, bool isLightModeProbe)
		{
			MailboxDatabaseInfo monitoringAccount = PopImapDiscoveryCommon.GetMonitoringAccount(isMbxProbe);
			if (monitoringAccount == null)
			{
				throw new ApplicationException("No monitoring account could be found for this server.");
			}
			string value = monitoringAccount.MailboxDatabaseGuid.ToString();
			if (isMbxProbe && !isLightModeProbe && propertyBag.ContainsKey("ExtensionAttributes"))
			{
				Dictionary<string, string> dictionary = DefinitionHelperBase.ConvertExtensionAttributesToDictionary(propertyBag["ExtensionAttributes"]);
				if (dictionary != null && dictionary.ContainsKey("Guid"))
				{
					value = dictionary["Guid"];
				}
			}
			ProbeDefinition probeDefinition = definition as ProbeDefinition;
			if (probeDefinition == null)
			{
				throw new ArgumentException("definition must be a ProbeDefinition");
			}
			probeDefinition.Account = monitoringAccount.MonitoringAccount + "@" + monitoringAccount.MonitoringAccountDomain;
			probeDefinition.AccountPassword = monitoringAccount.MonitoringAccountPassword;
			probeDefinition.TimeoutSeconds = 6;
			probeDefinition.Attributes["LightMode"] = isLightModeProbe.ToString();
			if (isMbxProbe)
			{
				probeDefinition.Endpoint = mbxEndpoint.Address.ToString();
				probeDefinition.SecondaryEndpoint = mbxEndpoint.Port.ToString();
				if (!isLightModeProbe)
				{
					probeDefinition.Attributes["DatabaseGuid"] = value;
				}
			}
			else
			{
				probeDefinition.Endpoint = cafeEndpoint.Address.ToString();
				probeDefinition.SecondaryEndpoint = cafeEndpoint.Port.ToString();
			}
			if (propertyBag.ContainsKey("Account"))
			{
				probeDefinition.Account = propertyBag["Account"];
			}
			if (propertyBag.ContainsKey("Password"))
			{
				probeDefinition.AccountPassword = propertyBag["Password"];
			}
			int timeoutSeconds;
			if (propertyBag.ContainsKey("TimeOutSeconds") && int.TryParse(propertyBag["TimeOutSeconds"], out timeoutSeconds))
			{
				probeDefinition.TimeoutSeconds = timeoutSeconds;
			}
			if (propertyBag.ContainsKey("Endpoint"))
			{
				probeDefinition.Endpoint = propertyBag["Endpoint"];
			}
			if (propertyBag.ContainsKey("SecondaryEndpoint"))
			{
				probeDefinition.SecondaryEndpoint = propertyBag["SecondaryEndpoint"];
			}
		}

		internal static IEnumerable<Microsoft.Office.Datacenter.WorkerTaskFramework.PropertyInformation> GetSubstitutePropertyInfo()
		{
			return new List<Microsoft.Office.Datacenter.WorkerTaskFramework.PropertyInformation>
			{
				new Microsoft.Office.Datacenter.WorkerTaskFramework.PropertyInformation("Endpoint", Strings.PopImapEndpoint, false),
				new Microsoft.Office.Datacenter.WorkerTaskFramework.PropertyInformation("SecondaryEndpoint", Strings.PopImapSecondaryEndpoint, false),
				new Microsoft.Office.Datacenter.WorkerTaskFramework.PropertyInformation("Guid", Strings.PopImapGuid, false)
			};
		}

		internal static ProbeDefinition CreateProbe(string assemblyPath, IPEndPoint targetEndpoint, MailboxDatabaseInfo dbInfo, string probeTypeName, string probeName, int recurrenceInterval, int timeOut, bool lightMode, bool mbxProbe, string targetResource, string healthSet)
		{
			ProbeDefinition probeDefinition = new ProbeDefinition();
			probeDefinition.AssemblyPath = assemblyPath;
			probeDefinition.TargetResource = targetResource;
			probeDefinition.ServiceName = healthSet;
			probeDefinition.TypeName = probeTypeName;
			probeDefinition.Name = probeName;
			probeDefinition.RecurrenceIntervalSeconds = recurrenceInterval;
			probeDefinition.TimeoutSeconds = timeOut;
			probeDefinition.MaxRetryAttempts = 3;
			probeDefinition.Endpoint = targetEndpoint.Address.ToString();
			probeDefinition.SecondaryEndpoint = targetEndpoint.Port.ToString();
			if (lightMode)
			{
				probeDefinition.Attributes["LightMode"] = true.ToString();
			}
			if (mbxProbe)
			{
				probeDefinition.Attributes["MbxProbe"] = true.ToString();
			}
			if (dbInfo != null)
			{
				probeDefinition.Account = dbInfo.MonitoringAccount + "@" + dbInfo.MonitoringAccountDomain;
				probeDefinition.AccountPassword = dbInfo.MonitoringAccountPassword;
				if (!lightMode && mbxProbe)
				{
					probeDefinition.Attributes["DatabaseGuid"] = dbInfo.MailboxDatabaseGuid.ToString();
				}
			}
			return probeDefinition;
		}

		internal static MonitorDefinition CreateMonitor(string assemblyPath, string targetResource, Type monitorType, string monitorName, int recurrenceInterval, int monitoringInterval, int? monitoringThreshold, string sampleMask, Component component)
		{
			MonitorDefinition monitorDefinition = new MonitorDefinition();
			monitorDefinition.AssemblyPath = assemblyPath;
			monitorDefinition.TypeName = monitorType.FullName;
			monitorDefinition.Name = monitorName;
			monitorDefinition.RecurrenceIntervalSeconds = recurrenceInterval;
			monitorDefinition.InsufficientSamplesIntervalSeconds = Math.Max(5 * monitorDefinition.RecurrenceIntervalSeconds, Convert.ToInt32(ConfigurationManager.AppSettings["InsufficientSamplesIntervalInSeconds"]));
			if (recurrenceInterval > 0)
			{
				monitorDefinition.TimeoutSeconds = recurrenceInterval / 2;
			}
			else
			{
				monitorDefinition.TimeoutSeconds = 30;
			}
			monitorDefinition.MaxRetryAttempts = 3;
			monitorDefinition.SampleMask = sampleMask;
			monitorDefinition.MonitoringIntervalSeconds = monitoringInterval;
			monitorDefinition.TargetResource = targetResource;
			monitorDefinition.ServiceName = component.Name;
			monitorDefinition.Component = component;
			if (monitoringThreshold != null)
			{
				monitorDefinition.MonitoringThreshold = (double)monitoringThreshold.Value;
			}
			return monitorDefinition;
		}

		internal static MonitorDefinition CreateServiceMonitor(string monitorName, string probeName, Component component, bool isMbxMonitor)
		{
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(monitorName, probeName, component.Name, component, 5, true, isMbxMonitor ? 900 : 1200);
			monitorDefinition.TargetResource = component.Name;
			return monitorDefinition;
		}

		internal static MonitorStateTransition[] CreateResponderChain(string monitorName, string serviceName, string targetResource, IMaintenanceWorkBroker broker, bool isImap, PopImapDiscoveryCommon.TargetScope responderScope, TracingContext traceContext)
		{
			bool enabled = VariantConfiguration.InvariantNoFlightingSnapshot.ActiveMonitoring.PopImapDiscoveryCommon.Enabled;
			MonitorStateTransition[] result = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded1, 0),
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy1, (int)TimeSpan.FromMinutes(2.0).TotalSeconds),
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy2, (int)TimeSpan.FromMinutes(20.0).TotalSeconds),
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, (int)TimeSpan.FromMinutes(40.0).TotalSeconds)
			};
			string serviceName2 = isImap ? ExchangeComponent.Imap.Name : ExchangeComponent.Pop.Name;
			string probeName = monitorName.Replace("Monitor", "Probe").ToString();
			string text = monitorName.Replace("Monitor", "RestartService").ToString();
			string responderName = monitorName.Replace("Monitor", "CafeOffline").ToString();
			string text2 = monitorName.Replace("Monitor", "Escalate").ToString();
			string escalationSubjectUnhealthy = Strings.EscalationSubjectUnhealthy.ToString();
			string text3 = Strings.GenericOverallXFailureEscalationMessage(serviceName).ToString();
			string format = "<br><br><a href='{0}'>Battlecards</a><br><a href='{1}'>OneNote Battlecards</a>";
			switch (responderScope)
			{
			case PopImapDiscoveryCommon.TargetScope.CTP:
				if (isImap)
				{
					serviceName2 = ExchangeComponent.Imap.Name;
					escalationSubjectUnhealthy = Strings.ImapEscalationSubject(probeName, Environment.MachineName).ToString();
					text3 = (enabled ? Strings.ImapCustomerTouchPointEscalationBodyDC(Environment.MachineName, probeName).ToString() : Strings.ImapCustomerTouchPointEscalationBodyENT(Environment.MachineName, probeName).ToString());
				}
				else
				{
					serviceName2 = ExchangeComponent.Pop.Name;
					escalationSubjectUnhealthy = Strings.PopEscalationSubject(probeName, Environment.MachineName).ToString();
					text3 = (enabled ? Strings.PopCustomerTouchPointEscalationBodyDC(Environment.MachineName, probeName).ToString() : Strings.PopCustomerTouchPointEscalationBodyENT(Environment.MachineName, probeName).ToString());
				}
				break;
			case PopImapDiscoveryCommon.TargetScope.MDT:
				if (isImap)
				{
					serviceName2 = ExchangeComponent.ImapProtocol.Name;
					escalationSubjectUnhealthy = Strings.ImapEscalationSubject(probeName, Environment.MachineName).ToString();
					text3 = (enabled ? Strings.ImapDeepTestEscalationBodyDC(Environment.MachineName, probeName).ToString() : Strings.ImapDeepTestEscalationBodyENT(Environment.MachineName, probeName).ToString());
					if (enabled)
					{
						text3 += string.Format(format, "http://aka.ms/decisiontrees?Page=RecoveryHome&service=Exchange&escalationteam=Pop3,%20Imap4,%20ActiveSync&alerttypeid=ImapSelfTestMonitor&id=0&alertname=dummy", "onenote:///\\\\exstore\\files\\userfiles\\servicesoncall\\OnCall%20Battlecard\\New%20Battlecards.one#EAS/Pop/Imap%20Synthetic%20Self,%20Deep%20and%20Proxy%20Test%20Issues&section-id={9E24DEE5-34D7-4463-AB20-386D859233BA}&page-id={E9CB5DFA-5F3B-4A87-B8D6-42F25F981A8E}&end");
					}
				}
				else
				{
					serviceName2 = ExchangeComponent.PopProtocol.Name;
					escalationSubjectUnhealthy = Strings.PopEscalationSubject(probeName, Environment.MachineName).ToString();
					text3 = (enabled ? Strings.PopDeepTestEscalationBodyDC(Environment.MachineName, probeName).ToString() : Strings.PopDeepTestEscalationBodyENT(Environment.MachineName, probeName).ToString());
					if (enabled)
					{
						text3 += string.Format(format, "http://aka.ms/decisiontrees?Page=RecoveryHome&service=Exchange&escalationteam=Pop3,%20Imap4,%20ActiveSync&alerttypeid=ImapSelfTestMonitor&id=0&alertname=dummy", "onenote:///\\\\exstore\\files\\userfiles\\servicesoncall\\OnCall%20Battlecard\\New%20Battlecards.one#EAS/Pop/Imap%20Synthetic%20Self,%20Deep%20and%20Proxy%20Test%20Issues&section-id={9E24DEE5-34D7-4463-AB20-386D859233BA}&page-id={E9CB5DFA-5F3B-4A87-B8D6-42F25F981A8E}&end");
					}
				}
				break;
			case PopImapDiscoveryCommon.TargetScope.PST:
				if (isImap)
				{
					serviceName2 = ExchangeComponent.ImapProtocol.Name;
					escalationSubjectUnhealthy = Strings.ImapEscalationSubject(probeName, Environment.MachineName).ToString();
					text3 = (enabled ? Strings.ImapSelfTestEscalationBodyDC(Environment.MachineName, probeName).ToString() : Strings.ImapSelfTestEscalationBodyENT(Environment.MachineName, probeName).ToString());
					if (enabled)
					{
						text3 += string.Format(format, "http://aka.ms/decisiontrees?Page=RecoveryHome&service=Exchange&escalationteam=Pop3,%20Imap4,%20ActiveSync&alerttypeid=ImapSelfTestMonitor&id=0&alertname=dummy", "onenote:///\\\\exstore\\files\\userfiles\\servicesoncall\\OnCall%20Battlecard\\New%20Battlecards.one#EAS/Pop/Imap%20Synthetic%20Self,%20Deep%20and%20Proxy%20Test%20Issues&section-id={9E24DEE5-34D7-4463-AB20-386D859233BA}&page-id={E9CB5DFA-5F3B-4A87-B8D6-42F25F981A8E}&end");
					}
				}
				else
				{
					serviceName2 = ExchangeComponent.PopProtocol.Name;
					escalationSubjectUnhealthy = Strings.PopEscalationSubject(probeName, Environment.MachineName).ToString();
					text3 = (enabled ? Strings.PopSelfTestEscalationBodyDC(Environment.MachineName, probeName).ToString() : Strings.PopSelfTestEscalationBodyENT(Environment.MachineName, probeName).ToString());
					if (enabled)
					{
						text3 += string.Format(format, "http://aka.ms/decisiontrees?Page=RecoveryHome&service=Exchange&escalationteam=Pop3,%20Imap4,%20ActiveSync&alerttypeid=ImapSelfTestMonitor&id=0&alertname=dummy", "onenote:///\\\\exstore\\files\\userfiles\\servicesoncall\\OnCall%20Battlecard\\New%20Battlecards.one#EAS/Pop/Imap%20Synthetic%20Self,%20Deep%20and%20Proxy%20Test%20Issues&section-id={9E24DEE5-34D7-4463-AB20-386D859233BA}&page-id={E9CB5DFA-5F3B-4A87-B8D6-42F25F981A8E}&end");
					}
				}
				break;
			case PopImapDiscoveryCommon.TargetScope.PT:
				if (isImap)
				{
					serviceName2 = ExchangeComponent.ImapProxy.Name;
					escalationSubjectUnhealthy = Strings.ImapEscalationSubject(probeName, Environment.MachineName).ToString();
					text3 = (enabled ? Strings.ImapProxyTestEscalationBodyDC(Environment.MachineName, probeName).ToString() : Strings.ImapProxyTestEscalationBodyENT(Environment.MachineName, probeName).ToString());
				}
				else
				{
					serviceName2 = ExchangeComponent.PopProxy.Name;
					escalationSubjectUnhealthy = Strings.PopEscalationSubject(probeName, Environment.MachineName).ToString();
					text3 = (enabled ? Strings.PopProxyTestEscalationBodyDC(Environment.MachineName, probeName).ToString() : Strings.PopProxyTestEscalationBodyENT(Environment.MachineName, probeName).ToString());
				}
				break;
			default:
				WTFDiagnostics.TraceInformation(ExTraceGlobals.IMAP4Tracer, traceContext, "PopImapDiscoveryCommon.DoWork: No TargetScope was passed to responder creation.", null, "CreateResponderChain", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PopImap\\PopImapDiscoveryCommon.cs", 505);
				break;
			}
			if (responderScope == PopImapDiscoveryCommon.TargetScope.PT)
			{
				ResponderDefinition responderDefinition = CafeOfflineResponder.CreateDefinition(responderName, monitorName, isImap ? ServerComponentEnum.ImapProxy : ServerComponentEnum.PopProxy, ServiceHealthStatus.Degraded1, serviceName2, -1.0, "", "Datacenter", "F5AvailabilityData", "MachineOut");
				responderDefinition.TargetResource = targetResource;
				responderDefinition.RecurrenceIntervalSeconds = 60;
				broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, traceContext);
			}
			ResponderDefinition responderDefinition2 = RestartServiceResponder.CreateDefinition(text, monitorName, serviceName, ServiceHealthStatus.Unhealthy1, 300, 300, 0, true, DumpMode.None, null, 25.0, 90, "Exchange", null, true, true, "Dag", false);
			responderDefinition2.ServiceName = serviceName2;
			responderDefinition2.TargetResource = targetResource;
			responderDefinition2.RecurrenceIntervalSeconds = 60;
			responderDefinition2.TimeoutSeconds = (int)TimeSpan.FromMinutes(10.0).TotalSeconds;
			broker.AddWorkDefinition<ResponderDefinition>(responderDefinition2, traceContext);
			ResponderDefinition responderDefinition3 = RestartServiceResponder.CreateDefinition(text + "Late", monitorName, serviceName, ServiceHealthStatus.Unhealthy2, 300, 300, 0, true, DumpMode.None, null, 25.0, 90, "Exchange", null, true, true, "Dag", false);
			responderDefinition3.ServiceName = serviceName2;
			responderDefinition3.TargetResource = targetResource;
			responderDefinition3.RecurrenceIntervalSeconds = 60;
			responderDefinition3.TimeoutSeconds = (int)TimeSpan.FromMinutes(10.0).TotalSeconds;
			broker.AddWorkDefinition<ResponderDefinition>(responderDefinition3, traceContext);
			ResponderDefinition responderDefinition4 = EscalateResponder.CreateDefinition(text2, serviceName2, monitorName, monitorName, targetResource, ServiceHealthStatus.Unrecoverable, "Pop3, Imap4, ActiveSync", escalationSubjectUnhealthy, text3, true, NotificationServiceClass.Urgent, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			responderDefinition4.RecurrenceIntervalSeconds = 60;
			if (text2.Contains("V2"))
			{
				responderDefinition4.NotificationServiceClass = NotificationServiceClass.UrgentInTraining;
			}
			broker.AddWorkDefinition<ResponderDefinition>(responderDefinition4, traceContext);
			return result;
		}

		internal static bool GetEndpointsFromConfig(PopImapAdConfiguration configuration, out IPEndPoint cafeEndpoint, out IPEndPoint mbxEndpoint)
		{
			cafeEndpoint = null;
			mbxEndpoint = null;
			if (configuration == null)
			{
				return false;
			}
			if (configuration.SSLBindings.Count <= 0)
			{
				return false;
			}
			int proxyTargetPort = configuration.ProxyTargetPort;
			IPAddress ipaddress = configuration.SSLBindings[0].Address;
			IPAddress ipaddress2 = (configuration.SSLBindings[0].AddressFamily == AddressFamily.InterNetworkV6) ? IPAddress.IPv6Any : IPAddress.Any;
			IPAddress ipaddress3 = (ipaddress2.AddressFamily == AddressFamily.InterNetworkV6) ? IPAddress.IPv6Loopback : IPAddress.Loopback;
			if (ipaddress.Equals(ipaddress2))
			{
				ipaddress = ipaddress3;
			}
			cafeEndpoint = new IPEndPoint(ipaddress, configuration.SSLBindings[0].Port);
			mbxEndpoint = new IPEndPoint(ipaddress3, proxyTargetPort);
			return true;
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
		}

		private static MailboxDatabaseInfo GetMonitoringAccount(bool isMbxProbe)
		{
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			if (instance.ExchangeServerRoleEndpoint == null)
			{
				return null;
			}
			if (instance.MailboxDatabaseEndpoint == null)
			{
				return null;
			}
			if (isMbxProbe)
			{
				using (IEnumerator<MailboxDatabaseInfo> enumerator = instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MailboxDatabaseInfo mailboxDatabaseInfo = enumerator.Current;
						if (!string.IsNullOrWhiteSpace(mailboxDatabaseInfo.MonitoringAccountPassword))
						{
							return mailboxDatabaseInfo;
						}
					}
					goto IL_A5;
				}
			}
			foreach (MailboxDatabaseInfo mailboxDatabaseInfo2 in instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForCafe)
			{
				if (!string.IsNullOrWhiteSpace(mailboxDatabaseInfo2.MonitoringAccountPassword))
				{
					return mailboxDatabaseInfo2;
				}
			}
			IL_A5:
			return null;
		}

		private const int MaxRetryAttempt = 3;

		private const string RestartImapService = "RestartImapService";

		private const string RestartPopService = "RestartPopService";

		private const string DatabaseFailover = "DatabaseFailover";

		private const string CafeOffline = "CafeOffline";

		private const string KillServer = "KillServer";

		private const string Escalate = "Escalate";

		private const string BattleCardPageUrl = "http://aka.ms/decisiontrees?Page=RecoveryHome&service=Exchange&escalationteam=Pop3,%20Imap4,%20ActiveSync&alerttypeid=ImapSelfTestMonitor&id=0&alertname=dummy";

		private const string OneNoteBattleCardUrl = "onenote:///\\\\exstore\\files\\userfiles\\servicesoncall\\OnCall%20Battlecard\\New%20Battlecards.one#EAS/Pop/Imap%20Synthetic%20Self,%20Deep%20and%20Proxy%20Test%20Issues&section-id={9E24DEE5-34D7-4463-AB20-386D859233BA}&page-id={E9CB5DFA-5F3B-4A87-B8D6-42F25F981A8E}&end";

		public enum TargetScope
		{
			CTP,
			MDT,
			PST,
			PT
		}
	}
}
