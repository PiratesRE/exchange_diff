using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.ActiveManager
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class ActiveManagerClientPerfmonInstance : PerformanceCounterInstance
	{
		internal ActiveManagerClientPerfmonInstance(string instanceName, ActiveManagerClientPerfmonInstance autoUpdateTotalInstance) : base(instanceName, "MSExchange Active Manager Client")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.GetServerForDatabaseClientCalls = new ExPerformanceCounter(base.CategoryName, "Client-side Calls", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseClientCalls);
				this.GetServerForDatabaseClientCallsPerSec = new ExPerformanceCounter(base.CategoryName, "Client-side Calls/Sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseClientCallsPerSec);
				this.GetServerForDatabaseClientCacheHits = new ExPerformanceCounter(base.CategoryName, "Client-side Cache Hits", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseClientCacheHits);
				this.GetServerForDatabaseClientCacheMisses = new ExPerformanceCounter(base.CategoryName, "Client-side Cache Misses", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseClientCacheMisses);
				this.GetServerForDatabaseClientCallsWithReadThrough = new ExPerformanceCounter(base.CategoryName, "Client-side Calls ReadThrough", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseClientCallsWithReadThrough);
				this.GetServerForDatabaseClientRpcCalls = new ExPerformanceCounter(base.CategoryName, "Client-side RPC Calls", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseClientRpcCalls);
				this.GetServerForDatabaseClientUniqueDatabases = new ExPerformanceCounter(base.CategoryName, "Unique databases queried", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseClientUniqueDatabases);
				this.GetServerForDatabaseClientUniqueServers = new ExPerformanceCounter(base.CategoryName, "Unique servers queried", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseClientUniqueServers);
				this.GetServerForDatabaseClientLocationCacheEntries = new ExPerformanceCounter(base.CategoryName, "Location cache entries", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseClientLocationCacheEntries);
				this.CacheUpdateTimeInSec = new ExPerformanceCounter(base.CategoryName, "Location cache update time", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.CacheUpdateTimeInSec);
				this.GetServerForDatabaseClientServerInformationCacheHits = new ExPerformanceCounter(base.CategoryName, "Server-Information Cache Hits", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseClientServerInformationCacheHits);
				this.GetServerForDatabaseClientServerInformationCacheMisses = new ExPerformanceCounter(base.CategoryName, "Server-Information Cache Misses", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseClientServerInformationCacheMisses);
				this.GetServerForDatabaseClientServerInformationCacheEntries = new ExPerformanceCounter(base.CategoryName, "Server-Information Cache Entries", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseClientServerInformationCacheEntries);
				this.CalculatePreferredHomeServerCalls = new ExPerformanceCounter(base.CategoryName, "CalculatePreferredHomeServer Calls", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.CalculatePreferredHomeServerCalls);
				this.CalculatePreferredHomeServerCallsPerSec = new ExPerformanceCounter(base.CategoryName, "CalculatePreferredHomeServer Calls/Sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.CalculatePreferredHomeServerCallsPerSec);
				this.CalculatePreferredHomeServerRedirects = new ExPerformanceCounter(base.CategoryName, "CalculatePreferredHomeServer Redirects", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.CalculatePreferredHomeServerRedirects);
				this.CalculatePreferredHomeServerRedirectsPerSec = new ExPerformanceCounter(base.CategoryName, "CalculatePreferredHomeServer Redirects/Sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.CalculatePreferredHomeServerRedirectsPerSec);
				this.GetServerForDatabaseWCFLatency = new ExPerformanceCounter(base.CategoryName, "Average WCF calls latency", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseWCFLatency);
				this.GetServerForDatabaseWCFLatencyTimeBase = new ExPerformanceCounter(base.CategoryName, "Average WCF calls latency time base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseWCFLatencyTimeBase);
				this.GetServerForDatabaseWCFLocalLatency = new ExPerformanceCounter(base.CategoryName, "Average WCF calls to the local server latency", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseWCFLocalLatency);
				this.GetServerForDatabaseWCFLocalLatencyTimeBase = new ExPerformanceCounter(base.CategoryName, "Average WCF calls to the local server time base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseWCFLocalLatencyTimeBase);
				this.GetServerForDatabaseWCFLocalDomainLocalSiteLatency = new ExPerformanceCounter(base.CategoryName, "Average WCF calls to a remote server in the local site latency", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseWCFLocalDomainLocalSiteLatency);
				this.GetServerForDatabaseWCFLocalDomainLocalSiteLatencyTimeBase = new ExPerformanceCounter(base.CategoryName, "Average WCF calls to a remote server in the local site latency time base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseWCFLocalDomainLocalSiteLatencyTimeBase);
				this.GetServerForDatabaseWCFLocalDomainRemoteSiteLatency = new ExPerformanceCounter(base.CategoryName, "Average WCF calls to a remote server in the remote site latency", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseWCFLocalDomainRemoteSiteLatency);
				this.GetServerForDatabaseWCFLocalDomainRemoteSiteLatencyTimeBase = new ExPerformanceCounter(base.CategoryName, "Average WCF calls to a remote server in the remote site latency time base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseWCFLocalDomainRemoteSiteLatencyTimeBase);
				this.GetServerForDatabaseWCFRemoteDomainLatency = new ExPerformanceCounter(base.CategoryName, "Average WCF calls to a remote server in a remote domain latency", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseWCFRemoteDomainLatency);
				this.GetServerForDatabaseWCFRemoteDomainLatencyTimeBase = new ExPerformanceCounter(base.CategoryName, "Average WCF calls to a remote server in a remote domain latency time base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseWCFRemoteDomainLatencyTimeBase);
				this.GetServerForDatabaseWCFCalls = new ExPerformanceCounter(base.CategoryName, "WCF calls", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseWCFCalls);
				this.GetServerForDatabaseWCFCallsPerSec = new ExPerformanceCounter(base.CategoryName, "WCF Calls/Sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseWCFCallsPerSec);
				this.GetServerForDatabaseWCFLocalCalls = new ExPerformanceCounter(base.CategoryName, "WCF calls to local server", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseWCFLocalCalls);
				this.GetServerForDatabaseWCFLocalCallsPerSec = new ExPerformanceCounter(base.CategoryName, "WCF Calls/Sec to local server", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseWCFLocalCallsPerSec);
				this.GetServerForDatabaseWCFLocalDomainLocalSiteCalls = new ExPerformanceCounter(base.CategoryName, "WCF calls to a remote server in local domain and local site", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseWCFLocalDomainLocalSiteCalls);
				this.GetServerForDatabaseWCFLocalDomainLocalSiteCallsPerSec = new ExPerformanceCounter(base.CategoryName, "WCF Calls/Sec to a remote server in local domain and local site", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseWCFLocalDomainLocalSiteCallsPerSec);
				this.GetServerForDatabaseWCFLocalDomainRemoteSiteCalls = new ExPerformanceCounter(base.CategoryName, "WCF calls to a remote server in remote site", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseWCFLocalDomainRemoteSiteCalls);
				this.GetServerForDatabaseWCFLocalDomainRemoteSiteCallsPerSec = new ExPerformanceCounter(base.CategoryName, "WCF Calls/Sec to a remote server in remote site", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseWCFLocalDomainRemoteSiteCallsPerSec);
				this.GetServerForDatabaseWCFRemoteDomainCalls = new ExPerformanceCounter(base.CategoryName, "WCF calls to a remote server in a remote domain", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseWCFRemoteDomainCalls);
				this.GetServerForDatabaseWCFRemoteDomainCallsPerSec = new ExPerformanceCounter(base.CategoryName, "WCF Calls/Sec to a remote server in a remote domain", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseWCFRemoteDomainCallsPerSec);
				this.GetServerForDatabaseWCFErrors = new ExPerformanceCounter(base.CategoryName, "WCF calls returning an error", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseWCFErrors);
				this.GetServerForDatabaseWCFErrorsPerSec = new ExPerformanceCounter(base.CategoryName, "WCF Calls/Sec returning an error", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseWCFErrorsPerSec);
				this.GetServerForDatabaseWCFTimeouts = new ExPerformanceCounter(base.CategoryName, "WCF calls timing out", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseWCFTimeouts);
				this.GetServerForDatabaseWCFTimeoutsPerSec = new ExPerformanceCounter(base.CategoryName, "WCF Calls/Sec timing out", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseWCFTimeoutsPerSec);
				long num = this.GetServerForDatabaseClientCalls.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter in list)
					{
						exPerformanceCounter.Close();
					}
				}
			}
			this.counters = list.ToArray();
		}

		internal ActiveManagerClientPerfmonInstance(string instanceName) : base(instanceName, "MSExchange Active Manager Client")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.GetServerForDatabaseClientCalls = new ExPerformanceCounter(base.CategoryName, "Client-side Calls", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseClientCalls);
				this.GetServerForDatabaseClientCallsPerSec = new ExPerformanceCounter(base.CategoryName, "Client-side Calls/Sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseClientCallsPerSec);
				this.GetServerForDatabaseClientCacheHits = new ExPerformanceCounter(base.CategoryName, "Client-side Cache Hits", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseClientCacheHits);
				this.GetServerForDatabaseClientCacheMisses = new ExPerformanceCounter(base.CategoryName, "Client-side Cache Misses", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseClientCacheMisses);
				this.GetServerForDatabaseClientCallsWithReadThrough = new ExPerformanceCounter(base.CategoryName, "Client-side Calls ReadThrough", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseClientCallsWithReadThrough);
				this.GetServerForDatabaseClientRpcCalls = new ExPerformanceCounter(base.CategoryName, "Client-side RPC Calls", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseClientRpcCalls);
				this.GetServerForDatabaseClientUniqueDatabases = new ExPerformanceCounter(base.CategoryName, "Unique databases queried", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseClientUniqueDatabases);
				this.GetServerForDatabaseClientUniqueServers = new ExPerformanceCounter(base.CategoryName, "Unique servers queried", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseClientUniqueServers);
				this.GetServerForDatabaseClientLocationCacheEntries = new ExPerformanceCounter(base.CategoryName, "Location cache entries", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseClientLocationCacheEntries);
				this.CacheUpdateTimeInSec = new ExPerformanceCounter(base.CategoryName, "Location cache update time", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.CacheUpdateTimeInSec);
				this.GetServerForDatabaseClientServerInformationCacheHits = new ExPerformanceCounter(base.CategoryName, "Server-Information Cache Hits", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseClientServerInformationCacheHits);
				this.GetServerForDatabaseClientServerInformationCacheMisses = new ExPerformanceCounter(base.CategoryName, "Server-Information Cache Misses", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseClientServerInformationCacheMisses);
				this.GetServerForDatabaseClientServerInformationCacheEntries = new ExPerformanceCounter(base.CategoryName, "Server-Information Cache Entries", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseClientServerInformationCacheEntries);
				this.CalculatePreferredHomeServerCalls = new ExPerformanceCounter(base.CategoryName, "CalculatePreferredHomeServer Calls", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.CalculatePreferredHomeServerCalls);
				this.CalculatePreferredHomeServerCallsPerSec = new ExPerformanceCounter(base.CategoryName, "CalculatePreferredHomeServer Calls/Sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.CalculatePreferredHomeServerCallsPerSec);
				this.CalculatePreferredHomeServerRedirects = new ExPerformanceCounter(base.CategoryName, "CalculatePreferredHomeServer Redirects", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.CalculatePreferredHomeServerRedirects);
				this.CalculatePreferredHomeServerRedirectsPerSec = new ExPerformanceCounter(base.CategoryName, "CalculatePreferredHomeServer Redirects/Sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.CalculatePreferredHomeServerRedirectsPerSec);
				this.GetServerForDatabaseWCFLatency = new ExPerformanceCounter(base.CategoryName, "Average WCF calls latency", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseWCFLatency);
				this.GetServerForDatabaseWCFLatencyTimeBase = new ExPerformanceCounter(base.CategoryName, "Average WCF calls latency time base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseWCFLatencyTimeBase);
				this.GetServerForDatabaseWCFLocalLatency = new ExPerformanceCounter(base.CategoryName, "Average WCF calls to the local server latency", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseWCFLocalLatency);
				this.GetServerForDatabaseWCFLocalLatencyTimeBase = new ExPerformanceCounter(base.CategoryName, "Average WCF calls to the local server time base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseWCFLocalLatencyTimeBase);
				this.GetServerForDatabaseWCFLocalDomainLocalSiteLatency = new ExPerformanceCounter(base.CategoryName, "Average WCF calls to a remote server in the local site latency", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseWCFLocalDomainLocalSiteLatency);
				this.GetServerForDatabaseWCFLocalDomainLocalSiteLatencyTimeBase = new ExPerformanceCounter(base.CategoryName, "Average WCF calls to a remote server in the local site latency time base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseWCFLocalDomainLocalSiteLatencyTimeBase);
				this.GetServerForDatabaseWCFLocalDomainRemoteSiteLatency = new ExPerformanceCounter(base.CategoryName, "Average WCF calls to a remote server in the remote site latency", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseWCFLocalDomainRemoteSiteLatency);
				this.GetServerForDatabaseWCFLocalDomainRemoteSiteLatencyTimeBase = new ExPerformanceCounter(base.CategoryName, "Average WCF calls to a remote server in the remote site latency time base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseWCFLocalDomainRemoteSiteLatencyTimeBase);
				this.GetServerForDatabaseWCFRemoteDomainLatency = new ExPerformanceCounter(base.CategoryName, "Average WCF calls to a remote server in a remote domain latency", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseWCFRemoteDomainLatency);
				this.GetServerForDatabaseWCFRemoteDomainLatencyTimeBase = new ExPerformanceCounter(base.CategoryName, "Average WCF calls to a remote server in a remote domain latency time base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseWCFRemoteDomainLatencyTimeBase);
				this.GetServerForDatabaseWCFCalls = new ExPerformanceCounter(base.CategoryName, "WCF calls", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseWCFCalls);
				this.GetServerForDatabaseWCFCallsPerSec = new ExPerformanceCounter(base.CategoryName, "WCF Calls/Sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseWCFCallsPerSec);
				this.GetServerForDatabaseWCFLocalCalls = new ExPerformanceCounter(base.CategoryName, "WCF calls to local server", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseWCFLocalCalls);
				this.GetServerForDatabaseWCFLocalCallsPerSec = new ExPerformanceCounter(base.CategoryName, "WCF Calls/Sec to local server", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseWCFLocalCallsPerSec);
				this.GetServerForDatabaseWCFLocalDomainLocalSiteCalls = new ExPerformanceCounter(base.CategoryName, "WCF calls to a remote server in local domain and local site", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseWCFLocalDomainLocalSiteCalls);
				this.GetServerForDatabaseWCFLocalDomainLocalSiteCallsPerSec = new ExPerformanceCounter(base.CategoryName, "WCF Calls/Sec to a remote server in local domain and local site", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseWCFLocalDomainLocalSiteCallsPerSec);
				this.GetServerForDatabaseWCFLocalDomainRemoteSiteCalls = new ExPerformanceCounter(base.CategoryName, "WCF calls to a remote server in remote site", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseWCFLocalDomainRemoteSiteCalls);
				this.GetServerForDatabaseWCFLocalDomainRemoteSiteCallsPerSec = new ExPerformanceCounter(base.CategoryName, "WCF Calls/Sec to a remote server in remote site", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseWCFLocalDomainRemoteSiteCallsPerSec);
				this.GetServerForDatabaseWCFRemoteDomainCalls = new ExPerformanceCounter(base.CategoryName, "WCF calls to a remote server in a remote domain", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseWCFRemoteDomainCalls);
				this.GetServerForDatabaseWCFRemoteDomainCallsPerSec = new ExPerformanceCounter(base.CategoryName, "WCF Calls/Sec to a remote server in a remote domain", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseWCFRemoteDomainCallsPerSec);
				this.GetServerForDatabaseWCFErrors = new ExPerformanceCounter(base.CategoryName, "WCF calls returning an error", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseWCFErrors);
				this.GetServerForDatabaseWCFErrorsPerSec = new ExPerformanceCounter(base.CategoryName, "WCF Calls/Sec returning an error", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseWCFErrorsPerSec);
				this.GetServerForDatabaseWCFTimeouts = new ExPerformanceCounter(base.CategoryName, "WCF calls timing out", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseWCFTimeouts);
				this.GetServerForDatabaseWCFTimeoutsPerSec = new ExPerformanceCounter(base.CategoryName, "WCF Calls/Sec timing out", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GetServerForDatabaseWCFTimeoutsPerSec);
				long num = this.GetServerForDatabaseClientCalls.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter in list)
					{
						exPerformanceCounter.Close();
					}
				}
			}
			this.counters = list.ToArray();
		}

		public override void GetPerfCounterDiagnosticsInfo(XElement topElement)
		{
			XElement xelement = null;
			foreach (ExPerformanceCounter exPerformanceCounter in this.counters)
			{
				try
				{
					if (xelement == null)
					{
						xelement = new XElement(ExPerformanceCounter.GetEncodedName(exPerformanceCounter.InstanceName));
						topElement.Add(xelement);
					}
					xelement.Add(new XElement(ExPerformanceCounter.GetEncodedName(exPerformanceCounter.CounterName), exPerformanceCounter.NextValue()));
				}
				catch (XmlException ex)
				{
					XElement content = new XElement("Error", ex.Message);
					topElement.Add(content);
				}
			}
		}

		public readonly ExPerformanceCounter GetServerForDatabaseClientCalls;

		public readonly ExPerformanceCounter GetServerForDatabaseClientCallsPerSec;

		public readonly ExPerformanceCounter GetServerForDatabaseClientCacheHits;

		public readonly ExPerformanceCounter GetServerForDatabaseClientCacheMisses;

		public readonly ExPerformanceCounter GetServerForDatabaseClientCallsWithReadThrough;

		public readonly ExPerformanceCounter GetServerForDatabaseClientRpcCalls;

		public readonly ExPerformanceCounter GetServerForDatabaseClientUniqueDatabases;

		public readonly ExPerformanceCounter GetServerForDatabaseClientUniqueServers;

		public readonly ExPerformanceCounter GetServerForDatabaseClientLocationCacheEntries;

		public readonly ExPerformanceCounter CacheUpdateTimeInSec;

		public readonly ExPerformanceCounter GetServerForDatabaseClientServerInformationCacheHits;

		public readonly ExPerformanceCounter GetServerForDatabaseClientServerInformationCacheMisses;

		public readonly ExPerformanceCounter GetServerForDatabaseClientServerInformationCacheEntries;

		public readonly ExPerformanceCounter CalculatePreferredHomeServerCalls;

		public readonly ExPerformanceCounter CalculatePreferredHomeServerCallsPerSec;

		public readonly ExPerformanceCounter CalculatePreferredHomeServerRedirects;

		public readonly ExPerformanceCounter CalculatePreferredHomeServerRedirectsPerSec;

		public readonly ExPerformanceCounter GetServerForDatabaseWCFLatency;

		public readonly ExPerformanceCounter GetServerForDatabaseWCFLatencyTimeBase;

		public readonly ExPerformanceCounter GetServerForDatabaseWCFLocalLatency;

		public readonly ExPerformanceCounter GetServerForDatabaseWCFLocalLatencyTimeBase;

		public readonly ExPerformanceCounter GetServerForDatabaseWCFLocalDomainLocalSiteLatency;

		public readonly ExPerformanceCounter GetServerForDatabaseWCFLocalDomainLocalSiteLatencyTimeBase;

		public readonly ExPerformanceCounter GetServerForDatabaseWCFLocalDomainRemoteSiteLatency;

		public readonly ExPerformanceCounter GetServerForDatabaseWCFLocalDomainRemoteSiteLatencyTimeBase;

		public readonly ExPerformanceCounter GetServerForDatabaseWCFRemoteDomainLatency;

		public readonly ExPerformanceCounter GetServerForDatabaseWCFRemoteDomainLatencyTimeBase;

		public readonly ExPerformanceCounter GetServerForDatabaseWCFCalls;

		public readonly ExPerformanceCounter GetServerForDatabaseWCFCallsPerSec;

		public readonly ExPerformanceCounter GetServerForDatabaseWCFLocalCalls;

		public readonly ExPerformanceCounter GetServerForDatabaseWCFLocalCallsPerSec;

		public readonly ExPerformanceCounter GetServerForDatabaseWCFLocalDomainLocalSiteCalls;

		public readonly ExPerformanceCounter GetServerForDatabaseWCFLocalDomainLocalSiteCallsPerSec;

		public readonly ExPerformanceCounter GetServerForDatabaseWCFLocalDomainRemoteSiteCalls;

		public readonly ExPerformanceCounter GetServerForDatabaseWCFLocalDomainRemoteSiteCallsPerSec;

		public readonly ExPerformanceCounter GetServerForDatabaseWCFRemoteDomainCalls;

		public readonly ExPerformanceCounter GetServerForDatabaseWCFRemoteDomainCallsPerSec;

		public readonly ExPerformanceCounter GetServerForDatabaseWCFErrors;

		public readonly ExPerformanceCounter GetServerForDatabaseWCFErrorsPerSec;

		public readonly ExPerformanceCounter GetServerForDatabaseWCFTimeouts;

		public readonly ExPerformanceCounter GetServerForDatabaseWCFTimeoutsPerSec;
	}
}
