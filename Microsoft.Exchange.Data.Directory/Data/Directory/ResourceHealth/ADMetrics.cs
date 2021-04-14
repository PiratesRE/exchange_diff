using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.EventLog;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.TopologyDiscovery;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Directory.ResourceHealth
{
	internal sealed class ADMetrics
	{
		internal ADMetrics(string forestFqdn)
		{
			this.forestFqdn = forestFqdn;
		}

		public string ForestFqdn
		{
			get
			{
				return this.forestFqdn;
			}
		}

		public static ADMetrics GetMetricsForForest(ADMetrics previousMetrics, string forestFqdn)
		{
			ADMetrics admetrics = new ADMetrics(forestFqdn);
			TopologyProvider instance = TopologyProvider.GetInstance();
			if (previousMetrics != null && previousMetrics.serversList != null && ExDateTime.UtcNow - previousMetrics.lastTopologyUpdateTime < ADMetrics.topologyRediscoveryInterval && (instance == null || instance.GetTopologyVersion(forestFqdn) == previousMetrics.topologyVersion))
			{
				ExTraceGlobals.ResourceHealthManagerTracer.TraceDebug(0L, "[ADMetrics::GetMetrics] Using existing topology for forest " + forestFqdn);
				admetrics.lastTopologyUpdateTime = previousMetrics.lastTopologyUpdateTime;
				admetrics.serversList = previousMetrics.serversList;
				admetrics.rediscoverTopology = false;
				admetrics.topologyVersion = previousMetrics.topologyVersion;
			}
			else
			{
				ExTraceGlobals.ResourceHealthManagerTracer.TraceDebug(0L, "[ADMetrics::GetMetrics] Rediscovering topology.");
				admetrics.rediscoverTopology = true;
			}
			if (admetrics.Populate(previousMetrics))
			{
				return admetrics;
			}
			Globals.LogEvent(DirectoryEventLogConstants.Tuple_ADHealthFailed, null, new object[0]);
			return null;
		}

		internal Dictionary<string, ADServerMetrics> GetTopNDomainControllers(int n)
		{
			Dictionary<string, ADServerMetrics> dictionary = new Dictionary<string, ADServerMetrics>();
			if (n > 0)
			{
				IOrderedEnumerable<KeyValuePair<string, ADServerMetrics>> source = from pair in this.allADServerMetrics
				orderby pair.Value.IncomingDebt descending
				select pair;
				IEnumerable<KeyValuePair<string, ADServerMetrics>> enumerable = source.Take(n);
				foreach (KeyValuePair<string, ADServerMetrics> keyValuePair in enumerable)
				{
					dictionary.Add(keyValuePair.Key, keyValuePair.Value);
				}
			}
			return dictionary;
		}

		internal void AddServerMetrics(ADServerMetrics metrics)
		{
			if (metrics == null)
			{
				throw new ArgumentNullException("metrics");
			}
			if (metrics.DnsHostName == null)
			{
				throw new ArgumentNullException("metrics.DnsHostName");
			}
			this.allADServerMetrics.Add(metrics.DnsHostName, metrics);
		}

		public ICollection<ADServerMetrics> AllServerMetrics
		{
			get
			{
				if (this.allADServerMetrics != null)
				{
					return this.allADServerMetrics.Values;
				}
				return null;
			}
		}

		public ADServerMetrics this[string dnsHostName]
		{
			get
			{
				ADServerMetrics result;
				try
				{
					result = this.allADServerMetrics[dnsHostName];
				}
				catch (KeyNotFoundException)
				{
					result = null;
				}
				return result;
			}
		}

		public ExDateTime UpdateTime
		{
			get
			{
				if (this.updateTime == null && this.AllServerMetrics != null)
				{
					if (this.AllServerMetrics.Any((ADServerMetrics i) => i.IsSuitable))
					{
						this.updateTime = new ExDateTime?((from i in this.AllServerMetrics
						where i.IsSuitable
						select i).Min((ADServerMetrics i) => i.UpdateTime));
					}
					else
					{
						this.updateTime = new ExDateTime?(ExDateTime.MinValue);
					}
				}
				ExDateTime? exDateTime = this.updateTime;
				if (exDateTime == null)
				{
					return ExDateTime.MinValue;
				}
				return exDateTime.GetValueOrDefault();
			}
		}

		public int IncomingHealth { get; set; }

		public string MinIncomingHealthServer { get; set; }

		public int OutgoingHealth { get; set; }

		public string MinOutgoingHealthServer { get; set; }

		private bool Populate(ADMetrics previousMetrics)
		{
			try
			{
				if (this.rediscoverTopology)
				{
					this.PopulateTopologyVersion();
					this.PopulateADServersList();
				}
				foreach (ADServer dc in this.serversList)
				{
					this.AddServerMetrics(new ADServerMetrics(dc));
				}
			}
			catch (ADTransientException arg)
			{
				ExTraceGlobals.ResourceHealthManagerTracer.TraceError<ADTransientException>((long)this.GetHashCode(), "[ADMetrics::Populate] Failed to get read a list of DC: {0}", arg);
				return false;
			}
			catch (ADOperationException arg2)
			{
				ExTraceGlobals.ResourceHealthManagerTracer.TraceError<ADOperationException>((long)this.GetHashCode(), "[ADMetrics::Populate] Failed to get read a list of DC: {0}", arg2);
				return false;
			}
			List<ADServerMetrics> list = new List<ADServerMetrics>();
			foreach (ADServerMetrics adserverMetrics in this.AllServerMetrics)
			{
				ADServerMetrics adserverMetrics2 = (previousMetrics != null) ? previousMetrics[adserverMetrics.DnsHostName] : null;
				if (this.rediscoverTopology || adserverMetrics2 == null || adserverMetrics2.IsSuitable)
				{
					list.Add(adserverMetrics);
				}
			}
			if (list.Count <= 0)
			{
				goto IL_1C3;
			}
			using (ActivityContext.SuppressThreadScope())
			{
				CancellationTokenSource cts = new CancellationTokenSource();
				using (new Timer(delegate(object _)
				{
					cts.Cancel();
				}, null, 120000, -1))
				{
					try
					{
						Parallel.ForEach<ADServerMetrics>(list, new ParallelOptions
						{
							CancellationToken = cts.Token
						}, delegate(ADServerMetrics item)
						{
							try
							{
								Interlocked.Increment(ref this.pooledDiscoveryCount);
								this.PopulateSingleServerMetrics(item);
							}
							finally
							{
								Interlocked.Decrement(ref this.pooledDiscoveryCount);
							}
						});
					}
					catch (OperationCanceledException arg3)
					{
						ExTraceGlobals.ResourceHealthManagerTracer.TraceError<OperationCanceledException>((long)this.GetHashCode(), "[ADMetrics::PopulateSingleServerMetrics] Timed out trying to read AD metrics from DCs: {0}", arg3);
					}
				}
				goto IL_1C3;
			}
			IL_1B9:
			Thread.Sleep(500);
			IL_1C3:
			if (this.pooledDiscoveryCount <= 0)
			{
				return this.AllServerMetrics.Any((ADServerMetrics server) => server.IsSuitable);
			}
			goto IL_1B9;
		}

		private void PopulateTopologyVersion()
		{
			TopologyProvider instance = TopologyProvider.GetInstance();
			if (instance != null)
			{
				this.topologyVersion = instance.GetTopologyVersion(this.forestFqdn);
			}
		}

		private void PopulateADServersList()
		{
			List<ADServer> list = new List<ADServer>();
			foreach (ADDomain addomain in ADForest.GetForest(new PartitionId(this.forestFqdn)).FindDomains())
			{
				foreach (ADServer adserver in addomain.FindAllDomainControllers(true))
				{
					if (adserver.DnsHostName != null)
					{
						list.Add(adserver);
					}
				}
			}
			this.lastTopologyUpdateTime = ExDateTime.UtcNow;
			this.serversList = list;
		}

		private void PopulateSingleServerMetrics(ADServerMetrics dcMetrics)
		{
			Exception ex = null;
			try
			{
				ExTraceGlobals.ResourceHealthManagerTracer.TraceDebug<string>((long)this.GetHashCode(), "[ADMetrics::PopulateSingleServerMetrics] Reading metrics for {0}.", dcMetrics.DnsHostName);
				string text;
				LocalizedString localizedString;
				if (!SuitabilityVerifier.IsServerSuitableIgnoreExceptions(dcMetrics.DnsHostName, false, null, out text, out localizedString))
				{
					ExTraceGlobals.ResourceHealthManagerTracer.TraceError<string, LocalizedString>((long)this.GetHashCode(), "[ADMetrics::PopulateSingleServerMetrics] Suitability checks failed for {0}: {1}", dcMetrics.DnsHostName, localizedString);
					dcMetrics.ErrorMessage = localizedString;
					return;
				}
				ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(dcMetrics.DnsHostName, true, ConsistencyMode.PartiallyConsistent, null, ADSessionSettings.FromAccountPartitionRootOrgScopeSet(new PartitionId(this.forestFqdn)), 470, "PopulateSingleServerMetrics", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\throttling\\ResourceHealth\\ADMetrics.cs");
				topologyConfigurationSession.UseGlobalCatalog = false;
				ExDateTime utcNow = ExDateTime.UtcNow;
				RootDse rootDse = topologyConfigurationSession.GetRootDse();
				dcMetrics.IsSynchronized = rootDse.IsSynchronized;
				if (!dcMetrics.IsSynchronized)
				{
					ExTraceGlobals.ResourceHealthManagerTracer.TraceError<string>((long)this.GetHashCode(), "[ADMetrics::PopulateSingleServerMetrics] {0} is not synchronized yet.", dcMetrics.DnsHostName);
					return;
				}
				MultiValuedProperty<ReplicationCursor> source;
				MultiValuedProperty<ReplicationNeighbor> neighbors;
				topologyConfigurationSession.ReadReplicationData(topologyConfigurationSession.ConfigurationNamingContext, out source, out neighbors);
				IEnumerable<ReplicationCursor> replicationCursors = from cursor in source
				where neighbors.Any((ReplicationNeighbor neighbor) => this.NotNullAndEquals(neighbor.SourceDsa, cursor.SourceDsa))
				select cursor;
				ICollection<ADReplicationLinkMetrics> configReplicationMetrics = this.ReadReplicationMetrics(replicationCursors);
				topologyConfigurationSession.UseConfigNC = false;
				topologyConfigurationSession.ReadReplicationData(dcMetrics.ServerId.DomainId, out source, out neighbors);
				replicationCursors = from cursor in source
				where neighbors.Any((ReplicationNeighbor neighbor) => this.NotNullAndEquals(neighbor.SourceDsa, cursor.SourceDsa))
				select cursor;
				ICollection<ADReplicationLinkMetrics> domainReplicationMetrics = this.ReadReplicationMetrics(replicationCursors);
				dcMetrics.Initialize(utcNow, rootDse.HighestCommittedUSN, configReplicationMetrics, domainReplicationMetrics);
				dcMetrics.IsSuitable = true;
				ExTraceGlobals.ResourceHealthManagerTracer.TraceDebug<string>((long)this.GetHashCode(), "[ADMetrics::PopulateSingleServerMetrics] Finished reading metrics for {0}.", dcMetrics.DnsHostName);
			}
			catch (ADTransientException ex2)
			{
				ex = ex2;
			}
			catch (ADOperationException ex3)
			{
				ex = ex3;
			}
			catch (OperationCanceledException ex4)
			{
				ex = ex4;
			}
			if (ex != null)
			{
				ExTraceGlobals.ResourceHealthManagerTracer.TraceError<string, Exception>((long)this.GetHashCode(), "[ADMetrics::PopulateSingleServerMetrics] Failed to get read AD metrics from {0}: {1}", dcMetrics.DnsHostName, ex);
				dcMetrics.ErrorMessage = new LocalizedString(ex.Message);
			}
		}

		private bool NotNullAndEquals(ADObjectId id1, ADObjectId id2)
		{
			return id1 != null && id2 != null && id1.DistinguishedName == id2.DistinguishedName;
		}

		private ICollection<ADReplicationLinkMetrics> ReadReplicationMetrics(IEnumerable<ReplicationCursor> replicationCursors)
		{
			List<ADReplicationLinkMetrics> list = new List<ADReplicationLinkMetrics>(2);
			foreach (ReplicationCursor replicationCursor in replicationCursors)
			{
				if (replicationCursor.SourceDsa != null)
				{
					using (IEnumerator<ADServerMetrics> enumerator2 = this.AllServerMetrics.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							ADServerMetrics adserverMetrics = enumerator2.Current;
							if (adserverMetrics.IsSuitable && string.Equals(replicationCursor.SourceDsa.Parent.Name, adserverMetrics.ServerId.Name, StringComparison.OrdinalIgnoreCase))
							{
								list.Add(new ADReplicationLinkMetrics(adserverMetrics.DnsHostName, replicationCursor.UpToDatenessUsn));
								break;
							}
						}
						continue;
					}
				}
				ExTraceGlobals.ResourceHealthManagerTracer.TraceDebug<Guid>((long)this.GetHashCode(), "[ADMetrics::ReadReplicationMetrics] Replication cursor with SourceInvocationId={0} does not have SourceDsa.", replicationCursor.SourceInvocationId);
			}
			return list;
		}

		public string GetReport()
		{
			StringBuilder stringBuilder = new StringBuilder();
			using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder))
			{
				xmlWriter.WriteStartElement("ADMetrics");
				xmlWriter.WriteAttributeString("InHealth", this.IncomingHealth.ToString());
				if (!string.IsNullOrEmpty(this.MinIncomingHealthServer))
				{
					xmlWriter.WriteAttributeString("MinInHealthServer", this.MinIncomingHealthServer.Split(new char[]
					{
						'.'
					})[0]);
				}
				xmlWriter.WriteAttributeString("OutHealth", this.OutgoingHealth.ToString());
				if (!string.IsNullOrEmpty(this.MinOutgoingHealthServer))
				{
					xmlWriter.WriteAttributeString("MinOutHealthServer", this.MinOutgoingHealthServer.Split(new char[]
					{
						'.'
					})[0]);
				}
				foreach (ADServerMetrics adserverMetrics in this.AllServerMetrics)
				{
					if (adserverMetrics.IsSuitable)
					{
						xmlWriter.WriteStartElement("Server");
						xmlWriter.WriteAttributeString("Name", adserverMetrics.DnsHostName.Split(new char[]
						{
							'.'
						})[0]);
						xmlWriter.WriteAttributeString("UpdateTime", adserverMetrics.UpdateTime.ToString());
						xmlWriter.WriteAttributeString("HighestUsn", adserverMetrics.HighestUsn.ToString());
						xmlWriter.WriteAttributeString("InjectRate", adserverMetrics.InjectionRate.ToString());
						xmlWriter.WriteAttributeString("InDebt", adserverMetrics.IncomingDebt.ToString());
						xmlWriter.WriteAttributeString("OutDebt", adserverMetrics.OutgoingDebt.ToString());
						xmlWriter.WriteAttributeString("InHealth", adserverMetrics.IncomingHealth.ToString());
						xmlWriter.WriteAttributeString("OutHealth", adserverMetrics.OutgoingHealth.ToString());
						if (adserverMetrics.ConfigReplicationMetrics != null && adserverMetrics.ConfigReplicationMetrics.Count > 0)
						{
							xmlWriter.WriteStartElement("Config");
							foreach (ADReplicationLinkMetrics adreplicationLinkMetrics in adserverMetrics.ConfigReplicationMetrics)
							{
								xmlWriter.WriteStartElement("Link");
								xmlWriter.WriteAttributeString("Neighbor", adreplicationLinkMetrics.NeighborDnsHostName.Split(new char[]
								{
									'.'
								})[0]);
								xmlWriter.WriteAttributeString("Usn", adreplicationLinkMetrics.UpToDatenessUsn.ToString());
								xmlWriter.WriteAttributeString("Debt", adreplicationLinkMetrics.Debt.ToString());
								xmlWriter.WriteEndElement();
							}
							xmlWriter.WriteEndElement();
						}
						if (adserverMetrics.DomainReplicationMetrics != null && adserverMetrics.DomainReplicationMetrics.Count > 0)
						{
							xmlWriter.WriteStartElement("Domain");
							foreach (ADReplicationLinkMetrics adreplicationLinkMetrics2 in adserverMetrics.DomainReplicationMetrics)
							{
								xmlWriter.WriteStartElement("Link");
								xmlWriter.WriteAttributeString("Neighbor", adreplicationLinkMetrics2.NeighborDnsHostName.Split(new char[]
								{
									'.'
								})[0]);
								xmlWriter.WriteAttributeString("Usn", adreplicationLinkMetrics2.UpToDatenessUsn.ToString());
								xmlWriter.WriteAttributeString("Debt", adreplicationLinkMetrics2.Debt.ToString());
								xmlWriter.WriteEndElement();
							}
							xmlWriter.WriteEndElement();
						}
						xmlWriter.WriteEndElement();
					}
				}
				xmlWriter.WriteEndElement();
			}
			return stringBuilder.ToString();
		}

		private static readonly TimeSpan topologyRediscoveryInterval = TimeSpan.FromMinutes(15.0);

		private Dictionary<string, ADServerMetrics> allADServerMetrics = new Dictionary<string, ADServerMetrics>();

		private ExDateTime? updateTime = null;

		private ExDateTime lastTopologyUpdateTime = ExDateTime.MinValue;

		private List<ADServer> serversList;

		private bool rediscoverTopology = true;

		private int topologyVersion;

		private int pooledDiscoveryCount;

		private readonly string forestFqdn;

		private sealed class AsyncState
		{
			public AsyncState(ADServerMetrics dcMetrics, ADMetrics adMetrics)
			{
				this.ServerMetrics = dcMetrics;
				this.ADMetrics = adMetrics;
			}

			public ADServerMetrics ServerMetrics { get; private set; }

			public ADMetrics ADMetrics { get; private set; }
		}
	}
}
