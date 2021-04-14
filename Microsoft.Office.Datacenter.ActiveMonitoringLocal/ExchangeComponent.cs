using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Exchange.Common;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	internal static class ExchangeComponent
	{
		static ExchangeComponent()
		{
			bool flag = Datacenter.IsMicrosoftHostedOnly(false) || Datacenter.IsDatacenterDedicated(false);
			Dictionary<string, ServerComponentEnum> dictionary = new Dictionary<string, ServerComponentEnum>(StringComparer.InvariantCultureIgnoreCase);
			dictionary.Add("ECP.Proxy", ServerComponentEnum.EcpProxy);
			dictionary.Add("OAB.Proxy", ServerComponentEnum.OabProxy);
			dictionary.Add("OWA.Proxy", ServerComponentEnum.OwaProxy);
			dictionary.Add("Outlook.Proxy", ServerComponentEnum.RpcProxy);
			dictionary.Add("OutlookMapiHttp.Proxy", ServerComponentEnum.MapiProxy);
			dictionary.Add("EWS.Proxy", ServerComponentEnum.EwsProxy);
			dictionary.Add("ActiveSync.Proxy", ServerComponentEnum.ActiveSyncProxy);
			dictionary.Add("Autodiscover.Proxy", ServerComponentEnum.AutoDiscoverProxy);
			dictionary.Add("XRop.Proxy", ServerComponentEnum.XropProxy);
			dictionary.Add("RWS.Proxy", ServerComponentEnum.RwsProxy);
			dictionary.Add("RPS.Proxy", ServerComponentEnum.RpsProxy);
			dictionary.Add("PushNotifications.Proxy", ServerComponentEnum.PushNotificationsProxy);
			Dictionary<string, ServerComponentEnum> dictionary2 = new Dictionary<string, ServerComponentEnum>(StringComparer.InvariantCultureIgnoreCase);
			foreach (string key in dictionary.Keys)
			{
				dictionary2[key] = dictionary[key];
			}
			if (flag)
			{
				Dictionary<string, ServerComponentEnum> dictionary3 = ExchangeComponent.RetrieveHttpProxyServerComponentOverride();
				foreach (string key2 in dictionary3.Keys)
				{
					if (dictionary2.ContainsKey(key2))
					{
						dictionary2[key2] = dictionary3[key2];
					}
				}
			}
			ExchangeComponent.OwaProxy = new Component("OWA.Proxy", HealthGroup.ServiceComponents, ExchangeComponent.Cafe.EscalationTeam, "Exchange", ManagedAvailabilityPriority.Low, dictionary2["OWA.Proxy"]);
			ExchangeComponent.OutlookProxy = new Component("Outlook.Proxy", HealthGroup.ServiceComponents, ExchangeComponent.Cafe.EscalationTeam, "Exchange", ManagedAvailabilityPriority.Low, dictionary2["Outlook.Proxy"]);
			ExchangeComponent.OutlookMapiProxy = new Component("OutlookMapiHttp.Proxy", HealthGroup.ServiceComponents, ExchangeComponent.Cafe.EscalationTeam, "Exchange", ManagedAvailabilityPriority.Low, dictionary2["OutlookMapiHttp.Proxy"]);
			ExchangeComponent.EwsProxy = new Component("EWS.Proxy", HealthGroup.ServiceComponents, ExchangeComponent.Cafe.EscalationTeam, "Exchange", ManagedAvailabilityPriority.Low, dictionary2["EWS.Proxy"]);
			ExchangeComponent.ActiveSyncProxy = new Component("ActiveSync.Proxy", HealthGroup.ServiceComponents, ExchangeComponent.Cafe.EscalationTeam, "Exchange", ManagedAvailabilityPriority.Low, dictionary2["ActiveSync.Proxy"]);
			ExchangeComponent.AutodiscoverProxy = new Component("Autodiscover.Proxy", HealthGroup.ServiceComponents, ExchangeComponent.Cafe.EscalationTeam, "Exchange", ManagedAvailabilityPriority.Low, dictionary2["Autodiscover.Proxy"]);
			ExchangeComponent.PushNotificationsProxy = new Component("PushNotifications.Proxy", HealthGroup.ServiceComponents, "Push Notification Services", "Exchange", ManagedAvailabilityPriority.Low, dictionary2["PushNotifications.Proxy"]);
			ExchangeComponent.EcpProxy = new Component("ECP.Proxy", HealthGroup.ServiceComponents, ExchangeComponent.Cafe.EscalationTeam, "Exchange", ManagedAvailabilityPriority.High, dictionary2["ECP.Proxy"]);
			ExchangeComponent.RpsProxy = new Component("RPS.Proxy", HealthGroup.ServiceComponents, ExchangeComponent.Cafe.EscalationTeam, "Exchange", ManagedAvailabilityPriority.Low, dictionary2["RPS.Proxy"]);
			ExchangeComponent.OabProxy = new Component("OAB.Proxy", HealthGroup.ServiceComponents, "People911", "Exchange", ManagedAvailabilityPriority.Low, dictionary2["OAB.Proxy"]);
			ExchangeComponent.RwsProxy = new Component("RWS.Proxy", HealthGroup.ServiceComponents, "Reporting Web Service", "Exchange", ManagedAvailabilityPriority.Low, dictionary2["RWS.Proxy"]);
			ExchangeComponent.XropProxy = new Component("XRop.Proxy", HealthGroup.ServiceComponents, ExchangeComponent.Cafe.EscalationTeam, "Exchange", ManagedAvailabilityPriority.Low, dictionary2["XRop.Proxy"]);
			ExchangeComponent.WellKnownComponents = (from field in typeof(ExchangeComponent).GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
			where field.FieldType == typeof(Component)
			select (Component)field.GetValue(null)).ToDictionary((Component component) => component.Name, StringComparer.InvariantCultureIgnoreCase);
		}

		private static Dictionary<string, ServerComponentEnum> RetrieveHttpProxyServerComponentOverride()
		{
			Dictionary<string, ServerComponentEnum> dictionary = new Dictionary<string, ServerComponentEnum>(StringComparer.InvariantCultureIgnoreCase);
			if (!string.IsNullOrEmpty(Settings.HttpProxyAvailabilityGroup))
			{
				string[] array = Settings.HttpProxyAvailabilityGroup.Split(new char[]
				{
					','
				}, StringSplitOptions.RemoveEmptyEntries);
				foreach (string text in array)
				{
					string[] array3 = text.Split(new char[]
					{
						':'
					}, StringSplitOptions.RemoveEmptyEntries);
					if (array3 != null && array3.Length == 2)
					{
						dictionary.Add(array3[0], (ServerComponentEnum)Enum.Parse(typeof(ServerComponentEnum), array3[1], true));
					}
				}
			}
			return dictionary;
		}

		private const string RecoveryActionComponentName = "RecoveryAction";

		private const string OwaProxyComponentName = "OWA.Proxy";

		private const string EcpProxyComponentName = "ECP.Proxy";

		private const string OutlookProxyComponentName = "Outlook.Proxy";

		private const string OutlookMapiProxyComponentName = "OutlookMapiHttp.Proxy";

		private const string EwsProxyComponentName = "EWS.Proxy";

		private const string ActiveSyncProxyComponentName = "ActiveSync.Proxy";

		private const string AutodiscoverProxyComponentName = "Autodiscover.Proxy";

		private const string XropProxyComponentName = "XRop.Proxy";

		private const string RwsProxyComponentName = "RWS.Proxy";

		private const string RpsProxyComponentName = "RPS.Proxy";

		private const string PushNotificationsProxyComponentName = "PushNotifications.Proxy";

		private const string OabProxyComponentName = "OAB.Proxy";

		internal static readonly Component RecoveryAction = new Component("RecoveryAction", HealthGroup.ServiceComponents, "Monitoring", "Exchange", ManagedAvailabilityPriority.Normal);

		internal static readonly Component Cafe = new Component("ClientAccess.Proxy", HealthGroup.ServiceComponents, "Client Access Front End", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component Owa = new Component("OWA", HealthGroup.CustomerTouchPoints, "OWA", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component OwaAttachments = new Component("OWA.Attachments", HealthGroup.CustomerTouchPoints, "OWA Attachments team", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component OwaWebServices = new Component("OWA.WebServices", HealthGroup.CustomerTouchPoints, "Web Services", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component OwaSuiteServices = new Component("OWA.SuiteServices", HealthGroup.CustomerTouchPoints, "Suite UX Services", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component OwaProxy;

		internal static readonly Component OwaProtocol = new Component("OWA.Protocol", HealthGroup.ServiceComponents, "OWA", "Exchange", ManagedAvailabilityPriority.Critical);

		internal static readonly Component OwaDependency = new Component("OWA.Protocol.Dep", HealthGroup.KeyDependencies, "OWA", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component Outlook = new Component("Outlook", HealthGroup.CustomerTouchPoints, "MOMT and XSO", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component OutlookMapiHttp = new Component("OutlookMapiHttp", HealthGroup.CustomerTouchPoints, "MOMT and XSO", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component HxServiceMail = new Component("HxService.Mail", HealthGroup.ServiceComponents, "HxService-Mail", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component HxServiceCalendar = new Component("HxService.Calendar", HealthGroup.ServiceComponents, "HxService-Calendar", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component OutlookProtocol = new Component("Outlook.Protocol", HealthGroup.ServiceComponents, ExchangeComponent.Outlook.EscalationTeam, "Exchange", ManagedAvailabilityPriority.Critical);

		internal static readonly Component OutlookMapiHttpProtocol = new Component("OutlookMapiHttp.Protocol", HealthGroup.ServiceComponents, ExchangeComponent.Outlook.EscalationTeam, "Exchange", ManagedAvailabilityPriority.Critical);

		internal static readonly Component UnifiedMailbox = new Component("UnifiedMailbox", HealthGroup.CustomerTouchPoints, "Web Services", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component OutlookProxy;

		internal static readonly Component OutlookMapiProxy;

		internal static readonly Component Calendaring = new Component("Calendaring", HealthGroup.CustomerTouchPoints, "Calendaring/Sharing", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component FreeBusy = new Component("FreeBusy", HealthGroup.ServiceComponents, "Calendaring/Sharing", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component Elc = new Component("ELC", HealthGroup.ServiceComponents, "MRM/Archive", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component Pop = new Component("POP", HealthGroup.CustomerTouchPoints, "Pop3, Imap4, ActiveSync", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component PopProtocol = new Component("POP.Protocol", HealthGroup.ServiceComponents, "Pop3, Imap4, ActiveSync", "Exchange", ManagedAvailabilityPriority.Normal);

		internal static readonly Component PopProxy = new Component("POP.Proxy", HealthGroup.ServiceComponents, "Pop3, Imap4, ActiveSync", "Exchange", ManagedAvailabilityPriority.Low, ServerComponentEnum.PopProxy);

		internal static readonly Component PushNotifications = new Component("PushNotifications", HealthGroup.CustomerTouchPoints, "Push Notification Services", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component PushNotificationsProxy;

		internal static readonly Component PushNotificationsProtocol = new Component("PushNotifications.Protocol", HealthGroup.ServiceComponents, "Push Notification Services", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component Imap = new Component("IMAP", HealthGroup.CustomerTouchPoints, "Pop3, Imap4, ActiveSync", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component ImapProtocol = new Component("IMAP.Protocol", HealthGroup.ServiceComponents, "Pop3, Imap4, ActiveSync", "Exchange", ManagedAvailabilityPriority.Normal);

		internal static readonly Component ImapProxy = new Component("IMAP.Proxy", HealthGroup.ServiceComponents, "Pop3, Imap4, ActiveSync", "Exchange", ManagedAvailabilityPriority.Low, ServerComponentEnum.ImapProxy);

		internal static readonly Component Eas = new Component("EAS", HealthGroup.CustomerTouchPoints, "Pop3, Imap4, ActiveSync", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component ActiveSyncProxy;

		internal static readonly Component SharedCache = new Component("SharedCache", HealthGroup.ServiceComponents, ExchangeComponent.Cafe.EscalationTeam, "Exchange", ManagedAvailabilityPriority.Low, ServerComponentEnum.SharedCache);

		internal static readonly Component ActiveSync = new Component("ActiveSync", HealthGroup.CustomerTouchPoints, "Pop3, Imap4, ActiveSync", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component ActiveSyncProtocol = new Component("ActiveSync.Protocol", HealthGroup.ServiceComponents, "Pop3, Imap4, ActiveSync", "Exchange", ManagedAvailabilityPriority.High);

		internal static readonly Component Ews = new Component("EWS", HealthGroup.CustomerTouchPoints, "Web Services", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component OnlineMeeting = new Component("OnlineMeeting", HealthGroup.CustomerTouchPoints, "Calendaring/Sharing", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component EwsProtocol = new Component("EWS.Protocol", HealthGroup.ServiceComponents, "Web Services", "Exchange", ManagedAvailabilityPriority.High);

		internal static readonly Component EwsProxy;

		internal static readonly Component Autodiscover = new Component("Autodiscover", HealthGroup.CustomerTouchPoints, "Web Services", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component AutodiscoverProtocol = new Component("Autodiscover.Protocol", HealthGroup.ServiceComponents, "Web Services", "Exchange", ManagedAvailabilityPriority.High);

		internal static readonly Component AutodiscoverProxy;

		internal static readonly Component UMCallRouter = new Component("UM.CallRouter", HealthGroup.ServiceComponents, "Unified Messaging", "Exchange", ManagedAvailabilityPriority.Low, ServerComponentEnum.UMCallRouter);

		internal static readonly Component UMProtocol = new Component("UM.Protocol", HealthGroup.ServiceComponents, "Unified Messaging", "Exchange", ManagedAvailabilityPriority.Normal);

		internal static readonly Component UM = new Component("UM", HealthGroup.CustomerTouchPoints, "Unified Messaging", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component Mailflow = new Component("Mailflow", HealthGroup.CustomerTouchPoints, "E15Transport", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component OutsideInMailflow = new Component("Outside-In Mailflow", HealthGroup.CustomerTouchPoints, "E15Transport", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component Provisioning = new Component("Provisioning", HealthGroup.CustomerTouchPoints, "Provisioning", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component ForwardSync = new Component("ForwardSync", HealthGroup.CustomerTouchPoints, "Provisioning", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component Umc = new Component("UMC", HealthGroup.CustomerTouchPoints, "Monitoring", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component Ecp = new Component("ECP", HealthGroup.CustomerTouchPoints, "ECP", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component InstantMessaging = new Component("Owa.InstantMessaging", HealthGroup.KeyDependencies, "OWA", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component EcpProxy;

		internal static readonly Component Osp = new Component("OSP", HealthGroup.CustomerTouchPoints, "OSP", "OBD", ManagedAvailabilityPriority.Low);

		internal static readonly Component SmartAlerts = new Component("SmartAlerts", HealthGroup.ServiceComponents, "SmartAlerts Team", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component Rps = new Component("RPS", HealthGroup.CustomerTouchPoints, "Cmdlet Infra/Recipients/RPS", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component RpsProtocol = new Component("RPS.Protocol", HealthGroup.ServiceComponents, "Cmdlet Infra/Recipients/RPS", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component RpsProxy;

		internal static readonly Component Store = new Component("Store", HealthGroup.ServiceComponents, "Store", "Exchange", ManagedAvailabilityPriority.Normal);

		internal static readonly Component RemoteStore = new Component("RemoteStore", HealthGroup.ServiceComponents, "Store", "Exchange", ManagedAvailabilityPriority.Normal);

		internal static readonly Component MailboxSpace = new Component("MailboxSpace", HealthGroup.ServerResources, "Store", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component EventAssistants = new Component("EventAssistants", HealthGroup.ServiceComponents, "Store", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component Transport = new Component("Transport", HealthGroup.ServiceComponents, "E15Transport", "Exchange", ManagedAvailabilityPriority.Low, ServerComponentEnum.HubTransport);

		internal static readonly Component Eds = new Component("EDS", HealthGroup.ServiceComponents, "Performance", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component Pum = new Component("Pum", HealthGroup.ServiceComponents, "Performance", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component DataProtection = new Component("DataProtection", HealthGroup.ServiceComponents, "High Availability", "Exchange", ManagedAvailabilityPriority.High);

		internal static readonly Component BitlockerDeployment = new Component("BitlockerDeployment", HealthGroup.ServiceComponents, "High Availability", "Exchange", ManagedAvailabilityPriority.High);

		internal static readonly Component Clustering = new Component("Clustering", HealthGroup.ServerResources, "High Availability", "Exchange", ManagedAvailabilityPriority.Normal);

		internal static readonly Component FEP = new Component("FEP", HealthGroup.ServiceComponents, "Security", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component DiskController = new Component("DiskController", HealthGroup.ServerResources, "High Availability", "Exchange", ManagedAvailabilityPriority.Normal);

		internal static readonly Component Mrs = new Component("MRS", HealthGroup.ServiceComponents, "Mailbox Migration", "Exchange", ManagedAvailabilityPriority.Normal);

		internal static readonly Component Oab = new Component("OAB", HealthGroup.ServiceComponents, "People911", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component OabProxy;

		internal static readonly Component Search = new Component("Search", HealthGroup.ServiceComponents, "Search", "Exchange", ManagedAvailabilityPriority.High);

		internal static readonly Component Inference = new Component("Inference", HealthGroup.ServiceComponents, "Inference", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component Monitoring = new Component("Monitoring", HealthGroup.ServiceComponents, "Monitoring", "Exchange", ManagedAvailabilityPriority.Normal);

		internal static readonly Component RemoteMonitoring = new Component("RemoteMonitoring", HealthGroup.ServiceComponents, "Monitoring", "Exchange", ManagedAvailabilityPriority.Normal);

		internal static readonly Component LighthouseMonitoring = new Component("LighthouseMonitoring", HealthGroup.ServiceComponents, "Monitoring", "Exchange", ManagedAvailabilityPriority.Normal, ServerComponentEnum.Monitoring);

		internal static readonly Component Security = new Component("Security", HealthGroup.ServiceComponents, "Security", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component ProcessIsolation = new Component("ProcessIsolation", HealthGroup.ServiceComponents, "Performance", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component UserThrottling = new Component("UserThrottling", HealthGroup.ServiceComponents, "Workload Management", "Exchange", ManagedAvailabilityPriority.Normal);

		internal static readonly Component Datamining = new Component("Datamining", HealthGroup.ServiceComponents, "Optics", "OBD", ManagedAvailabilityPriority.Low);

		internal static readonly Component ServiceAvailability = new Component("ServiceAvailability", HealthGroup.ServiceComponents, "Service Availability", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component RedAlert = new Component("RedAlert", HealthGroup.ServiceComponents, "Service Availability", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component STX = new Component("STX", HealthGroup.ServiceComponents, "Service Availability", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component FfoWebService = new Component("FfoWebService", HealthGroup.ServiceComponents, "Directory and Database Infrastructure", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component FfoBackground = new Component("FfoBackground", HealthGroup.ServiceComponents, "Service Automation & Monitoring", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component AnalystAlertingUntriagedFpExceededThreshold = new Component("AnalystAlertingUntriagedFpExceededThreshold", HealthGroup.ServiceComponents, "AntiSpam", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component AnalystAlertingUntriagedFnExceededThreshold = new Component("AnalystAlertingUntriagedFnExceededThreshold", HealthGroup.ServiceComponents, "AntiSpam", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component AnalystAlertingUntriagedSenExceededThreshold = new Component("AnalystAlertingUntriagedSenExceededThreshold", HealthGroup.ServiceComponents, "AntiSpam", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component AnalystAlertingUntriagedSewrExceededThreshold = new Component("AnalystAlertingUntriagedSewrExceededThreshold", HealthGroup.ServiceComponents, "AntiSpam", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component AnalystAlertingUntriagedFingerprintsExceededThreshold = new Component("AnalystAlertingUntriagedFingerprintsExceededThreshold", HealthGroup.ServiceComponents, "AntiSpam", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component AnalystAlertingUntriagedFpRulesClusterThreshold = new Component("AnalystAlertingUntriagedFpRulesClusterThreshold", HealthGroup.ServiceComponents, "AntiSpam", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component AnalystAlertingTopSubjectsExceededThreshold = new Component("AnalystAlertingTopSubjectsExceededThreshold", HealthGroup.ServiceComponents, "AntiSpam", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component AnalystAlertingFpUriRulesExceededThreshold = new Component("AnalystAlertingFpUriRulesExceededThreshold", HealthGroup.ServiceComponents, "AntiSpam", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component AnalystAlertingFpSpamRulesExceededThreshold = new Component("AnalystAlertingFpSpamRulesExceededThreshold", HealthGroup.ServiceComponents, "AntiSpam", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component AnalystAlertingSenderIpFpExceededThreshold = new Component("AnalystAlertingSenderIpFpExceededThreshold", HealthGroup.ServiceComponents, "AntiSpam", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component AnalystAlertingSenderIpFnExceededThreshold = new Component("AnalystAlertingSenderIpFnExceededThreshold", HealthGroup.ServiceComponents, "AntiSpam", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component AnalystAlertingUntriagedSubmitterExceededThreshold = new Component("AnalystAlertingUntriagedSubmitterExceededThreshold", HealthGroup.ServiceComponents, "AntiSpam", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component AnalystAlertingNoFpTraffic = new Component("AnalystAlertingNoFpTraffic", HealthGroup.ServiceComponents, "AntiSpam", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component AnalystAlertingNoFnTraffic = new Component("AnalystAlertingNoFnTraffic", HealthGroup.ServiceComponents, "AntiSpam", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component AnalystAlertingNoSenTraffic = new Component("AnalystAlertingNoSenTraffic", HealthGroup.ServiceComponents, "AntiSpam", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component AnalystAlertingNoSewrTraffic = new Component("AnalystAlertingNoSewrTraffic", HealthGroup.ServiceComponents, "AntiSpam", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component AnalystAlertingNoHpTraffic = new Component("AnalystAlertingNoHpTraffic", HealthGroup.ServiceComponents, "AntiSpam", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component FfoBackgroundFwdSync = new Component("FfoBackgroundFwdSync", HealthGroup.ServiceComponents, "Directory and Database Infrastructure", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component FfoBackgroundIpListGen = new Component("FfoBackgroundIpListGen", HealthGroup.ServiceComponents, "AntiSpam", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component FfoBackgroundUriPuller = new Component("FfoBackgroundUriPuller", HealthGroup.ServiceComponents, "AntiSpam", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component FfoBackgroundAntiSpamDataPumper = new Component("FfoBackgroundAntiSpamDataPumper", HealthGroup.ServiceComponents, "AntiSpam", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component FfoBackgroundAntiSpamScheduler = new Component("FfoBackgroundAntiSpamScheduler", HealthGroup.ServiceComponents, "AntiSpam", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component FfoBackgroundOutSpamAlert = new Component("FfoBackgroundOutSpamAlert", HealthGroup.ServiceComponents, "AntiSpam", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component FfoBackgroundRulesPub = new Component("FfoBackgroundRulesPub", HealthGroup.ServiceComponents, "AntiSpam", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component FfoBackgroundPackageManager = new Component("FfoBackgroundPackageManager", HealthGroup.ServiceComponents, "AntiSpam", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component FfoBackgroundInterServiceSpamDataSync = new Component("FfoBackgroundInterServiceSpamDataSync", HealthGroup.ServiceComponents, "AntiSpam", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component FfoBackgroundUriGen = new Component("FfoBackgroundUriGen", HealthGroup.ServiceComponents, "AntiSpam", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component FfoBackgroundAsyncQueue = new Component("FfoBackgroundAsyncQueue", HealthGroup.ServiceComponents, "Directory and Database Infrastructure", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component FfoBackgroundQuarMgr = new Component("FfoBackgroundQuarMgr", HealthGroup.ServiceComponents, "UI and Manageability", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component FfoBackgroundDataPump = new Component("FfoBackgroundDataPump", HealthGroup.ServiceComponents, "Intelligence and Reporting", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component FfoBackgroundJobSched = new Component("FfoBackgroundJobSched", HealthGroup.ServiceComponents, "Intelligence and Reporting", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component FfoBackgroundLatencyBucket = new Component("FfoBackgroundLatencyBucket", HealthGroup.ServiceComponents, "Directory and Database Infrastructure", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component FfoBackgroundQueueDigestMon = new Component("FfoBackgroundQueueDigestMon", HealthGroup.ServiceComponents, "E15Transport", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component FfoBackgroundCacheGen = new Component("FfoBackgroundCacheGen", HealthGroup.ServiceComponents, "Directory and Database Infrastructure", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component FfoBackgroundIpRepCheck = new Component("FfoBackgroundIpRepCheck", HealthGroup.ServiceComponents, "AntiSpam", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component FfoBackgroundRulesDataBlobGen = new Component("FfoBackgroundRulesDataBlobGen", HealthGroup.ServiceComponents, "AntiSpam", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component FfoDatabase = new Component("FfoDatabase", HealthGroup.ServiceComponents, "Directory and Database Infrastructure", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component DiskSpace = new Component("DiskSpace", HealthGroup.ServerResources, "Ops support", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component Cpu = new Component("CPU", HealthGroup.ServerResources, "Performance", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component Memory = new Component("Memory", HealthGroup.ServerResources, "Performance", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component AD = new Component("AD", HealthGroup.KeyDependencies, "Directory and LiveId Auth", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component Network = new Component("Network", HealthGroup.KeyDependencies, "Networking", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component LiveId = new Component("LiveId", HealthGroup.KeyDependencies, "Directory and LiveId Auth", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component OrgId = new Component("OrgId", HealthGroup.KeyDependencies, "Directory and LiveId Auth", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component Mserv = new Component("Mserv", HealthGroup.KeyDependencies, "Directory and LiveId Auth", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component AntiSpam = new Component("AntiSpam", HealthGroup.ServiceComponents, "AntiSpam", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component Capacity = new Component("Capacity", HealthGroup.ServiceComponents, "Capacity", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component Wascl = new Component("Wascl", HealthGroup.ServiceComponents, "Wascl", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component Classification = new Component("Classification", HealthGroup.ServiceComponents, "Classification", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component CentralAdmin = new Component("CentralAdmin", HealthGroup.ServiceComponents, "Central Admin", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component Dal = new Component("DAL", HealthGroup.ServiceComponents, "Directory and Database Infrastructure", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component Deployment = new Component("Deployment", HealthGroup.ServiceComponents, "Deployment", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component Dns = new Component("DNS", HealthGroup.CustomerTouchPoints, "GLS", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component Fips = new Component("FIPS", HealthGroup.ServiceComponents, "FIPS", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component Antimalware = new Component("Antimalware", HealthGroup.ServiceComponents, "FIPS", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component AMEUS = new Component("AMEUS", HealthGroup.ServiceComponents, "FIPS", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component AMFailedUpdate = new Component("AMFailedUpdate", HealthGroup.ServiceComponents, "FIPS", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component RusPublisherWeb = new Component("RusPublisherWeb", HealthGroup.ServiceComponents, "FIPS", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component EngineUpdatePublisher = new Component("EngineUpdatePublisher", HealthGroup.ServiceComponents, "FIPS", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component GenericRusClient = new Component("GenericRusClient", HealthGroup.ServiceComponents, "FIPS", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component GenericRusServer = new Component("GenericRusServer", HealthGroup.ServiceComponents, "FIPS", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component GenericUpdateService = new Component("GenericUpdateService", HealthGroup.ServiceComponents, "FIPS", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component AMService = new Component("AMService", HealthGroup.ServiceComponents, "FIPS", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component AMFMSService = new Component("AMFMSService", HealthGroup.ServiceComponents, "FIPS", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component AMUnifiedContentError = new Component("AMUnifiedContentError", HealthGroup.ServiceComponents, "FIPS", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component AMScannerCrash = new Component("AMScannerCrash", HealthGroup.ServiceComponents, "FIPS", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component AMScanners = new Component("AMScanners", HealthGroup.ServiceComponents, "FIPS", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component AMScanTimeout = new Component("AMScanTimeout", HealthGroup.ServiceComponents, "FIPS", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component AMMessagesDeferred = new Component("AMMessagesDeferred", HealthGroup.ServiceComponents, "FIPS", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component AMScanError = new Component("AMScanError", HealthGroup.ServiceComponents, "FIPS", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component AMSMTPProbe = new Component("AMSMTPProbe", HealthGroup.ServiceComponents, "FIPS", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component AMADError = new Component("AMADError", HealthGroup.ServiceComponents, "FIPS", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component AMTenantConfigError = new Component("AMTenantConfigError", HealthGroup.ServiceComponents, "FIPS", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component FfoRps = new Component("FfoRps", HealthGroup.ServiceComponents, "FIPS", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component FfoRws = new Component("FfoRws", HealthGroup.ServiceComponents, "UI and Manageability", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component FfoMobileDevices = new Component("FfoMobileDevices", HealthGroup.ServiceComponents, "Mobile Devices", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component FfoDatamining = new Component("FfoDatamining", HealthGroup.ServiceComponents, "Intelligence and Reporting", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component FfoDatamining_UrgentInTraining = new Component("FfoDatamining_UrgentInTraining", HealthGroup.ServiceComponents, "Intelligence and Reporting", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component FfoDatamining_Urgent = new Component("FfoDatamining_Urgent", HealthGroup.ServiceComponents, "Intelligence and Reporting", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component EmailManagement = new Component("EmailManagement", HealthGroup.ServiceComponents, "Intelligence and Reporting", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component FfoMonitoring = new Component("FfoMonitoring", HealthGroup.ServiceComponents, "Service Automation & Monitoring", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component FfoProvisioning = new Component("FfoProvisioning", HealthGroup.ServiceComponents, "Directory and Database Infrastructure", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component FfoQuarantine = new Component("FfoQuarantine", HealthGroup.ServiceComponents, "UI and Manageability", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component FfoTransport = new Component("FfoTransport", HealthGroup.ServiceComponents, "E15Transport", "Exchange", ManagedAvailabilityPriority.Normal);

		internal static readonly Component FfoWebstore = new Component("FfoWebstore", HealthGroup.ServiceComponents, "Deployment and Configuration Management", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component FfoUmc = new Component("FfoUmc", HealthGroup.CustomerTouchPoints, "UI and Manageability", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component FfoUcc = new Component("FfoUcc", HealthGroup.CustomerTouchPoints, "UI and Manageability", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component Gls = new Component("GLS", HealthGroup.ServiceComponents, "GLS", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component Gls_Local_Urgent = new Component("GLS_Local_Urgent", HealthGroup.ServiceComponents, "GLS", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component Gls_Local_UrgentInTraining = new Component("GLS_Local_UrgentInTraining", HealthGroup.ServiceComponents, "GLS", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component Gls_External_Urgent = new Component("GLS_External_Urgent", HealthGroup.ServiceComponents, "GLS", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component Gls_External_UrgentInTraining = new Component("GLS_External_UrgentInTraining", HealthGroup.ServiceComponents, "GLS", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component Smtp = new Component("SMTP", HealthGroup.CustomerTouchPoints, "E15Transport", "Exchange", ManagedAvailabilityPriority.Low, ServerComponentEnum.HubTransport);

		internal static readonly Component TransportSync = new Component("TransportSync", HealthGroup.ServiceComponents, "Mailbox Migration", "Exchange", ManagedAvailabilityPriority.Normal);

		internal static readonly Component HDPhoto = new Component("HDPhoto", HealthGroup.ServiceComponents, "People911", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component PeopleConnect = new Component("PeopleConnect", HealthGroup.ServiceComponents, "People911", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component Rws = new Component("RWS", HealthGroup.CustomerTouchPoints, "Reporting Web Service", "OBD", ManagedAvailabilityPriority.Low);

		internal static readonly Component RwsProxy;

		internal static readonly Component Compliance = new Component("Compliance", HealthGroup.CustomerTouchPoints, "MRM/Archive", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component ExtendedReportWeb = new Component("ExtendedReportWeb", HealthGroup.ServiceComponents, "Intelligence and Reporting", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component MessageTracing = new Component("MessageTracing", HealthGroup.ServiceComponents, "Intelligence and Reporting", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component MSExchangeCertificateDeployment = new Component("MSExchangeCertificateDeployment", HealthGroup.ServiceComponents, "MSExchangeCertificateDeployment", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component Rms = new Component("RMS", HealthGroup.ServiceComponents, "RMS", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component MailboxMigration = new Component("MailboxMigration", HealthGroup.ServiceComponents, "Mailbox Migration", "Exchange", ManagedAvailabilityPriority.Normal);

		internal static readonly Component FfoSelfRecoveryFx = new Component("FfoSelfRecoveryFx", HealthGroup.ServiceComponents, "Deployment and Configuration Management", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component FfoRaaNetworkValidator = new Component("FfoRaaNetworkValidator", HealthGroup.ServiceComponents, "Network", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component FfoRaaService = new Component("FfoRaaService", HealthGroup.ServiceComponents, "Deployment and Configuration Management", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component FfoWebserviceF5Threshold = new Component("FfoWebserviceF5Threshold", HealthGroup.ServiceComponents, "Deployment and Configuration Management", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component FfoHubTransportF5Threshold = new Component("FfoHubTransportF5Threshold", HealthGroup.ServiceComponents, "Deployment and Configuration Management", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component FfoFrontendTransportF5Threshold = new Component("FfoFrontendTransportF5Threshold", HealthGroup.ServiceComponents, "Deployment and Configuration Management", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component FfoDomainNameServerF5Threshold = new Component("FfoDomainNameServerF5Threshold", HealthGroup.ServiceComponents, "Deployment and Configuration Management", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component StreamingOptics = new Component("StreamingOptics", HealthGroup.ServiceComponents, "Antispam", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component SyslogListener = new Component("SyslogListener", HealthGroup.ServiceComponents, "Network", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component SyslogListenerServiceError = new Component("SyslogListenerServiceError", HealthGroup.ServiceComponents, "Network", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component SyslogListenerServiceParseError = new Component("SyslogListenerServiceParseError", HealthGroup.ServiceComponents, "Network", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component SyslogListener_F5AuditLoginFailure = new Component("SyslogListener_F5AuditLoginFailure", HealthGroup.ServiceComponents, "Network", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component SyslogListener_F5LTMFailover = new Component("SyslogListener_F5LTMFailover", HealthGroup.ServiceComponents, "Network", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component SyslogListener_F5DriveError = new Component("SyslogListener_F5DriveError", HealthGroup.ServiceComponents, "Network", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component SyslogListener_F5GTMWideIPDown = new Component("SyslogListener_F5GTMWideIPDown", HealthGroup.ServiceComponents, "Network", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component SyslogListener_F5GTMVirtualServerDown = new Component("SyslogListener_F5GTMVirtualServerDown", HealthGroup.ServiceComponents, "Network", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component SyslogListener_F5GTMPoolDown = new Component("SyslogListener_F5GTMPoolDown", HealthGroup.ServiceComponents, "Network", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component SyslogListener_F5GTMPoolMemberDown = new Component("SyslogListener_F5GTMPoolMemberDown", HealthGroup.ServiceComponents, "Network", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component SyslogListener_F5LTMPoolDown = new Component("SyslogListener_F5LTMPoolDown", HealthGroup.ServiceComponents, "Network", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component SyslogListener_F5ExcessiveResets = new Component("SyslogListener_F5ExcessiveResets", HealthGroup.ServiceComponents, "Network", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component SyslogListener_F5LTMExcessiveMemberReselects = new Component("SyslogListener_F5LTMExcessiveMemberReselects", HealthGroup.ServiceComponents, "Network", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component SyslogListener_F5Big3dCertErrors = new Component("SyslogListener_F5Big3dCertErrors", HealthGroup.ServiceComponents, "Network", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component SyslogListener_F5PortExhaustion = new Component("SyslogListener_F5PortExhaustion", HealthGroup.ServiceComponents, "Network", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component SyslogListener_F5EmergencyEvent = new Component("SyslogListener_F5EmergencyEvent", HealthGroup.ServiceComponents, "Network", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component SyslogListener_F5NTPError = new Component("SyslogListener_F5NTPError", HealthGroup.ServiceComponents, "Network", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component PowershellDataProvider = new Component("PowershellDataProvider", HealthGroup.ServiceComponents, "PowershellDataProvider", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component AsyncQueueDaemon = new Component("AsyncQueueDaemon", HealthGroup.ServiceComponents, "GLS", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component AsyncQueue = new Component("AsyncQueue", HealthGroup.ServiceComponents, "GLS", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component UnifiedGroups = new Component("UnifiedGroups", HealthGroup.CustomerTouchPoints, "Groups911", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component PublicFolders = new Component("PublicFolders", HealthGroup.CustomerTouchPoints, "Groups911", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component SiteMailbox = new Component("SiteMailbox", HealthGroup.CustomerTouchPoints, "Groups911", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component NotificationsBroker = new Component("NotificationsBroker", HealthGroup.CustomerTouchPoints, "Groups911", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component HubTransport = new Component("HubTransport", HealthGroup.ServiceComponents, "E15Transport", "Exchange", ManagedAvailabilityPriority.High, ServerComponentEnum.HubTransport);

		internal static readonly Component FrontendTransport = new Component("FrontendTransport", HealthGroup.ServiceComponents, "E15Transport", "Exchange", ManagedAvailabilityPriority.Low, ServerComponentEnum.FrontendTransport);

		internal static readonly Component EdgeTransport = new Component("EdgeTransport", HealthGroup.ServiceComponents, "E15Transport", "Exchange", ManagedAvailabilityPriority.High, ServerComponentEnum.EdgeTransport);

		internal static readonly Component MailboxTransport = new Component("MailboxTransport", HealthGroup.ServiceComponents, "E15Transport", "Exchange", ManagedAvailabilityPriority.High);

		internal static readonly Component TransportExtensibility = new Component("TransportExtensibility", HealthGroup.ServiceComponents, "E15Transport", "Exchange", ManagedAvailabilityPriority.High);

		internal static readonly Component Odc = new Component("ODC", HealthGroup.ServiceComponents, "ODC Office 15 Alerts", "ODC", ManagedAvailabilityPriority.Low);

		internal static readonly Component FFOMigration1415 = new Component("FFOMigration1415", HealthGroup.ServiceComponents, "GLS", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component O365SuiteExp = new Component("O365SuiteExp", HealthGroup.ServiceComponents, "OSE on-call", "OSE", ManagedAvailabilityPriority.Low);

		internal static readonly Component CustomerConnection = new Component("CustomerConnection", HealthGroup.ServiceComponents, "CC Service Management on-call", "OSE", ManagedAvailabilityPriority.Low);

		internal static readonly Component ExchangeDatacenterDeployment = new Component("ExDcDeployment", HealthGroup.ServiceComponents, "Deployment", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component FfoDeployment = new Component("FfoDeployment", HealthGroup.ServiceComponents, "Deployment and Configuration Management", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component FfoCentralAdmin = new Component("FfoCentralAdmin", HealthGroup.ServiceComponents, "Deployment and Configuration Management", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component FfoGtmValidation = new Component("FfoGtmValidation", HealthGroup.ServiceComponents, "Network", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component FfoLoadBalancerValidation = new Component("FfoLoadBalancerValidation", HealthGroup.ServiceComponents, "Network", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component FfoFailureStatisticValidation = new Component("FfoFailureStatisticValidation", HealthGroup.ServiceComponents, "Network", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component FfoGatewayConnectivityValidation = new Component("FfoGatewayConnectivityValidation", HealthGroup.ServiceComponents, "Network", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component FfoVipConnectivityValidation = new Component("FfoVipConnectivityValidation", HealthGroup.ServiceComponents, "Network", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component FfoLtmOpticsProvider = new Component("FfoLtmOpticsProvider", HealthGroup.ServiceComponents, "Network", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component FfoGtmOpticsProvider = new Component("FfoGtmOpticsProvider", HealthGroup.ServiceComponents, "Network", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component Psws = new Component("Psws", HealthGroup.CustomerTouchPoints, "Cmdlet Infra/Recipients/RPS", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component XropProxy;

		internal static readonly Component Places = new Component("Places", HealthGroup.CustomerTouchPoints, "Calendaring/Sharing", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component EdiscoveryProtocol = new Component("Ediscovery.Protocol", HealthGroup.ServiceComponents, "EDiscovery and Auditing", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component UnifiedComplianceSourceValidation = new Component("UnifiedComplianceSourceValidation", HealthGroup.ServiceComponents, "EDiscovery and Auditing", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component Auditing = new Component("Auditing", HealthGroup.ServiceComponents, "EDiscovery and Auditing", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component DLExpansion = new Component("DLExpansion", HealthGroup.ServiceComponents, "MRM/Archive", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component JournalArchive = new Component("JournalArchive", HealthGroup.ServiceComponents, "MRM/Archive", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component BPOSDMonitoring = new Component("BPOS-D.Monitoring", HealthGroup.ServiceComponents, "Monitoring", "BPOS-D", ManagedAvailabilityPriority.Low);

		internal static readonly Component E4E = new Component("E4E", HealthGroup.CustomerTouchPoints, "E4E", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component UnifiedPolicy = new Component("UnifiedPolicy", HealthGroup.ServiceComponents, "ETR and DLP", "EOP", ManagedAvailabilityPriority.Low);

		internal static readonly Component DataInsights = new Component("DataInsights", HealthGroup.ServiceComponents, "Data Insights", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component Horus = new Component("Horus", HealthGroup.CustomerTouchPoints, "Groups911", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component AddExternalUserProbe = new Component("AddExternalUserProbe", HealthGroup.CustomerTouchPoints, "spo-crackle", "spo", ManagedAvailabilityPriority.Low);

		internal static readonly Component AppIsolationProbe = new Component("App Isolation Probe", HealthGroup.CustomerTouchPoints, "spo-crackle", "spo", ManagedAvailabilityPriority.Low);

		internal static readonly Component AppManagementServiceGetDeploymentIdProbe = new Component("App Management Service GetDeploymentId Probe", HealthGroup.CustomerTouchPoints, "spo-crackle", "spo", ManagedAvailabilityPriority.Low);

		internal static readonly Component BcsSecureStore = new Component("BcsSecureStore", HealthGroup.CustomerTouchPoints, "spo-crackle", "spo", ManagedAvailabilityPriority.Low);

		internal static readonly Component CDNValidator = new Component("CDNValidator", HealthGroup.CustomerTouchPoints, "spo-crackle", "spo", ManagedAvailabilityPriority.Low);

		internal static readonly Component DAVGET_100MB_WMV = new Component("DAVGET_100MB_WMV", HealthGroup.CustomerTouchPoints, "spo-crackle", "spo", ManagedAvailabilityPriority.Low);

		internal static readonly Component DAVGET_10MB_WMV = new Component("DAVGET_10MB_WMV", HealthGroup.CustomerTouchPoints, "spo-crackle", "spo", ManagedAvailabilityPriority.Low);

		internal static readonly Component DAVGET_1MB_TXT = new Component("DAVGET_1MB_TXT", HealthGroup.CustomerTouchPoints, "spo-crackle", "spo", ManagedAvailabilityPriority.Low);

		internal static readonly Component DAVGET_200KB_TXT = new Component("DAVGET_200KB_TXT", HealthGroup.CustomerTouchPoints, "spo-crackle", "spo", ManagedAvailabilityPriority.Low);

		internal static readonly Component DAVGET_50MB_WMV = new Component("DAVGET_50MB_WMV", HealthGroup.CustomerTouchPoints, "spo-crackle", "spo", ManagedAvailabilityPriority.Low);

		internal static readonly Component DAVGET_SpeedOfLight = new Component("DAVGET_SpeedOfLight", HealthGroup.CustomerTouchPoints, "spo-crackle", "spo", ManagedAvailabilityPriority.Low);

		internal static readonly Component DAVGET_TypicalSmall_XLSX = new Component("DAVGET_TypicalSmall_XLSX", HealthGroup.CustomerTouchPoints, "spo-crackle", "spo", ManagedAvailabilityPriority.Low);

		internal static readonly Component DAVPUT_100MB_WMV = new Component("DAVPUT_100MB_WMV", HealthGroup.CustomerTouchPoints, "spo-crackle", "spo", ManagedAvailabilityPriority.Low);

		internal static readonly Component DAVPUT_10MB_WMV = new Component("DAVPUT_10MB_WMV", HealthGroup.CustomerTouchPoints, "spo-crackle", "spo", ManagedAvailabilityPriority.Low);

		internal static readonly Component DAVPUT_1MB_TXT = new Component("DAVPUT_1MB_TXT", HealthGroup.CustomerTouchPoints, "spo-crackle", "spo", ManagedAvailabilityPriority.Low);

		internal static readonly Component DAVPUT_200KB_TXT = new Component("DAVPUT_200KB_TXT", HealthGroup.CustomerTouchPoints, "spo-crackle", "spo", ManagedAvailabilityPriority.Low);

		internal static readonly Component DAVPUT_50MB_WMV = new Component("DAVPUT_50MB_WMV", HealthGroup.CustomerTouchPoints, "spo-crackle", "spo", ManagedAvailabilityPriority.Low);

		internal static readonly Component DAVPUT_SpeedOfLight = new Component("DAVPUT_SpeedOfLight", HealthGroup.CustomerTouchPoints, "spo-crackle", "spo", ManagedAvailabilityPriority.Low);

		internal static readonly Component DAVPUT_TypicalSmall_XLSX = new Component("DAVPUT_TypicalSmall_XLSX", HealthGroup.CustomerTouchPoints, "spo-crackle", "spo", ManagedAvailabilityPriority.Low);

		internal static readonly Component HomePage = new Component("HomePage", HealthGroup.CustomerTouchPoints, "spo-crackle", "spo", ManagedAvailabilityPriority.Low);

		internal static readonly Component InstallSPOnlyApp = new Component("Install SP Only App", HealthGroup.CustomerTouchPoints, "spo-crackle", "spo", ManagedAvailabilityPriority.Low);

		internal static readonly Component InstallUninstallAnataresApp = new Component("Install Uninstall Anatares App", HealthGroup.CustomerTouchPoints, "spo-crackle", "spo", ManagedAvailabilityPriority.Low);

		internal static readonly Component InstallUninstallSPOnlyApp = new Component("Install Uninstall SP Only App", HealthGroup.CustomerTouchPoints, "spo-crackle", "spo", ManagedAvailabilityPriority.Low);

		internal static readonly Component LaunchAntaresSqlAzureApp = new Component("Launch Antares SqlAzure App", HealthGroup.CustomerTouchPoints, "spo-crackle", "spo", ManagedAvailabilityPriority.Low);

		internal static readonly Component MDSProbe = new Component("MDSProbe", HealthGroup.CustomerTouchPoints, "spo-crackle", "spo", ManagedAvailabilityPriority.Low);

		internal static readonly Component MobilePage = new Component("MobilePage", HealthGroup.CustomerTouchPoints, "spo-crackle", "spo", ManagedAvailabilityPriority.Low);

		internal static readonly Component MySiteHomePage = new Component("My Site Home Page", HealthGroup.CustomerTouchPoints, "spo-crackle", "spo", ManagedAvailabilityPriority.Low);

		internal static readonly Component PingCCT = new Component("Ping CCT", HealthGroup.CustomerTouchPoints, "spo-crackle", "spo", ManagedAvailabilityPriority.Low);

		internal static readonly Component ProjectServerBICenterPage = new Component("Project Server BI Center Page", HealthGroup.CustomerTouchPoints, "spo-crackle", "spo", ManagedAvailabilityPriority.Low);

		internal static readonly Component ProjectServerProjectCenterPage = new Component("Project Server Project Center Page", HealthGroup.CustomerTouchPoints, "spo-crackle", "spo", ManagedAvailabilityPriority.Low);

		internal static readonly Component ProjectServerQueueServiceAvailability = new Component("Project Server Queue Service Availability", HealthGroup.CustomerTouchPoints, "spo-crackle", "spo", ManagedAvailabilityPriority.Low);

		internal static readonly Component ProjectServerWorkflow = new Component("Project Server Workflow", HealthGroup.CustomerTouchPoints, "spo-crackle", "spo", ManagedAvailabilityPriority.Low);

		internal static readonly Component PublicSiteBlog = new Component("PublicSiteBlog", HealthGroup.CustomerTouchPoints, "spo-crackle", "spo", ManagedAvailabilityPriority.Low);

		internal static readonly Component PublicSiteDefaultSettings = new Component("PublicSiteDefaultSettings", HealthGroup.CustomerTouchPoints, "spo-crackle", "spo", ManagedAvailabilityPriority.Low);

		internal static readonly Component PublicSiteHomePage = new Component("PublicSiteHomePage", HealthGroup.CustomerTouchPoints, "spo-crackle", "spo", ManagedAvailabilityPriority.Low);

		internal static readonly Component PublicSiteNavigationTerms = new Component("PublicSiteNavigationTerms", HealthGroup.CustomerTouchPoints, "spo-crackle", "spo", ManagedAvailabilityPriority.Low);

		internal static readonly Component PublicSiteSearch = new Component("PublicSiteSearch", HealthGroup.CustomerTouchPoints, "spo-crackle", "spo", ManagedAvailabilityPriority.Low);

		internal static readonly Component SandboxProbe = new Component("Sandbox Probe", HealthGroup.CustomerTouchPoints, "spo-crackle", "spo", ManagedAvailabilityPriority.Low);

		internal static readonly Component SearchSPO = new Component("SearchSPO", HealthGroup.CustomerTouchPoints, "spo-crackle", "spo", ManagedAvailabilityPriority.Low);

		internal static readonly Component Search14 = new Component("Search14", HealthGroup.CustomerTouchPoints, "spo-crackle", "spo", ManagedAvailabilityPriority.Low);

		internal static readonly Component SearchAnalyticsAnchor = new Component("SearchAnalyticsAnchor", HealthGroup.CustomerTouchPoints, "spo-crackle", "spo", ManagedAvailabilityPriority.Low);

		internal static readonly Component SearchAnalyticsIsIndexed = new Component("SearchAnalyticsIsIndexed", HealthGroup.CustomerTouchPoints, "spo-crackle", "spo", ManagedAvailabilityPriority.Low);

		internal static readonly Component SearchAnalyticsRecycle = new Component("SearchAnalyticsRecycle", HealthGroup.CustomerTouchPoints, "spo-crackle", "spo", ManagedAvailabilityPriority.Low);

		internal static readonly Component SearchAnalyticsSetup = new Component("SearchAnalyticsSetup", HealthGroup.CustomerTouchPoints, "spo-crackle", "spo", ManagedAvailabilityPriority.Low);

		internal static readonly Component SearchAnalyticsView = new Component("SearchAnalyticsView", HealthGroup.CustomerTouchPoints, "spo-crackle", "spo", ManagedAvailabilityPriority.Low);

		internal static readonly Component SearchFreshness = new Component("SearchFreshness", HealthGroup.CustomerTouchPoints, "spo-crackle", "spo", ManagedAvailabilityPriority.Low);

		internal static readonly Component TSAProbe = new Component("TSA Probe", HealthGroup.CustomerTouchPoints, "spo-crackle", "spo", ManagedAvailabilityPriority.Low);

		internal static readonly Component UPAEditProfileProbe = new Component("UPA Edit Profile Probe", HealthGroup.CustomerTouchPoints, "spo-crackle", "spo", ManagedAvailabilityPriority.Low);

		internal static readonly Component UPAGetCommonManagerProbe = new Component("UPA Get Common Manager Probe", HealthGroup.CustomerTouchPoints, "spo-crackle", "spo", ManagedAvailabilityPriority.Low);

		internal static readonly Component UPAGetProfileProbe = new Component("UPA Get Profile Probe", HealthGroup.CustomerTouchPoints, "spo-crackle", "spo", ManagedAvailabilityPriority.Low);

		internal static readonly Component UPAProbe = new Component("UPA Probe", HealthGroup.CustomerTouchPoints, "spo-crackle", "spo", ManagedAvailabilityPriority.Low);

		internal static readonly Component UploadDoc = new Component("UploadDoc", HealthGroup.CustomerTouchPoints, "spo-crackle", "spo", ManagedAvailabilityPriority.Low);

		internal static readonly Component YammerNTaxonomySvcGetDefaultSiteCollectionTermStore = new Component("YammerN Taxonomy Svc GetDefaultSiteCollectionTermStore", HealthGroup.CustomerTouchPoints, "spo-crackle", "spo", ManagedAvailabilityPriority.Low);

		internal static readonly Component YammerNTaxonomySvcGetTermsByLabel = new Component("YammerN Taxonomy Svc GetTermsByLabel", HealthGroup.CustomerTouchPoints, "spo-crackle", "spo", ManagedAvailabilityPriority.Low);

		internal static readonly Component YammerNTaxonomySvcTermStoreManagementPageProbe = new Component("YammerN Taxonomy Svc TermStoreManagementPageProbe", HealthGroup.CustomerTouchPoints, "spo-crackle", "spo", ManagedAvailabilityPriority.Low);

		internal static readonly Component YammerNTaxonomySvcTermStoreTaggingProbe = new Component("YammerN Taxonomy Svc TermStoreTaggingProbe", HealthGroup.CustomerTouchPoints, "spo-crackle", "spo", ManagedAvailabilityPriority.Low);

		internal static readonly Component MrmArchive = new Component("MRMArchive", HealthGroup.CustomerTouchPoints, "MRM/Archive", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component TimeBasedAssistants = new Component("TimeBasedAssistants", HealthGroup.ServiceComponents, "Resource Throttling, Time Based Assistants", "Exchange", ManagedAvailabilityPriority.Normal);

		internal static readonly Component DedicatedIAP = new Component("DedicatedIAP", HealthGroup.ServiceComponents, "Dedicated IAP", "Exchange", ManagedAvailabilityPriority.Low);

		internal static readonly Component OfficeGraph = new Component("OfficeGraph", HealthGroup.ServiceComponents, "OfficeGraph", "Exchange", ManagedAvailabilityPriority.Normal);

		internal static readonly Dictionary<string, Component> WellKnownComponents;
	}
}
