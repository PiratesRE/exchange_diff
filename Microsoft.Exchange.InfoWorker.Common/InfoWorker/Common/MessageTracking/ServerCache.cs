using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Transport.Logging.Search;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Win32;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal sealed class ServerCache
	{
		private static bool CheckWriteToStatsLog()
		{
			int defaultValue = VariantConfiguration.InvariantNoFlightingSnapshot.MessageTracking.StatsLogging.Enabled ? 1 : 0;
			int num = ServerCache.TryReadRegistryKey<int>("WriteToStatsLog", defaultValue);
			return num == 1;
		}

		private string GetServerNameForMailboxDatabase(ADObjectId mailboxDatabaseId)
		{
			return this.mailboxDatabaseConfigCache.Get(mailboxDatabaseId);
		}

		public static ServerCache Instance
		{
			get
			{
				return ServerCache.instance;
			}
		}

		public HostId HostId { get; private set; }

		public static KeyType TryReadRegistryKey<KeyType>(string value, KeyType defaultValue)
		{
			Exception ex = null;
			try
			{
				object value2 = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Transport\\DeliveryReports", value, defaultValue);
				if (value2 == null || !(value2 is KeyType))
				{
					return defaultValue;
				}
				return (KeyType)((object)value2);
			}
			catch (SecurityException ex2)
			{
				ex = ex2;
			}
			catch (IOException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				TraceWrapper.SearchLibraryTracer.TraceError<string, string, Exception>(0, "Failed to read registry key: {0}\\{1}, {2}", "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Transport\\DeliveryReports", value, ex);
			}
			return defaultValue;
		}

		public bool DiagnosticsDisabled
		{
			get
			{
				return (this.diagnosticsDisabledRegistryValue & 1) != 0;
			}
		}

		public bool ReportNonFatalBugs
		{
			get
			{
				return this.reportNonFatalBugs == 1;
			}
		}

		public bool UseE14RtmEwsSchema
		{
			get
			{
				return this.useE14RtmEwsSchema == 1;
			}
		}

		public int MaxRecipientsInReferrals
		{
			get
			{
				return this.maxRecipientsInReferrals;
			}
		}

		public int MaxTrackingEventBudgetForSingleQuery
		{
			get
			{
				return this.maxTrackingEventBudgetForSingleQuery;
			}
		}

		public int MaxTrackingEventBudgetForAllQueries
		{
			get
			{
				return this.maxTrackingEventBudgetForAllQueries;
			}
		}

		public int MaxDiagnosticsEvents
		{
			get
			{
				return this.maxDiagnosticsEvents;
			}
		}

		public int IWTimeoutSeconds
		{
			get
			{
				if (this.iwTimeoutSeconds > 0)
				{
					return this.iwTimeoutSeconds;
				}
				return 30;
			}
		}

		public int HelpdeskTimeoutSeconds
		{
			get
			{
				if (this.helpdeskTimeoutSeconds > 0)
				{
					return this.helpdeskTimeoutSeconds;
				}
				return 210;
			}
		}

		public TimeSpan ExpectedLoggingLatency
		{
			get
			{
				return this.expectedLoggingLatency;
			}
		}

		public bool IsTimeoutOverrideConfigured
		{
			get
			{
				return this.iwTimeoutSeconds > 0 && this.helpdeskTimeoutSeconds > 0;
			}
		}

		public int NumberOfThreadsAllowed
		{
			get
			{
				return this.numberOfThreadsAllowed;
			}
		}

		public int RowsBeforeTimeBudgetCheck
		{
			get
			{
				return this.rowsBeforeTimeBudgetCheck;
			}
		}

		public bool WriteToStatsLogs
		{
			get
			{
				return this.writeToStatsLog;
			}
		}

		public bool InitializeIfNeeded(HostId identity)
		{
			if (this.HostId == HostId.NotInitialized)
			{
				lock (this.initLock)
				{
					if (this.HostId == HostId.NotInitialized)
					{
						CommonDiagnosticsLog.Initialize(identity);
						ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), OrganizationId.ForestWideOrgId, null, false);
						ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, sessionSettings, 500, "InitializeIfNeeded", "f:\\15.00.1497\\sources\\dev\\infoworker\\src\\common\\MessageTracking\\ServerCache.cs");
						Server server = topologyConfigurationSession.FindLocalServer();
						if (server == null || string.IsNullOrEmpty(server.Fqdn) || string.IsNullOrEmpty(server.Domain))
						{
							TraceWrapper.SearchLibraryTracer.TraceError<string, string>(this.GetHashCode(), "Failed to get local server, or it is invalid Fqdn={0}, Domain={1}", (server == null) ? "<null>" : server.Fqdn, (server == null) ? "<null>" : server.Domain);
							return false;
						}
						this.localServer = server;
						ADSite localSite = topologyConfigurationSession.GetLocalSite();
						if (localSite == null)
						{
							TraceWrapper.SearchLibraryTracer.TraceError(this.GetHashCode(), "Failed to get local site.", new object[0]);
							return false;
						}
						this.localServerSiteId = localSite.Id;
						this.HostId = identity;
					}
				}
				return true;
			}
			return true;
		}

		public ADObjectId GetLocalServerSiteId(DirectoryContext directoryContext)
		{
			return this.localServerSiteId;
		}

		public Server GetLocalServer()
		{
			return this.localServer;
		}

		public ServerInfo GetUserServer(ADUser trackingUser)
		{
			ADObjectId database = trackingUser.Database;
			if (database == null)
			{
				TraceWrapper.SearchLibraryTracer.TraceError<ADObjectId>(this.GetHashCode(), "Null DataBase ID for user: {0}", trackingUser.Id);
				TrackingFatalException.RaiseED(ErrorCode.InvalidADData, "Null Database attribute for user {0}", new object[]
				{
					trackingUser.Id
				});
			}
			string serverNameForMailboxDatabase = this.GetServerNameForMailboxDatabase(database);
			return this.FindMailboxOrHubServer(serverNameForMailboxDatabase, 34UL);
		}

		public List<ServerInfo> GetDagServers(ServerInfo server)
		{
			List<ServerInfo> list = new List<ServerInfo>(8);
			ADObjectId databaseAvailabilityGroup = server.DatabaseAvailabilityGroup;
			bool flag = false;
			if (databaseAvailabilityGroup != null)
			{
				IList<string> list2 = this.databaseAvailabilityGroupCache.Get(databaseAvailabilityGroup);
				foreach (string text in list2)
				{
					if (string.Equals(text, server.Key, StringComparison.OrdinalIgnoreCase))
					{
						flag = true;
					}
					ServerInfo item = this.transportServerConfigCache.Get(text);
					list.Add(item);
				}
			}
			if (!flag)
			{
				if (databaseAvailabilityGroup == null)
				{
					TraceWrapper.SearchLibraryTracer.TraceDebug<string>(this.GetHashCode(), "Server {0} does not have a DAG specified", server.Key);
					list.Add(server);
				}
				else
				{
					TraceWrapper.SearchLibraryTracer.TraceError<string, ADObjectId>(this.GetHashCode(), "Server {0} was not present in DAG {1}", server.Key, databaseAvailabilityGroup);
					TrackingFatalException.RaiseED(ErrorCode.InvalidADData, "Server {0} not in DatabaseAvailabilityGroup {1}", new object[]
					{
						server.Key,
						databaseAvailabilityGroup
					});
				}
			}
			return list;
		}

		public bool ReadStatusReportingEnabled(DirectoryContext directoryContext)
		{
			OrganizationConfigCache.Item item = this.organizationConfigCache.Get(directoryContext.OrganizationId);
			bool readTrackingEnabled = item.ReadTrackingEnabled;
			TraceWrapper.SearchLibraryTracer.TraceDebug<bool>(this.GetHashCode(), "Read Tracking Enabled = {0}", readTrackingEnabled);
			return readTrackingEnabled;
		}

		public Uri GetCasServerUri(ADObjectId site, out int serverVersion)
		{
			return ServerCache.Instance.GetCasServerUri(site, Globals.E14Version, out serverVersion);
		}

		public Uri GetCasServerUri(ADObjectId site, int minServerVersionRequested, out int serverVersion)
		{
			SortedDictionary<int, List<WebServicesService>> uriVersions = null;
			serverVersion = 0;
			try
			{
				ServiceTopology currentServiceTopology = ServiceTopology.GetCurrentServiceTopology("f:\\15.00.1497\\sources\\dev\\infoworker\\src\\common\\MessageTracking\\ServerCache.cs", "GetCasServerUri", 673);
				currentServiceTopology.ForEach<WebServicesService>(delegate(WebServicesService service)
				{
					if (service.ServerVersionNumber >= minServerVersionRequested && service.Site.Id.Equals(site) && service.ClientAccessType == ClientAccessType.InternalNLBBypass)
					{
						if (uriVersions == null)
						{
							uriVersions = new SortedDictionary<int, List<WebServicesService>>();
						}
						int key2 = service.ServerVersionNumber >> 16;
						if (!uriVersions.ContainsKey(key2))
						{
							uriVersions[key2] = new List<WebServicesService>();
						}
						uriVersions[key2].Add(service);
					}
				}, "f:\\15.00.1497\\sources\\dev\\infoworker\\src\\common\\MessageTracking\\ServerCache.cs", "GetCasServerUri", 674);
			}
			catch (ServiceDiscoveryTransientException ex)
			{
				TraceWrapper.SearchLibraryTracer.TraceError<ServiceDiscoveryTransientException>(this.GetHashCode(), "Transient exception getting Internal CAS URI: {0}", ex);
				TrackingTransientException.RaiseETX(ErrorCode.CASUriDiscoveryFailure, site.ToString(), ex.ToString());
			}
			catch (ServiceDiscoveryPermanentException ex2)
			{
				TraceWrapper.SearchLibraryTracer.TraceError<ServiceDiscoveryPermanentException>(this.GetHashCode(), "Permanent exception getting Internal CAS URI: {0}", ex2);
				TrackingFatalException.RaiseETX(ErrorCode.CASUriDiscoveryFailure, site.ToString(), ex2.ToString());
			}
			if (uriVersions != null && uriVersions.Count > 0)
			{
				int key = uriVersions.Last<KeyValuePair<int, List<WebServicesService>>>().Key;
				List<WebServicesService> value = uriVersions.Last<KeyValuePair<int, List<WebServicesService>>>().Value;
				int index = ServerCache.rand.Next(value.Count);
				WebServicesService webServicesService = value.ElementAt(index);
				TraceWrapper.SearchLibraryTracer.TraceDebug<Uri, string>(this.GetHashCode(), "Using CAS URI: {0}, Version {1}", webServicesService.Url, new ServerVersion(webServicesService.ServerVersionNumber).ToString());
				serverVersion = webServicesService.ServerVersionNumber;
				return webServicesService.Url;
			}
			TraceWrapper.SearchLibraryTracer.TraceError<string, string>(this.GetHashCode(), "Failed to find any CAS server in site: {0}, with min version {1}", site.ToString(), new ServerVersion(minServerVersionRequested).ToString());
			return null;
		}

		public List<ServerInfo> GetCasServers(ADObjectId site)
		{
			SiteConfigCache.Item item = this.siteConfigCache.Get(site);
			if (item == null || item.CasServerInfos.Count == 0)
			{
				return null;
			}
			return item.CasServerInfos;
		}

		public ServerInfo GetHubServer(string name)
		{
			return this.transportServerConfigCache.Get(name);
		}

		public int GetHubServersInSite(ADObjectId site, out List<string> hubServerFqdns, out HashSet<string> hubServerFqdnTable)
		{
			hubServerFqdns = null;
			hubServerFqdnTable = null;
			SiteConfigCache.Item item = this.siteConfigCache.Get(site);
			if (item == null || item.HubServerFqdns.Count == 0)
			{
				return 0;
			}
			hubServerFqdns = item.HubServerFqdns;
			hubServerFqdnTable = item.HubServerTable;
			return hubServerFqdns.Count;
		}

		public string GetDefaultDomain(OrganizationId organizationId)
		{
			OrganizationConfigCache.Item item = this.organizationConfigCache.Get(organizationId);
			if (item == null || string.IsNullOrEmpty(item.DefaultDomain))
			{
				TraceWrapper.SearchLibraryTracer.TraceError(this.GetHashCode(), "Cannot get domain from org-id cache", new object[0]);
				TrackingFatalException.RaiseED(ErrorCode.InvalidADData, "No default domain found for Organization {0}", new object[]
				{
					organizationId
				});
			}
			return item.DefaultDomain;
		}

		public bool IsDomainAuthoritativeForOrganization(OrganizationId organizationId, string domain)
		{
			OrganizationConfigCache.Item item = this.organizationConfigCache.Get(organizationId);
			return item != null && item.AuthoritativeDomains != null && item.AuthoritativeDomains.Count != 0 && item.AuthoritativeDomains.Contains(domain);
		}

		public bool IsDomainInternalRelayForOrganization(OrganizationId organizationId, string domain)
		{
			OrganizationConfigCache.Item item = this.organizationConfigCache.Get(organizationId);
			return item != null && item.InternalRelayDomains != null && item.InternalRelayDomains.Count != 0 && item.InternalRelayDomains.Contains(domain);
		}

		public bool TryGetOrganizationId(string domain, out OrganizationId organizationId)
		{
			return this.organizationConfigCache.TryGetOrganizationId(domain, out organizationId);
		}

		public ServerInfo FindMailboxOrHubServer(string serverNameOrFqdn, ulong serverRoleMask)
		{
			return this.transportServerConfigCache.FindServer(serverNameOrFqdn, serverRoleMask);
		}

		public bool IsRemoteTrustedOrg(OrganizationId organizationId, string domain)
		{
			TraceWrapper.SearchLibraryTracer.TraceDebug<OrganizationId, string>(this.GetHashCode(), "Looking for organization relationship for Org: {0} and domain: {1}", organizationId, domain);
			OrganizationRelationship organizationRelationship = this.TryGetOrganizationRelationship(organizationId, domain);
			if (organizationRelationship == null)
			{
				TraceWrapper.SearchLibraryTracer.TraceDebug(this.GetHashCode(), "Organization relationship not found", new object[0]);
				return false;
			}
			TraceWrapper.SearchLibraryTracer.TraceDebug(this.GetHashCode(), "Organization relationship found", new object[0]);
			if (!organizationRelationship.Enabled)
			{
				TraceWrapper.SearchLibraryTracer.TraceDebug(this.GetHashCode(), "Org relationship disabled,", new object[0]);
				return false;
			}
			TraceWrapper.SearchLibraryTracer.TraceDebug(this.GetHashCode(), "Organization relationship is enabled", new object[0]);
			if (!organizationRelationship.DeliveryReportEnabled)
			{
				TraceWrapper.SearchLibraryTracer.TraceDebug(this.GetHashCode(), "Delivery Report disabled for relationship.", new object[0]);
				return false;
			}
			return true;
		}

		public IEnumerable<ADObjectId> GetAllSitesInOrg(ITopologyConfigurationSession session)
		{
			if (ExDateTime.UtcNow > this.allInOrgSiteIdsLastUpdated + ServerCache.allSiteIdsInOrgCacheInterval)
			{
				bool flag = ServerCache.Instance.WriteToStatsLogs && ServerCache.Instance.HostId == HostId.ECPApplicationPool;
				if (flag)
				{
					KeyValuePair<string, object>[] eventData = new KeyValuePair<string, object>[]
					{
						new KeyValuePair<string, object>("Event", "AllSitesRefreshStart")
					};
					CommonDiagnosticsLog.Instance.LogEvent(CommonDiagnosticsLog.Source.DeliveryReportsCache, eventData);
				}
				lock (this.initLock)
				{
					if (ExDateTime.UtcNow > this.allInOrgSiteIdsLastUpdated + ServerCache.allSiteIdsInOrgCacheInterval)
					{
						TraceWrapper.SearchLibraryTracer.TraceDebug<ExDateTime>(this.GetHashCode(), "Updating all sites id cache. It was last updated at {0}.", this.allInOrgSiteIdsLastUpdated);
						List<ADObjectId> list = new List<ADObjectId>();
						foreach (ADSite adsite in session.FindAllPaged<ADSite>())
						{
							list.Add(adsite.Id);
						}
						this.allInOrgSiteIds = list;
						this.allInOrgSiteIdsLastUpdated = ExDateTime.UtcNow;
					}
				}
				if (flag)
				{
					KeyValuePair<string, object>[] eventData = new KeyValuePair<string, object>[]
					{
						new KeyValuePair<string, object>("Event", "AllSitesRefreshDone")
					};
					CommonDiagnosticsLog.Instance.LogEvent(CommonDiagnosticsLog.Source.DeliveryReportsCache, eventData);
				}
			}
			return this.allInOrgSiteIds;
		}

		public SmtpAddress GetOrgMailboxForDomain(string domain)
		{
			return this.domainOrgMailboxCache.Get(domain);
		}

		private OrganizationRelationship TryGetOrganizationRelationship(OrganizationId orgId, string targetDomain)
		{
			OrganizationIdCacheValue organizationIdCacheValue = OrganizationIdCache.Singleton.Get(orgId);
			if (organizationIdCacheValue == null)
			{
				TraceWrapper.SearchLibraryTracer.TraceError(this.GetHashCode(), "Null cache value returned from OrganizationIdCacheValue", new object[0]);
				TrackingFatalException.RaiseED(ErrorCode.InvalidADData, "Organization Relationships could not be read for organization {0}", new object[]
				{
					orgId
				});
			}
			return organizationIdCacheValue.GetOrganizationRelationship(targetDomain);
		}

		private const string DeliveryReportsRegkey = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Transport\\DeliveryReports";

		internal const string CacheKey = "Key";

		internal const string CacheEvent = "Event";

		internal const string CacheType = "Type";

		internal const string CacheReason = "Reason";

		internal const string Cached = "Cached";

		private static readonly Random rand = new Random();

		private static readonly ServerCache instance = new ServerCache();

		private static readonly TimeSpan allSiteIdsInOrgCacheInterval = TimeSpan.FromHours(5.0);

		private object initLock = new object();

		private ADObjectId localServerSiteId;

		private Server localServer;

		private OrganizationConfigCache organizationConfigCache = new OrganizationConfigCache(VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled);

		private SiteConfigCache siteConfigCache = new SiteConfigCache();

		private TransportServerConfigCache transportServerConfigCache = new TransportServerConfigCache();

		private MailboxDatabaseConfigCache mailboxDatabaseConfigCache = new MailboxDatabaseConfigCache();

		private DatabaseAvailabilityGroupCache databaseAvailabilityGroupCache = new DatabaseAvailabilityGroupCache();

		private DomainOrgMailboxCache domainOrgMailboxCache = new DomainOrgMailboxCache();

		private int diagnosticsDisabledRegistryValue = ServerCache.TryReadRegistryKey<int>("DiagnosticsDisabled", 0);

		private int reportNonFatalBugs = ServerCache.TryReadRegistryKey<int>("ReportNonFatalBugs", 0);

		private int useE14RtmEwsSchema = ServerCache.TryReadRegistryKey<int>("UseE14RtmEwsSchema", 0);

		private int maxRecipientsInReferrals = ServerCache.TryReadRegistryKey<int>("MaximumRecipientsInReferrals", 10000);

		private int maxTrackingEventBudgetForSingleQuery = ServerCache.TryReadRegistryKey<int>("MaximumTrackingEventBudgetForSingleQuery", 1000000);

		private int maxTrackingEventBudgetForAllQueries = ServerCache.TryReadRegistryKey<int>("MaximumTrackingEventBudgetForAllQueries", 2000000);

		private int maxDiagnosticsEvents = ServerCache.TryReadRegistryKey<int>("MaximumDiagnosticsEvents", 512);

		private int iwTimeoutSeconds = ServerCache.TryReadRegistryKey<int>("IWTimeoutSeconds", 0);

		private int helpdeskTimeoutSeconds = ServerCache.TryReadRegistryKey<int>("HelpdeskTimeoutSeconds", 0);

		private int numberOfThreadsAllowed = ServerCache.TryReadRegistryKey<int>("NumberOfThreadsAllowed", 128);

		private int rowsBeforeTimeBudgetCheck = ServerCache.TryReadRegistryKey<int>("RowsBeforeTimeBudgetCheck", 1024);

		private TimeSpan expectedLoggingLatency = TimeSpan.FromSeconds((double)ServerCache.TryReadRegistryKey<int>("ExpectedLoggingLatencySeconds", 300));

		private bool writeToStatsLog = ServerCache.CheckWriteToStatsLog();

		private List<ADObjectId> allInOrgSiteIds;

		private ExDateTime allInOrgSiteIdsLastUpdated = ExDateTime.MinValue;

		[Flags]
		private enum DeliveryReportsRegkeyFlags
		{
			DisableDiagnosticsGlobally = 1
		}
	}
}
