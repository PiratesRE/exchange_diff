using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.HttpProxy.EventLogs;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.HttpProxy
{
	internal sealed class DownLevelServerManager
	{
		private DownLevelServerManager()
		{
		}

		public static bool IsApplicable
		{
			get
			{
				switch (HttpProxyGlobals.ProtocolType)
				{
				case ProtocolType.Eas:
				case ProtocolType.Ecp:
				case ProtocolType.Ews:
				case ProtocolType.Oab:
				case ProtocolType.Owa:
				case ProtocolType.OwaCalendar:
				case ProtocolType.PowerShell:
				case ProtocolType.PowerShellLiveId:
				case ProtocolType.RpcHttp:
				case ProtocolType.Autodiscover:
				case ProtocolType.Xrop:
					return true;
				}
				return false;
			}
		}

		public static DownLevelServerManager Instance
		{
			get
			{
				if (DownLevelServerManager.instance == null)
				{
					lock (DownLevelServerManager.staticLock)
					{
						if (DownLevelServerManager.instance == null)
						{
							DownLevelServerManager.instance = new DownLevelServerManager();
						}
					}
				}
				return DownLevelServerManager.instance;
			}
		}

		public static bool IsServerDiscoverable(string fqdn)
		{
			if (string.IsNullOrEmpty(fqdn))
			{
				throw new ArgumentNullException("fqdn");
			}
			try
			{
				ServiceTopology currentLegacyServiceTopology = ServiceTopology.GetCurrentLegacyServiceTopology("f:\\15.00.1497\\sources\\dev\\cafe\\src\\HttpProxy\\DownLevelServerManager\\DownLevelServerManager.cs", "IsServerDiscoverable", 176);
				currentLegacyServiceTopology.GetSite(fqdn, "f:\\15.00.1497\\sources\\dev\\cafe\\src\\HttpProxy\\DownLevelServerManager\\DownLevelServerManager.cs", "IsServerDiscoverable", 177);
			}
			catch (ServerNotFoundException)
			{
				return false;
			}
			catch (ServerNotInSiteException)
			{
				return false;
			}
			return true;
		}

		public void Initialize()
		{
			if (this.serverMapUpdateTimer != null)
			{
				return;
			}
			lock (this.instanceLock)
			{
				if (this.serverMapUpdateTimer == null)
				{
					ExTraceGlobals.VerboseTracer.TraceDebug((long)this.GetHashCode(), "[DownLevelServerManager::Initialize]: Initializing.");
					this.RefreshServerMap(false);
					this.serverMapUpdateTimer = new Timer(delegate(object o)
					{
						this.RefreshServerMap(true);
					}, null, DownLevelServerManager.DownLevelServerMapRefreshInterval.Value, DownLevelServerManager.DownLevelServerMapRefreshInterval.Value);
				}
			}
		}

		public BackEndServer GetDownLevelClientAccessServerWithPreferredServer<ServiceType>(AnchorMailbox anchorMailbox, string preferredCasServerFqdn, ClientAccessType clientAccessType, RequestDetailsLogger logger, int destinationVersion) where ServiceType : HttpService
		{
			Predicate<ServiceType> predicate = null;
			Predicate<ServiceType> predicate2 = null;
			if (anchorMailbox == null)
			{
				throw new ArgumentNullException("anchorMailbox");
			}
			if (string.IsNullOrEmpty(preferredCasServerFqdn))
			{
				throw new ArgumentException("preferredCasServerFqdn cannot be empty!");
			}
			ServiceTopology currentLegacyServiceTopology = ServiceTopology.GetCurrentLegacyServiceTopology("f:\\15.00.1497\\sources\\dev\\cafe\\src\\HttpProxy\\DownLevelServerManager\\DownLevelServerManager.cs", "GetDownLevelClientAccessServerWithPreferredServer", 253);
			Site site = currentLegacyServiceTopology.GetSite(preferredCasServerFqdn, "f:\\15.00.1497\\sources\\dev\\cafe\\src\\HttpProxy\\DownLevelServerManager\\DownLevelServerManager.cs", "GetDownLevelClientAccessServerWithPreferredServer", 254);
			Dictionary<string, List<DownLevelServerStatusEntry>> downLevelServerMap = this.GetDownLevelServerMap();
			List<DownLevelServerStatusEntry> list = null;
			if (!downLevelServerMap.TryGetValue(site.DistinguishedName, out list))
			{
				string text = string.Format("Unable to find site {0} in the down level server map.", site.DistinguishedName);
				ExTraceGlobals.VerboseTracer.TraceError<string>((long)this.GetHashCode(), "[DownLevelServerManager::GetDownLevelClientAccessServerWithPreferredServer]: {0}", text);
				ThreadPool.QueueUserWorkItem(delegate(object o)
				{
					this.RefreshServerMap(true);
				});
				throw new NoAvailableDownLevelBackEndException(text);
			}
			DownLevelServerStatusEntry downLevelServerStatusEntry = list.Find((DownLevelServerStatusEntry backend) => preferredCasServerFqdn.Equals(backend.BackEndServer.Fqdn, StringComparison.OrdinalIgnoreCase));
			if (downLevelServerStatusEntry == null)
			{
				string text2 = string.Format("Unable to find preferred server {0} in the back end server map.", preferredCasServerFqdn);
				ExTraceGlobals.VerboseTracer.TraceError<string>((long)this.GetHashCode(), "[DownLevelServerManager::GetDownLevelClientAccessServerWithPreferredServer]: {0}", text2);
				throw new NoAvailableDownLevelBackEndException(text2);
			}
			if (downLevelServerStatusEntry.IsHealthy)
			{
				ExTraceGlobals.VerboseTracer.TraceDebug<DownLevelServerStatusEntry>((long)this.GetHashCode(), "[DownLevelServerManager::GetDownLevelClientAccessServerWithPreferredServer]: The preferred server {0} is healthy.", downLevelServerStatusEntry);
				return downLevelServerStatusEntry.BackEndServer;
			}
			ServiceType serviceType = default(ServiceType);
			if (destinationVersion < Server.E14MinVersion)
			{
				try
				{
					List<DownLevelServerStatusEntry> serverList = list;
					ServiceTopology topology = currentLegacyServiceTopology;
					Site targetSite = site;
					if (predicate == null)
					{
						predicate = ((ServiceType service) => service.ServerVersionNumber >= Server.E2007MinVersion && service.ServerVersionNumber < Server.E14MinVersion);
					}
					serviceType = this.GetClientAccessServiceFromList<ServiceType>(serverList, topology, anchorMailbox, targetSite, clientAccessType, predicate, logger, DownLevelServerManager.DownlevelExchangeServerVersion.Exchange2007);
				}
				catch (NoAvailableDownLevelBackEndException)
				{
					ExTraceGlobals.VerboseTracer.TraceError((long)this.GetHashCode(), "[DownLevelServerManager::GetDownLevelClientAccessServerWithPreferredServer]: No E12 CAS could be found for E12 destination. Looking for E14 CAS.");
				}
			}
			if (serviceType == null)
			{
				List<DownLevelServerStatusEntry> serverList2 = list;
				ServiceTopology topology2 = currentLegacyServiceTopology;
				Site targetSite2 = site;
				if (predicate2 == null)
				{
					predicate2 = ((ServiceType service) => service.ServerVersionNumber >= Server.E14MinVersion && service.ServerVersionNumber < Server.E15MinVersion);
				}
				serviceType = this.GetClientAccessServiceFromList<ServiceType>(serverList2, topology2, anchorMailbox, targetSite2, clientAccessType, predicate2, logger, DownLevelServerManager.DownlevelExchangeServerVersion.Exchange2010);
			}
			return new BackEndServer(serviceType.ServerFullyQualifiedDomainName, serviceType.ServerVersionNumber);
		}

		public BackEndServer GetDownLevelClientAccessServer<ServiceType>(AnchorMailbox anchorMailbox, BackEndServer mailboxServer, ClientAccessType clientAccessType, RequestDetailsLogger logger, bool calculateRedirectUrl, out Uri redirectUrl) where ServiceType : HttpService
		{
			if (anchorMailbox == null)
			{
				throw new ArgumentNullException("anchorMailbox");
			}
			if (mailboxServer == null)
			{
				throw new ArgumentNullException("mailboxServer");
			}
			if (logger == null)
			{
				throw new ArgumentNullException("logger");
			}
			if (!DownLevelServerManager.IsApplicable)
			{
				throw new HttpProxyException(HttpStatusCode.NotFound, HttpProxySubErrorCode.EndpointNotFound, string.Format("{0} does not support down level server proxy.", HttpProxyGlobals.ProtocolType));
			}
			redirectUrl = null;
			if (mailboxServer.Version < Server.E14MinVersion)
			{
				ExTraceGlobals.VerboseTracer.TraceDebug<int, int, string>((long)this.GetHashCode(), "[DownLevelServerManager::GetDownLevelClientAccessServer]: Found mailbox server version {0}, which was pre-E14 minimum version {1}, so returning mailbox server FQDN {2}", mailboxServer.Version, Server.E14MinVersion, mailboxServer.Fqdn);
				return mailboxServer;
			}
			ServiceTopology currentLegacyServiceTopology = ServiceTopology.GetCurrentLegacyServiceTopology("f:\\15.00.1497\\sources\\dev\\cafe\\src\\HttpProxy\\DownLevelServerManager\\DownLevelServerManager.cs", "GetDownLevelClientAccessServer", 393);
			Site site = currentLegacyServiceTopology.GetSite(mailboxServer.Fqdn, "f:\\15.00.1497\\sources\\dev\\cafe\\src\\HttpProxy\\DownLevelServerManager\\DownLevelServerManager.cs", "GetDownLevelClientAccessServer", 394);
			ServiceType result = this.GetClientAccessServiceInSite<ServiceType>(currentLegacyServiceTopology, anchorMailbox, site, clientAccessType, (ServiceType service) => service.ServerVersionNumber >= Server.E14MinVersion && service.ServerVersionNumber < Server.E15MinVersion, logger);
			if (calculateRedirectUrl && !Utilities.IsPartnerHostedOnly && !VariantConfiguration.InvariantNoFlightingSnapshot.Cafe.NoCrossSiteRedirect.Enabled && result != null && !string.IsNullOrEmpty(result.ServerFullyQualifiedDomainName))
			{
				Site member = HttpProxyGlobals.LocalSite.Member;
				if (!member.DistinguishedName.Equals(result.Site.DistinguishedName))
				{
					HttpService httpService = currentLegacyServiceTopology.FindAny<ServiceType>(ClientAccessType.External, (ServiceType externalService) => externalService != null && externalService.ServerFullyQualifiedDomainName.Equals(result.ServerFullyQualifiedDomainName, StringComparison.OrdinalIgnoreCase), "f:\\15.00.1497\\sources\\dev\\cafe\\src\\HttpProxy\\DownLevelServerManager\\DownLevelServerManager.cs", "GetDownLevelClientAccessServer", 419);
					if (httpService != null)
					{
						redirectUrl = httpService.Url;
					}
				}
			}
			return new BackEndServer(result.ServerFullyQualifiedDomainName, result.ServerVersionNumber);
		}

		public BackEndServer GetRandomDownLevelClientAccessServer()
		{
			Dictionary<string, List<DownLevelServerStatusEntry>> downLevelServerMap = this.GetDownLevelServerMap();
			string distinguishedName = HttpProxyGlobals.LocalSite.Member.DistinguishedName;
			List<DownLevelServerStatusEntry> serverList = null;
			if (downLevelServerMap.TryGetValue(distinguishedName, out serverList))
			{
				serverList = downLevelServerMap[distinguishedName];
				BackEndServer backEndServer = this.PickRandomServerInSite(serverList);
				if (backEndServer != null)
				{
					return backEndServer;
				}
			}
			for (int i = 0; i < downLevelServerMap.Count; i++)
			{
				if (!(downLevelServerMap.ElementAt(i).Key == distinguishedName))
				{
					serverList = downLevelServerMap.ElementAt(i).Value;
					BackEndServer backEndServer = this.PickRandomServerInSite(serverList);
					if (backEndServer != null)
					{
						return backEndServer;
					}
				}
			}
			string text = string.Format("Unable to find a healthy downlevel server in any site.", new object[0]);
			ExTraceGlobals.VerboseTracer.TraceError<string>((long)this.GetHashCode(), "[DownLevelServerManager::GetRandomDownlevelClientAccessServer]: {0}", text);
			throw new NoAvailableDownLevelBackEndException(text);
		}

		public void Close()
		{
			lock (this.instanceLock)
			{
				if (this.pingManager != null)
				{
					this.pingManager.Dispose();
					this.pingManager = null;
				}
				if (this.serverMapUpdateTimer != null)
				{
					this.serverMapUpdateTimer.Dispose();
					this.serverMapUpdateTimer = null;
				}
			}
		}

		internal static int[] GetShuffledList(int length, int randomNumberSeed)
		{
			Random random = new Random(randomNumberSeed);
			int[] array = new int[length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = i;
			}
			array.Shuffle(random);
			return array;
		}

		internal List<DownLevelServerStatusEntry> GetFilteredServerListByVersion(List<DownLevelServerStatusEntry> serverList, DownLevelServerManager.DownlevelExchangeServerVersion serverVersion)
		{
			if (ExTraceGlobals.VerboseTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.VerboseTracer.TraceDebug<string>((long)this.GetHashCode(), "[DownLevelServerManager::GetFilteredServerListByVersion]: Filtering ServerList by Version: {0}", serverVersion.ToString());
			}
			switch (serverVersion)
			{
			case DownLevelServerManager.DownlevelExchangeServerVersion.Exchange2007:
				return serverList.FindAll((DownLevelServerStatusEntry server) => server.BackEndServer.Version >= Server.E2007MinVersion && server.BackEndServer.Version < Server.E14MinVersion);
			case DownLevelServerManager.DownlevelExchangeServerVersion.Exchange2010:
				return serverList.FindAll((DownLevelServerStatusEntry server) => server.BackEndServer.Version >= Server.E14MinVersion && server.BackEndServer.Version < Server.E15MinVersion);
			default:
				return serverList;
			}
		}

		private Dictionary<string, List<DownLevelServerStatusEntry>> GetDownLevelServerMap()
		{
			return this.downLevelServers;
		}

		private BackEndServer PickRandomServerInSite(List<DownLevelServerStatusEntry> serverList)
		{
			Random random = new Random();
			int num = random.Next(serverList.Count);
			int num2 = num;
			DownLevelServerStatusEntry downLevelServerStatusEntry;
			for (;;)
			{
				downLevelServerStatusEntry = serverList[num2];
				if (downLevelServerStatusEntry.IsHealthy)
				{
					break;
				}
				num2++;
				if (num2 >= serverList.Count)
				{
					num2 = 0;
				}
				if (num2 == num)
				{
					goto Block_3;
				}
			}
			return downLevelServerStatusEntry.BackEndServer;
			Block_3:
			return null;
		}

		private void RefreshServerMap(bool isTimer)
		{
			ExTraceGlobals.VerboseTracer.TraceDebug((long)this.GetHashCode(), "[DownLevelServerManager::RefreshServerMap]: Refreshing server map.");
			Diagnostics.Logger.LogEvent(FrontEndHttpProxyEventLogConstants.Tuple_RefreshingDownLevelServerMap, null, new object[]
			{
				HttpProxyGlobals.ProtocolType.ToString()
			});
			try
			{
				this.InternalRefresh();
			}
			catch (Exception exception)
			{
				if (!isTimer)
				{
					throw;
				}
				Diagnostics.ReportException(exception, FrontEndHttpProxyEventLogConstants.Tuple_InternalServerError, null, "Exception from RefreshServerMap: {0}");
			}
		}

		private ServiceType GetClientAccessServiceInSite<ServiceType>(ServiceTopology topology, AnchorMailbox anchorMailbox, Site targetSite, ClientAccessType clientAccessType, Predicate<ServiceType> otherFilter, RequestDetailsLogger logger) where ServiceType : HttpService
		{
			Dictionary<string, List<DownLevelServerStatusEntry>> downLevelServerMap = this.GetDownLevelServerMap();
			List<DownLevelServerStatusEntry> serverList = null;
			if (!downLevelServerMap.TryGetValue(targetSite.DistinguishedName, out serverList))
			{
				string text = string.Format("Unable to find site {0} in the down level server map.", targetSite.DistinguishedName);
				ExTraceGlobals.VerboseTracer.TraceError<string>((long)this.GetHashCode(), "[DownLevelServerManager::GetClientAccessServiceInSite]: {0}", text);
				ThreadPool.QueueUserWorkItem(delegate(object o)
				{
					this.RefreshServerMap(true);
				});
				throw new NoAvailableDownLevelBackEndException(text);
			}
			return this.GetClientAccessServiceFromList<ServiceType>(serverList, topology, anchorMailbox, targetSite, clientAccessType, otherFilter, logger, DownLevelServerManager.DownlevelExchangeServerVersion.Exchange2010);
		}

		private ServiceType GetClientAccessServiceFromList<ServiceType>(List<DownLevelServerStatusEntry> serverList, ServiceTopology topology, AnchorMailbox anchorMailbox, Site targetSite, ClientAccessType clientAccessType, Predicate<ServiceType> otherFilter, RequestDetailsLogger logger, DownLevelServerManager.DownlevelExchangeServerVersion targetDownlevelExchangeServerVersion) where ServiceType : HttpService
		{
			string text = anchorMailbox.ToCookieKey();
			int hashCode = HttpProxyBackEndHelper.GetHashCode(text);
			serverList = this.GetFilteredServerListByVersion(serverList, targetDownlevelExchangeServerVersion);
			int[] shuffledList = DownLevelServerManager.GetShuffledList(serverList.Count, hashCode);
			if (ExTraceGlobals.VerboseTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.VerboseTracer.TraceDebug<string, int, string>((long)this.GetHashCode(), "[DownLevelServerManager::GetClientAccessServiceFromList]: HashKey: {0}, HashCode: {1}, Anchor mailbox {2}.", text, hashCode, anchorMailbox.ToString());
			}
			for (int i = 0; i < shuffledList.Length; i++)
			{
				int num = shuffledList[i];
				DownLevelServerStatusEntry currentServer = serverList[num];
				if (ExTraceGlobals.VerboseTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.VerboseTracer.TraceDebug<string, int, bool>((long)this.GetHashCode(), "[DownLevelServerManager::GetClientAccessServiceFromList]: Back end server {0} is selected by current index {1}. IsHealthy = {2}", currentServer.BackEndServer.Fqdn, num, currentServer.IsHealthy);
				}
				if (currentServer.IsHealthy)
				{
					ServiceType serviceType = topology.FindAny<ServiceType>(clientAccessType, (ServiceType service) => service != null && service.ServerFullyQualifiedDomainName.Equals(currentServer.BackEndServer.Fqdn, StringComparison.OrdinalIgnoreCase) && !service.IsOutOfService && otherFilter(service), "f:\\15.00.1497\\sources\\dev\\cafe\\src\\HttpProxy\\DownLevelServerManager\\DownLevelServerManager.cs", "GetClientAccessServiceFromList", 767);
					if (serviceType != null)
					{
						ExTraceGlobals.VerboseTracer.TraceDebug<Uri, string>((long)this.GetHashCode(), "[DownLevelServerManager::GetClientAccessServiceFromList]: Found service {0} matching back end server {1}.", serviceType.Url, currentServer.BackEndServer.Fqdn);
						RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(logger, "DownLevelTargetRandomHashing", string.Format("{0}/{1}", i, serverList.Count));
						return serviceType;
					}
					ExTraceGlobals.VerboseTracer.TraceError<string>((long)this.GetHashCode(), "[DownLevelServerManager::GetClientAccessServiceFromList]: Back end server {0} cannot be found by ServiceDiscovery.", currentServer.BackEndServer.Fqdn);
				}
				else
				{
					ExTraceGlobals.VerboseTracer.TraceWarning<string>((long)this.GetHashCode(), "[DownLevelServerManager::GetClientAccessServiceFromList]: Back end server {0} is marked as unhealthy.", currentServer.BackEndServer.Fqdn);
				}
			}
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(logger, "DownLevelTargetRandomHashingFailure", string.Format("{0}", serverList.Count));
			this.TriggerServerMapRefreshIfNeeded(topology, serverList);
			string text2 = string.Format("Unable to find proper back end service for {0} in site {1}.", anchorMailbox, targetSite.DistinguishedName);
			ExTraceGlobals.VerboseTracer.TraceError<string>((long)this.GetHashCode(), "[DownLevelServerManager::GetClientAccessServiceFromList]: {0}", text2);
			throw new NoAvailableDownLevelBackEndException(text2);
		}

		private void TriggerServerMapRefreshIfNeeded(ServiceTopology topology, List<DownLevelServerStatusEntry> serverList)
		{
			bool flag = false;
			if (serverList.Count == 0)
			{
				flag = true;
			}
			foreach (DownLevelServerStatusEntry downLevelServerStatusEntry in serverList)
			{
				if (!DownLevelServerManager.IsServerDiscoverable(downLevelServerStatusEntry.BackEndServer.Fqdn))
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				ThreadPool.QueueUserWorkItem(delegate(object o)
				{
					this.RefreshServerMap(true);
				});
			}
		}

		private void InternalRefresh()
		{
			Exception ex = null;
			Server[] array = null;
			try
			{
				ITopologyConfigurationSession configurationSession = DirectoryHelper.GetConfigurationSession();
				ADPagedReader<Server> adpagedReader = configurationSession.FindPaged<Server>(null, QueryScope.SubTree, DownLevelServerManager.ServerVersionFilter, null, 0);
				array = adpagedReader.ReadAllPages();
			}
			catch (ADTransientException ex2)
			{
				ex = ex2;
			}
			catch (DataValidationException ex3)
			{
				ex = ex3;
			}
			catch (DataSourceOperationException ex4)
			{
				ex = ex4;
			}
			if (ex != null)
			{
				ExTraceGlobals.VerboseTracer.TraceError<Exception>((long)this.GetHashCode(), "[DownLevelServerManager::RefreshServerMap]: Active Directory exception: {0}", ex);
				Diagnostics.Logger.LogEvent(FrontEndHttpProxyEventLogConstants.Tuple_ErrorRefreshDownLevelServerMap, null, new object[]
				{
					HttpProxyGlobals.ProtocolType.ToString(),
					ex.ToString()
				});
				return;
			}
			Dictionary<string, List<DownLevelServerStatusEntry>> downLevelServerMap = this.GetDownLevelServerMap();
			Dictionary<string, List<DownLevelServerStatusEntry>> dictionary = new Dictionary<string, List<DownLevelServerStatusEntry>>(downLevelServerMap.Count, StringComparer.OrdinalIgnoreCase);
			Server[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				Server server = array2[i];
				if ((server.CurrentServerRole & ServerRole.ClientAccess) > ServerRole.None && server.ServerSite != null)
				{
					List<DownLevelServerStatusEntry> list = null;
					if (!dictionary.TryGetValue(server.ServerSite.DistinguishedName, out list))
					{
						list = new List<DownLevelServerStatusEntry>();
						dictionary.Add(server.ServerSite.DistinguishedName, list);
					}
					DownLevelServerStatusEntry downLevelServerStatusEntry = null;
					List<DownLevelServerStatusEntry> list2 = null;
					if (downLevelServerMap.TryGetValue(server.ServerSite.DistinguishedName, out list2))
					{
						downLevelServerStatusEntry = list2.Find((DownLevelServerStatusEntry x) => x.BackEndServer.Fqdn.Equals(server.Fqdn, StringComparison.OrdinalIgnoreCase));
					}
					if (downLevelServerStatusEntry == null)
					{
						downLevelServerStatusEntry = new DownLevelServerStatusEntry
						{
							BackEndServer = new BackEndServer(server.Fqdn, server.VersionNumber),
							IsHealthy = true
						};
					}
					list.Add(downLevelServerStatusEntry);
					list.Sort((DownLevelServerStatusEntry x, DownLevelServerStatusEntry y) => x.BackEndServer.Fqdn.CompareTo(y.BackEndServer.Fqdn));
				}
			}
			this.downLevelServers = dictionary;
			if (dictionary.Count > 0 && DownLevelServerManager.DownLevelServerPingEnabled.Value && this.pingManager == null)
			{
				this.pingManager = new DownLevelServerPingManager(new Func<Dictionary<string, List<DownLevelServerStatusEntry>>>(this.GetDownLevelServerMap));
			}
		}

		private static readonly QueryFilter ServerVersionFilter = new ComparisonFilter(ComparisonOperator.LessThan, ServerSchema.VersionNumber, Server.E15MinVersion);

		private static readonly TimeSpanAppSettingsEntry DownLevelServerMapRefreshInterval = new TimeSpanAppSettingsEntry(HttpProxySettings.Prefix("DownLevelServerMapRefreshInterval"), TimeSpanUnit.Minutes, TimeSpan.FromMinutes(360.0), ExTraceGlobals.VerboseTracer);

		private static readonly BoolAppSettingsEntry DownLevelServerPingEnabled = new BoolAppSettingsEntry(HttpProxySettings.Prefix("DownLevelServerPingEnabled"), VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Cafe.DownLevelServerPing.Enabled, ExTraceGlobals.VerboseTracer);

		private static DownLevelServerManager instance = null;

		private static object staticLock = new object();

		private object instanceLock = new object();

		private DownLevelServerPingManager pingManager;

		private Dictionary<string, List<DownLevelServerStatusEntry>> downLevelServers = new Dictionary<string, List<DownLevelServerStatusEntry>>(StringComparer.OrdinalIgnoreCase);

		private Timer serverMapUpdateTimer;

		internal enum DownlevelExchangeServerVersion
		{
			Exchange2007,
			Exchange2010
		}
	}
}
