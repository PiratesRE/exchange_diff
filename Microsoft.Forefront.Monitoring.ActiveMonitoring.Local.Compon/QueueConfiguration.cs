using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.Transport;
using Microsoft.Exchange.Data.QueueViewer;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Management.QueueDigest;
using Microsoft.Exchange.Net.DiagnosticsAggregation;
using Microsoft.Exchange.Transport.Categorizer;
using Microsoft.Exchange.Transport.DiagnosticsAggregationService;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	internal class QueueConfiguration
	{
		public QueueConfiguration()
		{
		}

		public QueueConfiguration(XmlNode queueNode)
		{
			this.Name = Utils.GetOptionalXmlAttribute<string>(queueNode, "Name", string.Empty);
			this.QueueType = Utils.GetMandatoryXmlEnumAttribute<QueueType>(queueNode, "QueueType");
			this.MessageCountThreshold = Utils.GetOptionalXmlAttribute<int>(queueNode, "MessageCountThreshold", 0);
			this.DeferredMessageCountThreshold = Utils.GetOptionalXmlAttribute<int>(queueNode, "DeferredMessageCountThreshold", 0);
			this.LockedMessageCountThreshold = Utils.GetOptionalXmlAttribute<int>(queueNode, "LockedMessageCountThreshold", 0);
			if (this.QueueType == QueueType.WellKnownDestination)
			{
				this.Destination = Utils.GetMandatoryXmlAttribute<string>(queueNode, "Destination");
				this.AggregatedBy = QueueDigestGroupBy.NextHopDomain;
				if (this.DeferredMessageCountThreshold == 0 && this.LockedMessageCountThreshold == 0 && this.MessageCountThreshold == 0)
				{
					throw new ConfigurationErrorsException("WellKnownDestination: missing valid thresholds for processing queue stats.");
				}
			}
			else
			{
				this.Destination = Utils.GetOptionalXmlAttribute<string>(queueNode, "Destination", null);
				this.AggregatedBy = Utils.GetOptionalXmlEnumAttribute<QueueDigestGroupBy>(queueNode, "AggregatedBy", QueueDigestGroupBy.NextHopDomain);
				this.MessageCountThreshold = Utils.GetOptionalXmlAttribute<int>(queueNode, "MessageCountThreshold", 0);
				this.AverageMessageCountThreshold = Utils.GetOptionalXmlAttribute<int>(queueNode, "AverageMessageCountThreshold", 0);
				this.TotalMessageCountThreshold = Utils.GetOptionalXmlAttribute<int>(queueNode, "TotalMessageCountThreshold", 0);
				this.ExceedsAverageByPercent = Utils.GetOptionalXmlAttribute<int>(queueNode, "ExceedsAverageByPercent", 0);
				this.ExceedsAverageByNumber = Utils.GetOptionalXmlAttribute<int>(queueNode, "ExceedsAverageByNumber", 0);
				this.ExceedsLowestByPercent = Utils.GetOptionalXmlAttribute<int>(queueNode, "ExceedsLowestByPercent", 0);
				this.ExceedsLowestByNumber = Utils.GetOptionalXmlAttribute<int>(queueNode, "ExceedsLowestByNumber", 0);
			}
			this.DeliveryType = Utils.GetOptionalXmlEnumAttribute<DeliveryType>(queueNode, "DeliveryType", DeliveryType.Undefined);
			this.EventNotificationComponent = Utils.GetMandatoryXmlAttribute<string>(queueNode, "EventNotificationComponent");
			this.EventNotificationServiceName = Utils.GetMandatoryXmlAttribute<string>(queueNode, "EventNotificationServiceName");
			this.DetailsLevel = DetailsLevel.Verbose;
			this.EventNotificationTag = Utils.GetOptionalXmlAttribute<string>(queueNode, "EventNotificationTag", string.Empty);
			this.EventNotificationSeverityLevel = Utils.GetOptionalXmlEnumAttribute<ResultSeverityLevel>(queueNode, "EventNotificationSeverityLevel", ResultSeverityLevel.Critical);
			this.ResultSize = Utils.GetOptionalXmlAttribute<uint>(queueNode, "ResultSize", uint.MaxValue);
			this.MessageCountGreaterThan = Utils.GetOptionalXmlAttribute<int>(queueNode, "DeferredMessageCountLessThan", 0);
			this.RaiseWarningOnNoStats = Utils.GetOptionalXmlAttribute<bool>(queueNode, "RaiseWarningOnNoStats", false);
			this.EventNotificationTagForNoStats = Utils.GetOptionalXmlAttribute<string>(queueNode, "EventNotificationTagForNoStats", null);
			this.FreshnessCutoffTime = Utils.GetOptionalXmlAttribute<TimeSpan>(queueNode, "FreshnessCutoffTime", TimeSpan.Parse("02:00:00"));
			this.Dag = Utils.GetOptionalXmlAttribute<string>(queueNode, "Dag", null);
			this.Server = Utils.GetOptionalXmlAttribute<string>(queueNode, "Server", null);
			if (this.Server != null && this.Server.ToLowerInvariant().Contains("localhost"))
			{
				this.Server = this.Server.Replace("localhost", Environment.MachineName);
			}
			this.Site = Utils.GetOptionalXmlAttribute<string>(queueNode, "Site", null);
			string optionalXmlAttribute = Utils.GetOptionalXmlAttribute<string>(queueNode, "RiskLevel", string.Empty);
			RiskLevel riskLevel;
			this.RiskLevel = (Enum.TryParse<RiskLevel>(optionalXmlAttribute, out riskLevel) ? riskLevel.ToString() : string.Empty);
			optionalXmlAttribute = Utils.GetOptionalXmlAttribute<string>(queueNode, "QueueStatus", string.Empty);
			QueueStatus queueStatus;
			this.QueueStatus = (Enum.TryParse<QueueStatus>(optionalXmlAttribute, out queueStatus) ? queueStatus.ToString() : string.Empty);
			this.ShouldExemptPoisonQueues = Utils.GetOptionalXmlAttribute<bool>(queueNode, "ExemptPoisonQueues", true);
			this.Session = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 186, ".ctor", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\QueueDigest\\Probes\\QueueConfiguration.cs");
			List<int> list;
			if (this.QueueType == QueueType.WellKnownDestination)
			{
				list = new List<int>
				{
					this.MessageCountThreshold,
					this.DeferredMessageCountThreshold,
					this.LockedMessageCountThreshold
				};
			}
			else
			{
				list = new List<int>
				{
					this.MessageCountThreshold,
					this.DeferredMessageCountThreshold,
					this.LockedMessageCountThreshold,
					this.AverageMessageCountThreshold,
					this.TotalMessageCountThreshold,
					this.ExceedsAverageByNumber,
					this.ExceedsAverageByPercent,
					this.ExceedsLowestByNumber,
					this.ExceedsLowestByPercent
				};
			}
			int num = 0;
			using (List<int>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current == 0)
					{
						num++;
					}
				}
			}
			if (num == list.Count)
			{
				throw new ConfigurationErrorsException(string.Format("You must specify a threshold for processing queue stats: {0}", this.ToString()));
			}
			foreach (int num2 in list)
			{
				if (num2 < 0)
				{
					throw new ConfigurationErrorsException(string.Format("{0} must be a non-negative number.", num2));
				}
			}
		}

		internal string Name { get; set; }

		internal QueueType QueueType { get; set; }

		internal QueueDigestGroupBy AggregatedBy { get; set; }

		internal string Destination { get; set; }

		internal DeliveryType DeliveryType { get; set; }

		internal int MessageCountThreshold { get; set; }

		internal string Server { get; set; }

		internal string Site { get; set; }

		internal string Dag { get; set; }

		internal string QueueStatus { get; set; }

		internal string RiskLevel { get; set; }

		internal uint ResultSize { get; set; }

		internal TimeSpan FreshnessCutoffTime { get; set; }

		internal DetailsLevel DetailsLevel { get; set; }

		internal bool ShouldExemptPoisonQueues { get; set; }

		internal string EventNotificationServiceName { get; set; }

		internal string EventNotificationComponent { get; set; }

		internal string EventNotificationTag { get; set; }

		internal ResultSeverityLevel EventNotificationSeverityLevel { get; set; }

		internal bool RaiseWarningOnNoStats { get; set; }

		internal string EventNotificationTagForNoStats { get; set; }

		internal int AverageMessageCountThreshold { get; set; }

		internal int TotalMessageCountThreshold { get; set; }

		internal int ExceedsAverageByPercent { get; set; }

		internal int ExceedsAverageByNumber { get; set; }

		internal int ExceedsLowestByPercent { get; set; }

		internal int ExceedsLowestByNumber { get; set; }

		internal int DeferredMessageCountThreshold { get; set; }

		internal int LockedMessageCountThreshold { get; set; }

		internal int MessageCountGreaterThan { get; set; }

		internal ITopologyConfigurationSession Session { get; set; }

		internal Guid ForestId { get; set; }

		internal string ForestName { get; set; }

		internal MultiValuedProperty<Guid> ServerIdList { get; set; }

		internal MultiValuedProperty<Guid> DagIdList { get; set; }

		internal MultiValuedProperty<Guid> SiteIdList { get; set; }

		internal MultiValuedProperty<ComparisonFilter> DataFilter { get; set; }

		internal string Filter { get; set; }

		internal uint ServerSideResultSize { get; set; }

		internal QueueAggregator Aggregator { get; set; }

		internal int WebServiceRequestsPending { get; set; }

		internal AutoResetEvent WebServiceRequestsDone { get; set; }

		internal Binding WebServiceBinding { get; set; }

		internal Dictionary<GroupOfServersKey, List<ADObjectId>> DagToServersMap
		{
			get
			{
				return this.dagToServersMap;
			}
		}

		internal HashSet<ADObjectId> ServersNotBelongingToAnyDag
		{
			get
			{
				return this.serversNotBelongingToAnyDag;
			}
		}

		internal List<string> FailedToConnectServers
		{
			get
			{
				return this.failedToConnectServers;
			}
		}

		internal List<string> FailedToConnectDags
		{
			get
			{
				return this.failedToConnectDags;
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("Name={0}, ", this.Name);
			stringBuilder.AppendFormat("DeferredMessageCountThreshold={0}, ", this.DeferredMessageCountThreshold);
			stringBuilder.AppendFormat("Destination={0}, ", this.Destination);
			stringBuilder.AppendFormat("Dag={0}, ", string.Join(",", new string[]
			{
				this.Dag
			}));
			stringBuilder.AppendFormat("EventNotificationComponent={0}, ", this.EventNotificationComponent);
			stringBuilder.AppendFormat("EventNotificationServiceName={0}, ", this.EventNotificationServiceName);
			stringBuilder.AppendFormat("EventNotificationSeverityLevel={0}, ", this.EventNotificationSeverityLevel.ToString());
			stringBuilder.AppendFormat("EventNotificationTag={0} ", this.EventNotificationTag);
			stringBuilder.AppendFormat("Filter={0} ", this.Filter);
			stringBuilder.AppendFormat("ForestId={0} ", this.ForestId);
			stringBuilder.AppendFormat("ForestName={0} ", this.ForestName);
			stringBuilder.AppendFormat("FreshnessCutoffTime={0} ", this.FreshnessCutoffTime);
			stringBuilder.AppendFormat("LockedMessageCountThreshold={0}, ", this.LockedMessageCountThreshold);
			stringBuilder.AppendFormat("MessageCountThreshold={0}, ", this.MessageCountThreshold);
			stringBuilder.AppendFormat("QueueStatus={0}, ", this.QueueStatus);
			stringBuilder.AppendFormat("QueueType={0}, ", this.QueueType.ToString());
			stringBuilder.AppendFormat("RiskLevel={0}, ", this.RiskLevel);
			stringBuilder.AppendFormat("Server={0}, ", string.Join(",", new string[]
			{
				this.Server
			}));
			stringBuilder.AppendFormat("Site={0}, ", string.Join(",", new string[]
			{
				this.Site
			}));
			if (this.QueueType == QueueType.Aggregated)
			{
				stringBuilder.AppendFormat("AverageMessageCountThreshold={0}, ", this.AverageMessageCountThreshold);
				stringBuilder.AppendFormat("ExceedsAverageByNumber={0} ", this.ExceedsAverageByNumber);
				stringBuilder.AppendFormat("ExceedsAverageByPercent={0} ", this.ExceedsAverageByPercent);
				stringBuilder.AppendFormat("ExceedsLowestByNumber={0} ", this.ExceedsLowestByNumber);
				stringBuilder.AppendFormat("ExceedsLowestByPercent={0} ", this.ExceedsLowestByPercent);
				stringBuilder.AppendFormat("MessageCountGreaterThan={0}, ", this.MessageCountGreaterThan);
				stringBuilder.AppendFormat("TotalMessageCountThreshold={0}, ", this.TotalMessageCountThreshold);
				stringBuilder.AppendFormat("ExemptPoisonQueues={0}, ", this.ShouldExemptPoisonQueues);
			}
			return stringBuilder.ToString();
		}

		internal static List<QueueConfiguration> GetRemoteDomains(ITopologyConfigurationSession session)
		{
			List<QueueConfiguration> list = new List<QueueConfiguration>();
			if (QueueConfiguration.lastADQueryTime == DateTime.MaxValue || DateTime.UtcNow - QueueConfiguration.lastADQueryTime >= TimeSpan.FromMinutes(QueueConfiguration.ADQueryIntervalMins))
			{
				QueueConfiguration.cachedWellKnownDestinations.Clear();
				QueueConfiguration.lastADQueryTime = DateTime.UtcNow;
				ADPagedReader<DomainContentConfig> remoteDomains = null;
				ADNotificationAdapter.TryRunADOperation(delegate()
				{
					remoteDomains = session.FindAllPaged<DomainContentConfig>();
				});
				if (remoteDomains == null)
				{
					return null;
				}
				using (IEnumerator<DomainContentConfig> enumerator = remoteDomains.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						DomainContentConfig domainContentConfig = enumerator.Current;
						if (domainContentConfig.MessageCountThreshold != 2147483647)
						{
							QueueConfiguration queueConfiguration = new QueueConfiguration();
							queueConfiguration.Name = domainContentConfig.Name;
							queueConfiguration.Destination = domainContentConfig.DomainName.Domain;
							queueConfiguration.MessageCountThreshold = domainContentConfig.MessageCountThreshold;
							queueConfiguration.QueueType = QueueType.WellKnownDestination;
							queueConfiguration.AggregatedBy = QueueDigestGroupBy.NextHopDomain;
							queueConfiguration.DeliveryType = DeliveryType.Undefined;
							queueConfiguration.EventNotificationServiceName = "Transport";
							queueConfiguration.EventNotificationComponent = "WellKnownDestinationMessageCountExceedsThreshold";
							queueConfiguration.DetailsLevel = DetailsLevel.Verbose;
							queueConfiguration.EventNotificationSeverityLevel = ResultSeverityLevel.Critical;
							queueConfiguration.ResultSize = uint.MaxValue;
							queueConfiguration.MessageCountGreaterThan = 0;
							queueConfiguration.RaiseWarningOnNoStats = false;
							queueConfiguration.FreshnessCutoffTime = TimeSpan.Parse("02:00:00");
							queueConfiguration.Session = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 583, "GetRemoteDomains", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\QueueDigest\\Probes\\QueueConfiguration.cs");
							if (QueueConfiguration.cachedWellKnownDestinations.TryAdd(domainContentConfig.Guid, queueConfiguration))
							{
								queueConfiguration.ResolveParameters();
								list.Add(queueConfiguration);
							}
						}
					}
					return list;
				}
			}
			foreach (KeyValuePair<Guid, QueueConfiguration> keyValuePair in QueueConfiguration.cachedWellKnownDestinations)
			{
				list.Add(keyValuePair.Value);
			}
			return list;
		}

		internal void ResolveParameters()
		{
			Guid forestId;
			string forestName;
			TransportADUtils.GetForestInformation(out forestId, out forestName);
			this.ForestId = forestId;
			this.ForestName = forestName;
			MultiValuedProperty<Guid> serverIdList;
			MultiValuedProperty<Guid> dagIdList;
			MultiValuedProperty<Guid> siteIdList;
			this.GetTargetResourceGuids(out serverIdList, out dagIdList, out siteIdList);
			this.ServerIdList = serverIdList;
			this.DagIdList = dagIdList;
			this.SiteIdList = siteIdList;
			this.DataFilter = this.GetDataFilter();
			this.Filter = this.GetFilters();
		}

		internal void GetServersToConnectPreferingServersSpecified(KeyValuePair<GroupOfServersKey, List<ADObjectId>> serversInDag, out List<ADObjectId> serversToConnect, out HashSet<ADObjectId> serversToInclude)
		{
			serversToConnect = serversInDag.Value;
			serversToInclude = null;
			if (this.serversToIncludeInDag.ContainsKey(serversInDag.Key))
			{
				serversToInclude = this.serversToIncludeInDag[serversInDag.Key];
				serversToConnect = new List<ADObjectId>(this.serversToIncludeInDag[serversInDag.Key]);
				foreach (ADObjectId item in serversInDag.Value)
				{
					if (!serversToInclude.Contains(item))
					{
						serversToConnect.Add(item);
					}
				}
			}
		}

		private MultiValuedProperty<ComparisonFilter> GetDataFilter()
		{
			MultiValuedProperty<ComparisonFilter> multiValuedProperty = new MultiValuedProperty<ComparisonFilter>();
			if (!string.IsNullOrWhiteSpace(this.Destination))
			{
				multiValuedProperty.Add(new ComparisonFilter(ComparisonOperator.Equal, TransportQueueQuerySchema.NextHopDomainQueryProperty, this.Destination));
			}
			if (!string.IsNullOrWhiteSpace(this.QueueStatus))
			{
				multiValuedProperty.Add(new ComparisonFilter(ComparisonOperator.Equal, TransportQueueQuerySchema.StatusQueryProperty, this.QueueStatus));
			}
			if (!string.IsNullOrWhiteSpace(this.RiskLevel))
			{
				multiValuedProperty.Add(new ComparisonFilter(ComparisonOperator.Equal, TransportQueueQuerySchema.RiskLevelQueryProperty, this.RiskLevel));
			}
			if (this.DeliveryType != DeliveryType.Undefined)
			{
				multiValuedProperty.Add(new ComparisonFilter(ComparisonOperator.Equal, TransportQueueQuerySchema.DeliveryTypeQueryProperty, this.DeliveryType));
			}
			if (this.MessageCountGreaterThan > 0)
			{
				multiValuedProperty.Add(new ComparisonFilter(ComparisonOperator.GreaterThan, TransportQueueQuerySchema.MessageCountQueryProperty, this.MessageCountGreaterThan));
			}
			return multiValuedProperty;
		}

		private string GetFilters()
		{
			List<string> list = new List<string>();
			if (!string.IsNullOrWhiteSpace(this.Destination))
			{
				list.Add(string.Format("NextHopDomain -eq '{0}'", this.Destination));
			}
			if (!string.IsNullOrWhiteSpace(this.QueueStatus))
			{
				list.Add(string.Format("Status -eq '{0}'", this.QueueStatus));
			}
			if (!string.IsNullOrWhiteSpace(this.RiskLevel))
			{
				list.Add(string.Format("RiskLevel -eq '{0}'", this.RiskLevel));
			}
			if (this.DeliveryType != DeliveryType.Undefined)
			{
				list.Add(string.Format("DeliveryType -eq '{0}'", this.DeliveryType.ToString()));
			}
			if (this.MessageCountGreaterThan > 0)
			{
				list.Add(string.Format("MessageCount -gt '{0}'", this.MessageCountGreaterThan));
			}
			if (this.ShouldExemptPoisonQueues)
			{
				list.Add(string.Format("NextHopDomain -ne '{0}'", "Poison Message"));
			}
			return string.Join(" -and ", list.ToArray());
		}

		private void GetTargetResourceGuids(out MultiValuedProperty<Guid> serverIds, out MultiValuedProperty<Guid> dagIds, out MultiValuedProperty<Guid> siteIds)
		{
			serverIds = null;
			dagIds = null;
			siteIds = null;
			if (!string.IsNullOrWhiteSpace(this.Server))
			{
				serverIds = this.GetServerIds();
			}
			else if (!string.IsNullOrWhiteSpace(this.Dag))
			{
				dagIds = this.GetDagIds();
			}
			else if (!string.IsNullOrWhiteSpace(this.Site))
			{
				siteIds = this.GetSiteIds();
			}
			else
			{
				this.ResolveForForest();
			}
			this.ServerSideResultSize = (((this.resolvedServers.Count == 1 || this.resolvedDags.Count == 1) && this.ResultSize != uint.MaxValue) ? this.ResultSize : uint.MaxValue);
		}

		private MultiValuedProperty<Guid> GetServerIds()
		{
			List<string> list = string.IsNullOrWhiteSpace(this.Server) ? new List<string>() : new List<string>(this.Server.Split(new char[]
			{
				','
			}));
			foreach (string text in list)
			{
				Guid guid;
				QueryFilter filter;
				if (Guid.TryParse(text, out guid))
				{
					filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Guid, guid);
				}
				else
				{
					filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, text);
				}
				Server[] array = this.Session.Find<Server>(null, QueryScope.SubTree, filter, null, 0);
				if (array == null || array.Length <= 0)
				{
					throw new ConfigurationErrorsException(string.Format("Failed to resolve server {0}", text));
				}
				Server server = array[0];
				this.AddServer(server);
				this.resolvedServers.Add(server.Id);
			}
			return this.GetObjectGuids(this.resolvedServers);
		}

		private MultiValuedProperty<Guid> GetDagIds()
		{
			List<string> list = string.IsNullOrWhiteSpace(this.Dag) ? new List<string>() : new List<string>(this.Dag.Split(new char[]
			{
				','
			}));
			foreach (string text in list)
			{
				Guid guid;
				QueryFilter filter;
				if (Guid.TryParse(text, out guid))
				{
					filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Guid, guid);
				}
				else
				{
					filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, text);
				}
				DatabaseAvailabilityGroup[] array = this.Session.Find<DatabaseAvailabilityGroup>(null, QueryScope.SubTree, filter, null, 0);
				if (array == null || array.Length <= 0)
				{
					throw new ConfigurationErrorsException(string.Format("Failed to resolve dag {0}", text));
				}
				DatabaseAvailabilityGroup databaseAvailabilityGroup = array[0];
				ADPagedReader<Server> adpagedReader = this.Session.FindPaged<Server>(null, QueryScope.SubTree, new AndFilter(new QueryFilter[]
				{
					new BitMaskAndFilter(ServerSchema.CurrentServerRole, 32UL),
					new ComparisonFilter(ComparisonOperator.Equal, ServerSchema.DatabaseAvailabilityGroup, databaseAvailabilityGroup.Id),
					DiagnosticsAggregationHelper.IsE15OrHigherQueryFilter
				}), null, 0);
				foreach (Server server in adpagedReader)
				{
					this.AddServer(server);
				}
				this.resolvedDags.Add(databaseAvailabilityGroup.Id);
			}
			return this.GetObjectGuids(this.resolvedDags);
		}

		private MultiValuedProperty<Guid> GetSiteIds()
		{
			List<string> list = string.IsNullOrWhiteSpace(this.Site) ? new List<string>() : new List<string>(this.Site.Split(new char[]
			{
				','
			}));
			foreach (string text in list)
			{
				Guid guid;
				QueryFilter filter;
				if (Guid.TryParse(text, out guid))
				{
					filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Guid, guid);
				}
				else
				{
					filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, text);
				}
				ADSite[] array = this.Session.Find<ADSite>(null, QueryScope.SubTree, filter, null, 0);
				if (array == null || array.Length <= 0)
				{
					throw new ConfigurationErrorsException(string.Format("Failed to resolve site {0}", text));
				}
				ADSite adsite = array[0];
				ADPagedReader<Server> adpagedReader = this.Session.FindPaged<Server>(null, QueryScope.SubTree, new AndFilter(new QueryFilter[]
				{
					new BitMaskAndFilter(ServerSchema.CurrentServerRole, 32UL),
					new ComparisonFilter(ComparisonOperator.Equal, ServerSchema.ServerSite, adsite.Id),
					DiagnosticsAggregationHelper.IsE15OrHigherQueryFilter
				}), null, 0);
				foreach (Server server in adpagedReader)
				{
					this.AddServer(server);
				}
				this.resolvedAdSites.Add(adsite.Id);
			}
			return this.GetObjectGuids(this.resolvedAdSites);
		}

		private void ResolveForForest()
		{
			ADPagedReader<Server> adpagedReader = this.Session.FindPaged<Server>(null, QueryScope.SubTree, new AndFilter(new QueryFilter[]
			{
				new BitMaskAndFilter(ServerSchema.CurrentServerRole, 32UL),
				DiagnosticsAggregationHelper.IsE15OrHigherQueryFilter
			}), null, 0);
			foreach (Server server in adpagedReader)
			{
				this.AddServer(server);
			}
		}

		private MultiValuedProperty<Guid> GetObjectGuids(HashSet<ADObjectId> objectIds)
		{
			MultiValuedProperty<Guid> multiValuedProperty = new MultiValuedProperty<Guid>();
			foreach (ADObjectId adobjectId in objectIds)
			{
				multiValuedProperty.Add(adobjectId.ObjectGuid);
			}
			return multiValuedProperty;
		}

		private void AddServer(Server server)
		{
			if (server.DatabaseAvailabilityGroup != null || server.ServerSite != null)
			{
				GroupOfServersKey key = (server.DatabaseAvailabilityGroup != null) ? GroupOfServersKey.CreateFromDag(server.DatabaseAvailabilityGroup) : GroupOfServersKey.CreateFromSite(server.ServerSite, server.MajorVersion);
				if (!this.dagToServersMap.ContainsKey(key))
				{
					HashSet<ADObjectId> groupForServer = DiagnosticsAggregationHelper.GetGroupForServer(server, this.Session);
					List<ADObjectId> list = new List<ADObjectId>(groupForServer);
					RoutingUtils.ShuffleList<ADObjectId>(list);
					this.dagToServersMap[key] = list;
				}
				if (!this.serversToIncludeInDag.ContainsKey(key))
				{
					this.serversToIncludeInDag[key] = new HashSet<ADObjectId>();
				}
				this.serversToIncludeInDag[key].Add(server.Id);
				return;
			}
			this.serversNotBelongingToAnyDag.Add(server.Id);
		}

		internal const string PoisonMessageQueueId = "Poison Message";

		private static readonly double ADQueryIntervalMins = 60.0;

		private static DateTime lastADQueryTime = DateTime.MaxValue;

		private static ConcurrentDictionary<Guid, QueueConfiguration> cachedWellKnownDestinations = new ConcurrentDictionary<Guid, QueueConfiguration>();

		private Dictionary<GroupOfServersKey, List<ADObjectId>> dagToServersMap = new Dictionary<GroupOfServersKey, List<ADObjectId>>();

		private Dictionary<GroupOfServersKey, HashSet<ADObjectId>> serversToIncludeInDag = new Dictionary<GroupOfServersKey, HashSet<ADObjectId>>();

		private HashSet<ADObjectId> serversNotBelongingToAnyDag = new HashSet<ADObjectId>();

		private HashSet<ADObjectId> resolvedAdSites = new HashSet<ADObjectId>();

		private HashSet<ADObjectId> resolvedDags = new HashSet<ADObjectId>();

		private HashSet<ADObjectId> resolvedServers = new HashSet<ADObjectId>();

		private List<string> failedToConnectServers = new List<string>();

		private List<string> failedToConnectDags = new List<string>();
	}
}
